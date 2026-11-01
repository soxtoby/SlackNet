#!/usr/bin/env dotnet
#:package DotNetDo.Core@0.9.0
using DotNetDo;
using static DotNetDo.Tools;

[assembly: TaskDescription("Publish the packed NuGet packages and GitHub release.")]

var apiKey = Do.Secret("nuget_api_key", description: "Temporary NuGet API key.").Required();

var version = (Do.RootDirectory / "Directory.Build.props").ReadXml().Descendants("VersionPrefix").Single().Value;
var tag = $"v{version}";

await (DotNet.NuGetPush with
    {
        Package = Do.RootDirectory / "artifacts" / "packages" / "*.nupkg",
        ApiKey = apiKey.Unwrap(),
        Source = "https://api.nuget.org/v3/index.json",
        SkipDuplicate = true,
    });

var notesFile = Do.CreateTempFile("slacknet-release-notes-", ".md");
notesFile.WriteLines((Do.RootDirectory / "CHANGELOG.md").ReadLines()
    .SkipWhile(line => line != $"## {tag}")
    .Skip(1)
    .TakeWhile(line => !line.StartsWith("## ")));
await Do.Exec($"gh release create {tag} --title {tag} --notes-file {notesFile.QuotedArgument()}");
