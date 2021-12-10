// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.LanguageServer.Providers
{
    public interface ILinterRulesProvider
    {
        ImmutableDictionary<string, string> GetLinterRules();
    }
}
