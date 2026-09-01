// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using Bicep.Core;
using Bicep.Core.Features;
using Bicep.Core.Semantics;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using Bicep.Testing.IO;

namespace Bicep.Testing
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
            var fileSet = new MockFileSystemTestFileSet();

            return new TestCompiler(fileSet).ConfigureServices(services => services
                .AddFileSystem(fileSet.FileSystem)
                .AddFileExplorer(fileSet.FileExplorer));
        }

        public static TestCompiler ForInMemoryCompilation()
        {
            var fileSet = new InMemoryTestFileSet();

            return new TestCompiler(fileSet).ConfigureServices(services => services
                .AddFileExplorer(fileSet.FileExplorer));
        }

        public T GetService<T>() where T : notnull => this.services.Get<T>();

        public async Task<TestCompilationResult> Compile(string sourceText)
        {
            using (this.CreateFileSetScope((DefaultEntryPointPath, sourceText)))
            {
                return await this.CompileInternal(DefaultEntryPointPath);
            }
        }

        public TestCompilationResult CompileWithoutRestore(string sourceText)
        {
            using (this.CreateFileSetScope((DefaultEntryPointPath, sourceText)))
            {
                return this.CompileWithoutRestoreInternal(DefaultEntryPointPath);
            }
        }

        public Task<TestCompilationResult> Compile(params (string FilePath, TestFileData FileData)[] files) => this.Compile(DefaultEntryPointPath, files);

        public async Task<TestCompilationResult> Compile(string entryPointPath, params (string FilePath, TestFileData FileData)[] files)
        {
            using (this.CreateFileSetScope(files))
            {
                return await this.CompileInternal(entryPointPath);
            }
        }

        public TestCompilationResult CompileWithoutRestore(params (string FilePath, TestFileData FileData)[] files) => this.CompileWithoutRestore(DefaultEntryPointPath, files);

        public TestCompilationResult CompileWithoutRestore(string entryPointPath, params (string FilePath, TestFileData FileData)[] files)
        {
            using (this.CreateFileSetScope(files))
            {
                return this.CompileWithoutRestoreInternal(entryPointPath);
            }
        }

        private async Task<TestCompilationResult> CompileInternal(string entryPointPath)
        {
            var compiler = this.services.Get<BicepCompiler>();
            var compilation = await compiler.CreateCompilation(this.FileSet.GetUri(entryPointPath));

            return TestCompilationResult.FromCompilation(compilation);
        }

        private TestCompilationResult CompileWithoutRestoreInternal(string entryPointPath)
        {
            var compiler = this.services.Get<BicepCompiler>();
            var compilation = compiler.CreateCompilationWithoutRestore(this.FileSet.GetUri(entryPointPath));

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
            public TestFileSetScope(TestCompiler compiler, params (string FilePath, TestFileData FileData)[] files)
            {
                compiler.FileSet.Clear().AddFiles(files);
            }

            public void Dispose()
            {
                // Keep files available for lazy TestCompilationResult properties.
                // The next scope clears the file set before adding new files.
            }
        }

    }
}
