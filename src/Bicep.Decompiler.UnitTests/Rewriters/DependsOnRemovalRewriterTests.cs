// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Rewriters;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests.ArmHelpers
{
    [TestClass]
    public class DependsOnRemovalRewriterTests
    {
        [TestMethod]
        public void Unnecessary_dependsOn_statements_are_removed()
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

            var (_, _, compilation) = CompilationHelper.Compile(("main.bicep", bicepFile));
            var rewriter = new DependsOnRemovalRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);
            PrintHelper.PrintAndCheckForParseErrors(newProgramSyntax).Should().Be(
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
        public void Unneccessary_dependsOn_statements_are_removed_for_modules()
        {
            var bicepFile = @"
resource resA 'My.Rp/resA@2020-01-01' = {
  name: 'resA'
}

module modB 'modb.bicep' = {
  name: 'modB'
  params: {
    resA: resA.name
  }
  dependsOn: [
    resA
  ]
}

var varA = resA.name
var varB = {
  resA: varA
  modB: modB.name
}

module modC 'modC.bicep' = {
  name: 'modC'
  params: {
    resA: varB
  }
  dependsOn: [
    resA
    modB
  ]
}";

            var (_, _, compilation) = CompilationHelper.Compile(("main.bicep", bicepFile));
            var rewriter = new DependsOnRemovalRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);
            PrintHelper.PrintAndCheckForParseErrors(newProgramSyntax).Should().Be(
@"resource resA 'My.Rp/resA@2020-01-01' = {
  name: 'resA'
}

module modB 'modb.bicep' = {
  name: 'modB'
  params: {
    resA: resA.name
  }
}

var varA = resA.name
var varB = {
  resA: varA
  modB: modB.name
}

module modC 'modC.bicep' = {
  name: 'modC'
  params: {
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

            var (_, _, compilation) = CompilationHelper.Compile(("main.bicep", bicepFile));
            var rewriter = new DependsOnRemovalRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);
            PrintHelper.PrintAndCheckForParseErrors(newProgramSyntax).Should().Be(
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
        public void Necessary_dependsOn_statements_are_not_removed_for_modules()
        {
            var bicepFile = @"
resource resA 'My.Rp/resA@2020-01-01' = {
  name: 'resA'
}

module modB 'modb.bicep' = {
  name: 'modB'
  dependsOn: [
    resA
  ]
}";

            var (_, _, compilation) = CompilationHelper.Compile(("main.bicep", bicepFile));
            var rewriter = new DependsOnRemovalRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);
            PrintHelper.PrintAndCheckForParseErrors(newProgramSyntax).Should().Be(
@"resource resA 'My.Rp/resA@2020-01-01' = {
  name: 'resA'
}

module modB 'modb.bicep' = {
  name: 'modB'
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

            var (_, _, compilation) = CompilationHelper.Compile(("main.bicep", bicepFile));
            var rewriter = new DependsOnRemovalRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);

            // Reference equality check to ensure we're not regenerating syntax unnecessarily
            newProgramSyntax.Should().BeSameAs(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);
        }
    }
}
