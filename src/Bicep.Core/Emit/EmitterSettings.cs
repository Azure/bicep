// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Features;

namespace Bicep.Core.Emit
{
    public class EmitterSettings
    {
        public EmitterSettings(IFeatureProvider features)
        {
            AssemblyFileVersion = features.AssemblyVersion;
            EnableSymbolicNames = features.SymbolicNameCodegenEnabled || features.ImportsEnabled;
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