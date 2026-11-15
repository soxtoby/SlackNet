using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using EasyAssertions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace SlackNet.Tests;

/// <summary>
/// Compares the API against the args and output schemas in <see cref="SlackChangeDetectionTests.ApiSpecChanges"/>' snapshot.
/// </summary>
public partial class ApiDocumentationTests
{
    private static readonly IContractResolver ContractResolver = Default.JsonSettings().SerializerSettings.ContractResolver;

    /// <summary>
    /// Args, as "method arg", that SlackNet sends although Slack's arg schemas don't include them.
    /// </summary>
    private static readonly HashSet<string> UndocumentedArgExceptions =
        [
            "admin.barriers.list filter_by_usergroup_name", // Only documented in the method's description
            "admin.workflows.triggers.types.permissions.set org_ids", // Only documented in the method's description
            "admin.workflows.triggers.types.permissions.set team_ids", // Only documented in the method's description
            "chat.scheduleMessage icon_emoji", // Sent for all messages
            "chat.scheduleMessage icon_url", // Sent for all messages
            "chat.scheduleMessage username", // Sent for all messages
            "chat.unfurl metadata", // Only documented in the method's description
            "files.list cursor", // Undocumented, but already released
            "pins.remove file", // Only documented in the method's description
            "pins.remove file_comment", // Only documented in the method's description
        ];

    /// <summary>
    /// Response properties, as "Type.Property", whose requiredness deliberately differs from Slack's output schemas.
    /// </summary>
    private static readonly HashSet<string> RequirednessExceptions =
        [
            "AdminUserResponse.User", // Slack's schema doesn't mark any of admin.users.getExpiration's output as required
            "AgentView.ViewKey", // Absent for diff views
            "AppMcpServer.AuthType", // Null when Slack returns an auth type SlackNet doesn't know
            "AuthPolicyEntity.EntityType", // Null when Slack returns an entity type SlackNet doesn't know
            "ChannelResponse.Channel", // Slack's example admin.conversations.setTeams response has no channel
            "EnterpriseTeam.Id", // Slack's schema requires team_id, which isn't one of its properties, instead of id
            "ErrorDetail.Code", // Also used for SlackException.ErrorDetails, which aren't documented to have a code
            "OauthV2AccessResponse.ExpiresIn", // Also used for oauth.v2.access, which only returns it with token rotation
            "OauthV2AccessResponse.RefreshToken", // Also used for oauth.v2.access, which only returns it with token rotation
            "Skill.AutoEnabled", // Only returned by admin.slackbot.skills.list
            "Skill.Permalink", // Not returned by admin.slackbot.skills.list
        ];

    /// <summary>
    /// Response properties, as "Type.Property" or a whole "Type", that Slack's output schemas don't include.
    /// </summary>
    private static readonly HashSet<string> UndocumentedPropertyExceptions =
        [
            "BotInfo.TeamId", // Included in bot_added events
            "ConversationPermission.UsergroupIds", // Not documented; needs checking against a real response
            "ConversationTeamsResponse.TeamIds", // In Slack's example response, but not its schema
            "ErrorDetail.Constraint", // Also used for SlackException.ErrorDetails
            "File", // Slack's inline file schemas only list some of a file's properties
            "FileComment.Channel", // Not in Slack's inline file comment schemas
            "FileComment.Reactions", // Not in Slack's inline file comment schemas
            "Icons.ImageDefault", // Also used for team icons
            "Reaction.Users", // In Slack's example response, but not its schema
            "StartResponse", // rtm.start's schema only lists some of its properties
            "Team.IsMigrating", // Only included in external org migration events
            "TeamPreferences.AllowMessageDeletion", // Not in Slack's schema, but already released
        ];

    /// <summary>
    /// Properties of Slack's general response format, which many output schemas leave out.
    /// </summary>
    private static readonly HashSet<string> GeneralResponsePropertyNames = ["ok", "warning", "response_metadata"];

    private static readonly Dictionary<string, JObject> ArgSchemas = SpecSchemas("arg");
    private static readonly Dictionary<string, JObject> OutputSchemas = SpecSchemas("output");

    [TestCaseSource(nameof(DocumentedMethodArgs))]
    public void ArgsAreDocumented(string slackMethod, ICollection<string> argNames) =>
        argNames
            .Except(((JObject)ArgSchemas[slackMethod]["properties"]!).Properties().Select(p => p.Name))
            .Where(argName => !UndocumentedArgExceptions.Contains($"{slackMethod} {argName}"))
            .ShouldBeEmpty();

    [TestCaseSource(nameof(DocumentedResponses))]
    public void RequirednessMatches(string slackMethod, Type responseType) =>
        SchemaObjects(responseType, OutputSchemas[slackMethod], slackMethod)
            .Where(o => HasNullableAnnotations(o.Type))
            .SelectMany(RequirednessMismatches)
            .ShouldBeEmpty();

    [TestCaseSource(nameof(DocumentedTypes))]
    public void PropertiesAreDocumented(Type type, ICollection<string> documentedPropertyNames) =>
        ((JsonObjectContract)ContractResolver.ResolveContract(type)).Properties
        .Where(p => p is { Ignored: false, Writable: true }
            && !GeneralResponsePropertyNames.Contains(p.PropertyName)
            && !documentedPropertyNames.Contains(p.PropertyName))
        .Select(p => $"{type.Name}.{p.UnderlyingName}")
        .Where(memberName => !UndocumentedPropertyExceptions.Contains(memberName) && !UndocumentedPropertyExceptions.Contains(type.Name))
        .ShouldBeEmpty();

    private static IEnumerable<TestCaseData> DocumentedMethodArgs() =>
        ApiLintTests.ImplementedSlackMethodCalls()
            .Where(m => ArgSchemas.ContainsKey(m.SlackMethod))
            .GroupBy(m => m.SlackMethod, m => m.ArgNames)
            .Select(m => new TestCaseData(m.Key, m.SelectMany(argNames => argNames).Distinct().ToList())
                .SetArgDisplayNames(m.Key));

    private static IEnumerable<TestCaseData> DocumentedResponses() =>
        DocumentedResponseTypes()
            .Select(m => new TestCaseData(m.SlackMethod, m.ResponseType)
                .SetArgDisplayNames(m.SlackMethod, m.ResponseType.Name));

    /// <summary>
    /// Each type with a documented schema, and the properties documented by any of its schemas, since types shared between methods can have different documented properties for each.
    /// </summary>
    private static IEnumerable<TestCaseData> DocumentedTypes() =>
        DocumentedResponseTypes()
            .SelectMany(m => SchemaObjects(m.ResponseType, OutputSchemas[m.SlackMethod], m.SlackMethod))
            .GroupBy(o => o.Type)
            .Select(type => new TestCaseData(type.Key, type.SelectMany(o => o.SchemaProperties.Properties().Select(p => p.Name)).ToHashSet())
                .SetArgDisplayNames(type.Key.Name));

    private static IEnumerable<SlackMethodCall> DocumentedResponseTypes() =>
        ApiLintTests.ImplementedSlackMethodCalls()
            .Where(m => m.ResponseType is not null && OutputSchemas.ContainsKey(m.SlackMethod))
            .DistinctBy(m => (m.SlackMethod, m.ResponseType));

    /// <summary>
    /// Rebuilds each method's arg or output schema from the snapshot's one line per arg or output property.
    /// </summary>
    private static Dictionary<string, JObject> SpecSchemas(string kind) =>
        System.IO.File.ReadLines(SlackChangeDetectionTests.ApiSpecSnapshotFile)
            .Select(line => SpecPropertyLine().Match(line))
            .Where(match => match.Success && match.Groups["kind"].Value == kind)
            .GroupBy(match => match.Groups["method"].Value)
            .ToDictionary(method => method.Key,
                method => new JObject
                    {
                        ["properties"] = new JObject(method.Select(match => new JProperty(match.Groups["property"].Value, JToken.Parse(match.Groups["schema"].Value)))),
                        ["required"] = new JArray(method.Where(match => match.Groups["required"].Success).Select(match => match.Groups["property"].Value))
                    });

    [GeneratedRegex(@"^(?<method>\S+) (?<kind>arg|output) (?<property>[^\s{]+)(?<required> \(required\))? (?<schema>.*)$")]
    private static partial Regex SpecPropertyLine();

    /// <summary>
    /// The type and its schema, followed by each type within it that has a schema of its own.
    /// </summary>
    private static IEnumerable<SchemaObject> SchemaObjects(Type type, JObject schema, string path)
    {
        if (schema?["properties"] is not JObject || ContractResolver.ResolveContract(type) is not JsonObjectContract contract)
            return [];

        var schemaObject = new SchemaObject(type, contract, schema, path);
        return schemaObject.DocumentedProperties()
            .SelectMany(p => NestedSchemaObjects(p.Property.PropertyType, p.SchemaProperty.Value, $"{path}.{p.SchemaProperty.Name}"))
            .Prepend(schemaObject);
    }

    private static IEnumerable<SchemaObject> NestedSchemaObjects(Type propertyType, JToken propertySchema, string propertyPath)
    {
        var nonNullSchema = NonNullSchema(propertySchema);
        return ElementType(propertyType) is { } elementType && nonNullSchema?["items"] is { } itemSchema
            ? SchemaObjects(elementType, NonNullSchema(itemSchema), propertyPath + "[]")
            : SchemaObjects(Nullable.GetUnderlyingType(propertyType) ?? propertyType, nonNullSchema, propertyPath);
    }

    private static JObject NonNullSchema(JToken schema) =>
        Alternatives(schema).FirstOrDefault(s => s["type"] is not JValue { Value: "null" });

    private static IEnumerable<JObject> Alternatives(JToken schema) =>
        ((schema as JObject)?["anyOf"] ?? (schema as JObject)?["oneOf"] ?? new JArray(schema)).OfType<JObject>();

    private static Type ElementType(Type type) =>
        type == typeof(string) || !typeof(IEnumerable).IsAssignableFrom(type) || typeof(IDictionary).IsAssignableFrom(type)
            ? null
            : type.IsArray
                ? type.GetElementType()
                : type.GetInterfaces().Append(type)
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    ?.GetGenericArguments()[0];

    /// <summary>
    /// Types compiled with nullable reference types disabled can't say whether their properties are nullable.
    /// </summary>
    private static bool HasNullableAnnotations(Type type) =>
        type.GetCustomAttributes().Any(a => a.GetType().Name == "NullableContextAttribute")
        || type.GetProperties().Any(p => p.CustomAttributes.Any(a => a.AttributeType.Name == "NullableAttribute"));

    /// <summary>
    /// Properties Slack always returns shouldn't be nullable, and properties it might not return shouldn't be <c>required</c>.
    /// </summary>
    private static IEnumerable<string> RequirednessMismatches(SchemaObject schemaObject)
    {
        var requiredPropertyNames = schemaObject.Schema["required"]?.Values<string>().ToHashSet() ?? [];
        return schemaObject.DocumentedProperties()
            .SelectMany(p => RequirednessMismatch(
                schemaObject.Type,
                p.Property,
                !requiredPropertyNames.Contains(p.SchemaProperty.Name) || AllowsNull(p.SchemaProperty.Value),
                $"{schemaObject.Path}.{p.SchemaProperty.Name}"));
    }

    private static IEnumerable<string> RequirednessMismatch(Type type, PropertyInfo property, bool optional, string propertyPath)
    {
        var memberName = $"{type.Name}.{property.Name}";
        return RequirednessExceptions.Contains(memberName) ? []
            : optional && property.IsDefined(typeof(RequiredMemberAttribute)) ? [$"{propertyPath} is optional, but {memberName} is required"]
            : !optional && IsNullable(property) ? [$"{propertyPath} is required, but {memberName} is nullable"]
            : [];
    }

    private static bool AllowsNull(JToken schema) =>
        Alternatives(schema).Any(s => s["type"] is JValue { Value: "null" } || s["type"] is JArray types && types.Values<string>().Contains("null"));

    private static bool IsNullable(PropertyInfo property) =>
        property.PropertyType.IsValueType
            ? Nullable.GetUnderlyingType(property.PropertyType) is not null
            : new NullabilityInfoContext().Create(property).ReadState == NullabilityState.Nullable;

    private record SchemaObject(Type Type, JsonObjectContract Contract, JObject Schema, string Path)
    {
        public JObject SchemaProperties => (JObject)Schema["properties"];

        /// <summary>
        /// Each documented property, paired with the property it's deserialized into.
        /// </summary>
        public IEnumerable<(JProperty SchemaProperty, PropertyInfo Property)> DocumentedProperties() =>
            SchemaProperties.Properties()
                .Select(schemaProperty => (schemaProperty, property: MatchingProperty(schemaProperty.Name)))
                .Where(p => p.property is not null);

        private PropertyInfo MatchingProperty(string jsonPropertyName) =>
            Contract.Properties.SingleOrDefault(p => p.PropertyName == jsonPropertyName && !p.Ignored) is { } jsonProperty
                ? Type.GetProperty(jsonProperty.UnderlyingName!)
                : null;
    }
}