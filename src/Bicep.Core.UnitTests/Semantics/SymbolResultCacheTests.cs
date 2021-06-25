// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using Bicep.Core.Semantics;
using FluentAssertions;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Semantics
{
    [TestClass]
    public class SymbolResultCacheTests
    {
        private static readonly MockRepository Repository = new MockRepository(MockBehavior.Strict);

        [TestMethod]
        public void Lookup_should_use_the_getter_function_to_select_items()
        {
            var trueSymbolMock = Repository.Create<Symbol>("casper");
            var falseSymbolMock = Repository.Create<Symbol>("evelynn");

            var cache = new SymbolResultCache<bool>(symbol => symbol.Name == "casper");

            cache.Lookup(trueSymbolMock.Object).Should().BeTrue();
            cache.Lookup(falseSymbolMock.Object).Should().BeFalse();
        }

        [TestMethod]
        public void Lookup_should_only_call_the_getter_func_once_per_symbol()
        {
            var symbol1Mock = Repository.Create<Symbol>("kira");
            var symbol2Mock = Repository.Create<Symbol>("indy");
            var callCounts = new Dictionary<Symbol, int>
            {
                [symbol1Mock.Object] = 0,
                [symbol2Mock.Object] = 0,
            };

            bool returnTrueAndIncrementCallCounts(Symbol symbol)
            {
                callCounts[symbol]++;
                return true;
            }

            var cache = new SymbolResultCache<bool>(returnTrueAndIncrementCallCounts);

            callCounts[symbol1Mock.Object].Should().Be(0);
            cache.Lookup(symbol1Mock.Object).Should().BeTrue();
            callCounts[symbol1Mock.Object].Should().Be(1);

            cache.Lookup(symbol1Mock.Object).Should().BeTrue();
            cache.Lookup(symbol1Mock.Object).Should().BeTrue();
            cache.Lookup(symbol1Mock.Object).Should().BeTrue();
            callCounts[symbol1Mock.Object].Should().Be(1);

            callCounts[symbol2Mock.Object].Should().Be(0);
            cache.Lookup(symbol2Mock.Object).Should().BeTrue();
            callCounts[symbol2Mock.Object].Should().Be(1);

            cache.Lookup(symbol2Mock.Object).Should().BeTrue();
            cache.Lookup(symbol2Mock.Object).Should().BeTrue();
            cache.Lookup(symbol2Mock.Object).Should().BeTrue();
            callCounts[symbol2Mock.Object].Should().Be(1);
        }

        [TestMethod]
        public void Lookup_allows_exceptions_to_bubble_up()
        {
            var symbolMock = Repository.Create<Symbol>("casper");
            bool throwAnException(Symbol symbol)
            {
                throw new InvalidOperationException();
            }

            var cache = new SymbolResultCache<bool>(throwAnException);

            cache.Invoking(x => x.Lookup(symbolMock.Object)).Should().Throw<InvalidOperationException>();
        }
    }
}
