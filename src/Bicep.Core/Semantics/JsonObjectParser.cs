using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.TypeSystem;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Semantics
{
    public class JsonObjectParser : ObjectParser
    {
        override public JToken ExtractTokenFromObject(string fileContent) => fileContent.TryFromJson<JToken>();
        override public ErrorType GetParsingError(IPositionable positionable)
        {
            return ErrorType.Create(DiagnosticBuilder.ForPosition(positionable).UnparseableJsonType());
        }
    }
}
