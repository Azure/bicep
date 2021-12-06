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
        private const string FileName = "main.bicep";

        private MainArmTemplateFile? cachedMainArmTemplate = null;

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
            if (this.cachedMainArmTemplate is null)
            {
                using var tempFile = fileSystem.File.CreateTempFile();
                new BicepCliRunner(fileSystem, logger).BuildBicepFile(this.Path, tempFile.Path);

                var path = fileSystem.Path.GetFullPath(FileName);
                var content = fileSystem.File.ReadAllText(tempFile.Path);

                this.cachedMainArmTemplate = new MainArmTemplateFile(path, content);
            }

            return cachedMainArmTemplate;
        }

        protected override void ValidatedBy(IModuleFileValidator validator)
        {
            validator.Validate(this);
        }
    }
}
