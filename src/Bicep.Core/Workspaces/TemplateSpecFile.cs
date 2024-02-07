// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Workspaces
{
    public class TemplateSpecFile(Uri fileUri, string? templateSpecId, ArmTemplateFile mainTemplateFile) : ISourceFile
    {
        public Uri FileUri { get; } = fileUri;

        public string GetOriginalSource() => MainTemplateFile.GetOriginalSource();

        public string? TemplateSpecId { get; } = templateSpecId;

        public ArmTemplateFile MainTemplateFile { get; } = mainTemplateFile;

        public bool HasErrors() => this.TemplateSpecId is null;
    }
}
