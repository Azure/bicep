using SharpYaml.Serialization;

namespace Bicep.Core.Semantics
{
    public static class YamlObjectParser : ObjectParser
    {
        public static JToken ExtractTokenFromObject(string fileContent)
        {
            return JToken.FromObject(new Serializer().Deserialize(fileContent)!);
        }
    }
}
