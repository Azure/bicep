// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Extensions;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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
            Row(TypeHelper.CreateTypeUnion(TypeFactory.CreateArrayType(minLength: 6, maxLength: 9), new TupleType(ImmutableArray.Create<ITypeReference>(LanguageConstants.String, LanguageConstants.Int), default)),
                TypeFactory.CreateArrayType(minLength: 6, maxLength: 7),
                new TupleType(ImmutableArray.Create<ITypeReference>(LanguageConstants.String, LanguageConstants.Int), default),
                TypeFactory.CreateArrayType(minLength: 8, maxLength: 9)),
            // collapse(string[], [string, string, string]) -> string[]
            Row(TypeFactory.CreateArrayType(LanguageConstants.String),
                TypeFactory.CreateArrayType(LanguageConstants.String),
                new TupleType(ImmutableArray.Create<ITypeReference>(LanguageConstants.String, LanguageConstants.String, LanguageConstants.String), default)),
            // collapse(string[], [string, int, string]) -> union(string[], [string, int, string])
            Row(TypeHelper.CreateTypeUnion(TypeFactory.CreateArrayType(LanguageConstants.String), new TupleType(ImmutableArray.Create<ITypeReference>(LanguageConstants.String, LanguageConstants.Int, LanguageConstants.String), default)),
                TypeFactory.CreateArrayType(LanguageConstants.String),
                new TupleType(ImmutableArray.Create<ITypeReference>(LanguageConstants.String, LanguageConstants.Int, LanguageConstants.String), default)),
            // collapse((string | int)[], [string, int, string]) -> (string | int)[]
            Row(TypeFactory.CreateArrayType(TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Int)),
                TypeFactory.CreateArrayType(TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Int)),
                new TupleType(ImmutableArray.Create<ITypeReference>(LanguageConstants.String, LanguageConstants.Int, LanguageConstants.String), default)),
        };
    }
}
