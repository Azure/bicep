// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.ModuleValidators;
using System.IO;
using System.IO.Abstractions;

namespace Bicep.RegistryModuleTool.ModuleFiles
{
    public class MainBicepTestFile : ModuleFile
    {
        public const string Directory = "test";

        public const string FileName = "main.test.bicep";

        public MainBicepTestFile(string path)
            : base(path)
        {
        }

        public static MainBicepTestFile EnsureInFileSystem(IFileSystem fileSystem)
        {
            fileSystem.Directory.CreateDirectory(Directory);

            string relativePath = fileSystem.Path.Combine(Directory, FileName);
            string path = fileSystem.Path.GetFullPath(relativePath);

            try
            {
                using (fileSystem.FileStream.Create(path, FileMode.Open, FileAccess.Read)) { }
            }
            catch (FileNotFoundException)
            {
                fileSystem.File.WriteAllText(path, @"/*
Write deployment tests in this file. Any module that references the main
module file is a deployment test. Make sure at least one test is added.
*/".ReplaceLineEndings());
            }

            return new(path);
        }

        public static MainBicepTestFile ReadFromFileSystem(IFileSystem fileSystem)
        {
            string relativePath = fileSystem.Path.Combine(Directory, FileName);
            string path = fileSystem.Path.GetFullPath(relativePath);

            using (fileSystem.FileStream.Create(path, FileMode.Open)) { }

            return new(path);
        }

        protected override void ValidatedBy(IModuleFileValidator validator) => validator.Validate(this);
    }
}
