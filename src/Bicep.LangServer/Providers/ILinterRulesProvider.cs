// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Bicep.LanguageServer.Providers
{
    public interface ILinterRulesProvider
    {
        Dictionary<string, string> GetLinterRules();
    }
}
