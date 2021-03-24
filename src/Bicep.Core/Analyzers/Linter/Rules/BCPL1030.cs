// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Parsing;
using System.Collections.Generic;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    internal class BCPL1030 : LinterRule
    {
        internal BCPL1030() : base(
            code: "BCPL1030",
            ruleName: "Secure parameter default not allowed",
            description: "Secure parameters can't have hardcoded default. This prevents storage of sensitive data in the Bicep declaration.",
            docUri: "https://bicep/linter/rules/BCPL1030")
        { }

    }
}
