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
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceLink;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
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
            var mockRegistry = new MockRegistry();
            var provider = new MockArtifactRegistryProvider(mockRegistry.AsEnumerable());

            var dispatcher = new ModuleDispatcher(provider);

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

        private static ImmutableArray<ArtifactReference> CreateArtifactReferences(MockRegistry mockRegistry, params string[] references) => references
            .Select(x => mockRegistry.TryParseArtifactReference(BicepTestConstants.DummyBicepFile, ArtifactType.Module, null, x).Unwrap())
            .ToImmutableArray();

        private class MockRegistry : IArtifactRegistry
        {
            public ConcurrentStack<IEnumerable<ArtifactReference>> ModuleRestores { get; } = new();

            public string Scheme => "mock";

            public RegistryCapabilities GetCapabilities(ArtifactType artifactType, ArtifactReference _) => throw new NotImplementedException();

            public bool IsArtifactRestoreRequired(ArtifactReference _) => true;

            public Task PublishModule(ArtifactReference _, BinaryData __, BinaryData? ___, string? ____, string? _____)
                => throw new NotImplementedException();

            public Task PublishExtension(ArtifactReference _, ExtensionPackage __)
                => throw new NotImplementedException();

            public Task<bool> CheckArtifactExists(ArtifactType artifactType, ArtifactReference reference) => throw new NotImplementedException();

            public Task<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> InvalidateArtifactsCache(IEnumerable<ArtifactReference> _)
            {
                throw new NotImplementedException();
            }

            public Task<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> RestoreArtifacts(IEnumerable<ArtifactReference> references)
            {
                this.ModuleRestores.Push(references);
                return Task.FromResult<IDictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>>(new Dictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>());
            }

            public string? GetDocumentationUri(ArtifactReference _) => null;

            public Task<string?> TryGetModuleDescription(ModuleSymbol module, ArtifactReference _) => Task.FromResult<string?>(null);

            public ResultWithDiagnosticBuilder<ArtifactReference> TryParseArtifactReference(BicepSourceFile referencingFile, ArtifactType _, string? __, string reference)
            {
                return new(new MockArtifactRef(referencingFile, reference));
            }

            public Task OnRestoreArtifacts(bool forceRestore) => Task.CompletedTask;
        }

        private class MockArtifactRef : ArtifactReference
        {
            public MockArtifactRef(BicepSourceFile referencingFile, string value)
                : base(referencingFile, "mock")
            {
                this.Value = value;
            }

            public string Value { get; }

            public override string UnqualifiedReference => this.Value;

            public override bool IsExternal => true;

            public override ResultWithDiagnosticBuilder<IFileHandle> TryGetEntryPointFileHandle() => new(DummyFileHandle.Instance);
        }

        private class MockArtifactRegistryProvider(IEnumerable<IArtifactRegistry> registries) : ArtifactRegistryProvider(registries)
        {
        }
    }
}
