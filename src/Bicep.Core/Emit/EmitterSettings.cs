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
            EnableSourceMapping = features.SourceMappingEnabled;
            EnableSymbolicNames = features.SymbolicNameCodegenEnabled || features.ExtensibilityEnabled || features.UserDefinedTypesEnabled;
        }

        /// <summary>
        /// Assembly File Version to emit into the metadata
        /// </summary>
        public string AssemblyFileVersion { get; }

        /// <summary>
        /// Generate source map during template output
        /// </summary>
        public bool EnableSourceMapping { get; }

        /// <summary>
        /// Generate symbolic names in template output?
        /// </summary>
        public bool EnableSymbolicNames { get; }
    }
}
