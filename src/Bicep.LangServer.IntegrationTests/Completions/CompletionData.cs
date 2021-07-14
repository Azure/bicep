// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        public static string GetDisplayName(MethodInfo methodInfo, object[] data) => ((CompletionData)data[0]).Prefix!;
    }
}
