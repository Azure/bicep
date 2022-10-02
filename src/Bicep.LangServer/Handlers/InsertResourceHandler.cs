// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Deployments.Core.Comparers;
using Azure.Deployments.Core.Definitions.Identifiers;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Az;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Extensions;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Bicep.Core.Extensions;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol;
using Bicep.Core.Navigation;
using Bicep.Core.Rewriters;
using System.Text.RegularExpressions;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Telemetry;
using System.Diagnostics;

namespace Bicep.LanguageServer.Handlers
{
    [Method("textDocument/insertResource", Direction.ClientToServer)]
    public record InsertResourceParams : TextDocumentPositionParams, IRequest
    {
        public string? ResourceId { get; init; }
    }

    public class InsertResourceHandler : IJsonRpcNotificationHandler<InsertResourceParams>
    {
        private readonly ILanguageServerFacade server;
        private readonly ICompilationManager compilationManager;
        private readonly IAzResourceProvider azResourceProvider;
        private readonly IAzResourceTypeLoader azResourceTypeLoader;
        private readonly TelemetryAndErrorHandlingHelper<Unit> helper;

        public InsertResourceHandler(ILanguageServerFacade server, ICompilationManager compilationManager, IAzResourceProvider azResourceProvider, IAzResourceTypeLoader azResourceTypeLoader, ITelemetryProvider telemetryProvider)
        {
            this.server = server;
            this.compilationManager = compilationManager;
            this.azResourceProvider = azResourceProvider;
            this.azResourceTypeLoader = azResourceTypeLoader;
            this.helper = new TelemetryAndErrorHandlingHelper<Unit>(server.Window, telemetryProvider, Unit.Value);
        }

        public Task<Unit> Handle(InsertResourceParams request, CancellationToken cancellationToken)
            => helper.ExecuteWithTelemetryAndErrorHandling(async () => {
                var context = compilationManager.GetCompilation(request.TextDocument.Uri);
                if (context is null)
                {
                    return (Unit.Value, null);
                }

                var model = context.Compilation.GetEntrypointSemanticModel();

                if (TryParseResourceId(request.ResourceId) is not { } resourceId)
                {
                    throw new TelemetryAndErrorHandlingException(
                        $"Failed to parse supplied resourceId \"{request.ResourceId}\".",
                        BicepTelemetryEvent.InsertResourceFailure("ParseResourceIdFailed"));
                }

                var matchedType = azResourceTypeLoader.GetAvailableTypes()
                    .Where(x => StringComparer.OrdinalIgnoreCase.Equals(resourceId.FullyQualifiedType, x.FormatType()))
                    .OrderByDescending(x => x.ApiVersion, ApiVersionComparer.Instance)
                    .FirstOrDefault();

                if (matchedType is null || matchedType.ApiVersion is null)
                {
                    throw new TelemetryAndErrorHandlingException(
                        $"Failed to find a Bicep type definition for resource of type \"{resourceId.FullyQualifiedType}\".",
                        BicepTelemetryEvent.InsertResourceFailure($"MissingType({resourceId.FullyQualifiedType})"));
                }

                JsonElement? resource = null;
                try
                {
                    // First attempt a direct GET on the resource using the Bicep type API version.
                    resource = await azResourceProvider.GetGenericResource(
                        model.Configuration,
                        resourceId,
                        apiVersion: matchedType.ApiVersion,
                        cancellationToken);
                }
                catch (Exception exception)
                {
                    // We want to keep going here - we'll try again without the API version.
                    Trace.WriteLine($"Failed to fetch resource '{resourceId}' with API version {matchedType.ApiVersion}: {exception}");
                }

                try
                {
                    // If the direct GET fails, attempt to look it up without the API version.
                    // This will use the latest version from the /providers/<provider> API.
                    if (resource is null)
                    {
                        resource = await azResourceProvider.GetGenericResource(
                            model.Configuration,
                            resourceId,
                            apiVersion: null,
                            cancellationToken);
                    }
                }
                catch (Exception exception)
                {
                    Trace.WriteLine($"Failed to fetch resource '{resourceId}' without API version: {exception}");

                    throw new TelemetryAndErrorHandlingException(
                        $"Caught exception fetching resource: {exception.Message}.",
                        BicepTelemetryEvent.InsertResourceFailure($"FetchResourceFailure"));
                }

                var resourceDeclaration = CreateResourceSyntax(resource.Value, resourceId, matchedType);
                var insertContext = GetInsertContext(context, request.Position);
                var replacement = GenerateCodeReplacement(context.Compilation, resourceDeclaration, insertContext);

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

                return (Unit.Value, BicepTelemetryEvent.InsertResourceSuccess(resourceId.FullyQualifiedType, matchedType.ApiVersion));
            });

        private record InsertContext(
            bool StartWithNewline,
            bool EndWithNewline,
            int InsertOffset);

        private static InsertContext GetInsertContext(CompilationContext context, Position requestPosition)
        {
            var cursorOffset = PositionHelper.GetOffset(context.LineStarts, requestPosition);
            var binder = context.Compilation.GetEntrypointSemanticModel().Binder;
            var node = context.ProgramSyntax.TryFindMostSpecificNodeInclusive(cursorOffset, n => binder.GetParent(n) is ProgramSyntax);
            if (node is null)
            {
                return new(true, false, context.ProgramSyntax.GetEndPosition());
            }

            var programChildren = context.ProgramSyntax.Children;
            var index = programChildren.IndexOf(node);
            var nextNode = index < (programChildren.Length - 1) ? programChildren[index + 1] : null;

            var startNewline = node is not Token { Type: TokenType.NewLine };
            var endNewline = nextNode is not null and not Token { Type: TokenType.NewLine };
            var insertOffset = node.GetEndPosition();

            return new(startNewline, endNewline, insertOffset);
        }

        private static CodeReplacement GenerateCodeReplacement(Compilation prevCompilation, ResourceDeclarationSyntax resourceDeclaration, InsertContext insertContext)
        {
            // Create a new document containing the resource to insert.
            // This allows us to apply syntax rewriters and formatting, before generating the code replacement.
            var printOptions = new PrettyPrintOptions(NewlineOption.LF, IndentKindOption.Space, 2, false);
            var program = new ProgramSyntax(
                new[] { resourceDeclaration },
                SyntaxFactory.CreateToken(TokenType.EndOfFile),
                ImmutableArray<IDiagnostic>.Empty);

            var printed = PrettyPrinter.PrintProgram(program, printOptions);
            var bicepFile = RewriterHelper.RewriteMultiple(
                prevCompilation,
                SourceFileFactory.CreateBicepFile(new Uri("inmemory:///generated.bicep"), printed),
                rewritePasses: 5,
                model => new TypeCasingFixerRewriter(model),
                model => new ReadOnlyPropertyRemovalRewriter(model));

            printed = PrettyPrinter.PrintProgram(bicepFile.ProgramSyntax, printOptions);
            if (insertContext.StartWithNewline)
            {
                printed = "\n" + printed;
            }
            if (insertContext.EndWithNewline)
            {
                printed = printed + "\n";
            }

            return new CodeReplacement(new TextSpan(insertContext.InsertOffset, 0), printed);
        }

        private static ResourceDeclarationSyntax CreateResourceSyntax(JsonElement resource, IAzResourceProvider.AzResourceIdentifier resourceId, ResourceTypeReference typeReference)
        {
            var properties = new List<ObjectPropertySyntax>();
            foreach (var property in resource.EnumerateObject())
            {
                switch (property.Name.ToLowerInvariant())
                {
                    case "id":
                    case "type":
                    case "apiVersion":
                        // Don't add these to the resource properties - they're part of the resource declaration.
                        break;
                    case "name":
                        // Use the fully-qualified name instead of the name returned by the RP.
                        properties.Add(SyntaxFactory.CreateObjectProperty(
                            "name",
                            SyntaxFactory.CreateStringLiteral(resourceId.FullyQualifiedName)));
                        break;
                    default:
                        properties.Add(SyntaxFactory.CreateObjectProperty(
                            property.Name,
                            ConvertJsonElement(property.Value)));
                        break;
                }
            }

            var description = SyntaxFactory.CreateDecorator(
                "description",
                SyntaxFactory.CreateStringLiteral($"Generated from {resourceId.FullyQualifiedId}"));

            return new ResourceDeclarationSyntax(
                new SyntaxBase[] { description, SyntaxFactory.NewlineToken, },
                SyntaxFactory.CreateToken(TokenType.Identifier, "resource"),
                SyntaxFactory.CreateIdentifier(Regex.Replace(resourceId.UnqualifiedName, "[^a-zA-Z]", "")),
                SyntaxFactory.CreateStringLiteral(typeReference.FormatName()),
                null,
                SyntaxFactory.CreateToken(TokenType.Assignment),
                SyntaxFactory.CreateObject(properties));
        }

        private static IAzResourceProvider.AzResourceIdentifier? TryParseResourceId(string? resourceIdString)
        {
            if (resourceIdString is null)
            {
                return null;
            }

            if (ResourceId.TryParse(resourceIdString, out var resourceId))
            {
                return new(
                    resourceId.FullyQualifiedId,
                    resourceId.FormatFullyQualifiedType(),
                    resourceId.FormatName(),
                    resourceId.NameHierarchy.Last(),
                    string.Empty);
            }

            var rgRegexOptions = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant;
            var rgRegex = new Regex(@"^/subscriptions/(?<subId>[^/]+)/resourceGroups/(?<rgName>[^/]+)$", rgRegexOptions);
            var rgRegexMatch = rgRegex.Match(resourceIdString);
            if (rgRegexMatch.Success)
            {
                return new(
                    resourceIdString,
                    "Microsoft.Resources/resourceGroups",
                    rgRegexMatch.Groups["rgName"].Value,
                    rgRegexMatch.Groups["rgName"].Value,
                    rgRegexMatch.Groups["subId"].Value);
            }

            return null;
        }

        private static SyntaxBase ConvertJsonElement(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    var properties = new List<ObjectPropertySyntax>();
                    foreach (var property in element.EnumerateObject())
                    {
                        properties.Add(SyntaxFactory.CreateObjectProperty(property.Name, ConvertJsonElement(property.Value)));
                    }
                    return SyntaxFactory.CreateObject(properties);
                case JsonValueKind.Array:
                    var items = new List<SyntaxBase>();
                    foreach (var value in element.EnumerateArray())
                    {
                        items.Add(ConvertJsonElement(value));
                    }
                    return SyntaxFactory.CreateArray(items);
                case JsonValueKind.String:
                    return SyntaxFactory.CreateStringLiteral(element.GetString()!);
                case JsonValueKind.Number:
                    if (element.TryGetInt64(out long intValue))
                    {
                        return SyntaxFactory.CreatePositiveOrNegativeInteger(intValue);
                    }
                    return SyntaxFactory.CreateStringLiteral(element.ToString()!);
                case JsonValueKind.True:
                    return SyntaxFactory.CreateToken(TokenType.TrueKeyword);
                case JsonValueKind.False:
                    return SyntaxFactory.CreateToken(TokenType.FalseKeyword);
                case JsonValueKind.Null:
                    return SyntaxFactory.CreateToken(TokenType.NullKeyword);
                default:
                    throw new InvalidOperationException($"Failed to deserialize JSON");
            }
        }
    }
}
