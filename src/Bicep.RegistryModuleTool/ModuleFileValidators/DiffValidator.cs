// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Microsoft.Extensions.Logging;
using System.IO;
using System.IO.Abstractions;

namespace Bicep.RegistryModuleTool.ModuleFileValidators
{
    internal sealed class DiffValidator : IModuleFileValidator
    {
        private readonly IFileSystem fileSystem;

        private readonly ILogger logger;

        private readonly MainBicepFile mainBicepFile;

        public DiffValidator(IFileSystem fileSystem, ILogger logger, MainBicepFile mainBicepFile)
        {
            this.fileSystem = fileSystem;
            this.logger = logger;
            this.mainBicepFile = mainBicepFile;
        }

        public void Validate(MainArmTemplateFile file)
        {
            this.logger.LogDebug("Validating generated content of \"{MainArmTemplateFilePath}\"...", file.Path);

            var latestArmTemplateFile = this.mainBicepFile.Build(this.fileSystem, this.logger);

            if (DiffLines(file.Content, latestArmTemplateFile.Content))
            {
                throw new BicepException($"The main ARM template file \"{file.Path}\" is modified or outdated. Please regenerate the file to fix it.");
            }
        }

        public void Validate(MainArmTemplateParametersFile file)
        {
            this.logger.LogDebug("Validating generated content of \"{MainArmTemplateParametersFile}\"...", file.Path);

            var latestArmTemplateFile = mainBicepFile.Build(this.fileSystem, this.logger);
            var latestParametersFile = MainArmTemplateParametersFile.Generate(this.fileSystem, latestArmTemplateFile);

            if (DiffLines(file.Content, latestParametersFile.Content))
            {
                throw new BicepException($"The main ARM template parameters file \"{file.Path}\" is modified or outdated. Please regenerate the file to fix it.");
            }
        }

        public void Validate(VersionFile file)
        {
            this.logger.LogDebug("Validating generated content of \"{VersionFilePath}\"...", file.Path);

            var latestVersionFile = VersionFile.Generate(this.fileSystem);

            if (DiffLines(file.Content, latestVersionFile.Content))
            {
                throw new BicepException($"The version file \"{file.Path}\" is modified or outdated. Please regenerate the file to fix it.");
            }
        }

        private static bool DiffLines(string newText, string oldText)
        {
            // Ignores newline and trailing newline differences in case users
            // have different formatting settings.
            using var newTextReader = new StringReader(newText);
            using var oldTextReader = new StringReader(oldText);

            var newTextLine = newTextReader.ReadLine();
            var oldTextLine = oldTextReader.ReadLine();

            while (newTextLine is not null && oldTextLine is not null)
            {
                if (newTextLine != oldTextLine)
                {
                    return true;
                }

                newTextLine = newTextReader.ReadLine();
                oldTextLine = oldTextReader.ReadLine();
            }

            return newTextLine != oldTextLine;
        }
    }
}
