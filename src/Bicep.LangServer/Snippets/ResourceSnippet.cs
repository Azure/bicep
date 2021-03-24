// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.LanguageServer.Snippets
{
    public class ResourceSnippet
    {
        public ResourceSnippet(string name, string detail, string text)
        {
            Name = name;
            Detail = detail;
            Text = text;
        }

        public string Name { get; }

        public string Detail { get; }

        public string Text { get; }
    }
}
