// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.TypeSystem;

[TestClass]
public class TypeHelperTests
{
    [DataTestMethod]
    [DynamicData(nameof(GetPrimitiveCollapsePositiveTestCases), DynamicDataSourceType.Method)]
    public void Primitive_collapse_preserves_and_fuses_refinements(TypeSymbol expected, params TypeSymbol[] toCollapse)
    {
        var actual = TypeHelper.TryCollapseTypes(toCollapse);
        actual.Should().Be(expected);
    }

    private static IEnumerable<object[]> GetPrimitiveCollapsePositiveTestCases()
    {
        static object[] Row(TypeSymbol expected, params TypeSymbol[] toCollapse)
            => expected.AsEnumerable().Concat(toCollapse).ToArray();

        // In the explanatory comments for test cases, the `{inclusiveMin,inclusiveMax}` suffix is used to denote length constraints
        return new[]
        {
            // collapse(true, false) -> bool
            Row(LanguageConstants.Bool, LanguageConstants.True, LanguageConstants.False),
            // collapse(bool, false) -> bool
            Row(LanguageConstants.Bool, LanguageConstants.Bool, LanguageConstants.False),
            // collapse(true, bool) -> bool
            Row(LanguageConstants.Bool, LanguageConstants.True, LanguageConstants.Bool),
            // collapse(true, false, null) -> bool?
            Row(TypeHelper.CreateTypeUnion(LanguageConstants.Bool, LanguageConstants.Null), LanguageConstants.True, LanguageConstants.False, LanguageConstants.Null),
            // collapse(null, true, false) -> bool?
            Row(TypeHelper.CreateTypeUnion(LanguageConstants.Bool, LanguageConstants.Null), LanguageConstants.Null, LanguageConstants.True, LanguageConstants.False),

            // collapse(<= 10, >= 9) -> int
            Row(LanguageConstants.Int, TypeFactory.CreateIntegerType(minValue: null, maxValue: 10), TypeFactory.CreateIntegerType(minValue: 9, maxValue: null)),
            // collapse(<= 10, >= 9, null) -> int?
            Row(TypeHelper.CreateTypeUnion(LanguageConstants.Int, LanguageConstants.Null), TypeFactory.CreateIntegerType(minValue: null, maxValue: 10), TypeFactory.CreateIntegerType(minValue: 9, maxValue: null), LanguageConstants.Null),
            // collapse(null, <= 10, >= 9) -> int?
            Row(TypeHelper.CreateTypeUnion(LanguageConstants.Int, LanguageConstants.Null), LanguageConstants.Null, TypeFactory.CreateIntegerType(minValue: null, maxValue: 10), TypeFactory.CreateIntegerType(minValue: 9, maxValue: null)),
            // collapse(<= 10, >= 9, 10) -> int
            Row(LanguageConstants.Int, TypeFactory.CreateIntegerType(minValue: null, maxValue: 10), TypeFactory.CreateIntegerType(minValue: 9, maxValue: null), TypeFactory.CreateIntegerLiteralType(10)),
            // collapse(<= 10, 10) -> <= 10
            Row(TypeFactory.CreateIntegerType(minValue: null, maxValue: 10), TypeFactory.CreateIntegerType(minValue: null, maxValue: 10), TypeFactory.CreateIntegerLiteralType(10)),
            // collapse(>= 7 && <= 9, 8) -> >= 7 && <= 9
            Row(TypeFactory.CreateIntegerType(minValue: 7, maxValue: 9), TypeFactory.CreateIntegerType(minValue: 7, maxValue: 9), TypeFactory.CreateIntegerLiteralType(8)),
            // collapse(>= 7 && <= 8, >= 8 && <= 9) -> >= 7 && <= 9
            Row(TypeFactory.CreateIntegerType(minValue: 7, maxValue: 9), TypeFactory.CreateIntegerType(minValue: 7, maxValue: 8), TypeFactory.CreateIntegerType(minValue: 8, maxValue: 9)),
            // collapse(7, 8, 9) -> >= 7 && <= 9
            Row(TypeFactory.CreateIntegerType(minValue: 7, maxValue: 9), TypeFactory.CreateIntegerLiteralType(7), TypeFactory.CreateIntegerLiteralType(8), TypeFactory.CreateIntegerLiteralType(9)),
            // collapse(5, 7, >= 1 && <= 3, 8, >= 2 && <= 4, 9) -> union(>= 1 && <= 5, >= 7 && <= 9)
            Row(TypeHelper.CreateTypeUnion(TypeFactory.CreateIntegerType(minValue: 1, maxValue: 5), TypeFactory.CreateIntegerType(minValue: 7, maxValue: 9)),
                TypeFactory.CreateIntegerLiteralType(5),
                TypeFactory.CreateIntegerLiteralType(7),
                TypeFactory.CreateIntegerType(1, 3),
                TypeFactory.CreateIntegerLiteralType(8),
                TypeFactory.CreateIntegerType(2, 4),
                TypeFactory.CreateIntegerLiteralType(9)),

            // collapse(string{null,10}, string{9,null}) -> string
            Row(LanguageConstants.String, TypeFactory.CreateStringType(minLength: null, maxLength: 10), TypeFactory.CreateStringType(minLength: 9, maxLength: null)),
            // collapse(string{null,10}, string{9,null}, null) -> string?
            Row(TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Null), TypeFactory.CreateStringType(minLength: null, maxLength: 10), TypeFactory.CreateStringType(minLength: 9, maxLength: null), LanguageConstants.Null),
            // collapse(null, string{null,10}, string{9,null}) -> string?
            Row(TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Null), LanguageConstants.Null, TypeFactory.CreateStringType(minLength: null, maxLength: 10), TypeFactory.CreateStringType(minLength: 9, maxLength: null)),
            // collapse(string{null,10}, string{9,null}, '10 letters') -> string
            Row(LanguageConstants.String, TypeFactory.CreateStringType(minLength: null, maxLength: 10), TypeFactory.CreateStringType(minLength: 9, maxLength: null), TypeFactory.CreateStringLiteralType("10 letters")),
            // collapse(string{null,10}, '10 letters') -> string{null,10}
            Row(TypeFactory.CreateStringType(minLength: null, maxLength: 10), TypeFactory.CreateStringType(minLength: null, maxLength: 10), TypeFactory.CreateStringLiteralType("10 letters")),
            // collapse(string{9,11}, '10 letters') -> string{9,11}
            Row(TypeFactory.CreateStringType(minLength: 9, maxLength: 11), TypeFactory.CreateStringType(minLength: 9, maxLength: 11), TypeFactory.CreateStringLiteralType("10 letters")),
            // collapse(string{9,11}, '10 letters', '13 characters') -> union(string{9,11}, '13 characters')
            Row(TypeHelper.CreateTypeUnion(TypeFactory.CreateStringType(minLength: 9, maxLength: 11), TypeFactory.CreateStringLiteralType("13 characters")),
                TypeFactory.CreateStringType(minLength: 9, maxLength: 11),
                TypeFactory.CreateStringLiteralType("10 letters"),
                TypeFactory.CreateStringLiteralType("13 characters")),
            // collapse(string{6,7}, string{8,9}) -> string{6,9}
            Row(TypeFactory.CreateStringType(minLength: 6, maxLength: 9), TypeFactory.CreateStringType(minLength: 6, maxLength: 7), TypeFactory.CreateStringType(minLength: 8, maxLength: 9)),
            // collapse(string{6,7}, string{11,15}, string{8,9}) -> union(string{6,9}, string{11,15})
            Row(TypeHelper.CreateTypeUnion(TypeFactory.CreateStringType(minLength: 6, maxLength: 9), TypeFactory.CreateStringType(minLength: 11, maxLength: 15)),
                TypeFactory.CreateStringType(minLength: 6, maxLength: 7),
                TypeFactory.CreateStringType(minLength: 11, maxLength: 15),
                TypeFactory.CreateStringType(minLength: 8, maxLength: 9)),

            // collapse(array{null,10}, array{9,null}) -> array
            Row(LanguageConstants.Array, TypeFactory.CreateArrayType(minLength: null, maxLength: 10), TypeFactory.CreateArrayType(minLength: 9, maxLength: null)),
            // collapse(array{null,10}, array{9,null}, null) -> array?
            Row(TypeHelper.CreateTypeUnion(LanguageConstants.Array, LanguageConstants.Null), TypeFactory.CreateArrayType(minLength: null, maxLength: 10), TypeFactory.CreateArrayType(minLength: 9, maxLength: null), LanguageConstants.Null),
            // collapse(null, array{null,10}, array{9,null}) -> array?
            Row(TypeHelper.CreateTypeUnion(LanguageConstants.Array, LanguageConstants.Null), LanguageConstants.Null, TypeFactory.CreateArrayType(minLength: null, maxLength: 10), TypeFactory.CreateArrayType(minLength: 9, maxLength: null)),
            // collapse(array{6,7}, array{8,9}) -> array{6,9}
            Row(TypeFactory.CreateArrayType(minLength: 6, maxLength: 9), TypeFactory.CreateArrayType(minLength: 6, maxLength: 7), TypeFactory.CreateArrayType(minLength: 8, maxLength: 9)),
            // collapse(array{6,7}, array{11,15}, array{8,9}) -> union(array{6,9}, array{11,15})
            Row(TypeHelper.CreateTypeUnion(TypeFactory.CreateArrayType(minLength: 6, maxLength: 9), TypeFactory.CreateArrayType(minLength: 11, maxLength: 15)),
                TypeFactory.CreateArrayType(minLength: 6, maxLength: 7),
                TypeFactory.CreateArrayType(minLength: 11, maxLength: 15),
                TypeFactory.CreateArrayType(minLength: 8, maxLength: 9)),
            // collapse(string[]{null,3}, int[]{100,null}, string[]{4,6}, int[]{5,99}) -> union(string[]{null,6}, int[]{5,null})
            Row(TypeHelper.CreateTypeUnion(TypeFactory.CreateArrayType(LanguageConstants.String, minLength: null, maxLength: 6), TypeFactory.CreateArrayType(LanguageConstants.Int, minLength: 5, maxLength: null)),
                TypeFactory.CreateArrayType(LanguageConstants.String, minLength: null, maxLength: 3),
                TypeFactory.CreateArrayType(LanguageConstants.Int, minLength: 100, maxLength: null),
                TypeFactory.CreateArrayType(LanguageConstants.String, minLength: 4, maxLength: 6),
                TypeFactory.CreateArrayType(LanguageConstants.Int, minLength: 5, maxLength: 99)),
            // collapse(array{6,7}, [string, int], array{8,9}) -> union(array{6,9}, [string, int])
            Row(TypeHelper.CreateTypeUnion(TypeFactory.CreateArrayType(minLength: 6, maxLength: 9), new TupleType([LanguageConstants.String, LanguageConstants.Int], default)),
                TypeFactory.CreateArrayType(minLength: 6, maxLength: 7),
                new TupleType([LanguageConstants.String, LanguageConstants.Int], default),
                TypeFactory.CreateArrayType(minLength: 8, maxLength: 9)),
            // collapse(string[], [string, string, string]) -> string[]
            Row(TypeFactory.CreateArrayType(LanguageConstants.String),
                TypeFactory.CreateArrayType(LanguageConstants.String),
                new TupleType([LanguageConstants.String, LanguageConstants.String, LanguageConstants.String], default)),
            // collapse(string[], [string, int, string]) -> union(string[], [string, int, string])
            Row(TypeHelper.CreateTypeUnion(TypeFactory.CreateArrayType(LanguageConstants.String), new TupleType([LanguageConstants.String, LanguageConstants.Int, LanguageConstants.String], default)),
                TypeFactory.CreateArrayType(LanguageConstants.String),
                new TupleType([LanguageConstants.String, LanguageConstants.Int, LanguageConstants.String], default)),
            // collapse((string | int)[], [string, int, string]) -> (string | int)[]
            Row(TypeFactory.CreateArrayType(TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Int)),
                TypeFactory.CreateArrayType(TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Int)),
                new TupleType([LanguageConstants.String, LanguageConstants.Int, LanguageConstants.String], default)),
        };
    }

    [TestMethod]
    public void Tagged_union_formed_from_object_collapse_where_possible()
    {
        var prospectiveTaggedUnionMembers = new ObjectType[]
        {
            new("{type: 'a', foo: string}",
                default,
                new NamedTypeProperty[]
                {
                    new("type", TypeFactory.CreateStringLiteralType("a"), TypePropertyFlags.Required),
                    new("foo", LanguageConstants.String, TypePropertyFlags.Required),
                },
                null),
            new("{type: 'b', bar: int}",
                default,
                new NamedTypeProperty[]
                {
                    new("type", TypeFactory.CreateStringLiteralType("b"), TypePropertyFlags.Required),
                    new("bar", LanguageConstants.Int, TypePropertyFlags.Required),
                },
                null),
        };

        var collapsed = TypeHelper.TryCollapseTypes(prospectiveTaggedUnionMembers);

        collapsed.Should().BeOfType<DiscriminatedObjectType>();
        collapsed.As<DiscriminatedObjectType>().DiscriminatorKey.Should().Be("type");
        collapsed.As<DiscriminatedObjectType>().UnionMembersByKey["'a'"].Should().Be(prospectiveTaggedUnionMembers[0]);
        collapsed.As<DiscriminatedObjectType>().UnionMembersByKey["'b'"].Should().Be(prospectiveTaggedUnionMembers[1]);
    }

    [TestMethod]
    public void Tagged_union_should_not_be_formed_when_discriminator_is_not_required_on_all_members()
    {
        var prospectiveTaggedUnionMembers = new ObjectType[]
        {
            new("{type: 'a', foo: string}",
                default,
                new NamedTypeProperty[]
                {
                    new("type", TypeFactory.CreateStringLiteralType("a"), TypePropertyFlags.Required),
                    new("foo", LanguageConstants.String, TypePropertyFlags.Required),
                },
                null),
            new("{type: 'b', bar: int}",
                default,
                new NamedTypeProperty[]
                {
                    new("type", TypeFactory.CreateStringLiteralType("b"), default),
                    new("bar", LanguageConstants.Int, TypePropertyFlags.Required),
                },
                null),
        };

        var collapsed = TypeHelper.TryCollapseTypes(prospectiveTaggedUnionMembers).Should().BeAssignableTo<ObjectType>().Subject;
        collapsed.Properties.Should().HaveCount(3);
        collapsed.Properties.ContainsKey("type").Should().BeTrue();
        collapsed.Properties["type"].TypeReference.Type.Name.Should().Be("'a' | 'b'");
        collapsed.Properties["type"].Flags.HasFlag(TypePropertyFlags.Required).Should().BeFalse();

        collapsed.Properties.ContainsKey("foo").Should().BeTrue();
        collapsed.Properties["foo"].TypeReference.Type.Name.Should().Be("string");
        collapsed.Properties["foo"].Flags.HasFlag(TypePropertyFlags.Required).Should().BeFalse();

        collapsed.Properties.ContainsKey("bar").Should().BeTrue();
        collapsed.Properties["bar"].TypeReference.Type.Name.Should().Be("int");
        collapsed.Properties["bar"].Flags.HasFlag(TypePropertyFlags.Required).Should().BeFalse();
    }

    [TestMethod]
    public void Tagged_union_should_not_be_formed_when_multiple_members_use_same_discriminator()
    {
        var prospectiveTaggedUnionMembers = new ObjectType[]
        {
            new("{type: 'a', foo: string}",
                default,
                new NamedTypeProperty[]
                {
                    new("type", TypeFactory.CreateStringLiteralType("a"), TypePropertyFlags.Required),
                    new("foo", LanguageConstants.String, TypePropertyFlags.Required),
                },
                null),
            new("{type: 'b', bar: int}",
                default,
                new NamedTypeProperty[]
                {
                    new("type", TypeFactory.CreateStringLiteralType("b"), TypePropertyFlags.Required),
                    new("bar", LanguageConstants.Int, TypePropertyFlags.Required),
                },
                null),
            new("{type: 'a', baz: int}",
                default,
                new NamedTypeProperty[]
                {
                    new("type", TypeFactory.CreateStringLiteralType("a"), TypePropertyFlags.Required),
                    new("baz", LanguageConstants.Int, TypePropertyFlags.Required),
                },
                null),
        };

        var collapsed = TypeHelper.TryCollapseTypes(prospectiveTaggedUnionMembers).Should().BeAssignableTo<ObjectType>().Subject;
        collapsed.Properties.Should().HaveCount(4);
        collapsed.Properties.ContainsKey("type").Should().BeTrue();
        collapsed.Properties["type"].TypeReference.Type.Name.Should().Be("'a' | 'b'");
        collapsed.Properties["type"].Flags.HasFlag(TypePropertyFlags.Required).Should().BeTrue();

        collapsed.Properties.ContainsKey("foo").Should().BeTrue();
        collapsed.Properties["foo"].TypeReference.Type.Name.Should().Be("string");
        collapsed.Properties["foo"].Flags.HasFlag(TypePropertyFlags.Required).Should().BeFalse();

        collapsed.Properties.ContainsKey("bar").Should().BeTrue();
        collapsed.Properties["bar"].TypeReference.Type.Name.Should().Be("int");
        collapsed.Properties["bar"].Flags.HasFlag(TypePropertyFlags.Required).Should().BeFalse();

        collapsed.Properties.ContainsKey("baz").Should().BeTrue();
        collapsed.Properties["baz"].TypeReference.Type.Name.Should().Be("int");
        collapsed.Properties["baz"].Flags.HasFlag(TypePropertyFlags.Required).Should().BeFalse();
    }

    [TestMethod]
    public void Union_should_not_be_collapsed_when_some_members_are_not_objects()
    {
        var prospectiveTaggedUnionMembers = new TypeSymbol[]
        {
            new ObjectType("{type: 'a', foo: string}",
                default,
                new NamedTypeProperty[]
                {
                    new("type", TypeFactory.CreateStringLiteralType("a"), TypePropertyFlags.Required),
                    new("foo", LanguageConstants.String, TypePropertyFlags.Required),
                },
                null),
            new ObjectType("{type: 'b', bar: int}",
                default,
                new NamedTypeProperty[]
                {
                    new("type", TypeFactory.CreateStringLiteralType("b"), TypePropertyFlags.Required),
                    new("bar", LanguageConstants.Int, TypePropertyFlags.Required),
                },
                null),
            LanguageConstants.String,
        };

        TypeHelper.TryCollapseTypes(prospectiveTaggedUnionMembers).Should().BeNull();
    }

    [TestMethod]
    public void Tagged_union_formation_should_prefer_discriminator_named_type_when_multiple_candidates_exist()
    {
        var prospectiveTaggedUnionMembers = new ObjectType[]
        {
            new("{type: 'a', foo: string}",
                default,
                new NamedTypeProperty[]
                {
                    new("type", TypeFactory.CreateStringLiteralType("a"), TypePropertyFlags.Required),
                    new("fizz", TypeFactory.CreateStringLiteralType("buzz"), TypePropertyFlags.Required),
                    new("foo", LanguageConstants.String, TypePropertyFlags.Required),
                },
                null),
            new("{type: 'b', bar: int}",
                default,
                new NamedTypeProperty[]
                {
                    new("type", TypeFactory.CreateStringLiteralType("b"), TypePropertyFlags.Required),
                    new("fizz", TypeFactory.CreateStringLiteralType("pop"), TypePropertyFlags.Required),
                    new("bar", LanguageConstants.Int, TypePropertyFlags.Required),
                },
                null),
        };

        var collapsed = TypeHelper.TryCollapseTypes(prospectiveTaggedUnionMembers);

        collapsed.Should().BeOfType<DiscriminatedObjectType>();
        collapsed.As<DiscriminatedObjectType>().DiscriminatorKey.Should().Be("type");
    }

    [TestMethod]
    public void Tagged_union_formation_should_prefer_discriminator_named_kind_when_multiple_candidates_exist()
    {
        var prospectiveTaggedUnionMembers = new ObjectType[]
        {
            new("{type: 'a', foo: string}",
                default,
                new NamedTypeProperty[]
                {
                    new("kind", TypeFactory.CreateStringLiteralType("a"), TypePropertyFlags.Required),
                    new("fizz", TypeFactory.CreateStringLiteralType("buzz"), TypePropertyFlags.Required),
                    new("foo", LanguageConstants.String, TypePropertyFlags.Required),
                },
                null),
            new("{type: 'b', bar: int}",
                default,
                new NamedTypeProperty[]
                {
                    new("kind", TypeFactory.CreateStringLiteralType("b"), TypePropertyFlags.Required),
                    new("fizz", TypeFactory.CreateStringLiteralType("pop"), TypePropertyFlags.Required),
                    new("bar", LanguageConstants.Int, TypePropertyFlags.Required),
                },
                null),
        };

        var collapsed = TypeHelper.TryCollapseTypes(prospectiveTaggedUnionMembers);

        collapsed.Should().BeOfType<DiscriminatedObjectType>();
        collapsed.As<DiscriminatedObjectType>().DiscriminatorKey.Should().Be("kind");
    }

    [TestMethod]
    public void Tagged_union_formation_should_prefer_alpha_sorted_discriminator_when_multiple_candidates_exist_but_not_kind_or_type()
    {
        var prospectiveTaggedUnionMembers = new ObjectType[]
        {
            new("{type: 'a', foo: string}",
                default,
                new NamedTypeProperty[]
                {
                    new("variety", TypeFactory.CreateStringLiteralType("a"), TypePropertyFlags.Required),
                    new("fizz", TypeFactory.CreateStringLiteralType("buzz"), TypePropertyFlags.Required),
                    new("foo", LanguageConstants.String, TypePropertyFlags.Required),
                },
                null),
            new("{type: 'b', bar: int}",
                default,
                new NamedTypeProperty[]
                {
                    new("variety", TypeFactory.CreateStringLiteralType("b"), TypePropertyFlags.Required),
                    new("fizz", TypeFactory.CreateStringLiteralType("pop"), TypePropertyFlags.Required),
                    new("bar", LanguageConstants.Int, TypePropertyFlags.Required),
                },
                null),
        };

        var collapsed = TypeHelper.TryCollapseTypes(prospectiveTaggedUnionMembers);

        collapsed.Should().BeOfType<DiscriminatedObjectType>();
        collapsed.As<DiscriminatedObjectType>().DiscriminatorKey.Should().Be("fizz");
    }

    [TestMethod]
    public void Object_collapse_should_incorporate_additionalProperties_types()
    {
        var toCollapse = new ObjectType[]
        {
            new("{foo: string}",
                default,
                new NamedTypeProperty[]
                {
                    new("foo", LanguageConstants.String, TypePropertyFlags.Required),
                },
                null),
            new("{bar: string, *: int}",
                default,
                new NamedTypeProperty[]
                {
                    new("bar", LanguageConstants.String, TypePropertyFlags.Required),
                },
                new TypeProperty(LanguageConstants.Int, Description: "Description of additional properties")),
        };

        var collapsed = TypeHelper.TryCollapseTypes(toCollapse).Should().BeAssignableTo<ObjectType>().Subject;
        collapsed.Properties.Should().HaveCount(2);
        collapsed.Properties.ContainsKey("foo").Should().BeTrue();
        collapsed.Properties["foo"].TypeReference.Type.Name.Should().Be("int | string");
        collapsed.Properties["foo"].Flags.HasFlag(TypePropertyFlags.Required).Should().BeFalse();

        collapsed.Properties.ContainsKey("bar").Should().BeTrue();
        collapsed.Properties["bar"].TypeReference.Type.Name.Should().Be("string");
        collapsed.Properties["bar"].Flags.HasFlag(TypePropertyFlags.Required).Should().BeFalse();

        var addlProps = collapsed.AdditionalProperties;
        addlProps.Should().NotBeNull();
        addlProps!.TypeReference.Type.Should().NotBeNull();
        addlProps.TypeReference.Type.Name.Should().Be("int");
        addlProps.Description.Should().Be("Description of additional properties");
        addlProps.Flags.HasFlag(TypePropertyFlags.FallbackProperty).Should().BeTrue();
    }

    [TestMethod]
    public void Scope_reference_objects_should_not_be_collapsed()
    {
        var scopeRef = new ResourceGroupScopeType(
            ImmutableArray<FunctionArgumentSyntax>.Empty,
            ImmutableArray<NamedTypeProperty>.Empty);

        var objects = new ObjectType[]
        {
            new("{foo: string}",
                default,
                new NamedTypeProperty[]
                {
                    new("foo", LanguageConstants.String, TypePropertyFlags.Required),
                },
                null),
            new("{bar: string}",
                default,
                new NamedTypeProperty[]
                {
                    new("bar", LanguageConstants.String, TypePropertyFlags.Required),
                },
                new TypeProperty(LanguageConstants.Int)),
        };

        TypeHelper.TryCollapseTypes(scopeRef.AsEnumerable()).Should().BeSameAs(scopeRef);
        TypeHelper.TryCollapseTypes(objects).Should().BeAssignableTo<ObjectType>();
        TypeHelper.TryCollapseTypes(objects.Append(scopeRef)).Should().BeNull();
    }

    [TestMethod]
    public void Namespace_objects_should_not_be_collapsed()
    {
        var @namespace = new NamespaceType(
            "alias",
            new(true, "bicepExtensionName", null, "templateExtensionName", "1.0"),
            ImmutableArray<NamedTypeProperty>.Empty,
            ImmutableArray<FunctionOverload>.Empty,
            ImmutableArray<BannedFunction>.Empty,
            ImmutableArray<Decorator>.Empty,
            new EmptyResourceTypeProvider());

        var objects = new ObjectType[]
        {
            new("{foo: string}",
                default,
                new NamedTypeProperty[]
                {
                    new("foo", LanguageConstants.String, TypePropertyFlags.Required),
                },
                null),
            new("{bar: string}",
                default,
                new NamedTypeProperty[]
                {
                    new("bar", LanguageConstants.String, TypePropertyFlags.Required),
                },
                new TypeProperty(LanguageConstants.Int)),
        };

        TypeHelper.TryCollapseTypes(@namespace.AsEnumerable()).Should().BeSameAs(@namespace);
        TypeHelper.TryCollapseTypes(objects).Should().BeAssignableTo<ObjectType>();
        TypeHelper.TryCollapseTypes(objects.Append(@namespace)).Should().BeNull();
    }
}
