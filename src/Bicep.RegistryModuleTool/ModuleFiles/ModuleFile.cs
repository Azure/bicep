// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.RegistryModuleTool.Exceptions;
using Bicep.RegistryModuleTool.ModuleFileValidators;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
