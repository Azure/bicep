// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.UnitTests.Assertions;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Cli.IntegrationTests.Commands;

[TestClass]
public class ConsoleCommandTests : TestBase
{
    [TestMethod]
    public async Task Redirected_input_context_with_single_line_input_should_succeed()
    {
        var input = "concat('Hello', ' ', 'World', '!')";

        var inputContext = new InputContext(
            Reader: new StringReader(input),
            IsRedirected: true);

        var result = await Bicep(inputContext, "console");
        
        result.Should().Succeed();
        result.WithoutAnsi().Stdout.Should().BeEquivalentToIgnoringNewlines("""
'Hello World!'

""");
    }

    [TestMethod]
    public async Task Redirected_input_context_with_multi_line_input_should_succeed()
    {
        var input = """
            var greeting = 'Hello'
            var target = {
                value: 'World'
            }
            
            '${greeting} ${target.value}!'
            """;

        var inputContext = new InputContext(
            Reader: new StringReader(input),
            IsRedirected: true);

        var result = await Bicep(inputContext, "console");

        result.Should().Succeed();
        result.WithoutAnsi().Stdout.Should().BeEquivalentToIgnoringNewlines("""
'Hello World!'

""");
    }
}
