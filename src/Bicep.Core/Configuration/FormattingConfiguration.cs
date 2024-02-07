// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.Extensions;
using Bicep.Core.PrettyPrintV2;

namespace Bicep.Core.Configuration
{
    public class FormattingConfiguration(PrettyPrinterV2Options data) : ConfigurationSection<PrettyPrinterV2Options>(data)
    {
        public static FormattingConfiguration Bind(JsonElement element)
            => new(element.ToNonNullObject<PrettyPrinterV2Options>());
    }
}
