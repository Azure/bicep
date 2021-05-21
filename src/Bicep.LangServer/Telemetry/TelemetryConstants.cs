// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.LanguageServer.Telemetry
{
    public class TelemetryConstants
    {
        public const string Prefix = "bicep/";
        public const string CommandName = "bicep.Telemetry";

        public class EventNames
        {
            public const string DeclarationSnippetCompletion = Prefix + nameof(DeclarationSnippetCompletion);
        }
    }
}
