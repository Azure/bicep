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

namespace Bicep.Core.UnitTests.TypeSystem.Az
{
    [TestClass]
    public class AzResourceTypeProviderTests
    {
        [TestMethod]
        public void AzResourceTypeProvider_can_deserialize_all_types_without_throwing()
        {
            var resourceTypeProvider = new AzResourceTypeProvider();

            var knownTypes = GetAllKnownTypes().ToImmutableArray();

            // sanity check - we know there should be a lot of types available
            knownTypes.Should().HaveCountGreaterThan(500);

            foreach (var knownType in knownTypes)
            {
                resourceTypeProvider.HasType(knownType).Should().BeTrue();
                var knownResourceType = resourceTypeProvider.GetType(knownType);

                try
                {
                    var visited = new HashSet<TypeSymbol>();
                    VisitAllReachableTypes(knownResourceType, visited);
                }
                catch (Exception exception)
                {
                    throw new InvalidOperationException($"Deserializing type {knownType.FormatName()} failed", exception);
                }
            }
        }

        [TestMethod]
        public void AzResourceTypeProvider_should_error_for_top_level_and_warn_for_nested_properties()
        {
            IEnumerable<SerializedTypes.Concrete.TypeBase> loadTypes(string providerName, string apiVersion)
            {
                if (providerName == "Mock.Rp" && apiVersion == "2020-01-01")
                {
                    var serializedTypes = CreateSerializedTypes("Mock.Rp", "mockType", "2020-01-01");

                    return TypeSerializer.Deserialize(serializedTypes);
                }

                return Enumerable.Empty<SerializedTypes.Concrete.TypeBase>();
            }

            var compilationHelper = new CompilationHelper(program => new Compilation(new AzResourceTypeProvider(loadTypes), SyntaxFactory.CreateFromText(program)));

            compilationHelper.ProgramShouldHaveDiagnostics(@"
resource missingRequired 'Mock.Rp/mockType@2020-01-01' = {
  name: 'missingRequired'
}
",
                ("BCP035", DiagnosticLevel.Error, "The specified object is missing the following required properties: \"properties\"."));

            compilationHelper.ProgramShouldHaveDiagnostics(@"
resource unexpectedTopLevel 'Mock.Rp/mockType@2020-01-01' = {
  name: 'unexpectedTopLevel'
  properties: {
    required: 'hello!'
  }
  madeUpProperty: true
}
",
                ("BCP037", DiagnosticLevel.Error, "The property \"madeUpProperty\" is not allowed on objects of type \"Mock.Rp/mockType@2020-01-01\"."));

            compilationHelper.ProgramShouldHaveDiagnostics(@"
resource missingRequiredProperty 'Mock.Rp/mockType@2020-01-01' = {
  name: 'missingRequiredProperty'
  properties: {    
  }
}
",
                ("BCP035", DiagnosticLevel.Warning, "The specified object is missing the following required properties: \"required\"."));

            compilationHelper.ProgramShouldHaveDiagnostics(@"
resource unexpectedPropertiesProperty 'Mock.Rp/mockType@2020-01-01' = {
  name: 'unexpectedPropertiesProperty'
  properties: {
    required: 'hello!'
    madeUpProperty: true
  }
}
",
                ("BCP038", DiagnosticLevel.Warning, "The property \"madeUpProperty\" is not allowed on objects of type \"Properties\". Permissible properties include \"readwrite\", \"writeonly\"."));
        }

        private static IEnumerable<ResourceTypeReference> GetAllKnownTypes()
        {
            // There's no index of types available via TypeLoader yet. When that's been added, this can be removed.
            foreach (var fileName in typeof(TypeLoader).Assembly.GetManifestResourceNames())
            {
                var splitPath = fileName.Split('/');
                if (splitPath.Length != 3 || splitPath[2] != "types.json")
                {
                    throw new InvalidOperationException($"Found unexpected manifest file {fileName}");
                }

                var providerName = splitPath[0];
                var apiVersion = splitPath[1];

                foreach (var type in TypeLoader.LoadTypes(providerName, apiVersion))
                {
                    if (!(type is SerializedTypes.Concrete.ResourceType concreteResourceType))
                    {
                        continue;
                    }

                    yield return ResourceTypeReference.Parse(concreteResourceType.Name!);
                }
            }
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

        private static string CreateSerializedTypes(string providerName, string resourceType, string apiVersion)
        {
            var resourceName = $"{providerName}/{resourceType}@{apiVersion}";
            var typeFactory = new SerializedTypes.Concrete.TypeFactory(Enumerable.Empty<SerializedTypes.Concrete.TypeBase>());
            var stringType = typeFactory.Create(() => new SerializedTypes.Concrete.BuiltInType { Kind = SerializedTypes.Concrete.BuiltInTypeKind.String });
            var apiVersionType = typeFactory.Create(() => new SerializedTypes.Concrete.StringLiteralType { Value = apiVersion });
            var typeType = typeFactory.Create(() => new SerializedTypes.Concrete.StringLiteralType { Value = $"{providerName}/{resourceType}" });
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
                Name = resourceName,
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
                Name = resourceName,
                Body = typeFactory.GetReference(bodyType),
            });

            return TypeSerializer.Serialize(typeFactory.GetTypes());
        }
    }
}