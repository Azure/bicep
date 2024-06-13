// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.RegistryModuleTool.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.RegistryModuleTool.UnitTests.Extensions
{
    [TestClass]
    public class ITypeReferenceExtensionsTests
    {
        [DataTestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void GetPrimitiveTypeName_PossibleParameterOrOutputTypes_ReturnsPrimitiveTypeName(ITypeReference typeReference, string expectedPrimitiveTypeName)
        {
            var actual = typeReference.GetPrimitiveTypeName();

            actual.Should().Be(expectedPrimitiveTypeName);
        }

        private static IEnumerable<object[]> GetTestData()
        {
            yield return CreateTestCase(LanguageConstants.Int, LanguageConstants.Int.Name);
            yield return CreateTestCase(LanguageConstants.Bool, LanguageConstants.Bool.Name);
            yield return CreateTestCase(LanguageConstants.String, LanguageConstants.String.Name);
            yield return CreateTestCase(LanguageConstants.SecureString, "securestring");
            yield return CreateTestCase(LanguageConstants.Array, LanguageConstants.ArrayType);
            yield return CreateTestCase(LanguageConstants.Object, LanguageConstants.ObjectType);
            yield return CreateTestCase(LanguageConstants.SecureObject, "secureObject");
            yield return CreateTestCase(LanguageConstants.Null, LanguageConstants.NullKeyword);

            yield return CreateTestCase(TypeFactory.CreateIntegerLiteralType(1), LanguageConstants.Int.Name);
            yield return CreateTestCase(TypeFactory.CreateBooleanLiteralType(true), LanguageConstants.Bool.Name);
            yield return CreateTestCase(TypeFactory.CreateStringLiteralType("foobar"), LanguageConstants.String.Name);

            var tupleType = new TupleType([LanguageConstants.Int, LanguageConstants.Bool], TypeSymbolValidationFlags.Default);
            yield return CreateTestCase(tupleType, LanguageConstants.ArrayType);

            var discriminatedObjectType = new DiscriminatedObjectType("", TypeSymbolValidationFlags.Default, "", []);
            yield return CreateTestCase(discriminatedObjectType, LanguageConstants.ObjectType);

            var unionType = new UnionType(
                "int | bool | object | array",
                [
                    LanguageConstants.Int,
                    TypeFactory.CreateIntegerLiteralType(0),
                    TypeFactory.CreateBooleanLiteralType(false),
                    LanguageConstants.Object,
                    discriminatedObjectType,
                    tupleType,
                    LanguageConstants.Array,
                ]);

            yield return CreateTestCase(unionType, unionType.Name);

            static object[] CreateTestCase(ITypeReference typeReference, string expectedPrimitiveTypeName) =>
            [
                typeReference,
                expectedPrimitiveTypeName,
            ];
        }
    }
}
