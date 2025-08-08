// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Definitions.Schema;
using Bicep.IO.Abstraction;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.SourceGraph
{
    public class ArmTemplateFile : ISourceFile
    {
        public ArmTemplateFile(IFileHandle fileHandle, string text, Template? template, JObject? templateObject)
        {
            if ((template is null && templateObject is not null) ||
                (template is not null && templateObject is null))
            {
                throw new ArgumentException($"Expected {nameof(template)} and {nameof(templateObject)} to both be non-null or both be null.");
            }

            this.Uri = fileHandle.Uri.ToUri();
            this.FileHandle = fileHandle;
            this.Text = text;
            this.Template = template;
            this.TemplateObject = templateObject;
        }

        public Uri Uri { get; }

        public IFileHandle FileHandle { get; }

        public string Text { get; }

        public Template? Template { get; }

        public JObject? TemplateObject { get; }

        public bool HasErrors() => this.Template is null;
    }
}
