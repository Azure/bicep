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
            EnableAsserts = features.AssertsEnabled;
            FileKind = fileKind;
        }

        /// <summary>
        /// Generate symbolic names in template output?
        /// </summary>
        public bool EnableSymbolicNames { get; }

        /// <summary>
        /// Generate asserts in template output
        /// </summary>
        public bool EnableAsserts { get; }

        public BicepSourceFileKind FileKind { get; }
    }
}
