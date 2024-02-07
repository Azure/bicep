// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Reflection;

namespace Bicep.LangServer.IntegrationTests.Completions
{
    public class CompletionData(string prefix, string snippetText)
    {
        public string Prefix { get; } = prefix;

        public string SnippetText { get; } = snippetText;

        public static string GetDisplayName(MethodInfo info, object[] data)
            => $"{info.Name}_{((CompletionData)data[0]).Prefix}";
    }
}
