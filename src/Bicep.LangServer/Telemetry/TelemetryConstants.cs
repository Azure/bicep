// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.LanguageServer.Telemetry
{
    public static class TelemetryConstants
    {
        public const string CommandName = "bicep.Telemetry";

        public static class EventNames
        {
            public const string NestedResourceDeclarationSnippetInsertion = nameof(NestedResourceDeclarationSnippetInsertion);
            public const string TopLevelDeclarationSnippetInsertion = nameof(TopLevelDeclarationSnippetInsertion);
            public const string ResourceBodySnippetInsertion = nameof(ResourceBodySnippetInsertion);
            public const string ModuleBodySnippetInsertion = nameof(ModuleBodySnippetInsertion);
            public const string ObjectBodySnippetInsertion = nameof(ObjectBodySnippetInsertion);

            public const string DisableNextLineDiagnostics = nameof(DisableNextLineDiagnostics);
        }
    }
}
