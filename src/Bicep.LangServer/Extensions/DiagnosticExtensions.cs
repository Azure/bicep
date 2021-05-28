// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.LanguageServer.Extensions
{
    public static class DiagnosticExtensions
    {
        public static IEnumerable<OmniSharp.Extensions.LanguageServer.Protocol.Models.Diagnostic> ToDiagnostics(this IEnumerable<IDiagnostic> source, ImmutableArray<int> lineStarts)
            => source.Select(diagnostic => CreateDiagnostic(diagnostic, lineStarts));

        private static OmniSharp.Extensions.LanguageServer.Protocol.Models.Diagnostic CreateDiagnostic(IDiagnostic diagnostic, ImmutableArray<int> lineStarts)
        {
            var (descCode, codeDescription) = GetDiagnosticDocumentation(diagnostic);
            return new OmniSharp.Extensions.LanguageServer.Protocol.Models.Diagnostic()
            {
                Severity = ToDiagnosticSeverity(diagnostic.Level),
                Code = descCode ?? diagnostic.Code,
                Message = diagnostic.Message,
                Source = diagnostic.Source,
                Range = diagnostic.ToRange(lineStarts),
                Tags = ToDiagnosticTags(diagnostic.Label),
                CodeDescription = codeDescription
            };
        }

        private static (string?, CodeDescription?) GetDiagnosticDocumentation(IDiagnostic diagnostic)
        {
            if (diagnostic.Uri != null)
            {
                // This shuffling of the Code to Uri gives us the message formatting
                // that is desired where the documentation link is displayed as the text
                // of the link. Otherwise the code is displayed rather than the Uri
                //
                // Default message format:
                //   Declared parameter must be referenced within the document scope. bicep core(no-unused-params) [2,7]
                //
                // Desired format:
                //   Declared parameter must be referenced within the document scope. bicep core(https://aka.ms/bicep/linter/no-unused-params) [2,7]

                return new(diagnostic.Uri.AbsoluteUri, new CodeDescription() { Href = diagnostic.Uri });
            }

            // no additional documentation
            return new(null, null);
        }

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

