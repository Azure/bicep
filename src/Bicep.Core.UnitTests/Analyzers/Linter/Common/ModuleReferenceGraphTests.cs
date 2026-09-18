// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Analyzers.Linter.Common;

[TestClass]
public class ModuleReferenceGraphTests
{
    [TestMethod]
    public void Build_SimpleChain_IncludesAllFilesWithCorrectParentPointers()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                module a 'a.bicep' = {
                  name: 'a'
                }
                """),
            ("a.bicep", """
                module b 'b.bicep' = {
                  name: 'b'
                }
                """),
            ("b.bicep", """
                output value string = 'leaf'
                """));

        var entryPoint = result.Compilation.GetEntrypointSemanticModel();
        var graph = entryPoint.GetModuleReferenceGraph();

        var entryPointUri = entryPoint.SourceFile.FileHandle.Uri;
        var aUri = graph.Nodes.Keys.Single(uri => uri.ToString().EndsWith("a.bicep"));
        var bUri = graph.Nodes.Keys.Single(uri => uri.ToString().EndsWith("b.bicep"));

        graph.Nodes.Should().HaveCount(3);
        graph.Nodes[entryPointUri].Parent.Should().BeNull();
        graph.Nodes[aUri].Parent.Should().Be(entryPointUri);
        graph.Nodes[bUri].Parent.Should().Be(aUri);
    }

    [TestMethod]
    public void Build_DiamondDependency_VisitsSharedModuleOnlyOnce()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                module a 'a.bicep' = {
                  name: 'a'
                }
                module b 'b.bicep' = {
                  name: 'b'
                }
                """),
            ("a.bicep", """
                module shared 'shared.bicep' = {
                  name: 'shared'
                }
                """),
            ("b.bicep", """
                module shared 'shared.bicep' = {
                  name: 'shared'
                }
                """),
            ("shared.bicep", """
                output value string = 'leaf'
                """));

        var entryPoint = result.Compilation.GetEntrypointSemanticModel();
        var graph = entryPoint.GetModuleReferenceGraph();

        // main + a + b + shared - shared must be visited exactly once, not twice.
        graph.Nodes.Should().HaveCount(4);
    }

    [TestMethod]
    public void Build_SameModuleReferencedTwiceFromSameFile_VisitsItOnlyOnce()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                module a1 'child.bicep' = {
                  name: 'a1'
                }
                module a2 'child.bicep' = {
                  name: 'a2'
                }
                """),
            ("child.bicep", """
                output value string = 'leaf'
                """));

        var entryPoint = result.Compilation.GetEntrypointSemanticModel();
        var graph = entryPoint.GetModuleReferenceGraph();

        graph.Nodes.Should().HaveCount(2);
    }

    [TestMethod]
    public void Build_BrokenModuleReference_SkipsGracefullyWithoutThrowing()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                module missing 'does-not-exist.bicep' = {
                  name: 'missing'
                }
                """));

        var entryPoint = result.Compilation.GetEntrypointSemanticModel();

        var graph = entryPoint.GetModuleReferenceGraph();

        // The broken reference contributes no node - only the entrypoint itself is present.
        graph.Nodes.Should().HaveCount(1);
        graph.Nodes.Values.Single().Parent.Should().BeNull();
    }

    [TestMethod]
    public void Build_BrokenModuleReferenceAlongsideValidOne_KeepsValidNodeAndSkipsBrokenOne()
    {
        // main references one broken module and one valid module - the broken one shouldn't
        // affect the valid sibling.
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                module missing 'does-not-exist.bicep' = {
                  name: 'missing'
                }
                module valid 'valid.bicep' = {
                  name: 'valid'
                }
                """),
            ("valid.bicep", """
                output value string = 'leaf'
                """));

        var entryPoint = result.Compilation.GetEntrypointSemanticModel();
        var graph = entryPoint.GetModuleReferenceGraph();

        // main + valid - the broken reference contributes no node.
        graph.Nodes.Should().HaveCount(2);
    }

    [TestMethod]
    public void Build_BrokenModuleReferenceDeepInTheChain_DoesNotAffectAncestorsOrThrow()
    {
        // main -> a (valid) -> broken. The break happens two levels down, not at the entrypoint.
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                module a 'a.bicep' = {
                  name: 'a'
                }
                """),
            ("a.bicep", """
                module missing 'does-not-exist.bicep' = {
                  name: 'missing'
                }
                """));

        var entryPoint = result.Compilation.GetEntrypointSemanticModel();
        var graph = entryPoint.GetModuleReferenceGraph();

        // main + a - the broken grandchild contributes no node and doesn't affect "a" itself.
        graph.Nodes.Should().HaveCount(2);
        graph.Nodes.Values.Should().Contain(n => ReferenceEquals(n.Parent, null)); // main
        graph.Nodes.Values.Should().Contain(n => !ReferenceEquals(n.Parent, null)); // a
    }

    [TestMethod]
    public void Build_UnreferencedFileInCompilation_IsNotIncludedInGraph()
    {
        // "orphan.bicep" is in the compilation but unreferenced - the graph only includes files
        // reached via `module`, not every file in the compilation.
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                module a 'a.bicep' = {
                  name: 'a'
                }
                """),
            ("a.bicep", """
                output value string = 'leaf'
                """),
            ("orphan.bicep", """
                output value string = 'unreferenced'
                """));

        var entryPoint = result.Compilation.GetEntrypointSemanticModel();
        var graph = entryPoint.GetModuleReferenceGraph();

        graph.Nodes.Should().HaveCount(2);
        graph.Nodes.Keys.Should().NotContain(uri => uri.ToString().EndsWith("orphan.bicep"));
    }

    [TestMethod]
    public void Build_EntrypointWithNoModules_ContainsOnlyTheEntrypoint()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                output value string = 'leaf'
                """));

        var entryPoint = result.Compilation.GetEntrypointSemanticModel();
        var graph = entryPoint.GetModuleReferenceGraph();

        graph.Nodes.Should().HaveCount(1);
        graph.Nodes.Values.Single().Model.Should().BeSameAs(entryPoint);
    }

    [TestMethod]
    public void Build_EachFileResolvesItsOwnBicepVersionConstraint_NotTheEntrypoints()
    {
        // main.bicep and a.bicep have different bicepconfig.json constraints (>=0.30.0 vs
        // >=0.40.0). Each node's Constraint should reflect its own file, not the entrypoint's.
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                module a 'a/a.bicep' = {
                  name: 'a'
                }
                """),
            ("bicepconfig.json", """{ "bicep": { "version": ">=0.30.0" } }"""),
            ("a/a.bicep", """
                output value string = 'leaf'
                """),
            ("a/bicepconfig.json", """{ "bicep": { "version": ">=0.40.0" } }"""));

        var entryPoint = result.Compilation.GetEntrypointSemanticModel();
        var graph = entryPoint.GetModuleReferenceGraph();

        var entryPointUri = entryPoint.SourceFile.FileHandle.Uri;
        var aUri = graph.Nodes.Keys.Single(uri => uri.ToString().EndsWith("a.bicep"));

        graph.Nodes[entryPointUri].Constraint!.ToString().Should().Be(">=0.30.0");
        graph.Nodes[aUri].Constraint!.ToString().Should().Be(">=0.40.0");
    }

    [TestMethod]
    public async Task Build_RegistryModuleReference_IsExcludedFromGraph()
    {
        var fileSystem = new MockFileSystem();
        var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
            fileSystem,
            new RegistryHelper.ModuleToPublish("br:mockregistry.io/test/published:v1", "output value string = 'leaf'"));

        var services = new ServiceBuilder().WithContainerRegistryClientFactory(clientFactory);
        var result = await CompilationHelper.RestoreAndCompile(
            services,
            ("main.bicep", """
                module fromRegistry 'br:mockregistry.io/test/published:v1' = {
                  name: 'fromRegistry'
                }
                """));

        var entryPoint = result.Compilation.GetEntrypointSemanticModel();
        var graph = entryPoint.GetModuleReferenceGraph();

        graph.Nodes.Should().HaveCount(1);
    }
}
