// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
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

public class TypeDefinitionBuilder
    : ITypeDefinitionBuilder
{
    private readonly HashSet<Type> visited;
    private readonly ITypeProvider typeProvider;
    private readonly ImmutableDictionary<Type, Func<TypeBase>> builtInTypes = new Dictionary<Type, Func<TypeBase>>
    {
        { typeof(string), () => new StringType() },
        { typeof(int), () => new IntegerType() },
        { typeof(bool), () => new BooleanType() },
    }.ToImmutableDictionary();

    protected readonly ConcurrentDictionary<Type, TypeBase> typeCache;
    private readonly string name;
    private readonly string version;
    private readonly bool isSingleton;
    private readonly Type? configurationType;
    protected readonly TypeFactory factory;


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
        this.visited = new HashSet<Type>();
        this.typeCache = new ConcurrentDictionary<Type, TypeBase>();
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
            .Select(x => GenerateResource(factory, typeCache, x.type, x.attribute))
            .ToDictionary(rt => rt.Name, rt => new CrossFileTypeReference(typesJsonPath, factory.GetIndex(rt)));

        CrossFileTypeReference? config = null;
        if (configurationType is not null)
        {
            var configReference = factory.Create(() => GenerateForRecord(factory, typeCache, configurationType));
            config = new CrossFileTypeReference(typesJsonPath, factory.GetIndex(configReference));
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

    protected virtual ResourceType GenerateResource(TypeFactory typeFactory, ConcurrentDictionary<Type, TypeBase> typeCache, Type type, ResourceTypeAttribute attribute)
        => typeFactory.Create(() => new ResourceType(
            name: attribute.FullName,
            body: typeFactory.GetReference(typeFactory.Create(() => GenerateForRecord(typeFactory, typeCache, type))),
            functions: null,
            writableScopes_in: ScopeType.All,
            readableScopes_in: ScopeType.All));

    protected virtual TypeBase GenerateForRecord(TypeFactory factory, ConcurrentDictionary<Type, TypeBase> typeCache, Type type)
    {
        var typeProperties = new Dictionary<string, ObjectTypeProperty>();

        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (visited.Contains(property.PropertyType))
            {
                continue;
            }

            var annotation = property.GetCustomAttributes<TypePropertyAttribute>(true).FirstOrDefault();
            var propertyType = property.PropertyType;

            TypeBase? typeReference = null;

            if (!TryResolveTypeReference(propertyType, annotation, out typeReference))
            {
                if (typeof(IDictionary).IsAssignableFrom(propertyType))
                {
                    throw new NotImplementedException($"Property '{property.Name}' references unsupported dictionary type: '{propertyType}'");
                }
                else if (typeof(IEnumerable).IsAssignableFrom(propertyType))
                {
                    // protect against infinite recursion
                    visited.Add(property.PropertyType);

                    Type? elementType = null;

                    if (propertyType.IsArray)
                    {
                        elementType = propertyType.GetElementType();
                    }
                    else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    {
                        elementType = propertyType.GetGenericArguments()[0];
                    }
                    else if (propertyType.GetInterfaces()
                        .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)) is {} enumerableType)
                    {
                        elementType = enumerableType.GetGenericArguments()[0];
                    }

                    if (elementType is null)
                    {
                        throw new NotImplementedException($"Property '{property.Name}' references unsupported collection type: '{propertyType}'");
                    }

                    if (!TryResolveTypeReference(elementType, annotation, out var elementTypeReference))
                    {
                        elementTypeReference = typeCache.GetOrAdd(elementType, _ => factory.Create(() => GenerateForRecord(factory, typeCache, elementType)));
                    }

                    typeReference = typeCache.GetOrAdd(propertyType, _ => factory.Create(() => new ArrayType(factory.GetReference(elementTypeReference))));
                }
                else if (propertyType.IsClass)
                {
                    visited.Add(property.PropertyType);

                    typeReference = typeCache.GetOrAdd(propertyType, _ => factory.Create(() => GenerateForRecord(factory, typeCache, propertyType)));
                }
                else if (propertyType.IsGenericType &&
                    propertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                    propertyType.GetGenericArguments()[0] is { IsEnum: true } enumType)
                {
                    var enumMembers = enumType.GetEnumNames()
                        .Select(x => factory.Create(() => new StringLiteralType(x)))
                        .Select(x => factory.GetReference(x))
                        .ToImmutableArray();

                    typeReference = typeCache.GetOrAdd(propertyType, _ => factory.Create(() => new UnionType(enumMembers)));
                }
                else
                {
                    throw new NotImplementedException($"Property '{property.Name}' references unsupported type: '{propertyType}'");
                }
            }

            typeProperties[CamelCase(property.Name)] = new ObjectTypeProperty(
            factory.GetReference(typeReference),
            annotation?.Flags ?? ObjectTypePropertyFlags.None,
            annotation?.Description);
        }

        return new ObjectType(
            $"{type.Name}",
            typeProperties,
            null);
    }

    private bool TryResolveTypeReference(Type type, TypePropertyAttribute? annotation, [NotNullWhen(true)] out TypeBase? typeReference)
    {
        typeReference = null;
        if (type == typeof(string) && annotation?.IsSecure == true)
        {
            typeReference = typeCache.GetOrAdd(type, _ => factory.Create(() => new StringType(sensitive: true)));
        }
        else if (builtInTypes.TryGetValue(type, out var typeFunc))
        {
            typeReference = typeCache.GetOrAdd(type, _ => factory.Create(typeFunc));
        }

        return typeReference is not null;
    }


    protected virtual string GetString(Action<Stream> streamWriteFunc)
    {
        using var memoryStream = new MemoryStream();
        streamWriteFunc(memoryStream);

        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }

    protected static string CamelCase(string input)
        => $"{input[..1].ToLowerInvariant()}{input[1..]}";
}
