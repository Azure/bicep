// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class DecoratorValidationIntegrationTests
    {
        [TestMethod]
        public void Decorator_on_using_statement_should_raise_error()
        {
            var result = CompilationHelper.CompileParams(
                ("parameters.bicepparam", @"
                    @description('test')
                    using 'main.bicep'
                    
                    param myParam = 'value'
                "),
                ("main.bicep", @"
                    param myParam string
                "));

            result.Should().ContainDiagnostic("BCP423", DiagnosticLevel.Error, "Decorators are not allowed on using declarations.");
        }

        [TestMethod]
        public void Decorator_on_extends_statement_should_raise_error()
        {
            var result = CompilationHelper.CompileParams(
                ("parameters.bicepparam", @"
                    @description('test')
                    extends 'parent.bicep'
                    
                    param myParam = 'value'
                "),
                ("parent.bicep", @"
                    param myParam string
                "));

            result.Should().ContainDiagnostic("BCP424", DiagnosticLevel.Error, "Decorators are not allowed on extends declarations.");
        }

        [TestMethod]
        public void Decorator_on_param_assignment_should_raise_error()
        {
            var result = CompilationHelper.CompileParams(
                ("parameters.bicepparam", @"
                    using 'main.bicep'
                    
                    @description('test')
                    param myParam = 'value'
                "),
                ("main.bicep", @"
                    param myParam string
                "));

            result.Should().ContainDiagnostic("BCP425", DiagnosticLevel.Error, "Decorators are not allowed on parameter assignments in .bicepparam files.");
        }

        [TestMethod]
        public void Multiple_decorators_on_statements_should_raise_multiple_errors()
        {
            var result = CompilationHelper.CompileParams(
                ("parameters.bicepparam", @"
                    @description('test')
                    @metadata({ author: 'me' })
                    using 'main.bicep'
                    
                    @description('param test')
                    param myParam = 'value'
                "),
                ("main.bicep", @"
                    param myParam string
                "));

            result.Diagnostics.Should().Contain(d => d.Code == "BCP423");
            result.Diagnostics.Where(d => d.Code == "BCP423").Should().HaveCount(2);
            result.Diagnostics.Should().Contain(d => d.Code == "BCP425");
        }

        [TestMethod]
        public void Valid_statements_without_decorators_should_not_raise_decorator_errors()
        {
            var result = CompilationHelper.CompileParams(
                ("parameters.bicepparam", @"
                    using 'main.bicep'
                    param myParam = 'value'
                "),
                ("main.bicep", @"
                    param myParam string
                "));

            result.Diagnostics.Should().NotContain(d => d.Code is "BCP423" or "BCP424" or "BCP425");
        }

        [TestMethod]
        public void Invalid_decorator_syntax_in_example_from_issue_should_raise_errors()
        {
            var result = CompilationHelper.CompileParams(
                ("parameters.bicepparam", @"
                    @foo('bar')
                    using 'foo.bicep'

                    @notARealDecorator(1, 2, module 34)
                    param fizz = 'buzz'
                "),
                ("foo.bicep", @"
                    param fizz string
                "));

            result.Diagnostics.Should().Contain(d => d.Code == "BCP423");
            result.Diagnostics.Should().Contain(d => d.Code == "BCP425");
        }

        [TestMethod]
        public void Regular_bicep_files_should_still_allow_decorators_on_param_declarations()
        {
            // Test that normal .bicep files are not affected by this change
            var result = CompilationHelper.Compile(@"
                @description('This is a valid parameter decorator')
                @secure()
                param mySecureParam string
                
                output test string = mySecureParam
            ");

            // Should not contain our new error codes
            result.Diagnostics.Should().NotContain(d => d.Code is "BCP423" or "BCP424" or "BCP425");
        }
    }
}