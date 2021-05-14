// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Bicep.Core;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Assertions;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Snippets;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.UnitTests.Snippets
{
    [TestClass]
    public class SnippetsProviderTests
    {
        [TestMethod]
        public void GetDescriptionAndText_WithEmptyInput_ReturnsEmptyDescriptionAndText()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();

            (string description, string text) = snippetsProvider.GetDescriptionAndText(string.Empty, @"C:\foo.bicep");

            Assert.IsTrue(description.Equals(string.Empty));
            Assert.IsTrue(text.Equals(string.Empty));
        }

        [TestMethod]
        public void GetDescriptionAndText_WithNullInput_ReturnsEmptyDescriptionAndText()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();

            (string description, string text) = snippetsProvider.GetDescriptionAndText(null, @"C:\foo.bicep");

            Assert.IsTrue(description.Equals(string.Empty));
            Assert.IsTrue(text.Equals(string.Empty));
        }

        [TestMethod]
        public void GetDescriptionAndText_WithOnlyWhitespaceInput_ReturnsEmptyDescriptionAndText()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();

            (string description, string text) = snippetsProvider.GetDescriptionAndText("   ", @"C:\foo.bicep");

            Assert.IsTrue(description.Equals(string.Empty));
            Assert.IsTrue(text.Equals(string.Empty));
        }

        [TestMethod]
        public void GetDescriptionAndText_WithValidInput_ReturnsDescriptionAndText()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();

            string template = @"// DNS Zone
resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: '${1:dnsZone}'
  location: 'global'
  tags: {
    displayName: '${1:dnsZone}'
  }
}";

            (string description, string text) = snippetsProvider.GetDescriptionAndText(template, @"C:\foo.bicep");

            string expectedText = @"resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: '${1:dnsZone}'
  location: 'global'
  tags: {
    displayName: '${1:dnsZone}'
  }
}";

            Assert.AreEqual("DNS Zone", description);
            Assert.AreEqual(expectedText, text);
        }

        [TestMethod]
        public void GetDescriptionAndText_WithMissingCommentInInput_ReturnsEmptyDescriptionAndValidText()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();

            string template = @"resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: '${1:dnsZone}'
  location: 'global'
  tags: {
    displayName: '${1:dnsZone}'
  }
}";

            (string description, string text) = snippetsProvider.GetDescriptionAndText(template, @"C:\foo.bicep");

            string expectedText = @"resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: '${1:dnsZone}'
  location: 'global'
  tags: {
    displayName: '${1:dnsZone}'
  }
}";

            Assert.IsTrue(description.Equals(string.Empty));
            Assert.AreEqual(expectedText, text);
        }

        [TestMethod]
        public void GetDescriptionAndText_WithCommentAndMissingDeclarations_ReturnsEmptyDescriptionAndText()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();

            string template = @"// DNS Zone";

            (string description, string text) = snippetsProvider.GetDescriptionAndText(template, @"C:\foo.bicep");

            Assert.IsTrue(description.Equals(string.Empty));
            Assert.IsTrue(text.Equals(string.Empty));
        }

        [TestMethod]
        public void CompletionPriorityOfResourceSnippets_ShouldBeHigh()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();

            IEnumerable<Snippet> snippets = snippetsProvider.GetTopLevelNamedDeclarationSnippets()
                .Where(x => x.Prefix.StartsWith("resource"));

            foreach (Snippet snippet in snippets)
            {
                Assert.AreEqual(CompletionPriority.High, snippet.CompletionPriority);
            }
        }


        [TestMethod]
        public void CompletionPriorityOfNonResourceSnippets_ShouldBeMedium()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();
            IEnumerable<Snippet> snippets = snippetsProvider.GetTopLevelNamedDeclarationSnippets()
                .Where(x => !x.Prefix.StartsWith("resource"));

            foreach (Snippet snippet in snippets)
            {
                Assert.AreEqual(CompletionPriority.Medium, snippet.CompletionPriority);
            }
        }

        [TestMethod]
        public void GetResourceBodyCompletionSnippets_WithStaticTemplateAndNoResourceDependencies_ShouldReturnSnippets()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();
            TypeSymbol typeSymbol = new ResourceType(
                    ResourceTypeReference.Parse("Microsoft.Network/dnsZones@2018-05-01"),
                    ResourceScope.ResourceGroup,
                    CreateObjectType("Microsoft.Network/dnsZones@2018-05-01",
                    ("name", LanguageConstants.String, TypePropertyFlags.Required),
                    ("location", LanguageConstants.String, TypePropertyFlags.Required)));

            IEnumerable<Snippet> snippets = snippetsProvider.GetResourceBodyCompletionSnippets(typeSymbol, false);

            snippets.Should().SatisfyRespectively(
                x =>
                {
                    x.Prefix.Should().Be("{}");
                    x.Detail.Should().Be("{}");
                    x.CompletionPriority.Should().Be(CompletionPriority.Medium);
                    x.Text.Should().Be("{\n\t$0\n}");
                },
                x =>
                {
                    x.Prefix.Should().Be("snippet");
                    x.Detail.Should().Be("DNS Zone");
                    x.CompletionPriority.Should().Be(CompletionPriority.Medium);
                    x.Text.Should().BeEquivalentToIgnoringNewlines(@"{
  name: ${2:'name'}
  location: 'global'
}
");
                },
                x =>
                {
                    x.Prefix.Should().Be("required-properties");
                    x.Detail.Should().Be("Required properties");
                    x.CompletionPriority.Should().Be(CompletionPriority.Medium);
                    x.Text.Should().BeEquivalentToIgnoringNewlines(@"{
	name: $1
	location: $2
	$0
}");
                });
        }

        [TestMethod]
        public void GetResourceBodyCompletionSnippets_WithStaticTemplateAndResourceDependencies_ShouldReturnSnippets()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();
            TypeSymbol typeSymbol = new ResourceType(
                    ResourceTypeReference.Parse("Microsoft.Automation/automationAccounts/modules@2015-10-31"),
                    ResourceScope.ResourceGroup,
                    CreateObjectType("Microsoft.Automation/automationAccounts/modules@2015-10-31",
                    ("name", LanguageConstants.String, TypePropertyFlags.Required),
                    ("location", LanguageConstants.String, TypePropertyFlags.Required)));

            IEnumerable<Snippet> snippets = snippetsProvider.GetResourceBodyCompletionSnippets(typeSymbol, false);

            snippets.Should().SatisfyRespectively(
                x =>
                {
                    x.Prefix.Should().Be("{}");
                    x.Detail.Should().Be("{}");
                    x.CompletionPriority.Should().Be(CompletionPriority.Medium);
                    x.Text.Should().Be("{\n\t$0\n}");
                },
                x =>
                {
                    x.Prefix.Should().Be("snippet");
                    x.Detail.Should().Be("Automation Module");
                    x.CompletionPriority.Should().Be(CompletionPriority.Medium);
                    x.Text.Should().BeEquivalentToIgnoringNewlines(@"{
  parent: automationAccount
  name: ${3:'name'}
  properties: {
    contentLink: {
      uri: ${4:'https://content-url.nupkg'}
    }
  }
}
resource automationAccount 'Microsoft.Automation/automationAccounts@2015-10-31' = {
  name: ${1:'name'}
}
");
                },
                x =>
                {
                    x.Prefix.Should().Be("required-properties");
                    x.Detail.Should().Be("Required properties");
                    x.CompletionPriority.Should().Be(CompletionPriority.Medium);
                    x.Text.Should().BeEquivalentToIgnoringNewlines(@"{
	name: $1
	location: $2
	$0
}");
                });
        }

        [TestMethod]
        public void GetResourceBodyCompletionSnippets_WithNoStaticTemplate_ShouldReturnSnippets()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();
            TypeSymbol typeSymbol = new ResourceType(
                    ResourceTypeReference.Parse("microsoft.aadiam/azureADMetrics@2020-07-01-preview"),
                    ResourceScope.ResourceGroup,
                    CreateObjectType("microsoft.aadiam/azureADMetrics@2020-07-01-preview",
                    ("name", LanguageConstants.String, TypePropertyFlags.Required),
                    ("location", LanguageConstants.String, TypePropertyFlags.Required),
                    ("kind", LanguageConstants.String, TypePropertyFlags.Required),
                    ("id", LanguageConstants.String, TypePropertyFlags.ReadOnly),
                    ("hostPoolType", LanguageConstants.String, TypePropertyFlags.Required),
                    ("sku", CreateObjectType("applicationGroup",
                            ("friendlyName", LanguageConstants.String, TypePropertyFlags.None),
                            ("properties", CreateObjectType("properties",
                                           ("loadBalancerType", LanguageConstants.String, TypePropertyFlags.Required),
                                           ("preferredAppGroupType", LanguageConstants.String, TypePropertyFlags.WriteOnly)),
                                           TypePropertyFlags.Required),
                            ("name", LanguageConstants.String, TypePropertyFlags.Required)),
                            TypePropertyFlags.Required)));

            IEnumerable<Snippet> snippets = snippetsProvider.GetResourceBodyCompletionSnippets(typeSymbol, false);

            snippets.Should().SatisfyRespectively(
                x =>
                {
                    x.Prefix.Should().Be("{}");
                    x.Detail.Should().Be("{}");
                    x.CompletionPriority.Should().Be(CompletionPriority.Medium);
                    x.Text.Should().Be("{\n\t$0\n}");
                },
                x =>
                {
                    x.Prefix.Should().Be("required-properties");
                    x.Detail.Should().Be("Required properties");
                    x.CompletionPriority.Should().Be(CompletionPriority.Medium);
                    x.Text.Should().BeEquivalentToIgnoringNewlines(@"{
	name: $1
	location: $2
	sku: {
		name: $3
		properties: {
			loadBalancerType: $4
		}
	}
	kind: $5
	hostPoolType: $6
	$0
}");
                });
        }

        [TestMethod]
        public void GetResourceBodyCompletionSnippets_WithNoRequiredProperties_ShouldReturnEmptySnippet()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();
            TypeSymbol typeSymbol = new ResourceType(
                    ResourceTypeReference.Parse("microsoft.aadiam/azureADMetrics@2020-07-01-preview"),
                    ResourceScope.ResourceGroup,
                    CreateObjectType("microsoft.aadiam/azureADMetrics@2020-07-01-preview"));

            IEnumerable<Snippet> snippets = snippetsProvider.GetResourceBodyCompletionSnippets(typeSymbol, false);

            snippets.Should().SatisfyRespectively(
                x =>
                {
                    x.Prefix.Should().Be("{}");
                    x.Detail.Should().Be("{}");
                    x.CompletionPriority.Should().Be(CompletionPriority.Medium);
                    x.Text.Should().Be("{\n\t$0\n}");
                });
        }

        [TestMethod]
        public void GetResourceBodyCompletionSnippets_WithDiscriminatedObjectTypeAndNoRequiredProperties_ShouldReturnEmptySnippet()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();

            var objectTypeA = new ObjectType("objA", TypeSymbolValidationFlags.Default, new[]
            {
                new TypeProperty("discKey", new StringLiteralType("keyA")),
                new TypeProperty("keyAProp", LanguageConstants.String),
            }, null);

            var objectTypeB = new ObjectType("objB", TypeSymbolValidationFlags.Default, new[]
            {
                new TypeProperty("discKey", new StringLiteralType("keyB")),
                new TypeProperty("keyBProp", LanguageConstants.String),
            }, null);

            var discriminatedObjectType = new DiscriminatedObjectType("discObj", TypeSymbolValidationFlags.Default, "discKey", new[] { objectTypeA, objectTypeB });

            TypeSymbol typeSymbol = new ResourceType(
                    ResourceTypeReference.Parse("microsoft.aadiam/azureADMetrics@2020-07-01-preview"),
                    ResourceScope.ResourceGroup,
                    discriminatedObjectType);

            IEnumerable<Snippet> snippets = snippetsProvider.GetResourceBodyCompletionSnippets(typeSymbol, false);

            snippets.Should().SatisfyRespectively(
                x =>
                {
                    x.Prefix.Should().Be("{}");
                    x.Detail.Should().Be("{}");
                    x.CompletionPriority.Should().Be(CompletionPriority.Medium);
                    x.Text.Should().Be("{\n\t$0\n}");
                });
        }

        [TestMethod]
        public void GetResourceBodyCompletionSnippets_WithDiscriminatedObjectTypeAndRequiredProperties_ShouldReturnRequiredPropertiesSnippet()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();

            var objectTypeA = new ObjectType("objA", TypeSymbolValidationFlags.Default, new[]
            {
                new TypeProperty("discKey", new StringLiteralType("keyA")),
                new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.Required),
                new TypeProperty("location", LanguageConstants.String, TypePropertyFlags.Required),
                new TypeProperty("id", LanguageConstants.String)
            }, null);

            var objectTypeB = new ObjectType("objB", TypeSymbolValidationFlags.Default, new[]
            {
                new TypeProperty("discKey", new StringLiteralType("keyB")),
                new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.Required),
                new TypeProperty("kind", LanguageConstants.String, TypePropertyFlags.ReadOnly),
                new TypeProperty("hostPoolType", LanguageConstants.String)
            }, null);

            var discriminatedObjectType = new DiscriminatedObjectType("discObj", TypeSymbolValidationFlags.Default, "discKey", new[] { objectTypeA, objectTypeB });

            TypeSymbol typeSymbol = new ResourceType(
                    ResourceTypeReference.Parse("microsoft.aadiam/azureADMetrics@2020-07-01-preview"),
                    ResourceScope.ResourceGroup,
                    discriminatedObjectType);

            IEnumerable<Snippet> snippets = snippetsProvider.GetResourceBodyCompletionSnippets(typeSymbol, false);

            snippets.Should().SatisfyRespectively(
                x =>
                {
                    x.Prefix.Should().Be("{}");
                    x.Detail.Should().Be("{}");
                    x.CompletionPriority.Should().Be(CompletionPriority.Medium);
                    x.Text.Should().Be("{\n\t$0\n}");
                },
                x =>
                {
                    x.Prefix.Should().Be("required-properties-keyA");
                    x.Detail.Should().Be("Required properties");
                    x.CompletionPriority.Should().Be(CompletionPriority.Medium);
                    x.Text.Should().BeEquivalentToIgnoringNewlines(@"{
	name: $1
	location: $2
	$0
}");
                },
                x =>
                {
                    x.Prefix.Should().Be("required-properties-keyB");
                    x.Detail.Should().Be("Required properties");
                    x.CompletionPriority.Should().Be(CompletionPriority.Medium);
                    x.Text.Should().BeEquivalentToIgnoringNewlines(@"{
	name: $1
	$0
}");
                });
        }

        [TestMethod]
        public void GetModuleBodyCompletionSnippets_WithNoRequiredProperties_ShouldReturnEmptySnippet()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();
            var objectType = new ObjectType("objA", TypeSymbolValidationFlags.Default, new[]
            {
                new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.ReadOnly),
                new TypeProperty("location", LanguageConstants.String, TypePropertyFlags.WriteOnly),
                new TypeProperty("id", LanguageConstants.String)
            }, null);
            TypeSymbol typeSymbol = new ModuleType("module", ResourceScope.Module, objectType);

            IEnumerable<Snippet> snippets = snippetsProvider.GetModuleBodyCompletionSnippets(typeSymbol);

            snippets.Should().SatisfyRespectively(
                x =>
                {
                    x.Prefix.Should().Be("{}");
                    x.Detail.Should().Be("{}");
                    x.CompletionPriority.Should().Be(CompletionPriority.Medium);
                    x.Text.Should().Be("{\n\t$0\n}");
                });
        }

        [TestMethod]
        public void GetModuleBodyCompletionSnippets_WithRequiredProperties_ShouldReturnEmptyAndRequiredPropertiesSnippets()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();
            var objectType = new ObjectType("objA", TypeSymbolValidationFlags.Default, new[]
            {
                new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.Required),
                new TypeProperty("location", LanguageConstants.String, TypePropertyFlags.Required),
                new TypeProperty("id", LanguageConstants.String)
            }, null);
            TypeSymbol typeSymbol = new ModuleType("module", ResourceScope.Module, objectType);

            IEnumerable<Snippet> snippets = snippetsProvider.GetModuleBodyCompletionSnippets(typeSymbol);

            snippets.Should().SatisfyRespectively(
                x =>
                {
                    x.Prefix.Should().Be("{}");
                    x.Detail.Should().Be("{}");
                    x.CompletionPriority.Should().Be(CompletionPriority.Medium);
                    x.Text.Should().Be("{\n\t$0\n}");
                },
                x =>
                {
                    x.Prefix.Should().Be("required-properties");
                    x.Detail.Should().Be("Required properties");
                    x.CompletionPriority.Should().Be(CompletionPriority.Medium);
                    x.Text.Should().BeEquivalentToIgnoringNewlines(@"{
	name: $1
	location: $2
	$0
}");
                });
        }

        private static ObjectType CreateObjectType(string name, params (string name, ITypeReference type, TypePropertyFlags typePropertyFlags)[] properties)
            => new(
                name,
                TypeSymbolValidationFlags.Default,
                properties.Select(val => new TypeProperty(val.name, val.type, val.typePropertyFlags)),
                null);
    }
}
