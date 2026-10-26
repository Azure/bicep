// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class UseRecognizedResourceTypeRuleTests : LinterRuleTestsBase
{
    private void CompileAndTest(string text, int expectedDiagnosticCount, Options? options = null)
        => AssertLinterRuleDiagnostics(UseRecognizedResourceTypeRule.Code, text, expectedDiagnosticCount, options ?? new Options(OnCompileErrors.Ignore, IncludePosition.None));

    private void CompileAndTest(string text, string[] expectedMessages, Options? options = null)
        => AssertLinterRuleDiagnostics(UseRecognizedResourceTypeRule.Code, text, expectedMessages, options ?? new Options(OnCompileErrors.Ignore, IncludePosition.None));

    [TestMethod]
    public void Rule_should_warn_for_unrecognized_resource_type_in_reference()
    {
        CompileAndTest(
            """
            output foo object = reference('Microsoft.Foo/bar', '2020-01-01')
            """,
            1);
    }

    [TestMethod]
    public void Rule_should_warn_for_unrecognized_resource_type_in_list_function()
    {
        CompileAndTest(
            """
            output foo object = listKeys('Microsoft.Foo/bar', '2020-01-01')
            """,
            1);
    }

    [TestMethod]
    public void Rule_should_not_warn_for_recognized_resource_type_in_reference()
    {
        CompileAndTest(
            """
            output foo object = reference('Microsoft.Storage/storageAccounts', '2022-09-01')
            """,
            0);
    }

    [TestMethod]
    public void Rule_should_not_warn_when_reference_has_no_arguments()
    {
        // This will produce a compile error, not a linter warning
        CompileAndTest(
            """
            output foo object = reference()
            """,
            0);
    }

    [TestMethod]
    public void Rule_should_not_warn_when_first_arg_is_not_a_resource_type_string()
    {
        // When the first arg is a non-resource-type string (e.g. a resource name), don't warn
        CompileAndTest(
            """
            output foo object = reference('myResourceName', '2020-01-01')
            """,
            0);
    }

    [TestMethod]
    public void Rule_should_warn_for_unrecognized_type_in_resourceId_call()
    {
        CompileAndTest(
            """
            output foo object = reference(resourceId('Microsoft.Foo/bar', 'name1'), '2020-01-01')
            """,
            1);
    }

    [TestMethod]
    public void Rule_should_not_warn_for_recognized_type_in_resourceId_call()
    {
        CompileAndTest(
            """
            output foo object = reference(resourceId('Microsoft.Storage/storageAccounts', 'name1'), '2022-09-01')
            """,
            0);
    }

    [TestMethod]
    public void Rule_should_warn_for_unrecognized_type_with_variable_reference()
    {
        CompileAndTest(
            """
            var resType = 'Microsoft.Foo/bar'
            output foo object = reference(resType, '2020-01-01')
            """,
            1);
    }

    [TestMethod]
    public void Rule_should_not_warn_for_parameter_with_no_default()
    {
        // When a parameter is used and has no default value, we can't determine the resource type
        CompileAndTest(
            """
            param resType string
            output foo object = reference(resType, '2020-01-01')
            """,
            0);
    }

    [TestMethod]
    public void Rule_should_warn_for_unrecognized_type_in_listKeys()
    {
        CompileAndTest(
            """
            output foo object = listKeys('Microsoft.Fake/nonExistent', '2020-01-01')
            """,
            1);
    }

    [TestMethod]
    public void Rule_message_includes_resource_type_and_function_name()
    {
        CompileAndTest(
            """
            output foo object = reference('Microsoft.Foo/bar', '2020-01-01')
            """,
            [
                """Resource type "Microsoft.Foo/bar" is not recognized in function "reference". If this resource type does exist, the API version must be specified as a function argument.""",
            ]);
    }
}
