// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using Bicep.Core.CodeAction;
using Bicep.Core.Text;

namespace Bicep.Core.Diagnostics;

// roughly equivalent to the 'SyntaxDiagnosticInfo' class in Roslyn
[DebuggerDisplay("Level = {" + nameof(Level) + "}, Code = {" + nameof(Code) + "}, Message = {" + nameof(Message) + "}")]
public record Diagnostic(
    TextSpan Span,
    DiagnosticLevel Level,
    DiagnosticSource Source,
    string Code,
    string Message) : IDiagnostic, IFixable
{
    public Uri? Uri { get; init; }

    public DiagnosticStyling Styling { get; init; } = DiagnosticStyling.Default;

    public ImmutableArray<CodeFix> Fixes { get; init; } = [];

    IEnumerable<CodeFix> IFixable.Fixes => Fixes;

    public Diagnostic WithAppendedFixes(params CodeFix[] fixes) => this with { Fixes = [.. this.Fixes, .. fixes] };
}
