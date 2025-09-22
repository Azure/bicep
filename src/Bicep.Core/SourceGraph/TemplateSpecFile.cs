// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;

namespace Bicep.Core.SourceGraph
{
    public class TemplateSpecFile : ISourceFile
    {
        public TemplateSpecFile(IFileHandle fileHandle, string text, string? templateSpecId, ArmTemplateFile mainTemplateFile)
        {
            this.Uri = fileHandle.Uri.ToUri();
            this.FileHandle = fileHandle;
            this.Text = text;
            this.TemplateSpecId = templateSpecId;
            this.MainTemplateFile = mainTemplateFile;
        }

        public Uri Uri { get; }

        public IFileHandle FileHandle { get; }

        public string Text { get; }

        public string? TemplateSpecId { get; }

        public ArmTemplateFile MainTemplateFile { get; }

        public bool HasErrors() => this.TemplateSpecId is null;
    }
}
