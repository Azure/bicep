// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.PrettyPrintV2;
using System.Text.Json;

namespace Bicep.Core.Configuration
{
    public class FormattingConfiguration : ConfigurationSection<PrettyPrinterV2Options>
    {
        public FormattingConfiguration(PrettyPrinterV2Options data)
            : base(data)
        {
        }

        public static FormattingConfiguration Bind(JsonElement element)
            => new(element.ToNonNullObject<PrettyPrinterV2Options>());
    }
}
