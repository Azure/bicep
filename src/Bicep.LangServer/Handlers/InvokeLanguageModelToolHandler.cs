// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Azure.Deployments.Core.Comparers;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace Bicep.LanguageServer.Handlers;

[Method("bicep/languageModelTool/invoke", Direction.ClientToServer)]
public record InvokeLanguageModelToolRequest(string ToolName, string InputJson)
    : IRequest<InvokeLanguageModelToolResponse>;

public record InvokeLanguageModelToolResponse(string Content);

public class InvokeLanguageModelToolHandler(
    IResourceTypeProviderFactory resourceTypeProviderFactory) : IJsonRpcRequestHandler<InvokeLanguageModelToolRequest, InvokeLanguageModelToolResponse>
{
    public async Task<InvokeLanguageModelToolResponse> Handle(InvokeLanguageModelToolRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();

        switch (request.ToolName)
        {
            case "bicep_getResourceType":
                return new(GetResourceTypeResponse(request.InputJson));
            default:
                throw new NotImplementedException($"Tool '{request.ToolName}' is not implemented.");
        }
    }

    private static JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private record GetAzureResourceSchemaRequest(string ResourceType, string? ApiVersion);

    private string GetResourceTypeResponse(string inputJson)
    {
        var typeProvider = resourceTypeProviderFactory.GetBuiltInAzResourceTypesProvider();
        var request = JsonSerializer.Deserialize<GetAzureResourceSchemaRequest>(inputJson, JsonSerializerOptions)
            ?? throw new InvalidOperationException("Failed to deserialize request.");

        var typeReference = typeProvider.GetAvailableTypes()
            .Where(x => x.Type == request.ResourceType)
            .Where(x => x.ApiVersion is null || x.ApiVersion == request.ApiVersion)
            .OrderByDescending(x => x.ApiVersion ?? "", ApiVersionComparer.Instance)
            .FirstOrDefault()
            ?? throw new InvalidOperationException($"Resource type '{request.ResourceType}' with API version '{request.ApiVersion}' not found.");

        var resourceType = typeProvider.TryGetDefinedType(
            AzNamespaceType.Create("az", ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup | ResourceScope.Resource, typeProvider, BicepSourceFileKind.BicepFile),
            typeReference,
            ResourceTypeGenerationFlags.None)
            ?? throw new InvalidOperationException($"Resource type '{request.ResourceType}' with API version '{request.ApiVersion}' not found.");

        return $"""
Here is the JSON schema for resource type `{typeReference}`:

```json
{ToJsonSchemaRecursive(resourceType.Body)}
```
""";
    }
    
    private static JsonObject ToJsonSchemaRecursive(ITypeReference typeReference)
    {
        // TODO handle cycles!
        RuntimeHelpers.EnsureSufficientExecutionStack();

        switch (typeReference.Type)
        {
            case StringLiteralType _:
            case StringType _:
            case UnionType _:
                return new JsonObject
                {
                    ["type"] = "string"
                };
            case IntegerType _:
                return new JsonObject
                {
                    ["type"] = "number"
                };
            case BooleanType _:
                return new JsonObject
                {
                    ["type"] = "boolean"
                };
            case ArrayType arrayType:
                return new JsonObject
                {
                    ["type"] = "array",
                    ["items"] = ToJsonSchemaRecursive(arrayType.Item.Type)
                };
            case ObjectType objectType:
                var writableProps = objectType.Properties.Where(x => !x.Value.Flags.HasFlag(TypePropertyFlags.ReadOnly));
                var requiredProps = writableProps.Where(x => x.Value.Flags.HasFlag(TypePropertyFlags.Required));

                var properties = writableProps.Select(x => new KeyValuePair<string, JsonNode?>(x.Key, ToJsonSchemaRecursive(x.Value.TypeReference)));
                return new JsonObject
                {
                    ["type"] = "object",
                    ["properties"] = new JsonObject(properties),
                    ["required"] = new JsonArray([.. requiredProps.Select(x => JsonValue.Create(x.Key))]),
                };
            default:
                // TODO discriminated object support
                throw new NotImplementedException($"{typeReference.Type.GetType()}");
        }
    }
}
