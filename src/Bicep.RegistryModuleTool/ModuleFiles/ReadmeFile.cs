// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Extensions;
using Bicep.RegistryModuleTool.ModuleFileValidators;
using System.Collections.Generic;
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

            builder.AppendLine($"# {metadataFile.Name}");
            builder.AppendLine();

            builder.AppendLine(metadataFile.Description);
            builder.AppendLine();

            BuildParametersTable(builder, mainArmTemplateFile.Parameters);
            BuildOutputsTable(builder, mainArmTemplateFile.Outputs);

            return new(fileSystem.Path.GetFullPath(FileName), builder.ToString());
        }

        public static ReadmeFile ReadFromFileSystem(IFileSystem fileSystem)
        {
            var path = fileSystem.Path.GetFullPath(FileName);
            var content = fileSystem.File.ReadAllText(FileName);

            return new(path, content);
        }

        public ReadmeFile WriteToFileSystem(IFileSystem fileSystem)
        {
            fileSystem.File.WriteAllText(FileName, this.Content);

            return this;
        }

        protected override void ValidatedBy(IModuleFileValidator validator) => validator.Validate(this);

        private static void BuildParametersTable(StringBuilder builder, IEnumerable<MainArmTemplateParameter> parameters)
        {
            builder.AppendLine("## Parameters");
            builder.AppendLine();
            builder.AppendLine(parameters
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
        }

        private static void BuildOutputsTable(StringBuilder builder, IEnumerable<MainArmTemplateOutput> outputs)
        {
            builder.AppendLine("## Outputs");
            builder.AppendLine();
            builder.AppendLine(outputs.ToMarkdownTable(
                columnName => columnName == nameof(MainArmTemplateOutput.Type)
                    ? MarkdownTableColumnAlignment.Center
                    : MarkdownTableColumnAlignment.Left));
            builder.AppendLine();
        }
    }
}
