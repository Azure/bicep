// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;

namespace Bicep.Core
{
    public static class LanguageConstants
    {
        public const int MaxParameterCount = 256;
        public const int MaxIdentifierLength = 255;

        public const string ListSeparator = ", ";

        public const string ParameterKeyword = "param";
        public const string OutputKeyword = "output";
        public const string VariableKeyword = "var";
        public const string ResourceKeyword = "resource";
        public const string ModuleKeyword = "module";

        public static ImmutableSortedSet<string> DeclarationKeywords = new[] {ParameterKeyword, VariableKeyword, ResourceKeyword, OutputKeyword, ModuleKeyword}.ToImmutableSortedSet(StringComparer.Ordinal);

        public const string ParameterAllowedPropertyName = "allowed";
        public const string ParameterDefaultPropertyName = "default";

        public static readonly StringComparer IdentifierComparer = StringComparer.Ordinal;
        public static readonly StringComparison IdentifierComparison = StringComparison.Ordinal;

        public const string StringDelimiter = "'";
        public const string StringHoleOpen = "${";
        public const string StringHoleClose = "}";

        public static readonly TypeSymbol Any = new AnyType();
        public static readonly TypeSymbol ResourceRef = new ResourceRefType();
        public static readonly TypeSymbol String = new PrimitiveType("string", TypeSymbolValidationFlags.Default);
        // LooseString should be regarded as equal to the 'string' type, but with different validation behavior
        public static readonly TypeSymbol LooseString = new PrimitiveType("string", TypeSymbolValidationFlags.AllowLooseStringAssignment);
        public static readonly TypeSymbol Object = new ObjectType("object");
        public static readonly TypeSymbol Int = new PrimitiveType("int", TypeSymbolValidationFlags.Default);
        public static readonly TypeSymbol Bool = new PrimitiveType("bool", TypeSymbolValidationFlags.Default);
        public static readonly TypeSymbol Null = new PrimitiveType("null", TypeSymbolValidationFlags.Default);
        public static readonly TypeSymbol Array = new ArrayType("array");

        // declares the description property but also allows any other property of any type
        public static readonly TypeSymbol ParameterModifierMetadata = new NamedObjectType(nameof(ParameterModifierMetadata), TypeSymbolValidationFlags.Default, CreateParameterModifierMetadataProperties(), Any, TypePropertyFlags.Constant);

        public static readonly TypeSymbol Tags = new NamedObjectType(nameof(Tags), TypeSymbolValidationFlags.Default, Enumerable.Empty<TypeProperty>(), String, TypePropertyFlags.None);

        // types allowed to use in output and parameter declarations
        public static readonly ImmutableSortedDictionary<string, TypeSymbol> DeclarationTypes = new[] {String, Object, Int, Bool, Array}.ToImmutableSortedDictionary(type => type.Name, type => type, StringComparer.Ordinal);

        public static TypeSymbol? TryGetDeclarationType(string? typeName)
        {
            if (typeName != null && DeclarationTypes.TryGetValue(typeName, out TypeSymbol primitiveType))
            {
                return primitiveType;
            }

            return null;
        }

        public static TypeSymbol CreateParameterModifierType(TypeSymbol primitiveType, TypeSymbol allowedValuesType)
        {
            return new NamedObjectType($"ParameterModifier<{allowedValuesType.Name}>", TypeSymbolValidationFlags.Default, CreateParameterModifierProperties(primitiveType, allowedValuesType), additionalPropertiesType: null);
        }

        private static IEnumerable<TypeProperty> CreateParameterModifierProperties(TypeSymbol primitiveType, TypeSymbol allowedValuesType)
        {
            /*
             * The primitiveType may be set to "any" when there's a parse error in the declared type syntax node.
             * In that case, we cannot determine which modifier properties are allowed, so we allow them all.
             */

            if (ReferenceEquals(primitiveType, String) || ReferenceEquals(primitiveType, Object) || ReferenceEquals(primitiveType, Any))
            {
                // only string and object types have secure equivalents
                yield return new TypeProperty("secure", Bool, TypePropertyFlags.Constant);
            }

            // default value is allowed to have expressions
            yield return new TypeProperty(ParameterDefaultPropertyName, allowedValuesType);

            yield return new TypeProperty(ParameterAllowedPropertyName, new TypedArrayType(allowedValuesType, TypeSymbolValidationFlags.Default), TypePropertyFlags.Constant);

            if (ReferenceEquals(primitiveType, Int) || ReferenceEquals(primitiveType, Any))
            {
                // value constraints are valid on integer parameters only
                yield return new TypeProperty("minValue", Int, TypePropertyFlags.Constant);
                yield return new TypeProperty("maxValue", Int, TypePropertyFlags.Constant);
            }

            if (ReferenceEquals(primitiveType, String) || ReferenceEquals(primitiveType, Array) || ReferenceEquals(primitiveType, Any))
            {
                // strings and arrays can have length constraints
                yield return new TypeProperty("minLength", Int, TypePropertyFlags.Constant);
                yield return new TypeProperty("maxLength", Int, TypePropertyFlags.Constant);
            }

            yield return new TypeProperty("metadata", ParameterModifierMetadata, TypePropertyFlags.Constant);
        }

        private static IEnumerable<TypeProperty> CreateParameterModifierMetadataProperties()
        {
            yield return new TypeProperty("description", String, TypePropertyFlags.Constant);
        }

        public static IEnumerable<TypeProperty> GetCommonResourceProperties(ResourceTypeReference reference)
        {
            yield return new TypeProperty("id", String, TypePropertyFlags.ReadOnly | TypePropertyFlags.SkipInlining);
            yield return new TypeProperty("name", String, TypePropertyFlags.Required | TypePropertyFlags.SkipInlining);
            yield return new TypeProperty("type", new StringLiteralType(reference.FullyQualifiedType), TypePropertyFlags.ReadOnly | TypePropertyFlags.SkipInlining);
            yield return new TypeProperty("apiVersion", new StringLiteralType(reference.ApiVersion), TypePropertyFlags.ReadOnly | TypePropertyFlags.SkipInlining);
        }

        public static IEnumerable<TypeProperty> CreateResourceProperties(ResourceTypeReference resourceTypeReference)
        {
            /*
             * The following properties are intentionally excluded from this model:
             * - SystemData - this is a read-only property that doesn't belong on PUTs
             * - id - that is not allowed in templates
             * - type - included in resource type on resource declarations
             * - apiVersion - included in resource type on resource declarations
             */

            foreach (var prop in GetCommonResourceProperties(resourceTypeReference))
            {
                yield return prop;
            }

            // TODO: Model type fully
            yield return new TypeProperty("sku", Object);

            yield return new TypeProperty("kind", String);
            yield return new TypeProperty("managedBy", String);

            var stringArray = new TypedArrayType(String, TypeSymbolValidationFlags.Default);
            yield return new TypeProperty("managedByExtended", stringArray);

            yield return new TypeProperty("location", String);

            // TODO: Model type fully
            yield return new TypeProperty("extendedLocation", Object);

            yield return new TypeProperty("zones", stringArray);

            yield return new TypeProperty("plan", Object);

            yield return new TypeProperty("eTag", String);

            yield return new TypeProperty("tags", Tags);

            // TODO: Model type fully
            yield return new TypeProperty("scale", Object);

            // TODO: Model type fully
            yield return new TypeProperty("identity", Object);

            yield return new TypeProperty("properties", Object);

            var resourceRefArray = new TypedArrayType(ResourceRef, TypeSymbolValidationFlags.Default);
            yield return new TypeProperty("dependsOn", resourceRefArray, TypePropertyFlags.WriteOnly);
        }
    }
}