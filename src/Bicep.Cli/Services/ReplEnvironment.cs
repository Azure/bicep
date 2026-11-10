// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO.Abstractions;
using System.Text;
using Bicep.Cli.Helpers.Repl;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.Text;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.Services;

public class ReplEnvironment
{
    private readonly InMemoryFileExplorer fileExplorer = new();
    private readonly BicepCompiler compiler;
    private readonly ReplEvaluator evaluator;

    // Auxiliary directory for resolving relative paths for load* functions.
    private readonly IDirectoryHandle auxiliaryDirectory;

    private static readonly IOUri replFileUri = IOUri.FromFilePath("/session.biceprepl");
    private readonly List<string> history = [];
    private int lastHistoryIndex = -1;

    public ReplEnvironment(
        BicepCompiler compiler,
        IFileSystem auxiliaryFileSystem,
        IFileExplorer auxiliaryFileExplorer)
    {
        this.compiler = compiler;
        this.auxiliaryDirectory = auxiliaryFileExplorer.GetDirectory(IOUri.FromFilePath(auxiliaryFileSystem.Directory.GetCurrentDirectory()));
        this.evaluator = new(CompileInternal);
    }

    public string HighlightInputLine(string prefix, string prevLines, IReadOnlyList<Rune> lineBuffer, int cursorOffset, bool printPrevLines, int terminalWidth = int.MaxValue, int? previousCursorOffset = null)
    {
        var currentLine = string.Concat(lineBuffer);
        var fullContent = $"{prevLines}{currentLine}";
        var lineStart = prevLines.Length;

        var compilation = CompileInternal(fullContent);
        var model = compilation.GetEntrypointSemanticModel();
        var width = lineBuffer.Take(cursorOffset).Sum(r => r.Utf16SequenceLength);

        if (printPrevLines)
        {
            var historyHighlighted = PrintHelper.PrintWithSyntaxHighlighting(model, fullContent, 0);

            return PrintHelper.PrintInputLine(prefix, historyHighlighted, width, terminalWidth, previousCursorOffset);
        }

        var highlighted = PrintHelper.PrintWithSyntaxHighlighting(model, fullContent, lineStart);

        return PrintHelper.PrintInputLine(prefix, highlighted, width, terminalWidth, previousCursorOffset);
    }

    public string HighlightSyntax(SyntaxBase syntax)
    {
        var context = PrettyPrinterV2Context.Create(PrettyPrinterV2Options.Default, EmptyDiagnosticLookup.Instance, EmptyDiagnosticLookup.Instance);
        var content = PrettyPrinterV2.Print(syntax, context);

        return HighlightSyntax(content);
    }

    public string HighlightSyntax(string content)
    {
        var compilation = CompileInternal(content);
        var model = compilation.GetEntrypointSemanticModel();

        return PrintHelper.PrintWithSyntaxHighlighting(model, content);
    }

    public string EvaluateAndGetOutput(string input)
    {
        var result = EvaluateInput(input);

        if (result.Value is { } value)
        {
            return HighlightSyntax(value) + '\n';
        }
        else if (result.AnnotatedDiagnostics.Any())
        {
            return PrintHelper.PrintWithAnnotations(input, result.AnnotatedDiagnostics, HighlightSyntax(input));
        }

        return string.Empty;
    }

    public AnnotatedReplResult EvaluateInput(string input)
    {
        history.Add(input);
        lastHistoryIndex = -1;

        var result = evaluator.Evaluate(input);
        return new(ParseJToken(result.Value), CreateAnnotatedDiagnostics(result.Diagnostics, result.UserText, result.CompiledTextLength));
    }

    public string? TryGetHistory(bool backwards)
    {
        if (backwards)
        {
            if (lastHistoryIndex <= -1)
            {
                lastHistoryIndex = history.Count - 1;
            }
            else if (lastHistoryIndex > 0)
            {
                lastHistoryIndex--;
            }
            else
            {
                // remain at oldest entry
            }
        }
        else
        {
            if (lastHistoryIndex > -1 && lastHistoryIndex < history.Count - 1)
            {
                lastHistoryIndex++;
            }
            else
            {
                // remain at newest entry
            }
        }

        if (lastHistoryIndex == -1)
        {
            return null;
        }

        return history[lastHistoryIndex].TrimEnd('\n');
    }

    private Compilation CompileInternal(string fullContent)
    {
        var fileHandle = fileExplorer.GetFile(replFileUri);
        var sourceFile = compiler.SourceFileFactory.CreateBicepReplFile(fileHandle, this.auxiliaryDirectory, fullContent);
        var workspace = new ActiveSourceFileSet();
        workspace.UpsertSourceFile(sourceFile);

        return compiler.CreateCompilationWithoutRestore(replFileUri, workspace);
    }

    // TODO: There's probably a better way to do this...
    private static IEnumerable<PrintHelper.AnnotatedDiagnostic> CreateAnnotatedDiagnostics(IEnumerable<IDiagnostic> diagnostics, string userText, int compiledTextLength)
    {
        // Calculate the offset where user's text starts in the compiled content
        // User's text is always at the end, so: totalLength - userTextLength = startOffset
        var textOffset = compiledTextLength - userText.Length;
        var annotatedDiagnostics = diagnostics
            .Where(d => d.Span.Position >= textOffset && d.Span.Position <= compiledTextLength) // only diagnostics that overlap with user text
            .Select(d => new PrintHelper.AnnotatedDiagnostic(d, () => new TextSpan(d.Span.Position - textOffset, d.Span.Length)));
        return annotatedDiagnostics;
    }

    [return: NotNullIfNotNull(nameof(value))]
    private static SyntaxBase? ParseJToken(JToken? value)
        => value switch
        {
            JObject jObject => ParseJObject(jObject),
            JArray jArray => ParseJArray(jArray),
            JValue jValue => ParseJValue(jValue),
            null => null,
            _ => throw new NotImplementedException($"Unrecognized token type {value.Type}"),
        };

    private static SyntaxBase ParseJValue(JValue value)
        => value.Type switch
        {
            JTokenType.Integer => SyntaxFactory.CreatePositiveOrNegativeInteger(value.Value<long>()),
            JTokenType.String => SyntaxFactory.CreateStringLiteral(value.ToString()),
            JTokenType.Boolean => SyntaxFactory.CreateBooleanLiteral(value.Value<bool>()),
            // Floats are currently not supported in Bicep, so fallback to string syntax
            JTokenType.Float => SyntaxFactory.CreateStringLiteral(value.ToString(CultureInfo.InvariantCulture)),
            JTokenType.Null => SyntaxFactory.CreateNullLiteral(),
            _ => throw new NotImplementedException($"Unrecognized token type {value.Type}"),
        };

    private static SyntaxBase ParseJArray(JArray jArray)
        => SyntaxFactory.CreateArray(
            jArray.Select(ParseJToken).WhereNotNull());

    private static SyntaxBase ParseJObject(JObject jObject)
        => SyntaxFactory.CreateObject(
            jObject.Properties()
                .Select(x => SyntaxFactory.CreateObjectProperty(x.Name, ParseJToken(x.Value))));

    /// <summary>
    /// Heuristic structural completeness check: ensures bracket/brace/paren balance
    /// and not inside (multi-line) string or interpolation expression.
    /// Not a full parse; parse errors still reported by real parser.
    /// </summary>
    public static ReplBufferState GetBufferState(string text, string currentLine)
    {
        // If line is blank, submit - even if structurally incomplete.
        // This avoids trapping the user in a state where they cannot recover.
        if (string.IsNullOrWhiteSpace(currentLine))
        {
            return new ReplBufferState(
                ShouldSubmit: true,
                IsTypeDeclaration: false);
        }

        var program = new ReplParser(text).Program();

        // Use the existence of skipped trivia to determine whether we have a complete structure.
        var allNodes = SyntaxAggregator.Aggregate(program, x => x is not Token, useCst: true);
        var shouldSubmit = allNodes.Last() is not SkippedTriviaSyntax;

        var finalExpression = program.Children
            .Where(x => x is not Token { Type: TokenType.NewLine })
            .LastOrDefault();

        return new ReplBufferState(
            ShouldSubmit: shouldSubmit,
            IsTypeDeclaration: finalExpression is TypeDeclarationSyntax);
    }

    public static bool ShouldSubmitBuffer(string text, string currentLine)
        => GetBufferState(text, currentLine).ShouldSubmit;
}

public record AnnotatedReplResult(SyntaxBase? Value, IEnumerable<PrintHelper.AnnotatedDiagnostic> AnnotatedDiagnostics);

public record ReplBufferState(bool ShouldSubmit, bool IsTypeDeclaration);
