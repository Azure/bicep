// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.TypeSystem
{
    [TestClass]
    public class ArrayTypeTests
    {
        [TestMethod]
        public void ArraysOfSimpleTypesShouldHaveExpectedDisplayString()
        {
            Create(LanguageConstants.Int).Name.Should().Be("int[]");
            Create(LanguageConstants.String).Name.Should().Be("string[]");
        }

        [TestMethod]
        public void ArraysOfCompoundTypesShouldHaveExpectedDisplayString()
        {
            Create(UnionType.Create(new StringLiteralType("one"), new StringLiteralType("two"))).Name.Should().Be("('one' | 'two')[]");

            Create(UnionType.Create(LanguageConstants.CreateResourceScopeReference(ResourceScope.ManagementGroup), new StringLiteralType("test"))).Name
                .Should().Be("('test' | managementGroup)[]");

            Create(UnionType.Create(LanguageConstants.CreateResourceScopeReference(ResourceScope.ManagementGroup | ResourceScope.Tenant), new StringLiteralType("test"))).Name
                .Should().Be("('test' | (tenant | managementGroup))[]");
        }

        private static TypedArrayType Create(TypeSymbol itemType) => new TypedArrayType(itemType, TypeSymbolValidationFlags.Default);
    }
}
