// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Compilation;
using Bicep.LanguageServer.Features.Custom.Visualization;
using Bicep.LanguageServer.Features.Custom.Visualization.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using CompilationHelper = Bicep.Core.UnitTests.Utils.CompilationHelper;

namespace Bicep.LangServer.UnitTests.Features.Visualization;

[TestClass]
public class VisualPaletteHandlerTests
{
    private static readonly DocumentUri DocumentUri = DocumentUri.From("file:///main.bicep");

    [DataTestMethod]
    [DataRow("", "resourceGroup")]
    [DataRow("targetScope = 'resourceGroup'", "resourceGroup")]
    [DataRow("targetScope = 'subscription'", "subscription")]
    [DataRow("targetScope = 'managementGroup'", "managementGroup")]
    [DataRow("targetScope = 'tenant'", "tenant")]
    [DataRow("targetScope = 'subscription'\nvar broken =", "subscription")]
    public async Task GraphUpdate_ReturnsScopeEvenForEmptyOrInvalidDocuments(string text, string expectedScope)
    {
        var manager = CreateCompilationManager(text);
        var handler = new VisualGraphUpdateHandler(NullLogger<VisualGraphUpdateHandler>.Instance, manager.Object);

        var response = await handler.Handle(new(new() { Uri = DocumentUri }, new RenderedGraph([], [])), CancellationToken.None);

        response.TargetScope.Should().Be(expectedScope);
    }

    [TestMethod]
    public async Task GraphUpdate_ReflectsScopeOnlyChanges()
    {
        var manager = CreateCompilationManager("targetScope = 'subscription'");
        var handler = new VisualGraphUpdateHandler(NullLogger<VisualGraphUpdateHandler>.Instance, manager.Object);
        var request = new VisualGraphUpdateParams(new() { Uri = DocumentUri }, new RenderedGraph([], []));

        var first = await handler.Handle(request, CancellationToken.None);
        var result = CompilationHelper.Compile(new ServiceBuilder().WithAzResources([]), "targetScope = 'tenant'");
        manager.Setup(x => x.GetCompilation(DocumentUri)).Returns(new CompilationContext(result.Compilation));
        var second = await handler.Handle(request, CancellationToken.None);

        first.TargetScope.Should().Be("subscription");
        second.TargetScope.Should().Be("tenant");
        second.Patches.Should().NotContain(patch => patch is GraphPatch.AddNode);
    }

    [TestMethod]
    public async Task GraphUpdate_WithoutCompilation_ReturnsUnknownScope()
    {
        var manager = new Mock<ICompilationManager>(MockBehavior.Strict);
        manager.Setup(x => x.GetCompilation(DocumentUri)).Returns((CompilationContext?)null);
        var handler = new VisualGraphUpdateHandler(NullLogger<VisualGraphUpdateHandler>.Instance, manager.Object);

        var response = await handler.Handle(new(new() { Uri = DocumentUri }, null), CancellationToken.None);

        response.TargetScope.Should().BeNull();
        response.Patches.Should().BeEmpty();
    }

    [TestMethod]
    public async Task VersionsHandler_ReturnsVersionsAndSharedCatalogId()
    {
        var manager = CreateCompilationManager(string.Empty);
        var service = new VisualResourceCreationService();
        var handler = new VisualResourceTypeVersionsHandler(
            NullLogger<VisualResourceTypeVersionsHandler>.Instance, manager.Object, service);

        var response = await handler.Handle(new(new() { Uri = DocumentUri }, "Test.Rp/widgets"), CancellationToken.None);

        response.ApiVersions.Should().Equal("2025-01-01-preview", "2024-01-01");
        response.CatalogId.Should().Be(service.GetResourceTypeNamespaces(
            manager.Object.GetCompilation(DocumentUri)!.Compilation.GetEntrypointSemanticModel()).CatalogId);
    }

    [DataTestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task VersionsHandler_MissingDocumentOrType_ReportsRpcFailure(bool compiled)
    {
        var manager = CreateCompilationManager(string.Empty);
        if (!compiled)
        {
            manager.Setup(x => x.GetCompilation(DocumentUri)).Returns((CompilationContext?)null);
        }
        var handler = new VisualResourceTypeVersionsHandler(
            NullLogger<VisualResourceTypeVersionsHandler>.Instance, manager.Object, new VisualResourceCreationService());

        Func<Task> act = () => handler.Handle(new(new() { Uri = DocumentUri }, "Test.Rp/missing"), CancellationToken.None);

        await act.Should().ThrowAsync<RpcErrorException>();
    }

    private static Mock<ICompilationManager> CreateCompilationManager(string text)
    {
        var result = CompilationHelper.Compile(new ServiceBuilder().WithAzResources(
        [
            TestTypeHelper.CreateCustomResourceType("Test.Rp/widgets", "2024-01-01", TypeSymbolValidationFlags.Default),
            TestTypeHelper.CreateCustomResourceType("Test.Rp/widgets", "2025-01-01-preview", TypeSymbolValidationFlags.Default),
        ]), text);
        var manager = new Mock<ICompilationManager>(MockBehavior.Strict);
        manager.Setup(x => x.GetCompilation(DocumentUri)).Returns(new CompilationContext(result.Compilation));
        return manager;
    }
}
