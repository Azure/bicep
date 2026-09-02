// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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

        var graph = VisualGraphBuilder.Build(context, result.SourceFile.FileHandle.Uri);

        graph.ErrorCount.Should().BeGreaterThan(0);
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

        var graph = VisualGraphBuilder.Build(context, result.SourceFile.FileHandle.Uri);

        graph.ErrorCount.Should().BeGreaterThan(0);
        graph.Nodes.Should().ContainSingle().Which.HasError.Should().BeFalse();
    }
}
