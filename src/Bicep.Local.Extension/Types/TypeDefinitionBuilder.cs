// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Concrete;
using Azure.Bicep.Types.Index;
using Azure.Bicep.Types.Serialization;
using Bicep.Local.Extension.Types.Attributes;
using static Google.Protobuf.Reflection.GeneratedCodeInfo.Types;

namespace Bicep.Local.Extension.Types;

public class TypeDefinitionBuilder : ITypeDefinitionBuilder
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
    private readonly string name;
    private readonly string version;
    private readonly bool isSingleton;
    private readonly Type? configurationType;
    private readonly TypeFactory factory;

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
        string name,
        string version,
        bool isSingleton,
        Type? configurationType,
        ITypeProvider typeProvider)
    {
        this.name = name;
        this.version = version;
        this.isSingleton = isSingleton;
        this.configurationType = configurationType;
        this.factory = new([]);
        this.typeProvider = typeProvider;
        this.typeCache = [];
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
        var typesJsonPath = "types.json";
        var resourceTypes = typeProvider.GetResourceTypes()
            .Select(x => GenerateResource(x.type, x.attribute))
            .Select(x => x.Type as ResourceType)
            .OfType<ResourceType>()
            .ToDictionary(rt => rt.Name, rt => new CrossFileTypeReference(typesJsonPath, factory.GetIndex(rt)));

        CrossFileTypeReference? config = null;
        if (configurationType is not null)
        {
            var configReference = GenerateForRecord(configurationType);
            config = new CrossFileTypeReference(typesJsonPath, factory.GetIndex(configReference.Type));
        }

        var index = new TypeIndex(
            resourceTypes,
            new Dictionary<string, IReadOnlyDictionary<string, IReadOnlyList<CrossFileTypeReference>>>(),
            new TypeSettings(name: name, version: version, isSingleton: isSingleton, configurationType: config!),
            null);

        return new(
            IndexFileContent: GetString(stream => TypeSerializer.SerializeIndex(stream, index)),
            TypeFileContents: new Dictionary<string, string>
            {
                [typesJsonPath] = GetString(stream => TypeSerializer.Serialize(stream, factory.GetTypes())),
            }.ToImmutableDictionary());
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
