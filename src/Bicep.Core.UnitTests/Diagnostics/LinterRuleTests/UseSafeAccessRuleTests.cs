// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class UseSafeAccessRuleTests : LinterRuleTestsBase
{
    private void AssertCodeFix(string inputFile, string resultFile, CompilationHelper.InputFile[]? supportingFiles = null)
        => AssertCodeFix(UseSafeAccessRule.Code, "Use the safe access (.?) operator", inputFile, resultFile, supportingFiles);

    private void AssertNoDiagnostics(string inputFile)
        => AssertLinterRuleDiagnostics(UseSafeAccessRule.Code, inputFile, [], new(OnCompileErrors.Ignore, IncludePosition.None));

    [TestMethod]
    public void Codefix_fixes_syntax_which_can_be_simplified() => AssertCodeFix("""
param foo object
var test = contai|ns(foo, 'bar') ? foo.bar : 'baz'
""", """
param foo object
var test = foo.?bar ?? 'baz'
""");

    [TestMethod]
    public void Codefix_fixes_syntax_which_can_be_simplified_array_access() => AssertCodeFix("""
param foo object
param target string
var test = contai|ns(foo, target) ? foo[target] : 'baz'
""", """
param foo object
param target string
var test = foo[?target] ?? 'baz'
""");

    [TestMethod]
    public void Codefix_fixes_syntax_which_can_be_simplified_named_array_access() => AssertCodeFix("""
param foo object
var test = contai|ns(foo, 'bar') ? foo['bar'] : 'baz'
""", """
param foo object
var test = foo.?bar ?? 'baz'
""");

    [TestMethod]
    public void Codefix_escapes_special_chars_correctly() => AssertCodeFix("""
param foo object
var test = contai|ns(foo, 'oh-dear') ? foo['oh-dear'] : 'baz'
""", """
param foo object
var test = foo[?'oh-dear'] ?? 'baz'
""");

    [TestMethod]
    public void Codefix_is_aware_of_operator_precedence() => AssertCodeFix("""
param foo object
var test = contai|ns(foo, 'bar') ? foo.bar : contains(foo, 'baz') ? foo.baz : 'qux'
""", """
param foo object
var test = foo.?bar ?? (contains(foo, 'baz') ? foo.baz : 'qux')
""");

    [TestMethod]
    public void Codefix_is_aware_of_operator_precedence_when_not_needed() => AssertCodeFix("""
param foo object
var test = contai|ns(foo, 'bar') ? foo.bar : 1 + 2
""", """
param foo object
var test = foo.?bar ?? 1 + 2
""");

    [TestMethod]
    public void Rule_ignores_syntax_which_cannot_be_simplified() => AssertNoDiagnostics("""
param foo object
var test = contains(foo, 'bar') ? foo.baz : 'baz'
""");

    [TestMethod]
    public void Rule_ignores_syntax_which_cannot_be_simplified_2() => AssertNoDiagnostics("""
param foo object
param target string
param notTarget string
var test = contains(foo, target) ? bar[notTarget] : 'baz'
""");

    [TestMethod]
    public void Rule_ignores_syntax_which_cannot_be_simplified_array_access() => AssertNoDiagnostics("""
param foo object
var test = contains(foo, 'bar') ? bar.bar : 'baz'
""");

    [TestMethod]
    // https://github.com/Azure/bicep/issues/14705
    public void Codefix_handles_issue14705_correctly() => AssertCodeFix("""
@description('Optional. Array of role assignments to create.')
param roleAssignments roleAssignmentType

var builtInRoleNames = {
  Contributor: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'b24988ac-6180-42a0-ab88-20f7382dd24c')
  Owner: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '8e3af657-a8ff-443c-a75c-2fe8c4bcb635')
  Reader: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'acdd72a7-3385-48ef-bd42-f606fba81ae7')
}

output formattedRoles array = [
  for (roleAssignment, index) in (roleAssignments ?? []): union(roleAssignment, {
    roleDefinitionId: conta|ins(builtInRoleNames, roleAssignment.roleDefinitionIdOrName)
      ? builtInRoleNames[roleAssignment.roleDefinitionIdOrName]
      : contains(roleAssignment.roleDefinitionIdOrName, '/providers/Microsoft.Authorization/roleDefinitions/')
          ? roleAssignment.roleDefinitionIdOrName
          : subscriptionResourceId('Microsoft.Authorization/roleDefinitions', roleAssignment.roleDefinitionIdOrName)
  })
]

// ================ //
// Definitions      //
// ================ //

type roleAssignmentType = {
  @description('Required. The role to assign. You can provide either the display name of the role definition, the role definition GUID, or its fully qualified ID in the following format: \'/providers/Microsoft.Authorization/roleDefinitions/c2f4ef07-c644-48eb-af81-4b1b4947fb11\'.')
  roleDefinitionIdOrName: string
}[]?
""", """
@description('Optional. Array of role assignments to create.')
param roleAssignments roleAssignmentType

var builtInRoleNames = {
  Contributor: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'b24988ac-6180-42a0-ab88-20f7382dd24c')
  Owner: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '8e3af657-a8ff-443c-a75c-2fe8c4bcb635')
  Reader: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'acdd72a7-3385-48ef-bd42-f606fba81ae7')
}

output formattedRoles array = [
  for (roleAssignment, index) in (roleAssignments ?? []): union(roleAssignment, {
    roleDefinitionId: builtInRoleNames[?roleAssignment.roleDefinitionIdOrName] ?? (contains(roleAssignment.roleDefinitionIdOrName, '/providers/Microsoft.Authorization/roleDefinitions/')
          ? roleAssignment.roleDefinitionIdOrName
          : subscriptionResourceId('Microsoft.Authorization/roleDefinitions', roleAssignment.roleDefinitionIdOrName))
  })
]

// ================ //
// Definitions      //
// ================ //

type roleAssignmentType = {
  @description('Required. The role to assign. You can provide either the display name of the role definition, the role definition GUID, or its fully qualified ID in the following format: \'/providers/Microsoft.Authorization/roleDefinitions/c2f4ef07-c644-48eb-af81-4b1b4947fb11\'.')
  roleDefinitionIdOrName: string
}[]?
""");

    [TestMethod]
    public void Codefix_recommends_safe_access_for_nullable_property_access() => AssertCodeFix("""
param foo {
  bar: string?
}

output bar string? = foo.b|ar
""", """
param foo {
  bar: string?
}

output bar string? = foo.?bar
""");

    [TestMethod]
    public void Rule_ignores_non_nullable_property_access() => AssertNoDiagnostics("""
param foo {
  bar: string
}

output bar string? = foo.bar
""");

    [TestMethod]
    public void Codefix_recommends_safe_access_for_nullable_module_output_property_access() => AssertCodeFix("""
module foo 'module.bicep' = {
  name: 'foo'
}

output bar string? = foo.outputs.b|ar
""", """
module foo 'module.bicep' = {
  name: 'foo'
}

output bar string? = foo.outputs.?bar
""",
supportingFiles: [new("module.bicep", """
output bar string? = null
""")]);

    [TestMethod]
    public void Codefix_recommends_safe_access_for_nullable_array_access() => AssertCodeFix("""
param foo {
  bar: string?
}

output bar string? = foo['b|ar']
""", """
param foo {
  bar: string?
}

output bar string? = foo[?'bar']
""");

    [TestMethod]
    public void Rule_ignores_non_nullable_array_access() => AssertNoDiagnostics("""
param foo {
  bar: string
}

output bar string? = foo['bar']
""");

    [TestMethod]
    public void Rule_ignores_array_access_on_array() => AssertNoDiagnostics("""
param foo (string | null)[]

output bar string? = foo[0]
""");

    [TestMethod]
    public void Codefix_recommends_safe_access_for_nullable_module_output_array_access() => AssertCodeFix("""
module foo 'module.bicep' = {
  name: 'foo'
}

output bar string? = foo.outputs['b|ar']
""", """
module foo 'module.bicep' = {
  name: 'foo'
}

output bar string? = foo.outputs[?'bar']
""",
supportingFiles: [new("module.bicep", """
output bar string? = null
""")]);

    [TestMethod]
    public void Rule_ignores_access_with_null_forgiving_operator() => AssertNoDiagnostics("""
        param foo {
          optionalProperty: string?
        }

        output bar string = foo.optionalProperty!
        """);
}
