// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Compilation;
using Bicep.LanguageServer.Features.Custom.Visualization;
using Bicep.LanguageServer.Features.Custom.Visualization.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LangServer.UnitTests.Features.Visualization;

[TestClass]
public class VisualGraphLayoutHandlerTests
{
    private const string MainBicep = """
        resource outside 'Test.Rp/basicTests@2020-01-01' = {
          name: 'outside'
        }

        module mod 'mod.bicep' = {
          name: 'mod'
        }
        """;

    private const string ModBicep = """
        resource a 'Test.Rp/basicTests@2020-01-01' = {
          name: 'a'
        }

        resource b 'Test.Rp/basicTests@2020-01-01' = {
          name: 'b'
          properties: {
            dependsOnA: a.id
          }
        }
        """;

    [TestMethod]
    public async Task Layout_WithRootNode_LaysOutOnlyThatModuleAtTheOriginWithItsBoxAsTheBounds()
    {
        var (handler, uri) = CreateHandler();
        var measured = new MeasuredGraph(
            [
                new("mod", GraphNodeKind.Module, null, 0, 0),
                new("mod::a", GraphNodeKind.Resource, "mod", 200, 80),
                new("mod::b", GraphNodeKind.Resource, "mod", 200, 80),
            ],
            [new("mod::b->mod::a", "mod::b", "mod::a")]);

        var result = await handler.Handle(new(new() { Uri = uri }, measured, null, [], RootNodeId: "mod"), CancellationToken.None);

        result.Status.Should().Be(VisualGraphLayoutStatus.Ok);
        result.Positions.Select(position => position.NodeId).Should().BeEquivalentTo(["mod", "mod::a", "mod::b"]);
        result.Positions.Single(position => position.NodeId == "mod").Should().Match<NodePosition>(position => position.X == 0 && position.Y == 0);

        // The client centers the module on where it was from the bounds alone, so they must be the module's box: its
        // children's extents plus the padding the canvas draws around them.
        var padding = VisualGraphLayoutOptions.Default.ModulePadding;
        var children = result.Positions.Where(position => position.NodeId != "mod").ToArray();
        var bounds = result.Bounds!;
        children.Min(position => position.X).Should().BeApproximately(padding.Left, 0.001);
        children.Min(position => position.Y).Should().BeApproximately(padding.Top, 0.001);
        bounds.Width.Should().BeApproximately(children.Max(position => position.X) + 200 + padding.Right, 0.001);
        bounds.Height.Should().BeApproximately(children.Max(position => position.Y) + 80 + padding.Bottom, 0.001);
    }

    [TestMethod]
    public async Task Layout_WithRootNode_RejectsAMeasuredGraphThatIsNotThatSubtree()
    {
        var (handler, uri) = CreateHandler();
        var measured = new MeasuredGraph(
            [
                new("mod", GraphNodeKind.Module, null, 0, 0),
                new("mod::a", GraphNodeKind.Resource, "mod", 200, 80),
                new("mod::b", GraphNodeKind.Resource, "mod", 200, 80),
                new("outside", GraphNodeKind.Resource, null, 200, 80),
            ],
            [new("mod::b->mod::a", "mod::b", "mod::a")]);

        var result = await handler.Handle(new(new() { Uri = uri }, measured, null, [], RootNodeId: "mod"), CancellationToken.None);

        result.Status.Should().Be(VisualGraphLayoutStatus.GraphChanged);
    }

    [TestMethod]
    public async Task Layout_WithCollapsedRootNode_ReportsGraphChanged()
    {
        var (handler, uri) = CreateHandler();
        var measured = new MeasuredGraph([new("mod", GraphNodeKind.Module, null, 200, 80)], []);

        var result = await handler.Handle(new(new() { Uri = uri }, measured, null, ["mod"], RootNodeId: "mod"), CancellationToken.None);

        result.Status.Should().Be(VisualGraphLayoutStatus.GraphChanged);
    }

    private static (VisualGraphLayoutHandler Handler, DocumentUri Uri) CreateHandler()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithAzResources(BuiltInTestTypes.Types),
            ("main.bicep", MainBicep),
            ("mod.bicep", ModBicep));
        var uri = result.SourceFile.FileHandle.Uri.ToUri();
        var manager = new Mock<ICompilationManager>(MockBehavior.Strict);
        manager.Setup(x => x.GetCompilation(It.IsAny<DocumentUri>())).Returns(new CompilationContext(result.Compilation));

        var handler = new VisualGraphLayoutHandler(
            NullLogger<VisualGraphLayoutHandler>.Instance,
            manager.Object,
            new MsaglVisualGraphLayoutEngine(NullLogger<MsaglVisualGraphLayoutEngine>.Instance),
            Mock.Of<IVisualModuleSourceProvider>(MockBehavior.Strict));

        return (handler, uri);
    }
}
