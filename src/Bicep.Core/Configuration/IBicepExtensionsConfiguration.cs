// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;

namespace Bicep.Core.Configuration
{
    public interface IBicepExtensionsConfiguration
    {
        IEnumerable<string> ExtensionNames { get; }

        ResultWithDiagnosticBuilder<ExtensionConfigEntry> TryGetExtensionSource(string extensionName);

        bool IsSysOrBuiltIn(string extensionName);
    }
}
