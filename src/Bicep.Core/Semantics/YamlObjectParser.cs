// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Text;
using Newtonsoft.Json.Linq;
using SharpYaml.Serialization;

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
            return new Serializer().Deserialize(fileContent);
        }
        catch
        {
            return null;
        }
    }
}
