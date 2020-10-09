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