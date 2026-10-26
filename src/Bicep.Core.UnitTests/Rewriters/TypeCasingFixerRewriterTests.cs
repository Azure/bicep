// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Extensions;
using Bicep.Core.Rewriters;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests.Utils;
using Bicep.Testing;
using Bicep.Testing.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Rewriters
{
    [TestClass]
    public class TypeCasingFixerRewriterTests
    {
        [TestMethod]
        public void ProgramSyntax_is_not_modified_if_no_changes_are_applied()
        {
            var bicepFile = @"
resource resA 'My.Rp/resA@2020-01-01' = {
  name: 'resA'
}";

            var compilation = Compile(bicepFile);
            var rewriter = new TypeCasingFixerRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);

            // Reference equality check to ensure we're not regenerating syntax unnecessarily
            newProgramSyntax.Should().BeSameAs(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);
        }

        [TestMethod]
        public void Casing_issues_are_corrected()
        {
            var bicepFile = """
                resource resA 'My.Rp/resA@2020-01-01' = {
                  name: 'resA'
                  properties: {
                    lowerCaseProp: 'abc'
                    camelcaseprop: 'def'
                    'lowerCaseQuoted=+.Prop': 'ghi'
                    'camelcasequoted=+.prop': 'jkl'
                    lowerCaseEnumProp: 'MyEnum'
                    pascalCaseEnumProp: 'myenum'
                    lowerCaseEnumUnionProp: 'MyEnum'
                    pascalCaseEnumUnionProp: 'myenum'
                  }
                }

                output myObj object = {
                  lowerCaseProp: resA.properties.lowerCaseProp
                  camelcaseprop: resA.properties.camelcaseprop
                }
                """;

            var typeDefinition = TestTypeHelper.CreateCustomResourceType("My.Rp/resA", "2020-01-01", TypeSymbolValidationFlags.WarnOnTypeMismatch,
                new NamedTypeProperty("lowercaseprop", LanguageConstants.String),
                new NamedTypeProperty("camelCaseProp", LanguageConstants.String),
                new NamedTypeProperty("lowercasequoted=+.prop", LanguageConstants.String),
                new NamedTypeProperty("camelCaseQuoted=+.Prop", LanguageConstants.String),
                new NamedTypeProperty("lowerCaseEnumProp", TypeFactory.CreateStringLiteralType("myenum")),
                new NamedTypeProperty("pascalCaseEnumProp", TypeFactory.CreateStringLiteralType("MyEnum")),
                new NamedTypeProperty("lowerCaseEnumUnionProp", TypeHelper.CreateTypeUnion(TypeFactory.CreateStringLiteralType("myenum"), TypeFactory.CreateStringLiteralType("blahblah"))),
                new NamedTypeProperty("pascalCaseEnumUnionProp", TypeHelper.CreateTypeUnion(TypeFactory.CreateStringLiteralType("MyEnum"), TypeFactory.CreateStringLiteralType("BlahBlah"))));
            var typeLoader = TestTypeHelper.CreateResourceTypeLoaderWithTypes(typeDefinition.AsEnumerable());

            var compilation = Compile(bicepFile, typeLoader);
            var rewriter = new TypeCasingFixerRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);
            TestPrinter.Print(newProgramSyntax).Should().BeValidBicepText(
                """
                resource resA 'My.Rp/resA@2020-01-01' = {
                  name: 'resA'
                  properties: {
                    lowercaseprop: 'abc'
                    camelCaseProp: 'def'
                    'lowercasequoted=+.prop': 'ghi'
                    'camelCaseQuoted=+.Prop': 'jkl'
                    lowerCaseEnumProp: 'myenum'
                    pascalCaseEnumProp: 'MyEnum'
                    lowerCaseEnumUnionProp: 'myenum'
                    pascalCaseEnumUnionProp: 'MyEnum'
                  }
                }

                output myObj object = {
                  lowerCaseProp: resA.properties.lowercaseprop
                  camelcaseprop: resA.properties.camelCaseProp
                }

                """);
        }

        [TestMethod]
        public void Nested_casing_issues_take_multiple_passes_to_correct()
        {
            var bicepFile = """
                resource resA 'My.Rp/resA@2020-01-01' = {
                  name: 'resA'
                  properties: {
                    lowerCaseObj: {
                      lowerCaseStr: 'test'
                    }
                  }
                }

                output myObj object = {
                  lowerCaseProp: resA.properties.lowerCaseObj.lowerCaseStr
                }
                """;

            var typeDefinition = TestTypeHelper.CreateCustomResourceType("My.Rp/resA", "2020-01-01", TypeSymbolValidationFlags.WarnOnTypeMismatch,
                new NamedTypeProperty("lowercaseobj", new ObjectType("lowercaseobj", TypeSymbolValidationFlags.Default, new[] {
                  new NamedTypeProperty("lowercasestr", LanguageConstants.String)
                }, null)));
            var typeLoader = TestTypeHelper.CreateResourceTypeLoaderWithTypes(typeDefinition.AsEnumerable());

            var compilation = Compile(bicepFile, typeLoader);
            var rewriter = new TypeCasingFixerRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);
            var firstPassBicepFile = TestPrinter.Print(newProgramSyntax);
            firstPassBicepFile.Should().BeValidBicepText(
                """
                resource resA 'My.Rp/resA@2020-01-01' = {
                  name: 'resA'
                  properties: {
                    lowercaseobj: {
                      lowerCaseStr: 'test'
                    }
                  }
                }

                output myObj object = {
                  lowerCaseProp: resA.properties.lowercaseobj.lowerCaseStr
                }

                """);

            compilation = Compile(firstPassBicepFile, typeLoader);
            rewriter = new TypeCasingFixerRewriter(compilation.GetEntrypointSemanticModel());

            newProgramSyntax = rewriter.Rewrite(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);
            TestPrinter.Print(newProgramSyntax).Should().BeValidBicepText(
                """
                resource resA 'My.Rp/resA@2020-01-01' = {
                  name: 'resA'
                  properties: {
                    lowercaseobj: {
                      lowercasestr: 'test'
                    }
                  }
                }

                output myObj object = {
                  lowerCaseProp: resA.properties.lowercaseobj.lowercasestr
                }

                """);
        }

        private static Compilation Compile(string bicepFile) =>
            TestCompiler.ForInMemoryCompilation().CompileWithoutRestore(bicepFile).Compilation;

        private static Compilation Compile(string bicepFile, IResourceTypeLoader typeLoader) => TestCompiler
            .ForInMemoryCompilation()
            .WithAzResourceTypeLoader(typeLoader)
            .CompileWithoutRestore(bicepFile)
            .Compilation;
    }
}
