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

            public const string NestedResourceDeclarationSnippetInsertion = "snippet/nestedresource";
            public const string TopLevelDeclarationSnippetInsertion = "snippet/toplevel";
            public const string ResourceBodySnippetInsertion = "snippet/resourcebody";
            public const string ModuleBodySnippetInsertion = "snippet/modulebody";
            public const string ObjectBodySnippetInsertion = "snippet/object";

            public const string DisableNextLineDiagnostics = "diagnostics/disablenextline";

            // Rule names are all in lower case to help ease querying. The names get lowercased before they are stored.
            // So doing it upfront here will avoid confusion while querying.
            public const string LinterCoreEnabledStateChange = "linter/coreenabledstatechange";
            public const string LinterRuleStateChange = "linter/rulestatechange";
            public const string LinterRuleStateOnBicepFileOpen = "linter/rulestateonopen";
        }
    }
}
