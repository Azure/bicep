// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Features;

namespace Bicep.Core.Emit
{
    public class EmitterSettings
    {
        public EmitterSettings(IFeatureProvider features)
        {
            EnableSymbolicNames = features.SymbolicNameCodegenEnabled || features.ExtensibilityEnabled || features.UserDefinedTypesEnabled;
        }

        /// <summary>
        /// Generate symbolic names in template output?
        /// </summary>
        public bool EnableSymbolicNames { get; }
    }
}
