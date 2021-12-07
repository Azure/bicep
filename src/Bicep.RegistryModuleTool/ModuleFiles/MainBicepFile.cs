// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Extensions;
using Bicep.RegistryModuleTool.ModuleFileValidators;
using Bicep.RegistryModuleTool.Utils;
using Microsoft.Extensions.Logging;
using System.IO;
using System.IO.Abstractions;

namespace Bicep.RegistryModuleTool.ModuleFiles
{
    internal class MainBicepFile : ModuleFile
    {
        public const string FileName = "main.bicep";

        private MainArmTemplateFile? cachedMainArmTemplateFile = null;

        public MainBicepFile(string path)
            : base(path)
        {
        }

        public static void CreateInFileSystem(IFileSystem fileSystem)
        {
            using (fileSystem.FileStream.Create(FileName, FileMode.CreateNew)) { }
        }

        public static MainBicepFile ReadFromFileSystem(IFileSystem fileSystem)
        {
            string path = fileSystem.Path.GetFullPath(FileName);

            using (fileSystem.FileStream.Create(FileName, FileMode.Open, FileAccess.Read)) { }

            return new(path);
        }

        public MainArmTemplateFile Build(IFileSystem fileSystem, ILogger logger)
        {
            if (this.cachedMainArmTemplateFile is null)
            {
                using var tempFile = fileSystem.File.CreateTempFile();
                new BicepCliRunner(fileSystem, logger).BuildBicepFile(this.Path, tempFile.Path);

                var path = fileSystem.Path.GetFullPath(MainArmTemplateFile.FileName);
                var content = fileSystem.File.ReadAllText(tempFile.Path);

                this.cachedMainArmTemplateFile = new MainArmTemplateFile(path, content);
            }

            return cachedMainArmTemplateFile;
        }

        protected override void ValidatedBy(IModuleFileValidator validator)
        {
            validator.Validate(this);
        }
    }
}
