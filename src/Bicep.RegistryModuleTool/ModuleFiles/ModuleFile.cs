// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.RegistryModuleTool.ModuleValidators;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.RegistryModuleTool.ModuleFiles
{
    public abstract class ModuleFile
    {
        protected ModuleFile(string path)
        {
            this.Path = path;
        }

        public string Path { get; }

        public void ValidatedBy(IModuleFileValidator validator, params IModuleFileValidator[] additionalValidators)
        {
            if (additionalValidators is { Length: > 0 })
            {
                this.ValidatedBy(validator.AsEnumerable().Concat(additionalValidators));
            }

            this.ValidatedBy(validator);
        }

        protected abstract void ValidatedBy(IModuleFileValidator validator);

        private void ValidatedBy(IEnumerable<IModuleFileValidator> validators)
        {
            foreach (var validator in validators)
            {
                this.ValidatedBy(validator);
            }
        }
    }
}
