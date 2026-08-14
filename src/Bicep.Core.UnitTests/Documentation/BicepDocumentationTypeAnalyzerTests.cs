// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using System.Threading.Tasks;
using Bicep.Core.Documentation;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Documentation;

[TestClass]
public class BicepDocumentationTypeAnalyzerTests
{
    [TestMethod]
    public async Task BuildModel_LiteralUnionParameters_ProjectsAllowedValuesForBoolIntAndStringUnions()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("""
            param stringChoice ('b' | 'a') = 'a'
            param intChoice (2 | 1) = 1
            param boolChoice (true | false) = true

            output stringChoiceOut string = stringChoice
            output intChoiceOut int = intChoice
            output boolChoiceOut bool = boolChoice
            """);

        result.Diagnostics.Should().NotContain(d => d.Level == Bicep.Core.Diagnostics.DiagnosticLevel.Error);

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation);

        var stringChoice = model.Parameters.Single(p => p.Name == "stringChoice");
        stringChoice.TypeName.Should().Be("string");
        stringChoice.AllowedValues.Should().Equal("a", "b");

        var intChoice = model.Parameters.Single(p => p.Name == "intChoice");
        intChoice.TypeName.Should().Be("int");
        intChoice.AllowedValues.Should().Equal("1", "2");

        var boolChoice = model.Parameters.Single(p => p.Name == "boolChoice");
        boolChoice.TypeName.Should().Be("bool");
        boolChoice.AllowedValues.Should().Equal("false", "true");
    }

    [TestMethod]
    public void BuildParameter_InternalUnionShapes_AreRepresentedDeterministically()
    {
        var mixedUnion = new UnionType("mixed", [
            TypeFactory.CreateStringLiteralType("a"),
            TypeFactory.CreateIntegerLiteralType(1),
        ]);
        var nonLiteralUnion = new UnionType("string | int", [
            LanguageConstants.String,
            LanguageConstants.Int,
        ]);
        var array = TypeFactory.CreateArrayType(nonLiteralUnion);

        var mixed = BicepDocumentationTypeAnalyzer.BuildParameter("mixed", mixedUnion, false, null, null);
        var nonLiteral = BicepDocumentationTypeAnalyzer.BuildParameter("nonLiteral", nonLiteralUnion, false, null, null);
        var arrayParameter = BicepDocumentationTypeAnalyzer.BuildParameter("array", array, false, null, null);

        mixed.TypeName.Should().Be("mixed");
        mixed.AllowedValues.Should().Equal("1", "a");
        nonLiteral.TypeName.Should().Be("string | int");
        nonLiteral.AllowedValues.Should().BeEmpty();
        arrayParameter.TypeName.Should().Be("array");
        arrayParameter.AllowedValues.Should().BeEmpty();
    }

    [TestMethod]
    public void BuildParameter_SingleCaseDiscriminator_ProjectsTheCase()
    {
        var objectType = new ObjectType(
            "single",
            TypeSymbolValidationFlags.Default,
            [new NamedTypeProperty(
                "kind",
                TypeFactory.CreateStringLiteralType("only"),
                TypePropertyFlags.Required)]);
        var discriminated = new DiscriminatedObjectType(
            "single union",
            TypeSymbolValidationFlags.Default,
            "kind",
            [objectType]);

        var parameter = BicepDocumentationTypeAnalyzer.BuildParameter("value", discriminated, false, null, null);

        parameter.Discriminator.Should().NotBeNull();
        parameter.Discriminator!.Cases.Should().ContainSingle();
        parameter.Discriminator.Cases[0].Value.Should().Be("only");
    }

    [TestMethod]
    public async Task BuildModel_ArrayWithLiteralUnionItemType_ProjectsAllowedValues()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("param environments ('dev' | 'test' | 'prod')[] = ['dev']");

        result.Diagnostics.Should().NotContain(d => d.Level == Bicep.Core.Diagnostics.DiagnosticLevel.Error);

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation);

        var environments = model.Parameters.Single(p => p.Name == "environments");
        environments.TypeName.Should().Be("array");
        environments.AllowedValues.Should().Equal("dev", "prod", "test");
    }

    [TestMethod]
    public async Task BuildModel_ArrayWithPlainItemType_HasNoAllowedValues()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("param names string[] = []");

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation);

        model.Parameters.Single(p => p.Name == "names").AllowedValues.Should().BeEmpty();
    }

    [TestMethod]
    public async Task BuildModel_NestedObjectParameter_ExpandsPropertiesWithObjectPrimitiveTypeNameAndSecureNestedField()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("""
            type credentials = {
              username: string
              @secure()
              password: string
            }
            param creds credentials
            """);

        result.Diagnostics.Should().NotContain(d => d.Level == Bicep.Core.Diagnostics.DiagnosticLevel.Error);

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation);

        var creds = model.Parameters.Single(p => p.Name == "creds");
        creds.TypeName.Should().Be("object");
        creds.Discriminator.Should().BeNull();
        creds.NestedProperties.Select(p => p.Name).Should().Equal("password", "username");

        var password = creds.NestedProperties.Single(p => p.Name == "password");
        password.IsSecure.Should().BeTrue();

        var username = creds.NestedProperties.Single(p => p.Name == "username");
        username.IsSecure.Should().BeFalse();
    }

    [TestMethod]
    public async Task BuildModel_DeeplyNestedObjectParameter_StopsExpandingBeyondMaxDepthAndUsesObjectTypeName()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile(BuildDeeplyNestedObjectSource());

        result.Diagnostics.Should().NotContain(d => d.Level == Bicep.Core.Diagnostics.DiagnosticLevel.Error);

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation);

        var root = model.Parameters.Single(p => p.Name == "root");
        root.TypeName.Should().Be("object");

        // Walk down 20 levels: each should still expand (depth < MaxDepth).
        var current = root;
        for (var i = 0; i < 20; i++)
        {
            current.NestedProperties.Should().ContainSingle($"level {i} should expand");
            current = current.NestedProperties.Single();
        }

        // The 21st level (depth == MaxDepth) must stop expanding, but still reports the object primitive type name.
        current.TypeName.Should().Be("object");
        current.NestedProperties.Should().BeEmpty();
    }

    [TestMethod]
    public async Task BuildModel_DeeplyNestedDiscriminatedUnionParameter_StopsExpandingBeyondMaxDepth()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile(BuildDeeplyNestedDiscriminatedSource());

        result.Diagnostics.Should().NotContain(d => d.Level == Bicep.Core.Diagnostics.DiagnosticLevel.Error);

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation);

        var current = model.Parameters.Single(p => p.Name == "root");
        for (var i = 0; i < 20; i++)
        {
            current.NestedProperties.Should().ContainSingle($"level {i} should expand");
            current = current.NestedProperties.Single();
        }

        // At depth == MaxDepth, the discriminated union must stop expanding without a discriminator.
        current.TypeName.Should().Be("object");
        current.Discriminator.Should().BeNull();
    }

    private static string BuildDeeplyNestedObjectSource()
    {
        var builder = new System.Text.StringBuilder();
        const int levels = 21;

        for (var i = 0; i < levels; i++)
        {
            builder.AppendLine(i == levels - 1
                ? $"type level{i} = {{ value: string }}"
                : $"type level{i} = {{ next: level{i + 1} }}");
        }

        builder.AppendLine("param root level0");

        return builder.ToString();
    }

    private static string BuildDeeplyNestedDiscriminatedSource()
    {
        var builder = new System.Text.StringBuilder();
        const int plainLevels = 20;

        builder.AppendLine("type leafA = { kind: 'a' }");
        builder.AppendLine("type leafB = { kind: 'b' }");
        builder.AppendLine("@discriminator('kind')");
        builder.AppendLine($"type level{plainLevels} = leafA | leafB");

        for (var i = 0; i < plainLevels; i++)
        {
            builder.AppendLine($"type level{i} = {{ next: level{i + 1} }}");
        }

        builder.AppendLine("param root level0");

        return builder.ToString();
    }
}
