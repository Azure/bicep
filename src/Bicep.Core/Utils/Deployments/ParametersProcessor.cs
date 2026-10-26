// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Utils.Deployments;

/// <summary>
/// Helper class to assist with deployments parameter file processing.
/// </summary>
public static class ParametersProcessor
{
    /// <summary>
    /// Resolves an external input.
    /// </summary>
    /// <param name="kind">The kind of the external input.</param>
    /// <param name="config">The config of the external input.</param>
    /// <returns>The resolved external input value.</returns>
    public delegate Task<string> ResolveExternalInput(string kind, JToken? config);

    /// <summary>
    /// Processes a parameters file to resolve external inputs.
    /// </summary>
    /// <param name="parametersFile">The parameters file.</param>
    /// <param name="onResolveExternalInput">The callback to resolve an external input.</param>
    /// <returns>The updated parameters file.</returns>
    public static async Task<string> Process(string parametersFile, ResolveExternalInput onResolveExternalInput)
    {
        // Intentionally avoid using a typed class here, because we don't want to interfere
        // with properties that aren't part of the official schema.
        var parameters = TryParse(parametersFile) ?? throw new InvalidOperationException("Failed to read the parameters file");

        if (TryGetPropertyInsensitive(parameters, "externalInputDefinitions") is not { } externalInputDefinitions)
        {
            return parametersFile;
        }

        if (externalInputDefinitions is not JObject inputDefinitions)
        {
            throw new InvalidOperationException("externalInputDefinitions must be an object");
        }

        if (TryGetPropertyInsensitive(parameters, "externalInputs") is { })
        {
            throw new InvalidOperationException("externalInputs must not already be defined");
        }

        var inputs = new JObject();
        foreach (var kvp in inputDefinitions)
        {
            if (kvp.Value is not JObject inputDefinition)
            {
                throw new InvalidOperationException($"externalInputDefinitions[{kvp.Key}] must be an object");
            }

            if (TryGetPropertyInsensitive(inputDefinition, "kind") is not { } kindToken ||
                kindToken.Type != JTokenType.String ||
                kindToken.Value<string>() is not { } kind)
            {
                throw new InvalidOperationException($"externalInputDefinitions[{kvp.Key}].kind must be defined");
            }

            var config = TryGetPropertyInsensitive(inputDefinition, "config");
            var value = await onResolveExternalInput(kind, config).ConfigureAwait(false);

            inputs[kvp.Key] = new JObject
            {
                ["value"] = value,
            };
        }

        parameters["externalInputs"] = inputs;

        return JsonConvert.SerializeObject(parameters, Formatting.Indented);
    }

    private static JToken? TryGetPropertyInsensitive(JObject jobject, string propertyName)
        => jobject
            .Properties()
            .FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase))?.Value;

    private static JObject? TryParse(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<JObject>(json);
        }
        catch
        {
            return null;
        }
    }
}
