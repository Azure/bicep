// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.UnitTests.Assertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Bicep.Core.UnitTests.Utils;
using Newtonsoft.Json.Linq;
using FluentAssertions.Execution;
using Bicep.Core.Diagnostics;
using Bicep.Core.TypeSystem;
using Bicep.Core.Resources;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ScopeTests
    {
        private const string ExpectedTenantSchema = "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#";
        private const string ExpectedMgSchema = "https://schema.management.azure.com/schemas/2019-08-01/managementGroupDeploymentTemplate.json#";
        private const string ExpectedSubSchema = "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#";
        private const string ExpectedRgSchema = "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#";

        [DataRow("tenant", "tenant()", "tenant", ExpectedTenantSchema, "[reference(tenantResourceId('Microsoft.Resources/deployments', 'myMod'), '2020-06-01').outputs.hello.value]", "[tenantResourceId('Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("tenant", "managementGroup('abc')", "managementGroup", ExpectedTenantSchema, "[reference(extensionResourceId(tenantResourceId('Microsoft.Management/managementGroups', 'abc'), 'Microsoft.Resources/deployments', 'myMod'), '2020-06-01').outputs.hello.value]", "[extensionResourceId(tenantResourceId('Microsoft.Management/managementGroups', 'abc'), 'Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("tenant", "subscription('abc')", "subscription", ExpectedTenantSchema, "[reference(subscriptionResourceId('abc', 'Microsoft.Resources/deployments', 'myMod'), '2020-06-01').outputs.hello.value]", "[subscriptionResourceId('abc', 'Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("tenant", "resourceGroup('abc', 'def')", "resourceGroup", ExpectedTenantSchema, "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', 'abc', 'def'), 'Microsoft.Resources/deployments', 'myMod'), '2020-06-01').outputs.hello.value]", "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', 'abc', 'def'), 'Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("managementGroup", "managementGroup()", "managementGroup", ExpectedMgSchema, "[reference(format('Microsoft.Resources/deployments/{0}', 'myMod'), '2020-06-01').outputs.hello.value]", "[format('Microsoft.Resources/deployments/{0}', 'myMod')]")]
        [DataRow("managementGroup", "subscription('abc')", "subscription", ExpectedMgSchema, "[reference(subscriptionResourceId('abc', 'Microsoft.Resources/deployments', 'myMod'), '2020-06-01').outputs.hello.value]", "[subscriptionResourceId('abc', 'Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("managementGroup", "resourceGroup('abc', 'def')", "resourceGroup", ExpectedMgSchema, "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', 'abc', 'def'), 'Microsoft.Resources/deployments', 'myMod'), '2020-06-01').outputs.hello.value]", "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', 'abc', 'def'), 'Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("subscription", "subscription()", "subscription", ExpectedSubSchema, "[reference(subscriptionResourceId('Microsoft.Resources/deployments', 'myMod'), '2020-06-01').outputs.hello.value]", "[subscriptionResourceId('Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("subscription", "subscription('abc')", "subscription", ExpectedSubSchema, "[reference(subscriptionResourceId('abc', 'Microsoft.Resources/deployments', 'myMod'), '2020-06-01').outputs.hello.value]", "[subscriptionResourceId('abc', 'Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("subscription", "resourceGroup('abc')", "resourceGroup", ExpectedSubSchema, "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'abc'), 'Microsoft.Resources/deployments', 'myMod'), '2020-06-01').outputs.hello.value]", "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'abc'), 'Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("subscription", "tenant()", "tenant", ExpectedSubSchema, "[reference(tenantResourceId('Microsoft.Resources/deployments', 'myMod'), '2020-06-01').outputs.hello.value]", "[tenantResourceId('Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("resourceGroup", "subscription()", "subscription", ExpectedRgSchema, "[reference(subscriptionResourceId('Microsoft.Resources/deployments', 'myMod'), '2020-06-01').outputs.hello.value]", "[subscriptionResourceId('Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("resourceGroup", "subscription('abc')", "subscription", ExpectedRgSchema, "[reference(subscriptionResourceId('abc', 'Microsoft.Resources/deployments', 'myMod'), '2020-06-01').outputs.hello.value]", "[subscriptionResourceId('abc', 'Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("resourceGroup", "resourceGroup()", "resourceGroup", ExpectedRgSchema, "[reference(resourceId('Microsoft.Resources/deployments', 'myMod'), '2020-06-01').outputs.hello.value]", "[resourceId('Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("resourceGroup", "resourceGroup('abc')", "resourceGroup", ExpectedRgSchema, "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'abc'), 'Microsoft.Resources/deployments', 'myMod'), '2020-06-01').outputs.hello.value]", "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'abc'), 'Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("resourceGroup", "resourceGroup('abc', 'def')", "resourceGroup", ExpectedRgSchema, "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', 'abc', 'def'), 'Microsoft.Resources/deployments', 'myMod'), '2020-06-01').outputs.hello.value]", "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', 'abc', 'def'), 'Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("resourceGroup", "tenant()", "tenant", ExpectedRgSchema, "[reference(tenantResourceId('Microsoft.Resources/deployments', 'myMod'), '2020-06-01').outputs.hello.value]", "[tenantResourceId('Microsoft.Resources/deployments', 'myMod')]")]
        [DataTestMethod]
        public void Emitter_should_generate_correct_module_output_scope_strings(string targetScope, string moduleScope, string moduleTargetScope, string expectedSchema, string expectedOutput, string expectedResourceDependsOn)
        {
            var (template, diags, _) = CompilationHelper.Compile(
                ("main.bicep", @"
targetScope = '$targetScope'

module myMod './module.bicep' = {
  name: 'myMod'
  scope: $moduleScope
}

resource resourceB 'My.Rp/myResource@2020-01-01' = {
  name: 'resourceB'
  dependsOn: [
    myMod
  ]
}

output hello string = myMod.outputs.hello
".Replace("$targetScope", targetScope).Replace("$moduleScope", moduleScope)),
                ("module.bicep", @"
targetScope = '$moduleTargetScope'

output hello string = 'hello!'
".Replace("$moduleTargetScope", moduleTargetScope)));

            using (new AssertionScope())
            {
                template.Should().HaveValueAtPath("$.['$schema']", expectedSchema);
                template.Should().HaveValueAtPath("$.outputs.hello.value", expectedOutput);
                template.Should().HaveValueAtPath("$.resources[?(@.name == 'resourceB')].dependsOn[0]", expectedResourceDependsOn);
            }
        }

        [DataRow("tenant", "[tenantResourceId('My.Rp/myResource', 'resourceA')]", "[tenantResourceId('Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("managementGroup", "[format('My.Rp/myResource/{0}', 'resourceA')]", "[format('Microsoft.Resources/deployments/{0}', 'myMod')]")]
        [DataRow("subscription", "[subscriptionResourceId('My.Rp/myResource', 'resourceA')]", "[subscriptionResourceId('Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("resourceGroup", "[resourceId('My.Rp/myResource', 'resourceA')]", "[resourceId('Microsoft.Resources/deployments', 'myMod')]")]
        [DataTestMethod]
        public void Emitter_should_generate_correct_dependsOn_resourceIds(string targetScope, string expectedModuleDependsOn, string expectedResourceDependsOn)
        {
            var (template, diags, _) = CompilationHelper.Compile(
                ("main.bicep", @"
targetScope = '$targetScope'

resource resourceA 'My.Rp/myResource@2020-01-01' = {
  name: 'resourceA'
}

module myMod './module.bicep' = {
  name: 'myMod'
  params: {
    dependency: resourceA.id
  }
}

resource resourceB 'My.Rp/myResource@2020-01-01' = {
  name: 'resourceB'
  dependsOn: [
    myMod
  ]
}
".Replace("$targetScope", targetScope)),
                ("module.bicep", @"
targetScope = '$targetScope'

param dependency string
".Replace("$targetScope", targetScope))
            );

            using (new AssertionScope())
            {
                template.Should().HaveValueAtPath("$.resources[?(@.name == 'resourceB')].dependsOn[0]", expectedResourceDependsOn);
                template.Should().HaveValueAtPath("$.resources[?(@.name == 'myMod')].dependsOn[0]", expectedModuleDependsOn);
            }
        }

        [TestMethod]
        public void Emitter_should_generate_correct_extension_scope_property_and_correct_dependsOn()
        {
            var (template, _, _) = CompilationHelper.Compile(@"
resource resourceA 'My.Rp/myResource@2020-01-01' = {
  name: 'resourceA'
}

resource resourceB 'My.Rp/myResource@2020-01-01' = {
  scope: resourceA
  name: 'resourceB'
}

resource resourceC 'My.Rp/myResource@2020-01-01' = {
  scope: resourceB
  name: 'resourceC'
}");

            using (new AssertionScope())
            {
                template.Should().HaveValueAtPath("$.resources[?(@.name == 'resourceB')].scope", "[format('My.Rp/myResource/{0}', 'resourceA')]");
                template.Should().HaveValueAtPath("$.resources[?(@.name == 'resourceB')].dependsOn[0]", "[resourceId('My.Rp/myResource', 'resourceA')]");

                template.Should().HaveValueAtPath("$.resources[?(@.name == 'resourceC')].scope", "[extensionResourceId(format('My.Rp/myResource/{0}', 'resourceA'), 'My.Rp/myResource', 'resourceB')]");
                template.Should().HaveValueAtPath("$.resources[?(@.name == 'resourceC')].dependsOn[0]", "[extensionResourceId(resourceId('My.Rp/myResource', 'resourceA'), 'My.Rp/myResource', 'resourceB')]");
            }
        }

        [DataRow("tenant", "[format('My.Rp/myResource/{0}', 'resourceA')]", "[reference(tenantResourceId('My.Rp/myResource', 'resourceA'), '2020-01-01').myProp]")]
        [DataRow("managementGroup", "[format('My.Rp/myResource/{0}', 'resourceA')]", "[reference(format('My.Rp/myResource/{0}', 'resourceA'), '2020-01-01').myProp]")]
        [DataRow("subscription", "[format('My.Rp/myResource/{0}', 'resourceA')]", "[reference(subscriptionResourceId('My.Rp/myResource', 'resourceA'), '2020-01-01').myProp]")]
        [DataRow("resourceGroup", "[format('My.Rp/myResource/{0}', 'resourceA')]", "[reference(resourceId('My.Rp/myResource', 'resourceA'), '2020-01-01').myProp]")]
        [DataTestMethod]
        public void Emitter_should_generate_correct_references_for_existing_resources(string targetScope, string expectedScopeExpression, string expectedReferenceExpression)
        {
            var (template, diags, _) = CompilationHelper.Compile(@"
targetScope = '$targetScope'

resource resourceA 'My.Rp/myResource@2020-01-01' existing = {
  name: 'resourceA'
}

resource resourceB 'My.Rp/myResource@2020-01-01' = {
  scope: resourceA
  name: 'resourceB'
}

output resourceARef string = resourceA.properties.myProp
".Replace("$targetScope", targetScope));

            using (new AssertionScope())
            {
                template.Should().HaveValueAtPath("$.resources[?(@.name == 'resourceB')].scope", expectedScopeExpression);
                template.Should().NotHaveValueAtPath("$.resources[?(@.name == 'resourceB')].dependsOn");

                template.Should().HaveValueAtPath("$.outputs['resourceARef'].value", expectedReferenceExpression);
            }
        }

        [TestMethod]
        public void Existing_resources_can_be_referenced_at_other_scopes()
        {
            var typeReference = ResourceTypeReference.Parse("My.Rp/myResource@2020-01-01");
            var typeLoader = TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(new [] {
                new ResourceType(typeReference, ResourceScope.ResourceGroup, new ObjectType(typeReference.FormatName(), TypeSymbolValidationFlags.Default, new [] {
                    new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.DeployTimeConstant, "name property"),
                    new TypeProperty("kind", LanguageConstants.String, TypePropertyFlags.ReadOnly, "kind property"),
                }, null))
            });

            // explicitly pass a valid scope
            var (template, _, _) = CompilationHelper.Compile(typeLoader, ("main.bicep", @"
resource resourceA 'My.Rp/myResource@2020-01-01' existing = {
  name: 'resourceA'
  scope: resourceGroup()
}

output resourceARef string = resourceA.kind
"));

            using (new AssertionScope())
            {
                template.Should().HaveValueAtPath("$.outputs['resourceARef'].value", "[reference(resourceId('My.Rp/myResource', 'resourceA'), '2020-01-01', 'full').kind]");
            }

            // use a valid targetScope without setting the scope property
            (template, _, _) = CompilationHelper.Compile(typeLoader, ("main.bicep", @"
targetScope = 'resourceGroup'

resource resourceA 'My.Rp/myResource@2020-01-01' existing = {
  name: 'resourceA'
}

output resourceARef string = resourceA.kind
"));

            using (new AssertionScope())
            {
                template.Should().HaveValueAtPath("$.outputs['resourceARef'].value", "[reference(resourceId('My.Rp/myResource', 'resourceA'), '2020-01-01', 'full').kind]");
            }
        }

        [TestMethod]
        public void Errors_are_raised_for_existing_resources_at_invalid_scopes()
        {
            var typeReference = ResourceTypeReference.Parse("My.Rp/myResource@2020-01-01");
            var typeLoader = TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(new [] {
                new ResourceType(typeReference, ResourceScope.ResourceGroup, new ObjectType(typeReference.FormatName(), TypeSymbolValidationFlags.Default, new [] {
                    new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.DeployTimeConstant, "name property"),
                }, null))
            });

            // explicitly pass an invalid scope
            var (_, diags, _) = CompilationHelper.Compile(typeLoader, ("main.bicep", @"
resource resourceA 'My.Rp/myResource@2020-01-01' existing = {
  name: 'resourceA'
  scope: subscription()
}
"));

            diags.Should().HaveDiagnostics(new[] {
                ("BCP135", DiagnosticLevel.Error, "Scope \"subscription\" is not valid for this resource type. Permitted scopes: \"resourceGroup\"."),
            });

            // use an invalid targetScope without setting the scope property
            (_, diags, _) = CompilationHelper.Compile(typeLoader, ("main.bicep", @"
targetScope = 'subscription'

resource resourceA 'My.Rp/myResource@2020-01-01' existing = {
  name: 'resourceA'
}
"));

            diags.Should().HaveDiagnostics(new[] {
                ("BCP135", DiagnosticLevel.Error, "Scope \"subscription\" is not valid for this resource type. Permitted scopes: \"resourceGroup\"."),
            });
        }

        [TestMethod]
        public void Errors_are_raised_for_extensions_of_existing_resources_at_invalid_scopes()
        {
            var typeReference = ResourceTypeReference.Parse("My.Rp/myResource@2020-01-01");
            var typeLoader = TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(new [] {
                new ResourceType(typeReference, ResourceScope.ResourceGroup | ResourceScope.Resource, new ObjectType(typeReference.FormatName(), TypeSymbolValidationFlags.Default, new [] {
                    new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.DeployTimeConstant, "name property"),
                }, null))
            });

            // extension resource of an existing resource at an invalid scope
            var (_, diags, _) = CompilationHelper.Compile(typeLoader, ("main.bicep", @"
resource resourceA 'My.Rp/myResource@2020-01-01' existing = {
  name: 'resourceA'
  scope: resourceGroup('different')
}

resource resourceB 'My.Rp/myResource@2020-01-01' = {
  name: 'resourceB'
  scope: resourceA
}
"));

            diags.Should().HaveDiagnostics(new[] {
                ("BCP139", DiagnosticLevel.Error, "The root resource scope must match that of the Bicep file. To deploy a resource to a different root scope, use a module."),
            });
        }

        [TestMethod]
        public void Extensions_of_existing_resources_are_permitted()
        {
            var typeReference = ResourceTypeReference.Parse("My.Rp/myResource@2020-01-01");
            var typeLoader = TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(new [] {
                new ResourceType(typeReference, ResourceScope.ResourceGroup | ResourceScope.Resource, new ObjectType(typeReference.FormatName(), TypeSymbolValidationFlags.Default, new [] {
                    new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.DeployTimeConstant, "name property"),
                }, null))
            });

            // extension resource of an existing resource at an invalid scope
            var (template, diags, _) = CompilationHelper.Compile(typeLoader, ("main.bicep", @"
resource resourceA 'My.Rp/myResource@2020-01-01' existing = {
  name: 'resourceA'
}

resource resourceB 'My.Rp/myResource@2020-01-01' = {
  name: 'resourceB'
  scope: resourceA
}
"));

            using (new AssertionScope())
            {
                diags.Should().BeEmpty();

                template.Should().HaveValueAtPath("$.resources[?(@.name == 'resourceB')].scope", "[format('My.Rp/myResource/{0}', 'resourceA')]");
            }
        }

        [DataRow("resourceGroup", true)]
        [DataRow("subscription", true)]
        [DataRow("managementGroup", true)]
        [DataRow("tenant", false)]
        [DataTestMethod]
        public void Tenant_scope_resources_can_be_deployed_from_anywhere(string targetScope, bool tenantScopeExpected)
        {
            var typeReference = ResourceTypeReference.Parse("My.Rp/myResource@2020-01-01");
            var typeLoader = TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(new[] {
                new ResourceType(typeReference, ResourceScope.Tenant, new ObjectType(typeReference.FormatName(), TypeSymbolValidationFlags.Default, new [] {
                    new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.DeployTimeConstant, "name property"),
                }, null))
            });

            var (template, diags, _) = CompilationHelper.Compile(typeLoader, ("main.bicep", @"
targetScope = 'TARGET_SCOPE'
resource resourceA 'My.Rp/myResource@2020-01-01' = {
  name: 'resourceA'
  scope: tenant()
}
".Replace("TARGET_SCOPE", targetScope)));

            using (new AssertionScope())
            {
                diags.Should().BeEmpty();
                template.Should().NotBeNull();

                const string path = "$.resources[?(@.name == 'resourceA')].scope";
                if (tenantScopeExpected)
                {
                    
                    template.Should().HaveValueAtPath(path, "/");
                }
                else
                {
                    template.Should().NotHaveValueAtPath(path);
                }
            }
        }
    }
}
