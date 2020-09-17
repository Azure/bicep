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

        public static readonly StringComparer IdentifierComparer = StringComparer.Ordinal;
        public static readonly StringComparison IdentifierComparison = StringComparison.Ordinal;

        public const string StringDelimiter = "'";
        public const string StringHoleOpen = "${";
        public const string StringHoleClose = "}";

        public static readonly TypeSymbol Any = new AnyType();
        public static readonly TypeSymbol ResourceRef = new ResourceRefType();
        public static readonly TypeSymbol String = new PrimitiveType("string");
        public static readonly TypeSymbol Object = new ObjectType("object");
        public static readonly TypeSymbol Int = new PrimitiveType("int");
        public static readonly TypeSymbol Bool = new PrimitiveType("bool");
        public static readonly TypeSymbol Null = new PrimitiveType("null");
        public static readonly TypeSymbol Array = new ArrayType("array");

        // declares the description property but also allows any other property of any type
        public static readonly TypeSymbol ParameterModifierMetadata = new NamedObjectType(nameof(ParameterModifierMetadata), CreateParameterModifierMetadataProperties(), Any, TypePropertyFlags.Constant);

        public static readonly TypeSymbol Tags = new NamedObjectType(nameof(Tags), Enumerable.Empty<TypeProperty>(), String, TypePropertyFlags.None);

        // types allowed to use in output and parameter declarations
        public static readonly ImmutableSortedDictionary<string, TypeSymbol> DeclarationTypes = new[] {String, Object, Int, Bool, Array}.ToImmutableSortedDictionary(type => type.Name, type => type, StringComparer.Ordinal);

        public static readonly string DeclarationTypesString = LanguageConstants.DeclarationTypes.Keys.ConcatString(ListSeparator);

        public static TypeSymbol CreateParameterModifierType(TypeSymbol parameterType)
        {
            if (parameterType.TypeKind != TypeKind.Primitive)
            {
                throw new ArgumentException($"Modifiers are not supported for type '{parameterType.Name}'.");
            }

            return new NamedObjectType($"ParameterModifier_{parameterType.Name}", CreateParameterModifierProperties(parameterType), additionalPropertiesType: null);
        }

        private static IEnumerable<TypeProperty> CreateParameterModifierProperties(TypeSymbol parameterType)
        {
            if (ReferenceEquals(parameterType, String) || ReferenceEquals(parameterType, Object))
            {
                // only string and object types have secure equivalents
                yield return new TypeProperty("secure", Bool, TypePropertyFlags.Constant);
            }

            // default value is allowed to have expressions
            yield return new TypeProperty("default", parameterType);

            yield return new TypeProperty("allowed", new TypedArrayType(parameterType), TypePropertyFlags.Constant);

            if (ReferenceEquals(parameterType, Int))
            {
                // value constraints are valid on integer parameters only
                yield return new TypeProperty("minValue", Int, TypePropertyFlags.Constant);
                yield return new TypeProperty("maxValue", Int, TypePropertyFlags.Constant);
            }

            if (ReferenceEquals(parameterType, String) || ReferenceEquals(parameterType, Array))
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

        public static IEnumerable<TypeProperty> CreateResourceProperties(ResourceTypeReference resourceTypeReference)
        {
            /*
             * The following properties are intentionally excluded from this model:
             * - SystemData - this is a read-only property that doesn't belong on PUTs
             * - id - that is not allowed in templates
             * - type - included in resource type on resource declarations
             * - apiVersion - included in resource type on resource declarations
             */

            yield return new TypeProperty("id", String, TypePropertyFlags.ReadOnly | TypePropertyFlags.SkipInlining);

            yield return new TypeProperty("name", String, TypePropertyFlags.Required | TypePropertyFlags.SkipInlining);

            yield return new TypeProperty("type", new StringLiteralType(resourceTypeReference.FullyQualifiedType), TypePropertyFlags.ReadOnly | TypePropertyFlags.SkipInlining);

            yield return new TypeProperty("apiVersion", new StringLiteralType(resourceTypeReference.ApiVersion), TypePropertyFlags.ReadOnly | TypePropertyFlags.SkipInlining);

            // TODO: Model type fully
            yield return new TypeProperty("sku", Object);

            yield return new TypeProperty("kind", String);
            yield return new TypeProperty("managedBy", String);

            var stringArray = new TypedArrayType(String);
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

            yield return new TypeProperty("dependsOn", new TypedArrayType(ResourceRef), TypePropertyFlags.WriteOnly);
        }
    }
}