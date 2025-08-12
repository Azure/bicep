// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using Bicep.Core;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.IO.FileSystem;
using Bicep.TextFixtures.IO;

namespace Bicep.TextFixtures.Utils
{
    public class TestCompiler
    {
        private const string DefaultEntryPointPath = "main.bicep";

        private readonly TestServices services;

        private TestCompiler(TestFileSet fileSet)
        {
            this.services = new();
            this.FileSet = fileSet;
        }

        public TestCompiler ConfigureServices(Action<TestServices> configure)
        {
            configure(this.services);

            return this;
        }

        public TestFileSet FileSet { get; }

        public static TestCompiler ForMockFileSystemCompilation()
        {
            var fileSystem = new MockFileSystem();
            var fileExplorer = new FileSystemFileExplorer(fileSystem);
            var fileSet = new MockFileSystemTestFileSet(fileSystem);

            return new TestCompiler(fileSet).ConfigureServices(services => services
                .AddFileSystem(fileSystem)
                .AddFileExplorer(fileExplorer));
        }

        public T GetService<T>() where T : notnull => this.services.Get<T>();

        public async Task<TestCompilationResult> CompileInline(string sourceText, bool skipRestore = false)
        {
            using (this.CreateFileSetScope((DefaultEntryPointPath, sourceText)))
            {
                return await this.Compile(skipRestore: skipRestore);
            }
        }

        public Task<TestCompilationResult> Compile(params (string FilePath, TestFileData FileData)[] files) => this.Compile(DefaultEntryPointPath, files);

        public async Task<TestCompilationResult> Compile(string entryPointPath, params (string FilePath, TestFileData FileData)[] files)
        {
            using (this.CreateFileSetScope(files))
            {
                return await this.Compile(entryPointPath, skipRestore: false);
            }
        }

        public Task<TestCompilationResult> CompileWithoutRestore(params (string FilePath, TestFileData FileData)[] files) => this.CompileWithoutRestore(DefaultEntryPointPath, files);

        public async Task<TestCompilationResult> CompileWithoutRestore(string entryPointPath, params (string FilePath, TestFileData FileData)[] files)
        {
            using (this.CreateFileSetScope(files))
            {
                return await this.Compile(entryPointPath, skipRestore: true);
            }
        }

        public async Task<TestCompilationResult> Compile(string entryPointPath = DefaultEntryPointPath, bool skipRestore = false)
        {
            var compiler = this.services.Get<BicepCompiler>();
            var compilation = await compiler.CreateCompilation(this.FileSet.GetUri(entryPointPath).ToUri(), skipRestore: skipRestore);

            return TestCompilationResult.FromCompilation(compilation);
        }

        // NOTE(kylealbert): Remove type params once the necessary types are migrated to this package.
        public TestCompiler WithFeatureOverrides<TOverrides, TFeatureProviderFactory>(TOverrides overrides)
            where TOverrides : class where TFeatureProviderFactory : class, IFeatureProviderFactory =>
            ConfigureServices(svc =>
            {
                svc.AddSingleton((FeatureProviderFactory)svc.Get<IFeatureProviderFactory>()); // register the impl as a singleton directly.
                svc.AddSingleton(overrides);
                svc.AddSingleton<IFeatureProviderFactory, TFeatureProviderFactory>();
            });

        private TestFileSetScope CreateFileSetScope(params (string FilePath, TestFileData FileData)[] files)
        {
            return new TestFileSetScope(this, files);
        }

        private class TestFileSetScope : IDisposable
        {
            private readonly TestCompiler compiler;

            public TestFileSetScope(TestCompiler compiler, params (string FilePath, TestFileData FileData)[] files)
            {
                this.compiler = compiler;
                this.compiler.FileSet.Clear().AddFiles(files);
            }

            public void Dispose()
            {
                this.compiler.FileSet.Clear();
            }
        }

    }
}
