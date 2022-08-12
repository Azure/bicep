// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.VSLanguageServerClient
{
    public class BicepLanguageServerClientConstants
    {
        public const string BicepContentType = "bicep";
        public const string BicepFileExtension = ".bicep";

        public const string BicepConfigFileName = "bicepconfig.json";

        public const string BicepConfigContentType = "BicepConfig";
        public const string BicepConfigFileExtension = ".json";

        // Code remote content type name. Lights up all TextMate features (colorization, brace completion, folding ranges).
        public const string CodeRemoteContentTypeName = "code-languageserver-preview";

        public const string JsonContentTypeName = "JSON";
    }
}
