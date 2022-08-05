// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    public class LinterRuleTestsBase
    {
        public enum OnCompileErrors
        {
            IncludeErrors, // Include compile errors in the list of messages to expect
            Ignore, // Ignore any compile errors
            IncludeErrorsAndWarnings, // Include compile errors and warnings in the list of messages to expect

            Default = IncludeErrors
        }

        public enum IncludePosition
        {
            None,
            LineNumber,

            Default = LineNumber,
        }

        public record Options(
            OnCompileErrors OnCompileErrors = OnCompileErrors.Default,
            IncludePosition IncludePosition = IncludePosition.Default,
            RootConfiguration? Configuration = null,
            ApiVersionProvider? ApiVersionProvider = null
        );

        private static string FormatDiagnostic(IDiagnostic diagnostic, ImmutableArray<int> lineStarts, IncludePosition includePosition)
        {
            if (includePosition == IncludePosition.LineNumber)
            {
                var position = TextCoordinateConverter.GetPosition(lineStarts, diagnostic.Span.Position);
                return $"[{position.line+1}] {diagnostic.Message}";
            }
            else
            {
                return diagnostic.Message;
            }
        }

        protected static void AssertLinterRuleDiagnostics(string ruleCode, string bicepText, string[] expectedMessagesForCode, Options? options = null)
        {
            AssertLinterRuleDiagnostics(ruleCode, bicepText, diags =>
            {
                var lineStarts = TextCoordinateConverter.GetLineStarts(bicepText);
                var messages = diags.Select(d => FormatDiagnostic(d, lineStarts, options?.IncludePosition ?? IncludePosition.Default));
                messages.Should().BeEquivalentTo(expectedMessagesForCode);
            },
            options);
        }

        protected static void AssertLinterRuleDiagnostics(string ruleCode, string bicepText, int expectedDiagnosticCountForCode, Options? options = null)
        {
            AssertLinterRuleDiagnostics(ruleCode, bicepText, diags =>
            {
                diags.Should().HaveCount(expectedDiagnosticCountForCode);
            },
            options);
        }

        protected static void AssertLinterRuleDiagnostics(string ruleCode, string bicepText, Action<IEnumerable<IDiagnostic>> assertAction, Options? options = null)
        {
            options ??= new Options();
            RunWithDiagnosticAnnotations(
                bicepText,
                diag =>
                    diag.Code == ruleCode
                    || (IsCompilerDiagnostic(diag) && options.OnCompileErrors == OnCompileErrors.IncludeErrors && diag.Level == DiagnosticLevel.Error)
                    || (IsCompilerDiagnostic(diag) && options.OnCompileErrors == OnCompileErrors.IncludeErrorsAndWarnings && (diag.Level == DiagnosticLevel.Error || diag.Level == DiagnosticLevel.Warning)),
                assertAction,
                options);
        }

        private static void RunWithDiagnosticAnnotations(
            string bicepText,
            Func<IDiagnostic, bool> filterFunc,
            Action<IEnumerable<IDiagnostic>> assertAction,
            Options? options)
        {
            options ??= new Options();
            var context = new CompilationHelper.CompilationHelperContext(Configuration: options.Configuration, ApiVersionProvider: options.ApiVersionProvider);
            var result = CompilationHelper.Compile(context, bicepText);
            using (new AssertionScope().WithFullSource(result.BicepFile))
            {
                result.Should().NotHaveDiagnosticsWithCodes(new[] { LinterAnalyzer.LinterRuleInternalError }, "There should never be linter LinterRuleInternalError errors");

                IDiagnostic[] diagnosticsMatchingCode = result.Diagnostics.Where(filterFunc).ToArray();
                DiagnosticAssertions.DoWithDiagnosticAnnotations(
                    result.Compilation.SourceFileGrouping.EntryPoint,
                    result.Diagnostics.Where(filterFunc),
                    assertAction);
            }
        }

        private static bool IsCompilerDiagnostic(IDiagnostic diagnostic)
        {
            return diagnostic.Code.StartsWith("BCP");
        }
    }
}
