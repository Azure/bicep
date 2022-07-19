// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter;
using Bicep.Core.ApiVersion;
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
            Ignore, // Ignore any compile errors
            IncludeErrors, // Include compile errors in the list of messages to expect
            IncludeErrorsAndWarnings, // Include compile errors and warnings in the list of messages to expect
        }

        public enum IncludePosition
        {
            None,
            LineNumber,
        }

        private string FormatDiagnostic(IDiagnostic diagnostic, ImmutableArray<int> lineStarts, IncludePosition includePosition)
        {
            if (includePosition == IncludePosition.LineNumber)
            {
                var position = TextCoordinateConverter.GetPosition(lineStarts, diagnostic.Span.Position);
                return $"[{position.line}] {diagnostic.Message}";
            }
            else
            {
                return diagnostic.Message;
            }
        }

        //asdfg ?? 
        static protected (string fileName, string fileContents) MainBicepFile(string bicepText)
        {
            return ("main.bicep", bicepText);
        }

        static protected (string fileName, string fileContents) ConfigFile(string configurationText)
        {
            return ("bicepconfig.json", configurationText);
        }

        static protected (string fileName, string fileContents)[] BicepFiles(string bicepText, string? configurationText)
        {
            List<(string fileName, string fileContents)> files = new List<(string fileName, string fileContents)>();
            files.Add(MainBicepFile(bicepText));
            if (configurationText is not null)
            {
                files.Add(ConfigFile(configurationText));
            }

            return files.ToArray();
        }

        protected void AssertLinterRuleDiagnostics(string ruleCode, string bicepText, string[] expectedMessagesForCode, OnCompileErrors onCompileErrors = OnCompileErrors.IncludeErrors, IncludePosition includePosition = IncludePosition.None, RootConfiguration? configuration = null, IApiVersionProvider? apiVersionProvider = null)
        {
            AssertLinterRuleDiagnostics(ruleCode, bicepText, onCompileErrors, diags =>
            {
                var lineStarts = TextCoordinateConverter.GetLineStarts(bicepText);
                var messages = diags.Select(d => FormatDiagnostic(d, lineStarts, includePosition));
                messages.Should().BeEquivalentTo(expectedMessagesForCode);
            },
            configuration,
            apiVersionProvider);
        }

        protected void AssertLinterRuleDiagnostics(string ruleCode, string bicepText, int expectedDiagnosticCountForCode, OnCompileErrors onCompileErrors = OnCompileErrors.IncludeErrors, IncludePosition includePosition/*asdfg not used*/ = IncludePosition.None, RootConfiguration? configuration = null, IApiVersionProvider? apiVersionProvider = null)
        {
            AssertLinterRuleDiagnostics(ruleCode, bicepText, onCompileErrors, diags =>
            {
                diags.Should().HaveCount(expectedDiagnosticCountForCode);
            },
            configuration,
            apiVersionProvider);
        }

        protected void AssertLinterRuleDiagnostics(string ruleCode, string bicepText, Action<IEnumerable<IDiagnostic>> assertAction, RootConfiguration? configuration = null, IApiVersionProvider? apiVersionProvider = null)
        {
            AssertLinterRuleDiagnostics(ruleCode, bicepText, OnCompileErrors.IncludeErrors, assertAction, configuration, apiVersionProvider);
        }

        protected void AssertLinterRuleDiagnostics(string ruleCode, string bicepText, OnCompileErrors onCompileErrors, Action<IEnumerable<IDiagnostic>> assertAction, RootConfiguration? configuration = null, IApiVersionProvider? apiVersionProvider = null)
        {
            RunWithDiagnosticAnnotations(
                bicepText,
                diag => diag.Code == ruleCode
                    || (onCompileErrors == OnCompileErrors.IncludeErrors && diag.Level == DiagnosticLevel.Error)
                    || (onCompileErrors == OnCompileErrors.IncludeErrorsAndWarnings && (diag.Level == DiagnosticLevel.Error || diag.Level == DiagnosticLevel.Warning)),
                onCompileErrors,
                assertAction,
                configuration,
                apiVersionProvider);
        }

        private static void RunWithDiagnosticAnnotations(string bicepText, Func<IDiagnostic, bool> filterFunc, OnCompileErrors onCompileErrors, Action<IEnumerable<IDiagnostic>> assertAction, RootConfiguration? configuration, IApiVersionProvider? apiVersionProvider)
        {
            var context = new CompilationHelper.CompilationHelperContext(Configuration: configuration, ApiVersionProvider: apiVersionProvider);
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
    }
}
