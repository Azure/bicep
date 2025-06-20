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
using Bicep.Local.Extension.Host.Attributes;
using static Google.Protobuf.Reflection.GeneratedCodeInfo.Types;

namespace Bicep.Local.Extension.Host.TypeDefinitionBuilder;
public class TypeDefinitionBuilder
    : ITypeDefinitionBuilder
{
    private readonly HashSet<Type> visited;
    private readonly ITypeProvider typeProvider;
    private readonly IDictionary<Type, Func<TypeBase>> typeToTypeBaseMap;

    protected readonly ConcurrentDictionary<Type, TypeBase> typeCache;
    protected readonly TypeFactory factory;

    public TypeSettings Settings { get; }

    public TypeDefinitionBuilder(TypeSettings typeSettings
                            , TypeFactory factory
                            , ITypeProvider typeProvider
                            , IDictionary<Type, Func<TypeBase>> typeToTypeBaseMap)
    {
        Settings = typeSettings ?? throw new ArgumentNullException(nameof(typeSettings));

        this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
        this.typeProvider = typeProvider ?? throw new ArgumentNullException(nameof(typeProvider));

        this.typeToTypeBaseMap = typeToTypeBaseMap is null || typeToTypeBaseMap.Count < 1
                ? throw new ArgumentNullException(nameof(typeToTypeBaseMap))
                : typeToTypeBaseMap;

        this.visited = new HashSet<Type>();
        this.typeCache = new ConcurrentDictionary<Type, TypeBase>();
    }

    public virtual TypeDefinition GenerateBicepResourceTypes()
    {
        var resourceTypes = typeProvider.GetResourceTypes()
                                .Select(rt => GenerateResource(factory, typeCache, rt))
                                .ToDictionary(rt => rt.Name, rt => new CrossFileTypeReference("types.json", factory.GetIndex(rt)));

        var index = new TypeIndex(resourceTypes
                    , new Dictionary<string, IReadOnlyDictionary<string, IReadOnlyList<CrossFileTypeReference>>>()
                    , Settings
                    , null);

        return new(GetString(stream => TypeSerializer.Serialize(stream, factory.GetTypes())),
                   GetString(stream => TypeSerializer.SerializeIndex(stream, index)));
    }

    protected virtual ResourceType GenerateResource(TypeFactory typeFactory, ConcurrentDictionary<Type, TypeBase> typeCache, Type type)
        => typeFactory.Create(() => new ResourceType(
            name: $"{type.Name}",
            scopeType: ScopeType.Unknown,
            readOnlyScopes: null,
            body: typeFactory.GetReference(typeFactory.Create(() => GenerateForRecord(typeFactory, typeCache, type))),
            flags: ResourceFlags.None,
            functions: null));

    protected virtual TypeBase GenerateForRecord(TypeFactory factory, ConcurrentDictionary<Type, TypeBase> typeCache, Type type)
    {
        var typeProperties = new Dictionary<string, ObjectTypeProperty>();



        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (visited.Contains(property.PropertyType))
            {
                // infinite recursion prevention
                continue;
            }

            var annotation = property.GetCustomAttributes<TypeAnnotationAttribute>(true).FirstOrDefault();
            var propertyType = property.PropertyType;

            TypeBase? typeReference = null;

            if (TryResolveTypeReference(propertyType, annotation, out typeReference)) { }
            else if (propertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(propertyType))// propertyType.IsAssignableTo(typeof(IEnumerable)))
            {
                // protect against infinite recursion
                visited.Add(property.PropertyType);

                Type? elementType = null;
                if (propertyType.IsArray)
                {
                    elementType = propertyType.GetElementType();
                }
                else if (propertyType.IsGenericType &&
                         propertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    elementType = propertyType.GetGenericArguments()[0];
                }

                if (elementType is null)
                {
                    throw new NotImplementedException($"Unsupported collection type {elementType}");
                }

                if (TryResolveTypeReference(elementType, annotation, out typeReference)) { }
                else
                {
                    typeReference = factory.Create(()
                         => new ArrayType(factory.GetReference(
                                                 typeCache.GetOrAdd(elementType
                                                , _ => factory.Create(() => GenerateForRecord(factory, typeCache, elementType))))));
                }
            }
            else if (propertyType.IsClass)
            {
                // protect against infinite recursion
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
                throw new NotImplementedException($"Unsupported property type {propertyType}");
            }

            if(typeReference is not null)
            {
                typeProperties[CamelCase(property.Name)] = new ObjectTypeProperty(
                factory.GetReference(typeReference),
                annotation?.Flags ?? ObjectTypePropertyFlags.None,
                annotation?.Description);
            }            
        }

        return new ObjectType(
            $"{type.Name}",
            typeProperties,
            null);
    }

    private bool TryResolveTypeReference(Type type, TypeAnnotationAttribute? annotation, [NotNullWhen(true)] out TypeBase? typeReference)
    {
        typeReference = null;
        if (type == typeof(string) && annotation?.IsSecure == true)
        {
            typeReference = factory.Create(() => new StringType(sensitive: true));
        }
        else if (typeToTypeBaseMap.TryGetValue(type, out var typeFunc))
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
