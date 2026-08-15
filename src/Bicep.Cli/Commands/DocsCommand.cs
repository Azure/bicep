// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Documentation;
using Bicep.Core.Exceptions;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.IO.Abstraction;

namespace Bicep.Cli.Commands;

public static class DocsCommand
{
    internal const string InputFailureCode = "DOCS001";
    internal const string WriteFailureCode = "DOCS002";
    internal const string RenderFailureCode = "DOCS003";

    internal static System.CommandLine.Command CreateCommand(CommandLineBuilderContext context)
    {
        var command = new System.CommandLine.Command(
            Constants.Command.Docs,
            "[Experimental] Generates documentation for Bicep modules.");
        command.Add(DocsGenerateCommand.CreateCommand(context));
        command.Add(DocsOutputCommand.CreateCommand(context));

        return command;
    }

    internal static ImmutableSortedDictionary<string, string> ParseCustomValues(IEnumerable<string> values)
    {
        var customValues = ImmutableSortedDictionary.CreateBuilder<string, string>(StringComparer.Ordinal);

        foreach (var value in values)
        {
            var separatorIndex = value.IndexOf('=');
            if (separatorIndex <= 0)
            {
                throw new CommandLineException($"The --set value \"{value}\" must use the format key=value.");
            }

            var key = value[..separatorIndex];
            if (!customValues.TryAdd(key, value[(separatorIndex + 1)..]))
            {
                throw new CommandLineException($"The --set key \"{key}\" cannot be specified more than once.");
            }
        }

        return customValues.ToImmutable();
    }

    internal static void ValidateSetOption(
        System.CommandLine.ParseResult result,
        System.CommandLine.Option<string[]> setOption)
    {
        if (result.GetResult(setOption) is { Implicit: false, Tokens.Count: 0 })
        {
            throw new CommandLineException("The --set parameter expects a key=value argument.");
        }
    }

    internal static IDiagnostic CreateDiagnostic(string code, string message) =>
        new Diagnostic(new(0, 0), DiagnosticLevel.Error, DiagnosticSource.Compiler, code, message);

    internal static DocsDiagnostics MergeDiagnostics(
        IEnumerable<(IOUri SourceUri, Compilation? Compilation, IDiagnostic? DocumentationDiagnostic)> results)
    {
        var byUri = new Dictionary<IOUri, (BicepSourceFile File, ImmutableArray<IDiagnostic>.Builder Diagnostics)>();
        var additionalDiagnostics = ImmutableArray.CreateBuilder<(IOUri SourceUri, IDiagnostic Diagnostic)>();
        foreach (var (sourceUri, compilation, documentationDiagnostic) in results)
        {
            if (compilation is not null)
            {
                foreach (var (file, diagnostics) in compilation.GetAllDiagnosticsByBicepFile())
                {
                    if (!byUri.ContainsKey(file.FileHandle.Uri))
                    {
                        byUri[file.FileHandle.Uri] = (file, diagnostics.ToBuilder());
                    }
                }
            }

            if (documentationDiagnostic is not null)
            {
                if (compilation is null)
                {
                    additionalDiagnostics.Add((sourceUri, documentationDiagnostic));
                }
                else
                {
                    var entryFile = compilation.GetEntrypointSemanticModel().SourceFile;
                    byUri[entryFile.FileHandle.Uri].Diagnostics.Add(documentationDiagnostic);
                }
            }
        }

        return new(
            byUri.Values.ToImmutableDictionary(item => item.File, item => item.Diagnostics.ToImmutable()),
            additionalDiagnostics.ToImmutable());
    }

    internal record DocsDiagnostics(
        ImmutableDictionary<BicepSourceFile, ImmutableArray<IDiagnostic>> ByFile,
        ImmutableArray<(IOUri SourceUri, IDiagnostic Diagnostic)> Additional);
}
