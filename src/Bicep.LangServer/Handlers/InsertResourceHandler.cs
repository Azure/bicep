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
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol;
using Bicep.Core.Navigation;
using Bicep.Core.Rewriters;
using System.Text.RegularExpressions;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using Bicep.LanguageServer.Providers;

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

        public InsertResourceHandler(ILanguageServerFacade server, ICompilationManager compilationManager, IAzResourceProvider azResourceProvider, IAzResourceTypeLoader azResourceTypeLoader)
        {
            this.server = server;
            this.compilationManager = compilationManager;
            this.azResourceProvider = azResourceProvider;
            this.azResourceTypeLoader = azResourceTypeLoader;
        }

        public async Task<Unit> Handle(InsertResourceParams request, CancellationToken cancellationToken)
        {
            var context = compilationManager.GetCompilation(request.TextDocument.Uri.ToUri());
            if (context is null)
            {
                return Unit.Value;
            }

            if (!ResourceId.TryParse(request.ResourceId, out var resourceId))
            {
                server.Window.ShowError($"Failed to parse supplied resourceId '{request.ResourceId}'");
                return Unit.Value;
            }
            var fullyQualifiedType = resourceId.FormatFullyQualifiedType();

            var allTypes = azResourceTypeLoader.GetAvailableTypes()
                .ToLookup(x => x.FullyQualifiedType, StringComparer.OrdinalIgnoreCase);

            var matchedType = allTypes[fullyQualifiedType]
                .OrderByDescending(x => x.ApiVersion, ApiVersionComparer.Instance)
                .FirstOrDefault();

            if (matchedType is null)
            {
                server.Window.ShowError($"Failed to find Bicep types for resource of type '{fullyQualifiedType}'");
                return Unit.Value;
            }

            var resource = await azResourceProvider.GetGenericResource(
                context.Compilation.Configuration,
                resourceId,
                matchedType.ApiVersion,
                cancellationToken);

            var cursorOffset = PositionHelper.GetOffset(context.LineStarts, request.Position);
            var binder = context.Compilation.GetEntrypointSemanticModel().Binder;
            var node = context.ProgramSyntax.TryFindMostSpecificNodeInclusive(cursorOffset, n => binder.GetParent(n) is ProgramSyntax) ?? context.ProgramSyntax;
            var insertOffset = node.Span.Position + node.Span.Length;

            var resourceDeclaration = CreateResourceSyntax(resource, resourceId, matchedType);
            var replacement = GenerateCodeReplacement(context.Compilation, resourceDeclaration, new TextSpan(insertOffset, 0));

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

            return Unit.Value;
        }

        private CodeReplacement GenerateCodeReplacement(Compilation prevCompilation, ResourceDeclarationSyntax resourceDeclaration, TextSpan replacementSpan)
        {
            var program = new ProgramSyntax(
                new[] { resourceDeclaration },
                SyntaxFactory.CreateToken(TokenType.EndOfFile),
                ImmutableArray<IDiagnostic>.Empty);

            var printed = PrettyPrinter.PrintProgram(program, new PrettyPrintOptions(NewlineOption.LF, IndentKindOption.Space, 2, false));
            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri("inmemory:///generated.bicep"), printed);

            for (var i = 0; i < 5; i++)
            {
                var model = new SemanticModel(prevCompilation, bicepFile, prevCompilation.SourceFileGrouping.FileResolver, prevCompilation.Configuration);
                var updated = new TypeCasingFixerRewriter(model).Rewrite(bicepFile.ProgramSyntax);
                bicepFile = SourceFileFactory.CreateBicepFile(bicepFile.FileUri, updated.ToTextPreserveFormatting());

                model = new SemanticModel(prevCompilation, bicepFile, prevCompilation.SourceFileGrouping.FileResolver, prevCompilation.Configuration);
                updated = new ReadOnlyPropertyRemovalRewriter(model).Rewrite(bicepFile.ProgramSyntax);
                bicepFile = SourceFileFactory.CreateBicepFile(bicepFile.FileUri, updated.ToTextPreserveFormatting());
            }

            printed = PrettyPrinter.PrintProgram(bicepFile.ProgramSyntax, new PrettyPrintOptions(NewlineOption.LF, IndentKindOption.Space, 2, false));
            return new CodeReplacement(replacementSpan, "\n" + printed);
        }

        private static ResourceDeclarationSyntax CreateResourceSyntax(JsonElement resource, ResourceId resourceId, ResourceTypeReference typeReference)
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
                            SyntaxFactory.CreateStringLiteral(resourceId.FormatName())));
                        break;
                    default:
                        properties.Add(SyntaxFactory.CreateObjectProperty(
                            property.Name,
                            ConvertJsonElement(property.Value)));
                        break;
                }
            }

            return new ResourceDeclarationSyntax(
                ImmutableArray<SyntaxBase>.Empty,
                SyntaxFactory.CreateToken(TokenType.Identifier, "resource"),
                SyntaxFactory.CreateIdentifier(Regex.Replace(resourceId.NameHierarchy.Last(), "[^a-zA-Z]", "")),
                SyntaxFactory.CreateStringLiteral(typeReference.FormatName()),
                null,
                SyntaxFactory.CreateToken(TokenType.Assignment),
                SyntaxFactory.CreateObject(properties));
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
                    if (element.TryGetInt32(out var intValue))
                    {
                        return SyntaxFactory.CreateIntegerLiteral(element.GetInt32()!);
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