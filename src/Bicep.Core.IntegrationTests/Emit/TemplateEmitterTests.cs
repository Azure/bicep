// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Bicep.Core.Emit;
using Bicep.Core.FileSystem;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests.Emit
{
    [TestClass]
    public class TemplateEmitterTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void ValidBicep_TemplateEmiterShouldProduceExpectedTemplate(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext, dataSet.Name);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);
            var compiledFilePath = FileHelper.GetResultFilePath(this.TestContext, Path.Combine(dataSet.Name, DataSet.TestFileMainCompiled));

            // emitting the template should be successful
            var result = this.EmitTemplate(SyntaxTreeGroupingBuilder.Build(new FileResolver(), new Workspace(), PathHelper.FilePathToFileUrl(bicepFilePath)), compiledFilePath);
            result.Status.Should().Be(EmitStatus.Succeeded);
            result.Diagnostics.Should().BeEmpty();

            var actual = JToken.Parse(File.ReadAllText(compiledFilePath));

            actual.Should().EqualWithJsonDiffOutput(
                TestContext, 
                JToken.Parse(dataSet.Compiled!),
                expectedLocation: DataSet.GetBaselineUpdatePath(dataSet, DataSet.TestFileMainCompiled),
                actualLocation: compiledFilePath);
        }

        [TestMethod]
        public void TemplateEmitter_output_should_not_include_UTF8_BOM()
        {
            var syntaxTreeGrouping = SyntaxTreeGroupingFactory.CreateFromText("");
            var compiledFilePath = FileHelper.GetResultFilePath(this.TestContext, "main.json");

            // emitting the template should be successful
            var result = this.EmitTemplate(syntaxTreeGrouping, compiledFilePath);
            result.Status.Should().Be(EmitStatus.Succeeded);
            result.Diagnostics.Should().BeEmpty();

            var bytes = File.ReadAllBytes(compiledFilePath);
            // No BOM at the start of the file
            bytes.Take(3).Should().NotBeEquivalentTo(new [] { 0xEF, 0xBB, 0xBF }, "BOM should not be present");
            bytes.First().Should().Be(0x7B, "template should always begin with a UTF-8 encoded open curly");
            bytes.Last().Should().Be(0x7D, "template should always end with a UTF-8 encoded close curly");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void ValidBicepTextWriter_TemplateEmiterShouldProduceExpectedTemplate(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext, dataSet.Name);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);
            MemoryStream memoryStream = new MemoryStream();

            // emitting the template should be successful
            var result = this.EmitTemplate(SyntaxTreeGroupingBuilder.Build(new FileResolver(), new Workspace(), PathHelper.FilePathToFileUrl(bicepFilePath)), memoryStream);
            result.Diagnostics.Should().BeEmpty();
            result.Status.Should().Be(EmitStatus.Succeeded);

            // normalizing the formatting in case there are differences in indentation
            // this way the diff between actual and expected will be clean
            var actual = JToken.ReadFrom(new JsonTextReader(new StreamReader(new MemoryStream(memoryStream.ToArray()))));
            var compiledFilePath = FileHelper.SaveResultFile(this.TestContext, Path.Combine(dataSet.Name, DataSet.TestFileMainCompiled), actual.ToString(Formatting.Indented));

            actual.Should().EqualWithJsonDiffOutput(
                TestContext, 
                JToken.Parse(dataSet.Compiled!),
                expectedLocation: DataSet.GetBaselineUpdatePath(dataSet, DataSet.TestFileMainCompiled),
                actualLocation: compiledFilePath);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetInvalidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void InvalidBicep_TemplateEmiterShouldNotProduceAnyTemplate(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext, dataSet.Name);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);
            string filePath = FileHelper.GetResultFilePath(this.TestContext, $"{dataSet.Name}_Compiled_Original.json");

            // emitting the template should fail
            var result = this.EmitTemplate(SyntaxTreeGroupingBuilder.Build(new FileResolver(), new Workspace(), PathHelper.FilePathToFileUrl(bicepFilePath)), filePath);
            result.Status.Should().Be(EmitStatus.Failed);
            result.Diagnostics.Should().NotBeEmpty();
        }

        private const string ExpectedTenantSchema = "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#";
        private const string ExpectedMgSchema = "https://schema.management.azure.com/schemas/2019-08-01/managementGroupDeploymentTemplate.json#";
        private const string ExpectedSubSchema = "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#";
        private const string ExpectedRgSchema = "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#";

        [DataRow("tenant", "tenant()", "tenant", ExpectedTenantSchema, "[reference(tenantResourceId('Microsoft.Resources/deployments', 'myMod'), '2019-10-01').outputs.hello.value]", "[tenantResourceId('Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("tenant", "managementGroup('abc')", "managementGroup", ExpectedTenantSchema, "[reference(extensionResourceId(tenantResourceId('Microsoft.Management/managementGroups', 'abc'), 'Microsoft.Resources/deployments', 'myMod'), '2019-10-01').outputs.hello.value]", "[extensionResourceId(tenantResourceId('Microsoft.Management/managementGroups', 'abc'), 'Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("tenant", "subscription('abc')", "subscription", ExpectedTenantSchema, "[reference(subscriptionResourceId('abc', 'Microsoft.Resources/deployments', 'myMod'), '2019-10-01').outputs.hello.value]", "[subscriptionResourceId('abc', 'Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("tenant", "resourceGroup('abc', 'def')", "resourceGroup", ExpectedTenantSchema, "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', 'abc', 'def'), 'Microsoft.Resources/deployments', 'myMod'), '2019-10-01').outputs.hello.value]", "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', 'abc', 'def'), 'Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("managementGroup", "managementGroup()", "managementGroup", ExpectedMgSchema, "[reference(format('Microsoft.Resources/deployments/{0}', 'myMod'), '2019-10-01').outputs.hello.value]", "[format('Microsoft.Resources/deployments/{0}', 'myMod')]")]
        [DataRow("managementGroup", "subscription('abc')", "subscription", ExpectedMgSchema, "[reference(subscriptionResourceId('abc', 'Microsoft.Resources/deployments', 'myMod'), '2019-10-01').outputs.hello.value]", "[subscriptionResourceId('abc', 'Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("managementGroup", "resourceGroup('abc', 'def')", "resourceGroup", ExpectedMgSchema, "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', 'abc', 'def'), 'Microsoft.Resources/deployments', 'myMod'), '2019-10-01').outputs.hello.value]", "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', 'abc', 'def'), 'Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("subscription", "subscription()", "subscription", ExpectedSubSchema, "[reference(subscriptionResourceId('Microsoft.Resources/deployments', 'myMod'), '2019-10-01').outputs.hello.value]", "[subscriptionResourceId('Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("subscription", "subscription('abc')", "subscription", ExpectedSubSchema, "[reference(subscriptionResourceId('abc', 'Microsoft.Resources/deployments', 'myMod'), '2019-10-01').outputs.hello.value]", "[subscriptionResourceId('abc', 'Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("subscription", "resourceGroup('abc')", "resourceGroup", ExpectedSubSchema, "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'abc'), 'Microsoft.Resources/deployments', 'myMod'), '2019-10-01').outputs.hello.value]", "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'abc'), 'Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("subscription", "tenant()", "tenant", ExpectedSubSchema, "[reference(tenantResourceId('Microsoft.Resources/deployments', 'myMod'), '2019-10-01').outputs.hello.value]", "[tenantResourceId('Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("resourceGroup", "subscription()", "subscription", ExpectedRgSchema, "[reference(subscriptionResourceId('Microsoft.Resources/deployments', 'myMod'), '2019-10-01').outputs.hello.value]", "[subscriptionResourceId('Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("resourceGroup", "subscription('abc')", "subscription", ExpectedRgSchema, "[reference(subscriptionResourceId('abc', 'Microsoft.Resources/deployments', 'myMod'), '2019-10-01').outputs.hello.value]", "[subscriptionResourceId('abc', 'Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("resourceGroup", "resourceGroup()", "resourceGroup", ExpectedRgSchema, "[reference(extensionResourceId(resourceGroup().id, 'Microsoft.Resources/deployments', 'myMod'), '2019-10-01').outputs.hello.value]", "[extensionResourceId(resourceGroup().id, 'Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("resourceGroup", "resourceGroup('abc')", "resourceGroup", ExpectedRgSchema, "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'abc'), 'Microsoft.Resources/deployments', 'myMod'), '2019-10-01').outputs.hello.value]", "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'abc'), 'Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("resourceGroup", "resourceGroup('abc', 'def')", "resourceGroup", ExpectedRgSchema, "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', 'abc', 'def'), 'Microsoft.Resources/deployments', 'myMod'), '2019-10-01').outputs.hello.value]", "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', 'abc', 'def'), 'Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("resourceGroup", "tenant()", "tenant", ExpectedRgSchema, "[reference(tenantResourceId('Microsoft.Resources/deployments', 'myMod'), '2019-10-01').outputs.hello.value]", "[tenantResourceId('Microsoft.Resources/deployments', 'myMod')]")]
        [DataTestMethod]
        public void Emitter_should_generate_correct_module_output_scope_strings(string targetScope, string moduleScope, string moduleTargetScope, string expectedSchema, string expectedOutput, string expectedResourceDependsOn)
        {
            var (json, diags) = CompilationHelper.Compile(
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

            json.Should().NotBeNull();
            var template = JObject.Parse(json!);

            using (new AssertionScope())
            {
                template.SelectToken("$.['$schema']")!.ToString().Should().Be(expectedSchema);
                template.SelectToken("$.outputs.hello.value")!.ToString().Should().Be(expectedOutput);
                template.SelectToken("$.resources[?(@.name == 'resourceB')].dependsOn[0]")!.ToString().Should().Be(expectedResourceDependsOn);
            }
        }

        [DataRow("tenant", "[tenantResourceId('My.Rp/myResource', 'resourceA')]", "[tenantResourceId('Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("managementGroup", "[format('My.Rp/myResource/{0}', 'resourceA')]", "[format('Microsoft.Resources/deployments/{0}', 'myMod')]")]
        [DataRow("subscription", "[subscriptionResourceId('My.Rp/myResource', 'resourceA')]", "[subscriptionResourceId('Microsoft.Resources/deployments', 'myMod')]")]
        [DataRow("resourceGroup", "[resourceId('My.Rp/myResource', 'resourceA')]", "[extensionResourceId(resourceGroup().id, 'Microsoft.Resources/deployments', 'myMod')]")]
        [DataTestMethod]
        public void Emitter_should_generate_correct_dependsOn_resourceIds(string targetScope, string expectedModuleDependsOn, string expectedResourceDependsOn)
        {
            var (json, diags) = CompilationHelper.Compile(
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

            json.Should().NotBeNull();
            var template = JObject.Parse(json!);

            using (new AssertionScope())
            {
                template.SelectToken("$.resources[?(@.name == 'resourceB')].dependsOn[0]")!.ToString().Should().Be(expectedResourceDependsOn);
                template.SelectToken("$.resources[?(@.name == 'myMod')].dependsOn[0]")!.ToString().Should().Be(expectedModuleDependsOn);
            }
        }

        [TestMethod]
        public void Emitter_should_generate_correct_extension_scope_property_and_correct_dependsOn()
        {
            var (json, diags) = CompilationHelper.Compile(@"
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

            json.Should().NotBeNull();
            var template = JObject.Parse(json!);

            using (new AssertionScope())
            {
                template.SelectToken("$.resources[?(@.name == 'resourceB')].scope")!.ToString().Should().Be("[format('My.Rp/myResource/{0}', 'resourceA')]");
                template.SelectToken("$.resources[?(@.name == 'resourceB')].dependsOn[0]")!.ToString().Should().Be("[resourceId('My.Rp/myResource', 'resourceA')]");
                
                template.SelectToken("$.resources[?(@.name == 'resourceC')].scope")!.ToString().Should().Be("[extensionResourceId(format('My.Rp/myResource/{0}', 'resourceA'), 'My.Rp/myResource', 'resourceB')]");
                template.SelectToken("$.resources[?(@.name == 'resourceC')].dependsOn[0]")!.ToString().Should().Be("[extensionResourceId(format('My.Rp/myResource/{0}', 'resourceA'), 'My.Rp/myResource', 'resourceB')]");
            }
        }

        private EmitResult EmitTemplate(SyntaxTreeGrouping syntaxTreeGrouping, string filePath)
        {
            var compilation = new Compilation(TestResourceTypeProvider.Create(), syntaxTreeGrouping);
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());

            using var stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            return emitter.Emit(stream);
        }

        private EmitResult EmitTemplate(SyntaxTreeGrouping syntaxTreeGrouping, MemoryStream memoryStream)
        {
            var compilation = new Compilation(TestResourceTypeProvider.Create(), syntaxTreeGrouping);
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());

            TextWriter tw = new StreamWriter(memoryStream);
            return emitter.Emit(tw);
        }

        private static IEnumerable<object[]> GetValidDataSets() => DataSets
            .AllDataSets
            .Where(ds => ds.IsValid)
            .ToDynamicTestData();

        private static IEnumerable<object[]> GetInvalidDataSets() => DataSets
            .AllDataSets
            .Where(ds => ds.IsValid == false)
            .ToDynamicTestData();
    }
}

