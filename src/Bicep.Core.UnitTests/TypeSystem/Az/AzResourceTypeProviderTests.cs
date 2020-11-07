// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Resources;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.SerializedTypes;
using Bicep.SerializedTypes.Az;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Core.UnitTests.TypeSystem.Az
{
    [TestClass]
    public class AzResourceTypeProviderTests
    {
        [TestMethod]
        public void AzResourceTypeProvider_can_deserialize_all_types_without_throwing()
        {
            var resourceTypeProvider = new AzResourceTypeProvider();
            var availableTypes = resourceTypeProvider.GetAvailableTypes();

            // sanity check - we know there should be a lot of types available
            availableTypes.Should().HaveCountGreaterThan(2000);

            foreach (var availableType in availableTypes)
            {
                resourceTypeProvider.HasType(availableType).Should().BeTrue();
                var knownResourceType = resourceTypeProvider.GetType(availableType);

                try
                {
                    var visited = new HashSet<TypeSymbol>();
                    VisitAllReachableTypes(knownResourceType, visited);
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
            availableTypes.Should().HaveCountGreaterThan(2000);
        }

        [TestMethod]
        public void AzResourceTypeProvider_should_warn_for_missing_resource_types()
        {
            
            var typeLoader = CreateMockTypeLoader(ResourceTypeReference.Parse("Mock.Rp/mockType@2020-01-01"));
            Compilation createCompilation(string program)
                => new Compilation(new AzResourceTypeProvider(typeLoader), SyntaxFactory.CreateFromText(program));

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
                => new Compilation(new AzResourceTypeProvider(typeLoader), SyntaxFactory.CreateFromText(program));

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
                ("BCP037", DiagnosticLevel.Error, "The property \"madeUpProperty\" is not allowed on objects of type \"Mock.Rp/mockType@2020-01-01\"."),
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
            var resourceType = deserializedType.OfType<SerializedTypes.Concrete.ResourceType>().Single();

            var mockTypeLocation = new TypeLocation();
            var mockTypeLoader = new Mock<ITypeLoader>();
            mockTypeLoader.Setup(x => x.ListAllAvailableTypes()).Returns(
                new Dictionary<string, TypeLocation>
                {
                    [resourceTypeReference.FormatName()] = mockTypeLocation,
                });
            mockTypeLoader.Setup(x => x.LoadResourceType(mockTypeLocation)).Returns(resourceType);

            return mockTypeLoader.Object;
        }

        private static string CreateSerializedTypes(ResourceTypeReference resourceTypeReference)
        {
            var typeFactory = new SerializedTypes.Concrete.TypeFactory(Enumerable.Empty<SerializedTypes.Concrete.TypeBase>());
            var stringType = typeFactory.Create(() => new SerializedTypes.Concrete.BuiltInType { Kind = SerializedTypes.Concrete.BuiltInTypeKind.String });
            var apiVersionType = typeFactory.Create(() => new SerializedTypes.Concrete.StringLiteralType { Value = resourceTypeReference.ApiVersion });
            var typeType = typeFactory.Create(() => new SerializedTypes.Concrete.StringLiteralType { Value = resourceTypeReference.FullyQualifiedType });
            var propertiesType = typeFactory.Create(() => new SerializedTypes.Concrete.ObjectType
            {
                Name = "Properties",
                Properties = new Dictionary<string, SerializedTypes.Concrete.ObjectProperty>
                {
                    ["readwrite"] = new SerializedTypes.Concrete.ObjectProperty { Type = typeFactory.GetReference(stringType), Flags = SerializedTypes.Concrete.ObjectPropertyFlags.None },
                    ["readonly"] = new SerializedTypes.Concrete.ObjectProperty { Type = typeFactory.GetReference(stringType), Flags = SerializedTypes.Concrete.ObjectPropertyFlags.ReadOnly },
                    ["writeonly"] = new SerializedTypes.Concrete.ObjectProperty { Type = typeFactory.GetReference(stringType), Flags = SerializedTypes.Concrete.ObjectPropertyFlags.WriteOnly },
                    ["required"] = new SerializedTypes.Concrete.ObjectProperty { Type = typeFactory.GetReference(stringType), Flags = SerializedTypes.Concrete.ObjectPropertyFlags.Required },
                }
            });
            var bodyType = typeFactory.Create(() => new SerializedTypes.Concrete.ObjectType
            {
                Name = resourceTypeReference.FormatName(),
                Properties = new Dictionary<string, SerializedTypes.Concrete.ObjectProperty>
                {
                    ["name"] = new SerializedTypes.Concrete.ObjectProperty { Type = typeFactory.GetReference(stringType), Flags = SerializedTypes.Concrete.ObjectPropertyFlags.DeployTimeConstant | SerializedTypes.Concrete.ObjectPropertyFlags.Required },
                    ["type"] = new SerializedTypes.Concrete.ObjectProperty { Type = typeFactory.GetReference(typeType), Flags = SerializedTypes.Concrete.ObjectPropertyFlags.DeployTimeConstant | SerializedTypes.Concrete.ObjectPropertyFlags.ReadOnly },
                    ["apiVersion"] = new SerializedTypes.Concrete.ObjectProperty { Type = typeFactory.GetReference(apiVersionType), Flags = SerializedTypes.Concrete.ObjectPropertyFlags.DeployTimeConstant | SerializedTypes.Concrete.ObjectPropertyFlags.ReadOnly },
                    ["id"] = new SerializedTypes.Concrete.ObjectProperty { Type = typeFactory.GetReference(stringType), Flags = SerializedTypes.Concrete.ObjectPropertyFlags.DeployTimeConstant | SerializedTypes.Concrete.ObjectPropertyFlags.ReadOnly },
                    ["properties"] = new SerializedTypes.Concrete.ObjectProperty { Type = typeFactory.GetReference(propertiesType), Flags = SerializedTypes.Concrete.ObjectPropertyFlags.Required },
                },
            });

            typeFactory.Create(() => new SerializedTypes.Concrete.ResourceType
            {
                Name = resourceTypeReference.FormatName(),
                Body = typeFactory.GetReference(bodyType),
            });

            return TypeSerializer.Serialize(typeFactory.GetTypes());
        }
    }
}