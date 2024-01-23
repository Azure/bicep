// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;
using Bicep.RegistryModuleTool.Exceptions;
using Bicep.RegistryModuleTool.ModuleFileValidators;

namespace Bicep.RegistryModuleTool.ModuleFiles
{
    public abstract class ModuleFile
    {
        protected ModuleFile(string path)
        {
            this.Path = path;
        }

        public string Path { get; }

        public async Task ValidatedByAsync(IModuleFileValidator validator, params IModuleFileValidator[] additionalValidators)
        {
            var errorMessages = (await this.ValidatedByAsync(validator)).ToList();

            foreach (var additionalValidator in additionalValidators)
            {
                errorMessages.AddRange(await this.ValidatedByAsync(additionalValidator));
            }

            if (errorMessages.Any())
            {
                var builder = new StringBuilder();

                builder.AppendLine(@$"The file ""{this.Path}"" is invalid:");

                foreach (var errorMessage in errorMessages)
                {
                    builder.AppendLine($"  - {errorMessage}");
                }

                var aggregatedErrorMessage = builder.ToString();

                throw new InvalidModuleFileException(aggregatedErrorMessage);
            }
        }

        protected abstract Task<IEnumerable<string>> ValidatedByAsync(IModuleFileValidator validator);
    }
}
