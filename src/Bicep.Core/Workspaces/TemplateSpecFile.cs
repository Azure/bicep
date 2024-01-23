// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Workspaces
{
    public class TemplateSpecFile : ISourceFile
    {
        public TemplateSpecFile(Uri fileUri, string? templateSpecId, ArmTemplateFile mainTemplateFile)
        {
            this.FileUri = fileUri;
            this.TemplateSpecId = templateSpecId;
            this.MainTemplateFile = mainTemplateFile;
        }

        public Uri FileUri { get; }

        public string GetOriginalSource() => MainTemplateFile.GetOriginalSource();

        public string? TemplateSpecId { get; }

        public ArmTemplateFile MainTemplateFile { get; }

        public bool HasErrors() => this.TemplateSpecId is null;
    }
}
