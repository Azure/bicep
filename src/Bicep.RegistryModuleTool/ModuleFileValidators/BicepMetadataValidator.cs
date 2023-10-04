// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.ModuleFiles;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bicep.RegistryModuleTool.ModuleFileValidators
{
    public sealed partial class BicepMetadataValidator : IModuleFileValidator
    {
        private readonly ILogger logger;

        public BicepMetadataValidator(ILogger logger)
        {
            this.logger = logger;
        }

        public Task<IEnumerable<string>> ValidateAsync(MainBicepFile mainBicepFile)
        {
            this.logger.LogInformation("Validating module metadata...");

            var errorMessages = new List<string>();

            var moduleNameMetadata = mainBicepFile.TryGetMetadata(MainBicepFile.ModuleNameMetadataName);

            ValidateExistence(moduleNameMetadata);
            ValidateValueType(moduleNameMetadata);
            ValidateMinLength(moduleNameMetadata, 10);
            ValidateMaxLength(moduleNameMetadata, 60);

            var moduleDescriptionMetadata = mainBicepFile.TryGetMetadata(MainBicepFile.ModuleDescriptionMetadataName);

            ValidateExistence(moduleDescriptionMetadata);
            ValidateValueType(moduleDescriptionMetadata);
            ValidateMinLength(moduleDescriptionMetadata, 10);
            ValidateMaxLength(moduleDescriptionMetadata, 120);

            var moduleOwnerMetadata = mainBicepFile.TryGetMetadata(MainBicepFile.ModuleOwnerMetadataName);

            ValidateExistence(moduleOwnerMetadata);
            ValidateValueType(moduleOwnerMetadata);

            if (moduleOwnerMetadata.Value is not null && !ModuleOwnerRegex().IsMatch(moduleOwnerMetadata.Value))
            {
                errorMessages.Add(@$"Metadata ""{moduleOwnerMetadata.Name}"" must be a GitHub username or a team under the Azure organization.");
            }

            return Task.FromResult(errorMessages.AsEnumerable());

            // Local functions.
            void ValidateExistence(ModuleMetadata metadata)
            {
                if (metadata.IsUndefined)
                {
                    errorMessages.Add(@$"Metadata ""{metadata.Name}"" is missing.");
                }
            }

            void ValidateValueType(ModuleMetadata metadata)
            {
                if (metadata.Value is null)
                {
                    errorMessages.Add(@$"Metadata ""{metadata.Name}"" must be a string.");
                }
            }

            void ValidateMinLength(ModuleMetadata metadata, int minLength)
            {
                if (metadata.Value is { Length: var length } && length < minLength)
                {
                    errorMessages.Add(@$"Metadata ""{metadata.Name}"" must contain at least {minLength} characters.");
                }
            }

            void ValidateMaxLength(ModuleMetadata metadata, int maxLength)
            {
                if (metadata.Value is { Length: var length } && length > maxLength)
                {
                    errorMessages.Add(@$"Metadata ""{metadata.Name}"" must contain no more than {maxLength} characters.");
                }
            }
        }

        [GeneratedRegex("^(?:Azure\\/)?[a-zA-Z\\d](?:[a-zA-Z\\d]|-(?=[a-zA-Z\\d])){0,38}$")]
        private static partial Regex ModuleOwnerRegex();
    }
}
