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
            var yamlStream = YamlStream.Load(new StringReader(fileContent), null);
            if (yamlStream.Count == 0 || yamlStream[0].Contents is not { } contents)
            {
                return null;
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
}
