// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
using Bicep.Core.Configuration;
using Bicep.Core.Semantics.Namespaces;

namespace Bicep.Core.UnitTests.TypeSystem.Az
{
    [TestClass]
    public class AzResourceTypeProviderTests
    {
        private static readonly ImmutableHashSet<string> ExpectedLoopVariantProperties = new[]
        {
            LanguageConstants.ResourceNamePropertyName,
            LanguageConstants.ResourceScopePropertyName,
            LanguageConstants.ResourceParentPropertyName
        }.ToImmutableHashSet(LanguageConstants.IdentifierComparer);

        [DataTestMethod]
        [DataRow(ResourceTypeGenerationFlags.None)]
        [DataRow(ResourceTypeGenerationFlags.ExistingResource)]
        [DataRow(ResourceTypeGenerationFlags.PermitLiteralNameProperty)]
        [DataRow(ResourceTypeGenerationFlags.NestedResource)]
        [DataRow(ResourceTypeGenerationFlags.ExistingResource | ResourceTypeGenerationFlags.PermitLiteralNameProperty)]
        public void AzResourceTypeProvider_can_deserialize_all_types_without_throwing(ResourceTypeGenerationFlags flags)
        {
            var resourceTypeProvider = new AzResourceTypeProvider(new AzResourceTypeLoader());
            var availableTypes = resourceTypeProvider.GetAvailableTypes();

            // sanity check - we know there should be a lot of types available
            var expectedTypeCount = 3000;
            availableTypes.Should().HaveCountGreaterThan(expectedTypeCount);

            foreach (var availableType in availableTypes)
            {
                resourceTypeProvider.HasDefinedType(availableType).Should().BeTrue();
                var resourceType = resourceTypeProvider.TryGetDefinedType(availableType, flags)!;

                try
                {
                    var visited = new HashSet<TypeSymbol>();
                    VisitAllReachableTypes(resourceType, visited);
                }
                catch (Exception exception)
                {
                    throw new InvalidOperationException($"Deserializing type {availableType.FormatName()} failed", exception);
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
                    symbolicProperties.Should().OnlyContain(property => property.Flags.HasFlag(TypePropertyFlags.DisallowAny), $"because all symbolic properties in type '{availableType.FullyQualifiedType}' and api version '{availableType.ApiVersion}' should have the {nameof(TypePropertyFlags.DisallowAny)} flag.");

                    var loopVariantProperties = topLevelProperties.Where(property =>
                        ExpectedLoopVariantProperties.Contains(property.Name) &&
                        (!string.Equals(property.Name, LanguageConstants.ResourceScopePropertyName, LanguageConstants.IdentifierComparison) || IsSymbolicProperty(property)));
                    loopVariantProperties.Should().NotBeEmpty();
                    loopVariantProperties.Should().OnlyContain(property => property.Flags.HasFlag(TypePropertyFlags.LoopVariant), $"because all loop variant properties in type '{availableType.FullyQualifiedType}' and api version '{availableType.ApiVersion}' should have the {nameof(TypePropertyFlags.LoopVariant)} flag.");

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
            var resourceTypeProvider = new AzResourceTypeProvider(new AzResourceTypeLoader());
            var availableTypes = resourceTypeProvider.GetAvailableTypes();

            // sanity check - we know there should be a lot of types available
            var expectedTypeCount = 3000;
            availableTypes.Should().HaveCountGreaterThan(expectedTypeCount);
        }

        [TestMethod]
        public void AzResourceTypeProvider_should_warn_for_missing_resource_types()
        {
            var configuration = BicepTestConstants.BuiltInConfigurationWithAnalyzersDisabled;
            Compilation createCompilation(string program)
                    => new Compilation(new DefaultNamespaceProvider(new AzResourceTypeLoader(), BicepTestConstants.Features), SourceFileGroupingFactory.CreateFromText(program, new Mock<IFileResolver>(MockBehavior.Strict).Object), configuration);

            // Missing top-level properties - should be an error
            var compilation = createCompilation(@"
resource missingResource 'Mock.Rp/madeUpResourceType@2020-01-01' = {
  name: 'missingResource'
}
");
            compilation.Should().HaveDiagnostics(new[] {
                ("BCP081", DiagnosticLevel.Warning, "Resource type \"Mock.Rp/madeUpResourceType@2020-01-01\" does not have types available.")
            });
        }

        [TestMethod]
        public void AzResourceTypeProvider_should_error_for_top_level_and_warn_for_nested_properties()
        {
            Compilation createCompilation(string program)
                => new Compilation(BuiltInTestTypes.Create(), SourceFileGroupingFactory.CreateFromText(program, new Mock<IFileResolver>(MockBehavior.Strict).Object), BicepTestConstants.BuiltInConfiguration);

            // Missing top-level properties - should be an error
            var compilation = createCompilation(@"
resource missingRequired 'Test.Rp/readWriteTests@2020-01-01' = {
  name: 'missingRequired'
}
");
            compilation.Should().HaveDiagnostics(new[] {
                ("BCP035", DiagnosticLevel.Error, "The specified \"resource\" declaration is missing the following required properties: \"properties\".")
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
                ("BCP035", DiagnosticLevel.Warning, "The specified \"object\" declaration is missing the following required properties: \"required\"."),
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
