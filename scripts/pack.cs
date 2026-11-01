#!/usr/bin/env dotnet
#:package DotNetDo.Core@0.9.0
using DotNetDo;
using static DotNetDo.Tools;

[assembly: TaskDescription("Test and pack the NuGet packages into artifacts/packages.")]

await DotNet.Test;

// The release publishes everything here, so a stale package from an earlier version must not survive.
var packages = (Do.RootDirectory / "artifacts" / "packages").RecreateDirectory();

await (DotNet.Pack with { Output = packages });
