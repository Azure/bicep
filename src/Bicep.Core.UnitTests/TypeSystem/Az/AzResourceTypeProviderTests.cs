// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
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

        public static IEnumerable<ResourceTypeReference> GetAllKnownTypes()
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

        public void VisitAllReachableTypes(TypeSymbol typeSymbol, HashSet<TypeSymbol> visited)
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