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
        var analyzer = new BicepDocumentationTypeAnalyzer();

        var mixed = analyzer.BuildParameter("mixed", mixedUnion, false, null, null);
        var nonLiteral = analyzer.BuildParameter("nonLiteral", nonLiteralUnion, false, null, null);
        var arrayParameter = analyzer.BuildParameter("array", array, false, null, null);

        mixed.TypeName.Should().Be("mixed");
        mixed.AllowedValues.Should().Equal("1", "a");
        nonLiteral.TypeName.Should().Be("string | int");
        nonLiteral.AllowedValues.Should().BeEmpty();
        arrayParameter.TypeName.Should().Be("array");
        arrayParameter.AllowedValues.Should().BeEmpty();

        var singleLiteral = analyzer.BuildParameter(
            "singleLiteral",
            TypeFactory.CreateStringLiteralType("public"),
            false,
            null,
            null);
        singleLiteral.TypeName.Should().Be("string");
        singleLiteral.AllowedValues.Should().Equal("public");
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

        var parameter = new BicepDocumentationTypeAnalyzer().BuildParameter("value", discriminated, false, null, null);

        parameter.Discriminator.Should().NotBeNull();
        parameter.Discriminator!.Cases.Should().ContainSingle();
        parameter.Discriminator.Cases[0].Value.Should().Be("only");
    }

    [TestMethod]
    public void BuildParameter_ReusedRootType_ReusesTheCachedAnalysis()
    {
        var objectType = new ObjectType(
            "shared",
            TypeSymbolValidationFlags.Default,
            [new NamedTypeProperty("name", LanguageConstants.String)]);
        var analyzer = new BicepDocumentationTypeAnalyzer();

        var first = analyzer.BuildParameter("first", objectType, false, null, null);
        var second = analyzer.BuildParameter("second", objectType, false, null, null);

        (first.NestedProperties == second.NestedProperties).Should().BeTrue();
    }

    [TestMethod]
    public void BuildParameter_CyclicCompoundTypes_StopAtTheRepeatedType()
    {
        TypedArrayType? cyclicArray = null;
        var cyclicArrayItem = new ObjectType(
            "cyclicArrayItem",
            TypeSymbolValidationFlags.Default,
            [new NamedTypeProperty("next", new DeferredTypeReference(() => cyclicArray!))]);
        cyclicArray = new TypedArrayType(
            cyclicArrayItem,
            TypeSymbolValidationFlags.Default);

        ObjectType? cyclicObject = null;
        cyclicObject = new ObjectType(
            "cyclicObject",
            TypeSymbolValidationFlags.Default,
            [new NamedTypeProperty("next", new DeferredTypeReference(() => cyclicObject!))]);

        DiscriminatedObjectType? cyclicDiscriminator = null;
        var variant = new ObjectType(
            "variant",
            TypeSymbolValidationFlags.Default,
            [
                new NamedTypeProperty(
                    "kind",
                    TypeFactory.CreateStringLiteralType("only"),
                    TypePropertyFlags.Required),
                new NamedTypeProperty(
                    "next",
                    new DeferredTypeReference(() => cyclicDiscriminator!)),
            ]);
        cyclicDiscriminator = new DiscriminatedObjectType(
            "cyclicDiscriminator",
            TypeSymbolValidationFlags.Default,
            "kind",
            [variant]);

        var analyzer = new BicepDocumentationTypeAnalyzer();
        var arrayParameter = analyzer.BuildParameter(
            "array",
            cyclicArray,
            false,
            null,
            null);
        var objectParameter = analyzer.BuildParameter(
            "object",
            cyclicObject,
            false,
            null,
            null);
        var discriminatorParameter = analyzer.BuildParameter(
            "discriminator",
            cyclicDiscriminator,
            false,
            null,
            null);

        arrayParameter.NestedProperties.Single().NestedProperties.Should().BeEmpty();
        arrayParameter.NestedProperties.Single().IsTruncated.Should().BeTrue();
        objectParameter.NestedProperties.Single().NestedProperties.Should().BeEmpty();
        objectParameter.NestedProperties.Single().IsTruncated.Should().BeTrue();
        discriminatorParameter.Discriminator!.Cases.Single()
            .Properties.Single(property => property.Name == "next")
            .Discriminator.Should().BeNull();
        discriminatorParameter.Discriminator.Cases.Single()
            .Properties.Single(property => property.Name == "next")
            .IsTruncated.Should().BeTrue();
    }

    [TestMethod]
    public async Task BuildModel_RecursiveBicepType_StopsAtTheFirstRepeatedType()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("""
            type node = {
              name: string
              left: node?
              right: node?
            }
            param tree node
            """);

        result.Diagnostics.Should().NotContain(diagnostic => diagnostic.Level == Bicep.Core.Diagnostics.DiagnosticLevel.Error);

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var tree = generator.BuildModel(result.Compilation).Parameters.Single(parameter => parameter.Name == "tree");

        tree.NestedProperties.Select(property => property.Name).Should().Equal("left", "name", "right");
        tree.NestedProperties.Single(property => property.Name == "left").IsTruncated.Should().BeTrue();
        tree.NestedProperties.Single(property => property.Name == "right").IsTruncated.Should().BeTrue();
    }

    [TestMethod]
    public void GetTypeName_Array_ReturnsPrimitiveNameWithoutExpansion()
    {
        new BicepDocumentationTypeAnalyzer().GetTypeName(TypeFactory.CreateStringArrayType())
            .Should().Be("array");
    }

    [TestMethod]
    public void GetTypeName_ProjectsPrimitiveCompoundAndLiteralTypes()
    {
        var analyzer = new BicepDocumentationTypeAnalyzer();
        var objectType = new ObjectType("object", TypeSymbolValidationFlags.Default, []);
        var discriminatorMember = new ObjectType(
            "case",
            TypeSymbolValidationFlags.Default,
            [new NamedTypeProperty("kind", TypeFactory.CreateStringLiteralType("only"), TypePropertyFlags.Required)]);
        var discriminated = new DiscriminatedObjectType(
            "discriminated",
            TypeSymbolValidationFlags.Default,
            "kind",
            [discriminatorMember]);
        var stringUnion = new UnionType("string union", [
            TypeFactory.CreateStringLiteralType("a"),
            TypeFactory.CreateStringLiteralType("b"),
        ]);
        var mixedUnion = new UnionType("mixed union", [
            TypeFactory.CreateStringLiteralType("a"),
            TypeFactory.CreateIntegerLiteralType(1),
        ]);
        var nonLiteralUnion = new UnionType("string | int", [LanguageConstants.String, LanguageConstants.Int]);

        analyzer.GetTypeName(stringUnion).Should().Be("string");
        analyzer.GetTypeName(mixedUnion).Should().Be("mixed union");
        analyzer.GetTypeName(nonLiteralUnion).Should().Be("string | int");
        analyzer.GetTypeName(TypeFactory.CreateStringLiteralType("a")).Should().Be("string");
        analyzer.GetTypeName(LanguageConstants.Int).Should().Be("int");
        analyzer.GetTypeName(LanguageConstants.String).Should().Be("string");
        analyzer.GetTypeName(objectType).Should().Be("object");
        analyzer.GetTypeName(discriminated).Should().Be("object");
        analyzer.GetTypeName(LanguageConstants.Bool).Should().Be("bool");
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
    public void BuildParameter_ArrayWithLiteralItem_ProjectsAllowedValue()
    {
        var array = TypeFactory.CreateArrayType(TypeFactory.CreateStringLiteralType("only"));

        var parameter = new BicepDocumentationTypeAnalyzer().BuildParameter("items", array, false, null, null);

        parameter.AllowedValues.Should().Equal("only");
    }

    [TestMethod]
    public void BuildParameter_DeeplyNestedArrays_StopsAtTheDepthLimit()
    {
        TypeSymbol type = LanguageConstants.String;
        for (var index = 0; index <= 20; index++)
        {
            type = TypeFactory.CreateArrayType(type);
        }

        var parameter = new BicepDocumentationTypeAnalyzer().BuildParameter("items", type, false, null, null);

        parameter.IsTruncated.Should().BeTrue();
    }

    [TestMethod]
    public void BuildParameter_BranchingTypeGraph_StopsAtTheNodeBudget()
    {
        TypeSymbol type = new ObjectType(
            "leaf",
            TypeSymbolValidationFlags.Default,
            [new NamedTypeProperty("value", LanguageConstants.String)]);
        for (var index = 0; index < 20; index++)
        {
            var childType = type;
            type = new ObjectType(
                $"level{index}",
                TypeSymbolValidationFlags.Default,
                [
                    new NamedTypeProperty("left", childType),
                    new NamedTypeProperty("right", childType),
                ]);
        }

        var parameter = new BicepDocumentationTypeAnalyzer().BuildParameter("root", type, false, null, null);
        var descendants = Flatten(parameter).ToArray();

        descendants.Length.Should().BeLessThan(10_100);
        descendants.Should().Contain(item => item.IsTruncated);
    }

    [TestMethod]
    public async Task BuildParameter_ObservesCancellation()
    {
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();
        var analyzer = new BicepDocumentationTypeAnalyzer(cancellation.Token);

        var action = () => analyzer.BuildParameter("value", LanguageConstants.String, false, null, null);

        action.Should().Throw<OperationCanceledException>();
    }

    [TestMethod]
    public void BuildParameter_TopLevelParametersBeyondTheNodeBudget_AreTruncated()
    {
        var analyzer = new BicepDocumentationTypeAnalyzer();
        BicepDocumentationParameter parameter = null!;
        for (var index = 0; index <= 10_000; index++)
        {
            parameter = analyzer.BuildParameter($"value{index}", LanguageConstants.String, false, null, null);
        }

        parameter.IsTruncated.Should().BeTrue();
        parameter.TypeName.Should().Be("string");
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
    public async Task BuildModel_ArrayOfObjects_ExpandsItemPropertiesAndAdditionalProperties()
    {
        var compiler = TestCompiler.ForMockFileSystemCompilation();
        var result = await compiler.Compile("""
            type item = {
              name: string
              *: int
            }
            param items item[]
            """);

        var generator = compiler.GetService<IBicepDocumentationGenerator>();
        var model = generator.BuildModel(result.Compilation);
        var items = model.Parameters.Single(p => p.Name == "items");

        items.NestedProperties.Select(property => property.Name).Should().Equal(
            ">Any_other_property<",
            "name");
        items.NestedProperties.Single(property => property.Name == "name").TypeName.Should().Be("string");
        items.NestedProperties.Single(property => property.Name == ">Any_other_property<").TypeName.Should().Be("int");
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
        current.IsTruncated.Should().BeTrue();
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
        current.IsTruncated.Should().BeTrue();
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

    private static IEnumerable<BicepDocumentationParameter> Flatten(BicepDocumentationParameter parameter)
    {
        yield return parameter;
        foreach (var child in parameter.NestedProperties.SelectMany(Flatten))
        {
            yield return child;
        }
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
