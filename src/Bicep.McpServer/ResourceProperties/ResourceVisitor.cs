// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Bicep.Types;
using Azure.Bicep.Types.Az;
using Azure.Bicep.Types.Concrete;
using Azure.Bicep.Types.Index;
using Bicep.McpServer.ResourceProperties.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.ResourceStack.Common.Collections;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.McpServer.ResourceProperties;

public class ResourceVisitor
{
    private readonly ILogger<ResourceVisitor> _logger;

    public ResourceVisitor(ILogger<ResourceVisitor> logger)
    {
        _logger = logger;
    }

    public TypesDefinitionResult LoadSingleResourceType(string fullyQualifiedResourceType, string apiVersion)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var loader = new AzTypeLoader();
        TypeIndex typeIndex = loader.LoadTypeIndex();

        // Sample key of a resource: "Microsoft.App/containerApps/authConfigs@2024-03-01"
        var resources = new InsensitiveDictionary<CrossFileTypeReference>(typeIndex.Resources.ToDictionary());

        // Sample key of a resource function: "microsoft.app/containerapps"
        // Sample value of a resource function: { "2024-03-01": [ function1, function2 ], "2024-08-02-preview": [ function1, function2] }
        var resourceFunctions = new InsensitiveDictionary<IReadOnlyDictionary<string, IReadOnlyList<CrossFileTypeReference>>>(typeIndex.ResourceFunctions.ToDictionary());

        _logger.LogInformation("Total Resources: {ResourceCount}", resources.Count);
        _logger.LogInformation("Total Resource Functions: {ResourceFunctionCount}", resourceFunctions.Count);

        var typesToWrite = new List<TypeBase>();
        CrossFileTypeReference resourceReference = resources.FirstOrDefault(r => r.Key.EqualsOrdinalInsensitively($"{fullyQualifiedResourceType}@{apiVersion}")).Value
            ?? throw new InvalidDataException($"Resource type {fullyQualifiedResourceType} with API version {apiVersion} not found.");

        var selectedResourceFunctions = resourceFunctions.Where(r => r.Key.ContainsInsensitively(fullyQualifiedResourceType)).ToList();

        var result = new TypesDefinitionResult
        {
            ResourceProvider = fullyQualifiedResourceType.Split("/").First(),
            ApiVersion = apiVersion
        };

        ResourceType resourceType = loader.LoadResourceType(resourceReference);
        FindTypesToWrite(typesToWrite, resourceType.Body);

        if (WriteComplexType(resourceType) is ResourceTypeEntity resourceTypeEntity)
        {
            result.ResourceTypeEntities.Add(resourceTypeEntity);
        }
        else
        {
            throw new InvalidDataException($"Resource type {resourceType.Name} failed to be converted to ResourceTypeEntity.");
        }

        foreach (KeyValuePair<string, IReadOnlyDictionary<string, IReadOnlyList<CrossFileTypeReference>>> resourceFunction in selectedResourceFunctions)
        {
            var functions = resourceFunction.Value.Where(r => r.Key.Equals(apiVersion)).SelectMany(r => r.Value).ToList();

            foreach (CrossFileTypeReference? function in functions)
            {
                ResourceFunctionType resourceFunctionType = loader.LoadResourceFunctionType(function);
                if (resourceFunctionType.Input != null)
                {
                    typesToWrite.Add(resourceFunctionType.Input.Type);
                    FindTypesToWrite(typesToWrite, resourceFunctionType.Input);
                }

                typesToWrite.Add(resourceFunctionType.Output.Type);
                FindTypesToWrite(typesToWrite, resourceFunctionType.Output);

                if (WriteComplexType(resourceFunctionType) is ResourceFunctionTypeEntity resourceFunctionTypeEntity)
                {
                    result.ResourceFunctionTypeEntities.Add(resourceFunctionTypeEntity);
                }
                else
                {
                    throw new InvalidDataException($"Resource function type {resourceFunctionType.Name} failed to be converted to ResourceFunctionTypeEntity.");
                }
            }
        }

        // Sort by name first (e.g. listSecrets), then by resource type (e.g. Microsoft.ApiManagement/service/authorizationServers)
        result.ResourceFunctionTypeEntities.Sort((a, b) =>
        {
            int nameComparison = string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
            return nameComparison != 0 ? nameComparison : string.Compare(a.ResourceType, b.ResourceType, StringComparison.OrdinalIgnoreCase);
        });

        foreach (TypeBase type in typesToWrite)
        {
            if (IsComplexType(type))
            {
                result.OtherComplexTypeEntities.Add(WriteComplexType(type));
            }
        }

        // Note(ligar): Dedupe here because OtherComplexTypeEntities can contain duplicates. This is because instances of the same type (TypeBase) can have different hash codes (Refer to ProcessTypeLinks() method).
        result.OtherComplexTypeEntities = [.. result.OtherComplexTypeEntities
            .GroupBy(e => e.Name)
            .Select(g => g.First())];

        result.OtherComplexTypeEntities.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));

        _logger.LogInformation("Total Complex Types: {OtherComplexTypeEntityCount}", result.OtherComplexTypeEntities.Count);

        stopwatch.Stop();
        _logger.LogInformation("LoadSingleResourceType took {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);

        return result;
    }

    // This finds all the types the referenced type depends on, and adds them to the typesToWrite list.
    private void FindTypesToWrite(List<TypeBase> typesToWrite, ITypeReference typeReference)
    {
        switch (typeReference.Type)
        {
            case ArrayType arrayType:
                ProcessTypeLinks(typesToWrite, arrayType.ItemType, false);
                break;
            case ObjectType objectType:
                foreach (KeyValuePair<string, ObjectTypeProperty> property in objectType.Properties.OrderByAscendingOrdinalInsensitively(kvp => kvp.Key))
                {
                    ProcessTypeLinks(typesToWrite, property.Value.Type, false);
                }
                if (objectType.AdditionalProperties != null)
                {
                    ProcessTypeLinks(typesToWrite, objectType.AdditionalProperties, false);
                }
                break;
            case DiscriminatedObjectType discriminatedObjectType:
                foreach (KeyValuePair<string, ITypeReference> property in discriminatedObjectType.Elements.OrderByAscendingOrdinalInsensitively(kvp => kvp.Key))
                {
                    // Don't display discriminated object elements as individual types
                    ProcessTypeLinks(typesToWrite, property.Value, true);
                }
                break;
            default:
                // In this method, we don't care about simple types such as IntegerType
                break;

        }
    }

    private void ProcessTypeLinks(List<TypeBase> typesToWrite, ITypeReference typeReference, bool skipParent)
    {
        if (!typesToWrite.Contains(typeReference.Type))
        {
            if (!skipParent)
            {
                typesToWrite.Add(typeReference.Type);
            }

            FindTypesToWrite(typesToWrite, typeReference);
        }
    }

    private ComplexType WriteComplexType(TypeBase typeBase)
    {
        switch (typeBase)
        {
            case ResourceType resourceType:
                var rtEntity = new ResourceTypeEntity
                {
                    Name = resourceType.Name,
                    BodyType = WriteComplexType(resourceType.Body.Type),
                    // Resource is ReadOnly if there are no writable scopes (matches legacy ReadOnly behavior)
                    Flags = (resourceType.WritableScopes == Azure.Bicep.Types.Concrete.ScopeType.None ? "ReadOnly" : "None"),
                    // Use intersection of readable and writable scopes (where resource can be both read and deployed)
                    ScopeType = (resourceType.ReadableScopes & resourceType.WritableScopes).ToString(),
                    // Calculate truly read only scopes: readable scopes minus writable scopes
                    ReadOnlyScopes = (resourceType.ReadableScopes & ~resourceType.WritableScopes).ToString()
                };
                return rtEntity;
            case ResourceFunctionType resourceFunctionType:
                var rftEntity = new ResourceFunctionTypeEntity
                {
                    Name = resourceFunctionType.Name,
                    ResourceType = resourceFunctionType.ResourceType,
                    ApiVersion = resourceFunctionType.ApiVersion,
                    InputType = resourceFunctionType.Input != null ? GetTypeName(resourceFunctionType.Input.Type) : null,
                    OutputType = GetTypeName(resourceFunctionType.Output.Type)
                };
                return rftEntity;
            case ObjectType objectType:
                var otEntity = new ObjectTypeEntity
                {
                    Name = objectType.Name,
                    Sensitive = objectType.Sensitive,
                    AdditionalPropertiesType = objectType.AdditionalProperties != null ? GetTypeName(objectType.AdditionalProperties.Type) : null
                };
                foreach (KeyValuePair<string, ObjectTypeProperty> property in objectType.Properties.OrderByAscendingOrdinalInsensitively(kvp => kvp.Key))
                {
                    otEntity.Properties.Add(WriteTypeProperty(property.Key, property.Value));
                }
                return otEntity;
            case DiscriminatedObjectType discriminatedObjectType:
                var dotEntity = new DiscriminatedObjectTypeEntity
                {
                    Name = discriminatedObjectType.Name
                };
                foreach (KeyValuePair<string, ObjectTypeProperty> baseProperty in discriminatedObjectType.BaseProperties.OrderByAscendingOrdinalInsensitively(kvp => kvp.Key))
                {
                    dotEntity.BaseProperties.Add(WriteTypeProperty(baseProperty.Key, baseProperty.Value));
                }
                foreach (KeyValuePair<string, ITypeReference> element in discriminatedObjectType.Elements.OrderByAscendingOrdinalInsensitively(kvp => kvp.Key))
                {
                    dotEntity.Elements.Add(WriteComplexType(element.Value.Type));
                }

                return dotEntity;
            default:
                throw new InvalidDataException("Unexpected type");
        }
    }

    private PropertyInfo WriteTypeProperty(string propertyName, ObjectTypeProperty property)
    {
        return new PropertyInfo(
            propertyName,
            GetTypeName(property.Type.Type),
            property.Description ?? string.Empty,
            property.Flags.ToString(),
            GetModifers(property.Type.Type));
    }

    private string GetTypeName(TypeBase typeBase)
    {
        return typeBase switch
        {
            ResourceType resourceType => resourceType.Name,
            ResourceFunctionType resourceFunctionType => $"{resourceFunctionType.Name} ({resourceFunctionType.ResourceType}@{resourceFunctionType.ApiVersion})",
            ObjectType objectType => objectType.Name,
            DiscriminatedObjectType discriminatedObjectType => discriminatedObjectType.Name,
            ArrayType arrayType => $"{GetTypeName(arrayType.ItemType.Type)}[]",
            UnionType unionType => string.Join(" | ", unionType.Elements.Select(e => GetTypeName(e.Type))),
            AnyType anyType => "any",
            NullType nullType => "null",
            BooleanType booleanType => "bool",
            IntegerType integerType => "int",
            StringType stringType => "string",
            StringLiteralType stringLiteralType => stringLiteralType.Value,
            BuiltInType builtInType => builtInType.Kind.ToString().ToLower(),
            _ => throw new InvalidDataException("Unrecognized type"),
        };
    }

    private string? GetModifers(TypeBase typeBase)
    {
        return typeBase switch
        {
            IntegerType integerType => GetIntegerModifiers(integerType),
            StringType stringType => GetStringModifiers(stringType),
            _ => null,
        };
    }

    private string? GetIntegerModifiers(IntegerType integerType)
    {
        return FormatModifiers(
            integerType.MinValue != null ? $"minValue: {integerType.MinValue}" : null,
            integerType.MaxValue != null ? $"maxValue: {integerType.MaxValue}" : null);
    }

    private string? GetStringModifiers(StringType stringType)
    {
        return FormatModifiers(
            stringType.Sensitive == true ? "sensitive" : null,
            stringType.MinLength != null ? $"minLength: {stringType.MinLength}" : null,
            stringType.MaxLength != null ? $"maxLength: {stringType.MaxLength}" : null,
            stringType.Pattern != null ? $"pattern: {stringType.Pattern}" : null);
    }

    private string? FormatModifiers(params string?[] modifiers)
    {
        string modifiersString = string.Join(", ", modifiers.Where(m => !string.IsNullOrEmpty(m)));
        return string.IsNullOrEmpty(modifiersString) ? null : modifiersString;
    }

    private bool IsComplexType(TypeBase typeBase)
    {
        return typeBase switch
        {
            ResourceType => true,
            ResourceFunctionType => true,
            ObjectType => true,
            DiscriminatedObjectType => true,
            _ => false,
        };
    }
}
