// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class DiagnosticsPragmaSyntaxTrivia : SyntaxTrivia
{
    public DiagnosticsPragmaSyntaxTrivia(DiagnosticsPragmaType pragmaType, TextSpan span, string text, IEnumerable<Token> diagnosticCodes)
        : base(SyntaxTriviaType.DiagnosticsPragma, span, text)
    {
        PragmaType = pragmaType;
        DiagnosticCodes = [.. diagnosticCodes];
    }

    public DiagnosticsPragmaType PragmaType { get; }

    public ImmutableArray<Token> DiagnosticCodes { get; }
}

public enum DiagnosticsPragmaType
{
    Disable,
    Restore,
    DisableNextLine,
}

public static class DiagnosticsPragmaTypeExtensions
{
    public static string GetKeyword(this DiagnosticsPragmaType pragmaType) => pragmaType switch
        {
            DiagnosticsPragmaType.Disable => LanguageConstants.DisableDiagnosticsKeyword,
            DiagnosticsPragmaType.Restore => LanguageConstants.RestoreDiagnosticsKeyword,
            DiagnosticsPragmaType.DisableNextLine => LanguageConstants.DisableNextLineDiagnosticsKeyword,
            _ => throw new ArgumentOutOfRangeException(nameof(pragmaType), pragmaType, null),
        };
}
