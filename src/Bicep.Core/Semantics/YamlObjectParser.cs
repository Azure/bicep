using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.TypeSystem;
using Newtonsoft.Json.Linq;
using SharpYaml.Serialization;

namespace Bicep.Core.Semantics
{
    public class YamlObjectParser : ObjectParser
    {
        override public JToken ExtractTokenFromObject(string fileContent) => JToken.FromObject(new Serializer().Deserialize(fileContent)!);

        override public ErrorType GetError(IPositionable positionable)
        {
            return ErrorType.Create(DiagnosticBuilder.ForPosition(positionable).UnparseableYamlType());
        }
    }
}
