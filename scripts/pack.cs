#!/usr/bin/env dotnet
#:package DotNetDo.Core@0.9.0
using DotNetDo;
using static DotNetDo.Tools;

await DotNet.Test;

await (DotNet.Pack with
    {
        Version = (await GitVersion).FullSemVer,
        Output = Do.RootDirectory / "output"
    });