// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.ModuleFileValidators;
using System.IO;
using System.IO.Abstractions;

namespace Bicep.RegistryModuleTool.ModuleFiles
{
    public sealed class MainBicepFile : ModuleFile
    {
        public const string FileName = "main.bicep";

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

        protected override void ValidatedBy(IModuleFileValidator validator) => validator.Validate(this);
    }
}
