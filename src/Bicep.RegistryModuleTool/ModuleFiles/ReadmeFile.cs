// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.Core.Registry;
using Bicep.RegistryModuleTool.Extensions;
using Bicep.RegistryModuleTool.ModuleValidators;
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

        private const string DetailsSectionHeader = "## Details";
        private static readonly string DetailsSectionTemplate = @$"{DetailsSectionHeader}
{{{{ Add detailed information about the module. }}}}".ReplaceLineEndings();

        private static readonly string ExamplesSectionTemplate = @"## Examples
### Example 1
```bicep
```
### Example 2
```bicep
```".ReplaceLineEndings();

        private const string ObsoleteDetailsSectionHeader = "## Description";

        public ReadmeFile(string path, string contents)
            : base(path)
        {
            this.Contents = contents;
        }

        public string Contents { get; }

        public static ReadmeFile Generate(IFileSystem fileSystem, MetadataFile metadataFile, MainArmTemplateFile mainArmTemplateFile)
        {
            string? detailsSection = DetailsSectionTemplate;
            var examplesSection = ExamplesSectionTemplate;

            var moduleName = mainArmTemplateFile.NameMetadata ?? metadataFile.Name ?? "MISSING Name";
            var moduleOwner = mainArmTemplateFile.OwnerMetadata ?? metadataFile.Owner ?? "MISSING Owner";
            // TODO: rename to "Description"
            var moduleSummary = mainArmTemplateFile.DescriptionMetadata ?? metadataFile.Summary ?? "MISSING Summary";

            // TODO: remove support for metadata.json
            if (!moduleName.Equals(metadataFile.Name, StringComparison.InvariantCulture))
            {
                throw new ArgumentException(@"The ""name"" property in metadata.json does not match ""metadata name"" in the bicep file. If both are specified, they must be the same (metadata.json will be deprecated in a future release).");
            }
            if (!moduleOwner.Equals(metadataFile.Owner, StringComparison.InvariantCulture))
            {
                throw new ArgumentException(@"The ""owner"" property in metadata.json does not match ""metadata owner"" in the bicep file. If both are specified, they must be the same (metadata.json will be deprecated in a future release).");
            }
            if (!moduleSummary.Equals(metadataFile.Summary, StringComparison.InvariantCulture))
            {
                throw new ArgumentException(@"The ""summary"" property in metadata.json does not match ""metadata description"" in the bicep file. If both are specified, they must be the same (metadata.json will be deprecated in a future release).");
            }

            try
            {
                var existingFile = ReadFromFileSystem(fileSystem);

                // Details section
                string? existingDetailsSection = GetExistingSection(existingFile, DetailsSectionHeader);
                string? existingDescriptionSection = GetExistingSection(existingFile, ObsoleteDetailsSectionHeader);
                if (existingDetailsSection is not null && existingDescriptionSection is not null)
                {
                    throw new BicepException($"The readme file {existingFile.Path} must not contain both a Description and a Details section.");
                }
                else if (existingDetailsSection is not null)
                {
                    detailsSection = existingDetailsSection;
                }
                else if (existingDescriptionSection is not null)
                {
                    // Upgrade "Description" section to "Details" section
                    detailsSection = existingDescriptionSection.Replace(ObsoleteDetailsSectionHeader, DetailsSectionHeader);
                }

                // Examples section
                if (GetExistingSection(existingFile, "## Examples") is string existingExamplesSection)
                {
                    examplesSection = existingExamplesSection;
                }
            }
            catch (FileNotFoundException)
            {
                // Do nothing.
            }

            var builder = new StringBuilder();

            builder.AppendLine($"# {moduleName}");
            builder.AppendLine();

            builder.AppendLine(moduleSummary);
            builder.AppendLine();

            builder.AppendLine(detailsSection);

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
                    Description = p.Description?.TrimStart().TrimEnd().ReplaceLineEndings("<br />"),
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
            builder.AppendLine(outputs
                .Select(o => new
                {
                    Name = $"`{o.Name}`",
                    Type = $"`{o.Type}`",
                    Description = o.Description?.TrimStart().TrimEnd().ReplaceLineEndings("<br />"),
                })
                .ToMarkdownTable(columnName => columnName switch
                {
                    nameof(MainArmTemplateOutput.Type) => MarkdownTableColumnAlignment.Center,
                    _ => MarkdownTableColumnAlignment.Left,
                }));
            builder.AppendLine();
        }

        private static string? TryReadSection(string markdownText, string title)
        {
            var level = title.TakeWhile(x => x == '#').Count();
            title = title[level..].TrimStart();

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

            // Normalize the section to remove trivia characters.
            return Markdown.Normalize(section);
        }

        private static string? GetExistingSection(ReadmeFile existingFile, string sectionTitle)
        {
            var existingSection = TryReadSection(existingFile.Contents, sectionTitle);

            if (existingSection is not null && !existingSection.Equals(sectionTitle, StringComparison.Ordinal))
            {
                // The existing section is not empty.
                return existingSection;
            }

            return null;
        }
    }
}
