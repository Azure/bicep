// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.CodeAction;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;

namespace Bicep.Core.Analyzers.Linter;

// Note: These aren't currently exposed to users (but might be later), they're currently used just to determine the default diagnostic level for a rule
public enum LinterRuleCategory
{
    BestPractice,
    DeploymentError,
    Portability,
    PotentialCodeIssues,
    ResourceLocationRules,
    Security,
    Style,
}
