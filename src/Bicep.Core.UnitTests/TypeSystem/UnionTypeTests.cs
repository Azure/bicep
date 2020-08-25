// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.TypeSystem;
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
            var actual = UnionType.Create();
            actual.Name.Should().Be("never");
            actual.TypeKind.Should().Be(TypeKind.Never);
            actual.Should().BeOfType<UnionType>();

            ((UnionType) actual).Members.Should().BeEmpty();
        }

        [TestMethod]
        public void FlatUnionTypeShouldBeConstructedCorrectlyViaParamsSyntax()
        {
            var actual = UnionType.Create(LanguageConstants.Bool, LanguageConstants.String);
            actual.Name.Should().Be("bool | string");
            actual.TypeKind.Should().Be(TypeKind.Union);
            actual.Should().BeOfType<UnionType>();

            ((UnionType) actual).Members.Should().Equal(LanguageConstants.Bool, LanguageConstants.String);
        }

        [TestMethod]
        public void FlatUnionTypeShouldBeConstructedCorrectlyViaEnumerableSyntax()
        {
            var actual = UnionType.Create(LanguageConstants.Null, LanguageConstants.Int, LanguageConstants.Bool);
            actual.Name.Should().Be("bool | int | null");
            actual.TypeKind.Should().Be(TypeKind.Union);
            actual.Should().BeOfType<UnionType>();

            ((UnionType)actual).Members.Should().Equal(LanguageConstants.Bool, LanguageConstants.Int, LanguageConstants.Null);
        }

        [TestMethod]
        public void UnionTypeShouldFlattenInnerUnionsAndDeduplicateTypes()
        {
            var innerMost = UnionType.Create(LanguageConstants.Null, LanguageConstants.String);
            var inner = UnionType.Create(innerMost, LanguageConstants.Array);
            var actual = UnionType.Create(LanguageConstants.Bool, inner, LanguageConstants.Object);

            actual.Name.Should().Be("array | bool | null | object | string");
            actual.TypeKind.Should().Be(TypeKind.Union);
            actual.Should().BeOfType<UnionType>();

            ((UnionType) actual).Members.Should().Equal(LanguageConstants.Array, LanguageConstants.Bool, LanguageConstants.Null, LanguageConstants.Object, LanguageConstants.String);
        }

        [TestMethod]
        public void UnionTypeShouldDeduplicateTypes()
        {
            var innerMost = UnionType.Create(LanguageConstants.Null, LanguageConstants.String, LanguageConstants.String);
            var inner = UnionType.Create(innerMost, LanguageConstants.Array, LanguageConstants.Null, LanguageConstants.Array);
            var actual = UnionType.Create(LanguageConstants.Bool, LanguageConstants.Null, inner, inner, LanguageConstants.Object, innerMost, LanguageConstants.String);

            actual.Name.Should().Be("array | bool | null | object | string");
            actual.TypeKind.Should().Be(TypeKind.Union);
            actual.Should().BeOfType<UnionType>();

            ((UnionType)actual).Members.Should().Equal(LanguageConstants.Array, LanguageConstants.Bool, LanguageConstants.Null, LanguageConstants.Object, LanguageConstants.String);
        }

        [TestMethod]
        public void SingletonUnionCreationShouldProduceSingletonType()
        {
            UnionType.Create(LanguageConstants.Int).Should().BeSameAs(LanguageConstants.Int);
            UnionType.Create(LanguageConstants.String).Should().BeSameAs(LanguageConstants.String);

            UnionType.Create(UnionType.Create(UnionType.Create(LanguageConstants.Bool))).Should().BeSameAs(LanguageConstants.Bool);
        }
    }
}

