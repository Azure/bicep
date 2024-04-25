// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
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
            var clientFactory = dataSet.CreateMockRegistryClients();
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory);

            var fileUri = PathHelper.FilePathToFileUrl(Path.Combine(outputDirectory, DataSet.TestFileMain));

            var badCacheDirectory = FileHelper.GetCacheRootPath(TestContext);
            Directory.CreateDirectory(badCacheDirectory);

            var badCachePath = Path.Combine(badCacheDirectory, "file.txt");
            File.Create(badCachePath);
            File.Exists(badCachePath).Should().BeTrue();

            // cache root points to a file
            var featureOverrides = BicepTestConstants.FeatureOverrides with
            {
                RegistryEnabled = true,
                CacheRootDirectory = badCachePath
            };
            var featuresFactory = BicepTestConstants.CreateFeatureProviderFactory(featureOverrides);

            var services = Services
                .WithFeatureOverrides(new(RegistryEnabled: true, CacheRootDirectory: badCachePath))
                .WithContainerRegistryClientFactory(clientFactory)
                .WithTemplateSpecRepositoryFactory(templateSpecRepositoryFactory)
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

            var publishSource = true;
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var clientFactory = dataSet.CreateMockRegistryClients();
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory, publishSource);

            var cacheDirectory = FileHelper.GetCacheRootPath(TestContext);
            Directory.CreateDirectory(cacheDirectory);

            var services = Services
                .WithFeatureOverrides(new(CacheRootDirectory: cacheDirectory))
                .WithContainerRegistryClientFactory(clientFactory)
                .WithTemplateSpecRepositoryFactory(templateSpecRepositoryFactory)
                .Build();

            var dispatcher = services.Construct<IModuleDispatcher>();

            var moduleReferences = dataSet.RegistryModules.Values
                .OrderBy(m => m.Metadata.Target)
                .Select(m => dispatcher.TryGetModuleReference(m.Metadata.Target, RandomFileUri()).Unwrap())
                .ToImmutableList();

            moduleReferences.Should().HaveCount(7);

            // initially the cache should be empty
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
            var clientFactory = dataSet.CreateMockRegistryClients();
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory, publishSource: publishSource);

            var cacheDirectory = FileHelper.GetCacheRootPath(TestContext);
            Directory.CreateDirectory(cacheDirectory);

            var fileResolver = BicepTestConstants.FileResolver;
            var services = Services
                .WithFeatureOverrides(new(CacheRootDirectory: cacheDirectory))
                .WithContainerRegistryClientFactory(clientFactory)
                .WithTemplateSpecRepositoryFactory(templateSpecRepositoryFactory)
                .WithFileResolver(fileResolver)
                .Build();

            var dispatcher = services.Construct<IModuleDispatcher>();

            var configuration = BicepTestConstants.BuiltInConfigurationWithAllAnalyzersDisabled;
            var moduleReferences = moduleInfos
                .OrderBy(m => m.Metadata.Target)
                .Select(m => dispatcher.TryGetModuleReference(m.Metadata.Target, RandomFileUri()).IsSuccess(out var @ref) ? @ref : throw new AssertFailedException($"Invalid module target '{m.Metadata.Target}'."))
                .ToImmutableList();

            moduleReferences.Should().HaveCount(moduleCount);

            // initially the cache should be empty
            foreach (var moduleReference in moduleReferences)
            {
                dispatcher.GetArtifactRestoreStatus(moduleReference, out _).Should().Be(ArtifactRestoreStatus.Unknown);
            }

            dispatcher.TryGetLocalArtifactEntryPointUri(moduleReferences[0]).IsSuccess(out var moduleFileUri).Should().BeTrue();
            moduleFileUri.Should().NotBeNull();

            var moduleFilePath = moduleFileUri!.LocalPath;
            var moduleDirectory = Path.GetDirectoryName(moduleFilePath)!;
            Directory.CreateDirectory(moduleDirectory);

            var lockFileName = Path.Combine(moduleDirectory, "lock");
            var lockFileUri = new Uri(lockFileName);

            var @lock = fileResolver.TryAcquireFileLock(lockFileUri);
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
            var clientFactory = dataSet.CreateMockRegistryClients();
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory);

            var cacheDirectory = FileHelper.GetCacheRootPath(TestContext);
            Directory.CreateDirectory(cacheDirectory);

            var fileResolver = BicepTestConstants.FileResolver;
            var services = Services
                .WithFeatureOverrides(new(CacheRootDirectory: cacheDirectory))
                .WithContainerRegistryClientFactory(clientFactory)
                .WithTemplateSpecRepositoryFactory(templateSpecRepositoryFactory)
                .WithFileResolver(fileResolver)
                .Build();

            var dispatcher = services.Construct<IModuleDispatcher>();

            var moduleReferences = moduleInfos
                .OrderBy(m => m.Metadata.Target)
                .Select(m => dispatcher.TryGetModuleReference(m.Metadata.Target, RandomFileUri()).IsSuccess(out var @ref) ? @ref : throw new AssertFailedException($"Invalid module target '{m.Metadata.Target}'."))
                .ToImmutableList();

            moduleReferences.Should().HaveCount(moduleCount);

            // initially the cache should be empty
            foreach (var moduleReference in moduleReferences)
            {
                dispatcher.GetArtifactRestoreStatus(moduleReference, out _).Should().Be(ArtifactRestoreStatus.Unknown);
            }

            dispatcher.TryGetLocalArtifactEntryPointUri(moduleReferences[0]).IsSuccess(out var moduleFileUri).Should().BeTrue();
            moduleFileUri.Should().NotBeNull();

            var moduleFilePath = moduleFileUri!.LocalPath;
            var moduleDirectory = Path.GetDirectoryName(moduleFilePath)!;
            Directory.CreateDirectory(moduleDirectory);

            var lockFileName = Path.Combine(moduleDirectory, "lock");
            var lockFileUri = new Uri(lockFileName);

            var @lock = fileResolver.TryAcquireFileLock(lockFileUri);
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
                failureBuilder!.Should().HaveMessageStartWith($"Unable to delete the module with reference \"{moduleReferences[0].FullyQualifiedReference}\" from cache: Exceeded the timeout of \"00:00:05\" for the lock on file \"{lockFileUri}\" to be released.");
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
            var clientFactory = dataSet.CreateMockRegistryClients();
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory, publishSource);

            var cacheDirectory = FileHelper.GetCacheRootPath(TestContext);
            Directory.CreateDirectory(cacheDirectory);

            var fileResolver = BicepTestConstants.FileResolver;
            var services = Services
                .WithFeatureOverrides(new(CacheRootDirectory: cacheDirectory))
                .WithContainerRegistryClientFactory(clientFactory)
                .WithTemplateSpecRepositoryFactory(templateSpecRepositoryFactory)
                .WithFileResolver(fileResolver)
                .Build();

            var dispatcher = services.Construct<IModuleDispatcher>();

            var configuration = BicepTestConstants.BuiltInConfigurationWithAllAnalyzersDisabled;
            var moduleReferences = moduleInfos
                .OrderBy(m => m.Metadata.Target)
                .Select(m => dispatcher.TryGetModuleReference(m.Metadata.Target, RandomFileUri()).IsSuccess(out var @ref) ? @ref : throw new AssertFailedException($"Invalid module target '{m.Metadata.Target}'."))
                .ToImmutableList();

            moduleReferences.Should().HaveCount(moduleCount);

            // initially the cache should be empty
            foreach (var moduleReference in moduleReferences)
            {
                dispatcher.GetArtifactRestoreStatus(moduleReference, out _).Should().Be(ArtifactRestoreStatus.Unknown);
            }

            dispatcher.TryGetLocalArtifactEntryPointUri(moduleReferences[0]).IsSuccess(out var moduleFileUri).Should().BeTrue();
            moduleFileUri.Should().NotBeNull();

            var moduleFilePath = moduleFileUri!.LocalPath;
            var moduleDirectory = Path.GetDirectoryName(moduleFilePath)!;
            Directory.CreateDirectory(moduleDirectory);

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

        private static Uri RandomFileUri() => PathHelper.FilePathToFileUrl(Path.GetTempFileName());
    }
}
