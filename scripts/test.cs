#!/usr/bin/env dotnet
#:package DotNetDo.Core@0.9.0
using DotNetDo;
using Serilog;
using static DotNetDo.Tools;

await DotNet.Test;

ValidatePackages();

static void ValidatePackages()
{
    string[] expected =
        [
            "SlackNet",
            "SlackNet.AspNetCore",
            "SlackNet.Autofac",
            "SlackNet.AzureFunctions",
            "SlackNet.Bot",
            "SlackNet.Extensions.DependencyInjection",
            "SlackNet.SimpleInjector",
        ];

    var unexpectedPackages = Do.Solution.Projects
        .Where(p => !expected.Contains(p.Path.NameWithoutExtension)
            && p.Project.GetPropertyValue("IsPackable").Equals("true", StringComparison.OrdinalIgnoreCase))
        .Select(p => p.Path.NameWithoutExtension)
        .ToArray();

    if (unexpectedPackages.Any())
    {
        Log.Error("Unexpected packages: {Packages}", unexpectedPackages);
        throw new InvalidOperationException("Unexpected packages.");
    }
}