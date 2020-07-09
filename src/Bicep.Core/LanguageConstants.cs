using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.TypeSystem;

namespace Bicep.Core
{
    public static class LanguageConstants
    {
        public const int MaxParameterCount = 256;
        public const int MaxIdentifierLength = 255;

        public const string ListSeparator = ", ";

        public static readonly TypeSymbol Any = new AnyType();
        public static readonly TypeSymbol String = new PrimitiveType("string");
        public static readonly TypeSymbol Object = new ObjectType("object");
        public static readonly TypeSymbol Int = new PrimitiveType("int");
        public static readonly TypeSymbol Bool = new PrimitiveType("bool");
        public static readonly TypeSymbol Array = new ArrayType("array");

        // strict schema on the modifier - no extra properties allowed
        public static readonly TypeSymbol ParameterModifier = new NamedObjectType(nameof(ParameterModifier), CreateParameterModifierProperties(), additionalPropertiesType: null);

        // declares the description property but also allows any other property of any type
        public static readonly TypeSymbol ParameterModifierMetadata = new NamedObjectType(nameof(ParameterModifierMetadata), CreateParameterModifierMetadataProperties(), Any);

        public static readonly TypeSymbol Tags = new NamedObjectType(nameof(Tags), Enumerable.Empty<TypeProperty>(), String);

        public static readonly TypeSymbol StringArray = new NamedArrayType("StringArray", String);

        public static readonly ImmutableArray<TypeProperty> TopLevelResourceProperties = CreateResourceProperties().ToImmutableArray();

        public static readonly ImmutableSortedDictionary<string, TypeSymbol> PrimitiveTypes = new[] {String, Object, Int, Bool, Array}.ToImmutableSortedDictionary(type => type.Name, type => type, StringComparer.Ordinal);

        public static readonly string PrimitiveTypesString = LanguageConstants.PrimitiveTypes.Keys.ConcatString(ListSeparator);

        private static IEnumerable<TypeProperty> CreateParameterModifierProperties()
        {
            yield return new TypeProperty("secure", Bool, required: false);
            yield return new TypeProperty("defaultValue", Any, required: false);

            yield return new TypeProperty("allowedValues", Array, required: false);

            yield return new TypeProperty("minValue", Int, required: false);
            yield return new TypeProperty("maxValue", Int, required: false);

            yield return new TypeProperty("minLength", Int, required: false);
            yield return new TypeProperty("maxLength", Int, required: false);

            yield return new TypeProperty("metadata", ParameterModifierMetadata, false);
        }

        private static IEnumerable<TypeProperty> CreateParameterModifierMetadataProperties()
        {
            yield return new TypeProperty("description", String, required: false);
        }

        private static IEnumerable<TypeProperty> CreateResourceProperties()
        {
            /*
             * The following properties are intentionally excluded from this model:
             * - SystemData - this is a read-only property that doesn't belong on PUTs
             * - id - that is not allowed in templates
             * - type - included in resource type on resource declarations
             * - apiVersion - included in resource type on resource declarations
             */

            yield return new TypeProperty("name", String, required: true);

            // TODO: Model type fully
            yield return new TypeProperty("sku", Object, required: false);

            yield return new TypeProperty("kind", String, required: false);
            yield return new TypeProperty("managedBy", String, required: false);

            yield return new TypeProperty("managedByExtended", StringArray, required: false);

            yield return new TypeProperty("location", String, required: false);

            // TODO: Model type fully
            yield return new TypeProperty("extendedLocation", Object, required: false);

            yield return new TypeProperty("zones", StringArray, required: false);

            yield return new TypeProperty("plan", Object, required: false);

            yield return new TypeProperty("eTag", String, required: false);

            yield return new TypeProperty("tags", Tags, required: false);

            // TODO: Model type fully
            yield return new TypeProperty("scale", Object, required: false);

            // TODO: Model type fully
            yield return new TypeProperty("identity", Object, required: false);

            yield return new TypeProperty("properties", Object, required: false);
        }
    }
}
