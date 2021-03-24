// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Parsing;
using System.Collections.Generic;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    internal class BCPL1070 : LinterRule
    {
        internal BCPL1070() : base(
            code: "BCPL1070",
            ruleName: "Unecessary dependsOn",
            description: "Best Practice: unnecessary dependsOn encountered",
            docUri: "https://bicep/linter/rules/BCPL1070")
        { }

    }
}
