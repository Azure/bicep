// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Analyzers.Linter;

// Note: These aren't currently exposed to users (but might be later), they're currently used just to determine the default diagnostic level for a rule
public enum LinterRuleCategory
{
    BestPractice,
    DeploymentError,

    /// Rules concerning compatibility of bicep version constraints across Bicep files.
    VersionConstraintCompatibility,

    /// Informs the user that something will not work if the template is deployed by a Deployment stack.
    DeploymentStackIncompatibility,
    Portability,
    PotentialCodeIssues,
    ResourceLocationRules,
    Security,
    Style,
}
