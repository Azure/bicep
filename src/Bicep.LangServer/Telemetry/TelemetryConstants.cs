// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.LanguageServer.Telemetry
{
    public static class TelemetryConstants
    {
        public const string CommandName = "bicep.Telemetry";

        public static class EventNames
        {
            public const string BicepFileOpen = "file/bicepopen";

            public const string NestedResourceDeclarationSnippetInsertion = nameof(NestedResourceDeclarationSnippetInsertion);
            public const string TopLevelDeclarationSnippetInsertion = nameof(TopLevelDeclarationSnippetInsertion);
            public const string ResourceBodySnippetInsertion = nameof(ResourceBodySnippetInsertion);
            public const string ModuleBodySnippetInsertion = nameof(ModuleBodySnippetInsertion);
            public const string ObjectBodySnippetInsertion = nameof(ObjectBodySnippetInsertion);

            public const string DisableNextLineDiagnostics = nameof(DisableNextLineDiagnostics);

            // Rule names are all in lower case to help ease querying. The names get lowercased before they are stored.
            // So doing it upfront here will avoid confusion while querying.
            public const string LinterCoreEnabledStateChange = "linter/coreenabledstatechange";
            public const string LinterRuleStateChange = "linter/rulestatechange";
            public const string LinterRuleStateOnBicepFileOpen = "linter/rulestateonopen";
        }
    }
}
