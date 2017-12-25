var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");
var verbosity = Argument<DotNetCoreVerbosity>("verbosity", DotNetCoreVerbosity.Minimal);
var version = new Lazy<GitVersion>(GitVersion);
var slnFolder = ".";

if (AppVeyor.IsRunningOnAppVeyor)
    AppVeyor.UpdateBuildVersion(version.Value.FullSemVer);

Task("Build")
    .IsDependentOn("Restore")
    .Does(() => {
        DotNetCoreBuild(slnFolder, new DotNetCoreBuildSettings{
            Verbosity = verbosity,
            Configuration = configuration,
            MSBuildSettings = MSBuildSettings()
        });
    });

Task("Restore")
    .Does(() => {
        DotNetCoreRestore(slnFolder, new DotNetCoreRestoreSettings {
            Verbosity = verbosity,
            MSBuildSettings = MSBuildSettings()
        });
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() => {
        DotNetCoreTest($"{slnFolder}/SlackNet.Tests", new DotNetCoreTestSettings {
            Verbosity = verbosity,
            Configuration = configuration
        });
    });

Task("Pack")
    .IsDependentOn("Test")
    .Does(() => {
        DotNetCorePack(slnFolder, new DotNetCorePackSettings {
            Verbosity = verbosity,
            Configuration = configuration,
            MSBuildSettings = MSBuildSettings()
        });
    });

DotNetCoreMSBuildSettings MSBuildSettings() {
    return new DotNetCoreMSBuildSettings()
        .WithProperty("Version", version.Value.SemVer);
}

Task("Clean")
    .Does(() => {
        CleanDirectories($"{slnFolder}/**/bin");
        CleanDirectories($"{slnFolder}/**/obj");
    });

RunTarget(target);