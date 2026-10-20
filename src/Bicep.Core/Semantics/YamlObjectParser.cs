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

    private static object? TryDeserialize(string fileContent)
    {
        try
        {
            // SharpYaml reads its EOF sentinel as a block scalar's final line break if no line break is present.
            if (fileContent.Length > 0 && fileContent[^1] is not ('\r' or '\n' or '\u0085' or '\u2028' or '\u2029'))
            {
                fileContent += "\n";
            }

            var yamlStream = YamlStream.Load(new StringReader(fileContent), null);
            if (yamlStream.Count == 0 || yamlStream[0].Contents is not { } contents)
            {
                return null;
            }

            // ToObject() trims the serialized YAML before reparsing it, removing block scalar line breaks.
            using var writer = new StringWriter();
            contents.WriteTo(writer, suppressDocumentTags: true);
            return YamlSerializer.Deserialize<object>(writer.ToString(), new YamlSerializerOptions
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
