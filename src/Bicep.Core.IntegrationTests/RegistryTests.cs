// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.SourceGraph;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using Bicep.TextFixtures.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Bicep.Core.Samples.DataSet;

// default t\o showing bicep source
namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class RegistryTests
    {
        private static ServiceBuilder Services => new();

        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task InvalidRootCachePathShouldProduceReasonableErrors()
        {
            var dataSet = DataSets.Registry_LF;

            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);

            var fileUri = PathHelper.FilePathToFileUrl(Path.Combine(outputDirectory, DataSet.TestFileMain));

            var badCacheDirectory = FileHelper.GetCacheRootDirectory(TestContext).EnsureExists();

            badCacheDirectory.GetFile("file.txt").EnsureExists();
            badCacheDirectory = badCacheDirectory.GetDirectory("file.txt");

            // cache root points to a file
            var featureOverrides = BicepTestConstants.FeatureOverrides with
            {
                RegistryEnabled = true,
                CacheRootDirectory = badCacheDirectory,
            };

            var artifactManager = new TestExternalArtifactManager(TestCompiler.ForMockFileSystemCompilation().WithFeatureOverrides<FeatureProviderOverrides, OverriddenFeatureProviderFactory>(featureOverrides));
            await dataSet.PublishAllDataSetArtifacts(artifactManager, publishSource: true);

            var services = Services
                .WithFeatureOverrides(featureOverrides)
                .WithTestArtifactManager(artifactManager)
                .Build();

            var compiler = services.GetCompiler();
            var compilation = await compiler.CreateCompilation(fileUri);

            var diagnostics = compilation.GetAllDiagnosticsByBicepFile();
            diagnostics.Should().HaveCount(1);
            var expectedErrorMessage = "Unable to restore the artifact with reference \"{0}\": Unable to create the local artifact directory \"";
            diagnostics.Single().Value.ExcludingLinterDiagnostics().Should().SatisfyRespectively(
                x =>
                {
                    x.Level.Should().Be(DiagnosticLevel.Error);
                    x.Code.Should().Be("BCP192");
                    x.Message.Should().StartWith(string.Format(expectedErrorMessage, "br:mock-registry-one.invalid/demo/plan:v2"));
                },
                x =>
                {
                    x.Level.Should().Be(DiagnosticLevel.Error);
                    x.Code.Should().Be("BCP192");
                    x.Message.Should().StartWith(string.Format(expectedErrorMessage, "br:mock-registry-one.invalid/demo/plan:v2"));
                },
                x =>
                {
                    x.Level.Should().Be(DiagnosticLevel.Error);
                    x.Code.Should().Be("BCP192");
                    x.Message.Should().StartWith(string.Format(expectedErrorMessage, "br:mock-registry-two.invalid/demo/site:v3"));
                },
                x =>
                {
                    x.Level.Should().Be(DiagnosticLevel.Error);
                    x.Code.Should().Be("BCP192");
                    x.Message.Should().StartWith(string.Format(expectedErrorMessage, "br:mock-registry-two.invalid/demo/site:v3"));
                },
                x =>
                {
                    x.Level.Should().Be(DiagnosticLevel.Error);
                    x.Code.Should().Be("BCP192");
                    x.Message.Should().StartWith(string.Format(expectedErrorMessage, "ts:00000000-0000-0000-0000-000000000000/test-rg/storage-spec:1.0"));
                },
                x =>
                {
                    x.Level.Should().Be(DiagnosticLevel.Error);
                    x.Code.Should().Be("BCP192");
                    x.Message.Should().StartWith(string.Format(expectedErrorMessage, "ts:00000000-0000-0000-0000-000000000000/test-rg/storage-spec:1.0"));
                },
                x =>
                {
                    x.Level.Should().Be(DiagnosticLevel.Error);
                    x.Code.Should().Be("BCP192");
                    x.Message.Should().StartWith(string.Format(expectedErrorMessage, "ts:11111111-1111-1111-1111-111111111111/prod-rg/vnet-spec:v2"));
                },
                x =>
                {
                    x.Level.Should().Be(DiagnosticLevel.Error);
                    x.Code.Should().Be("BCP062");
                    x.Message.Should().Be("The referenced declaration with name \"siteDeploy\" is not valid.");
                },
                x =>
                {
                    x.Level.Should().Be(DiagnosticLevel.Error);
                    x.Code.Should().Be("BCP192");
                    x.Message.Should().StartWith(string.Format(expectedErrorMessage, "br:localhost:5000/passthrough/port:v1"));
                },
                x =>
                {
                    x.Level.Should().Be(DiagnosticLevel.Error);
                    x.Code.Should().Be("BCP192");
                    x.Message.Should().StartWith(string.Format(expectedErrorMessage, "br:127.0.0.1/passthrough/ipv4:v1"));
                },
                x =>
                {
                    x.Level.Should().Be(DiagnosticLevel.Error);
                    x.Code.Should().Be("BCP192");
                    x.Message.Should().StartWith(string.Format(expectedErrorMessage, "br:127.0.0.1:5000/passthrough/ipv4port:v1"));
                },
                x =>
                {
                    x.Level.Should().Be(DiagnosticLevel.Error);
                    x.Code.Should().Be("BCP192");
                    x.Message.Should().StartWith(string.Format(expectedErrorMessage, "br:[::1]/passthrough/ipv6:v1"));
                },
                x =>
                {
                    x.Level.Should().Be(DiagnosticLevel.Error);
                    x.Code.Should().Be("BCP192");
                    x.Message.Should().StartWith(string.Format(expectedErrorMessage, "br:[::1]:5000/passthrough/ipv6port:v1"));
                });
        }

        [TestMethod]
        [DoNotParallelize()]
        public async Task ModuleRestoreContentionShouldProduceConsistentState()
        {
            var dataSet = DataSets.Registry_LF;

            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var features = new FeatureProviderOverrides(TestContext);

            var artifactManager = new TestExternalArtifactManager(TestCompiler.ForMockFileSystemCompilation().WithFeatureOverrides<FeatureProviderOverrides, OverriddenFeatureProviderFactory>(features));
            await dataSet.PublishAllDataSetArtifacts(artifactManager, publishSource: true);

            var services = Services
                .WithFeatureOverrides(features)
                .WithTestArtifactManager(artifactManager)
                .Build();

            var dispatcher = services.Construct<IModuleDispatcher>();
            var dummyFile = CreateDummyReferencingFile(services);
            var moduleReferences = dataSet.RegistryModules.Values
                .OrderBy(m => m.Metadata.Target)
                .Select(m => TryGetModuleReference(dispatcher, dummyFile, m.Metadata.Target).Unwrap())
                .ToImmutableList();

            moduleReferences.Should().HaveCount(7);

            // initially the cache should be empty.
            foreach (var moduleReference in moduleReferences)
            {
                dispatcher.GetArtifactRestoreStatus(moduleReference, out _).Should().Be(ArtifactRestoreStatus.Unknown);
            }

            const int ConcurrentTasks = 10;
            var tasks = new List<Task<bool>>();
            for (int i = 0; i < ConcurrentTasks; i++)
            {
                tasks.Add(Task.Run(() => dispatcher.RestoreArtifacts(moduleReferences, forceRestore: false)));
            }

            var result = await Task.WhenAll(tasks);
            result.Should().HaveCount(ConcurrentTasks);

            // modules should now be in the cache
            foreach (var moduleReference in moduleReferences)
            {
                var restoreResult = dispatcher.GetArtifactRestoreStatus(moduleReference, out var errorBuilder);
                var error = errorBuilder?.Invoke(DiagnosticBuilder.ForDocumentStart());

                restoreResult.Should().Be(ArtifactRestoreStatus.Succeeded, $"code: {error?.Code}, message: {error?.Message}");
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetModuleInfoData), DynamicDataSourceType.Method)]
        public async Task ModuleRestoreWithStuckFileLockShouldFailAfterTimeout(IEnumerable<ExternalModuleInfo> moduleInfos, int moduleCount, bool publishSource)
        {
            var dataSet = DataSets.Registry_LF;

            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);

            var cacheDirectory = FileHelper.GetCacheRootDirectory(TestContext).EnsureExists();
            var features = new FeatureProviderOverrides(CacheRootDirectory: cacheDirectory);

            var artifactManager = new TestExternalArtifactManager(TestCompiler.ForMockFileSystemCompilation().WithFeatureOverrides<FeatureProviderOverrides, OverriddenFeatureProviderFactory>(features));
            await dataSet.PublishAllDataSetArtifacts(artifactManager, publishSource: true);

            var fileResolver = BicepTestConstants.FileResolver;

            var services = Services
                .WithFeatureOverrides(features)
                .WithTestArtifactManager(artifactManager)
                .WithFileResolver(fileResolver)
                .Build();

            var dispatcher = services.Construct<IModuleDispatcher>();
            var dummyFile = CreateDummyReferencingFile(services);

            var moduleReferences = moduleInfos
                .OrderBy(m => m.Metadata.Target)
                .Select(m => TryGetModuleReference(dispatcher, dummyFile, m.Metadata.Target).IsSuccess(out var @ref) ? @ref : throw new AssertFailedException($"Invalid module target '{m.Metadata.Target}'."))
                .ToImmutableList();

            moduleReferences.Should().HaveCount(moduleCount);

            // initially the cache should be empty
            foreach (var moduleReference in moduleReferences)
            {
                dispatcher.GetArtifactRestoreStatus(moduleReference, out _).Should().Be(ArtifactRestoreStatus.Unknown);
            }

            dispatcher.TryGetLocalArtifactEntryPointFileHandle(moduleReferences[0]).IsSuccess(out var moduleFile).Should().BeTrue();

            var moduleDirectory = moduleFile!.GetParent().EnsureExists();
            var lockFile = moduleDirectory.GetFile("lock");

            var @lock = lockFile.TryLock();
            @lock.Should().NotBeNull();

            // let's try to restore a module while holding a lock
            using (@lock)
            {
                (await dispatcher.RestoreArtifacts(moduleReferences, forceRestore: false)).Should().BeTrue();
            }

            // the first module should have failed due to a timeout
            dispatcher.GetArtifactRestoreStatus(moduleReferences[0], out var failureBuilder).Should().Be(ArtifactRestoreStatus.Failed);
            using (new AssertionScope())
            {
                failureBuilder!.Should().HaveCode("BCP192");
                failureBuilder!.Should().HaveMessageStartWith($"Unable to restore the artifact with reference \"{moduleReferences[0].FullyQualifiedReference}\": Exceeded the timeout of \"00:00:05\" to acquire the lock on file \"");
            }

            // all other modules should have succeeded
            foreach (var moduleReference in moduleReferences.Skip(1))
            {
                dispatcher.GetArtifactRestoreStatus(moduleReference, out _).Should().Be(ArtifactRestoreStatus.Succeeded);
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetModuleInfoData), DynamicDataSourceType.Method)]
        public async Task ForceModuleRestoreWithStuckFileLockShouldFailAfterTimeout(IEnumerable<ExternalModuleInfo> moduleInfos, int moduleCount, bool publishSource)
        {
            var dataSet = DataSets.Registry_LF;

            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var cacheDirectory = FileHelper.GetCacheRootDirectory(TestContext).EnsureExists();
            var features = new FeatureProviderOverrides(CacheRootDirectory: cacheDirectory);

            var artifactManager = new TestExternalArtifactManager(TestCompiler.ForMockFileSystemCompilation().WithFeatureOverrides<FeatureProviderOverrides, OverriddenFeatureProviderFactory>(features));
            await dataSet.PublishAllDataSetArtifacts(artifactManager, publishSource: true);

            var fileResolver = BicepTestConstants.FileResolver;

            var services = Services
                .WithFeatureOverrides(features)
                .WithTestArtifactManager(artifactManager)
                .WithFileResolver(fileResolver)
                .Build();

            var dispatcher = services.Construct<IModuleDispatcher>();
            var dummyFile = CreateDummyReferencingFile(services);

            var moduleReferences = moduleInfos
                .OrderBy(m => m.Metadata.Target)
                .Select(m => TryGetModuleReference(dispatcher, dummyFile, m.Metadata.Target).IsSuccess(out var @ref) ? @ref : throw new AssertFailedException($"Invalid module target '{m.Metadata.Target}'."))
                .ToImmutableList();

            moduleReferences.Should().HaveCount(moduleCount);

            // initially the cache should be empty
            foreach (var moduleReference in moduleReferences)
            {
                dispatcher.GetArtifactRestoreStatus(moduleReference, out _).Should().Be(ArtifactRestoreStatus.Unknown);
            }

            dispatcher.TryGetLocalArtifactEntryPointFileHandle(moduleReferences[0]).IsSuccess(out var moduleFile).Should().BeTrue();

            var moduleDirectory = moduleFile!.GetParent().EnsureExists();
            var lockFile = moduleDirectory.GetFile("lock");

            var @lock = lockFile.TryLock();
            @lock.Should().NotBeNull();

            // let's try to restore a module while holding a lock
            using (@lock)
            {
                (await dispatcher.RestoreArtifacts(moduleReferences, forceRestore: true)).Should().BeTrue();
            }

            // REF: FileLockTests.cs/FileLockShouldNotThrowIfLockFileIsDeleted()
            // Delete will succeed on Linux and Mac due to advisory nature of locks there
            using (new AssertionScope())
            {
#if WINDOWS_BUILD
                dispatcher.GetArtifactRestoreStatus(moduleReferences[0], out var failureBuilder).Should().Be(ArtifactRestoreStatus.Failed);

                failureBuilder!.Should().HaveCode("BCP233");
                failureBuilder!.Should().HaveMessageStartWith($"Unable to delete the module with reference \"{moduleReferences[0].FullyQualifiedReference}\" from cache: Exceeded the timeout of \"00:00:05\" for the lock on file \"{lockFile.Uri}\" to be released.");
#else
                dispatcher.GetArtifactRestoreStatus(moduleReferences[0], out _).Should().Be(ArtifactRestoreStatus.Succeeded);
#endif

                // all other modules should have succeeded
                foreach (var moduleReference in moduleReferences.Skip(1))
                {
                    dispatcher.GetArtifactRestoreStatus(moduleReference, out _).Should().Be(ArtifactRestoreStatus.Succeeded);
                }
            }

        }

        [DataTestMethod]
        [DynamicData(nameof(GetModuleInfoData), DynamicDataSourceType.Method)]
        public async Task ForceModuleRestoreShouldRestoreAllModules(IEnumerable<ExternalModuleInfo> moduleInfos, int moduleCount, bool publishSource)
        {
            var dataSet = DataSets.Registry_LF;

            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);

            var cacheDirectory = FileHelper.GetCacheRootDirectory(TestContext).EnsureExists();
            var features = new FeatureProviderOverrides(CacheRootDirectory: cacheDirectory);

            var artifactManager = new TestExternalArtifactManager(TestCompiler.ForMockFileSystemCompilation().WithFeatureOverrides<FeatureProviderOverrides, OverriddenFeatureProviderFactory>(features));
            await dataSet.PublishAllDataSetArtifacts(artifactManager, publishSource: true);

            var fileResolver = BicepTestConstants.FileResolver;

            var services = Services
                .WithFeatureOverrides(new(CacheRootDirectory: cacheDirectory))
                .WithTestArtifactManager(artifactManager)
                .WithFileResolver(fileResolver)
                .Build();

            var dispatcher = services.Construct<IModuleDispatcher>();
            var dummyFile = CreateDummyReferencingFile(services);

            var moduleReferences = moduleInfos
                .OrderBy(m => m.Metadata.Target)
                .Select(m => TryGetModuleReference(dispatcher, dummyFile, m.Metadata.Target).IsSuccess(out var @ref) ? @ref : throw new AssertFailedException($"Invalid module target '{m.Metadata.Target}'."))
                .ToImmutableList();

            moduleReferences.Should().HaveCount(moduleCount);

            // initially the cache should be empty
            foreach (var moduleReference in moduleReferences)
            {
                dispatcher.GetArtifactRestoreStatus(moduleReference, out _).Should().Be(ArtifactRestoreStatus.Unknown);
            }

            dispatcher.TryGetLocalArtifactEntryPointFileHandle(moduleReferences[0]).IsSuccess(out var moduleFile).Should().BeTrue();

            moduleFile!.GetParent().EnsureExists();

            (await dispatcher.RestoreArtifacts(moduleReferences, forceRestore: true)).Should().BeTrue();

            // all other modules should have succeeded
            foreach (var moduleReference in moduleReferences)
            {
                dispatcher.GetArtifactRestoreStatus(moduleReference, out _).Should().Be(ArtifactRestoreStatus.Succeeded);
            }
        }

        public static IEnumerable<object[]> GetModuleInfoData()
        {
            yield return new object[] { DataSets.Registry_LF.RegistryModules.Values, 7, false /* publishSource */ };
            yield return new object[] { DataSets.Registry_LF.RegistryModules.Values, 7, true };
            yield return new object[] { DataSets.Registry_LF.TemplateSpecs.Values, 2, false };
            yield return new object[] { DataSets.Registry_LF.TemplateSpecs.Values, 2, true };
        }

        private static BicepFile CreateDummyReferencingFile(IDependencyHelper dependencyHelper)
        {
            var sourceFileFactory = dependencyHelper.Construct<ISourceFileFactory>();

            return sourceFileFactory.CreateBicepFile(new Uri("inmemory:///main.bicep"), "");
        }

        private static ResultWithDiagnosticBuilder<ArtifactReference> TryGetModuleReference(IModuleDispatcher moduleDispatcher, BicepSourceFile referencingFile, string reference) =>
            moduleDispatcher.TryGetArtifactReference(referencingFile, ArtifactType.Module, reference);
    }
}
