// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.SourceGraph
{
    // TODO: Move to a more common namespace
    public enum BicepSourceFileKind
    {
        /// <summary>
        /// A Bicep file containing parameter declarations, resources, variables, outputs,
        /// and references to other modules. This is sometimes known as a Bicep module.
        /// </summary>
        BicepFile,

        /// <summary>
        /// A Bicep parameters file that may contain a reference to a Bicep file and may
        /// also set values of the parameters declared in the referenced Bicep file.
        /// </summary>
        ParamsFile,

        /// <summary>
        /// A Bicep file used in the REPL environment.
        /// </summary>
        ReplFile
    }
}
