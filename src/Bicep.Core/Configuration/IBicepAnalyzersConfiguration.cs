// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Configuration
{
    public interface IBicepAnalyzersConfiguration
    {
        T GetValue<T>(string path, T defaultValue);
    }
}
