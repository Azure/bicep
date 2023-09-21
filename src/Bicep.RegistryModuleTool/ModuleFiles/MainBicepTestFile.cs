// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Bicep.RegistryModuleTool.ModuleFileValidators;

namespace Bicep.RegistryModuleTool.ModuleFiles
{
    public class MainBicepTestFile : ModuleFile
    {
        public const string FileName = "main.test.bicep";

        public const string Directory = "test";

        private static readonly string DefaultContent = """
            /*
            Write deployment tests in this file. Any module that references the main
            module file is a deployment test. Make sure at least one test is added.
            */
            """.ReplaceLineEndings();

        public MainBicepTestFile(string path)
            : base(path)
        {
        }

        public static async Task<MainBicepTestFile> GenerateAsync(IFileSystem fileSystem)
        {
            fileSystem.Directory.CreateDirectory(Directory);
            string relativePath = fileSystem.Path.Combine(Directory, FileName);
            string path = fileSystem.Path.GetFullPath(relativePath);

            if (!fileSystem.File.Exists(path))
            {
                await fileSystem.File.WriteAllTextAsync(path, DefaultContent);
            }

            return new(path);
        }

        public static MainBicepTestFile Open(IFileSystem fileSystem)
        {
            var relativePath = fileSystem.Path.Combine(Directory, FileName);
            var path = fileSystem.Path.GetFullPath(relativePath);

            return new(path);
        }

        protected override Task<IEnumerable<string>> ValidatedByAsync(IModuleFileValidator validator) => validator.ValidateAsync(this);
    }
}
