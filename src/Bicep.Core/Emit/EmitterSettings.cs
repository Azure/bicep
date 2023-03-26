// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Features;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Emit
{
    public class EmitterSettings
    {
        public EmitterSettings(IFeatureProvider features, BicepSourceFileKind fileKind)
        {
            EnableSymbolicNames = features.SymbolicNameCodegenEnabled || features.ExtensibilityEnabled || features.UserDefinedTypesEnabled;
            FileKind = fileKind;
        }

        /// <summary>
        /// Generate symbolic names in template output?
        /// </summary>
        public bool EnableSymbolicNames { get; }

        public BicepSourceFileKind FileKind { get; }
    }
}
