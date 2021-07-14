// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Decompiler.Rewriters;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests.ArmHelpers
{
    [TestClass]
    public class ForExpressionSimplifierRewriterTests
    {
        [TestMethod]
        public void Length_based_for_loop_is_rewritten()
        {
            var bicepFile = @"
var items = [
  'a'
  'b'
  'c'
]
output test array = [for i in range(0, length(items)): {
  name: '${items[i]}'
  value: items[i]
}]
";

            var (_, _, compilation) = CompilationHelper.Compile(("main.bicep", bicepFile));
            var rewriter = new ForExpressionSimplifierRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);
            PrintHelper.PrintAndCheckForParseErrors(newProgramSyntax).Should().Be(
@"var items = [
  'a'
  'b'
  'c'
]
output test array = [for item in items: {
  name: '${item}'
  value: item
}]");
        }

        [TestMethod]
        public void Length_based_for_loop_with_index_access_is_rewritten()
        {
            var bicepFile = @"
var items = [
  'a'
  'b'
  'c'
]
output test array = [for i in range(0, length(items)): {
  name: '${items[i]}'
  value: items[i]
  index: i
}]
";

            var (_, _, compilation) = CompilationHelper.Compile(("main.bicep", bicepFile));
            var rewriter = new ForExpressionSimplifierRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);
            PrintHelper.PrintAndCheckForParseErrors(newProgramSyntax).Should().Be(
@"var items = [
  'a'
  'b'
  'c'
]
output test array = [for (item, i) in items: {
  name: '${item}'
  value: item
  index: i
}]");
        }

        [TestMethod]
        public void Loop_inside_a_loop()
        {
            var bicepFile = @"
var vmNames = [
  'vm1'
  'vm2'
]

var dataDisks = [
  {
    lun: 0
    diskSizeGb: 1023
  }
  {
    lun: 1
    diskSizeGb: 1023
  }
]

resource vmsLoop 'Microsoft.Compute/virtualMachines@2020-06-01' = [for i in range(0, length(vmNames)): {
  name: '${vmPrefix}-${vmNames[i]}'
  location: resourceGroup().location
  properties: {
    hardwareProfile: {
      vmSize: vmSize
    }
    osProfile: {
      computerName: '${vmPrefix}-${vmNames[i]}'
      adminUsername: 'vmadmin'
      adminPassword: adminPassword
    }
    storageProfile: {
      dataDisks: [for j in range(0, length(dataDisks)): {
        diskSizeGB: dataDisks[j].diskSizeGb
        lun: dataDisks[j].lun
        createOption: 'Empty'
      }]
    }
  }
}]
";

            var (_, _, compilation) = CompilationHelper.Compile(("main.bicep", bicepFile));
            var rewriter = new ForExpressionSimplifierRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);
            PrintHelper.PrintAndCheckForParseErrors(newProgramSyntax).Should().Be(
@"var vmNames = [
  'vm1'
  'vm2'
]

var dataDisks = [
  {
    lun: 0
    diskSizeGb: 1023
  }
  {
    lun: 1
    diskSizeGb: 1023
  }
]

resource vmsLoop 'Microsoft.Compute/virtualMachines@2020-06-01' = [for item_1 in vmNames: {
  name: '${vmPrefix}-${item_1}'
  location: resourceGroup().location
  properties: {
    hardwareProfile: {
      vmSize: vmSize
    }
    osProfile: {
      computerName: '${vmPrefix}-${item_1}'
      adminUsername: 'vmadmin'
      adminPassword: adminPassword
    }
    storageProfile: {
      dataDisks: [for item in dataDisks: {
        diskSizeGB: item.diskSizeGb
        lun: item.lun
        createOption: 'Empty'
      }]
    }
  }
}]");
        }

        [TestMethod]
        public void Length_based_for_loop_with_anything_other_than_variable_access_cannot_be_rewritten()
        {
            // without the ability to perform a deep syntax comparison, this scenario (somewhat common in decompiled templates)
            // is not supported :(

            var bicepFile = @"
var items = [
  'a'
  'b'
  'c'
]
var container = {
  items: items
}
output test array = [for i in range(0, length(container.items)): {
  name: '${container.items[i]}'
  value: container.items[i]
  index: i
}]
";

            var (_, _, compilation) = CompilationHelper.Compile(("main.bicep", bicepFile));
            var rewriter = new ForExpressionSimplifierRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);

            // Reference equality check to ensure we're not regenerating syntax unnecessarily
            newProgramSyntax.Should().BeSameAs(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);
        }

        [TestMethod]
        public void Length_based_for_loop_is_rewritten_using_namespaces()
        {
            var bicepFile = @"
var items = [
  'a'
  'b'
  'c'
]
output test array = [for i in sys.range(0, sys.length(items)): {
  name: '${items[i]}'
  value: items[i]
}]
";

            var (_, _, compilation) = CompilationHelper.Compile(("main.bicep", bicepFile));
            var rewriter = new ForExpressionSimplifierRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);
            PrintHelper.PrintAndCheckForParseErrors(newProgramSyntax).Should().Be(
@"var items = [
  'a'
  'b'
  'c'
]
output test array = [for item in items: {
  name: '${item}'
  value: item
}]");
        }

        [TestMethod]
        public void Length_based_for_loop_is_not_rewritten_with_incorrect_namespace()
        {
            var bicepFile = @"
var items = [
  'a'
  'b'
  'c'
]

// oops - we've used the wrong namespace!
output test array = [for i in az.range(0, az.length(items)): {
  name: '${items[i]}'
  value: items[i]
}]
";

            var (_, _, compilation) = CompilationHelper.Compile(("main.bicep", bicepFile));
            var rewriter = new ForExpressionSimplifierRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);

            // Reference equality check to ensure syntax has not been modified
            newProgramSyntax.Should().BeSameAs(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);
        }
    }
}