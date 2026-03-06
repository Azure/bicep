// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Bicep.Core;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.Text;
using Bicep.Core.PrettyPrintV2;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Refactor;
using Bicep.LanguageServer.Telemetry;
using Bicep.LanguageServer.Utils;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer.Handlers;

[Method("bicep/extractToModule", Direction.ClientToServer)]
public record ExtractToModuleParams : IRequest<ExtractToModuleResponse>
{
    public required TextDocumentIdentifier TextDocument { get; init; }

    public required Range Range { get; init; }

    public required string ModuleFilePath { get; init; }
}

public record ExtractToModuleResponse
{
    public required Range ReplacementRange { get; init; }

    public required string ReplacementText { get; init; }

    public required string ModuleFileContents { get; init; }

    public Position? RenamePosition { get; init; }
}

public class ExtractToModuleHandler : IJsonRpcRequestHandler<ExtractToModuleParams, ExtractToModuleResponse>
{
    private readonly ICompilationManager compilationManager;
    private readonly ILanguageServerFacade server;
    private readonly TelemetryAndErrorHandlingHelper<ExtractToModuleResponse> helper;

    public ExtractToModuleHandler(ICompilationManager compilationManager, ILanguageServerFacade server, ITelemetryProvider telemetryProvider)
    {
        this.compilationManager = compilationManager;
        this.server = server;
        this.helper = new TelemetryAndErrorHandlingHelper<ExtractToModuleResponse>(server.Window, telemetryProvider);
    }

    public Task<ExtractToModuleResponse> Handle(ExtractToModuleParams request, CancellationToken cancellationToken)
        => helper.ExecuteWithTelemetryAndErrorHandling(async () =>
        {
            var context = compilationManager.GetCompilation(request.TextDocument.Uri);
            if (context is null)
            {
                throw helper.CreateException(
                    "Unable to locate an active compilation for the current document.",
                    BicepTelemetryEvent.ExtractToModuleFailure("MissingCompilation"),
                    CreateEmptyResponse(request.Range));
            }

            var semanticModel = context.Compilation.GetEntrypointSemanticModel();
            var selectionSpan = GetSelectionSpan(request, context.LineStarts);

            var selectedStatements = context.ProgramSyntax.Children.OfType<StatementSyntax>()
                .Where(statement => ContainsSpan(selectionSpan, statement.Span))
                .OrderBy(statement => statement.Span.Position)
                .ToList();

            if (selectedStatements.Count == 0)
            {
                throw helper.CreateException(
                    "Selection must include at least one complete top-level declaration.",
                    BicepTelemetryEvent.ExtractToModuleFailure("EmptySelection"),
                    CreateEmptyResponse(request.Range));
            }

            if (context.ProgramSyntax.Children.OfType<StatementSyntax>().Any(statement =>
                TextSpan.AreOverlapping(selectionSpan, statement.Span) && !ContainsSpan(selectionSpan, statement.Span)))
            {
                throw helper.CreateException(
                    "Selection must fully cover one or more top-level declarations.",
                    BicepTelemetryEvent.ExtractToModuleFailure("PartialStatementSelection"),
                    CreateEmptyResponse(request.Range));
            }

            if (selectedStatements.Any(s => s is not ResourceDeclarationSyntax))
            {
                throw helper.CreateException(
                    "Extract to module currently supports resource declarations only.",
                    BicepTelemetryEvent.ExtractToModuleFailure("UnsupportedDeclarationType"),
                    CreateEmptyResponse(request.Range));
            }

            var selectedResources = selectedStatements.Cast<ResourceDeclarationSyntax>().ToImmutableArray();
            var selectedSymbols = selectedResources
                .Select(resource => semanticModel.GetSymbolInfo(resource))
                .OfType<ResourceSymbol>()
                .ToImmutableArray();

            foreach (var symbol in selectedSymbols)
            {
                var externalReference = semanticModel.FindReferences(symbol)
                    .FirstOrDefault(reference => !ContainsSpan(selectionSpan, reference.Span));

                if (externalReference is not null)
                {
                    throw helper.CreateException(
                        "Selected resources are referenced outside the selection.",
                        BicepTelemetryEvent.ExtractToModuleFailure("ExternalReference"),
                        CreateEmptyResponse(request.Range));
                }
            }

            var declaredInsideSelection = selectedStatements
                .Select(syntax => semanticModel.Root.DeclarationsBySyntax.TryGetValue(syntax, out var symbol) ? symbol : null)
                .OfType<DeclaredSymbol>()
                .ToHashSet();

            var externalDependencies = CollectExternalDependencies(selectedStatements, semanticModel, declaredInsideSelection);
            var newline = semanticModel.Configuration.Formatting.Data.NewlineKind.ToEscapeSequence();

            var moduleParams = externalDependencies
                .OrderBy(dep => dep.DeclaringSyntax.Span.Position)
                .Select(dep => CreateModuleParam(dep, semanticModel))
                .ToList();

            var moduleFileContents = BuildModuleFileContents(moduleParams, selectedResources, semanticModel.SourceFile.Text, newline);

            var documentDirectory = Path.GetDirectoryName(request.TextDocument.Uri.GetFileSystemPath()) ?? string.Empty;
            var relativeModulePath = GetModuleRelativePath(request.ModuleFilePath, documentDirectory);
            var moduleSymbolName = FindModuleSymbolName(relativeModulePath, semanticModel);

            var replacementText = BuildModuleDeclarationText(moduleSymbolName, relativeModulePath, moduleParams, newline);
            var renamePosition = CalculateRenamePosition(request.Range, replacementText, moduleSymbolName);

            return (new ExtractToModuleResponse
            {
                ReplacementRange = request.Range,
                ReplacementText = replacementText,
                ModuleFileContents = moduleFileContents,
                RenamePosition = renamePosition,
            }, BicepTelemetryEvent.ExtractToModuleSuccess(moduleParams.Count, selectedResources.Length));
        });

    private static ExtractToModuleResponse CreateEmptyResponse(Range range) => new()
    {
        ReplacementRange = range,
        ReplacementText = string.Empty,
        ModuleFileContents = string.Empty,
        RenamePosition = null,
    };

    private static TextSpan GetSelectionSpan(ExtractToModuleParams request, ImmutableArray<int> lineStarts)
    {
        var startOffset = PositionHelper.GetOffset(lineStarts, request.Range.Start);
        var endOffset = PositionHelper.GetOffset(lineStarts, request.Range.End);

        return new TextSpan(startOffset, Math.Max(0, endOffset - startOffset));
    }

    private static bool ContainsSpan(TextSpan outer, TextSpan inner)
    {
        var outerEnd = outer.Position + outer.Length;
        var innerEnd = inner.Position + inner.Length;
        return outer.Position <= inner.Position && outerEnd >= innerEnd;
    }

    private static IEnumerable<DeclaredSymbol> CollectExternalDependencies(IEnumerable<StatementSyntax> selectedStatements, SemanticModel semanticModel, HashSet<DeclaredSymbol> declaredInsideSelection)
    {
        var collector = new DependencyCollector(semanticModel);
        foreach (var statement in selectedStatements)
        {
            collector.Visit(statement);
        }

        return collector.ReferencedSymbols
            .Where(symbol => symbol is ParameterSymbol or VariableSymbol)
            .Where(symbol => !declaredInsideSelection.Contains(symbol))
            .Cast<DeclaredSymbol>()
            .Distinct();
    }

    private static string CreateModuleParam(DeclaredSymbol symbol, SemanticModel semanticModel)
    {
        var type = semanticModel.GetTypeInfo(symbol.DeclaringSyntax);
        var typeString = TypeStringifier.Stringify(type, null, TypeStringifier.Strictness.Medium, removeTopLevelNullability: true);
        return $"{LanguageConstants.ParameterKeyword} {symbol.Name} {typeString}";
    }

    private static string BuildModuleFileContents(IEnumerable<string> moduleParams, IEnumerable<ResourceDeclarationSyntax> resources, string sourceText, string newline)
    {
        var builder = new StringBuilder();

        var paramList = moduleParams.ToList();
        foreach (var param in paramList)
        {
            builder.Append(param);
            builder.Append(newline);
        }

        if (paramList.Any())
        {
            builder.Append(newline);
        }

        var resourceList = resources.ToList();
        var firstResource = resourceList.First();
        var lastResource = resourceList.Last();
        var start = firstResource.Span.Position;
        var end = lastResource.GetEndPosition();
        var length = end - start;
        var selectedText = sourceText.Substring(start, length);

        builder.Append(selectedText.TrimEnd('\r', '\n'));
        builder.Append(newline);

        return builder.ToString();
    }

    private static string BuildModuleDeclarationText(string moduleSymbolName, string relativeModulePath, IEnumerable<string> moduleParams, string newline)
    {
        var paramList = moduleParams.ToList();
        var builder = new StringBuilder();
        builder.Append($"{LanguageConstants.ModuleKeyword} ");
        builder.Append(moduleSymbolName);
        builder.Append(" '");
        builder.Append(relativeModulePath);
        builder.Append("' = {");
        builder.Append(newline);
        builder.Append("  name: '");
        builder.Append(moduleSymbolName);
        builder.Append("'");

        if (paramList.Any())
        {
            builder.Append(newline);
            builder.Append($"  {LanguageConstants.ModuleParamsPropertyName}: {{");
            builder.Append(newline);
            foreach (var param in paramList)
            {
                var name = param.Split(' ', StringSplitOptions.RemoveEmptyEntries)[1];
                builder.Append("    ");
                builder.Append(name);
                builder.Append(": ");
                builder.Append(name);
                builder.Append(newline);
            }

            builder.Append("  }");
        }

        builder.Append(newline);
        builder.Append("}");
        builder.Append(newline);

        return builder.ToString();
    }

    private static Position? CalculateRenamePosition(Range replacementRange, string replacementText, string moduleSymbolName)
    {
        var lineStarts = TextCoordinateConverter.GetLineStarts(replacementText);
        var offset = replacementText.IndexOf(moduleSymbolName, StringComparison.Ordinal);
        if (offset < 0)
        {
            return null;
        }

        var relativePosition = TextCoordinateConverter.GetPosition(lineStarts, offset);

        var absoluteLine = replacementRange.Start.Line + relativePosition.line;
        var absoluteCharacter = relativePosition.line == 0
            ? replacementRange.Start.Character + relativePosition.character
            : relativePosition.character;

        return new Position(absoluteLine, absoluteCharacter);
    }

    private static string GetModuleRelativePath(string moduleFilePath, string documentDirectory)
    {
        var relative = Path.GetRelativePath(documentDirectory, moduleFilePath);
        relative = relative.Replace(Path.DirectorySeparatorChar, '/');
        if (!relative.StartsWith('.' ) && !relative.StartsWith('/'))
        {
            relative = "./" + relative;
        }

        return relative;
    }

    private static string FindModuleSymbolName(string relativeModulePath, SemanticModel semanticModel)
    {
        var baseName = Path.GetFileNameWithoutExtension(relativeModulePath);
        if (string.IsNullOrWhiteSpace(baseName))
        {
            baseName = "extractedModule";
        }

        baseName = Regex.Replace(baseName, "[^A-Za-z0-9_]", "_");
        if (!Lexer.IsValidIdentifier(baseName))
        {
            baseName = "extractedModule";
        }

        var candidate = baseName;
        var index = 1;
        while (semanticModel.Root.GetDeclarationsByName(candidate).Any())
        {
            candidate = $"{baseName}{index}";
            index++;
        }

        return candidate;
    }

    private sealed class DependencyCollector : CstVisitor
    {
        private readonly SemanticModel semanticModel;

        public DependencyCollector(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
            this.ReferencedSymbols = new HashSet<Symbol>();
        }

        public HashSet<Symbol> ReferencedSymbols { get; }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            if (semanticModel.GetSymbolInfo(syntax) is { } symbol)
            {
                ReferencedSymbols.Add(symbol);
            }

            base.VisitVariableAccessSyntax(syntax);
        }
    }
}
