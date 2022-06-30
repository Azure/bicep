// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Reflection;

namespace Bicep.LangServer.IntegrationTests.Completions
{
    public class CompletionData
    {
        public CompletionData(string prefix, string snippetText)
        {
            Prefix = prefix;
            SnippetText = snippetText;
        }

        public string Prefix { get; }

        public string SnippetText { get; }

        public static string GetDisplayName(MethodInfo info, object[] data)
            => $"{info.Name}_{((CompletionData)data[0]).Prefix}";
    }
}
