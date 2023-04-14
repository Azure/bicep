using SharpYaml.Serialization;

namespace Bicep.Core.Semantics
{
    public static class JsonObjectParser : ObjectParser
    {
        public static JToken ExtractTokenFromObject(string fileContent) => JsonObjectParser.ExtractTokenFromObject(fileContent);
    }
}
