// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Semantics
{
    [TestClass]
    public class BinaryOperationResolverTests
    {
        [DynamicData(nameof(GetAllBinaryOperators), DynamicDataSourceType.Method)]
        [DataTestMethod]
        public void OverloadedOperatorsShouldHaveSameReturnType(BinaryOperator @operator)
        {
            var matches = BinaryOperationResolver.GetMatches(@operator).ToImmutableArray();
            matches.Should().HaveCountGreaterThanOrEqualTo(1);

            var first = matches.First();

            matches.Should().AllSatisfy(match => match.ReturnType.Should().Be(first.ReturnType), "because the type checker depends on overloaded operators having the same return type");
        }

        private static IEnumerable<object[]> GetAllBinaryOperators() => Enum.GetValues<BinaryOperator>().Select(value => new object[] { value });
    }
}
