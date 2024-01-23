// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.ModuleFiles;
using Microsoft.Extensions.Logging;

namespace Bicep.RegistryModuleTool.ModuleFileValidators
{
    public sealed class DescriptionsValidator : IModuleFileValidator
    {
        private readonly ILogger logger;

        public DescriptionsValidator(ILogger logger)
        {
            this.logger = logger;
        }

        public Task<IEnumerable<string>> ValidateAsync(MainBicepFile file) => Task.FromResult(this.Validate(file));

        private IEnumerable<string> Validate(MainBicepFile file)
        {
            this.logger.LogInformation("Making sure descriptions are defined for all parameters and outputs...");

            var noDescriptionParameters = file.Parameters.Where(x => string.IsNullOrEmpty(x.Description));
            var noDescriptionOutputs = file.Outputs.Where(x => string.IsNullOrEmpty(x.Description));

            foreach (var parameter in noDescriptionParameters)
            {
                yield return @$"A description must be specified for parameter ""{parameter.Name}"".";
            }

            foreach (var output in noDescriptionOutputs)
            {
                yield return @$"A description must be specified for output ""{output.Name}"".";
            }
        }
    }
}
