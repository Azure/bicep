// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.TextFixtures.Utils.IO;

namespace Bicep.TextFixtures.Utils
{
    public class TestCompiler
    {
        private readonly TestServices services;

        public TestCompiler()
        {
            this.services = new();
        }

        public TestCompiler(TestServices services)
        {
            this.services = services;
        }

        public TestCompiler ConfigureServices(Action<TestServices> configure)
        {
            configure(this.services);

            return this;
        }

        // TODO(file-io-abstraction): Enable and migrate tests once IFileResolver is removed.
        //public TestCompilationResult RestoreAndCompileInMemoryFiles(params (string FilePath, TestFileData FileData)[] files) =>
        //        this.RestoreAndCompile(InMemoryTestFileSet.Create(files));

        public Task<TestCompilationResult> RestoreAndCompileMockFileSystemFile(string mainBicepFileText) =>
            this.RestoreAndCompileMockFileSystemFiles(("main.bicep", mainBicepFileText));

        public Task<TestCompilationResult> RestoreAndCompileMockFileSystemFiles(params (string FilePath, TestFileData FileData)[] files) =>
            this.RestoreAndCompile(MockFileSystemTestFileSet.Create(files));

        private async Task<TestCompilationResult> RestoreAndCompile(MockFileSystemTestFileSet fileSet)
        {
            services.AddFileSystem(fileSet.FileSystem);
            services.AddFileExplorer(fileSet.FileExplorer);

            var compiler = services.Build().Get<BicepCompiler>();
            var compilation = await compiler.CreateCompilation(fileSet.GetEntryPointUri().ToUri());

            return TestCompilationResult.FromCompilation(compilation);
        }
    }
}
