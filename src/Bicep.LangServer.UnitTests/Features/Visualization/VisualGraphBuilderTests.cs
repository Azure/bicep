// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Compilation;
using Bicep.LanguageServer.Features.Custom.Visualization;
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

        graph.Nodes.Should().Contain(node => node.Id == "inner" && node.HasChildren && node.IsCollection && node.SourceStatus == null);
        graph.Nodes.Should().Contain(node => node.Id == "inner::nsg" && node.ParentId == "inner");
        sources["inner"].Uri.Should().StartWith("file:").And.EndWith("/main.bicep");
        sources["inner::nsg"].Uri.Should().StartWith("file:").And.EndWith("/modules/inner.bicep");
        graph.Nodes.Should().AllSatisfy(node => builder.TryGetRestorableReference(context, node.Id).Should().BeNull());
    }

    internal static VisualGraphBuilder CreateBuilder(IDependencyHelper? services = null)
    {
        services ??= new ServiceBuilder().Build();

        return new(services.GetCompiler(), services.Construct<IModuleDispatcher>());
    }
}
