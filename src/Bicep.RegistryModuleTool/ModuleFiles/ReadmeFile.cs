// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Extensions;
using Bicep.RegistryModuleTool.ModuleFileValidators;
using System.IO.Abstractions;
using System.Linq;
using System.Text;

namespace Bicep.RegistryModuleTool.ModuleFiles
{
    public sealed class ReadmeFile : ModuleFile
    {
        public const string FileName = "README.md";

        public ReadmeFile(string path, string content)
            : base(path)
        {
            this.Content = content;
        }

        public string Content { get; }

        public static ReadmeFile Generate(IFileSystem fileSystem, MetadataFile metadataFile, MainArmTemplateFile mainArmTemplateFile)
        {
            var builder = new StringBuilder();

            // TODO: generate badges.

            builder.AppendLine($"# {metadataFile.ItemDisplayName}");
            builder.AppendLine();

            builder.AppendLine(metadataFile.Description);
            builder.AppendLine();

            builder.AppendLine("## Parameters");
            builder.AppendLine();
            builder.AppendLine(mainArmTemplateFile.Parameters
                .Select(p => new
                {
                    Name = $"`{p.Name}`",
                    Type = $"`{p.Type}`",
                    Required = p.Required ? "Yes" : "No",
                    p.Description,
                })
                .ToMarkdownTable(columnName => columnName switch
                {
                    nameof(MainArmTemplateParameter.Type) or
                    nameof(MainArmTemplateParameter.Required) => MarkdownTableColumnAlignment.Center,
                    _ => MarkdownTableColumnAlignment.Left,
                }));
            builder.AppendLine();

            builder.AppendLine("## Outputs");
            builder.AppendLine();
            builder.AppendLine(mainArmTemplateFile.Outputs.ToMarkdownTable(
                columnName => columnName == nameof(MainArmTemplateOutput.Type)
                    ? MarkdownTableColumnAlignment.Center
                    : MarkdownTableColumnAlignment.Left));
            builder.AppendLine();

            return new(fileSystem.Path.GetFullPath(FileName), builder.ToString());
        }

        public static ReadmeFile ReadFromFileSystem(IFileSystem fileSystem)
        {
            var path = fileSystem.Path.GetFullPath(FileName);
            var content = fileSystem.File.ReadAllText(FileName);

            return new(path, content);
        }

        public void WriteToFileSystem(IFileSystem fileSystem) => fileSystem.File.WriteAllText(FileName, this.Content);

        protected override void ValidatedBy(IModuleFileValidator validator) => validator.Validate(this);
    }
}
