// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bicep.Core.Extensions;
using Moq;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics.Namespaces;
using System.Reflection;
using Bicep.Core.Resources;

namespace Bicep.Core.UnitTests.TypeSystem.Az
{
    [TestClass]
    public class AzResourceTypeProviderTests
    {
        private static ServiceBuilder Services => new ServiceBuilder();

        private static readonly ImmutableHashSet<string> ExpectedLoopVariantProperties = new[]
        {
            AzResourceTypeProvider.ResourceNamePropertyName,
            LanguageConstants.ResourceScopePropertyName,
            LanguageConstants.ResourceParentPropertyName
        }.ToImmutableHashSet(LanguageConstants.IdentifierComparer);

        private static NamespaceType GetAzNamespaceType()
        {
            var nsProvider = new DefaultNamespaceProvider(new AzResourceTypeLoader());

            return nsProvider.TryGetNamespace("az", "az", ResourceScope.ResourceGroup, BicepTestConstants.Features)!;
        }

        private static IEnumerable<object[]> GetDeserializeTestData()
        {
            var typesToTest = new[]
            {
                // it's too expensive to test all permutations.
                // keep this list short, but updated with a set of important providers.
                "microsoft.network/2022-05-01",
                "microsoft.compute/2022-08-01",
                "microsoft.documentdb/2022-08-15",
                "microsoft.storage/2022-05-01",
                "microsoft.web/2022-03-01",
            }.ToHashSet(StringComparer.OrdinalIgnoreCase);

            var flagPermutationsToTest = new [] {
                ResourceTypeGenerationFlags.None,
                ResourceTypeGenerationFlags.ExistingResource,
                ResourceTypeGenerationFlags.HasParentDefined,
                ResourceTypeGenerationFlags.NestedResource,
                ResourceTypeGenerationFlags.ExistingResource | ResourceTypeGenerationFlags.HasParentDefined,
            };

            foreach (var providerGrouping in GetAzNamespaceType().ResourceTypeProvider.GetAvailableTypes().GroupBy(x => x.TypeSegments[0])) {
                foreach (var apiVersionGrouping in providerGrouping.GroupBy(x => x.ApiVersion)) {
                    var providerName = providerGrouping.Key;
                    var apiVersion = apiVersionGrouping.Key!;

                    if (!typesToTest.Remove($"{providerName}/{apiVersion}"))
                    {
                        continue;
                    }

                    foreach (var flags in flagPermutationsToTest) {
                        var resourceTypes = apiVersionGrouping.Select(x => x.FormatName()).ToList();

                        yield return new object[] { providerName, apiVersion, flags, resourceTypes };
                    }
                }
            }

            typesToTest.Should().BeEmpty();
        }

        public static string GetDeserializeTestDisplayName(MethodInfo info, object[] values)
            => $"{info.Name} ({string.Join(',', new[] { values[0], values[1], values[2] }.Select(x => x.ToString()))})";

        [DataTestMethod]
        [DynamicData(nameof(GetDeserializeTestData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDeserializeTestDisplayName))]
        public void AzResourceTypeProvider_can_deserialize_all_types_without_throwing(string providerName, string apiVersion, ResourceTypeGenerationFlags flags, IReadOnlyList<string> resourceTypes)
        {
            // We deliberately load a new instance here for each test iteration rather than re-using an instance.
            // This is becase there are various internal caches which will consume too much memory and crash in CI if allowed to grow unrestricted.
            var azNamespaceType = GetAzNamespaceType();
            var resourceTypeProvider = azNamespaceType.ResourceTypeProvider;

            foreach (var availableType in resourceTypes)
            {
                var typeReference = ResourceTypeReference.Parse(availableType);

                resourceTypeProvider.HasDefinedType(typeReference).Should().BeTrue();
                var resourceType = resourceTypeProvider.TryGetDefinedType(azNamespaceType, typeReference, flags)!;

                try
                {
                    var visited = new HashSet<TypeSymbol>();
                    VisitAllReachableTypes(resourceType, visited);
                }
                catch (Exception exception)
                {
                    throw new InvalidOperationException($"Deserializing type {availableType} failed", exception);
                }

                bool IsSymbolicProperty(TypeProperty property)
                {
                    var type = property.TypeReference.Type;
                    return type is IScopeReference || type == LanguageConstants.ResourceOrResourceCollectionRefItem || type == LanguageConstants.ResourceOrResourceCollectionRefArray;
                }

                /*
                   This test is the most expensive one because it deserializes all the types.
                   Creating a separate test to add a bit of extra validation would basically double the runtime of the Az provider tests.
                 */
                {
                    // some types include a top-level scope property that is different than our own scope property
                    // so we need to filter by type
                    var topLevelProperties = GetTopLevelProperties(resourceType);
                    var symbolicProperties = topLevelProperties.Where(property => IsSymbolicProperty(property));
                    symbolicProperties.Should().NotBeEmpty();
                    symbolicProperties.Should().OnlyContain(property => property.Flags.HasFlag(TypePropertyFlags.DisallowAny), $"because all symbolic properties in type '{availableType}' should have the {nameof(TypePropertyFlags.DisallowAny)} flag.");

                    var loopVariantProperties = topLevelProperties.Where(property =>
                        ExpectedLoopVariantProperties.Contains(property.Name) &&
                        (!string.Equals(property.Name, LanguageConstants.ResourceScopePropertyName, LanguageConstants.IdentifierComparison) || IsSymbolicProperty(property)));
                    loopVariantProperties.Should().NotBeEmpty();
                    loopVariantProperties.Should().OnlyContain(property => property.Flags.HasFlag(TypePropertyFlags.LoopVariant), $"because all loop variant properties in type '{availableType}' should have the {nameof(TypePropertyFlags.LoopVariant)} flag.");

                    if (flags.HasFlag(ResourceTypeGenerationFlags.NestedResource))
                    {
                        // syntactically nested resources should not have the parent property
                        topLevelProperties.Should().NotContain(property => string.Equals(property.Name, LanguageConstants.ResourceParentPropertyName, LanguageConstants.IdentifierComparison));
                    }
                }
            }
        }

        [TestMethod]
        public void AzResourceTypeProvider_can_list_all_types_without_throwing()
        {
            var resourceTypeProvider = GetAzNamespaceType().ResourceTypeProvider;
            var availableTypes = resourceTypeProvider.GetAvailableTypes();

            // sanity check - we know there should be a lot of types available
            var minExpectedTypes = 3000;
            availableTypes.Should().HaveCountGreaterThan(minExpectedTypes);

            // verify there aren't any duplicates
            availableTypes.Select(x => x.FormatName().ToLowerInvariant()).Should().OnlyHaveUniqueItems();
        }

        [TestMethod]
        public void AzResourceTypeProvider_should_warn_for_missing_resource_types()
        {
            // Missing top-level properties - should be an error
            var compilation = Services.BuildCompilation(@"
resource missingResource 'Mock.Rp/madeUpResourceType@2020-01-01' = {
  name: 'missingResource'
}
");
            compilation.Should().HaveDiagnostics(new[] {
                ("BCP081", DiagnosticLevel.Warning, "Resource type \"Mock.Rp/madeUpResourceType@2020-01-01\" does not have types available.")
            });
        }

        [TestMethod]
        public void AzResourceTypeProvider_should_error_for_top_level_system_properties_and_warn_for_rest()
        {
            Compilation createCompilation(string program) => Services
                .WithAzResources(BuiltInTestTypes.Types)
                .BuildCompilation(program);

            // Missing top-level properties - should be an error
            var compilation = createCompilation(@"
resource missingRequired 'Test.Rp/readWriteTests@2020-01-01' = {
  properties: {
    required: 'hello!'
  }
}
");
            compilation.Should().HaveDiagnostics(new[] {
                ("BCP035", DiagnosticLevel.Error, "The specified \"resource\" declaration is missing the following required properties: \"name\".")
            });

            compilation = createCompilation(@"
resource missingRequired 'Test.Rp/readWriteTests@2020-01-01' = {
  name: 'missingRequired'
}
");
            compilation.Should().HaveDiagnostics(new[] {
                ("BCP035", DiagnosticLevel.Warning, "The specified \"resource\" declaration is missing the following required properties: \"properties\". If this is an inaccuracy in the documentation, please report it to the Bicep Team.")
            });

            // Top-level properties that aren't part of the type definition - should be an error
            compilation = createCompilation(@"
resource unexpectedTopLevel 'Test.Rp/readWriteTests@2020-01-01' = {
  name: 'unexpectedTopLevel'
  properties: {
    required: 'hello!'
  }
  madeUpProperty: true
}
");
            compilation.Should().HaveDiagnostics(new[] {
                ("BCP037", DiagnosticLevel.Error, "The property \"madeUpProperty\" is not allowed on objects of type \"Test.Rp/readWriteTests@2020-01-01\". Permissible properties include \"dependsOn\". If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
            });

            // Missing non top-level properties - should be a warning
            compilation = createCompilation(@"
resource missingRequiredProperty 'Test.Rp/readWriteTests@2020-01-01' = {
  name: 'missingRequiredProperty'
  properties: {
  }
}
");
            compilation.Should().HaveDiagnostics(new[] {
                ("BCP035", DiagnosticLevel.Warning, "The specified \"object\" declaration is missing the following required properties: \"required\". If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
            });

            // Non top-level properties that aren't part of the type definition - should be a warning
            compilation = createCompilation(@"
resource unexpectedPropertiesProperty 'Test.Rp/readWriteTests@2020-01-01' = {
  name: 'unexpectedPropertiesProperty'
  properties: {
    required: 'hello!'
    madeUpProperty: true
  }
}
");
            compilation.Should().HaveDiagnostics(new[] {
                ("BCP037", DiagnosticLevel.Warning, "The property \"madeUpProperty\" is not allowed on objects of type \"Properties\". Permissible properties include \"readwrite\", \"writeonly\". If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
            });
        }

        private static ImmutableHashSet<TypeSymbol> ExpectedBuiltInTypes { get; } = new[]
        {
            LanguageConstants.Any,
            LanguageConstants.Null,
            LanguageConstants.Bool,
            LanguageConstants.Int,
            LanguageConstants.String,
            LanguageConstants.Object,
            LanguageConstants.Array,
            LanguageConstants.ResourceRef,
        }.ToImmutableHashSet();

        private static IEnumerable<TypeProperty> GetTopLevelProperties(TypeSymbol type) => type switch
        {
            ResourceType resourceType => GetTopLevelProperties(resourceType.Body.Type),
            ObjectType objectType => objectType.Properties.Values,
            UnionType union => union.Members
                .SelectMany(member => GetTopLevelProperties(member.Type)),
            DiscriminatedObjectType discriminated => discriminated.DiscriminatorProperty
                .AsEnumerable()
                .Concat(discriminated.UnionMembersByKey.Values.SelectMany(member => GetTopLevelProperties(member))),

            _ => Enumerable.Empty<TypeProperty>()
        };

        private static void VisitAllReachableTypes(TypeSymbol typeSymbol, HashSet<TypeSymbol> visited)
        {
            if (visited.Contains(typeSymbol))
            {
                return;
            }
            visited.Add(typeSymbol);

            if (ExpectedBuiltInTypes.Contains(typeSymbol))
            {
                return;
            }

            switch (typeSymbol)
            {
                case ArrayType arrayType:
                    VisitAllReachableTypes(arrayType.Item.Type, visited);
                    return;
                case ObjectType objectType:
                    foreach (var property in objectType.Properties)
                    {
                        VisitAllReachableTypes(property.Value.TypeReference.Type, visited);
                    }
                    if (objectType.AdditionalPropertiesType != null)
                    {
                        VisitAllReachableTypes(objectType.AdditionalPropertiesType.Type, visited);
                    }
                    return;
                case ResourceType resourceType:
                    VisitAllReachableTypes(resourceType.Body.Type, visited);
                    return;
                case UnionType unionType:
                    foreach (var member in unionType.Members)
                    {
                        VisitAllReachableTypes(member.Type, visited);
                    }
                    return;
                case StringLiteralType stringLiteralType:
                    return;
                case DiscriminatedObjectType discriminatedObjectType:
                    return;
            }
        }
    }
}
