// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts.Restore;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Compilation;
using Bicep.LanguageServer.Features.Custom.Visualization;
using Bicep.LanguageServer.Features.Custom.Visualization.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.UnitTests.Features.Visualization;

[TestClass]
public class VisualGraphBuilderTests
{
    [TestMethod]
    public void Build_ResourceWithEmptyPropertyValue_LinksParserErrorToResourceNode()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithAzResources(BuiltInTestTypes.Types),
            """
            resource basicTest 'Test.Rp/basicTests@2020-01-01' = {
              name:
            }
            """);
        var context = new CompilationContext(result.Compilation);

        var graph = CreateBuilder().Build(context).Graph;

        graph.Nodes.Should().ContainSingle().Which.HasError.Should().BeTrue();
    }

    [TestMethod]
    public void Build_ErrorAfterResource_DoesNotLinkErrorToResourceNode()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithAzResources(BuiltInTestTypes.Types),
            """
            resource basicTest 'Test.Rp/basicTests@2020-01-01' = {
              name: 'example'
            }

            var invalid =
            """);
        var context = new CompilationContext(result.Compilation);

        var graph = CreateBuilder().Build(context).Graph;

        graph.Nodes.Should().ContainSingle().Which.HasError.Should().BeFalse();
    }

    [TestMethod]
    public void Build_LocalModule_ExpandsAndRevealsItsFile()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                module inner 'modules/inner.bicep' = [for i in range(0, 2): {
                  name: 'inner${i}'
                }]
                """),
            ("modules/inner.bicep", """
                resource nsg 'Microsoft.Network/networkSecurityGroups@2023-04-01' = {
                  name: 'nsg'
                }
                """));

        var context = new CompilationContext(result.Compilation);
        var builder = CreateBuilder();
        var (graph, sources) = builder.Build(context);

        graph.Nodes.Should().Contain(node => node.Id == "inner" && node.HasChildren && node.IsCollection && node.SourceStatus == null && !node.IsRemote);
        graph.Nodes.Should().Contain(node => node.Id == "inner::nsg" && node.ParentId == "inner");
        sources["inner"].Uri.Should().StartWith("file:").And.EndWith("/main.bicep");
        sources["inner::nsg"].Uri.Should().StartWith("file:").And.EndWith("/modules/inner.bicep");
        graph.Nodes.Should().AllSatisfy(node => builder.TryGetRestorableReference(context, node.Id).Should().BeNull());
    }

    [TestMethod]
    public void Build_ChildResources_AreDrawnInsideTheirParentAndKeepTheirIds()
    {
        var result = CompilationHelper.Compile("""
            resource vnet 'Microsoft.Network/virtualNetworks@2023-04-01' = {
              name: 'vnet'

              resource app 'subnets' = {
                name: 'app'
              }
            }

            resource data 'Microsoft.Network/virtualNetworks/subnets@2023-04-01' = {
              parent: vnet
              name: 'data'
            }

            resource nic 'Microsoft.Network/networkInterfaces@2023-04-01' = {
              name: 'nic'
              properties: {
                ipConfigurations: [{ name: 'ip', properties: { subnet: { id: vnet::app.id } } }]
              }
            }

            resource vm 'Microsoft.Compute/virtualMachines@2023-03-01' = {
              name: 'vm'
              properties: {
                networkProfile: { networkInterfaces: [{ id: nic.id }] }
              }
            }
            """);

        var graph = CreateBuilder().Build(new CompilationContext(result.Compilation)).Graph;
        var nodes = graph.Nodes.ToDictionary(node => node.Id);

        nodes["vnet"].Should().Match<GraphNode>(node => node.Kind == GraphNodeKind.Resource && node.HasChildren && node.ParentId == null);
        nodes["app"].ParentId.Should().Be("vnet");
        nodes["data"].ParentId.Should().Be("vnet");

        // A resource with nothing inside it, or a property reference to a subnet, makes no container.
        nodes["vm"].HasChildren.Should().BeFalse();
        nodes["nic"].Should().Match<GraphNode>(node => !node.HasChildren && node.ParentId == null);

        // A child's dependency on its parent is implied; a dependency into the container crosses its boundary.
        graph.Edges.Select(edge => edge.Id).Should().BeEquivalentTo("nic->app", "vm->nic");
    }

    [TestMethod]
    public void Build_DeclarationsScopedToAResourceGroup_AreDrawnInsideIt()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                targetScope = 'subscription'

                resource rg 'Microsoft.Resources/resourceGroups@2023-07-01' = {
                  name: 'rg'
                  location: 'westus'
                }

                module inside 'inner.bicep' = {
                  name: 'inside'
                  scope: rg
                }

                module byName 'inner.bicep' = {
                  name: 'byName'
                  scope: resourceGroup(rg.name)
                }
                """),
            ("inner.bicep", """
                resource nsg 'Microsoft.Network/networkSecurityGroups@2023-04-01' = {
                  name: 'nsg'
                }
                """));

        var graph = CreateBuilder().Build(new CompilationContext(result.Compilation)).Graph;
        var nodes = graph.Nodes.ToDictionary(node => node.Id);

        nodes["rg"].HasChildren.Should().BeTrue();
        nodes["inside"].ParentId.Should().Be("rg");
        nodes["inside::nsg"].ParentId.Should().Be("inside");

        // A scope that only computes the resource group's name may name another group, so it is a dependency instead.
        nodes["byName"].ParentId.Should().BeNull();
        graph.Edges.Select(edge => edge.Id).Should().BeEquivalentTo("byName->rg");
    }

    [TestMethod]
    public void Build_ResourceInsideACollection_IsNotContained()
    {
        var result = CompilationHelper.Compile("""
            resource vnets 'Microsoft.Network/virtualNetworks@2023-04-01' = [for i in range(0, 2): {
              name: 'vnet${i}'
            }]

            resource subnet 'Microsoft.Network/virtualNetworks/subnets@2023-04-01' = {
              parent: vnets[0]
              name: 'subnet'
            }
            """);

        var graph = CreateBuilder().Build(new CompilationContext(result.Compilation)).Graph;
        var nodes = graph.Nodes.ToDictionary(node => node.Id);

        nodes["vnets"].HasChildren.Should().BeFalse();
        nodes["subnet"].ParentId.Should().BeNull();
        graph.Edges.Select(edge => edge.Id).Should().BeEquivalentTo("subnet->vnets");
    }

    internal static VisualGraphBuilder CreateBuilder(IDependencyHelper? services = null)
    {
        services ??= new ServiceBuilder().Build();

        return new(services.GetCompiler(), services.Construct<IArtifactRestorer>());
    }
}
