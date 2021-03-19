// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using System.Collections.Generic;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    internal class BL1040 : LinterRule
    {
        internal BL1040() : base(
            code: "BL1040",
            ruleName: "Location set by parameter",
            description: "Best practice dictates that Location be set via parameter.",
            docUri: "https://bicep/linter/rules/BL1040")
        { }

    }
}
