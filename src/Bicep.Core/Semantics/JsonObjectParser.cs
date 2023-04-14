using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;
using SharpYaml.Serialization;

namespace Bicep.Core.Semantics
{
    public class JsonObjectParser : ObjectParser
    {
        override public JToken ExtractTokenFromObject(string fileContent) => fileContent.TryFromJson<JToken>();
    }
}
