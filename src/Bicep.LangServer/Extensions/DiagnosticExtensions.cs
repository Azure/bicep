// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Diagnostic = Bicep.Core.Diagnostics.Diagnostic;

namespace Bicep.LanguageServer.Extensions
{
    public static class DiagnosticExtensions
    {
        public static IEnumerable<OmniSharp.Extensions.LanguageServer.Protocol.Models.Diagnostic> ToDiagnostics(this IEnumerable<Diagnostic> source, ImmutableArray<int> lineStarts) =>
            source.Select(diagnostic => new OmniSharp.Extensions.LanguageServer.Protocol.Models.Diagnostic
            {
                Severity = ToDiagnosticSeverity(diagnostic.Level),
                Code = diagnostic.Code,
                Message = diagnostic.Message,
                Source = LanguageServerConstants.LanguageId,
                Range = diagnostic.ToRange(lineStarts)
            });

        private static DiagnosticSeverity ToDiagnosticSeverity(DiagnosticLevel level)
            => level switch {
                DiagnosticLevel.Info => DiagnosticSeverity.Information,
                DiagnosticLevel.Warning => DiagnosticSeverity.Warning,
                DiagnosticLevel.Error => DiagnosticSeverity.Error,
                _ => throw new ArgumentException($"Unrecognized level {level}"),
            };
    }
}

