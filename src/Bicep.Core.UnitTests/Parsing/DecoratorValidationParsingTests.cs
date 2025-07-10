// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Parsing
{
    [TestClass]
    public class DecoratorValidationParsingTests
    {
        [TestMethod]
        public void Decorator_on_using_statement_should_raise_error()
        {
            var program = ParserHelper.ParamsParse(@"
                @description('test')
                using 'main.bicep'
            ", out var lexingErrors, out var parsingErrors);

            var diagnostics = lexingErrors.Concat(parsingErrors);
            diagnostics.Should().Contain(d => d.Code == "BCP423" && d.Message.Contains("Decorators are not allowed on using declarations"));
        }

        [TestMethod]
        public void Decorator_on_extends_statement_should_raise_error()
        {
            var program = ParserHelper.ParamsParse(@"
                @description('test')
                extends 'parent.bicep'
            ", out var lexingErrors, out var parsingErrors);

            var diagnostics = lexingErrors.Concat(parsingErrors);
            diagnostics.Should().Contain(d => d.Code == "BCP424" && d.Message.Contains("Decorators are not allowed on extends declarations"));
        }

        [TestMethod]
        public void Decorator_on_param_assignment_should_raise_error()
        {
            var program = ParserHelper.ParamsParse(@"
                using 'main.bicep'
                
                @description('test')
                param myParam = 'value'
            ", out var lexingErrors, out var parsingErrors);

            var diagnostics = lexingErrors.Concat(parsingErrors);
            diagnostics.Should().Contain(d => d.Code == "BCP425" && d.Message.Contains("Decorators are not allowed on parameter assignments"));
        }

        [TestMethod]
        public void Multiple_decorators_on_using_statement_should_raise_multiple_errors()
        {
            var program = ParserHelper.ParamsParse(@"
                @description('test')
                @metadata({ author: 'me' })
                using 'main.bicep'
            ", out var lexingErrors, out var parsingErrors);

            var diagnostics = lexingErrors.Concat(parsingErrors);
            var decoratorErrors = diagnostics.Where(d => d.Code == "BCP423").ToList();
            decoratorErrors.Should().HaveCount(2);
        }

        [TestMethod]
        public void Valid_statements_without_decorators_should_not_raise_errors()
        {
            var program = ParserHelper.ParamsParse(@"
                using 'main.bicep'
                extends 'parent.bicep'
                param myParam = 'value'
            ", out var lexingErrors, out var parsingErrors);

            var diagnostics = lexingErrors.Concat(parsingErrors);
            diagnostics.Should().NotContain(d => d.Code is "BCP423" or "BCP424" or "BCP425");
        }
    }
}