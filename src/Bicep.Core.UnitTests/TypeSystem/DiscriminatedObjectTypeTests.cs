// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.TypeSystem;
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
            var namedObjectA = new NamedObjectType("objA", new []
            { 
                new TypeProperty("discKey", new StringLiteralType("keyA")),
                new TypeProperty("keyAProp", LanguageConstants.String),
            }, null);

            var namedObjectB = new NamedObjectType("objB", new []
            { 
                new TypeProperty("discKey", new StringLiteralType("keyB")),
                new TypeProperty("keyBProp", LanguageConstants.String),
            }, null);

            var discObj = new DiscriminatedObjectType("discObj", "discKey", new [] { namedObjectA, namedObjectB });

            discObj.UnionMembersByKey.Keys.Should().BeEquivalentTo("'keyA'", "'keyB'");
            discObj.TypeKind.Should().Be(TypeKind.DiscriminatedObject);

            discObj.UnionMembersByKey[new StringLiteralType("keyA").Name].Type.Should().Be(namedObjectA);
            discObj.UnionMembersByKey[new StringLiteralType("keyB").Name].Type.Should().Be(namedObjectB);
        }

        [TestMethod]
        public void DiscriminatedObject_should_throw_for_various_badly_formatted_object_arguments()
        {
            var namedObjectA = new NamedObjectType("objA", new []
            { 
                new TypeProperty("discKey", new StringLiteralType("keyA")),
                new TypeProperty("keyAProp", LanguageConstants.String),
            }, null);

            var missingKeyObject = new NamedObjectType("objB", new []
            {
                new TypeProperty("keyBProp", LanguageConstants.String),
            }, null);
            Action missingKeyConstructorAction = () => new DiscriminatedObjectType("discObj", "discKey", new [] { namedObjectA, missingKeyObject });
            missingKeyConstructorAction.Should().Throw<ArgumentException>();

            var invalidKeyTypeObject = new NamedObjectType("objB", new []
            { 
                new TypeProperty("discKey", LanguageConstants.String),
                new TypeProperty("keyBProp", LanguageConstants.String),
            }, null);
            Action invalidKeyTypeConstructorAction = () => new DiscriminatedObjectType("discObj", "discKey", new [] { namedObjectA, invalidKeyTypeObject });
            invalidKeyTypeConstructorAction.Should().Throw<ArgumentException>();

            var duplicateKeyObject = new NamedObjectType("objB", new []
            { 
                new TypeProperty("discKey", new StringLiteralType("keyA")),
                new TypeProperty("keyBProp", LanguageConstants.String),
            }, null);
            Action duplicateKeyConstructorAction = () => new DiscriminatedObjectType("discObj", "discKey", new [] { namedObjectA, duplicateKeyObject });
            duplicateKeyConstructorAction.Should().Throw<ArgumentException>();
        }
    }
}