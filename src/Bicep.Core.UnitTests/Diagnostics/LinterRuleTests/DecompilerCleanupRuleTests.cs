// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using Bicep.Core.Analyzers.Linter.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Bicep.Core.UnitTests.Diagnostics.LinterRuleTests.LinterRuleTestsBase;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class DecompilerCleanupRuleTests : LinterRuleTestsBase
    {
        [DataTestMethod]
        [DataRow(
            @"
            ",
            new string[]
            {
                // pass
            },
            DisplayName = "empty bicep")]
        [DataRow(
            @"
                resource applicationName_resource 'Microsoft.Solutions/applications@2019-07-01' = {
                  name: 'applicationName'
                  location: 'location'
                  kind: 'ServiceCatalog'
                }",
            new string[]
            {
                "applicationName_resource"
            },
            DisplayName = "applicationName_resource")]
        [DataRow(
            @"
                resource applicationName_Resource 'Microsoft.Solutions/applications@2019-07-01' = {
                  name: 'applicationName'
                  location: 'location'
                  kind: 'ServiceCatalog'
                }",
            new string[]
            {
                // pass (
            },
            DisplayName = "resource name case sensitive")]
        [DataRow(
            @"
                resource _resource 'Microsoft.Solutions/applications@2019-07-01' = {
                  name: 'applicationName'
                  location: 'location'
                  kind: 'ServiceCatalog'
                }",
            new string[]
            {
                // pass
            },
            DisplayName = "just _resource")]
        [DataRow(
            @"
                var applicationName_Resource = 'pass'
                param applicationName2_Resource string = 'pass'
                output applicationName3_Resource string = 'pass'
            ",
            new string[]
            {
                // pass
            },
            DisplayName = "_resource passes for non-resource names")]
        [DataRow(
            @"
                param location string

                resource namespace1_resource 'Microsoft.ServiceBus/namespaces@2018-01-01-preview' = {
                  name: 'namespace1'
                  location: location
                  properties: {
                  }
                }

                resource namespace1_queue1_resource 'Microsoft.ServiceBus/namespaces/queues@2017-04-01' = {
                  parent: namespace1_resource
                  name: 'queue1'
                }

                resource namespace1_queue1_rule1_resource 'Microsoft.ServiceBus/namespaces/queues/authorizationRules@2015-08-01' = {
                  parent: namespace1_queue1_resource
                  name: 'rule1'
                }

                resource namespace1_queue2_resource 'Microsoft.ServiceBus/namespaces/queues@2017-04-01' = {
                  name: 'namespace1/queue1'
                }

                resource namespace1_queue2_rule2_resource 'Microsoft.ServiceBus/namespaces/queues/authorizationRules@2018-01-01-preview' = {
                  parent: namespace1_queue2_resource
                  name: 'rule2'
                }

                resource namespace1_queue2_rule3_resource 'Microsoft.ServiceBus/namespaces/queues/authorizationRules@2017-04-01' = {
                  name: 'namespace1/queue2/rule3'
                }

                resource namespace2_resource 'Microsoft.ServiceBus/namespaces@2018-01-01-preview' = {
                  name: 'namespace2'
                  location: location

                  resource queue1_resource 'queues@2015-08-01' = {
                    name: 'queue1'
                    location: location

                    resource rule1_resource 'authorizationRules@2018-01-01-preview' = {
                      name: 'rule1'
                    }
                  }
                }

                resource namespace2_queue1_rule2_resource 'Microsoft.ServiceBus/namespaces/queues/authorizationRules@2017-04-01' = {
                  name: 'namespace2/queue1/rule2'
                }

                resource namespace2_queue1_rule3_resource 'Microsoft.ServiceBus/namespaces/queues/authorizationRules@2017-04-01' = {
                  parent: namespace2_resource::queue1_resource
                  name: 'rule3'
                }
            ",
            new string[]
            {
                "namespace1_resource",
                "namespace1_queue1_resource",
                "namespace1_queue1_rule1_resource",
                "namespace1_queue2_resource",
                "namespace1_queue2_rule2_resource",
                "namespace1_queue2_rule3_resource",
                "namespace2_resource",
                "queue1_resource",
                "rule1_resource",
                "namespace2_queue1_rule2_resource",
                "namespace2_queue1_rule3_resource",
            },
            DisplayName = "nested resources")]
        public void ResourceNameFluff(string bicep, string[] expectedFailingResourceNames)
        {
            AssertLinterRuleDiagnostics(
                DecompilerCleanupRule.Code,
                bicep,
                expectedFailingResourceNames.Select(name =>
                    $"The symbolic name of resource '{name}' appears to have originated from a naming conflict during a decompilation from JSON. Consider renaming it and removing the suffix (using the editor's rename functionality)."
                ).ToArray(),
                new() { IncludePosition = IncludePosition.None });
        }

        [DataTestMethod]
        [DataRow(
            @"
            ",
            new string[]
            {
                // pass
            },
            DisplayName = "empty bicep")]
        [DataRow(
            @"
                var hostpoolName = 'gene'
                var hostpoolName_var = replace(hostpoolName, '""', '')
                var appGroupName_var = '${hostpoolName_var}-DAG'

                resource appGroupName 'Microsoft.DesktopVirtualization/applicationgroups@2019-12-10-preview' = {
                  name: appGroupName_var
                  location: 'location'
                }
            ",
            new string[]
            {
                "hostpoolName_var",
                "appGroupName_var",
            },
            DisplayName = "hostpoolName_var, appGroupName_var")]
        [DataRow(
            @"
                var hostpoolName = 'Olympic'
                var hostpoolName_Var = replace(hostpoolName, '""', '')
            ",
            new string[]
            {
                // pass
            },
            DisplayName = "case sensitive")]
        [DataRow(
            @"
                var hostpoolName = 'billiards'
                var hostpoolName_variable = replace(hostpoolName, '""', '')
            ",
            new string[]
            {
                // pass
            },
            DisplayName = "doesn't end with _var")]
        [DataRow(
            @"
                var hostpoolName = 'car'
                var _var = replace(hostpoolName, '""', '')
            ",
            new string[]
            {
                // pass
            },
            DisplayName = "just _var")]
        [DataRow(
            @"
                param hostpoolName_var string = 'pass'
                output hostpoolName2_var string = 'pass'
                resource hostpoolName3_var 'Microsoft.DesktopVirtualization/applicationgroups@2019-12-10-preview' = {
                  name: 'name'
                  location: 'location'
                }
            ",
            new string[]
            {
                // pass
            },
            DisplayName = "_var passes for non-variable names")]
        public void VariableNameFluff(string bicep, string[] expectedFailingResourceNames)
        {
            AssertLinterRuleDiagnostics(
                DecompilerCleanupRule.Code,
                bicep,
                expectedFailingResourceNames.Select(name =>
                    $"The name of variable '{name}' appears to have originated from a naming conflict during a decompilation from JSON. Consider renaming it and removing the suffix (using the editor's rename functionality)."
                ).ToArray(),
                new() { IncludePosition = IncludePosition.None });
        }

        [DataTestMethod]
        [DataRow(
            @"
            ",
            new string[]
            {
                // pass
            },
            DisplayName = "empty bicep")]
        [DataRow(
            @"
                param hostpoolName string = 'gene'
                var hostpoolName_var = replace(hostpoolName, '""', '')
                var appGroupName_var = '${hostpoolName_var}-DAG'

                resource appGroupName 'Microsoft.DesktopVirtualization/applicationgroups@2019-12-10-preview' = {
                  name: appGroupName_var
                  location: 'location'
                }
            ",
            new string[]
            {
                "hostpoolName_var",
                "appGroupName_var",
            },
            DisplayName = "hostpoolName_var, appGroupName_var")]
        [DataRow(
            @"
                var hostpoolName = 'Olympic'
                var hostpoolName_Var = replace(hostpoolName, '""', '')
            ",
            new string[]
            {
                // pass
            },
            DisplayName = "case sensitive")]
        [DataRow(
            @"
                var hostpoolName = 'billiards'
                var hostpoolName_variable = replace(hostpoolName, '""', '')
            ",
            new string[]
            {
                // pass
            },
            DisplayName = "doesn't end with _var")]
        [DataRow(
            @"
                var hostpoolName = 'car'
                var _var = replace(hostpoolName, '""', '')
            ",
            new string[]
            {
                // pass
            },
            DisplayName = "just _var")]
        [DataRow(
            @"
                param hostpoolName_var string = 'pass'
                output hostpoolName2_var string = 'pass'
                resource hostpoolName3_var 'Microsoft.DesktopVirtualization/applicationgroups@2019-12-10-preview' = {
                  name: 'name'
                  location: 'location'
                }
            ",
            new string[]
            {
                // pass
            },
            DisplayName = "_var passes for non-variable names")]
        public void ParameterNameFluff(string bicep, string[] expectedFailingResourceNames)
        {
            AssertLinterRuleDiagnostics(
                DecompilerCleanupRule.Code,
                bicep,
                expectedFailingResourceNames.Select(name =>
                    $"The name of variable '{name}' appears to have originated from a naming conflict during a decompilation from JSON. Consider renaming it and removing the suffix (using the editor's rename functionality)."
                ).ToArray(),
                new() { IncludePosition = IncludePosition.None });
        }
    }
}

