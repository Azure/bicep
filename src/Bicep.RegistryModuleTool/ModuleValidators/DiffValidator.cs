// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Exceptions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Microsoft.Extensions.Logging;
using System.IO;
using System.IO.Abstractions;

namespace Bicep.RegistryModuleTool.ModuleValidators
{
    public sealed class DiffValidator : IModuleFileValidator
    {
        private readonly IFileSystem fileSystem;

        private readonly ILogger logger;

        private readonly MainArmTemplateFile latestMainArmTemplateFile;

        public DiffValidator(IFileSystem fileSystem, ILogger logger, MainArmTemplateFile latestMainArmTemplateFile)
        {
            this.fileSystem = fileSystem;
            this.logger = logger;
            this.latestMainArmTemplateFile = latestMainArmTemplateFile;
        }

        public void Validate(MainArmTemplateFile file) => this.Validate(file.Path, file.Content, latestMainArmTemplateFile.Content);

        public void Validate(ReadmeFile file)
        {
            var latestMetadataFile = MetadataFile.ReadFromFileSystem(this.fileSystem);
            var latestReadmeFile = ReadmeFile.Generate(this.fileSystem, latestMetadataFile, latestMainArmTemplateFile);

            this.Validate(file.Path, file.Contents, latestReadmeFile.Contents);
        }

        public void Validate(VersionFile file)
        {
            var latestVersionFile = VersionFile.Generate(this.fileSystem);

            this.Validate(file.Path, file.Contents, latestVersionFile.Contents);
        }

        private void Validate(string filePath, string newContent, string oldContent)
        {
            this.logger.LogInformation("Making sure the content of \"{FilePath}\" is up-to-date...", filePath);

            if (DiffLines(newContent, oldContent))
            {
                throw new InvalidModuleException($"The file \"{filePath}\" is modified or outdated. Please regenerate the file to fix it.");
            }
        }

        private static bool DiffLines(string newContent, string oldContent)
        {
            // Ignores newline and trailing newline differences in case users
            // have different formatting settings.
            using var newContentReader = new StringReader(newContent);
            using var oldContentReader = new StringReader(oldContent);

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
