using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyAssertions;
using NSubstitute.Core;
using NUnit.Framework;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;
using SSC = SlackNet.AspNetCore.SlackServiceConfiguration;

namespace SlackNet.Tests
{
    public class ServiceConfigurationLintTest
    {
        [Test]
        public void StandardRegistrationMethods()
        {
            var publicMethods = typeof(SSC)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName)
                .ToList();

            // Non-registrations
            ExpectMethod(publicMethods, nameof(SSC.UseApiToken), new Type[0], new[] { typeof(string) });

            // Standard registrations
            ExpectReplaceMethod(publicMethods, nameof(SSC.ReplaceEventHandling), typeof(IEventHandler));
            ExpectRegistrationTriplet(publicMethods, nameof(SSC.RegisterEventHandler), typeof(IEventHandler), new Type[0], new Type[0]);
            ExpectRegistrationTriplet(publicMethods, nameof(SSC.RegisterEventHandler), typeof(IEventHandler<Event>), new[] { typeof(Event) }, new Type[0]);

            ExpectReplaceMethod(publicMethods, nameof(SSC.ReplaceBlockActionHandling), typeof(IBlockActionHandler));
            ExpectRegistrationTriplet(publicMethods, nameof(SSC.RegisterBlockActionHandler), typeof(IBlockActionHandler), new Type[0], new Type[0]);
            ExpectRegistrationTriplet(publicMethods, nameof(SSC.RegisterBlockActionHandler), typeof(IBlockActionHandler<BlockAction>), new[] { typeof(BlockAction) }, new Type[0]);
            ExpectRegistrationTriplet(publicMethods, nameof(SSC.RegisterBlockActionHandler), typeof(IBlockActionHandler<BlockAction>), new[] { typeof(BlockAction) }, new[] { typeof(string)});

            ExpectReplaceMethod(publicMethods, nameof(SSC.ReplaceMessageShortcutHandling), typeof(IMessageShortcutHandler));
            ExpectRegistrationTriplet(publicMethods, nameof(SSC.RegisterMessageShortcutHandler), typeof(IMessageShortcutHandler), new Type[0], new Type[0]);
            ExpectRegistrationTriplet(publicMethods, nameof(SSC.RegisterMessageShortcutHandler), typeof(IMessageShortcutHandler), new Type[0], new[] { typeof(string) });

            ExpectReplaceMethod(publicMethods, nameof(SSC.ReplaceGlobalShortcutHandling), typeof(IGlobalShortcutHandler));
            ExpectRegistrationTriplet(publicMethods, nameof(SSC.RegisterGlobalShortcutHandler), typeof(IGlobalShortcutHandler), new Type[0], new Type[0]);
            ExpectRegistrationTriplet(publicMethods, nameof(SSC.RegisterGlobalShortcutHandler), typeof(IGlobalShortcutHandler), new Type[0], new[] { typeof(string) });

            ExpectReplaceAndKeyedRegistrationTriplet(publicMethods, nameof(SSC.ReplaceBlockOptionProviding), nameof(SSC.RegisterBlockOptionProvider), typeof(IBlockOptionProvider));
            ExpectReplaceAndKeyedRegistrationTriplet(publicMethods, nameof(SSC.ReplaceViewSubmissionHandling), nameof(SSC.RegisterViewSubmissionHandler), typeof(IViewSubmissionHandler));
            ExpectReplaceAndKeyedRegistrationTriplet(publicMethods, nameof(SSC.ReplaceSlashCommandHandling), nameof(SSC.RegisterSlashCommandHandler), typeof(ISlashCommandHandler));

            // Legacy interaction
            ExpectReplaceMethod(publicMethods, nameof(SSC.ReplaceLegacyInteractiveMessageHandling), typeof(IInteractiveMessageHandler));
            ExpectMethod(publicMethods, nameof(SSC.RegisterInteractiveMessageHandler), new[] { typeof(IInteractiveMessageHandler) }, new[] { typeof(string) });

            ExpectReplaceMethod(publicMethods, nameof(SSC.ReplaceLegacyOptionProviding), typeof(IOptionProvider));
            ExpectMethod(publicMethods, nameof(SSC.RegisterOptionProvider), new[] { typeof(IOptionProvider) }, new[] { typeof(string) });

            ExpectReplaceMethod(publicMethods, nameof(SSC.ReplaceLegacyDialogSubmissionHandling), typeof(IDialogSubmissionHandler));
            ExpectMethod(publicMethods, nameof(SSC.RegisterDialogSubmissionHandler), new[] { typeof(IDialogSubmissionHandler) }, new[] { typeof(string) });
            
            // Backwards compatibility
            ExpectRegistrationTriplet(publicMethods, nameof(SSC.RegisterMessageActionHandler), typeof(IMessageActionHandler), new Type[0], new Type[0]);
            ExpectMethod(publicMethods, nameof(SSC.RegisterMessageActionHandler), new[] { typeof(IMessageActionHandler) }, new[] { typeof(string) });

            // Experimental async API
            ExpectReplaceMethod(publicMethods, nameof(SSC.ReplaceAsyncBlockActionHandling), typeof(IAsyncBlockActionHandler));
            ExpectRegistrationTriplet(publicMethods, nameof(SSC.RegisterAsyncBlockActionHandler), typeof(IAsyncBlockActionHandler), new Type[0], new Type[0]);
            ExpectRegistrationTriplet(publicMethods, nameof(SSC.RegisterAsyncBlockActionHandler), typeof(IAsyncBlockActionHandler<BlockAction>), new[] { typeof(BlockAction) }, new Type[0]);
            ExpectRegistrationTriplet(publicMethods, nameof(SSC.RegisterAsyncBlockActionHandler), typeof(IAsyncBlockActionHandler<BlockAction>), new[] { typeof(BlockAction) }, new[] { typeof(string) });

            ExpectReplaceMethod(publicMethods, nameof(SSC.ReplaceAsyncMessageShortcutHandling), typeof(IAsyncMessageShortcutHandler));
            ExpectRegistrationTriplet(publicMethods, nameof(SSC.RegisterAsyncMessageShortcutHandler), typeof(IAsyncMessageShortcutHandler), new Type[0], new Type[0]);
            ExpectRegistrationTriplet(publicMethods, nameof(SSC.RegisterAsyncMessageShortcutHandler), typeof(IAsyncMessageShortcutHandler), new Type[0], new[] { typeof(string) });

            ExpectReplaceMethod(publicMethods, nameof(SSC.ReplaceAsyncGlobalShortcutHandling), typeof(IAsyncGlobalShortcutHandler));
            ExpectRegistrationTriplet(publicMethods, nameof(SSC.RegisterAsyncGlobalShortcutHandler), typeof(IAsyncGlobalShortcutHandler), new Type[0], new Type[0]);
            ExpectRegistrationTriplet(publicMethods, nameof(SSC.RegisterAsyncGlobalShortcutHandler), typeof(IAsyncGlobalShortcutHandler), new Type[0], new[] { typeof(string) });

            ExpectReplaceAndKeyedRegistrationTriplet(publicMethods, nameof(SSC.ReplaceAsyncViewSubmissionHandling), nameof(SSC.RegisterAsyncViewSubmissionHandler), typeof(IAsyncViewSubmissionHandler));
            ExpectReplaceAndKeyedRegistrationTriplet(publicMethods, nameof(SSC.ReplaceAsyncSlashCommandHandling), nameof(SSC.RegisterAsyncSlashCommandHandler), typeof(IAsyncSlashCommandHandler));

            RemainingMethods(publicMethods)
                .ShouldBeEmpty("Unexpected public method(s)");
        }

        private static void ExpectReplaceAndKeyedRegistrationTriplet(List<MethodInfo> publicMethods, string replaceMethodName, string registerMethodName, Type handlerType)
        {
            ExpectReplaceMethod(publicMethods, replaceMethodName, handlerType);
            ExpectRegistrationTriplet(publicMethods, registerMethodName, handlerType, new Type[0], new[] { typeof(string) });
        }

        private static void ExpectRegistrationTriplet(List<MethodInfo> publicMethods, string name, Type handlerType, Type[] genericArgs, Type[] parameters)
        {
            name.ShouldStartWith("Register");
            ExpectMethod(publicMethods, name, genericArgs.Concat(new[] { handlerType }), parameters);
            ExpectMethod(publicMethods, name, genericArgs, parameters.Concat(new[] { handlerType }));
            ExpectMethod(publicMethods, name, genericArgs, parameters.Concat(new[] { FactoryFunc(handlerType) }));
        }

        private static void ExpectReplaceMethod(List<MethodInfo> publicMethods, string name, Type handlerType)
        {
            name.ShouldStartWith("Replace");
            ExpectMethod(publicMethods, name, new Type[0], new[] { FactoryFunc(handlerType) });
        }

        private static Type FactoryFunc(Type implementationTypeConstraint) => 
            typeof(Func<,>).MakeGenericType(typeof(IServiceProvider), implementationTypeConstraint);

        private static void ExpectMethod(List<MethodInfo> methods, string name, IEnumerable<Type> genericParameters, IEnumerable<Type> methodParameters) =>
            FindMethod(methods, name, genericParameters, methodParameters)
                .ShouldBe(1, $"Missing {MethodSignature(name, genericParameters, methodParameters, typeof(SSC))}");

        private static int FindMethod(List<MethodInfo> methods, string name, IEnumerable<Type> genericParameters, IEnumerable<Type> methodParameters) =>
            methods.RemoveAll(m => m.Name == name
                && m.GetGenericArguments().SequenceEqual(genericParameters, GenericTypeComparer.Instance)
                && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(methodParameters, GenericTypeComparer.Instance));

        private static IEnumerable<string> RemainingMethods(IEnumerable<MethodInfo> publicMethods) =>
            publicMethods.Select(m => MethodSignature(m.Name, m.GetGenericArguments(), m.GetParameters().Select(p => p.ParameterType), m.ReturnType));

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