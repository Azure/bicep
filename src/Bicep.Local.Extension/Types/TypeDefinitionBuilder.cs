// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Concrete;
using Azure.Bicep.Types.Index;
using Azure.Bicep.Types.Serialization;
using Bicep.Local.Extension.Builder;
using Bicep.Local.Extension.Types.Attributes;
using Bicep.Local.Extension.Types.Models;
using Microsoft.Extensions.Options;
using static Google.Protobuf.Reflection.GeneratedCodeInfo.Types;

namespace Bicep.Local.Extension.Types;

public class TypeDefinitionBuilder
    : ITypeDefinitionBuilder
{
    private readonly ITypeProvider typeProvider;
    private readonly ImmutableDictionary<Type, TypeBase> builtInTypes = new Dictionary<Type, TypeBase>
    {
        [typeof(string)] = new StringType(),
        [typeof(int)] = new IntegerType(),
        [typeof(bool)] = new BooleanType(),
        [typeof(NullReferenceType)] = new NullType(),
        [typeof(SecureStringReferenceType)] = new StringType(sensitive: true),
    }.ToImmutableDictionary();

    /// <summary>
    /// A placeholder type to represent null in nullable types.
    /// </summary>
    private record NullReferenceType();

    /// <summary>
    /// A placeholder type to represent a secure string.
    /// </summary>
    private record SecureStringReferenceType();

    private readonly Dictionary<Type, ITypeReference> typeCache;
    private readonly BicepExtensionInfo extensionInfo;
    private readonly Type? configurationType;

    protected readonly TypeFactory factory;

    private static readonly string typesJsonPath = "types.json";

    /// <summary>
    /// Provides functionality to generate Bicep resource type definitions from .NET types.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="TypeDefinitionBuilder"/> inspects resource types provided by an <see cref="ITypeProvider"/>,
    /// analyzes their public properties and associated <see cref="TypePropertyAttribute"/> metadata,
    /// and produces a <see cref="TypeDefinition"/> containing serialized type and index metadata
    /// suitable for Bicep extension consumption.
    /// </para>
    /// <para>
    /// The builder supports primitive types (string, int, bool), arrays, nullable enums, and nested complex types.
    /// If a property type cannot be mapped to a supported Bicep type, a <see cref="NotImplementedException"/> is thrown.
    /// </para>
    /// </remarks>
    public TypeDefinitionBuilder(
        BicepExtensionInfo extensionInfo,,
        ITypeProvider typeProvider,
        TypeDefinitionBuilderOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        this.extensionInfo = extensionInfo ?? throw new ArgumentNullException(nameof(extensionInfo));
        this.configurationType = options.ConfigurationType;
        this.typeCache = [];
        this.factory = new([]);
        this.typeProvider = typeProvider;
    }

    /// <summary>
    /// Generates Bicep resource type definitions based on the types provided by the <see cref="ITypeProvider"/>.
    /// This method inspects the resource types, their properties, and associated attributes to produce
    /// a <see cref="TypeDefinition"/> containing the serialized type and index metadata for use in Bicep extensions.
    /// </summary>
    /// <returns>
    /// A <see cref="TypeDefinition"/> object containing the JSON representations of the resource types and their index.
    /// </returns>
    /// <remarks>
    /// This method will throw a <see cref="NotImplementedException"/> if a property type is encountered that cannot be mapped
    /// to a supported Bicep type (e.g., unsupported primitives or collections).
    /// </remarks>
    public virtual TypeDefinition GenerateTypeDefinition()
    {
        var resourceTypes = typeProvider.GetResourceTypes()
            .Select(x => GenerateResource(x.Type, x.Attribute))
            .Select(x => x.Type as ResourceType)
            .OfType<ResourceType>()
            .ToDictionary(rt => rt.Name, rt => new CrossFileTypeReference(typesJsonPath, factory.GetIndex(rt)));

        var fallbackResourceType = typeProvider.GetFallbackType();

        CrossFileTypeReference? config = configurationType is not null ? CreateCrossFileTypeReference(configurationType) : null;        
        CrossFileTypeReference? fallbackType = fallbackResourceType?.Type is not null ? CreateCrossFileTypeReference(fallbackResourceType.Type) : null;
        
        var index = new TypeIndex(
            resources: resourceTypes,
            resourceFunctions: new Dictionary<string, IReadOnlyDictionary<string, IReadOnlyList<CrossFileTypeReference>>>(),
            settings: new TypeSettings(name: extensionInfo.Name, version: extensionInfo.Version, isSingleton: extensionInfo.IsSingleton, configurationType: config!),
            fallbackResourceType: fallbackType);

        return new(
            IndexFileContent: GetString(stream => TypeSerializer.SerializeIndex(stream, index)),
            TypeFileContents: new Dictionary<string, string>
            {
                [typesJsonPath] = GetString(stream => TypeSerializer.Serialize(stream, factory.GetTypes())),
            }.ToImmutableDictionary());
    }

    private CrossFileTypeReference? CreateCrossFileTypeReference(Type type)
    {
        var reference =  GenerateForRecord(type);
        return new CrossFileTypeReference(typesJsonPath, this.factory.GetIndex(reference.Type));
    }

    private ITypeReference GenerateResource(Type type, ResourceTypeAttribute attribute)
        => AddType(type, new ResourceType(
            name: attribute.FullName,
            body: GenerateForType(type, null) ?? throw new NotImplementedException($"Unsupported resource body type: '{type}'"),
            functions: null,
            writableScopes_in: ScopeType.All,
            readableScopes_in: ScopeType.All));

    private ITypeReference AddType(Type type, TypeBase bicepType, bool doNotCache = false)
    {
        var result = factory.GetReference(factory.Create(() => bicepType));
        if (!doNotCache)
        {
            typeCache[type] = result;
        }
        return result;
    }

    private ITypeReference? GenerateForType(Type type, TypePropertyAttribute? annotation)
    {
        if (type == typeof(string) && annotation?.IsSecure == true)
        {
            // Use a placeholder type to differentiate against non-secure strings.
            type = typeof(SecureStringReferenceType);
        }

        if (typeCache.TryGetValue(type, out var cachedValue))
        {
            return cachedValue;
        }

        if (builtInTypes.TryGetValue(type, out var bicepType))
        {
            return AddType(type, bicepType);
        }

        if (type.IsGenericType &&
            type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
            type.GetGenericArguments()[0] is { } innerType)
        {
            if (GenerateForType(typeof(NullReferenceType), null) is { } nullType &&
                GenerateForType(innerType, annotation) is { } innerBicepType)
            {
                return AddType(type, new UnionType([nullType, innerBicepType]));
            }

            return null;
        }

        if (typeof(IDictionary).IsAssignableFrom(type))
        {
            Type? keyType = null;
            Type? valueType = null;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            {
                keyType = type.GetGenericArguments()[0];
                valueType = type.GetGenericArguments()[1];
            }
            else if (type.GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>)) is { } dictType)
            {
                keyType = dictType.GetGenericArguments()[0];
                valueType = dictType.GetGenericArguments()[1];
            }

            if (keyType is null ||
                valueType is null ||
                keyType != typeof(string) ||
                GenerateForType(valueType, null) is not { } valueTypeReference)
            {
                throw new NotImplementedException($"Unsupported dictionary type: '{type}'");
            }

            return AddType(type, new ObjectType(
                $"Dictionary<string, {valueType.Name}>",
                properties: ImmutableDictionary<string, ObjectTypeProperty>.Empty,
                additionalProperties: valueTypeReference));
        }

        if (typeof(IEnumerable).IsAssignableFrom(type))
        {
            Type? elementType = null;

            if (type.IsArray)
            {
                elementType = type.GetElementType();
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                elementType = type.GetGenericArguments()[0];
            }
            else if (type.GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)) is { } enumerableType)
            {
                elementType = enumerableType.GetGenericArguments()[0];
            }

            if (elementType is null)
            {
                throw new NotImplementedException($"Unsupported collection type: '{type}'");
            }

            if (GenerateForType(elementType, annotation) is not { } elementTypeReference)
            {
                throw new NotImplementedException($"Unsupported element type: '{elementType}'");
            }

            return AddType(type, new ArrayType(elementTypeReference));
        }

        if (type.IsClass)
        {
            return GenerateForRecord(type);
        }

        if (type.IsEnum)
        {
            var enumMembers = type.GetEnumNames()
                .Select(x => factory.Create(() => new StringLiteralType(x)))
                .Select(x => factory.GetReference(x))
                .ToImmutableArray();

            return AddType(type, new UnionType(enumMembers));
        }

        return null;
    }

    private ITypeReference GenerateForRecord(Type type)
    {
        var typeProperties = new Dictionary<string, ObjectTypeProperty>();

        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var annotation = property.GetCustomAttributes<TypePropertyAttribute>(true).FirstOrDefault();
            var propertyType = property.PropertyType;

            if (GenerateForType(propertyType, annotation) is not { } typeReference)
            {
                throw new NotImplementedException($"Property '{property.Name}' references unsupported type: '{propertyType}'");
            }

            typeProperties[CamelCase(property.Name)] = new ObjectTypeProperty(
                typeReference,
                annotation?.Flags ?? ObjectTypePropertyFlags.None,
                annotation?.Description);
        }

        return AddType(type, new ObjectType(
            $"{type.Name}",
            typeProperties,
            null));
    }

    private string GetString(Action<Stream> streamWriteFunc)
    {
        using var memoryStream = new MemoryStream();
        streamWriteFunc(memoryStream);

        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }

    private static string CamelCase(string input)
        => $"{input[..1].ToLowerInvariant()}{input[1..]}";
}
