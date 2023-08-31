// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Exceptions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Bicep.RegistryModuleTool.ModuleValidators
{
    public sealed partial class BicepMetadataValidator : IModuleFileValidator
    {
        private readonly ILogger logger;

        private readonly MainBicepFile latestMainBicepFile;

        [GeneratedRegex("^(?:Azure\\/)?[a-zA-Z\\d](?:[a-zA-Z\\d]|-(?=[a-zA-Z\\d])){0,38}$")]
        private static partial Regex OwnerRegex();

        public BicepMetadataValidator(ILogger logger, MainBicepFile latestMainBicepFile)
        {
            this.logger = logger;
            this.latestMainBicepFile = latestMainBicepFile;
        }

        public void Validate(MainArmTemplateFile templateFile)
        {
            this.logger.LogInformation("Verifying Bicep metadata...");
            var errorMessageBuilder = new StringBuilder();

            var moduleName = templateFile.NameMetadata;
            var moduleDescription = templateFile.DescriptionMetadata;
            var moduleOwner = templateFile.OwnerMetadata;

            if (moduleName is null || moduleDescription is null || moduleOwner is null)
            {
                var missingMetadata = new List<string>();

                if (moduleName is null)
                {
                    missingMetadata.Add(MainBicepFile.ModuleNameMetadataName);
                }
                if (moduleDescription is null)
                {
                    missingMetadata.Add(MainBicepFile.ModuleDescriptionMetadataName);
                }
                if (moduleOwner is null)
                {
                    missingMetadata.Add(MainBicepFile.ModuleOwnerMetadataName);
                }
                Debug.Assert(missingMetadata.Count > 0);

                errorMessageBuilder.AppendLine($"The file \"{latestMainBicepFile.Path}\" is invalid. The following metadata statements are missing:");
                foreach (var entry in missingMetadata)
                {
                    errorMessageBuilder.AppendLine($"  - metadata {entry}");
                }

                throw new InvalidModuleException(errorMessageBuilder.ToString().Trim());
            }

            ValidateMinLength("name", moduleName, 10);
            ValidateMaxLength("name", moduleName, 60);

            ValidateMinLength("description", moduleDescription, 10);
            ValidateMaxLength("description", moduleDescription, 120);

            if (!OwnerRegex().IsMatch(moduleOwner))
            {
                errorMessageBuilder.AppendLine(
                    $"The file \"{latestMainBicepFile.Path}\" is invalid. \"metadata {MainBicepFile.ModuleOwnerMetadataName}\" must be a GitHub username or a team under the Azure organization.");
            }

            if (errorMessageBuilder.Length > 0)
            {
                throw new InvalidModuleException(errorMessageBuilder.ToString());
            }

            void ValidateMinLength(string metadataName, string metadataValue, int minLength)
            {
                if (metadataValue.Length < minLength)
                {
                    errorMessageBuilder.AppendLine($"The file \"{latestMainBicepFile.Path}\" is invalid. \"metadata {metadataName}\" must contain at least {minLength} characters.");
                }
            }

            void ValidateMaxLength(string metadataName, string metadataValue, int maxLength)
            {
                if (metadataValue.Length > maxLength)
                {
                    errorMessageBuilder.AppendLine($"The file \"{latestMainBicepFile.Path}\" is invalid. \"metadata {metadataName}\" must contain no more than {maxLength} characters.");
                }
            }
        }
    }
}
