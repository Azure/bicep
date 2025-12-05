// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class DirectiveTests
{
    [TestMethod]
    public void DisableNextLine_should_disable_diagnostics_on_next_line_only()
    {
        var result = CompilationHelper.Compile("""
            // Some initial comment
            // that describes the file
            
            #disable-next-line no-unused-params
            param unusedParam string
            param anotherUnusedParam string
            """);

        result.Diagnostics.Should().HaveCount(1);
        result.Diagnostics.Single().Code.Should().Be("no-unused-params");
        result.Diagnostics.Single().Message.Should().Be("Parameter \"anotherUnusedParam\" is declared but never used.");
    }

    [TestMethod]
    public void DisableDiagnostics_and_RestoreDiagnostics_should_work_as_expected()
    {
        var result = CompilationHelper.Compile("""
            // Some initial comment
            // that describes the file

            #disable-diagnostics no-unused-params no-unused-vars
            param unusedParam1 string
            var unusedVariable = 42

            #restore-diagnostics no-unused-vars
            param unusedParam2 string
            var anotherUnusedVariable = 100

            #restore-diagnostics no-unused-params
            param unusedParam3 string
            """);

        result.Diagnostics.Should().SatisfyRespectively(
            diag =>
            {
                diag.Code.Should().Be("no-unused-vars");
                diag.Message.Should().Be("Variable \"anotherUnusedVariable\" is declared but never used.");
            },
            diag =>
            {
                diag.Code.Should().Be("no-unused-params");
                diag.Message.Should().Be("Parameter \"unusedParam3\" is declared but never used.");
            });
    }

    [TestMethod]
    public void Unclosed_disableDiagnostics_should_persist_to_end_of_file()
    {
        var result = CompilationHelper.Compile("""
            #disable-diagnostics no-unused-params
            param unusedParam1 string
            param unusedParam2 string
            param unusedParam3 string
            param unusedParam4 string
            param unusedParam5 string
            param unusedParam6 string
            param unusedParam7 string
            param unusedParam8 string
            param unusedParam9 string
            param unusedParam10 string
            """);

        result.Diagnostics.Should().BeEmpty();
    }

    [TestMethod]
    public void RestoreDiagnostics_without_matching_disable_should_have_no_effect()
    {
        var result = CompilationHelper.Compile("""
            #restore-diagnostics no-unused-params
            param unusedParam string
            """);
        result.Diagnostics.Should().HaveCount(1);
        result.Diagnostics.Single().Code.Should().Be("no-unused-params");
        result.Diagnostics.Single().Message.Should().Be("Parameter \"unusedParam\" is declared but never used.");
    }
}
