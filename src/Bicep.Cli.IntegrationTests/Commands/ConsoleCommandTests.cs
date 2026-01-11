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
        // "concat('Hello', ' ', 'World', '!')" | bicep console
        var input = "concat('Hello', ' ', 'World', '!')";
        var result = await Bicep(
            (@out, err) => new IOContext(
                new(new StringReader(input), IsRedirected: true),
                new(@out, IsRedirected: false),
                new(err, IsRedirected: false)),
            "console");

        result.Should().Succeed();
        result.WithoutAnsi().Stdout.Should().BeEquivalentToIgnoringNewlines("""
'Hello World!'

""");
    }

    [TestMethod]
    public async Task Redirected_input_context_with_multi_line_input_should_succeed()
    {
        /*
var greeting = 'Hello'
var target = {
    value: 'World'
}

'${greeting} ${target.value}!' | bicep console
         */
        var input = """
            var greeting = 'Hello'
            var target = {
                value: 'World'
            }

            '${greeting} ${target.value}!'
            """;

        var result = await Bicep(
            (@out, err) => new IOContext(
                new(new StringReader(input), IsRedirected: true),
                new(@out, IsRedirected: false),
                new(err, IsRedirected: false)),
            "console");

        result.Should().Succeed();
        result.WithoutAnsi().Stdout.Should().BeEquivalentToIgnoringNewlines("""
'Hello World!'

""");
    }

    [TestMethod]
    public async Task Redirected_output_context_should_not_have_ansi_codes()
    {
        // "concat('Hello', ' ', 'World', '!')" | bicep console >
        var input = "concat('Hello', ' ', 'World', '!')";
        var result = await Bicep(
            (@out, err) => new IOContext(
                new(new StringReader(input), IsRedirected: true),
                new(@out, IsRedirected: true),
                new(err, IsRedirected: false)),
            "console");

        result.Should().Succeed();
        var withoutAnsi = result.WithoutAnsi();
        result.Stdout.Should().Be(withoutAnsi.Stdout);
    }
}
