// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using static Bicep.Core.UnitTests.Utils.CompilationHelper;

namespace Bicep.Core.UnitTests.Utils;

public static class ICompilationResultExtensions
{
    public static CursorLookupResult GetInfoAtCursor(this ICompilationResult result, int cursor)
    {
        var model = result.Compilation.GetEntrypointSemanticModel();

        var node = result.SourceFile.ProgramSyntax.TryFindMostSpecificNodeInclusive(
            cursor,
            n => n is not IdentifierSyntax && n is not Token);

        node.Should().NotBeNull();

        var symbol = model.GetSymbolInfo(node!);
        var type = model.GetTypeInfo(node!);

        symbol.Should().NotBeNull();

        return new(node!, symbol!, type);
    }

    public static ImmutableArray<CursorLookupResult> GetInfoAtCursors(this ICompilationResult result, IEnumerable<int> cursors)
        => cursors.Select(x => GetInfoAtCursor(result, x)).ToImmutableArray();

    public static string ApplyCodeFix(this ICompilationResult result, IDiagnostic diagnostic)
    {
        if (diagnostic is not IFixable fixable)
        {
            throw new InvalidOperationException("Diagnostic is not fixable");
        }

        // TODO - support multiple fixes / replacements
        var fix = fixable.Fixes.Single();
        var replacement = fix.Replacements.Single();

        var originalFile = result.SourceFile.GetOriginalSource();

        return string.Concat(
            originalFile.AsSpan(0, replacement.Span.Position),
            replacement.Text,
            originalFile.AsSpan(replacement.Span.GetEndPosition()));
    }
}
