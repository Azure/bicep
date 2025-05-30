// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.TypeSystem.Providers.Extensibility
{
    public static class MicrosoftGraphExtensionFacts
    {
        public const string builtInExtensionName = "microsoftGraph";
        public const string BicepExtensionBetaName = "MicrosoftGraphBeta";
        public const string BicepExtensionV1Name = "MicrosoftGraph";
        public const string TemplateExtensionName = "MicrosoftGraph";

        public static bool IsMicrosoftGraphExtension(string extensionName) =>
            LanguageConstants.IdentifierComparer.Equals(extensionName, BicepExtensionBetaName) ||
            LanguageConstants.IdentifierComparer.Equals(extensionName, BicepExtensionV1Name);
    }
}
