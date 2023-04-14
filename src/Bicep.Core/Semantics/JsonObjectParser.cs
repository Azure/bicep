using SharpYaml.Serialization;

namespace Bicep.Core.Semantics
{
    public static class JsonObjectParser : ObjectParser
    {
        public static JToken ExtractTokenFromObject(string fileContent)
        {
            if (JsonObjectParser.ExtractTokenFromObject(fileContent) is not { } token)
            {
                // Instead of catching and returning the JSON parse exception, we simply return a generic error.
                // This avoids having to deal with localization, and avoids possible confusion regarding line endings in the message.
                return new(ErrorType.Create(DiagnosticBuilder.ForPosition(arguments[0]).UnparseableJsonType()));
            }
            return token;
        }
    }
}
