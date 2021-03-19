// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core;
using Bicep.Core.Diagnostics;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Diagnostic = Bicep.Core.Diagnostics.Diagnostic;

namespace Bicep.LanguageServer.Extensions
{
    public static class DiagnosticExtensions
    {
        public static IEnumerable<OmniSharp.Extensions.LanguageServer.Protocol.Models.Diagnostic> ToDiagnostics(this IEnumerable<IDiagnostic> source, ImmutableArray<int> lineStarts) =>
            source.Select(diagnostic => new OmniSharp.Extensions.LanguageServer.Protocol.Models.Diagnostic
            {
                Severity = ToDiagnosticSeverity(diagnostic.Level),
                Code = diagnostic.Code,
                Message = diagnostic.Message,
                Source = LanguageConstants.LanguageId,
                Range = diagnostic.ToRange(lineStarts),
                Tags = ToDiagnosticTags(diagnostic.Label),
            });

        private static DiagnosticSeverity ToDiagnosticSeverity(DiagnosticLevel level)
            => level switch
            {
                DiagnosticLevel.Info => DiagnosticSeverity.Information,
                DiagnosticLevel.Warning => DiagnosticSeverity.Warning,
                DiagnosticLevel.Error => DiagnosticSeverity.Error,
                _ => throw new ArgumentException($"Unrecognized level {level}"),
            };

        private static Container<DiagnosticTag>? ToDiagnosticTags(DiagnosticLabel? label) => label switch
        {
            null => null,
            DiagnosticLabel.Unnecessary => new Container<DiagnosticTag>(DiagnosticTag.Unnecessary),
            DiagnosticLabel.Deprecated => new Container<DiagnosticTag>(DiagnosticTag.Deprecated),
            _ => throw new ArgumentException($"Unrecognized label {label}"),
        };
    }
}

