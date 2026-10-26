// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class UserDefinedFunctionTests
{
    [NotNull] public TestContext? TestContext { get; set; }

    [TestMethod]
    public void User_defined_functions_basic_case()
    {
        var result = CompilationHelper.Compile(@"
func buildUrl(https bool, hostname string, path string) string => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'

output foo string = buildUrl(true, 'google.com', 'search')
");

        result.Should().NotHaveAnyDiagnostics();
        var evaluated = TemplateEvaluator.Evaluate(result.Template).ToJToken();

        evaluated.Should().HaveValueAtPath("$.outputs['foo'].value", "https://google.com/search");
    }

    [TestMethod]
    public void Outer_scope_symbolic_references_are_blocked()
    {
        var result = CompilationHelper.Compile(@"
param foo string
var bar = 'abc'
func getBaz() string => 'baz'

func testFunc(baz string) string => '${foo}-${bar}-${baz}-${getBaz()}'
");

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
            ("BCP057", DiagnosticLevel.Error, """The name "foo" does not exist in the current context."""),
        });
    }

    [TestMethod]
    public void User_defined_functions_import_variable()
    {
        var result = CompilationHelper.Compile([("exports.bicep", @"
    @export()
    var greeting = 'Hello {0}!'
"),
            ("main.bicep", @"
import { greeting } from './exports.bicep'
func greet(name string) string =>  format(greeting, name)

output outputFoo string = greet('userName')
")]);

        result.Should().NotHaveAnyDiagnostics();
        var evaluated = TemplateEvaluator.Evaluate(result.Template).ToJToken();

        evaluated.Should().HaveValueAtPath("$.outputs['outputFoo'].value", "Hello userName!");
    }

    [TestMethod]
    public void Outer_scope_symbolic_variables_are_allowed()
    {
        var result = CompilationHelper.Compile(@"
var bar = 'abc'
func getBaz() string => 'baz'

func testFunc(baz string) string => '${bar}-${baz}-${getBaz()}'

func isStringEqual(input string) bool => input == testFunc(bar)

output outputBool bool = isStringEqual('abc-abc-baz')
output outputFoo string = testFunc('def')
");

        result.Should().NotHaveAnyDiagnostics();
        var evaluated = TemplateEvaluator.Evaluate(result.Template).ToJToken();

        evaluated.Should().HaveValueAtPath("$.outputs['outputBool'].value", true);
        evaluated.Should().HaveValueAtPath("$.outputs['outputFoo'].value", "abc-def-baz");
    }

    [TestMethod]
    public void Inlined_variables_in_user_defined_functions_are_not_allowed()
    {
        var result = CompilationHelper.Compile(@"

resource sa 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
  name: 'myaccount'
}

var saAccessTier = sa.properties.accessTier

func testFunc() string => '${saAccessTier}'

output outputFoo string = testFunc()
");

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
            ("BCP341", DiagnosticLevel.Error, """This expression is being used inside a function declaration, which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start ("saAccessTier" -> "sa"). Properties of sa which can be calculated at the start include "apiVersion", "id", "name", "type".""")
        });
    }

    [TestMethod]
    public void Resource_deploy_time_properties_are_allowed_in_user_defined_functions()
    {
        var result = CompilationHelper.Compile("""
            param storageAccountName string

            resource storage 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
              name: storageAccountName
            }
            resource storage2 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
              name: storageAccountName
            }

            var storageId = storage.id

            func getStorageId() string => storageId
            func getStorage2Id() string => storage2.id
            func getStorage2Name() string => storage2.name
            func getStorage2Type() string => storage2.type
            func getStorage2ApiVersion() string => storage2.apiVersion

            output s1 string = getStorageId()
            output s2 string = getStorage2Id()
            output s3 string = getStorage2Name()
            output s4 string = getStorage2Type()
            output s5 string = getStorage2ApiVersion()
            """);

        result.Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath(
            "$.functions[0].members['getStorage2Id'].output.value",
            "[resourceId('Microsoft.Storage/storageAccounts', parameters('storageAccountName'))]");
        result.Template.Should().HaveValueAtPath(
            "$.functions[0].members['getStorage2Name'].output.value",
            "[parameters('storageAccountName')]");
        result.Template.Should().HaveValueAtPath(
            "$.functions[0].members['getStorage2Type'].output.value",
            "Microsoft.Storage/storageAccounts");
        result.Template.Should().HaveValueAtPath(
            "$.functions[0].members['getStorage2ApiVersion'].output.value",
            "2022-09-01");
    }

    [TestMethod]
    public void Resource_runtime_properties_are_not_allowed_in_user_defined_functions()
    {
        var result = CompilationHelper.Compile("""
            resource storage 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
              name: 'mystorage'
            }

            func getAccessTier() string => storage.properties.accessTier
            """);

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
            ("BCP341", DiagnosticLevel.Error, """This expression is being used inside a function declaration, which requires a value that can be calculated at the start of the deployment. Properties of storage which can be calculated at the start include "apiVersion", "id", "name", "type".""")
        });
    }

    [TestMethod]
    public void Resource_references_are_not_allowed_in_user_defined_functions()
    {
        var result = CompilationHelper.Compile("""
            resource storage 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
              name: 'mystorage'
            }

            func getStorage() object => storage
            """);

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
            ("BCP341", DiagnosticLevel.Error, """This expression is being used inside a function declaration, which requires a value that can be calculated at the start of the deployment. Properties of storage which can be calculated at the start include "apiVersion", "id", "name", "type".""")
        });
    }

    [TestMethod]
    public void Functions_can_have_descriptions_applied()
    {
        var result = CompilationHelper.Compile(@"
@description('Returns foo')
func returnFoo() string => 'foo'

output outputFoo string = returnFoo()
");

        result.Should().NotHaveAnyDiagnostics();
        var evaluated = TemplateEvaluator.Evaluate(result.Template).ToJToken();

        evaluated.Should().HaveValueAtPath("$.outputs['outputFoo'].value", "foo");
    }

    [TestMethod]
    public void User_defined_functions_support_custom_types()
    {
        var result = CompilationHelper.Compile(@"
func getAOrB(aOrB ('a' | 'b')) bool => (aOrB == 'a')
");

        result.Should().NotHaveAnyDiagnostics();

        result = CompilationHelper.Compile(@"
func getAOrB(aOrB bool) ('a' | 'b') => aOrB ? 'a' : 'b'
");

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void User_defined_functions_unsupported_runtime_functions()
    {
        var result = CompilationHelper.Compile(@"
func useRuntimeFunction() string => reference('foo').bar
");

        result.Should().HaveDiagnostics(new[] {
            ("BCP341", DiagnosticLevel.Error, "This expression is being used inside a function declaration, which requires a value that can be calculated at the start of the deployment."),
        });
    }

    [TestMethod]
    public void Function_parameter_types_are_validated()
    {
        var result = CompilationHelper.Compile("""
            type environmentType = 'AzureCloud' | 'AzureChinaCloud' | 'AzureUSGovernment'

            @export()
            @description('Get the graph endpoint for the given environment')
            func getGraphEndpoint(environment environmentType | string) string =>
                {
                AzureCloud: 'https://graph.windows.net'
                AzureChinaCloud: 'https://graph.chinacloudapi.cn'
                AzureUSGovernment: 'https://graph.windows.net'
                }[environment]

            @export()
            @description('Get the Portal URL for the given environment')
            func getPortalUrl(environment environmentType | string) string =>
                {
                AzureCloud: 'https://portal.azure.com'
                AzureChinaCloud: 'https://portal.azure.cn'
                AzureUSGovernment: 'https://portal.azure.us'
                }[environment]
            """);

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP293", DiagnosticLevel.Error, "All members of a union type declaration must be literal values."),
            ("BCP293", DiagnosticLevel.Error, "All members of a union type declaration must be literal values."),
        });
    }

    [TestMethod]
    public void Function_return_types_are_validated()
    {
        var result = CompilationHelper.Compile("""
            func foo() 'bar' | 'baz' | string => 'bar'
            """);

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP293", DiagnosticLevel.Error, "All members of a union type declaration must be literal values."),
        });
    }
}
