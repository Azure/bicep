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
            EnableAsserts = features.AssertsEnabled;
            EnableSymbolicNames = features.SymbolicNameCodegenEnabled || features.ExtensibilityEnabled || features.UserDefinedTypesEnabled || EnableAsserts;
            FileKind = fileKind;
        }

        /// <summary>
        /// Generate asserts in template output
        /// </summary>
        public bool EnableAsserts { get; }

        /// <summary>
        /// Generate symbolic names in template output?
        /// </summary>
        public bool EnableSymbolicNames { get; }

        public BicepSourceFileKind FileKind { get; }
    }
}
