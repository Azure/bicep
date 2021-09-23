// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using Bicep.LangServer.IntegrationTests;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Registry;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.LangServer.UnitTests.Registry
{
    [TestClass]
    public class ModuleRestoreSchedulerTests
    {
        private static readonly MockRepository Repository = new(MockBehavior.Strict);

        [TestMethod]
        [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Not needed")]
        public async Task DisposeAfterCreateShouldNotThrow()
        {
            var moduleDispatcher = Repository.Create<IModuleDispatcher>();
            await using (var scheduler = new ModuleRestoreScheduler(moduleDispatcher.Object))
            {
                // intentional extra call to dispose()
                await scheduler.DisposeAsync();
            }
        }

        [TestMethod]
        [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Not needed")]
        public async Task DisposeAfterStartShouldNotThrow()
        {
            var moduleDispatcher = Repository.Create<IModuleDispatcher>();
            await using (var scheduler = new ModuleRestoreScheduler(moduleDispatcher.Object))
            {
                scheduler.Start();

                // intentional extra call to dispose()
                await scheduler.DisposeAsync();
            }

            await using(var scheduler = new ModuleRestoreScheduler(moduleDispatcher.Object))
            {
                await Task.Yield();
                await Task.Delay(TimeSpan.FromSeconds(1));

                scheduler.Start();
            }
        }

        [TestMethod]
        [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Not needed")]
        public async Task PublicMethodsShouldThrowAfterDispose()
        {
            var moduleDispatcher = Repository.Create<IModuleDispatcher>();
            var scheduler = new ModuleRestoreScheduler(moduleDispatcher.Object);
            await scheduler.DisposeAsync();

            Action startFail = () => scheduler.Start();
            startFail.Should().Throw<ObjectDisposedException>();

            Action requestFail = () => scheduler.RequestModuleRestore(Repository.Create<ICompilationManager>().Object, DocumentUri.From("untitled://one"), Enumerable.Empty<ModuleDeclarationSyntax>());
            requestFail.Should().Throw<ObjectDisposedException>();
        }

        [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Not needed")]
        [TestMethod]
        public async Task RestoreShouldBeScheduledAsRequested()
        {
            var provider = Repository.Create<IModuleRegistryProvider>();
            var mockRegistry = new MockRegistry();
            provider.Setup(m => m.Registries).Returns(((IModuleRegistry)mockRegistry).AsEnumerable().ToImmutableArray());

            var dispatcher = new ModuleDispatcher(provider.Object);

            var firstUri = DocumentUri.From("foo://one");
            var firstSource = new TaskCompletionSource<bool>();

            var secondUri = DocumentUri.From("foo://two");
            var secondSource = new TaskCompletionSource<bool>();

            var thirdUri = DocumentUri.From("foo://three");
            var thirdSource = new TaskCompletionSource<bool>();

            var compilationManager = Repository.Create<ICompilationManager>();
            compilationManager.Setup(m => m.RefreshCompilation(firstUri, It.IsAny<bool>())).Callback<DocumentUri, bool>((uri, reloadBicepConfig) => firstSource.SetResult(true));
            compilationManager.Setup(m => m.RefreshCompilation(secondUri, It.IsAny<bool>())).Callback<DocumentUri, bool>((uri, reloadBicepConfig) => secondSource.SetResult(true));
            compilationManager.Setup(m => m.RefreshCompilation(thirdUri, It.IsAny<bool>())).Callback<DocumentUri, bool>((uri, reloadBicepConfig) => thirdSource.SetResult(true));

            var firstFileSet = CreateModules("mock:one", "mock:two");
            var secondFileSet = CreateModules("mock:three", "mock:four");
            var thirdFileSet = CreateModules("mock:five", "mock:six");

            await using (var scheduler = new ModuleRestoreScheduler(dispatcher))
            {
                scheduler.RequestModuleRestore(compilationManager.Object, firstUri, firstFileSet);
                scheduler.RequestModuleRestore(compilationManager.Object, secondUri, secondFileSet);

                // start processing, which will immediately pick up all the items in the queue
                scheduler.Start();

                // wait until both compilation managers are notified
                await IntegrationTestHelper.WithTimeoutAsync(Task.WhenAll(firstSource.Task, secondSource.Task));

                // two separate requests should have been unified into single restore
                if (mockRegistry.ModuleRestores.TryPop(out var initialRefs))
                {
                    mockRegistry.ModuleRestores.Should().BeEmpty();
                    initialRefs.Select(mr => mr.FullyQualifiedReference).Should().BeEquivalentTo("mock:one", "mock:two", "mock:three", "mock:four");
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

                if(mockRegistry.ModuleRestores.TryPop(out var followingRefs))
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

        private static ImmutableArray<ModuleDeclarationSyntax> CreateModules(params string[] references)
        {
            var buffer = new StringBuilder();
            foreach(var reference in references)
            {
                buffer.AppendLine($"module foo '{reference}' = {{}}");
            }

            var file = SourceFileFactory.CreateBicepFile(new System.Uri("untitled://hello"), buffer.ToString());
            return file.ProgramSyntax.Declarations.OfType<ModuleDeclarationSyntax>().ToImmutableArray();
        }

        private class MockRegistry : IModuleRegistry
        {
            public ConcurrentStack<IEnumerable<ModuleReference>> ModuleRestores { get; } = new();

            public string Scheme => "mock";

            public RegistryCapabilities Capabilities => throw new NotImplementedException();

            public bool IsModuleRestoreRequired(ModuleReference reference) => true;

            public Task PublishModule(ModuleReference moduleReference, Stream compiled)
            {
                throw new NotImplementedException();
            }

            public Task<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>> RestoreModules(IEnumerable<ModuleReference> references)
            {
                this.ModuleRestores.Push(references);
                return Task.FromResult<IDictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>>(new Dictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>());
            }

            public Uri? TryGetLocalModuleEntryPointUri(Uri? parentModuleUri, ModuleReference reference, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
            {
                throw new NotImplementedException();
            }

            public ModuleReference? TryParseModuleReference(string reference, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
            {
                failureBuilder = null;
                return new MockModuleRef(reference);
            }
        }

        private class MockModuleRef : ModuleReference
        {
            public MockModuleRef(string value)
                : base("mock")
            {
                this.Value = value;
            }

            public string Value { get; }

            public override string UnqualifiedReference => this.Value;

            public override bool IsExternal => true;
        }
    }
}
