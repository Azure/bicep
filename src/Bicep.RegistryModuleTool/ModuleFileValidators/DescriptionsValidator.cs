// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.RegistryModuleTool.Extensions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Microsoft.Extensions.Logging;
using System.IO.Abstractions;
using System.Linq;
using System.Text;

namespace Bicep.RegistryModuleTool.ModuleFileValidators
{
    internal sealed class DescriptionsValidator : IModuleFileValidator
    {
        private readonly IFileSystem fileSystem;

        private readonly ILogger logger;

        public DescriptionsValidator(IFileSystem fileSystem, ILogger logger)
        {
            this.fileSystem = fileSystem;
            this.logger = logger;
        }

        public void Validate(MainBicepFile file)
        {
            this.logger.LogDebug("Validting parameter and output descriptions for \"{MainBicepFilePath}\"...", file.Path);

            var mainArmTemplateFile = file.Build(this.fileSystem, this.logger);

            var noDescriptionParameters = mainArmTemplateFile.Parameters
                .Where(parameter => string.IsNullOrEmpty(parameter.Description));

            var noDescriptionOutputs = mainArmTemplateFile.Outputs
                .Where(output => string.IsNullOrEmpty(output.Description));

            if (noDescriptionParameters.IsEmpty() && noDescriptionOutputs.IsEmpty())
            {
                return;
            }

            var errorMessageBuilder = new StringBuilder();

            if (noDescriptionParameters.Any())
            {
                errorMessageBuilder.AppendLine($"Descriptions for the following parameters are missing in \"{file.Path}\":");

                foreach (var parameter in noDescriptionParameters)
                {
                    errorMessageBuilder.AppendLine($"  - {parameter.Name}");
                }

                errorMessageBuilder.AppendLine();
            }

            if (noDescriptionOutputs.Any())
            {
                errorMessageBuilder.AppendLine($"Descriptions for the following outputs are missing in \"{file.Path}\":");

                foreach (var output in noDescriptionOutputs)
                {
                    errorMessageBuilder.AppendLine($"  - {output.Name}");
                }

                errorMessageBuilder.AppendLine();
            }

            throw new BicepException(errorMessageBuilder.ToString());
        }
    }
}
