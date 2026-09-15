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
    protected override ResultWithDiagnostic<JToken> ExtractTokenFromObject(string fileContent, IPositionable positionable)
    {
        if (TryDeserialize(fileContent) is { } deserialized)
        {
            return new(JToken.FromObject(deserialized));
        }

        return new(DiagnosticBuilder.ForPosition(positionable).UnparsableYamlType());
    }

    /// <summary>
    /// The underlying YAML scanner emits a NUL character instead of a line break when a block scalar is terminated by
    /// the end of the input rather than by a line break. Appending a line break avoids the corruption and is a no-op
    /// as far as YAML semantics are concerned.
    /// </summary>
    private static string EnsureTrailingLineBreak(string yamlContent)
        => yamlContent.Length > 0 && yamlContent[^1] is not '\n' and not '\r'
            ? yamlContent + '\n'
            : yamlContent;

    private static object? TryDeserialize(string fileContent)
    {
        try
        {
            var yamlStream = YamlStream.Load(new StringReader(EnsureTrailingLineBreak(fileContent)), null);
            if (yamlStream.Count == 0 || yamlStream[0].Contents is not { } contents)
            {
                return null;
            }

            // YamlNode.ToObject() round-trips the node through its YAML representation, which is not line break
            // terminated, so the content needs normalizing again before it gets deserialized.
            return YamlSerializer.Deserialize(EnsureTrailingLineBreak(contents.ToString()), typeof(object), new YamlSerializerOptions
            {
                ReferenceHandling = YamlReferenceHandling.Preserve,
            });
        }
        catch
        {
            return null;
        }
    }
}
