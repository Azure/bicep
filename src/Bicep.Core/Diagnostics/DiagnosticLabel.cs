// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.Diagnostics
{
    public enum DiagnosticStyling
    {
        /// <summary>
        /// No particular diagnostic styling - this should be the default for most diagnostic failures
        /// </summary>
        Default = 0,

        /// <summary>
        /// Shows the code inside the error dimmed out, i.e. for unused variables and other code
        /// </summary>
        ShowCodeAsUnused = 1,

        /// <summary>
        /// Indicate the code in the error should be shown as deprecated
        /// </summary>
        ShowCodeDeprecated = 2
    }
}
