// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Workspaces
{
    public class TemplateSpecFile : ISourceFile
    {
        public TemplateSpecFile(Uri fileUri, string text, string? templateSpecId, ArmTemplateFile mainTemplateFile)
        {
            this.Uri = fileUri;
            this.Text = text;
            this.TemplateSpecId = templateSpecId;
            this.MainTemplateFile = mainTemplateFile;
        }

        public Uri Uri { get; }

        public string Text { get; }

        public string? TemplateSpecId { get; }

        public ArmTemplateFile MainTemplateFile { get; }

        public bool HasErrors() => this.TemplateSpecId is null;
    }
}
