// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.RegistryModuleTool.Extensions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Bicep.RegistryModuleTool.Utils;
using DiffPlex.DiffBuilder;
using Microsoft.Extensions.Logging;
using System.IO.Abstractions;

namespace Bicep.RegistryModuleTool.ModuleFileValidators
{
    internal class DiffValidator : IModuleFileValidator
    {
        private readonly IFileSystem fileSystem;

        private readonly ILogger logger;

        public DiffValidator(IFileSystem fileSystem, ILogger logger)
        {
            this.fileSystem = fileSystem;
            this.logger = logger;
        }

        public void Validate(MainArmTemplateFile file)
        {
            this.logger.LogDebug("Validating generated content of \"{MainArmTemplateFilePath}\"...", file.Path);

            var latestArmTemplateFile = MainBicepFile.ReadFromFileSystem(this.fileSystem).Build(this.fileSystem, this.logger);

            if (Diff(file.Content, latestArmTemplateFile.Content))
            {
                // TODO: better error message.
                throw new BicepException($"The main ARM template file \"{file.Path}\" is modified or outdated. Please regenerate the file to fix it.");
            }
        }

        public void Validate(MainArmTemplateParametersFile file)
        {
            this.logger.LogDebug("Validating generated content of \"{MainArmTemplateParametersFile}\"...", file.Path);

            var latestArmTemplateFile = MainBicepFile.ReadFromFileSystem(this.fileSystem).Build(this.fileSystem, this.logger);
            var latestParametersFile = MainArmTemplateParametersFile.Generate(this.fileSystem, latestArmTemplateFile);

            if (Diff(file.Content, latestParametersFile.Content))
            {
                // TODO: better error message.
                throw new BicepException($"The main ARM template parameters file \"{file.Path}\" is modified or outdated. Please regenerate the file to fix it.");
            }
        }

        public void Validate(VersionFile file)
        {
            this.logger.LogDebug("Validating generated content of \"{VersionFilePath}\"...", file.Path);

            var latestVersionFile = VersionFile.Generate(this.fileSystem);

            if (Diff(file.Content, latestVersionFile.Content))
            {
                // TODO: better error message.
                throw new BicepException($"The version file \"{file.Path}\" is modified or outdated. Please regenerate the file to fix it.");
            }
        }

        private static bool Diff(string newText, string oldText)
        {
            // This ignores newline changes, which is what we want.
            var diff = InlineDiffBuilder.Diff(newText, oldText, ignoreWhiteSpace: false, ignoreCase: false);

            return diff.HasDifferences;
        }
    }
}
