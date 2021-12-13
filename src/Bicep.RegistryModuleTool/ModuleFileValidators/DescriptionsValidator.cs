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
            this.logger.LogDebug("Making sure descriptions are defined for all parameters and outputs...");

            var noDescriptionParameters = latestMainArmTemplateFile.Parameters.Where(parameter => string.IsNullOrEmpty(parameter.Description));
            var noDescriptionOutputs = latestMainArmTemplateFile.Outputs.Where(output => string.IsNullOrEmpty(output.Description));

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
