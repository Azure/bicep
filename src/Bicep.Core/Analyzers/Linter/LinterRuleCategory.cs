// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Analyzers.Linter;

// Note: These aren't currently exposed to users (but might be later), they're currently used just to determine the default diagnostic level for a rule
public enum LinterRuleCategory
{
    BestPractice,
    DeploymentError,

    /// Informs the user that something may go wrong at deployment time due to an external tool's choices (e.g. version selection).
    VersionConstraintCompatibility,

    /// Informs the user that something will not work if the template is deployed by a Deployment stack.
    DeploymentStackIncompatibility,
    Portability,
    PotentialCodeIssues,
    ResourceLocationRules,
    Security,
    Style,
}
