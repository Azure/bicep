// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using Bicep.McpServer.Core.ResourceProperties.Entities;

namespace Bicep.McpServer.Core.ResourceProperties.Helpers;

public static class JsonSchemaWriter
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        // UnsafeRelaxedJsonEscaping avoids unnecessary Unicode escapes (e.g., \u0027 for ' and \u002B for +)
        // while still escaping structurally significant characters (double quotes, backslashes, control chars).
        // Safe here because the input is Azure type definitions, not arbitrary user input.
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public static string Write(TypesDefinitionResult typesDefinition)
    {
        // Build a set of all known complex type names for distinguishing $ref from const
        var knownTypeNames = new HashSet<string>(StringComparer.Ordinal);
        foreach (var entity in typesDefinition.OtherComplexTypeEntities)
        {
            knownTypeNames.Add(entity.Name);
        }

        var root = new JsonObject();

        // There should be exactly one resource type entity
        var resourceTypeEntity = typesDefinition.ResourceTypeEntities[0];
        var bodyType = resourceTypeEntity.BodyType as ObjectTypeEntity
            ?? throw new InvalidDataException($"Expected bodyType to be ObjectTypeEntity, got {resourceTypeEntity.BodyType.GetType().Name}");

        root["$schema"] = "https://json-schema.org/draft/2020-12/schema";
        root["title"] = resourceTypeEntity.Name;
        root["description"] = $"Bicep resource schema for {resourceTypeEntity.Name}";
        root["x-bicep-resource-flags"] = resourceTypeEntity.Flags;
        root["x-bicep-resource-readable-scopes"] = resourceTypeEntity.ReadableScopes;
        root["x-bicep-resource-writable-scopes"] = resourceTypeEntity.WritableScopes;

        // Write resource functions
        if (typesDefinition.ResourceFunctionTypeEntities.Count > 0)
        {
            var functions = new JsonArray();
            foreach (var func in typesDefinition.ResourceFunctionTypeEntities)
            {
                var funcObj = new JsonObject
                {
                    ["name"] = func.Name,
                    ["apiVersion"] = func.ApiVersion,
                };

                if (func.InputType is not null)
                {
                    funcObj["input"] = WriteTypeReference(func.InputType, knownTypeNames);
                }

                funcObj["output"] = WriteTypeReference(func.OutputType, knownTypeNames);

                functions.Add(funcObj);
            }
            root["x-bicep-resource-functions"] = functions;
        }

        // Write the body type properties at the root level
        WriteObjectTypeInline(root, bodyType, knownTypeNames);

        // Write $defs for all other complex types
        if (typesDefinition.OtherComplexTypeEntities.Count > 0)
        {
            var defs = new JsonObject();
            foreach (var complexType in typesDefinition.OtherComplexTypeEntities)
            {
                defs[complexType.Name] = WriteComplexTypeDefinition(complexType, knownTypeNames);
            }
            root["$defs"] = defs;
        }

        return root.ToJsonString(SerializerOptions);
    }

    private static void WriteObjectTypeInline(JsonObject target, ObjectTypeEntity objectType, HashSet<string> knownTypeNames)
    {
        target["type"] = "object";

        if (objectType.Sensitive == true)
        {
            target["x-bicep-sensitive"] = true;
        }

        var properties = new JsonObject();
        var required = new JsonArray();

        foreach (var prop in objectType.Properties)
        {
            var propSchema = WritePropertySchema(prop, knownTypeNames);
            properties[prop.Name] = propSchema;

            if (prop.Flags.Contains("Required"))
            {
                required.Add(prop.Name);
            }
        }

        target["properties"] = properties;

        if (required.Count > 0)
        {
            target["required"] = required;
        }

        if (objectType.AdditionalPropertiesType is not null)
        {
            target["additionalProperties"] = WriteTypeReference(objectType.AdditionalPropertiesType, knownTypeNames);
        }
    }

    private static JsonObject WriteComplexTypeDefinition(ComplexType complexType, HashSet<string> knownTypeNames)
    {
        return complexType switch
        {
            ObjectTypeEntity objectTypeEntity => WriteObjectTypeDefinition(objectTypeEntity, knownTypeNames),
            DiscriminatedObjectTypeEntity discriminatedObjectTypeEntity => WriteDiscriminatedObjectTypeDefinition(discriminatedObjectTypeEntity, knownTypeNames),
            _ => throw new InvalidDataException($"Unexpected complex type: {complexType.GetType().Name}"),
        };
    }

    private static JsonObject WriteObjectTypeDefinition(ObjectTypeEntity objectType, HashSet<string> knownTypeNames)
    {
        var schema = new JsonObject();
        WriteObjectTypeInline(schema, objectType, knownTypeNames);
        return schema;
    }

    private static JsonObject WriteDiscriminatedObjectTypeDefinition(DiscriminatedObjectTypeEntity discriminatedType, HashSet<string> knownTypeNames)
    {
        var schema = new JsonObject();

        // Write base properties shared across all variants
        if (discriminatedType.BaseProperties.Count > 0)
        {
            var baseProperties = new JsonObject();
            var baseRequired = new JsonArray();

            foreach (var prop in discriminatedType.BaseProperties)
            {
                baseProperties[prop.Name] = WritePropertySchema(prop, knownTypeNames);

                if (prop.Flags.Contains("Required"))
                {
                    baseRequired.Add(prop.Name);
                }
            }

            schema["properties"] = baseProperties;

            if (baseRequired.Count > 0)
            {
                schema["required"] = baseRequired;
            }
        }

        // Write discriminated variants as oneOf
        if (discriminatedType.Elements.Count > 0)
        {
            var oneOf = new JsonArray();
            foreach (var element in discriminatedType.Elements)
            {
                oneOf.Add(WriteComplexTypeDefinition(element, knownTypeNames));
            }
            schema["oneOf"] = oneOf;
        }

        return schema;
    }

    private static JsonObject WritePropertySchema(PropertyInfo prop, HashSet<string> knownTypeNames)
    {
        var schema = WriteTypeReference(prop.Type, knownTypeNames);

        if (!string.IsNullOrEmpty(prop.Description))
        {
            schema["description"] = prop.Description;
        }

        if (prop.Flags.Contains("ReadOnly"))
        {
            schema["readOnly"] = true;
        }

        if (prop.Flags.Contains("WriteOnly"))
        {
            schema["writeOnly"] = true;
        }

        if (prop.Flags.Contains("DeployTimeConstant"))
        {
            schema["x-bicep-deploy-time-constant"] = true;
        }

        // Apply modifiers
        if (prop.Modifiers is not null)
        {
            ApplyModifiers(schema, prop.Modifiers);
        }

        return schema;
    }

    private static JsonObject WriteTypeReference(string typeString, HashSet<string> knownTypeNames)
    {
        // Array type: "Foo[]"
        if (typeString.EndsWith("[]"))
        {
            var itemType = typeString[..^2];
            return new JsonObject
            {
                ["type"] = "array",
                ["items"] = WriteTypeReference(itemType, knownTypeNames),
            };
        }

        // Union type: "A | B | string" or "A | B"
        if (typeString.Contains(" | "))
        {
            return WriteUnionType(typeString, knownTypeNames);
        }

        // Primitive types
        return typeString switch
        {
            "string" => new JsonObject { ["type"] = "string" },
            "bool" => new JsonObject { ["type"] = "boolean" },
            "int" => new JsonObject { ["type"] = "integer" },
            "any" => new JsonObject(),
            "null" => new JsonObject { ["type"] = "null" },
            // Known complex type — reference to $defs
            _ when knownTypeNames.Contains(typeString) => new JsonObject { ["$ref"] = $"#/$defs/{typeString}" },
            // Everything else is a constant/literal value (API versions, resource type names, enum values)
            _ => new JsonObject { ["const"] = typeString },
        };
    }

    private static JsonObject WriteUnionType(string typeString, HashSet<string> knownTypeNames)
    {
        var elements = typeString.Split(" | ");
        var literals = new List<string>();
        var hasOpenString = false;
        var hasOpenArray = false;
        var otherTypes = new List<JsonNode>();

        foreach (var element in elements)
        {
            var trimmed = element.Trim();
            switch (trimmed)
            {
                case "string":
                    hasOpenString = true;
                    break;
                case "string[]":
                    hasOpenArray = true;
                    break;
                case "bool":
                    otherTypes.Add(new JsonObject { ["type"] = "boolean" });
                    break;
                case "int":
                    otherTypes.Add(new JsonObject { ["type"] = "integer" });
                    break;
                case "null":
                    otherTypes.Add(new JsonObject { ["type"] = "null" });
                    break;
                default:
                    if (knownTypeNames.Contains(trimmed))
                    {
                        otherTypes.Add(new JsonObject { ["$ref"] = $"#/$defs/{trimmed}" });
                    }
                    else
                    {
                        literals.Add(trimmed);
                    }
                    break;
            }
        }

        // Simple enum (all literals, no open types)
        if (literals.Count > 0 && !hasOpenString && !hasOpenArray && otherTypes.Count == 0)
        {
            var enumArray = new JsonArray();
            foreach (var literal in literals)
            {
                enumArray.Add(literal);
            }
            return new JsonObject { ["enum"] = enumArray };
        }

        // Enum + open string (e.g., "Allow | Deny | string")
        if (literals.Count > 0 && hasOpenString && !hasOpenArray && otherTypes.Count == 0)
        {
            var enumArray = new JsonArray();
            foreach (var literal in literals)
            {
                enumArray.Add(literal);
            }
            return new JsonObject
            {
                ["anyOf"] = new JsonArray
                {
                    new JsonObject { ["enum"] = enumArray },
                    new JsonObject { ["type"] = "string" },
                },
            };
        }

        // Enum literals + open array (e.g., "all | get | list | string[]")
        if (literals.Count > 0 && hasOpenArray && !hasOpenString && otherTypes.Count == 0)
        {
            var enumArray = new JsonArray();
            foreach (var literal in literals)
            {
                enumArray.Add(literal);
            }
            return new JsonObject
            {
                ["type"] = "array",
                ["items"] = new JsonObject
                {
                    ["anyOf"] = new JsonArray
                    {
                        new JsonObject { ["enum"] = enumArray },
                        new JsonObject { ["type"] = "string" },
                    },
                },
            };
        }

        // General case: anyOf with all elements
        var anyOf = new JsonArray();
        if (literals.Count > 0)
        {
            var enumArr = new JsonArray();
            foreach (var literal in literals)
            {
                enumArr.Add(literal);
            }
            anyOf.Add(new JsonObject { ["enum"] = enumArr });
        }
        if (hasOpenString)
        {
            anyOf.Add(new JsonObject { ["type"] = "string" });
        }
        if (hasOpenArray)
        {
            anyOf.Add(new JsonObject { ["type"] = "array", ["items"] = new JsonObject { ["type"] = "string" } });
        }
        foreach (var other in otherTypes)
        {
            anyOf.Add(other);
        }

        return new JsonObject { ["anyOf"] = anyOf };
    }

    private static void ApplyModifiers(JsonObject schema, string modifiers)
    {
        foreach (var modifier in modifiers.Split(", "))
        {
            var parts = modifier.Split(": ", 2);
            if (parts.Length != 2)
            {
                // Handle simple flag modifiers like "sensitive"
                if (modifier == "sensitive")
                {
                    schema["x-bicep-sensitive"] = true;
                }
                continue;
            }

            var key = parts[0].Trim();
            var value = parts[1].Trim();

            switch (key)
            {
                case "minLength":
                    if (int.TryParse(value, out var minLen))
                    {
                        schema["minLength"] = minLen;
                    }
                    break;
                case "maxLength":
                    if (int.TryParse(value, out var maxLen))
                    {
                        schema["maxLength"] = maxLen;
                    }
                    break;
                case "minValue":
                    if (long.TryParse(value, out var minVal))
                    {
                        schema["minimum"] = minVal;
                    }
                    break;
                case "maxValue":
                    if (long.TryParse(value, out var maxVal))
                    {
                        schema["maximum"] = maxVal;
                    }
                    break;
                case "pattern":
                    schema["pattern"] = value;
                    break;
            }
        }
    }
}
