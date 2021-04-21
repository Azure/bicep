// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Bicep.Core;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
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
        public void GetResourceBodyCompletionSnippet_WithSnippetSourcedFromStaticTemplateAndNoResourceDependencies_ShouldReturnNonEmptySnippet()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();
            TypeSymbol typeSymbol = new ResourceType(
                    ResourceTypeReference.Parse("Microsoft.Network/dnsZones@2018-05-01"),
                    ResourceScope.ResourceGroup,
                    TestTypeHelper.CreateObjectType(
                        "Microsoft.Network/dnsZones@2018-05-01",
                        ("name", LanguageConstants.String)));

            Snippet? snippet = snippetsProvider.GetResourceBodyCompletionSnippet(typeSymbol);

            Assert.IsNotNull(snippet);
            Assert.AreEqual("{}", snippet.Prefix);
            Assert.AreEqual("{}", snippet.Detail);
            Assert.AreEqual(CompletionPriority.Medium, snippet.CompletionPriority);
            snippet.Text.Should().BeEquivalentToIgnoringNewlines(@"{
  name: ${1:'dnsZone'}
  location: 'global'
}
");
        }

        [TestMethod]
        public void GetResourceBodyCompletionSnippet_WithSnippetSourcedFromStaticTemplateAndResourceDependencies_ShouldReturnNonEmptySnippet()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();
            TypeSymbol typeSymbol = new ResourceType(
                    ResourceTypeReference.Parse("Microsoft.Automation/automationAccounts/modules@2015-10-31"),
                    ResourceScope.ResourceGroup,
                    TestTypeHelper.CreateObjectType(
                        "Microsoft.Automation/automationAccounts/modules@2015-10-31",
                        ("name", LanguageConstants.String)));

            Snippet snippet = snippetsProvider.GetResourceBodyCompletionSnippet(typeSymbol);

            Assert.IsNotNull(snippet);
            Assert.AreEqual("{}", snippet.Prefix);
            Assert.AreEqual("{}", snippet.Detail);
            Assert.AreEqual(CompletionPriority.Medium, snippet.CompletionPriority);
            snippet.Text.Should().BeEquivalentToIgnoringNewlines(@"{
  name: '${automationAccount.name}/${2:automationVariable}'
  properties: {
    contentLink: {
      uri: ${3:'https://content-url.nupkg'}
    }
  }
}
resource automationAccount 'Microsoft.Automation/automationAccounts@2015-10-31' = {
  name: ${1:'automationAccount'}
}
");
        }

        [TestMethod]
        public void GetResourceBodyCompletionSnippet_WithPropertiesFromSwaggerSpec_ShouldReturnSnippetWithRequiredProperties()
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
                            ("name", LanguageConstants.String, TypePropertyFlags.Required),
                            ("friendlyName", LanguageConstants.String, TypePropertyFlags.None),
                            ("properties", CreateObjectType("properties",
                                           ("loadBalancerType", LanguageConstants.String, TypePropertyFlags.Required),
                                           ("preferredAppGroupType", LanguageConstants.String, TypePropertyFlags.WriteOnly)),
                                           TypePropertyFlags.Required)),
                            TypePropertyFlags.Required)));

            Snippet snippet = snippetsProvider.GetResourceBodyCompletionSnippet(typeSymbol);

            Assert.IsNotNull(snippet);
            Assert.AreEqual("{}", snippet.Prefix);
            Assert.AreEqual("{}", snippet.Detail);
            Assert.AreEqual(CompletionPriority.Medium, snippet.CompletionPriority);
            snippet.Text.Should().BeEquivalentToIgnoringNewlines(@"{
	hostPoolType: $1
	kind: $2
	location: $3
	name: $4
	sku: {
	name: $5
	properties: {
	loadBalancerType: $6
	}
	}
	$0
}");
        }

        [TestMethod]
        public void GetResourceBodyCompletionSnippet_WithNoRequiredProperties_ShouldReturnEmptySnippet()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();
            TypeSymbol typeSymbol = new ResourceType(
                    ResourceTypeReference.Parse("microsoft.aadiam/azureADMetrics@2020-07-01-preview"),
                    ResourceScope.ResourceGroup,
                    CreateObjectType("microsoft.aadiam/azureADMetrics@2020-07-01-preview"));

            Snippet snippet = snippetsProvider.GetResourceBodyCompletionSnippet(typeSymbol);

            Assert.IsNotNull(snippet);
            Assert.AreEqual("{}", snippet.Prefix);
            Assert.AreEqual("{}", snippet.Detail);
            Assert.AreEqual(CompletionPriority.Medium, snippet.CompletionPriority);
            snippet.Text.Should().BeEquivalentToIgnoringNewlines("{\n\t$0\n}");
        }

        private static ObjectType CreateObjectType(string name, params (string name, ITypeReference type, TypePropertyFlags typePropertyFlags)[] properties)
            => new(
                name,
                TypeSymbolValidationFlags.Default,
                properties.Select(val => new TypeProperty(val.name, val.type, val.typePropertyFlags)),
                null);
    }
}
