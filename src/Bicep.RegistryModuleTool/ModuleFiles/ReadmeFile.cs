// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Extensions;
using Bicep.RegistryModuleTool.ModuleFileValidators;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;

namespace Bicep.RegistryModuleTool.ModuleFiles
{
    public sealed class ReadmeFile : ModuleFile
    {
        public const string FileName = "README.md";

        public ReadmeFile(string path, string contents)
            : base(path)
        {
            this.Contents = contents;
        }

        public string Contents { get; }

        public static ReadmeFile Generate(IFileSystem fileSystem, MetadataFile metadataFile, MainArmTemplateFile mainArmTemplateFile)
        {
            var examplesSection = @"## Examples
### Example 1
```bicep
```
### Example 2
```bicep
```".ReplaceLineEndings();

            try
            {
                var existingFile = ReadFromFileSystem(fileSystem);
                var existingExamplesSection = TryReadSection(existingFile.Contents, 2, "Examples");

                if (existingExamplesSection is not null && !existingExamplesSection.Equals("## Examples", StringComparison.Ordinal))
                {
                    // The existing examples section is not empty.
                    examplesSection = existingExamplesSection;
                }
            }
            catch (FileNotFoundException)
            {
                // Do thing.
            }

            var builder = new StringBuilder();

            builder.AppendLine($"# {metadataFile.Name}");
            builder.AppendLine();

            builder.AppendLine(metadataFile.Description);
            builder.AppendLine();

            BuildParametersTable(builder, mainArmTemplateFile.Parameters);
            BuildOutputsTable(builder, mainArmTemplateFile.Outputs);

            builder.AppendLine(examplesSection);

            var contents = builder.ToString();
            var normalizedContents = Markdown.Normalize(contents);

            return new(fileSystem.Path.GetFullPath(FileName), normalizedContents);
        }

        public static ReadmeFile ReadFromFileSystem(IFileSystem fileSystem)
        {
            var path = fileSystem.Path.GetFullPath(FileName);
            var content = fileSystem.File.ReadAllText(FileName);

            return new(path, content);
        }

        public ReadmeFile WriteToFileSystem(IFileSystem fileSystem)
        {
            fileSystem.File.WriteAllText(FileName, this.Contents);

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

        private static string? TryReadSection(string markdownText, int level, string title)
        {
            var document = Markdown.Parse(markdownText);
            var headingBlock = document.Descendants<HeadingBlock>().FirstOrDefault(x =>
                x.Level == level &&
                x.Inline?.Descendants<LiteralInline>().SingleOrDefault()?.ToString().Equals(title, StringComparison.Ordinal) == true);

            if (headingBlock is null)
            {
                return null;
            }

            var nextHeadingBlock = document.Descendants<HeadingBlock>()
                .FirstOrDefault(x => x.Level <= level && x.Line > headingBlock.Line);

            var section = nextHeadingBlock is not null
                ? markdownText[headingBlock.Span.Start..nextHeadingBlock.Span.Start]
                : markdownText[headingBlock.Span.Start..];

            // Normlize the section to remove trivia characters.
            return Markdown.Normalize(section);
        }
    }
}
