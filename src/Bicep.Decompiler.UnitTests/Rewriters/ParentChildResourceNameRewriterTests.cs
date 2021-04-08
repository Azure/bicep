// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Decompiler.Rewriters;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests.ArmHelpers
{
    [TestClass]
    public class ParentChildResourceNameRewriterTests
    {
        [TestMethod]
        public void Parent_syntax_common_variable_reference_can_be_replaced()
        {
            var bicepFile = @"
var parentName = 'resA'
        
resource resA 'My.Rp/resA@2020-01-01' = {
  name: parentName
}

resource resB 'My.Rp/resA/childB@2020-01-01' = {
  name: '${parentName}/resB'
  dependsOn: [
    resA
  ]
}";

            var (_, _, compilation) = CompilationHelper.Compile(("main.bicep", bicepFile));
            var rewriter = new ParentChildResourceNameRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SyntaxTreeGrouping.EntryPoint.ProgramSyntax);
            PrintHelper.PrintAndCheckForParseErrors(newProgramSyntax).Should().Be(
@"var parentName = 'resA'

resource resA 'My.Rp/resA@2020-01-01' = {
  name: parentName
}

resource resB 'My.Rp/resA/childB@2020-01-01' = {
  name: '${resA.name}/resB'
  dependsOn: [
    resA
  ]
}");
        }

        [TestMethod]
        public void Parent_syntax_common_variable_reference_in_string_can_be_replaced()
        {
            var bicepFile = @"
var parentName = 'resA'
        
resource resA 'My.Rp/resA@2020-01-01' = {
  name: '${parentName}'
}

resource resB 'My.Rp/resA/childB@2020-01-01' = {
  name: '${parentName}/resB'
  dependsOn: [
    resA
  ]
}";

            var (_, _, compilation) = CompilationHelper.Compile(("main.bicep", bicepFile));
            var rewriter = new ParentChildResourceNameRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SyntaxTreeGrouping.EntryPoint.ProgramSyntax);
            PrintHelper.PrintAndCheckForParseErrors(newProgramSyntax).Should().Be(
@"var parentName = 'resA'

resource resA 'My.Rp/resA@2020-01-01' = {
  name: '${parentName}'
}

resource resB 'My.Rp/resA/childB@2020-01-01' = {
  name: '${resA.name}/resB'
  dependsOn: [
    resA
  ]
}");
        }

        [TestMethod]
        public void Parent_syntax_common_multiple_variable_references_in_string_can_be_replaced()
        {
            var bicepFile = @"
param parentName string = 'resA'
var parentSuffix = 'suffix'
var test = 'hello'

resource resA 'My.Rp/resA@2020-01-01' = {
  name: 'a${parentName}b${parentSuffix}'
}

resource resB 'My.Rp/resA/childB@2020-01-01' = {
  name: 'a${parentName}b${parentSuffix}/${test}'
  dependsOn: [
    resA
  ]
}

resource resC 'My.Rp/resA/childB/childC@2020-01-01' = {
  name: 'a${parentName}b${parentSuffix}/${test}/test'
  dependsOn: [
    resB
  ]
}";

            var (_, _, compilation) = CompilationHelper.Compile(("main.bicep", bicepFile));
            var rewriter = new ParentChildResourceNameRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SyntaxTreeGrouping.EntryPoint.ProgramSyntax);
            PrintHelper.PrintAndCheckForParseErrors(newProgramSyntax).Should().Be(
@"param parentName string = 'resA'
var parentSuffix = 'suffix'
var test = 'hello'

resource resA 'My.Rp/resA@2020-01-01' = {
  name: 'a${parentName}b${parentSuffix}'
}

resource resB 'My.Rp/resA/childB@2020-01-01' = {
  name: '${resA.name}/${test}'
  dependsOn: [
    resA
  ]
}

resource resC 'My.Rp/resA/childB/childC@2020-01-01' = {
  name: '${resB.name}/test'
  dependsOn: [
    resB
  ]
}");
        }

        [TestMethod]
        public void Mismatching_resource_type_will_not_be_replaced()
        {
            var bicepFile = @"
var parentName = 'resA'
        
resource resA 'My.Rp/resA@2020-01-01' = {
  name: '${parentName}'
}

resource resB 'My.Rp/resB/childB@2020-01-01' = {
  name: '${parentName}/resB'
  dependsOn: [
    resA
  ]
}";

            var (_, _, compilation) = CompilationHelper.Compile(("main.bicep", bicepFile));
            var rewriter = new ParentChildResourceNameRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SyntaxTreeGrouping.EntryPoint.ProgramSyntax);
            PrintHelper.PrintAndCheckForParseErrors(newProgramSyntax).Should().Be(
@"var parentName = 'resA'

resource resA 'My.Rp/resA@2020-01-01' = {
  name: '${parentName}'
}

resource resB 'My.Rp/resB/childB@2020-01-01' = {
  name: '${parentName}/resB'
  dependsOn: [
    resA
  ]
}");
        }

        [TestMethod]
        public void Parent_resources_with_expression_names_are_identified()
        {
            // this is a minimal repro for https://github.com/Azure/bicep/issues/2008
            var bicepFile = @"
var resAName = 'resA'
var resBName = 'resB'

resource resA 'My.Rp/parent@2020-01-01' = {
  name: '${resAName}'
}

resource resB 'My.Rp/parent@2020-01-01' = {
  name: '${resBName}'
}

resource childA 'My.Rp/parent/child@2020-01-01' = {
  name: '${resAName}/child'
}

resource childB 'My.Rp/parent/child@2020-01-01' = {
  name: '${resBName}/child'
}";

            var (_, _, compilation) = CompilationHelper.Compile(("main.bicep", bicepFile));
            var rewriter = new ParentChildResourceNameRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SyntaxTreeGrouping.EntryPoint.ProgramSyntax);
            PrintHelper.PrintAndCheckForParseErrors(newProgramSyntax).Should().Be(
@"var resAName = 'resA'
var resBName = 'resB'

resource resA 'My.Rp/parent@2020-01-01' = {
  name: '${resAName}'
}

resource resB 'My.Rp/parent@2020-01-01' = {
  name: '${resBName}'
}

resource childA 'My.Rp/parent/child@2020-01-01' = {
  name: '${resA.name}/child'
}

resource childB 'My.Rp/parent/child@2020-01-01' = {
  name: '${resB.name}/child'
}");
        }
    }
}