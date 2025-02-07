// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.TypeSystem
{
    [TestClass]
    public class DiscriminatedObjectTypeTests
    {
        [TestMethod]
        public void DiscriminatedObjectType_should_be_correctly_instantiated()
        {
            var objectA = new ObjectType("objA", TypeSymbolValidationFlags.Default, new[]
            {
                new NamedTypeProperty("discKey", TypeFactory.CreateStringLiteralType("keyA")),
                new NamedTypeProperty("keyAProp", LanguageConstants.String),
            }, null);

            var objectB = new ObjectType("objB", TypeSymbolValidationFlags.Default, new[]
            {
                new NamedTypeProperty("discKey", TypeFactory.CreateStringLiteralType("keyB")),
                new NamedTypeProperty("keyBProp", LanguageConstants.String),
            }, null);

            var discObj = new DiscriminatedObjectType("discObj", TypeSymbolValidationFlags.Default, "discKey", new[] { objectA, objectB });

            discObj.UnionMembersByKey.Keys.Should().BeEquivalentTo("'keyA'", "'keyB'");
            discObj.TypeKind.Should().Be(TypeKind.DiscriminatedObject);

            discObj.UnionMembersByKey[TypeFactory.CreateStringLiteralType("keyA").Name].Type.Should().Be(objectA);
            discObj.UnionMembersByKey[TypeFactory.CreateStringLiteralType("keyB").Name].Type.Should().Be(objectB);
        }

        [TestMethod]
        public void DiscriminatedObject_should_throw_for_various_badly_formatted_object_arguments()
        {
            var objectA = new ObjectType("objA", TypeSymbolValidationFlags.Default, new[]
            {
                new NamedTypeProperty("discKey", TypeFactory.CreateStringLiteralType("keyA")),
                new NamedTypeProperty("keyAProp", LanguageConstants.String),
            }, null);

            var missingKeyObject = new ObjectType("objB", TypeSymbolValidationFlags.Default, new[]
            {
                new NamedTypeProperty("keyBProp", LanguageConstants.String),
            }, null);
            Action missingKeyConstructorAction = () => new DiscriminatedObjectType("discObj", TypeSymbolValidationFlags.Default, "discKey", new[] { objectA, missingKeyObject });
            missingKeyConstructorAction.Should().Throw<ArgumentException>();

            var invalidKeyTypeObject = new ObjectType("objB", TypeSymbolValidationFlags.Default, new[]
            {
                new NamedTypeProperty("discKey", LanguageConstants.String),
                new NamedTypeProperty("keyBProp", LanguageConstants.String),
            }, null);
            Action invalidKeyTypeConstructorAction = () => new DiscriminatedObjectType("discObj", TypeSymbolValidationFlags.Default, "discKey", new[] { objectA, invalidKeyTypeObject });
            invalidKeyTypeConstructorAction.Should().Throw<ArgumentException>();

            var duplicateKeyObject = new ObjectType("objB", TypeSymbolValidationFlags.Default, new[]
            {
                new NamedTypeProperty("discKey", TypeFactory.CreateStringLiteralType("keyA")),
                new NamedTypeProperty("keyBProp", LanguageConstants.String),
            }, null);
            Action duplicateKeyConstructorAction = () => new DiscriminatedObjectType("discObj", TypeSymbolValidationFlags.Default, "discKey", new[] { objectA, duplicateKeyObject });
            duplicateKeyConstructorAction.Should().Throw<ArgumentException>();
        }
    }
}
