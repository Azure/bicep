// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Extensions;
using Bicep.Core.Rewriters;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests.Utils;
using Bicep.Testing;
using Bicep.Testing.Assertions;
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

            var compilation = TestCompiler.ForInMemoryCompilation().CompileWithoutRestore(bicepFile).Compilation;
            var rewriter = new ReadOnlyPropertyRemovalRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);

            // Reference equality check to ensure we're not regenerating syntax unnecessarily
            newProgramSyntax.Should().BeSameAs(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);
        }

        [TestMethod]
        public void Readonly_properties_are_removed()
        {
            var bicepFile = """
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

                """;

            var typeDefinition = TestTypeHelper.CreateCustomResourceType("My.Rp/resA", "2020-01-01", TypeSymbolValidationFlags.WarnOnTypeMismatch,
                new NamedTypeProperty("readOnlyProp", LanguageConstants.String, TypePropertyFlags.ReadOnly),
                new NamedTypeProperty("readWriteProp", LanguageConstants.String, TypePropertyFlags.None),
                new NamedTypeProperty("writeOnlyProp", LanguageConstants.String, TypePropertyFlags.WriteOnly));
            var typeLoader = TestTypeHelper.CreateResourceTypeLoaderWithTypes(typeDefinition.AsEnumerable());

            var compilation = TestCompiler
                .ForInMemoryCompilation()
                .WithAzResourceTypeLoader(typeLoader)
                .CompileWithoutRestore(bicepFile)
                .Compilation;
            var rewriter = new ReadOnlyPropertyRemovalRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);
            TestPrinter.Print(newProgramSyntax).Should().BeValidBicepText(
                """
                resource resA 'My.Rp/resA@2020-01-01' = {
                  name: 'resA'
                  properties: {
                    readWriteProp: 'def'
                    writeOnlyProp: 'ghi'
                  }
                }

                output myObj object = {
                  readOnlyProp: resA.properties.readOnlyProp
                  readWriteProp: resA.properties.readWriteProp
                }

                """);
        }
    }
}
