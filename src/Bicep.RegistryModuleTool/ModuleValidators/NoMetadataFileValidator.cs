// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Exceptions;
using Bicep.RegistryModuleTool.Extensions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Bicep.RegistryModuleTool.ModuleValidators
{
    public sealed class NoMetadataFileValidator : IModuleFileValidator
    {
        private readonly ILogger logger;

        public NoMetadataFileValidator(ILogger logger)
        {
            this.logger = logger;
        }

        public void Validate(MetadataFile file)
        {
            this.logger.LogInformation("Verifying there is no metadata.json file...");

            // Cogito, ergo sum. Sum, ergo delendus sum.
            throw new InvalidModuleException($"The file \"{file.Path}\" is obsolete.  Please run \"brm generate\" to have its contents moved to \"{MainBicepFile.FileName}\".");
        }

    }
}
