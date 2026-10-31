using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SlackNet.Events;
using SlackNet.SocketMode;
using SlackNet.WebApi;

namespace SlackNet.Tests;

public class ModelLintTests
{
    [Test]
    public void AllModelsHaveACatchAllForUnknownProperties()
    {
        ModelTypes
            .Where(t => !t.IsAssignableTo(typeof(SlackObject)) && !WithTheirOwnCatchAll.Any(t.IsAssignableTo))
            .ShouldBeEmpty("Models should derive from SlackObject, so that nothing Slack adds to them is lost");
    }

    [Test]
    public void UnknownPropertiesAreReadButNotWritten()
    {
        var settings = Default.JsonSettings().SerializerSettings;

        ModelTypes
            .Where(t => t.IsAssignableTo(typeof(SlackObject)))
            .Where(t =>
                {
                    var model = (SlackObject)JsonConvert.DeserializeObject(@"{ ""not_modelled_yet"": ""value"" }", t, settings);
                    return model?.ExtraProperties.ContainsKey("not_modelled_yet") != true
                        || JsonConvert.SerializeObject(model, settings).Contains("not_modelled_yet");
                })
            .ShouldBeEmpty("Unknown properties should end up in ExtraProperties, and not be written back when the model is sent to Slack");
    }

    // Everything Json.NET can fill in from a JSON object
    private static IEnumerable<Type> ModelTypes => typeof(SlackObject).Assembly
        .GetExportedTypes()
        .Where(t => t is { IsClass: true, IsAbstract: false, IsGenericTypeDefinition: false }
            && t.GetConstructor(Type.EmptyTypes) != null
            && t.GetProperties(BindingFlags.Public | BindingFlags.Instance).Any(p => p.SetMethod?.IsPublic == true && p.GetIndexParameters().Length == 0))
        .Except(NotModels);

    private static readonly Type[] NotModels =
        [
            typeof(SlackRequestContext), // Local context for the request being handled
            typeof(SocketModeConnectionOptions), // Local configuration
        ];

    // These have extension data of their own, which Json.NET fills instead of the one on SlackObject, so they don't derive from it
    private static readonly Type[] WithTheirOwnCatchAll =
        [
            typeof(Event), // Also written back, which it always has been
            typeof(MessageSearchResult), // Same as Event
            typeof(Icons), // Images are what's left over after the known properties
            typeof(ErrorResponse), // Info is what's left over after the known properties
            typeof(WebApiResponse), // Data is the whole response
        ];
}
