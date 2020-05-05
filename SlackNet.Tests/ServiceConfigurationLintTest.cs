using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using EasyAssertions;
using NSubstitute.Core;
using NUnit.Framework;
using SlackNet.AspNetCore;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Interaction;

namespace SlackNet.Tests
{
    public class ServiceConfigurationLintTest
    {
        [Test]
        public void StandardRegistrationMethods()
        {
            var publicMethods = typeof(SlackServiceConfiguration)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName)
                .ToList();

            // Non-registrations
            ExpectMethod(publicMethods, nameof(SlackServiceConfiguration.UseApiToken), new Type[0], new[] { typeof(string) });

            // Standard registrations
            ExpectRegistrationTriplet(publicMethods, nameof(SlackServiceConfiguration.RegisterEventHandler), typeof(IEventHandler), new Type[0], new Type[0]);
            ExpectRegistrationTriplet(publicMethods, nameof(SlackServiceConfiguration.RegisterEventHandler), typeof(IEventHandler<Event>), new[] { typeof(Event) }, new Type[0]);

            ExpectRegistrationTriplet(publicMethods, nameof(SlackServiceConfiguration.RegisterBlockActionHandler), typeof(IBlockActionHandler), new Type[0], new Type[0]);
            ExpectRegistrationTriplet(publicMethods, nameof(SlackServiceConfiguration.RegisterBlockActionHandler), typeof(IBlockActionHandler<BlockAction>), new[] { typeof(BlockAction) }, new Type[0]);
            ExpectRegistrationTriplet(publicMethods, nameof(SlackServiceConfiguration.RegisterBlockActionHandler), typeof(IBlockActionHandler<BlockAction>), new[] { typeof(BlockAction) }, new[] { typeof(string)});

            ExpectRegistrationTriplet(publicMethods, nameof(SlackServiceConfiguration.RegisterMessageShortcutHandler), typeof(IMessageShortcutHandler), new Type[0], new Type[0]);
            ExpectRegistrationTriplet(publicMethods, nameof(SlackServiceConfiguration.RegisterMessageShortcutHandler), typeof(IMessageShortcutHandler), new Type[0], new[] { typeof(string) });

            ExpectKeyedRegistrationTriplet(publicMethods, nameof(SlackServiceConfiguration.RegisterBlockOptionProvider), typeof(IBlockOptionProvider));
            ExpectKeyedRegistrationTriplet(publicMethods, nameof(SlackServiceConfiguration.RegisterViewSubmissionHandler), typeof(IViewSubmissionHandler));
            ExpectKeyedRegistrationTriplet(publicMethods, nameof(SlackServiceConfiguration.RegisterSlashCommandHandler), typeof(ISlashCommandHandler));

            // Legacy interaction
            ExpectMethod(publicMethods, nameof(SlackServiceConfiguration.RegisterInteractiveMessageHandler), new[] { typeof(IInteractiveMessageHandler) }, new[] { typeof(string) });
            ExpectMethod(publicMethods, nameof(SlackServiceConfiguration.RegisterOptionProvider), new[] { typeof(IOptionProvider) }, new[] { typeof(string) });
            ExpectMethod(publicMethods, nameof(SlackServiceConfiguration.RegisterDialogSubmissionHandler), new[] { typeof(IDialogSubmissionHandler) }, new[] { typeof(string) });

            RemainingMethods(publicMethods)
                .ShouldBeEmpty("Unexpected public method(s)");
        }

        private static void ExpectKeyedRegistrationTriplet(List<MethodInfo> publicMethods, string name, Type handlerType)
        {
            ExpectRegistrationTriplet(publicMethods, name, handlerType, new Type[0], new[] { typeof(string) });
        }

        private static void ExpectRegistrationTriplet(List<MethodInfo> publicMethods, string name, Type handlerType, Type[] genericArgs, Type[] parameters)
        {
            ExpectMethod(publicMethods, name, genericArgs.Concat(new[] { handlerType }), parameters);
            ExpectMethod(publicMethods, name, genericArgs, parameters.Concat(new[] { handlerType }));
            ExpectMethod(publicMethods, name, genericArgs, parameters.Concat(new[] { typeof(Func<,>).MakeGenericType(typeof(IServiceProvider), handlerType) }));
        }

        private static void ExpectMethod(List<MethodInfo> methods, string name, IEnumerable<Type> genericParameters, IEnumerable<Type> methodParameters)
        {
            FindMethod(methods, name, genericParameters, methodParameters)
                .ShouldBe(1, $"Missing {MethodSignature(name, genericParameters, methodParameters, typeof(SlackServiceConfiguration))}");
        }

        private static int FindMethod(List<MethodInfo> methods, string name, IEnumerable<Type> genericParameters, IEnumerable<Type> methodParameters) =>
            methods.RemoveAll(m => m.Name == name
                && m.GetGenericArguments().SequenceEqual(genericParameters, GenericTypeComparer.Instance)
                && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(methodParameters, GenericTypeComparer.Instance));

        private static IEnumerable<string> RemainingMethods(IEnumerable<MethodInfo> publicMethods)
        {
            return publicMethods
                .Select(m => MethodSignature(m.Name, m.GetGenericArguments(), m.GetParameters().Select(p => p.ParameterType), m.ReturnType));
        }

        private static string MethodSignature(string name, IEnumerable<Type> genericParameters, IEnumerable<Type> methodParameters, Type returnType) =>
            $"{name}{GenericSignature(genericParameters)}({methodParameters.Select(p => p.Name).Join(", ")}) => {returnType.Name} ";

        private static string GenericSignature(IEnumerable<Type> genericParameters) =>
            genericParameters.Any()
                ? $"<{genericParameters.Select(p => p.Name).Join(", ")}>"
                : string.Empty;

        class GenericTypeComparer : IEqualityComparer<Type>
        {
            public static readonly GenericTypeComparer Instance = new GenericTypeComparer();

            public bool Equals(Type genericArgumentType, Type typeConstraint)
            {
                if (genericArgumentType == null)
                    return false;

                if (genericArgumentType == typeConstraint)
                    return true;

                if (Equals(genericArgumentType.BaseType, typeConstraint))
                    return true;

                if (genericArgumentType.GetInterfaces().Any(i => Equals(i, typeConstraint)))
                    return true;

                if (genericArgumentType.IsGenericType && typeConstraint.IsGenericType)
                {
                    return genericArgumentType.GetGenericTypeDefinition() == typeConstraint.GetGenericTypeDefinition()
                        && genericArgumentType.GetGenericArguments().SequenceEqual(typeConstraint.GetGenericArguments(), Instance);
                }

                return false;
            }

            public int GetHashCode(Type obj) => throw new NotImplementedException();
        }
    }
}