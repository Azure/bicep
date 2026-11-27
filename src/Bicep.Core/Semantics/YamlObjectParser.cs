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
        var (deserialized, isMultiDocument) = TryDeserialize(fileContent);
        if (isMultiDocument)
        {
            return new(DiagnosticBuilder.ForPosition(positionable).MultiDocumentYamlNotSupported());
        }

        if (deserialized is { })
        {
            return new(JToken.FromObject(deserialized));
        }

        return new(DiagnosticBuilder.ForPosition(positionable).UnparsableYamlType());
    }

    private static (object? Value, bool IsMultiDocument) TryDeserialize(string fileContent)
    {
        try
        {
            var yamlStream = YamlStream.Load(new StringReader(fileContent), null);
            if (yamlStream.Count > 1)
            {
                return (null, true);
            }

            if (yamlStream.Count == 0 || yamlStream[0].Contents is not { } contents)
            {
                return (null, false);
            }

            return (contents.ToObject<object>(new YamlSerializerOptions
            {
                ReferenceHandling = YamlReferenceHandling.Preserve,
            }), false);
        }
        catch
        {
            return (null, false);
        }
    }
}
