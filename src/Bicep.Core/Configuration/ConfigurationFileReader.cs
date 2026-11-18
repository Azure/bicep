// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.Diagnostics;
using Bicep.Core.Json;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Configuration;

/// <summary>
/// Reads and parses one Bicep configuration file. Comments are allowed. Never throws: every failure is a diagnostic.
/// </summary>
public static class ConfigurationFileReader
{
    private static readonly DiagnosticBuilder.DiagnosticBuilderInternal DiagnosticBuilder = Diagnostics.DiagnosticBuilder.ForDocumentStart();

    public static ResultWithDiagnostic<ConfigurationFile> Read(IFileHandle fileHandle)
    {
        JsonElement root;
        try
        {
            using var stream = fileHandle.OpenRead();
            root = JsonElementFactory.CreateElementFromStream(stream);
        }
        catch (JsonException exception)
        {
            return new(DiagnosticBuilder.UnparsableBicepConfigFile(fileHandle.Uri, exception.Message));
        }
        catch (Exception exception)
        {
            return new(DiagnosticBuilder.UnloadableBicepConfigFile(fileHandle.Uri, exception.Message));
        }

        if (root.ValueKind != JsonValueKind.Object)
        {
            return new(DiagnosticBuilder.InvalidBicepConfigFile(fileHandle.Uri, "The configuration must be a JSON object."));
        }

        return new(new ConfigurationFile(fileHandle.Uri, root));
    }
}
