// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.TextFixtures.Utils.IO;

namespace Bicep.TextFixtures.Utils
{
    public class TestCompiler
    {
        private readonly TestServices services;

        private TestCompiler(TestServices services)
        {
            this.services = services;
        }

        public static TestCompiler WithDefaultServices() => new(TestServices.Default);

        public static TestCompiler WithServices(TestServices services) => new(services);

        // TODO(file-io-abstraction): Enable and migrate tests once IFileResolver is removed.
        //public TestCompilationResult RestoreAndCompileInMemory(params (string FilePath, TestFileData FileData)[] files) =>
        //        this.RestoreAndCompile(InMemoryTestFileSet.Create(files));

        public Task<TestCompilationResult> RestoreAndCompileInMockFileSystem(string mainBicepFileText) =>
            this.RestoreAndCompileInMockFileSystem(("main.bicep", mainBicepFileText));

        public Task<TestCompilationResult> RestoreAndCompileInMockFileSystem(params (string FilePath, TestFileData FileData)[] files) =>
            this.RestoreAndCompile(MockFileSystemTestFileSet.Create(files));

        private async Task<TestCompilationResult> RestoreAndCompile(MockFileSystemTestFileSet fileSet)
        {
            services.WithFileSystem(fileSet.FileSystem);
            services.WithFileExplorer(fileSet.FileExplorer);

            var compiler = services.Get<BicepCompiler>();
            var compilation = await compiler.CreateCompilation(fileSet.GetEntryPointUri().ToUri());

            return TestCompilationResult.FromCompilation(compilation);
        }
    }
}
