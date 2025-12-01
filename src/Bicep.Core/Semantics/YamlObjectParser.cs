// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Text;
using Newtonsoft.Json.Linq;
using SharpYaml.Serialization;
using System.IO;

namespace Bicep.Core.Semantics
{
    public class YamlObjectParser : ObjectParser
    {
        private bool isMultiDocumentYaml = false;

        /// <summary>
        /// Deserialize raises an exception if the fileContent is not a valid YAML object
        /// </summary>
        override protected JToken? ExtractTokenFromObject(string fileContent)
        {
            try
            {
                // Check for multi-document YAML using YamlStream
                var yamlStream = new YamlStream();
                using (var reader = new StringReader(fileContent))
                {
                    yamlStream.Load(reader);
                }

                if (yamlStream.Documents.Count > 1)
                {
                    isMultiDocumentYaml = true;
                    return null;
                }

                // If single document or empty, proceed with normal deserialization
                return new Serializer().Deserialize(fileContent) is { } deserialized ? JToken.FromObject(deserialized) : null;
            }
            catch
            {
                return null;
            }
        }

        override protected Diagnostic GetExtractTokenErrorType(IPositionable positionable)
            => isMultiDocumentYaml
                ? DiagnosticBuilder.ForPosition(positionable).MultiDocumentYamlNotSupported()
                : DiagnosticBuilder.ForPosition(positionable).UnparsableYamlType();
    }
}