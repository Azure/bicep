// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Configuration
{
    public interface IBicepImplicitExtensionsConfiguration
    {
        IEnumerable<string> GetImplicitExtensionNames();
    }
}
