// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using LspDiagnostic = OmniSharp.Extensions.LanguageServer.Protocol.Models.Diagnostic;

namespace Bicep.LanguageServer.Extensions
{
    public static class DiagnosticExtensions
    {
        // "bicep.version" constraint not satisfied (DiagnosticBuilder.BicepVersionConstraintNotSatisfied).
        private const string BicepVersionConstraintNotSatisfiedCode = "BCP456";

        public static IEnumerable<LspDiagnostic> ToDiagnostics(this IEnumerable<IDiagnostic> source, ImmutableArray<int> lineStarts)
            => source.Select(diagnostic => CreateDiagnostic(diagnostic, lineStarts));

        private static LspDiagnostic CreateDiagnostic(IDiagnostic diagnostic, ImmutableArray<int> lineStarts)
            => new()
            {
                Severity = ToDiagnosticSeverity(diagnostic),
                Code = diagnostic.Code,
                Message = diagnostic.Message,
                Source = diagnostic.Source.ToSourceString(),
                Range = diagnostic.ToRange(lineStarts),
                Tags = ToDiagnosticTags(diagnostic.Styling),
                CodeDescription = GetDiagnosticDocumentation(diagnostic),
            };

        private static CodeDescription? GetDiagnosticDocumentation(IDiagnostic diagnostic)
            => diagnostic.Uri is { } ? new() { Href = diagnostic.Uri } : null;

        private static DiagnosticSeverity ToDiagnosticSeverity(IDiagnostic diagnostic)
        {
            var level = diagnostic.Level;

            // BCP456 (the "bicep.version" constraint violation) is shown as a warning instead of an error
            // when surfaced through VS Code. This stems from the current coupling where the language
            // server ships with and runs its own bundled Bicep binary, decoupled from the CLI used for
            // builds, so a stale extension shouldn't block editing with a false-positive error.
            if (diagnostic.Code == BicepVersionConstraintNotSatisfiedCode && level == DiagnosticLevel.Error)
            {
                level = DiagnosticLevel.Warning;
            }

            return level switch
            {
                DiagnosticLevel.Info => DiagnosticSeverity.Information,
                DiagnosticLevel.Warning => DiagnosticSeverity.Warning,
                DiagnosticLevel.Error => DiagnosticSeverity.Error,
                _ => throw new ArgumentException($"Unrecognized level {level}"),
            };
        }

        private static Container<DiagnosticTag>? ToDiagnosticTags(DiagnosticStyling styling) => styling switch
        {
            DiagnosticStyling.Default => null,
            DiagnosticStyling.ShowCodeAsUnused => new Container<DiagnosticTag>(DiagnosticTag.Unnecessary),
            DiagnosticStyling.ShowCodeDeprecated => new Container<DiagnosticTag>(DiagnosticTag.Deprecated),
            _ => throw new ArgumentException($"Unrecognized {nameof(DiagnosticStyling)} {styling}"),
        };
    }
}
