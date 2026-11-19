// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json;
using Bicep.Core.Diagnostics;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Configuration;

/// <param name="Files">The files that were read successfully, leaf first.</param>
/// <param name="Error">The error that stopped the chain from loading, or null if it loaded.</param>
public sealed record ConfigurationChainResolution(ImmutableArray<ConfigurationFile> Files, ConfigurationDiagnostic? Error);

/// <summary>
/// Follows "extends" from a leaf configuration file. The value must be a relative path; the chain can't contain a cycle
/// or be deeper than <see cref="MaxChainDepth"/> files. Never throws for bad input: the first problem stops the chain
/// and is returned as <see cref="ConfigurationChainResolution.Error"/>.
/// </summary>
public static class ConfigurationChainResolver
{
    public const int MaxChainDepth = 64;

    private static readonly DiagnosticBuilder.DiagnosticBuilderInternal DiagnosticBuilder = Diagnostics.DiagnosticBuilder.ForDocumentStart();

    /// <param name="leafUri">The file to start from.</param>
    /// <param name="tryReadFile">Reads a file, or returns null if it doesn't exist.</param>
    public static ConfigurationChainResolution Resolve(IOUri leafUri, Func<IOUri, ResultWithDiagnostic<ConfigurationFile>?> tryReadFile)
    {
        var files = ImmutableArray.CreateBuilder<ConfigurationFile>();
        var chainUris = new List<IOUri>();
        var currentUri = leafUri;

        ConfigurationChainResolution Fail(IOUri file, IDiagnostic diagnostic) => new(files.ToImmutable(), new(file, diagnostic));

        while (true)
        {
            chainUris.Add(currentUri);

            if (tryReadFile(currentUri) is not { } readResult)
            {
                // A missing file has nowhere to show an error, so it's reported on the file that extends it.
                return Fail(files.Count > 0 ? files[^1].Uri : currentUri, DiagnosticBuilder.UnloadableBicepConfigFile(currentUri, "File not found."));
            }

            if (!readResult.IsSuccess(out var file, out var readError))
            {
                return Fail(currentUri, readError);
            }

            files.Add(file);

            if (file.Extends is not { } extends)
            {
                break;
            }

            if (extends.ValueKind != JsonValueKind.String)
            {
                return Fail(file.Uri, DiagnosticBuilder.BicepConfigExtendsMustBeRelativePath(file.Uri));
            }

            var extendsPath = extends.GetString();

            if (string.IsNullOrWhiteSpace(extendsPath))
            {
                break;
            }

            if (IOUri.IsAbsoluteFilePath(extendsPath))
            {
                return Fail(file.Uri, DiagnosticBuilder.BicepConfigExtendsMustBeRelativePath(file.Uri));
            }

            var nextUri = file.Uri.Resolve(extendsPath);

            // Cycle and depth errors describe the chain starting from the leaf, so they're reported on the leaf.
            if (chainUris.Contains(nextUri))
            {
                return Fail(leafUri, DiagnosticBuilder.BicepConfigExtendsCycle(leafUri, string.Join(" -> ", chainUris.Append(nextUri))));
            }

            if (files.Count >= MaxChainDepth)
            {
                return Fail(leafUri, DiagnosticBuilder.BicepConfigExtendsChainTooDeep(leafUri));
            }

            currentUri = nextUri;
        }

        return new(files.ToImmutable(), null);
    }
}
