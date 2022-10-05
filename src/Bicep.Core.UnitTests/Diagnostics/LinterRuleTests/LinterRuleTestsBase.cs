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
using Microsoft.Extensions.Primitives;
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
            LineNumberAndColumn,

            Default = LineNumber,
        }

        public record Options(
            OnCompileErrors OnCompileErrors = OnCompileErrors.Default,
            IncludePosition IncludePosition = IncludePosition.Default,
            RootConfiguration? Configuration = null,
            ApiVersionProvider? ApiVersionProvider = null,
            (string path, string contents)[]? AdditionalFiles = null
        );

        private static string FormatDiagnostic(IDiagnostic diagnostic, ImmutableArray<int> lineStarts, IncludePosition includePosition)
        {
            var (line, character) = TextCoordinateConverter.GetPosition(lineStarts, diagnostic.Span.Position);
            return includePosition switch
            {
                IncludePosition.LineNumber => $"[{line + 1}] {diagnostic.Message}",
                IncludePosition.LineNumberAndColumn => $"[{line + 1}:{character + 1}] {diagnostic.Message}",
                IncludePosition.None => diagnostic.Message,
                _ => throw new InvalidOperationException($"Invalid IncludePosition value {includePosition}"),
            };
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
            var files = new List<(string path, string content)>
            {
                ("main.bicep", bicepText)
            };
            if (options.AdditionalFiles is not null)
            {
                files.AddRange(options.AdditionalFiles);
            }

            RunWithDiagnosticAnnotations(
                files.ToArray(),
                diag =>
                    diag.Code == ruleCode
                    || (IsCompilerDiagnostic(diag) && options.OnCompileErrors == OnCompileErrors.IncludeErrors && diag.Level == DiagnosticLevel.Error)
                    || (IsCompilerDiagnostic(diag) && options.OnCompileErrors == OnCompileErrors.IncludeErrorsAndWarnings && (diag.Level == DiagnosticLevel.Error || diag.Level == DiagnosticLevel.Warning)),
                assertAction,
                options);
        }

        private static void RunWithDiagnosticAnnotations(
            (string path, string contents)[] files,
            Func<IDiagnostic, bool> filterFunc,
            Action<IEnumerable<IDiagnostic>> assertAction,
            Options? options)
        {
            options ??= new Options();
            var services = new ServiceBuilder();
            services = options.Configuration is {} ? services.WithConfiguration(options.Configuration) : services;
            services = options.ApiVersionProvider is {} ? services.WithApiVersionProvider(options.ApiVersionProvider) : services;
            var result = CompilationHelper.Compile(services, files);
            using (new AssertionScope().WithFullSource(result.BicepFile))
            {
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
