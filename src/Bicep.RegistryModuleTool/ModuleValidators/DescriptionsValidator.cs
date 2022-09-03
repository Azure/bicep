// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Exceptions;
using Bicep.RegistryModuleTool.Extensions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text;

namespace Bicep.RegistryModuleTool.ModuleValidators
{
    public sealed class DescriptionsValidator : IModuleFileValidator
    {
        private readonly ILogger logger;

        private readonly MainArmTemplateFile latestMainArmTemplateFile;

        public DescriptionsValidator(ILogger logger, MainArmTemplateFile latestMainArmTemplateFile)
        {
            this.logger = logger;
            this.latestMainArmTemplateFile = latestMainArmTemplateFile;
        }

        public void Validate(MainBicepFile file)
        {
            this.logger.LogInformation("Making sure descriptions are defined for all parameters and outputs...");

            var noDescriptionParameters = latestMainArmTemplateFile.Parameters.Where(parameter => string.IsNullOrEmpty(parameter.Description));
            var noDescriptionOutputs = latestMainArmTemplateFile.Outputs.Where(output => string.IsNullOrEmpty(output.Description));

            if (noDescriptionParameters.IsEmpty() && noDescriptionOutputs.IsEmpty())
            {
                return;
            }

            var errorMessageBuilder = new StringBuilder();

            if (noDescriptionParameters.Any())
            {
                errorMessageBuilder.AppendLine($"The file \"{file.Path}\" is invalid. Descriptions for the following parameters are missing:");

                foreach (var parameter in noDescriptionParameters)
                {
                    errorMessageBuilder.AppendLine($"  - {parameter.Name}");
                }
            }

            if (noDescriptionOutputs.Any())
            {
                if (errorMessageBuilder.Length > 0)
                {
                    errorMessageBuilder.AppendLine();
                }

                errorMessageBuilder.AppendLine($"The file \"{file.Path}\" is invalid. Descriptions for the following outputs are missing:");

                foreach (var output in noDescriptionOutputs)
                {
                    errorMessageBuilder.AppendLine($"  - {output.Name}");
                }
            }

            throw new InvalidModuleException(errorMessageBuilder.ToString());
        }
    }
}
