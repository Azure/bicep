// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests;
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
        private readonly SnippetsProvider snippetsProvider = new(BicepTestConstants.NamespaceProvider, BicepTestConstants.FileResolver, BicepTestConstants.ConfigurationManager);

        [TestMethod]
        public void GetDescriptionAndText_WithEmptyInput_ReturnsEmptyDescriptionAndText()
        {
            (string description, string text) = snippetsProvider.GetDescriptionAndText(string.Empty, @"C:\foo.bicep");

            Assert.IsTrue(description.Equals(string.Empty));
            Assert.IsTrue(text.Equals(string.Empty));
        }

        [TestMethod]
        public void GetDescriptionAndText_WithNullInput_ReturnsEmptyDescriptionAndText()
        {
            (string description, string text) = snippetsProvider.GetDescriptionAndText(null, @"C:\foo.bicep");

            Assert.IsTrue(description.Equals(string.Empty));
            Assert.IsTrue(text.Equals(string.Empty));
        }

        [TestMethod]
        public void GetDescriptionAndText_WithOnlyWhitespaceInput_ReturnsEmptyDescriptionAndText()
        {
            (string description, string text) = snippetsProvider.GetDescriptionAndText("   ", @"C:\foo.bicep");

            Assert.IsTrue(description.Equals(string.Empty));
            Assert.IsTrue(text.Equals(string.Empty));
        }

        [TestMethod]
        public void GetDescriptionAndText_WithValidInput_ReturnsDescriptionAndText()
        {
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
            string template = @"// DNS Zone";

            (string description, string text) = snippetsProvider.GetDescriptionAndText(template, @"C:\foo.bicep");

            Assert.IsTrue(description.Equals(string.Empty));
            Assert.IsTrue(text.Equals(string.Empty));
        }

        [TestMethod]
        public void CompletionPriorityOfResourceSnippets_ShouldBeHigh()
        {
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
            ResourceType resourceType = new ResourceType(
                ResourceTypeReference.Parse("Microsoft.DataLakeStore/accounts@2016-11-01"),
                ResourceScope.ResourceGroup,
                CreateObjectType("Microsoft.DataLakeStore/accounts@2016-11-01",
                ("name", LanguageConstants.String, TypePropertyFlags.Required),
                ("location", LanguageConstants.String, TypePropertyFlags.Required)));

            IEnumerable<Snippet> snippets = snippetsProvider.GetResourceBodyCompletionSnippets(resourceType, isExistingResource: false, isResourceNested: false);

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
                    x.Detail.Should().Be("Data Lake Store Account");
                    x.CompletionPriority.Should().Be(CompletionPriority.Medium);
                    x.Text.Should().BeEquivalentToIgnoringNewlines(@"{
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    newTier: ${3|'Consumption','Commitment_1TB','Commitment_10TB','Commitment_100TB','Commitment_500TB','Commitment_1PB','Commitment_5PB'|}
    encryptionState: ${4|'Enabled','Disabled'|}
  }
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
}$0");
                });
        }

        [TestMethod]
        public void GetResourceBodyCompletionSnippets_WithStaticTemplateAndResourceDependencies_ShouldReturnSnippets()
        {
            ResourceType resourceType = new ResourceType(
                ResourceTypeReference.Parse("Microsoft.Automation/automationAccounts/modules@2015-10-31"),
                ResourceScope.ResourceGroup,
                CreateObjectType("Microsoft.Automation/automationAccounts/modules@2015-10-31",
                ("name", LanguageConstants.String, TypePropertyFlags.Required),
                ("location", LanguageConstants.String, TypePropertyFlags.Required)));

            IEnumerable<Snippet> snippets = snippetsProvider.GetResourceBodyCompletionSnippets(resourceType, isExistingResource: false, isResourceNested: false);

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
}$0");
                });
        }

        [TestMethod]
        public void GetResourceBodyCompletionSnippets_WithNestedResource_ShouldReturnSnippets()
        {
            ResourceType resourceType = new ResourceType(
                ResourceTypeReference.Parse("Microsoft.Automation/automationAccounts/certificates@2019-06-01"),
                ResourceScope.ResourceGroup,
                CreateObjectType("Microsoft.Automation/automationAccounts/certificates@2019-06-01",
                ("name", LanguageConstants.String, TypePropertyFlags.Required),
                ("location", LanguageConstants.String, TypePropertyFlags.Required)));

            IEnumerable<Snippet> snippets = snippetsProvider.GetResourceBodyCompletionSnippets(resourceType, isExistingResource: false, isResourceNested: true);

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
                    x.Detail.Should().Be("Automation Certificate");
                    x.CompletionPriority.Should().Be(CompletionPriority.Medium);
                    x.Text.Should().BeEquivalentToIgnoringNewlines(@"{
  name: ${3:'name'}
  properties: {
    base64Value: ${4:'base64Value'}
    description: ${5:'description'}
    thumbprint: ${6:'thumbprint'}
    isExportable: ${7|true,false|}
  }
}");
                },
                x =>
                {
                    x.Prefix.Should().Be("required-properties");
                    x.Detail.Should().Be("Required properties");
                    x.CompletionPriority.Should().Be(CompletionPriority.Medium);
                    x.Text.Should().BeEquivalentToIgnoringNewlines(@"{
	name: $1
	location: $2
}$0");
                });
        }

        [TestMethod]
        public void GetResourceBodyCompletionSnippets_WithNoStaticTemplate_ShouldReturnSnippets()
        {
            ResourceType resourceType = new ResourceType(
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

            IEnumerable<Snippet> snippets = snippetsProvider.GetResourceBodyCompletionSnippets(resourceType, isExistingResource: false, isResourceNested: false);

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
}$0");
                });
        }

        [TestMethod]
        public void GetResourceBodyCompletionSnippets_WithNoRequiredProperties_ShouldReturnEmptySnippet()
        {
            ResourceType resourceType = new ResourceType(
                ResourceTypeReference.Parse("microsoft.aadiam/azureADMetrics@2020-07-01-preview"),
                ResourceScope.ResourceGroup,
                CreateObjectType("microsoft.aadiam/azureADMetrics@2020-07-01-preview"));

            IEnumerable<Snippet> snippets = snippetsProvider.GetResourceBodyCompletionSnippets(resourceType, isExistingResource: false, isResourceNested: false);

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
        public void GetNestedResourceDeclarationSnippets_WithChildResources_ShouldReturnCustomAndDefaultResourceSnippets()
        {
            ResourceTypeReference resourceTypeReference = ResourceTypeReference.Parse("Microsoft.Automation/automationAccounts@2019-06-01");

            IEnumerable<Snippet> snippets = snippetsProvider.GetNestedResourceDeclarationSnippets(resourceTypeReference);

            snippets.Should().SatisfyRespectively(
                x =>
                {
                    x.Prefix.Should().Be("resource-with-defaults");
                },
                x =>
                {
                    x.Prefix.Should().Be("resource-without-defaults");
                },
                x =>
                {
                    x.Prefix.Should().Be("res-automation-cert");
                },
                x =>
                {
                    x.Prefix.Should().Be("res-automation-cred");
                },
                x =>
                {
                    x.Prefix.Should().Be("res-automation-job-schedule");
                },
                x =>
                {
                    x.Prefix.Should().Be("res-automation-runbook");
                },
                x =>
                {
                    x.Prefix.Should().Be("res-automation-schedule");
                },
                x =>
                {
                    x.Prefix.Should().Be("res-automation-variable");
                });
        }

        [TestMethod]
        public void GetNestedResourceDeclarationSnippets_WithNoChildResources_ShouldReturnDefaultResourceSnippets()
        {
            ResourceTypeReference resourceTypeReference = ResourceTypeReference.Parse("Microsoft.Automation/automationAccounts/runbooks@2019-06-01");

            IEnumerable<Snippet> snippets = snippetsProvider.GetNestedResourceDeclarationSnippets(resourceTypeReference);

            snippets.Should().SatisfyRespectively(
                x =>
                {
                    x.Prefix.Should().Be("resource-with-defaults");
                },
                x =>
                {
                    x.Prefix.Should().Be("resource-without-defaults");
                });
        }

        [TestMethod]
        public void GetResourceBodyCompletionSnippets_WithDiscriminatedObjectTypeAndNoRequiredProperties_ShouldReturnEmptySnippet()
        {
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

            ResourceType resourceType = new ResourceType(
                ResourceTypeReference.Parse("microsoft.aadiam/azureADMetrics@2020-07-01-preview"),
                ResourceScope.ResourceGroup,
                discriminatedObjectType);

            IEnumerable<Snippet> snippets = snippetsProvider.GetResourceBodyCompletionSnippets(resourceType, isExistingResource: false, isResourceNested: false);

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
            var objectTypeA = new ObjectType("objA", TypeSymbolValidationFlags.Default, new[]
            {
                new TypeProperty("discKey", new StringLiteralType("keyA")),
                new TypeProperty("name", new StringLiteralType("keyA"), TypePropertyFlags.Required),
                new TypeProperty("location", LanguageConstants.String, TypePropertyFlags.Required),
                new TypeProperty("id", LanguageConstants.String)
            }, null);

            var objectTypeB = new ObjectType("objB", TypeSymbolValidationFlags.Default, new[]
            {
                new TypeProperty("discKey", new StringLiteralType("keyB")),
                new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.Required),
                new TypeProperty("kind", new StringLiteralType("discKey"), TypePropertyFlags.ReadOnly),
                new TypeProperty("hostPoolType", LanguageConstants.String)
            }, null);

            var discriminatedObjectType = new DiscriminatedObjectType("discObj", TypeSymbolValidationFlags.Default, "discKey", new[] { objectTypeA, objectTypeB });

            ResourceType resourceType = new ResourceType(
                ResourceTypeReference.Parse("microsoft.aadiam/azureADMetrics@2020-07-01-preview"),
                ResourceScope.ResourceGroup,
                discriminatedObjectType);

            IEnumerable<Snippet> snippets = snippetsProvider.GetResourceBodyCompletionSnippets(resourceType, isExistingResource: false, isResourceNested: false);

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
	name: 'keyA'
	location: $1
}$0");
                },
                x =>
                {
                    x.Prefix.Should().Be("required-properties-keyB");
                    x.Detail.Should().Be("Required properties");
                    x.CompletionPriority.Should().Be(CompletionPriority.Medium);
                    x.Text.Should().BeEquivalentToIgnoringNewlines(@"{
	name: $1
}$0");
                });
        }

        [TestMethod]
        public void GetModuleBodyCompletionSnippets_WithNoRequiredProperties_ShouldReturnEmptySnippet()
        {
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
}$0");
                });
        }

        [TestMethod]
        public void GetObjectBodyCompletionSnippets_WithNoRequiredProperties_ShouldReturnEmptySnippet()
        {
            var objectType = new ObjectType("objA", TypeSymbolValidationFlags.Default, new[]
            {
                new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.ReadOnly),
                new TypeProperty("location", LanguageConstants.String, TypePropertyFlags.WriteOnly),
                new TypeProperty("id", LanguageConstants.String)
            }, null);

            IEnumerable<Snippet> snippets = snippetsProvider.GetObjectBodyCompletionSnippets(objectType);

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
        public void GetObjectBodyCompletionSnippets_WithRequiredProperties_ShouldReturnEmptyAndRequiredPropertiesSnippets()
        {
            var objectType = new ObjectType("objA", TypeSymbolValidationFlags.Default, new[]
            {
                new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.Required),
                new TypeProperty("location", LanguageConstants.String, TypePropertyFlags.Required),
                new TypeProperty("id", LanguageConstants.String)
            }, null);

            IEnumerable<Snippet> snippets = snippetsProvider.GetObjectBodyCompletionSnippets(objectType);

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
}$0");
                });
        }

        [TestMethod]
        public void GetObjectBodyCompletionSnippets_WithDiscriminatedObjectTypeAndNoRequiredProperties_ShouldReturnEmptySnippet()
        {
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

            IEnumerable<Snippet> snippets = snippetsProvider.GetObjectBodyCompletionSnippets(discriminatedObjectType);

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
        public void GetObjectBodyCompletionSnippets_WithDiscriminatedObjectTypeAndRequiredProperties_ShouldReturnEmptyAndRequiredPropertiesSnippets()
        {
            var objectTypeA = new ObjectType("objA", TypeSymbolValidationFlags.Default, new[]
            {
                new TypeProperty("discKey", new StringLiteralType("keyA")),
                new TypeProperty("name", new StringLiteralType("keyA"), TypePropertyFlags.Required),
                new TypeProperty("location", LanguageConstants.String, TypePropertyFlags.Required),
                new TypeProperty("id", LanguageConstants.String)
            }, null);

            var objectTypeB = new ObjectType("objB", TypeSymbolValidationFlags.Default, new[]
            {
                new TypeProperty("discKey", new StringLiteralType("keyB")),
                new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.Required),
                new TypeProperty("kind", new StringLiteralType("discKey"), TypePropertyFlags.ReadOnly),
                new TypeProperty("hostPoolType", LanguageConstants.String)
            }, null);

            var discriminatedObjectType = new DiscriminatedObjectType("discObj", TypeSymbolValidationFlags.Default, "discKey", new[] { objectTypeA, objectTypeB });

            IEnumerable<Snippet> snippets = snippetsProvider.GetObjectBodyCompletionSnippets(discriminatedObjectType);

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
	name: 'keyA'
	location: $1
}$0");
                },
                x =>
                {
                    x.Prefix.Should().Be("required-properties-keyB");
                    x.Detail.Should().Be("Required properties");
                    x.CompletionPriority.Should().Be(CompletionPriority.Medium);
                    x.Text.Should().BeEquivalentToIgnoringNewlines(@"{
	name: $1
}$0");
                });
        }

        [DataTestMethod]
        [DataRow(null, null)]
        [DataRow("", "")]
        [DataRow("   ", "   ")]
        public void RemoveSnippetPlaceholderComments_WithInvalidInput_ReturnsInputTextAsIs(string input, string expected)
        {
            string actual = snippetsProvider.RemoveSnippetPlaceholderComments(input);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RemoveSnippetPlaceholderComments_WithoutMatchingSnippetPlaceholderCommentPatternInInput_ReturnsInputTextAsIs()
        {
            string input = @"resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'name'
  location: resourceGroup().location
}";

            string actual = snippetsProvider.RemoveSnippetPlaceholderComments(input);

            actual.Should().BeEquivalentToIgnoringNewlines(input);
        }

        [TestMethod]
        public void RemoveSnippetPlaceholderComments_WithMatchingSnippetPlaceholderCommentPatternInInput_RemovesSnippetPlaceholderComments()
        {
            string input = @"// DNS Record
resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: /*${1:'name'}*/'name'
  location: resourceGroup().location
}

resource /*${2:dnsRecord}*/dnsRecord 'Microsoft.Network/dnsZones//*${3|A,AAAA,CNAME,MX,NS,PTR,SOA,SRV,TXT|}*/A@2018-05-01' = {
  parent: dnsZone
  name: /*${4:'name'}*/'name'
  properties: {
    TTL: 3600
    mode: /*'${5|Detection,Prevention|}'*/'Detection'
    /*'hidden-related:${resourceGroup().id}/providers/Microsoft.Web/serverfarms/${6:'appServicePlan'}'*/'resource': 'Resource'
    '/*${7|ARecords,AAAARecords,MXRecords,NSRecords,PTRRecords,SRVRecords,TXTRecords,CNAMERecord,SOARecord|}*/ARecords': []
    precision: /*${8:-1}*/-1
    appSettings: [
    {
      name: 'AzureWebJobsDashboard'
      value: /*'DefaultEndpointsProtocol=https;AccountName=${4:storageAccountName1};AccountKey=${listKeys(${5:'storageAccountID1'}, '2019-06-01').key1}'*/'value'
    }
    id: /*$0*/
  }
}";

            string actual = snippetsProvider.RemoveSnippetPlaceholderComments(input);

            actual.Should().BeEquivalentToIgnoringNewlines(@"// DNS Record
resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: ${1:'name'}
  location: resourceGroup().location
}

resource ${2:dnsRecord} 'Microsoft.Network/dnsZones/${3|A,AAAA,CNAME,MX,NS,PTR,SOA,SRV,TXT|}@2018-05-01' = {
  parent: dnsZone
  name: ${4:'name'}
  properties: {
    TTL: 3600
    mode: '${5|Detection,Prevention|}'
    'hidden-related:${resourceGroup().id}/providers/Microsoft.Web/serverfarms/${6:'appServicePlan'}': 'Resource'
    '${7|ARecords,AAAARecords,MXRecords,NSRecords,PTRRecords,SRVRecords,TXTRecords,CNAMERecord,SOARecord|}': []
    precision: ${8:-1}
    appSettings: [
    {
      name: 'AzureWebJobsDashboard'
      value: 'DefaultEndpointsProtocol=https;AccountName=${4:storageAccountName1};AccountKey=${listKeys(${5:'storageAccountID1'}, '2019-06-01').key1}'
    }
    id: $0
  }
}");
        }


        private static ObjectType CreateObjectType(string name, params (string name, ITypeReference type, TypePropertyFlags typePropertyFlags)[] properties)
            => new(
                name,
                TypeSymbolValidationFlags.Default,
                properties.Select(val => new TypeProperty(val.name, val.type, val.typePropertyFlags)),
                null);
    }
}
