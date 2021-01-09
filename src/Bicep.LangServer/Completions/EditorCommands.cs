// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.LanguageServer.Completions
{
    /// <summary>
    /// Constants for editor commands to be used with completions.
    /// </summary>
    /// <remarks>This is very VS-code specific. An editor-agnostic solution does not currently exist.</remarks>
    public static class EditorCommands
    {
        public const string SignatureHelp = "editor.action.triggerParameterHints";

        public const string RequestCompletions = "editor.action.triggerSuggest";
    }
}
