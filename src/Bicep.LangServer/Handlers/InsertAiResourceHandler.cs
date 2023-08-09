// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Parsing;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Extensions;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Bicep.Core.Extensions;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol;
using System.Text.RegularExpressions;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Telemetry;
using Bicep.Core.Semantics.Namespaces;
using System.Collections.Immutable;
using Azure.AI.OpenAI;
using Bicep.Core.TypeSystem;
using Newtonsoft.Json.Linq;
using Bicep.Core.Json;
using System.Runtime.CompilerServices;

namespace Bicep.LanguageServer.Handlers;

[Method("textDocument/insertAiResource", Direction.ClientToServer)]
public record InsertAiResourceParams : TextDocumentPositionParams, IRequest
{
    public required string OpenAiEndpoint { get; init; }
    public required string? OpenAiKey { get; init; }
    public required string ResourceType { get; init; }
    public required string ApiVersion { get; init; }
    public required string Scenario { get; init; }
}

public class InsertAiResourceHandler : IJsonRpcNotificationHandler<InsertAiResourceParams>
{
    private readonly ILanguageServerFacade server;
    private readonly ICompilationManager compilationManager;
    private readonly OpenAiProvider openAiProvider;
    private readonly TelemetryAndErrorHandlingHelper<Unit> helper;

    public InsertAiResourceHandler(
        ILanguageServerFacade server,
        ICompilationManager compilationManager,
        OpenAiProvider openAiProvider,
        ITelemetryProvider telemetryProvider)
    {
        this.server = server;
        this.compilationManager = compilationManager;
        this.openAiProvider = openAiProvider;
        this.helper = new TelemetryAndErrorHandlingHelper<Unit>(server.Window, telemetryProvider);
    }

    public Task<Unit> Handle(InsertAiResourceParams request, CancellationToken cancellationToken)
        => helper.ExecuteWithTelemetryAndErrorHandling(async () =>
        {
            var context = compilationManager.GetCompilation(request.TextDocument.Uri);
            if (context is null)
            {
                return (Unit.Value, null);
            }

            var model = context.Compilation.GetEntrypointSemanticModel();

            var nsResolver = model.Binder.NamespaceResolver;
            var namespaces = nsResolver.GetNamespaceNames().Select(nsResolver.TryGetNamespace).WhereNotNull();
            var azNamespace = namespaces.First(ns => ns.ProviderName == AzNamespaceType.BuiltInName);

            var typeReference = ResourceTypeReference.Parse($"{request.ResourceType}@{request.ApiVersion}");
            if (azNamespace.ResourceTypeProvider.TryGetDefinedType(azNamespace, typeReference, ResourceTypeGenerationFlags.None) is not {} resourceType) {
                throw helper.CreateException(
                    $"Failed to find a Bicep type definition for resource of type \"{typeReference}\".",
                    BicepTelemetryEvent.InsertAiResourceFailure($"MissingType({typeReference})"),
                    Unit.Value);
            }

            var response = await GetOpenAiResponse(model, new(request.OpenAiEndpoint), request.OpenAiKey, resourceType, request.Scenario);
            if (response is not {} body)
            {
                return (Unit.Value, null);
            }

            var bodySyntax = InsertResourceHandler.ConvertJsonElement(body);
            var insertContext = InsertResourceHandler.GetInsertContext(context, request.Position);

            var resource = new ResourceDeclarationSyntax(
                ImmutableArray<SyntaxBase>.Empty,
                SyntaxFactory.CreateIdentifierToken("resource"),
                SyntaxFactory.CreateIdentifier("aiGenerated"),
                SyntaxFactory.CreateStringLiteral(typeReference.FormatName()),
                null,
                SyntaxFactory.CreateToken(TokenType.Assignment),
                ImmutableArray<Token>.Empty,
                bodySyntax);
            var replacement = InsertResourceHandler.GenerateCodeReplacement(context.Compilation, resource, insertContext);

            await server.Workspace.ApplyWorkspaceEdit(new ApplyWorkspaceEditParams
            {
                Edit = new()
                {
                    Changes = new Dictionary<DocumentUri, IEnumerable<TextEdit>>
                    {
                        [request.TextDocument.Uri] = new[] {
                            new TextEdit
                            {
                                Range = replacement.ToRange(context.LineStarts),
                                NewText = replacement.Text,
                            },
                        },
                    },
                },
            }, cancellationToken);

            return (Unit.Value, BicepTelemetryEvent.InsertAiResourceSuccess(request.ResourceType, request.ApiVersion));
        });

    private async Task<JsonElement?> GetOpenAiResponse(SemanticModel model, Uri openAiEndpoint, string? openAiKey, ResourceType resourceType, string scenario)
    {
        var client = openAiProvider.CreateClient(model.Configuration, openAiEndpoint, openAiKey);
        var jsonSchema = ToJsonSchemaRecursive(resourceType.Body.Type);

        var systemPromps = """
You write code as queried by the user. Only output code. Wrap the code like that:

```json
{code}
```

Put explanations directly in the code as comments.
""";

        var prompt = $"""
Generate JSON for an Aure resource body of type '{resourceType.TypeReference.FormatType()}' and api version '{resourceType.TypeReference.ApiVersion}' to accomplish the following scenario: {scenario}.

Avoid using ARM Template expressions as much as possible.

Your response should adhere to the following JSON schema, omitting non-required properties if necessary:
```json
{jsonSchema}
```
""";

        //https://www.windmill.dev/blog/windmill-ai
        var response = await client.GetChatCompletionsAsync(
            "chatmodel",
            new ChatCompletionsOptions(new ChatMessage[] {
                new(ChatRole.System, systemPromps),
                new(ChatRole.User, prompt),
            }));

        var choice = response.Value.Choices.First();

        var matches = Regex.Matches(choice.Message.Content, "```json\n(.*)\n```", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        if (matches.FirstOrDefault() is not {} match ||
            match.Groups.Count < 2 ||
            match.Groups[1] is not {} group)
        {
            return null;
        }

        return JsonElementFactory.CreateElement(group.Value);
    }

    private JObject ToJsonSchemaRecursive(TypeSymbol type)
    {
        // TODO handle cycles!
        RuntimeHelpers.EnsureSufficientExecutionStack();

        switch (type)
        {
            case StringLiteralType _:
            case StringType _:
            case UnionType _:
                return new JObject {
                    ["type"] = "string"
                };
            case IntegerLiteralType _:
            case IntegerType _:
                return new JObject { 
                    ["type"] = "number"
                };
            case BooleanLiteralType _:
            case BooleanType _:
                return new JObject { 
                    ["type"] = "boolean"
                };
            case ArrayType arrayType:
                return new JObject {
                    ["type"] = "array",
                    ["items"] = ToJsonSchemaRecursive(arrayType.Item.Type)
                };
            case ObjectType objectType:
                var writableProps = objectType.Properties.Values.Where(x => !x.Flags.HasFlag(TypePropertyFlags.ReadOnly));
                var requiredProps = writableProps.Where(x => x.Flags.HasFlag(TypePropertyFlags.Required));

                var properties = writableProps.Select(x => new JProperty(x.Name, ToJsonSchemaRecursive(x.TypeReference.Type)));                
                return new JObject {
                    ["type"] = "object",
                    ["properties"] = new JObject(properties),
                    ["required"] = new JArray(requiredProps.Select(x => x.Name)),
                };
            case ResourceParentType _:
                return new JObject {};
            default:
                throw new NotImplementedException($"{type.GetType()}");
            // TODO discriminated object support
        }
    }
}