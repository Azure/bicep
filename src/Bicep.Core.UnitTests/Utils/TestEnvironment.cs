// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Utils;

namespace Bicep.Core.UnitTests.Utils;

public record TestEnvironment(
    ImmutableDictionary<string, string?> Variables,
    string CurrentDirectory
) : IEnvironment
{
    public static TestEnvironment Default = new(
        ImmutableDictionary<string, string?>.Empty,
        System.Environment.CurrentDirectory);

    public IEnvironment WithVariables(params (string key, string? value)[] variables)
        => this with { Variables = variables.ToImmutableDictionary(x => x.key, x => x.value) };

    public string? GetVariable(string variable)
        => Variables.TryGetValue(variable, out var value) ? value : null;

    public IEnumerable<string> GetVariableNames()
        => Variables.Keys;
}
