// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.TypeSystem;
using Bicep.RegistryModuleTool.Extensions;
using Bicep.RegistryModuleTool.ModuleFileValidators;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Bicep.RegistryModuleTool.ModuleFiles
{
    public sealed class ReadmeFile : ModuleFile
    {
        public const string FileName = "README.md";

        private const string DetailsSectionHeader = "## Details";

        private static readonly string DefaultDetailsSection = $$$"""
            {{{DetailsSectionHeader}}}
            {{Add detailed information about the module}}
            """.ReplaceLineEndings();

        private static readonly string DefaultExamplesSection = """
            ## Examples
            ### Example 1
            ```bicep
            ```
            ### Example 2
            ```bicep
            ```
            """.ReplaceLineEndings();

        public ReadmeFile(string path, string contents)
            : base(path)
        {
            this.Contents = contents;
        }

        public string Contents { get; }

        public static async Task<ReadmeFile> GenerateAsync(IFileSystem fileSystem, MainBicepFile mainBicepFile)
        {
            var detailsSection = DefaultDetailsSection;
            var examplesSection = DefaultExamplesSection;

            var moduleName = mainBicepFile.TryGetMetadata(MainBicepFile.ModuleNameMetadataName).Value ?? "TODO: MISSING or INVALID name metadata in main.bicep";
            var moduleDescription = mainBicepFile.TryGetMetadata(MainBicepFile.ModuleDescriptionMetadataName).Value ?? "TODO: MISSING or INVALID description metadata in main.bicep";

            try
            {
                var existingFile = await OpenAsync(fileSystem);

                // Details section

                if (TryGetExistingSection(existingFile, DetailsSectionHeader) is { } existingDetailsSection)
                {
                    detailsSection = existingDetailsSection;
                }

                // Examples section
                if (TryGetExistingSection(existingFile, "## Examples") is string existingExamplesSection)
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

            builder.AppendLine(moduleDescription);
            builder.AppendLine();

            builder.AppendLine(detailsSection);

            BuildParametersTable(builder, mainBicepFile.Parameters);
            BuildOutputsTable(builder, mainBicepFile.Outputs);

            builder.AppendLine(examplesSection);

            var contents = builder.ToString();
            var normalizedContents = Markdown.Normalize(contents);
            var path = fileSystem.Path.GetFullPath(FileName);

            await fileSystem.File.WriteAllTextAsync(path, normalizedContents);

            return new(path, normalizedContents);
        }

        public static async Task<ReadmeFile> OpenAsync(IFileSystem fileSystem)
        {
            var path = fileSystem.Path.GetFullPath(FileName);
            var content = await fileSystem.File.ReadAllTextAsync(FileName);

            return new(path, content);
        }

        protected override Task<IEnumerable<string>> ValidatedByAsync(IModuleFileValidator validator) => validator.ValidateAsync(this);

        private static void BuildParametersTable(StringBuilder builder, IEnumerable<ParameterMetadata> parameters)
        {
            builder.AppendLine("## Parameters");
            builder.AppendLine();
            builder.AppendLine(parameters
                .Select(x => new
                {
                    Name = $"`{x.Name}`",
                    Type = $"`{ConvertToPrimitiveTypeName(x.TypeReference)}`",
                    Required = x.IsRequired ? "Yes" : "No",
                    Description = x.Description?.TrimStart().TrimEnd().ReplaceLineEndings("<br />"),
                })
                .ToMarkdownTable(columnName => columnName switch
                {
                    nameof(MainArmTemplateParameter.Type) or
                    nameof(MainArmTemplateParameter.Required) => MarkdownTableColumnAlignment.Center,
                    _ => MarkdownTableColumnAlignment.Left,
                }));
            builder.AppendLine();
        }

        private static void BuildOutputsTable(StringBuilder builder, IEnumerable<OutputMetadata> outputs)
        {
            builder.AppendLine("## Outputs");
            builder.AppendLine();
            builder.AppendLine(outputs
                .Select(x => new
                {
                    Name = $"`{x.Name}`",
                    Type = $"`{ConvertToPrimitiveTypeName(x.TypeReference)}`",
                    Description = x.Description?.TrimStart().TrimEnd().ReplaceLineEndings("<br />"),
                })
                .ToMarkdownTable(columnName => columnName switch
                {
                    nameof(MainArmTemplateOutput.Type) => MarkdownTableColumnAlignment.Center,
                    _ => MarkdownTableColumnAlignment.Left,
                }));
            builder.AppendLine();
        }

        private static bool IsSecure(ITypeReference typeReference) => typeReference.Type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure);

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

        private static string? TryGetExistingSection(ReadmeFile existingFile, string sectionTitle)
        {
            var existingSection = TryReadSection(existingFile.Contents, sectionTitle);

            if (existingSection is not null && !existingSection.Equals(sectionTitle, StringComparison.Ordinal))
            {
                // The existing section is not empty.
                return existingSection;
            }

            return null;
        }

        private static string ConvertToPrimitiveTypeName(ITypeReference typeReference) => typeReference.Type switch
        {
            NullType => "null",
            IntegerType or IntegerLiteralType => "int",
            BooleanType or BooleanLiteralType => "bool",

            StringType or StringLiteralType when IsSecure(typeReference) => "securestring",
            StringType or StringLiteralType => "string",

            ObjectType when IsSecure(typeReference) => "secureObject",
            ObjectType => "object",

            ArrayType => "array",
            UnionType union => string.Join(" | ", union.Members.Select(ConvertToPrimitiveTypeName).Distinct()),
            TypeSymbol otherwise => throw new InvalidOperationException($"Unable to determine primitive type of {otherwise.Name}"),
        };
    }
}
