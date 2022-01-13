// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.Analyzers.Linter
{
    public interface ILinterRulesProvider
    {
        ImmutableDictionary<string, string> GetLinterRules();

        IEnumerable<Type> GetRuleTypes();
    }
}
