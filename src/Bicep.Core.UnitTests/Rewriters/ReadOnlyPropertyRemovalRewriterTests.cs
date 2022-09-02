// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Extensions;
using Bicep.Core.Rewriters;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Rewriters
{
    [TestClass]
    public class ReadOnlyPropertyRemovalRewriterTests
    {
        [TestMethod]
        public void ProgramSyntax_is_not_modified_if_no_changes_are_applied()
        {
            var bicepFile = @"
resource resA 'My.Rp/resA@2020-01-01' = {
  name: 'resA'
}";

            var (_, _, compilation) = CompilationHelper.Compile(("main.bicep", bicepFile));
            var rewriter = new ReadOnlyPropertyRemovalRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);

            // Reference equality check to ensure we're not regenerating syntax unnecessarily
            newProgramSyntax.Should().BeSameAs(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);
        }

        [TestMethod]
        public void Readonly_properties_are_removed()
        {
            var bicepFile = @"
resource resA 'My.Rp/resA@2020-01-01' = {
  name: 'resA'
  properties: {
    readOnlyProp: 'abc'
    readWriteProp: 'def'
    writeOnlyProp: 'ghi'
  }
}

output myObj object = {
  readOnlyProp: resA.properties.readOnlyProp
  readWriteProp: resA.properties.readWriteProp
}
";

            var typeDefinition = TestTypeHelper.CreateCustomResourceType("My.Rp/resA", "2020-01-01", TypeSymbolValidationFlags.WarnOnTypeMismatch,
                new TypeProperty("readOnlyProp", LanguageConstants.String, TypePropertyFlags.ReadOnly),
                new TypeProperty("readWriteProp", LanguageConstants.String, TypePropertyFlags.None),
                new TypeProperty("writeOnlyProp", LanguageConstants.String, TypePropertyFlags.WriteOnly));
            var typeLoader = TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(typeDefinition.AsEnumerable());

            var (_, _, compilation) = CompilationHelper.Compile(typeLoader, ("main.bicep", bicepFile));
            var rewriter = new ReadOnlyPropertyRemovalRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);
            PrintHelper.PrintAndCheckForParseErrors(newProgramSyntax).Should().Be(
@"resource resA 'My.Rp/resA@2020-01-01' = {
  name: 'resA'
  properties: {
    readWriteProp: 'def'
    writeOnlyProp: 'ghi'
  }
}

output myObj object = {
  readOnlyProp: resA.properties.readOnlyProp
  readWriteProp: resA.properties.readWriteProp
}");
        }
    }
}
