// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Bicep.Core.Utils;

namespace Bicep.Core.UnitTests.Utils;

public record TestEnvironment(
    ImmutableDictionary<string, string?> Variables,
    string CurrentDirectory,
    IEnvironment.BicepVersionInfo? VersionOverride = null
) : IEnvironment
{
    public static TestEnvironment Default = new([], System.Environment.CurrentDirectory);

    private readonly IEnvironment realEnvironment = new Core.Utils.Environment();

    public IEnvironment WithVariables(params (string key, string? value)[] variables)
        => this with { Variables = variables.ToImmutableDictionary(x => x.key, x => x.value) };

    public IEnvironment WithVersion(string version, string? commitRef = null)
        => this with { VersionOverride = new IEnvironment.BicepVersionInfo(version, commitRef) };

    public string? GetVariable(string variable)
        => Variables.TryGetValue(variable, out var value) ? value : null;

    public IEnumerable<string> GetVariableNames()
        => Variables.Keys;

    public OSPlatform? CurrentPlatform => realEnvironment.CurrentPlatform;

    public Architecture CurrentArchitecture => realEnvironment.CurrentArchitecture;

    public IEnvironment.BicepVersionInfo CurrentVersion => VersionOverride ?? realEnvironment.CurrentVersion;
}
