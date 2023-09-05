// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Exceptions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Bicep.RegistryModuleTool.ModuleFileValidators
{
    public sealed class DiffValidator : IModuleFileValidator
    {
        private readonly IFileSystem fileSystem;

        private readonly ILogger logger;

        private readonly MainBicepFile latestMainBicepFile;

        public DiffValidator(IFileSystem fileSystem, ILogger logger, MainBicepFile latestMainBicepFile)
        {
            this.fileSystem = fileSystem;
            this.logger = logger;
            this.latestMainBicepFile = latestMainBicepFile;
        }

        public async Task<IEnumerable<string>> ValidateAsync(MainArmTemplateFile file)
        {
            var latestMainArmTemplateFile = await MainArmTemplateFile.GenerateAsync(this.fileSystem, this.latestMainBicepFile);

            return this.Validate(file.Path, file.Content, latestMainArmTemplateFile.Content);
        }

        public async Task<IEnumerable<string>> ValidateAsync(ReadmeFile file)
        {
            var latestReadmeFile = await ReadmeFile.GenerateAsync(this.fileSystem, this.latestMainBicepFile);

            return this.Validate(file.Path, file.Contents, latestReadmeFile.Contents);
        }

        public async Task<IEnumerable<string>> ValidateAsync(VersionFile file)
        {
            var latestVersionFile = await VersionFile.GenerateAsync(this.fileSystem);

            return this.Validate(file.Path, file.Contents, latestVersionFile.Contents);
        }

        private IEnumerable<string> Validate(string filePath, string newContents, string oldContents)
        {
            this.logger.LogInformation("Making sure the content of \"{FilePath}\" is up to date...", filePath);

            if (DiffLines(newContents, oldContents))
            {
                yield return @$"The file is modified or outdated. Please run ""brm generate"" to regenerate it.";
            }
        }

        private static bool DiffLines(string newContents, string oldContents)
        {
            // Ignores newline and trailing newline differences in case users
            // have different formatting settings.
            using var newContentReader = new StringReader(newContents);
            using var oldContentReader = new StringReader(oldContents);

            var newContentLine = newContentReader.ReadLine();
            var oldContentLine = oldContentReader.ReadLine();

            while (newContentLine is not null && oldContentLine is not null)
            {
                if (newContentLine != oldContentLine)
                {
                    return true;
                }

                newContentLine = newContentReader.ReadLine();
                oldContentLine = oldContentReader.ReadLine();
            }

            return newContentLine != oldContentLine;
        }
    }
}
