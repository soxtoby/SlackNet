#!/usr/bin/env dotnet
#:package DotNetDo.Core@0.9.0
using DotNetDo;
using Serilog;
using Serilog.Events;

[assembly: TaskDescription("Prepare the next release version and changelog.")]

Logging.Level = LogEventLevel.Information;

if (Do.GitRepo.IsDirty)
    throw new InvalidOperationException("prepare-release requires a clean Git worktree.");

var projectFile = Do.RootDirectory / "Directory.Build.props";
var changelogFile = Do.RootDirectory / "CHANGELOG.md";

var current = Version.Parse(projectFile.ReadXml().Descendants("VersionPrefix").Single().Value);
var changelog = changelogFile.ReadLines().ToList();

var latestRelease = changelog.First(line => line.StartsWith("## v"));
if (latestRelease != $"## v{current}")
    throw new InvalidOperationException($"Project version {current} does not match latest changelog release '{latestRelease}'.");

var unreleasedIndex = changelog.IndexOf("## Unreleased");
var headings = changelog
    .Skip(unreleasedIndex + 1)
    .TakeWhile(line => !line.StartsWith("## "))
    .Where(line => line.StartsWith("### "))
    .Select(line => line[4..].Trim())
    .ToHashSet();

// A misspelled heading would quietly release the wrong version.
var unknown = headings.Except(["Breaking", "Added", "Changed", "Fixed"]).ToArray();
if (unknown.Any())
    throw new InvalidOperationException($"Unknown Unreleased change heading: {unknown.JoinWith(", ")}.");

var next = (current.Major, headings.Contains("Breaking"), headings.Overlaps(["Added", "Changed"])) switch
    {
        // Before 1.0, breaking changes bump the minor version, and everything else bumps the patch version.
        (0, true, _) => new Version(0, current.Minor + 1, 0),
        (_, true, _) => new Version(current.Major + 1, 0, 0),
        (> 0, _, true) => new Version(current.Major, current.Minor + 1, 0),
        _ => new Version(current.Major, current.Minor, current.Build + 1),
    };

projectFile.WriteText(projectFile.ReadText().Replace(
    $"<VersionPrefix>{current}</VersionPrefix>",
    $"<VersionPrefix>{next}</VersionPrefix>"));

changelog.InsertRange(unreleasedIndex + 1, ["", $"## v{next}"]);
changelogFile.WriteLines(changelog);

var quiet = new ExecOptions() { LogCommand = false, Log = ExecLog.ErrorsOnly };
var maintainer = (await Do.GitRepo.Exec("config user.name", quiet)).ReadText().Trim();
var contributors = (await Do.GitRepo.Exec($"log v{current}..HEAD --format=%aN", quiet)).OutputLines()
    .Where(author => author != maintainer && !author.EndsWith("[bot]"))
    .Distinct()
    .ToArray();

Log.Information("Next version: {Next}", next);
if (contributors.Any())
    Log.Warning("Contributors since v{Current}: {Contributors} - remember to thank them!", current, contributors);