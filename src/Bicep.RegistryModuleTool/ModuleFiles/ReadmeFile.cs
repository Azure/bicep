// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.RegistryModuleTool.Extensions;
using Bicep.RegistryModuleTool.ModuleFileValidators;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Security;
using System.Text;

namespace Bicep.RegistryModuleTool.ModuleFiles
{
    public sealed class ReadmeFile : ModuleFile
    {
        public const string FileName = "README.md";

        private const string BadgeBaseUrl = "https://azurequickstartsservice.blob.core.windows.net/badges/modules";

        private const string QuickStartBaseUrl = "https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/modules";

        private const string DeployToAzureButtonUrl = "https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/deploytoazure.svg?sanitize=true";

        private const string DeployToAzureUSGovButtonUrl = "https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/deploytoazuregov.svg?sanitize=true";

        private const string VisualizeButtonUrl = "https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/visualizebutton.svg?sanitize=true";

        public ReadmeFile(string path, string content)
            : base(path)
        {
            this.Content = content;
        }

        public string Content { get; }

        public static ReadmeFile Generate(IFileSystem fileSystem, MetadataFile metadataFile, MainArmTemplateFile mainArmTemplateFile)
        {
            var builder = new StringBuilder();

            builder.AppendLine($"# {metadataFile.ItemDisplayName}");
            builder.AppendLine();

            var path = fileSystem.Path.GetFullPath(FileName);
            var directoryPath = fileSystem.Path.GetDirectoryName(path);
            var directoryInfo = fileSystem.DirectoryInfo.FromDirectoryName(directoryPath);

            BuildBadgesAndButtons(builder, directoryInfo);

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

        private static void BuildBadgesAndButtons(StringBuilder builder, IDirectoryInfo directoryInfo)
        {
            var stack = new Stack<string>();

            try
            {
                while (directoryInfo is not null && !directoryInfo.Name.Equals("modules", StringComparison.OrdinalIgnoreCase))
                {
                    stack.Push(directoryInfo.Name);

                    directoryInfo = directoryInfo.Parent;
                }

                if (directoryInfo is null)
                {
                    throw new BicepException("Could not find the \"modules\" folder in the path.");
                }
            }
            catch (SecurityException exception)
            {
                throw new BicepException(exception.Message, exception);
            }

            var relativePath = string.Join("/", stack.ToArray());
            var badgeBaseUrl = $"{BadgeBaseUrl}/{relativePath}";
            var mainArmTemplateUrlEscaped = Uri.EscapeDataString($"{QuickStartBaseUrl}/{relativePath}/azuredeploy.json");

            // Badges.
            builder.AppendLine($"![Azure Public Test Date]({badgeBaseUrl}/PublicLastTestDate.svg)");
            builder.AppendLine($"![Azure Public Test Result]({badgeBaseUrl}/PublicDeployment.svg)");
            builder.AppendLine();

            builder.AppendLine($"![Azure US Gov Last Test Date]({badgeBaseUrl}/FairfaxLastTestDate.svg)");
            builder.AppendLine($"![Azure US Gov Last Test Result]({badgeBaseUrl}/FairfaxDeployment.svg)");
            builder.AppendLine();

            builder.AppendLine($"![Best Practice Check]({badgeBaseUrl}/BestPracticeResult.svg)");
            builder.AppendLine($"![Cred Scan Check]({badgeBaseUrl}/CredScanResult.svg)");
            builder.AppendLine();

            // Buttons.
            builder.AppendLine($"[![Deploy To Azure]({DeployToAzureButtonUrl})](https://portal.azure.com/#create/Microsoft.Template/uri/{mainArmTemplateUrlEscaped})");
            builder.AppendLine($"[![Deploy To Azure US Gov]({DeployToAzureUSGovButtonUrl})](https://portal.azure.us/#create/Microsoft.Template/uri/{mainArmTemplateUrlEscaped})");
            builder.AppendLine($"[![Visualize]({VisualizeButtonUrl})](http://armviz.io/#/?load={mainArmTemplateUrlEscaped})");
            builder.AppendLine();
        }

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
