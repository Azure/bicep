// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Text;
using Newtonsoft.Json.Linq;
using SharpYaml;
using SharpYaml.Model;

namespace Bicep.Core.Semantics;

public class YamlObjectParser : ObjectParser
{
    /// <summary>
    /// SharpYaml's scanner reads the line break terminating each line of a block scalar without guarding against
    /// the end of the input. When a block scalar runs to the end of the input, the scanner reads the look-ahead
    /// buffer's end-of-input sentinel and appends that in place of the line break, so scalars using clip (the
    /// default) or keep chomping end up carrying a NUL character where their trailing line break should be.
    /// SharpYaml exposes no setting to opt out of this, so the affected scalars have to be repaired here.
    /// </summary>
    private const char EndOfInputMarker = '\0';

    protected override ResultWithDiagnostic<JToken> ExtractTokenFromObject(string fileContent, IPositionable positionable)
    {
        if (TryDeserialize(fileContent) is { } deserialized)
        {
            return new(RestoreTrailingLineBreaks(JToken.FromObject(deserialized)));
        }

        return new(DiagnosticBuilder.ForPosition(positionable).UnparsableYamlType());
    }

    private static object? TryDeserialize(string fileContent)
    {
        try
        {
            var yamlStream = YamlStream.Load(new StringReader(fileContent), null);
            if (yamlStream.Count == 0 || yamlStream[0].Contents is not { } contents)
            {
                return null;
            }

            if (!EndsWithLineBreak(fileContent))
            {
                // The final block scalar was terminated by the end of the input, so the marker the scanner left
                // behind stands in for a line break that the file does not actually contain. Drop it - doing so
                // also lets the scalar round-trip below as strip chomped, which is what it now is.
                RemoveEndOfInputMarkers(contents);
            }

            return contents.ToObject<object>(new YamlSerializerOptions
            {
                ReferenceHandling = YamlReferenceHandling.Preserve,
            });
        }
        catch
        {
            return null;
        }
    }

    private static bool EndsWithLineBreak(string fileContent)
        => fileContent.Length > 0 && fileContent[^1] is '\n' or '\r' or '\u0085' or '\u2028' or '\u2029';

    /// <summary>
    /// Strips the end-of-input marker left on a block scalar that the source file did not terminate with a line
    /// break. Only values are visited: rewriting a mapping key would invalidate the mapping, and a block scalar
    /// can only pick up the marker by being the last node of the document.
    /// </summary>
    private static void RemoveEndOfInputMarkers(YamlElement? element)
    {
        switch (element)
        {
            case YamlValue value when value.Value is { } scalar && scalar.EndsWith(EndOfInputMarker):
                value.Value = scalar[..^1];
                break;
            case YamlSequence sequence:
                foreach (var item in sequence)
                {
                    RemoveEndOfInputMarkers(item);
                }
                break;
            case YamlMapping mapping:
                foreach (var entry in mapping)
                {
                    RemoveEndOfInputMarkers(entry.Value);
                }
                break;
        }
    }

    /// <summary>
    /// Repairs the end-of-input markers introduced while deserializing. ToObject() routes the node through its
    /// YAML text representation, which is never line break terminated, so the document's final block scalar
    /// always trips the scanner on the way back in. Unlike the markers handled by RemoveEndOfInputMarkers, these
    /// stand in for a line break that the scalar genuinely has.
    /// </summary>
    private static JToken RestoreTrailingLineBreaks(JToken token)
    {
        switch (token)
        {
            case JObject @object:
                foreach (var property in @object.Properties())
                {
                    property.Value = RestoreTrailingLineBreaks(property.Value);
                }

                return @object;
            case JArray array:
                for (var i = 0; i < array.Count; i++)
                {
                    array[i] = RestoreTrailingLineBreaks(array[i]);
                }

                return array;
            case JValue { Value: string scalar } when scalar.EndsWith(EndOfInputMarker):
                return new JValue(scalar[..^1] + '\n');
            default:
                return token;
        }
    }
}
