// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Text.RegularExpressions;

namespace Bicep.Core.TypeSystem.Providers.Az
{
    public static class AzConstants
    {
        public static readonly Regex ListWildcardFunctionRegex = new("^list[a-zA-Z]*$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    }
}
