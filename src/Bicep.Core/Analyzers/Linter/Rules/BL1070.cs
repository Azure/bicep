// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Parsing;
using System.Collections.Generic;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    internal class BL1070 : LinterRule
    {
        internal BL1070() : base(
            code: "BL1070",
            ruleName: "Unecessary dependsOn",
            description: "Best Practice: unnecessary dependsOn encountered",
            docUri: "https://bicep/linter/rules/BL1070")
        { }

    }
}
