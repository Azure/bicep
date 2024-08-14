// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Utils;

namespace Bicep.Core.UnitTests.Utils;

public class TestEnvironment : IEnvironment
{
    private readonly ImmutableDictionary<string, string?> variables;

    public TestEnvironment(ImmutableDictionary<string, string?> variables)
    {
        this.variables = variables;
    }

    public static IEnvironment Create(params (string key, string? value)[] variables)
        => new TestEnvironment(variables.ToImmutableDictionary(x => x.key, x => x.value));

    public string? GetVariable(string variable)
        => variables.TryGetValue(variable, out var value) ? value : null;

    public IEnumerable<string> GetVariableNames()
        => variables.Keys;
}
