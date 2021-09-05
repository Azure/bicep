// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Emit
{
    public class EmitterSettings
    {
        public EmitterSettings(string assemblyFileVersion, bool enableSymbolicNames)
        {
            AssemblyFileVersion = assemblyFileVersion;
            EnableSymbolicNames = enableSymbolicNames;
        }

        /// <summary>
        /// Assembly File Version to emit into the metadata
        /// </summary>
        public string AssemblyFileVersion { get; }

        /// <summary>
        /// Generate symbolic names in template output?
        /// </summary>
        public bool EnableSymbolicNames { get; }
    }
}