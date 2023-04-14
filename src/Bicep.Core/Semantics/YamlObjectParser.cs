using Newtonsoft.Json.Linq;
using SharpYaml.Serialization;

namespace Bicep.Core.Semantics
{
    public class YamlObjectParser : ObjectParser
    {
        override public JToken ExtractTokenFromObject(string fileContent) => JToken.FromObject(new Serializer().Deserialize(fileContent)!);
    }
}
