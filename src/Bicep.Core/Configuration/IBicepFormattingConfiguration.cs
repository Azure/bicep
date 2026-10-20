// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.PrettyPrintV2;

namespace Bicep.Core.Configuration
{
    public interface IBicepFormattingConfiguration
    {
        PrettyPrinterV2Options Data { get; }
    }
}
