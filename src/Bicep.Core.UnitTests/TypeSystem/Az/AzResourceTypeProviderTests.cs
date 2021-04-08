// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Az;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Core.UnitTests.TypeSystem.Az
{
    [TestClass]
    public class AzResourceTypeProviderTests
    {
        [DataTestMethod]
        [DataRow(ResourceTypeGenerationFlags.None)]
        [DataRow(ResourceTypeGenerationFlags.ExistingResource)]
        [DataRow(ResourceTypeGenerationFlags.PermitLiteralNameProperty)]
        [DataRow(ResourceTypeGenerationFlags.ExistingResource | ResourceTypeGenerationFlags.PermitLiteralNameProperty)]
        public void AzResourceTypeProvider_can_deserialize_all_types_without_throwing(ResourceTypeGenerationFlags flags)
        {
            var resourceTypeProvider = new AzResourceTypeProvider();
            var availableTypes = resourceTypeProvider.GetAvailableTypes();

            // sanity check - we know there should be a lot of types available
            var expectedTypeCount = 3000;
            availableTypes.Should().HaveCountGreaterThan(expectedTypeCount);

            foreach (var availableType in availableTypes)
            {
                resourceTypeProvider.HasType(availableType).Should().BeTrue();
                var resourceType = resourceTypeProvider.GetType(availableType, flags);

                try
                {
                    var visited = new HashSet<TypeSymbol>();
                    VisitAllReachableTypes(resourceType, visited);
                }
                catch (Exception exception)
                {
                    throw new InvalidOperationException($"Deserializing type {availableType.FormatName()} failed", exception);
                }
            }
        }

        [TestMethod]
        public void AzResourceTypeProvider_can_list_all_types_without_throwing()
        
        {
            var resourceTypeProvider = new AzResourceTypeProvider();
            var availableTypes = resourceTypeProvider.GetAvailableTypes();

            // sanity check - we know there should be a lot of types available
            var expectedTypeCount = 3000;
            availableTypes.Should().HaveCountGreaterThan(expectedTypeCount);
        }

        [TestMethod]
        public void AzResourceTypeProvider_should_warn_for_missing_resource_types()
        {
            
            var typeLoader = CreateMockTypeLoader(ResourceTypeReference.Parse("Mock.Rp/mockType@2020-01-01"));
            Compilation createCompilation(string program)
                => new Compilation(new AzResourceTypeProvider(typeLoader), SyntaxTreeGroupingFactory.CreateFromText(program));

            // Missing top-level properties - should be an error
            var compilation = createCompilation(@"
resource missingResource 'Mock.Rp/madeUpResourceType@2020-01-01' = {
  name: 'missingResource'
}
");
            compilation.Should().HaveDiagnostics(new [] {
                ("BCP081", DiagnosticLevel.Warning, "Resource type \"Mock.Rp/madeUpResourceType@2020-01-01\" does not have types available.")
            });
        }

        [TestMethod]
        public void AzResourceTypeProvider_should_error_for_top_level_and_warn_for_nested_properties()
        {
            var typeLoader = CreateMockTypeLoader(ResourceTypeReference.Parse("Mock.Rp/mockType@2020-01-01"));
            Compilation createCompilation(string program)
                => new Compilation(new AzResourceTypeProvider(typeLoader), SyntaxTreeGroupingFactory.CreateFromText(program));

            // Missing top-level properties - should be an error
            var compilation = createCompilation(@"
resource missingRequired 'Mock.Rp/mockType@2020-01-01' = {
  name: 'missingRequired'
}
");
            compilation.Should().HaveDiagnostics(new [] {
                ("BCP035", DiagnosticLevel.Error, "The specified \"resource\" declaration is missing the following required properties: \"properties\".")
            });

            // Top-level properties that aren't part of the type definition - should be an error
            compilation = createCompilation(@"
resource unexpectedTopLevel 'Mock.Rp/mockType@2020-01-01' = {
  name: 'unexpectedTopLevel'
  properties: {
    required: 'hello!'
  }
  madeUpProperty: true
}
");
            compilation.Should().HaveDiagnostics(new [] {
                ("BCP038", DiagnosticLevel.Error, "The property \"madeUpProperty\" is not allowed on objects of type \"Mock.Rp/mockType@2020-01-01\". Permissible properties include \"dependsOn\"."),
            });

            // Missing non top-level properties - should be a warning
            compilation = createCompilation(@"
resource missingRequiredProperty 'Mock.Rp/mockType@2020-01-01' = {
  name: 'missingRequiredProperty'
  properties: {    
  }
}
");
            compilation.Should().HaveDiagnostics(new [] {
                ("BCP035", DiagnosticLevel.Warning, "The specified \"object\" declaration is missing the following required properties: \"required\"."),
            });

            // Non top-level properties that aren't part of the type definition - should be a warning
            compilation = createCompilation(@"
resource unexpectedPropertiesProperty 'Mock.Rp/mockType@2020-01-01' = {
  name: 'unexpectedPropertiesProperty'
  properties: {
    required: 'hello!'
    madeUpProperty: true
  }
}
");
            compilation.Should().HaveDiagnostics(new [] {
                ("BCP038", DiagnosticLevel.Warning, "The property \"madeUpProperty\" is not allowed on objects of type \"Properties\". Permissible properties include \"readwrite\", \"writeonly\"."),
            });
        }

        private static ImmutableHashSet<TypeSymbol> ExpectedBuiltInTypes { get; } = new []
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

        private static ITypeLoader CreateMockTypeLoader(ResourceTypeReference resourceTypeReference)
        {
            var serializedTypes = CreateSerializedTypes(resourceTypeReference);
            var deserializedType = TypeSerializer.Deserialize(serializedTypes);
            var resourceType = deserializedType.OfType<Azure.Bicep.Types.Concrete.ResourceType>().Single();

            var mockTypeLocation = new TypeLocation();
            var mockTypeLoader = new Mock<ITypeLoader>();
            var resourceTypes = new Dictionary<string, TypeLocation>
            {
                [resourceTypeReference.FormatName()] = mockTypeLocation,
            };
            mockTypeLoader.Setup(x => x.GetIndexedTypes()).Returns(new Azure.Bicep.Types.Az.Index.TypeIndex(resourceTypes));
            mockTypeLoader.Setup(x => x.LoadResourceType(mockTypeLocation)).Returns(resourceType);

            return mockTypeLoader.Object;
        }

        private static string CreateSerializedTypes(ResourceTypeReference resourceTypeReference)
        {
            var typeFactory = new Azure.Bicep.Types.Concrete.TypeFactory(Enumerable.Empty<Azure.Bicep.Types.Concrete.TypeBase>());
            var stringType = typeFactory.Create(() => new Azure.Bicep.Types.Concrete.BuiltInType(Azure.Bicep.Types.Concrete.BuiltInTypeKind.String));
            var apiVersionType = typeFactory.Create(() => new Azure.Bicep.Types.Concrete.StringLiteralType(resourceTypeReference.ApiVersion));
            var typeType = typeFactory.Create(() => new Azure.Bicep.Types.Concrete.StringLiteralType(resourceTypeReference.FullyQualifiedType));
            var propertiesType = typeFactory.Create(() => new Azure.Bicep.Types.Concrete.ObjectType(
                "Properties",
                new Dictionary<string, Azure.Bicep.Types.Concrete.ObjectProperty>
                {
                    ["readwrite"] = new Azure.Bicep.Types.Concrete.ObjectProperty(typeFactory.GetReference(stringType), Azure.Bicep.Types.Concrete.ObjectPropertyFlags.None, "readwrite property"),
                    ["readonly"] = new Azure.Bicep.Types.Concrete.ObjectProperty(typeFactory.GetReference(stringType), Azure.Bicep.Types.Concrete.ObjectPropertyFlags.ReadOnly, "readonly property"),
                    ["writeonly"] = new Azure.Bicep.Types.Concrete.ObjectProperty(typeFactory.GetReference(stringType), Azure.Bicep.Types.Concrete.ObjectPropertyFlags.WriteOnly, "writeonly property"),
                    ["required"] = new Azure.Bicep.Types.Concrete.ObjectProperty(typeFactory.GetReference(stringType), Azure.Bicep.Types.Concrete.ObjectPropertyFlags.Required, "required property"),
                },
                null));
            var bodyType = typeFactory.Create(() => new Azure.Bicep.Types.Concrete.ObjectType(
                resourceTypeReference.FormatName(),
                new Dictionary<string, Azure.Bicep.Types.Concrete.ObjectProperty>
                {
                    ["name"] = new Azure.Bicep.Types.Concrete.ObjectProperty(typeFactory.GetReference(stringType), Azure.Bicep.Types.Concrete.ObjectPropertyFlags.DeployTimeConstant | Azure.Bicep.Types.Concrete.ObjectPropertyFlags.Required, "name property"),
                    ["type"] = new Azure.Bicep.Types.Concrete.ObjectProperty(typeFactory.GetReference(typeType), Azure.Bicep.Types.Concrete.ObjectPropertyFlags.DeployTimeConstant | Azure.Bicep.Types.Concrete.ObjectPropertyFlags.ReadOnly, "type property"),
                    ["apiVersion"] = new Azure.Bicep.Types.Concrete.ObjectProperty(typeFactory.GetReference(apiVersionType), Azure.Bicep.Types.Concrete.ObjectPropertyFlags.DeployTimeConstant | Azure.Bicep.Types.Concrete.ObjectPropertyFlags.ReadOnly, "apiVersion property"),
                    ["id"] = new Azure.Bicep.Types.Concrete.ObjectProperty(typeFactory.GetReference(stringType), Azure.Bicep.Types.Concrete.ObjectPropertyFlags.DeployTimeConstant | Azure.Bicep.Types.Concrete.ObjectPropertyFlags.ReadOnly, "id property"),
                    ["properties"] = new Azure.Bicep.Types.Concrete.ObjectProperty(typeFactory.GetReference(propertiesType), Azure.Bicep.Types.Concrete.ObjectPropertyFlags.Required, "properties property"),
                },
                null));

            typeFactory.Create(() => new Azure.Bicep.Types.Concrete.ResourceType(
                resourceTypeReference.FormatName(),
                Azure.Bicep.Types.Concrete.ScopeType.ResourceGroup,
                typeFactory.GetReference(bodyType)));

            return TypeSerializer.Serialize(typeFactory.GetTypes());
        }
    }
}