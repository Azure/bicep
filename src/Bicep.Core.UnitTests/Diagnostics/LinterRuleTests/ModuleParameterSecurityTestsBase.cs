// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using static Bicep.Core.UnitTests.Utils.CompilationHelper;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

// Shared by the two rules that check values passed to module parameters. Both rules run on every
// compilation, so a test for one rule also shows what the other rule reports for the same module.
public abstract class ModuleParameterSecurityTestsBase : LinterRuleTestsBase
{
    // sql/server forwards the administrator password into a key vault secrets export. The real expression has a
    // second conditional element for a plain connection string, which is left out so the password stands alone.
    // Used by both rules' tests, so the `SECURE` marker is left for the caller.
    protected const string SqlSecretsExport = """
        SECURE
        param administratorLoginPassword string
        param secretsExportConfiguration {
          sqlAdminPasswordSecretName: string?
        }
        module export 'child.bicep' = {
          name: 'export'
          params: {
            secretsToSet: union(
              [],
              contains(secretsExportConfiguration, 'sqlAdminPasswordSecretName')
                ? [
                    {
                      name: secretsExportConfiguration.?sqlAdminPasswordSecretName
                      value: administratorLoginPassword
                    }
                  ]
                : []
            )
          }
        }
        """;

    protected static void AssertFlow(string parent, string child, int inputWarnings, int leakWarnings, string childPath = "child.bicep")
    {
        var result = CompileWithoutErrors(parent, child, childPath);
        GetWarnings(result, UseSecureValueForSecureInputsRule.Code).Should().HaveCount(inputWarnings);
        GetWarnings(result, NoSecureValuesInInsecureInputsRule.Code).Should().HaveCount(leakWarnings);
    }

    protected static void AssertFlow(string parent, string child, string[] inputWarnings, string[] leakWarnings, string childPath = "child.bicep")
    {
        var result = CompileWithoutErrors(parent, child, childPath);
        GetWarnings(result, UseSecureValueForSecureInputsRule.Code).Select(diagnostic => diagnostic.Message).Should().BeEquivalentTo(inputWarnings);
        GetWarnings(result, NoSecureValuesInInsecureInputsRule.Code).Select(diagnostic => diagnostic.Message).Should().BeEquivalentTo(leakWarnings);
    }

    // Replaces the `SECURE` marker in the parent with nothing, then with `@secure()`, to check both directions
    // of hardening. A secure child warns while the parent stays plain, and the warning goes away once the parent
    // is secure. The same secure parent then leaks into a child that has lost `@secure()`, once for each of the
    // `leakSources` that reaches it. The leak is reported at `path` unless the plain child resolves to a different
    // path, and `plainChild` replaces the default of dropping every `@secure()` from the child for children that
    // don't use the decorator, like compiled JSON.
    protected static void AssertHardening(string parent, string child, string path, string[] leakSources, string childPath = "child.bicep", string? plainChild = null, string? leakPath = null)
    {
        AssertFlow(parent.Replace("SECURE", ""), child, [InputWarning(path)], [], childPath);
        AssertFlow(parent.Replace("SECURE", "@secure()"), child, [], [], childPath);
        AssertFlow(parent.Replace("SECURE", "@secure()"), plainChild ?? child.Replace("@secure()", ""), [],
            [.. leakSources.Select(source => LeakWarning(leakPath ?? path, source))], childPath);
    }

    protected static string InputWarning(string input)
        => $"Module parameter '{input}' expects a secure value, but the value provided may not be secure.";

    protected static string LeakWarning(string input, string source)
        => $"Module parameter '{input}' is not secure, but is assigned secure value '{source}'. The value will be visible in the module deployment's inputs.";

    protected static CompilationResult CompileModule(string parent, string child, string childPath = "child.bicep")
        => CompilationHelper.Compile(("main.bicep", parent), (childPath, child));

    private static CompilationResult CompileWithoutErrors(string parent, string child, string childPath)
    {
        var result = CompileModule(parent, child, childPath);
        result.Diagnostics.Where(diagnostic => diagnostic.Level == DiagnosticLevel.Error).Should().BeEmpty();

        return result;
    }

    private static IDiagnostic[] GetWarnings(CompilationResult result, string code)
    {
        var diagnostics = result.Diagnostics.Where(diagnostic => diagnostic.Code == code).ToArray();
        diagnostics.Where(diagnostic => diagnostic.Level != DiagnosticLevel.Warning).Should().BeEmpty();

        return diagnostics;
    }
}
