// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Decompiler.Rewriters;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests.ArmHelpers
{
    [TestClass]
    public class DependsOnRemovalRewriterTests
    {
        [TestMethod]
        public void Unneccessary_dependsOn_statements_are_removed()
        {
            var bicepFile = @"
resource resA 'My.Rp/resA@2020-01-01' = {
  name: 'resA'
}

resource resB 'My.Rp/resB@2020-01-01' = {
  name: 'resB'
  properties: {
    resA: resA.name
  }
  dependsOn: [
    resA
  ]
}

var varA = resA.name
var varB = {
  resA: varA
  resB: resB.name
}

resource resC 'My.Rp/resB@2020-01-01' = {
  name: 'resC'
  properties: {
    resA: varB
  }
  dependsOn: [
    resA
    resB
  ]
}";

            var compilation = CompilationHelper.CreateCompilation(("main.bicep", bicepFile));
            var rewriter = new DependsOnRemovalRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SyntaxTreeGrouping.EntryPoint.ProgramSyntax);
            PrintHelper.PrettyPrint(newProgramSyntax).Should().Be(
@"resource resA 'My.Rp/resA@2020-01-01' = {
  name: 'resA'
}

resource resB 'My.Rp/resB@2020-01-01' = {
  name: 'resB'
  properties: {
    resA: resA.name
  }
}

var varA = resA.name
var varB = {
  resA: varA
  resB: resB.name
}

resource resC 'My.Rp/resB@2020-01-01' = {
  name: 'resC'
  properties: {
    resA: varB
  }
}");
        }

        [TestMethod]
        public void Necessary_dependsOn_statements_are_not_removed()
        {
            var bicepFile = @"
resource resA 'My.Rp/resA@2020-01-01' = {
  name: 'resA'
}

resource resB 'My.Rp/resB@2020-01-01' = {
  name: 'resB'
  dependsOn: [
    resA
  ]
}";

            var compilation = CompilationHelper.CreateCompilation(("main.bicep", bicepFile));
            var rewriter = new DependsOnRemovalRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SyntaxTreeGrouping.EntryPoint.ProgramSyntax);
            PrintHelper.PrettyPrint(newProgramSyntax).Should().Be(
@"resource resA 'My.Rp/resA@2020-01-01' = {
  name: 'resA'
}

resource resB 'My.Rp/resB@2020-01-01' = {
  name: 'resB'
  dependsOn: [
    resA
  ]
}");
        }

        [TestMethod]
        public void ProgramSyntax_is_not_modified_if_no_changes_are_applied()
        {
          var bicepFile = @"
resource resA 'My.Rp/resA@2020-01-01' = {
  name: 'resA'
}";

            var compilation = CompilationHelper.CreateCompilation(("main.bicep", bicepFile));
            var rewriter = new DependsOnRemovalRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SyntaxTreeGrouping.EntryPoint.ProgramSyntax);

            // Check that the two references are exactly the same
            newProgramSyntax.Should().BeSameAs(compilation.SyntaxTreeGrouping.EntryPoint.ProgramSyntax);
        }
    }
}