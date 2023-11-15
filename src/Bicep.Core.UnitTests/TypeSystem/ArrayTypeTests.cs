// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
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
            Create(TypeHelper.CreateTypeUnion(TypeFactory.CreateStringLiteralType("one"), TypeFactory.CreateStringLiteralType("two"))).Name.Should().Be("('one' | 'two')[]");

            Create(TypeHelper.CreateTypeUnion(LanguageConstants.CreateResourceScopeReference(ResourceScope.ManagementGroup), TypeFactory.CreateStringLiteralType("test"))).Name
                .Should().Be("('test' | managementGroup)[]");

            Create(TypeHelper.CreateTypeUnion(LanguageConstants.CreateResourceScopeReference(ResourceScope.ManagementGroup | ResourceScope.Tenant), TypeFactory.CreateStringLiteralType("test"))).Name
                .Should().Be("('test' | (tenant | managementGroup))[]");
        }

        private static TypedArrayType Create(TypeSymbol itemType) => new(itemType, TypeSymbolValidationFlags.Default);
    }
}
