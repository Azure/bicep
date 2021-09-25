// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Emit;

namespace Bicep.Core.UnitTests.Utils
{
    public static class EmitterSettingsHelper
    {
        public static EmitterSettings DefaultTestSettings
            => new EmitterSettings(BicepTestConstants.DevAssemblyFileVersion, false);

        public static EmitterSettings WithSymbolicNamesEnabled
            => new EmitterSettings(BicepTestConstants.DevAssemblyFileVersion, true);
    }
}
