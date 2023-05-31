// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Bicep.Core;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Snippets;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.UnitTests.Snippets
{
    [TestClass]
    public class SnippetsProviderTests
    {
        private ISnippetsProvider CreateSnippetsProvider()
            => ServiceBuilder.Create(s => s.AddSingleton<SnippetsProvider>()).Construct<SnippetsProvider>();

        private readonly NamespaceType azNamespaceType = BicepTestConstants.NamespaceProvider.TryGetNamespace("az", "az", ResourceScope.ResourceGroup, BicepTestConstants.Features)!;

        [TestMethod]
        public void CompletionPriorityOfResourceSnippets_ShouldBeHigh()
        {
            IEnumerable<Snippet> snippets = CreateSnippetsProvider().GetTopLevelNamedDeclarationSnippets()
                .Where(x => x.Prefix.StartsWith("resource"));

            foreach (Snippet snippet in snippets)
            {
                snippet.CompletionPriority.Should().Be(CompletionPriority.High);
            }
        }


        [TestMethod]
        public void CompletionPriorityOfNonResourceSnippets_ShouldBeMedium()
        {
            IEnumerable<Snippet> snippets = CreateSnippetsProvider().GetTopLevelNamedDeclarationSnippets()
       .Where(x => !x.Prefix.StartsWith("resource"));

            foreach (Snippet snippet in snippets)
            {
                snippet.CompletionPriority.Should().Be(CompletionPriority.Medium);
            }
        }

        [TestMethod]
        public void GetResourceBodyCompletionSnippets_WithStaticTemplateAndNoResourceDependencies_ShouldReturnSnippets()
        {
            ResourceType resourceType = new ResourceType(
                azNamespaceType,
                ResourceTypeReference.Parse("Microsoft.DataLakeStore/accounts@2016-11-01"),
                ResourceScope.ResourceGroup,
                ResourceScope.None,
                ResourceFlags.None,
                CreateObjectType(
                    "Microsoft.DataLakeStore/accounts@2016-11-01",
                    ("name", LanguageConstants.String, TypePropertyFlags.Required),
                    ("location", LanguageConstants.String, TypePropertyFlags.Required)),
                AzResourceTypeProvider.UniqueIdentifierProperties);

            IEnumerable<Snippet> snippets = CreateSnippetsProvider().GetResourceBodyCompletionSnippets(resourceType, isExistingResource: false, isResourceNested: false);

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
  location: ${3:location}
  properties: {
    newTier: ${4|'Consumption','Commitment_1TB','Commitment_10TB','Commitment_100TB','Commitment_500TB','Commitment_1PB','Commitment_5PB'|}
    encryptionState: ${5|'Enabled','Disabled'|}
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
                azNamespaceType,
                ResourceTypeReference.Parse("Microsoft.Automation/automationAccounts/modules@2019-06-01"),
                ResourceScope.ResourceGroup,
                ResourceScope.None,
                ResourceFlags.None,
                CreateObjectType(
                    "Microsoft.Automation/automationAccounts/modules@2015-10-31",
                    ("name", LanguageConstants.String, TypePropertyFlags.Required),
                    ("location", LanguageConstants.String, TypePropertyFlags.Required)),
                AzResourceTypeProvider.UniqueIdentifierProperties);

            IEnumerable<Snippet> snippets = CreateSnippetsProvider().GetResourceBodyCompletionSnippets(resourceType, isExistingResource: false, isResourceNested: false);

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
resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
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
                azNamespaceType,
                ResourceTypeReference.Parse("Microsoft.Automation/automationAccounts/certificates@2019-06-01"),
                ResourceScope.ResourceGroup,
                ResourceScope.None,
                ResourceFlags.None,
                CreateObjectType(
                    "Microsoft.Automation/automationAccounts/certificates@2019-06-01",
                    ("name", LanguageConstants.String, TypePropertyFlags.Required),
                    ("location", LanguageConstants.String, TypePropertyFlags.Required)),
                AzResourceTypeProvider.UniqueIdentifierProperties);

            IEnumerable<Snippet> snippets = CreateSnippetsProvider().GetResourceBodyCompletionSnippets(resourceType, isExistingResource: false, isResourceNested: true);

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
                azNamespaceType,
                ResourceTypeReference.Parse("microsoft.aadiam/azureADMetrics@2020-07-01-preview"),
                ResourceScope.ResourceGroup,
                ResourceScope.None,
                ResourceFlags.None,
                CreateObjectType(
                    "microsoft.aadiam/azureADMetrics@2020-07-01-preview",
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
                        TypePropertyFlags.Required)),
                AzResourceTypeProvider.UniqueIdentifierProperties);

            IEnumerable<Snippet> snippets = CreateSnippetsProvider().GetResourceBodyCompletionSnippets(resourceType, isExistingResource: false, isResourceNested: false);

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
                azNamespaceType,
                ResourceTypeReference.Parse("microsoft.aadiam/azureADMetrics@2020-07-01-preview"),
                ResourceScope.ResourceGroup,
                ResourceScope.None,
                ResourceFlags.None,
                CreateObjectType("microsoft.aadiam/azureADMetrics@2020-07-01-preview"),
                AzResourceTypeProvider.UniqueIdentifierProperties);

            IEnumerable<Snippet> snippets = CreateSnippetsProvider().GetResourceBodyCompletionSnippets(resourceType, isExistingResource: false, isResourceNested: false);

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

            IEnumerable<Snippet> snippets = CreateSnippetsProvider().GetNestedResourceDeclarationSnippets(resourceTypeReference);

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
                    x.Prefix.Should().Be("res-automation-module");
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

            IEnumerable<Snippet> snippets = CreateSnippetsProvider().GetNestedResourceDeclarationSnippets(resourceTypeReference);

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
                new TypeProperty("discKey", TypeFactory.CreateStringLiteralType("keyA")),
                new TypeProperty("keyAProp", LanguageConstants.String),
            }, null);

            var objectTypeB = new ObjectType("objB", TypeSymbolValidationFlags.Default, new[]
            {
                new TypeProperty("discKey", TypeFactory.CreateStringLiteralType("keyB")),
                new TypeProperty("keyBProp", LanguageConstants.String),
            }, null);

            var discriminatedObjectType = new DiscriminatedObjectType("discObj", TypeSymbolValidationFlags.Default, "discKey", new[] { objectTypeA, objectTypeB });

            ResourceType resourceType = new ResourceType(
                azNamespaceType,
                ResourceTypeReference.Parse("microsoft.aadiam/azureADMetrics@2020-07-01-preview"),
                ResourceScope.ResourceGroup,
                ResourceScope.None,
                ResourceFlags.None,
                discriminatedObjectType,
                AzResourceTypeProvider.UniqueIdentifierProperties);

            IEnumerable<Snippet> snippets = CreateSnippetsProvider().GetResourceBodyCompletionSnippets(resourceType, isExistingResource: false, isResourceNested: false);

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
                new TypeProperty("discKey", TypeFactory.CreateStringLiteralType("keyA")),
                new TypeProperty("name", TypeFactory.CreateStringLiteralType("keyA"), TypePropertyFlags.Required),
                new TypeProperty("location", LanguageConstants.String, TypePropertyFlags.Required),
                new TypeProperty("id", LanguageConstants.String)
            }, null);

            var objectTypeB = new ObjectType("objB", TypeSymbolValidationFlags.Default, new[]
            {
                new TypeProperty("discKey", TypeFactory.CreateStringLiteralType("keyB")),
                new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.Required),
                new TypeProperty("kind", TypeFactory.CreateStringLiteralType("discKey"), TypePropertyFlags.ReadOnly),
                new TypeProperty("hostPoolType", LanguageConstants.String)
            }, null);

            var discriminatedObjectType = new DiscriminatedObjectType("discObj", TypeSymbolValidationFlags.Default, "discKey", new[] { objectTypeA, objectTypeB });

            ResourceType resourceType = new ResourceType(
                azNamespaceType,
                ResourceTypeReference.Parse("microsoft.aadiam/azureADMetrics@2020-07-01-preview"),
                ResourceScope.ResourceGroup,
                ResourceScope.None,
                ResourceFlags.None,
                discriminatedObjectType,
                AzResourceTypeProvider.UniqueIdentifierProperties);

            IEnumerable<Snippet> snippets = CreateSnippetsProvider().GetResourceBodyCompletionSnippets(resourceType, isExistingResource: false, isResourceNested: false);

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

            IEnumerable<Snippet> snippets = CreateSnippetsProvider().GetModuleBodyCompletionSnippets(typeSymbol);

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

            IEnumerable<Snippet> snippets = CreateSnippetsProvider().GetModuleBodyCompletionSnippets(typeSymbol);

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

            IEnumerable<Snippet> snippets = CreateSnippetsProvider().GetObjectBodyCompletionSnippets(objectType);

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

            IEnumerable<Snippet> snippets = CreateSnippetsProvider().GetObjectBodyCompletionSnippets(objectType);

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
                new TypeProperty("discKey", TypeFactory.CreateStringLiteralType("keyA")),
                new TypeProperty("keyAProp", LanguageConstants.String),
            }, null);

            var objectTypeB = new ObjectType("objB", TypeSymbolValidationFlags.Default, new[]
            {
                new TypeProperty("discKey", TypeFactory.CreateStringLiteralType("keyB")),
                new TypeProperty("keyBProp", LanguageConstants.String),
            }, null);

            var discriminatedObjectType = new DiscriminatedObjectType("discObj", TypeSymbolValidationFlags.Default, "discKey", new[] { objectTypeA, objectTypeB });

            IEnumerable<Snippet> snippets = CreateSnippetsProvider().GetObjectBodyCompletionSnippets(discriminatedObjectType);

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
                new TypeProperty("discKey", TypeFactory.CreateStringLiteralType("keyA")),
                new TypeProperty("name", TypeFactory.CreateStringLiteralType("keyA"), TypePropertyFlags.Required),
                new TypeProperty("location", LanguageConstants.String, TypePropertyFlags.Required),
                new TypeProperty("id", LanguageConstants.String)
            }, null);

            var objectTypeB = new ObjectType("objB", TypeSymbolValidationFlags.Default, new[]
            {
                new TypeProperty("discKey", TypeFactory.CreateStringLiteralType("keyB")),
                new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.Required),
                new TypeProperty("kind", TypeFactory.CreateStringLiteralType("discKey"), TypePropertyFlags.ReadOnly),
                new TypeProperty("hostPoolType", LanguageConstants.String)
            }, null);

            var discriminatedObjectType = new DiscriminatedObjectType("discObj", TypeSymbolValidationFlags.Default, "discKey", new[] { objectTypeA, objectTypeB });

            IEnumerable<Snippet> snippets = CreateSnippetsProvider().GetObjectBodyCompletionSnippets(discriminatedObjectType);

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


        private static ObjectType CreateObjectType(string name, params (string name, ITypeReference type, TypePropertyFlags typePropertyFlags)[] properties)
            => new(
                name,
                TypeSymbolValidationFlags.Default,
                properties.Select(val => new TypeProperty(val.name, val.type, val.typePropertyFlags)),
                null);
    }
}
