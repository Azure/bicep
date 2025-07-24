// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.IO.Abstraction;

namespace Bicep.Core.SourceGraph
{
    public class TemplateSpecFile : ISourceFile
    {
        public TemplateSpecFile(Uri fileUri, IFileHandle fileHandle, string text, string? templateSpecId, ArmTemplateFile mainTemplateFile)
        {
            this.Uri = fileUri;
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

        public RootConfiguration Configuration => MainTemplateFile.Configuration;

        public IFeatureProvider Features => MainTemplateFile.Features;

        public BicepSourceFileKind FileKind => BicepSourceFileKind.ArmTemplateFile;
    }
}
