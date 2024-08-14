// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.TypeSystem
{
    [TestClass]
    public class UnionTypeTests
    {
        [TestMethod]
        public void CreatingEmptyUnionShouldProduceTheNeverType()
        {
            var actual = TypeHelper.CreateTypeUnion(Enumerable.Empty<ITypeReference>());
            actual.Name.Should().Be("never");
            actual.TypeKind.Should().Be(TypeKind.Never);
            actual.Should().BeOfType<UnionType>();

            ((UnionType)actual).Members.Should().BeEmpty();
        }

        [TestMethod]
        public void FlatUnionTypeShouldBeConstructedCorrectlyViaParamsSyntax()
        {
            var actual = TypeHelper.CreateTypeUnion(LanguageConstants.Bool, LanguageConstants.String);
            actual.Name.Should().Be("bool | string");
            actual.TypeKind.Should().Be(TypeKind.Union);
            actual.Should().BeOfType<UnionType>();

            ((UnionType)actual).Members.Select(x => x.Type).Should().Equal(LanguageConstants.Bool, LanguageConstants.String);
        }

        [TestMethod]
        public void FlatUnionTypeShouldBeConstructedCorrectlyViaEnumerableSyntax()
        {
            var actual = TypeHelper.CreateTypeUnion(LanguageConstants.Null, LanguageConstants.Int, LanguageConstants.Bool);
            actual.Name.Should().Be("bool | int | null");
            actual.TypeKind.Should().Be(TypeKind.Union);
            actual.Should().BeOfType<UnionType>();

            ((UnionType)actual).Members.Select(x => x.Type).Should().Equal(LanguageConstants.Bool, LanguageConstants.Int, LanguageConstants.Null);
        }

        [TestMethod]
        public void UnionTypeShouldFlattenInnerUnionsAndDeduplicateTypes()
        {
            var innerMost = TypeHelper.CreateTypeUnion(LanguageConstants.Null, LanguageConstants.String);
            var inner = TypeHelper.CreateTypeUnion(innerMost, LanguageConstants.Array);
            var actual = TypeHelper.CreateTypeUnion(LanguageConstants.Bool, inner, LanguageConstants.Object);

            actual.Name.Should().Be("array | bool | null | object | string");
            actual.TypeKind.Should().Be(TypeKind.Union);
            actual.Should().BeOfType<UnionType>();

            ((UnionType)actual).Members.Select(x => x.Type).Should().Equal(LanguageConstants.Array, LanguageConstants.Bool, LanguageConstants.Null, LanguageConstants.Object, LanguageConstants.String);
        }

        [TestMethod]
        public void UnionTypeShouldDeduplicateTypes()
        {
            var innerMost = TypeHelper.CreateTypeUnion(LanguageConstants.Null, LanguageConstants.String, LanguageConstants.String);
            var inner = TypeHelper.CreateTypeUnion(innerMost, LanguageConstants.Array, LanguageConstants.Null, LanguageConstants.Array);
            var actual = TypeHelper.CreateTypeUnion(LanguageConstants.Bool, LanguageConstants.Null, inner, inner, LanguageConstants.Object, innerMost, LanguageConstants.String);

            actual.Name.Should().Be("array | bool | null | object | string");
            actual.TypeKind.Should().Be(TypeKind.Union);
            actual.Should().BeOfType<UnionType>();

            ((UnionType)actual).Members.Select(x => x.Type).Should().Equal(LanguageConstants.Array, LanguageConstants.Bool, LanguageConstants.Null, LanguageConstants.Object, LanguageConstants.String);
        }

        [TestMethod]
        public void UnionTypeShouldDisplayStringLiteralsCorrectly()
        {
            var unionType = TypeHelper.CreateTypeUnion(
                TypeFactory.CreateStringLiteralType("Error"),
                TypeFactory.CreateStringLiteralType("Warning"),
                TypeFactory.CreateStringLiteralType("Info")
            );

            unionType.Name.Should().Be("'Error' | 'Info' | 'Warning'");
        }

        [TestMethod]
        public void UnionTypeInvolvingResourceScopeTypesShouldProduceExpectedDisplayString()
        {
            var unionType = TypeHelper.CreateTypeUnion(
                TypeFactory.CreateStringLiteralType("Test"),
                LanguageConstants.CreateResourceScopeReference(ResourceScope.Resource),
                LanguageConstants.CreateResourceScopeReference(ResourceScope.Subscription | ResourceScope.Tenant)
            );

            unionType.Name.Should().Be("'Test' | resource | (tenant | subscription)");
        }

        [TestMethod]
        public void SingletonUnionCreationShouldProduceSingletonType()
        {
            TypeHelper.CreateTypeUnion(LanguageConstants.Int).Should().BeSameAs(LanguageConstants.Int);
            TypeHelper.CreateTypeUnion(LanguageConstants.String).Should().BeSameAs(LanguageConstants.String);

            TypeHelper.CreateTypeUnion(TypeHelper.CreateTypeUnion(TypeHelper.CreateTypeUnion(LanguageConstants.Bool))).Should().BeSameAs(LanguageConstants.Bool);
        }

        [TestMethod]
        public void UnionsOfStringsAndStringLiteralTypesShouldNotDropLiterals()
        {
            TypeHelper.CreateTypeUnion(LanguageConstants.String, TypeFactory.CreateStringLiteralType("hello"), TypeFactory.CreateStringLiteralType("there")).Name.Should().Be("'hello' | 'there' | string");

            TypeHelper.CreateTypeUnion(LanguageConstants.String, TypeFactory.CreateStringLiteralType("hello"), TypeFactory.CreateStringLiteralType("there"), LanguageConstants.String).Name.Should().Be("'hello' | 'there' | string");

            TypeHelper.CreateTypeUnion(LanguageConstants.String, TypeFactory.CreateStringLiteralType("hello"), LanguageConstants.Bool, TypeFactory.CreateStringLiteralType("there")).Name.Should().Be("'hello' | 'there' | bool | string");
        }

        [TestMethod]
        public void UnionsOfUntypedAndTypedArraysShouldProduceUntypedArrayType()
        {
            TypeHelper.CreateTypeUnion(LanguageConstants.Array, new TypedArrayType(LanguageConstants.String, TypeSymbolValidationFlags.Default)).Should().BeSameAs(LanguageConstants.Array);

            var actual = TypeHelper.CreateTypeUnion(
                LanguageConstants.Array,
                new TypedArrayType(LanguageConstants.Int, TypeSymbolValidationFlags.Default),
                LanguageConstants.String,
                new ObjectType("myObj", TypeSymbolValidationFlags.Default, [], null));
            actual.Name.Should().Be("array | myObj | string");
        }

        [TestMethod]
        public void UnionsInvolvingAnyTypeShouldProduceAnyType()
        {
            TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Int, new TypedArrayType(LanguageConstants.Int, TypeSymbolValidationFlags.Default), LanguageConstants.Any).Should().BeSameAs(LanguageConstants.Any);
        }
    }
}
