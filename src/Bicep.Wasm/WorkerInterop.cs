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
    public static async Task<string> CompileAndEmitDiagnostics(string content, string? sourcePath)
    {
        var result = await Interop.Value.CompileAndEmitDiagnostics(content, sourcePath);
        return JsonSerialize(result);
    }

    [JSExport]
    public static async Task<string> Decompile(string content)
    {
        var result = await Interop.Value.Decompile(content);
        return JsonSerialize(result);
    }

    [JSExport]
    public static string GetSemanticTokensLegend() =>
        JsonSerialize(Interop.Value.GetSemanticTokensLegend());

    [JSExport]
    public static async Task<string> GetSemanticTokens(string content, string? sourcePath)
    {
        var result = await Interop.Value.GetSemanticTokens(content, sourcePath);
        return JsonSerialize(result);
    }

    private static string JsonSerialize<T>(T value) => JsonSerializer.Serialize(value, JsonOptions);
}

[SupportedOSPlatform("browser")]
internal static partial class WorkerImports
{
    [JSImport("loadQuickstart", "bicepWorker")]
    public static partial Task<string?> LoadQuickstart(string filePath);
}
