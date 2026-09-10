// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Text.Json;

namespace Bicep.Wasm;

[SupportedOSPlatform("browser")]
public static partial class WorkerInterop
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly Lazy<Interop> Interop = new(() =>
        new Interop(WorkerImports.LoadQuickstart, WasmServiceProvider.Create()));

    [JSExport]
    public static string Ping(string value) => value;

    [JSExport]
    public static async Task<string> CompileAndEmitDiagnostics(string content, string? sourcePath) =>
        JsonSerializer.Serialize(
            await Interop.Value.CompileAndEmitDiagnostics(content, sourcePath),
            JsonOptions);

    [JSExport]
    public static async Task<string> Decompile(string content) =>
        JsonSerializer.Serialize(await Interop.Value.Decompile(content), JsonOptions);

    [JSExport]
    public static string GetSemanticTokensLegend() =>
        JsonSerializer.Serialize(Interop.Value.GetSemanticTokensLegend(), JsonOptions);

    [JSExport]
    public static async Task<string> GetSemanticTokens(string content, string? sourcePath) =>
        JsonSerializer.Serialize(
            await Interop.Value.GetSemanticTokens(content, sourcePath),
            JsonOptions);
}

[SupportedOSPlatform("browser")]
internal static partial class WorkerImports
{
    [JSImport("loadQuickstart", "bicepWorker")]
    public static partial Task<string?> LoadQuickstart(string filePath);
}
