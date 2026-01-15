// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Text;
using Newtonsoft.Json.Linq;
using SharpYaml.Serialization;
using System.IO;
using System.Globalization;
using YamlNode = SharpYaml.Serialization.YamlNode;
using YamlScalarNode = SharpYaml.Serialization.YamlScalarNode;
using YamlSequenceNode = SharpYaml.Serialization.YamlSequenceNode;
using YamlMappingNode = SharpYaml.Serialization.YamlMappingNode;
using ScalarStyle = SharpYaml.ScalarStyle;

namespace Bicep.Core.Semantics;

public class YamlObjectParser : ObjectParser
{
    protected override ResultWithDiagnostic<JToken> ExtractTokenFromObject(string fileContent, IPositionable positionable)
    {
        try
        {
            // Check for multi-document YAML using YamlStream
            var yamlStream = new YamlStream();
            using (var reader = new StringReader(fileContent))
            {
                yamlStream.Load(reader);
            }

            if (yamlStream.Documents.Count > 1)
            {
                return new(DiagnosticBuilder.ForPosition(positionable).MultiDocumentYamlNotSupported());
            }

            if (yamlStream.Documents.Count == 0)
            {
                // If empty, return error
                return new(DiagnosticBuilder.ForPosition(positionable).UnparsableYamlType());
            }

            if (ConvertYamlNodeToJToken(yamlStream.Documents[0].RootNode) is not { } jToken)
            {
                return new(DiagnosticBuilder.ForPosition(positionable).UnparsableYamlType());
            }

            return new(jToken);
        }
        catch
        {
            return new(DiagnosticBuilder.ForPosition(positionable).UnparsableYamlType());
        }
    }

    /// <summary>
    /// Converts a SharpYaml YamlNode directly to a Newtonsoft JToken
    /// </summary>
    private static JToken? ConvertYamlNodeToJToken(YamlNode? node)
    {
        if (node == null)
        {
            return null;
        }

        switch (node)
        {
            case YamlScalarNode scalar:
                return ConvertScalarToJValue(scalar);

            case YamlSequenceNode sequence:
                var array = new JArray();
                foreach (var item in sequence.Children)
                {
                    var converted = ConvertYamlNodeToJToken(item);
                    array.Add(converted ?? JValue.CreateNull());
                }
                return array;

            case YamlMappingNode mapping:
                var obj = new JObject();
                foreach (var entry in mapping.Children)
                {
                    // Convert key to string (JSON only supports string keys)
                    string key;
                    if (entry.Key is YamlScalarNode keyScalar)
                    {
                        key = keyScalar.Value ?? string.Empty;
                    }
                    else
                    {
                        // For complex keys (arrays, objects), serialize to string representation
                        var keyToken = ConvertYamlNodeToJToken(entry.Key);
                        key = keyToken?.ToString() ?? string.Empty;
                    }

                    var value = ConvertYamlNodeToJToken(entry.Value);
                    obj[key] = value ?? JValue.CreateNull();
                }
                return obj;

            default:
                return null;
        }
    }

    /// <summary>
    /// Converts a YAML scalar value to a JValue with type inference.
    /// Note: This implements basic JSON-compatible type inference only.
    /// Advanced YAML features are not supported:
    /// - Octal/hex numbers (e.g., 0o123, 0x1A)
    /// - Extended boolean values (yes/no/on/off)
    /// - Tagged scalars (!!int, !!str)
    /// - Anchors and aliases
    /// Values are parsed as: null, boolean (true/false), number, or string (default).
    /// Quoted scalars in YAML are always treated as strings.
    /// </summary>
    private static JValue ConvertScalarToJValue(YamlScalarNode scalar)
    {
        var value = scalar.Value;

        if (string.IsNullOrEmpty(value) || value == "null" || value == "~")
        {
            return JValue.CreateNull();
        }

        // If the scalar is quoted (single or double quotes), treat it as a string
        // In SharpYaml, ScalarStyle can be: Plain, SingleQuoted, DoubleQuoted, Literal, Folded
        if (scalar.Style == ScalarStyle.SingleQuoted || scalar.Style == ScalarStyle.DoubleQuoted)
        {
            return new JValue(value);
        }

        // Try to parse as boolean (strict JSON-style true/false only)
        if (value == "true" || value == "True" || value == "TRUE")
        {
            return new JValue(true);
        }
        if (value == "false" || value == "False" || value == "FALSE")
        {
            return new JValue(false);
        }

        // Try to parse as integer (using invariant culture for consistent behavior)
        if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longValue))
        {
            return new JValue(longValue);
        }

        // Try to parse as float (using invariant culture for consistent behavior)
        if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var doubleValue))
        {
            return new JValue(doubleValue);
        }

        // Default to string
        return new JValue(value);
    }
}