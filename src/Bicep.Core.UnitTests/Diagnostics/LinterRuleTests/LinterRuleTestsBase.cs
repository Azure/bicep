// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.CodeAction;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

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
        Func<RootConfiguration, RootConfiguration>? ConfigurationPatch = null,
        IResourceTypeLoader? AzResourceTypeLoader = null,
        (string path, string contents)[]? AdditionalFiles = null,
        FeatureProviderOverrides? FeatureOverrides = null
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
            [.. files],
            diag =>
                diag.Code == ruleCode
                || (IsCompilerDiagnostic(diag) && options.OnCompileErrors == OnCompileErrors.IncludeErrors && diag.IsError())
                || (IsCompilerDiagnostic(diag) && options.OnCompileErrors == OnCompileErrors.IncludeErrorsAndWarnings && (diag.IsError() || diag.Level == DiagnosticLevel.Warning)),
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
        var services = new ServiceBuilder().WithConfiguration(BicepTestConstants.BuiltInConfigurationWithStableAnalyzers);
        services = options.ConfigurationPatch is not null ? services.WithConfigurationPatch(options.ConfigurationPatch) : services;
        services = options.AzResourceTypeLoader is { } ? services.WithAzResourceTypeLoader(options.AzResourceTypeLoader) : services;
        services = options.FeatureOverrides is not null ? services.WithFeatureOverrides(options.FeatureOverrides) : services;
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

    protected static void AssertCodeFix(string expectedCode, string expectedFixTitle, string inputFile, string resultFile, CompilationHelper.InputFile[]? supportingFiles = null)
    {
        supportingFiles ??= [];
        var (file, cursor) = ParserHelper.GetFileWithSingleCursor(inputFile, '|');
        var result = CompilationHelper.Compile([.. supportingFiles, new("main.bicep", file)]);

        using (new AssertionScope().WithVisualCursor(result.Compilation.GetEntrypointSemanticModel().SourceFile, cursor))
        {
            var matchingDiagnostics = result.Diagnostics
                .Where(x => x.Source == DiagnosticSource.CoreLinter)
                .Where(x => x.Span.IsOverlapping(cursor));

            matchingDiagnostics.Should().ContainSingle(x => x.Code == expectedCode);
            var diagnostic = matchingDiagnostics.Single(x => x.Code == expectedCode);

            diagnostic.Fixes.Should().ContainSingle(x => x.Title == expectedFixTitle);
            var fix = diagnostic.Fixes.Single(x => x.Title == expectedFixTitle);
            fix.Kind.Should().Be(CodeFixKind.QuickFix);

            fix.Should().HaveResult(file, resultFile);
        }
    }
}
