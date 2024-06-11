// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.SourceCode;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Registry;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LangServer.IntegrationTests.Registry
{
    [TestClass]
    public class ModuleRestoreSchedulerTests
    {
        private static readonly MockRepository Repository = new(MockBehavior.Strict);

        private static readonly RootConfiguration Configuration = IConfigurationManager.GetBuiltInConfiguration();

        [TestMethod]
        public async Task DisposeAfterCreateShouldNotThrow()
        {
            var moduleDispatcher = Repository.Create<IModuleDispatcher>();
            await using var scheduler = new ModuleRestoreScheduler(moduleDispatcher.Object);
            // intentional extra call to dispose()
            await scheduler.DisposeAsync();
        }

        [TestMethod]
        public async Task DisposeAfterStartShouldNotThrow()
        {
            var moduleDispatcher = Repository.Create<IModuleDispatcher>();
            await using (var scheduler = new ModuleRestoreScheduler(moduleDispatcher.Object))
            {
                scheduler.Start();

                // intentional extra call to dispose()
                await scheduler.DisposeAsync();
            }

            await using (var scheduler = new ModuleRestoreScheduler(moduleDispatcher.Object))
            {
                await Task.Yield();
                await Task.Delay(TimeSpan.FromSeconds(1));

                scheduler.Start();
            }
        }

        [TestMethod]
        public async Task PublicMethodsShouldThrowAfterDispose()
        {
            var moduleDispatcher = Repository.Create<IModuleDispatcher>();
            var scheduler = new ModuleRestoreScheduler(moduleDispatcher.Object);
            await scheduler.DisposeAsync();

            Action startFail = () => scheduler.Start();
            startFail.Should().Throw<ObjectDisposedException>();

            Action requestFail = () => scheduler.RequestModuleRestore(Repository.Create<ICompilationManager>().Object, DocumentUri.From("untitled://one"), []);
            requestFail.Should().Throw<ObjectDisposedException>();
        }

        [TestMethod]
        public async Task RestoreShouldBeScheduledAsRequested()
        {
            var provider = Repository.Create<IArtifactRegistryProvider>();
            var mockRegistry = new MockRegistry();
            provider.Setup(m => m.Registries(It.IsAny<Uri>())).Returns(((IArtifactRegistry)mockRegistry).AsEnumerable().ToImmutableArray());

            var dispatcher = new ModuleDispatcher(provider.Object, IConfigurationManager.WithStaticConfiguration(Configuration));

            var firstUri = DocumentUri.From("foo://one");
            var firstSource = new TaskCompletionSource<bool>();

            var secondUri = DocumentUri.From("foo://two");
            var secondSource = new TaskCompletionSource<bool>();

            var thirdUri = DocumentUri.From("foo://three");
            var thirdSource = new TaskCompletionSource<bool>();

            var compilationManager = Repository.Create<ICompilationManager>();
            compilationManager.Setup(m => m.RefreshCompilation(firstUri, false)).Callback<DocumentUri, bool>((uri, _) => firstSource.SetResult(true));
            compilationManager.Setup(m => m.RefreshCompilation(secondUri, false)).Callback<DocumentUri, bool>((uri, _) => secondSource.SetResult(true));
            compilationManager.Setup(m => m.RefreshCompilation(thirdUri, false)).Callback<DocumentUri, bool>((uri, _) => thirdSource.SetResult(true));

            var firstFileSet = CreateArtifactReferences(mockRegistry, "one", "two");
            var secondFileSet = CreateArtifactReferences(mockRegistry, "three", "four");
            var thirdFileSet = CreateArtifactReferences(mockRegistry, "five", "six");

            await using (var scheduler = new ModuleRestoreScheduler(dispatcher))
            {
                scheduler.RequestModuleRestore(compilationManager.Object, firstUri, firstFileSet);
                scheduler.RequestModuleRestore(compilationManager.Object, secondUri, secondFileSet);

                // start processing, which will immediately pick up all the items in the queue
                scheduler.Start();

                // wait until both compilation managers are notified
                await IntegrationTestHelper.WithTimeoutAsync(Task.WhenAll(firstSource.Task, secondSource.Task));

                if (mockRegistry.ModuleRestores.TryPop(out var initialRefs))
                {
                    mockRegistry.ModuleRestores.Should().NotBeEmpty();
                    initialRefs.Select(mr => mr.FullyQualifiedReference).Should().BeEquivalentTo("mock:three", "mock:four");
                }
                else
                {
                    throw new AssertFailedException("Scheduler did not perform the expected restores.");
                }

                if (mockRegistry.ModuleRestores.TryPop(out var secondRefs))
                {
                    mockRegistry.ModuleRestores.Should().BeEmpty();
                    secondRefs.Select(mr => mr.FullyQualifiedReference).Should().BeEquivalentTo("mock:one", "mock:two");
                }
                else
                {
                    throw new AssertFailedException("Scheduler did not perform the expected restores.");
                }

                // request more restores
                mockRegistry.ModuleRestores.Should().BeEmpty();
                scheduler.RequestModuleRestore(compilationManager.Object, thirdUri, thirdFileSet);

                // wait for completion
                await IntegrationTestHelper.WithTimeoutAsync(thirdSource.Task);

                if (mockRegistry.ModuleRestores.TryPop(out var followingRefs))
                {
                    mockRegistry.ModuleRestores.Should().BeEmpty();
                    followingRefs.Select(mr => mr.FullyQualifiedReference).Should().BeEquivalentTo("mock:five", "mock:six");
                }
                else
                {
                    throw new AssertFailedException("Scheduler did not perform the expected restores.");
                }
            }
        }

        private static ImmutableArray<ArtifactReference> CreateArtifactReferences(MockRegistry mockRegistry, params string[] references)
        {
            return references
                .Select(x => mockRegistry.TryParseArtifactReference(ArtifactType.Module, null, x).Unwrap())
                .ToImmutableArray<ArtifactReference>();
        }

        private class MockRegistry : IArtifactRegistry
        {
            public ConcurrentStack<IEnumerable<ArtifactReference>> ModuleRestores { get; } = new();

            public string Scheme => "mock";

            public RegistryCapabilities GetCapabilities(ArtifactType artifactType, ArtifactReference _) => throw new NotImplementedException();

            public bool IsArtifactRestoreRequired(ArtifactReference _) => true;

            public Task PublishModule(ArtifactReference _, BinaryData __, BinaryData? ___, string? ____, string? _____)
                => throw new NotImplementedException();

            public Task PublishProvider(ArtifactReference _, ProviderPackage __)
                => throw new NotImplementedException();

            public Task<bool> CheckArtifactExists(ArtifactType artifactType, ArtifactReference reference) => throw new NotImplementedException();

            public Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> InvalidateArtifactsCache(IEnumerable<ArtifactReference> _)
            {
                throw new NotImplementedException();
            }

            public Task<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreArtifacts(IEnumerable<ArtifactReference> references)
            {
                this.ModuleRestores.Push(references);
                return Task.FromResult<IDictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>>(new Dictionary<ArtifactReference, DiagnosticBuilder.ErrorBuilderDelegate>());
            }

            public ResultWithDiagnostic<Uri> TryGetLocalArtifactEntryPointUri(ArtifactReference _)
            {
                throw new NotImplementedException();
            }

            public string? GetDocumentationUri(ArtifactReference _) => null;

            public Task<string?> TryGetModuleDescription(ModuleSymbol module, ArtifactReference _) => Task.FromResult<string?>(null);

            public ResultWithDiagnostic<ArtifactReference> TryParseArtifactReference(ArtifactType _, string? __, string reference)
            {
                return new(new MockArtifactRef(reference, PathHelper.FilePathToFileUrl(Path.GetTempFileName())));
            }

            public ResultWithException<SourceArchive> TryGetSource(ArtifactReference artifactReference) => new(new SourceNotAvailableException());

            public Uri? TryGetProviderBinary(ArtifactReference reference) => null;
        }

        private class MockArtifactRef : ArtifactReference
        {
            public MockArtifactRef(string value, Uri parentModuleUri)
                : base("mock", parentModuleUri)
            {
                this.Value = value;
            }

            public string Value { get; }

            public override string UnqualifiedReference => this.Value;

            public override bool IsExternal => true;
        }
    }
}
