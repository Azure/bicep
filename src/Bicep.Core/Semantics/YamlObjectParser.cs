// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.TypeSystem;
using Newtonsoft.Json.Linq;
using SharpYaml.Serialization;

namespace Bicep.Core.Semantics
{
    public class YamlObjectParser : ObjectParser
    {
        /// <summary>
        /// Deserialize raises an exception if the fileContent is not a valid YAML object
        /// </summary>
        override protected JToken? ExtractTokenFromObject(string fileContent)
        {
            try
            {
                return new Serializer().Deserialize(fileContent) is { } deserialized ? JToken.FromObject(deserialized) : null;
            }
            catch
            {
                return null;
            }
        }

        override protected ErrorDiagnostic GetExtractTokenErrorType(IPositionable positionable)
            => DiagnosticBuilder.ForPosition(positionable).UnparseableYamlType();

    }
}
