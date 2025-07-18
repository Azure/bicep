// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Utils;

namespace Bicep.TextFixtures.Utils
{
    public record TestEnvironment(ImmutableDictionary<string, string?> Variables, string CurrentDirectory) : IEnvironment
    {
        public readonly static TestEnvironment Default = new([], System.Environment.CurrentDirectory);

        public TestEnvironment(Dictionary<string, string?> variables, string currentDirectory)
            : this(variables.ToImmutableDictionary(), currentDirectory)
        {
        }

        public IEnvironment WithVariables(params (string key, string? value)[] variables) =>
            this with { Variables = variables.ToImmutableDictionary(x => x.key, x => x.value) };

        public string? GetVariable(string variable) => this.Variables.TryGetValue(variable, out var value) ? value : null;

        public IEnumerable<string> GetVariableNames() => this.Variables.Keys;
    }
}
