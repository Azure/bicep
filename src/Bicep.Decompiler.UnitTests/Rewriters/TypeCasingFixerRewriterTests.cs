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

            var (_, _, compilation) = CompilationHelper.Compile(("main.bicep", bicepFile));
            var rewriter = new TypeCasingFixerRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);

            // Reference equality check to ensure we're not regenerating syntax unnecessarily
            newProgramSyntax.Should().BeSameAs(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);
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
}

output myObj object = {
  lowerCaseProp: resA.properties.lowerCaseProp
  camelcaseprop: resA.properties.camelcaseprop
}
";

            var typeDefinition = TestTypeHelper.CreateCustomResourceType("My.Rp/resA", "2020-01-01", TypeSymbolValidationFlags.WarnOnTypeMismatch,
                new TypeProperty("lowercaseprop", LanguageConstants.String),
                new TypeProperty("camelCaseProp", LanguageConstants.String),
                new TypeProperty("lowercasequoted=+.prop", LanguageConstants.String),
                new TypeProperty("camelCaseQuoted=+.Prop", LanguageConstants.String),
                new TypeProperty("lowerCaseEnumProp", new StringLiteralType("myenum")),
                new TypeProperty("pascalCaseEnumProp", new StringLiteralType("MyEnum")),
                new TypeProperty("lowerCaseEnumUnionProp", UnionType.Create(new StringLiteralType("myenum"), new StringLiteralType("blahblah"))),
                new TypeProperty("pascalCaseEnumUnionProp", UnionType.Create(new StringLiteralType("MyEnum"), new StringLiteralType("BlahBlah"))));
            var typeLoader = TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(typeDefinition.AsEnumerable());

            var (_, _, compilation) = CompilationHelper.Compile(typeLoader, ("main.bicep", bicepFile));
            var rewriter = new TypeCasingFixerRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);
            PrintHelper.PrintAndCheckForParseErrors(newProgramSyntax).Should().Be(
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
}

output myObj object = {
  lowerCaseProp: resA.properties.lowercaseprop
  camelcaseprop: resA.properties.camelCaseProp
}");
        }

        [TestMethod]
        public void Nested_casing_issues_take_multiple_passes_to_correct()
        {
            var bicepFile = @"
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
";

            var typeDefinition = TestTypeHelper.CreateCustomResourceType("My.Rp/resA", "2020-01-01", TypeSymbolValidationFlags.WarnOnTypeMismatch,
                new TypeProperty("lowercaseobj", new ObjectType("lowercaseobj", TypeSymbolValidationFlags.Default, new [] {
                  new TypeProperty("lowercasestr", LanguageConstants.String)
                }, null)));
            var typeLoader = TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(typeDefinition.AsEnumerable());

            var (_, _, compilation) = CompilationHelper.Compile(typeLoader, ("main.bicep", bicepFile));
            var rewriter = new TypeCasingFixerRewriter(compilation.GetEntrypointSemanticModel());

            var newProgramSyntax = rewriter.Rewrite(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);
            var firstPassBicepFile = PrintHelper.PrintAndCheckForParseErrors(newProgramSyntax);
            firstPassBicepFile.Should().Be(
@"resource resA 'My.Rp/resA@2020-01-01' = {
  name: 'resA'
  properties: {
    lowercaseobj: {
      lowerCaseStr: 'test'
    }
  }
}

output myObj object = {
  lowerCaseProp: resA.properties.lowercaseobj.lowerCaseStr
}");

            (_, _, compilation) = CompilationHelper.Compile(typeLoader, ("main.bicep", firstPassBicepFile));
            rewriter = new TypeCasingFixerRewriter(compilation.GetEntrypointSemanticModel());

            newProgramSyntax = rewriter.Rewrite(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);
            PrintHelper.PrintAndCheckForParseErrors(newProgramSyntax).Should().Be(
@"resource resA 'My.Rp/resA@2020-01-01' = {
  name: 'resA'
  properties: {
    lowercaseobj: {
      lowercasestr: 'test'
    }
  }
}

output myObj object = {
  lowerCaseProp: resA.properties.lowercaseobj.lowercasestr
}");
        }
    }
}