// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Bicep.Core.Utils;

namespace Bicep.Testing.Fakes;

public record FakeEnvironment(ImmutableDictionary<string, string?> Variables, string CurrentDirectory) : IEnvironment
{
    public static readonly FakeEnvironment Default = new([], System.Environment.CurrentDirectory);

    private readonly IEnvironment realEnvironment = new Core.Utils.Environment();

    public IEnvironment WithVariables(params (string key, string? value)[] variables) =>
        this with { Variables = variables.ToImmutableDictionary(x => x.key, x => x.value) };

    public string? GetVariable(string variable) => this.Variables.TryGetValue(variable, out var value) ? value : null;

    public IEnumerable<string> GetVariableNames() => this.Variables.Keys;

    public OSPlatform? CurrentPlatform => this.realEnvironment.CurrentPlatform;

    public Architecture CurrentArchitecture => this.realEnvironment.CurrentArchitecture;

    public IEnvironment.BicepVersionInfo CurrentVersion => this.realEnvironment.CurrentVersion;
}
