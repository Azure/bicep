// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Decompiler.Rewriters;
using Bicep.Core.Extensions;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests.ArmHelpers
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

            var compilation = CompilationHelper.CreateCompilation(("main.bicep", bicepFile));
            var rewriter = new TypeCasingFixerRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SyntaxTreeGrouping.EntryPoint.ProgramSyntax);

            // Check that the two references are exactly the same
            newProgramSyntax.Should().BeSameAs(compilation.SyntaxTreeGrouping.EntryPoint.ProgramSyntax);
        }

        [TestMethod]
        public void Casing_issues_are_corrected()
        {
            var bicepFile = @"
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
}";

            var typeDefinition = ResourceTypeProviderHelper.CreateCustomResourceType("My.Rp/resA", "2020-01-01", TypeSymbolValidationFlags.WarnOnTypeMismatch,
                new TypeProperty("lowercaseprop", LanguageConstants.String),
                new TypeProperty("camelCaseProp", LanguageConstants.String),
                new TypeProperty("lowercasequoted=+.prop", LanguageConstants.String),
                new TypeProperty("camelCaseQuoted=+.Prop", LanguageConstants.String),
                new TypeProperty("lowerCaseEnumProp", new StringLiteralType("myenum")),
                new TypeProperty("pascalCaseEnumProp", new StringLiteralType("MyEnum")),
                new TypeProperty("lowerCaseEnumUnionProp", UnionType.Create(new StringLiteralType("myenum"), new StringLiteralType("blahblah"))),
                new TypeProperty("pascalCaseEnumUnionProp", UnionType.Create(new StringLiteralType("MyEnum"), new StringLiteralType("BlahBlah"))));
            var typeProvider = ResourceTypeProviderHelper.CreateMockTypeProvider(typeDefinition.AsEnumerable());

            var compilation = CompilationHelper.CreateCompilation(typeProvider, ("main.bicep", bicepFile));
            var rewriter = new TypeCasingFixerRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SyntaxTreeGrouping.EntryPoint.ProgramSyntax);
            PrintHelper.PrettyPrint(newProgramSyntax).Should().Be(
@"resource resA 'My.Rp/resA@2020-01-01' = {
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
}");
        }
    }
}