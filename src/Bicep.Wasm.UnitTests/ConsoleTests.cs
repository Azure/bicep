// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace Bicep.Wasm.UnitTests;

[TestClass]
public class ConsoleTests
{
    [TestMethod]
    [DataRow("toUpper('hello')", "\"HELLO\"")]
    [DataRow("true && false", "false")]
    [DataRow("{\n  name: 'Bicep'\n  count: 3\n}", "{\"name\":\"Bicep\",\"count\":3}")]
    [DataRow("'hello \U0001F30D'", "\"hello \U0001F30D\"")]
    [DataRow("\n// comment\n1 + 1\n\n", "2")]
    public void Console_returns_json_values(string input, string expected)
    {
        using var services = new ServiceCollection().AddBicepWasm().BuildServiceProvider();
        var interop = new Interop(_ => Task.FromResult<string?>(null), services);
        var result = interop.EvaluateConsole(input, interop.ResetConsole());
        result.Diagnostics.Should().BeEmpty();
        JToken.DeepEquals(JToken.Parse(result.Output!), JToken.Parse(expected)).Should().BeTrue();
    }

    [TestMethod]
    [DataRow("""
        getRoleAssignmentName({
          scope: '/subscriptions/11111111-1111-1111-1111-111111111111/resourceGroups/myRg'
          principalId: '22222222-2222-2222-2222-222222222222'
          roleDefinitionId: '/providers/Microsoft.Authorization/roleDefinitions/33333333-3333-3333-3333-333333333333'
        })
        """, "eec47c62-a9a9-54dd-b8d8-f9e2839fab35")]
    [DataRow("subscriptionResourceId('00000000-0000-0000-0000-000000000001', 'Microsoft.Authorization/roleDefinitions', 'reader')",
        "/subscriptions/00000000-0000-0000-0000-000000000001/providers/Microsoft.Authorization/roleDefinitions/reader")]
    [DataRow("az.subscriptionResourceId('00000000-0000-0000-0000-000000000001', 'Microsoft.Authorization/roleDefinitions', 'reader')",
        "/subscriptions/00000000-0000-0000-0000-000000000001/providers/Microsoft.Authorization/roleDefinitions/reader")]
    [DataRow("resourceId('00000000-0000-0000-0000-000000000001', 'my-rg', 'Microsoft.Storage/storageAccounts', 'storage')",
        "/subscriptions/00000000-0000-0000-0000-000000000001/resourceGroups/my-rg/providers/Microsoft.Storage/storageAccounts/storage")]
    [DataRow("resourceId('00000000-0000-0000-0000-000000000001', 'my-rg', 'Microsoft.Network/virtualNetworks/subnets', 'vnet', 'subnet')",
        "/subscriptions/00000000-0000-0000-0000-000000000001/resourceGroups/my-rg/providers/Microsoft.Network/virtualNetworks/vnet/subnets/subnet")]
    [DataRow("tenantResourceId('Microsoft.Management/managementGroups', 'my-mg')",
        "/providers/Microsoft.Management/managementGroups/my-mg")]
    [DataRow("managementGroupResourceId('my-mg', 'Microsoft.Authorization/policyDefinitions', 'policy')",
        "/providers/Microsoft.Management/managementGroups/my-mg/providers/Microsoft.Authorization/policyDefinitions/policy")]
    [DataRow("extensionResourceId('/subscriptions/00000000-0000-0000-0000-000000000001', 'Microsoft.Authorization/roleAssignments', 'assignment')",
        "/subscriptions/00000000-0000-0000-0000-000000000001/providers/Microsoft.Authorization/roleAssignments/assignment")]
    [DataRow("extensionResourceId(tenantResourceId('Microsoft.Management/managementGroups', 'my-mg'), 'Microsoft.Authorization/policyDefinitions', 'policy')",
        "/providers/Microsoft.Management/managementGroups/my-mg/providers/Microsoft.Authorization/policyDefinitions/policy")]
    public void Console_pure_az_namespace_functions_succeed(string input, string expected)
    {
        using var services = new ServiceCollection().AddBicepWasm().BuildServiceProvider();
        var interop = new Interop(_ => Task.FromResult<string?>(null), services);
        var result = interop.EvaluateConsole(input, interop.ResetConsole());

        result.Diagnostics.Should().BeEmpty();
        JToken.Parse(result.Output!).Value<string>().Should().Be(expected);
    }

    [TestMethod]
    public void Console_evaluates_resource_ids_using_declarations_and_expressions()
    {
        using var services = new ServiceCollection().AddBicepWasm().BuildServiceProvider();
        var interop = new Interop(_ => Task.FromResult<string?>(null), services);
        var session = interop.ResetConsole();

        interop.EvaluateConsole("var subscriptionId = '00000000-0000-0000-0000-000000000001'", session).Diagnostics.Should().BeEmpty();
        interop.EvaluateConsole("var roleId = subscriptionResourceId(subscriptionId, 'Microsoft.Authorization/roleDefinitions', toLower('READER'))", session).Diagnostics.Should().BeEmpty();
        interop.EvaluateConsole("roleId", session).Output.Should().Be(
            "\"/subscriptions/00000000-0000-0000-0000-000000000001/providers/Microsoft.Authorization/roleDefinitions/reader\"");
        interop.EvaluateConsole("func policyId(name string) string => tenantResourceId('Microsoft.Authorization/policyDefinitions', name)", session).Diagnostics.Should().BeEmpty();
        interop.EvaluateConsole("policyId('policy')", session).Output.Should().Be(
            "\"/providers/Microsoft.Authorization/policyDefinitions/policy\"");
        interop.EvaluateConsole("func scopedRoleId(sub string, role string) string => subscriptionResourceId(sub, 'Microsoft.Authorization/roleDefinitions', role)", session).Diagnostics.Should().BeEmpty();
        interop.EvaluateConsole("scopedRoleId(subscriptionId, 'reader')", session).Output.Should().Be(
            "\"/subscriptions/00000000-0000-0000-0000-000000000001/providers/Microsoft.Authorization/roleDefinitions/reader\"");
    }

    [TestMethod]
    [DataRow("subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'reader')")]
    [DataRow("subscriptionResourceId('Microsoft.Test/parents/children', 'parent', 'child')")]
    [DataRow("resourceId('my-rg', 'Microsoft.Storage/storageAccounts', 'storage')")]
    [DataRow("managementGroupResourceId('Microsoft.Authorization/policyDefinitions', 'policy')")]
    [DataRow("reference('/subscriptions/sub/resourceGroups/rg/providers/Microsoft.Storage/storageAccounts/storage', '2023-05-01')")]
    [DataRow("listKeys('/subscriptions/sub/resourceGroups/rg/providers/Microsoft.Storage/storageAccounts/storage', '2023-05-01')")]
    [DataRow("subscription()")]
    [DataRow("resourceGroup()")]
    [DataRow("deployment()")]
    [DataRow("environment()")]
    [DataRow("tenantResourceId('Microsoft.Test/parents/children', 'parent')")]
    [DataRow("resourceId('00000000-0000-0000-0000-000000000001', 'my-rg', 'Microsoft.Network/virtualNetworks/subnets', 'vnet/subnet')")]
    [DataRow("subscriptionResourceId(123, 'Microsoft.Authorization/roleDefinitions', 'reader')")]
    public void Console_non_pure_az_namespace_functions_or_invalid_arguments_error(string input)
    {
        using var services = new ServiceCollection().AddBicepWasm().BuildServiceProvider();
        var interop = new Interop(_ => Task.FromResult<string?>(null), services);
        var session = interop.ResetConsole();
        var result = interop.EvaluateConsole(input, session);

        result.Output.Should().BeNull();
        result.Diagnostics.Should().Contain(d => d.Level == "Error");
        interop.EvaluateConsole("1 + 1", session).Output.Should().Be("2");
    }

    [TestMethod]
    [DataRow("var broken =")]
    [DataRow("unknownName")]
    [DataRow("toUpper(123)")]
    [DataRow("'unterminated")]
    public void Console_reports_invalid_input_and_recovers(string input)
    {
        using var services = new ServiceCollection().AddBicepWasm().BuildServiceProvider();
        var interop = new Interop(_ => Task.FromResult<string?>(null), services);
        var session = interop.ResetConsole();
        var result = interop.EvaluateConsole(input, session);
        result.Output.Should().BeNull();
        result.Diagnostics.Should().Contain(d => d.Level == "Error");
        interop.EvaluateConsole("1 + 1", session).Output.Should().Be("2");
    }

    [TestMethod]
    public void Console_evaluates_expressions_and_retains_declarations()
    {
        using var services = new ServiceCollection().AddBicepWasm().BuildServiceProvider();
        var interop = new Interop(_ => Task.FromResult<string?>(null), services);
        var session = interop.ResetConsole();

        interop.EvaluateConsole("var greeting = 'hello'", session).Diagnostics.Should().BeEmpty();
        interop.EvaluateConsole("toUpper(greeting)", session).Output.Should().Be("\"HELLO\"");
        interop.EvaluateConsole("type Person = { name: string }", session).Diagnostics.Should().BeEmpty();
        interop.EvaluateConsole("func greet(person Person) string => 'Hello ${person.name}'", session).Diagnostics.Should().BeEmpty();
        interop.EvaluateConsole("greet({ name: 'Bicep' })", session).Output.Should().Be("\"Hello Bicep\"");

        var array = interop.EvaluateConsole("[for i in range(0, 3): i * 2]", session);
        JToken.Parse(array.Output!).Should().BeEquivalentTo(JArray.Parse("[0, 2, 4]"));
        interop.EvaluateConsole("null", session).Output.Should().Be("null");
    }

    [TestMethod]
    public void Console_preserves_last_valid_declaration_after_errors()
    {
        using var services = new ServiceCollection().AddBicepWasm().BuildServiceProvider();
        var interop = new Interop(_ => Task.FromResult<string?>(null), services);
        var session = interop.ResetConsole();

        interop.EvaluateConsole("var count = 1", session);
        interop.EvaluateConsole("var count = unknown", session).Diagnostics.Should().Contain(d => d.Level == "Error");
        interop.EvaluateConsole("count", session).Output.Should().Be("1");
        interop.EvaluateConsole("var count = 2", session).Diagnostics.Should().BeEmpty();
        interop.EvaluateConsole("count", session).Output.Should().Be("2");
        interop.EvaluateConsole("1 / 0", session).Diagnostics.Should().Contain(d => d.Level == "Error");
        interop.EvaluateConsole("1 + 1", session).Output.Should().Be("2");
    }

    [TestMethod]
    public async Task Console_is_independent_of_editor_and_reset_invalidates_old_session()
    {
        using var services = new ServiceCollection().AddBicepWasm().BuildServiceProvider();
        var interop = new Interop(_ => Task.FromResult<string?>(null), services);
        var session = interop.ResetConsole();
        interop.EvaluateConsole("var count = 7", session);

        await interop.CompileAndEmitDiagnostics("var count = 99\noutput result int = count", null);
        interop.EvaluateConsole("count", session).Output.Should().Be("7");
        var newSession = interop.ResetConsole();
        interop.Invoking(i => i.EvaluateConsole("count", session)).Should()
            .Throw<InvalidOperationException>().WithMessage("*Reset the console*");
        interop.EvaluateConsole("count", newSession).Diagnostics.Should().Contain(d => d.Level == "Error");
    }

    [TestMethod]
    public void Console_reports_lost_sessions_and_rejects_multiple_commands_without_changing_state()
    {
        using var services = new ServiceCollection().AddBicepWasm().BuildServiceProvider();
        var interop = new Interop(_ => Task.FromResult<string?>(null), services);
        interop.Invoking(i => i.EvaluateConsole("1", "missing")).Should()
            .Throw<InvalidOperationException>().WithMessage("*Reset the console*");
        var session = interop.ResetConsole();
        interop.Invoking(i => i.EvaluateConsole("var first = 1\nvar second = 2", session)).Should()
            .Throw<InvalidOperationException>().WithMessage("*one expression or declaration*");
        interop.EvaluateConsole("first", session).Diagnostics.Should().Contain(d => d.Level == "Error");
        interop.EvaluateConsole("second", session).Diagnostics.Should().Contain(d => d.Level == "Error");
    }
}
