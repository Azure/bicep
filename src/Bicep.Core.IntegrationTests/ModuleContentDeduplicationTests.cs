// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ModuleContentDeduplicationTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private ServiceBuilder ServicesWithFeature
            => new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, ModuleContentDeduplicationEnabled: true));

        [TestMethod]
        public void Enabled_emits_templateLink_contentId_and_deduplicates_identical_modules()
        {
            var result = CompilationHelper.Compile(
                ServicesWithFeature,
                ("main.bicep", """
                    module mod1 'mod.bicep' = {
                      name: 'mod1'
                      params: { foo: 'a' }
                    }

                    module mod2 'mod.bicep' = {
                      name: 'mod2'
                      params: { foo: 'b' }
                    }
                    """),
                ("mod.bicep", """
                    param foo string
                    output bar string = foo
                    """));

            result.Should().NotHaveAnyDiagnostics();
            var template = result.Template!;

            var contentId = template.SelectToken("$.resources['mod1'].properties.templateLink.contentId")?.ToString();
            contentId.Should().NotBeNull();
            contentId.Should().StartWith("sha256:");

            using (new AssertionScope())
            {
                // The nested template is no longer inlined; both modules reference the shared content by id.
                template.Should().NotHaveValueAtPath("$.resources['mod1'].properties.template");
                template.Should().HaveValueAtPath("$.resources['mod1'].properties.templateLink", JToken.Parse($$"""
                    { "contentId": "{{contentId}}" }
                    """));

                // Identical module bodies deduplicate to the same contentId.
                template.Should().HaveValueAtPath("$.resources['mod2'].properties.templateLink", JToken.Parse($$"""
                    { "contentId": "{{contentId}}" }
                    """));

                // The stored body is wrapped under "value".
                template.Should().HaveValueAtPath($"$.content['{contentId}'].value.parameters", JToken.Parse("""
                    {
                      "foo": { "type": "string" }
                    }
                    """));
                template.Should().HaveValueAtPath($"$.content['{contentId}'].value.outputs", JToken.Parse("""
                    {
                      "bar": { "type": "string", "value": "[parameters('foo')]" }
                    }
                    """));

                // The shared store holds exactly one (deduplicated) entry.
                ((JObject)template.SelectToken("$.content")!).Properties().Should().ContainSingle();
            }
        }

        [TestMethod]
        public void Enabled_deduplicates_nested_modules_into_single_top_level_store()
        {
            var result = CompilationHelper.Compile(
                ServicesWithFeature,
                ("main.bicep", """
                    module parent 'parent.bicep' = {
                      name: 'parent'
                    }
                    """),
                ("parent.bicep", """
                    module child1 'leaf.bicep' = {
                      name: 'child1'
                      params: { foo: 'a' }
                    }
                    module child2 'leaf.bicep' = {
                      name: 'child2'
                      params: { foo: 'b' }
                    }
                    """),
                ("leaf.bicep", """
                    param foo string
                    output bar string = foo
                    """));

            result.Should().NotHaveAnyDiagnostics();
            var template = result.Template!;

            // The single top-level content store contains the parent and the (deduplicated) leaf.
            var content = (JObject)template.SelectToken("$.content")!;
            content.Properties().Should().HaveCount(2);

            // Locate the parent's stored body (the entry whose value declares child modules).
            var parentValue = content.Properties()
                .Select(p => p.Value["value"]!)
                .Single(v => v["resources"]?["child1"] is not null);

            var child1Link = parentValue.SelectToken("resources.child1.properties.templateLink");

            using (new AssertionScope())
            {
                // Nested module bodies also reference content by id rather than inlining templates.
                parentValue.SelectToken("resources.child1.properties.template").Should().BeNull();
                child1Link.Should().NotBeNull();

                // Both children reference the same deduplicated leaf content.
                parentValue.SelectToken("resources.child2.properties.templateLink").Should().DeepEqual(child1Link!);

                // And the referenced leaf actually exists in the shared top-level store.
                var leafContentId = child1Link!["contentId"]!.ToString();
                content.Properties().Select(p => p.Name).Should().Contain(leafContentId);
            }
        }

        [TestMethod]
        public void Disabled_by_default_inlines_nested_templates_and_omits_content_store()
        {
            // Symbolic names are enabled purely so the emitted resources are an object map (stable JSONPath);
            // the content-deduplication feature itself remains disabled.
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, SymbolicNameCodegenEnabled: true));
            var result = CompilationHelper.Compile(
                services,
                ("main.bicep", """
                    module mod 'mod.bicep' = {
                      name: 'mod'
                      params: { foo: 'a' }
                    }
                    """),
                ("mod.bicep", """
                    param foo string
                    output bar string = foo
                    """));

            result.Should().NotHaveAnyDiagnostics();
            var template = result.Template!;

            using (new AssertionScope())
            {
                template.Should().HaveValueAtPath("$.resources['mod'].properties.template.$schema", "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#");
                template.Should().NotHaveValueAtPath("$.resources['mod'].properties.templateLink");
                template.Should().NotHaveValueAtPath("$.content");
            }
        }
    }
}
