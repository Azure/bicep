// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.SemanticVersioning;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Configuration
{
    /// <summary>
    /// Checks the running Bicep compiler version against the "bicep.version" constraint from the effective
    /// configuration for a single file.
    /// </summary>
    public static class CompilerVersionValidator
    {
        /// <summary>
        /// Validates <paramref name="runningVersion"/> against <paramref name="constraint"/>. Returns a diagnostic
        /// when the constraint is violated, a warning diagnostic when the running version cannot be parsed (so the
        /// constraint could not be checked), or null when there is no constraint or the constraint is satisfied.
        /// <paramref name="configFileUri"/> is the effective bicepconfig.json that declared the constraint (or
        /// null if it came from the built-in defaults), and is included in any diagnostic produced.
        /// </summary>
        public static IDiagnostic? Validate(VersionRange? constraint, string runningVersion, IOUri? configFileUri)
        {
            if (constraint is null)
            {
                // No constraint configured anywhere in the bicepconfig.json resolution chain - fall back to the
                // currently installed version without performing any check.
                return null;
            }

            if (!SemanticVersion.TryParse(runningVersion, out var parsedRunningVersion))
            {
                // The running version can't be parsed, so we can't meaningfully compare it. Warn rather than
                // silently skipping, so users are aware the "bicep.version" constraint was not actually checked.
                return DiagnosticBuilder.ForDocumentStart().BicepVersionConstraintCouldNotBeChecked(constraint.ToString(), runningVersion, configFileUri);
            }

            if (constraint.IsSatisfiedBy(parsedRunningVersion))
            {
                return null;
            }

            return DiagnosticBuilder.ForDocumentStart().BicepVersionConstraintNotSatisfied(constraint.ToString(), runningVersion, configFileUri);
        }
    }
}
