// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.IntegrationTests.Extensibility;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class ParameterUsingWithTests
{
    private static readonly ServiceBuilder Services = new ServiceBuilder()
        .WithFeatureOverrides(new(DeployCommandsEnabled: true));

    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public void Feature_is_blocked_unless_experimental_feature_enabled()
    {
        var result = CompilationHelper.CompileParams(
            ("parameters.bicepparam", """
var subscriptionId = externalInput('sys.cliArg', 'subscription-id')
var resourceGroup = externalInput('sys.cliArg', 'resource-group')

using 'main.bicep' with {
  mode: 'deployment'
  scope: '/subscriptions/${subscriptionId}/resourceGroups/${resourceGroup}'
}

param foo = 'foo/bar/baz'
"""),
            ("main.bicep", """
param foo string
"""));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics([
            ("BCP435", DiagnosticLevel.Error, """Using the "with" keyword with a "using" statement requires enabling EXPERIMENTAL feature "DeployCommands"."""),
        ]);
    }

    [TestMethod]
    public void Using_with_clause_required_if_experimental_feature_enabled()
    {
        // https://github.com/Azure/bicep/issues/18328
        var result = CompilationHelper.CompileParams(Services,
            ("parameters.bicepparam", """
using 'main.bicep'

param foo = 'foo/bar/baz'
"""),
            ("main.bicep", """
param foo string
"""));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics([
            ("BCP443", DiagnosticLevel.Error, """The "using" statement requires a "with" clause if the EXPERIMENTAL feature "DeployCommands" is enabled."""),
        ]);
    }

    [TestMethod]
    public void Scope_information_and_deployment_can_be_parsed()
    {
        var result = CompilationHelper.CompileParams(Services,
            ("parameters.bicepparam", """
var subscriptionId = externalInput('sys.cliArg', 'subscription-id')
var resourceGroup = externalInput('sys.cliArg', 'resource-group')

using 'main.bicep' with {
  mode: 'deployment'
  scope: '/subscriptions/${subscriptionId}/resourceGroups/${resourceGroup}'
}

param foo = 'foo/bar/baz'
"""),
            ("main.bicep", """
param foo string
"""));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Scope_information_and_stack_can_be_parsed()
    {
        var result = CompilationHelper.CompileParams(Services,
            ("parameters.bicepparam", """
                var subscriptionId = readEnvVar('SUBSCRIPTION_ID')
                var resourceGroup = readEnvVar('RESOURCE_GROUP')
    
                using 'main.bicep' with {
                  name: 'asdf9uasd9'
                  mode: 'stack'
                  scope: '/subscriptions/${subscriptionId}/resourceGroups/${resourceGroup}'
                  description: 'my stack'
                  actionOnUnmanage: {
                    resources: 'delete'
                  }
                  denySettings: {
                    mode: 'denyDelete'
                  }
                }
    
                param foo = 'bar'
                """),
            ("main.bicep", """
                param foo string
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Using_with_mode_type_checking_works()
    {
        var result = CompilationHelper.CompileParams(Services,
            ("parameters.bicepparam", """
var subscriptionId = externalInput('sys.cliArg', 'subscription-id')
var resourceGroup = externalInput('sys.cliArg', 'resource-group')

using 'main.bicep' with {
  mode: 'steven'
  scope: '/subscriptions/${subscriptionId}/resourceGroups/${resourceGroup}'
}

param foo = 'foo/bar/baz'
"""),
            ("main.bicep", """
param foo string
"""));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics([
            ("BCP036", DiagnosticLevel.Error, """The property "mode" expected a value of type "'deployment' | 'stack'" but the provided value is of type "'steven'"."""),
        ]);
    }

    [TestMethod]
    public void Using_without_mode_raises_error()
    {
        var result = CompilationHelper.CompileParams(Services,
            ("parameters.bicepparam", """
var subscriptionId = externalInput('sys.cliArg', 'subscription-id')
var resourceGroup = externalInput('sys.cliArg', 'resource-group')

using 'main.bicep' with {
  scope: '/subscriptions/${subscriptionId}/resourceGroups/${resourceGroup}'
}
"""),
            ("main.bicep", """

"""));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics([
            ("BCP078", DiagnosticLevel.Error, """The property "mode" requires a value of type "'deployment' | 'stack'", but none was supplied."""),
        ]);
    }

    [TestMethod]
    public void Using_with_deployment_properties_types_are_checked()
    {
        var result = CompilationHelper.CompileParams(Services,
            ("parameters.bicepparam", """
var subscriptionId = externalInput('sys.cliArg', 'subscription-id')
var resourceGroup = externalInput('sys.cliArg', 'resource-group')

using 'main.bicep' with {
  mode: 'deployment'
  scope2: '/subscriptions/${subscriptionId}/resourceGroups/${resourceGroup}'
}

param foo = 'foo/bar/baz'
"""),
            ("main.bicep", """
param foo string
"""));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics([
            ("BCP035", DiagnosticLevel.Error, """The specified "object" declaration is missing the following required properties: "scope"."""),
            ("BCP089", DiagnosticLevel.Error, """The property "scope2" is not allowed on objects of type "DeploymentConfig". Did you mean "scope"?"""),
        ]);
    }

    [TestMethod]
    public void Using_with_stacks_properties_types_are_checked()
    {
        var result = CompilationHelper.CompileParams(Services,
            ("parameters.bicepparam", """
var subscriptionId = externalInput('sys.cliArg', 'subscription-id')
var resourceGroup = externalInput('sys.cliArg', 'resource-group')

using 'main.bicep' with {
  mode: 'stack'
  scope2: '/subscriptions/${subscriptionId}/resourceGroups/${resourceGroup}'
}

param foo = 'foo/bar/baz'
"""),
            ("main.bicep", """
param foo string
"""));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics([
            ("BCP035", DiagnosticLevel.Error, """The specified "object" declaration is missing the following required properties: "actionOnUnmanage", "denySettings", "scope"."""),
            ("BCP089", DiagnosticLevel.Error, """The property "scope2" is not allowed on objects of type "StackConfig". Did you mean "scope"?"""),
        ]);
    }

    [TestMethod]
    public void ReadEnvironmentVariable_function_is_blocked()
    {
        var result = CompilationHelper.CompileParams(Services,
            ("parameters.bicepparam", """
var subscriptionId = readEnvironmentVariable('SUBSCRIPTION_ID')
var resourceGroup = readEnvironmentVariable('RESOURCE_GROUP')

using 'main.bicep' with {
  mode: 'deployment'
  scope: '/subscriptions/${subscriptionId}/resourceGroups/${resourceGroup}'
}

param foo = 'foo/bar/baz'
"""),
            ("main.bicep", """
param foo string
"""));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics([
            ("BCP057", DiagnosticLevel.Error, """The name "readEnvironmentVariable" does not exist in the current context."""),
            ("BCP057", DiagnosticLevel.Error, """The name "readEnvironmentVariable" does not exist in the current context."""),
            ("BCP062", DiagnosticLevel.Error, """The referenced declaration with name "subscriptionId" is not valid."""),
            ("BCP062", DiagnosticLevel.Error, """The referenced declaration with name "resourceGroup" is not valid."""),
        ]);
    }
}
