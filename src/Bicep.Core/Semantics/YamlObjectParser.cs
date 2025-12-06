// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Text;
using Newtonsoft.Json.Linq;
using SharpYaml.Serialization;

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
                // Reset the flag
                isMultiDocumentYaml = false;

                // Check for multi-document YAML first
                if (IsMultiDocumentYaml(fileContent))
                {
                    isMultiDocumentYaml = true;
                    return null; // Return null to trigger error handling
                }

                return new Serializer().Deserialize(fileContent) is { } deserialized ? JToken.FromObject(deserialized) : null;
            }
            catch
            {
                return null;
            }
        }

        override protected Diagnostic GetExtractTokenErrorType(IPositionable positionable)
        {
            return isMultiDocumentYaml 
                ? DiagnosticBuilder.ForPosition(positionable).MultiDocumentYamlNotSupported()
                : DiagnosticBuilder.ForPosition(positionable).UnparsableYamlType();
        }

        // Helper method to detect multi-document YAML
        private bool IsMultiDocumentYaml(string content)
        {
            using var reader = new StringReader(content);
            string? line;
            bool foundFirstDocument = false;
            
            while ((line = reader.ReadLine()) != null)
            {
                // Skip empty lines and comments
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith('#'))
                {
                    continue;
                }
                    
                // Check for document separator
                if (line.Trim() == "---")
                {
                    if (foundFirstDocument)
                    {
                        // Found a second document separator, meaning multi-document YAML
                        return true;
                    }
                    foundFirstDocument = true;
                }
                else if (!foundFirstDocument && !string.IsNullOrWhiteSpace(line))
                {
                    // Found content, so we're in the first document
                    foundFirstDocument = true;
                }
            }
            
            return false;
        }
    }
}