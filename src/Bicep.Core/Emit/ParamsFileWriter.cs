// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Syntax;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Immutable;
using System.IO;

namespace Bicep.Core.Emit
{
    public class ParamsFileWriter
    {
        private readonly ProgramSyntax syntax;
        public ParamsFileWriter(ProgramSyntax syntax)
        {
            this.syntax = syntax;
        }

        public JToken GenerateTemplate()
        {
            using var stringWriter = new StringWriter();
            using var jsonWriter = new JsonTextWriter(stringWriter);
            //var emitter = new ExpressionEmitter(jsonWriter, this.context);

            jsonWriter.WriteStartObject();

            jsonWriter.WritePropertyName("$schema");
            jsonWriter.WriteValue("https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#");

            jsonWriter.WritePropertyName("contentVersion");
            jsonWriter.WriteValue("1.0.0.0");

            var parameters =  syntax.Children.OfType<ParameterAssignmentSyntax>().ToImmutableList();

            if (parameters.Count > 0)
            {
                jsonWriter.WritePropertyName("parameters");
                jsonWriter.WriteStartObject();

                foreach (var parameter in parameters)
                {
                    jsonWriter.WritePropertyName(parameter.Name.IdentifierName);

                    jsonWriter.WriteStartObject();

                    jsonWriter.WritePropertyName("value");
                    this.EmitExpression(parameter.Value, jsonWriter);
                    jsonWriter.WriteEndObject();
                }
                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();

            var content = stringWriter.ToString();

            return content.FromJson<JToken>();
        }

        public void EmitExpression(SyntaxBase syntax, JsonTextWriter jsonWriter)
        {
            switch (syntax)
            {      
                case BooleanLiteralSyntax booleanLiteralSyntax:
                {
                    jsonWriter.WriteValue(booleanLiteralSyntax.Value);
                    break;
                }
                case IntegerLiteralSyntax integerLiteralSyntax:
                {
                    jsonWriter.WriteValue(integerLiteralSyntax.Value);
                    break;
                }
                case StringSyntax stringSyntax:
                {
                    jsonWriter.WriteValue(string.Join("", stringSyntax.SegmentValues));
                    break;
                }
                case ObjectSyntax objectSyntax:
                {
                    jsonWriter.WriteStartObject();
                    EmitObjectProperties(objectSyntax, jsonWriter);
                    jsonWriter.WriteEndObject();
                    break;
                }
                case ArraySyntax arraySyntax:
                {
                    jsonWriter.WriteStartArray();
                    foreach (ArrayItemSyntax itemSyntax in arraySyntax.Items)
                    {
                        EmitExpression(itemSyntax.Value, jsonWriter);
                    }
                    jsonWriter.WriteEndArray();
                    break;
                }
                case NullLiteralSyntax _:
                {
                    jsonWriter.WriteNull();
                    break;
                }
            }
        }

        public void EmitObjectProperties(ObjectSyntax objectSyntax, JsonTextWriter jsonWriter/*, ISet<string>? propertiesToOmit = null*/)
        {
            // var propertyLookup = objectSyntax.Properties.ToLookup(property => property.Value is ForSyntax);

            // // emit loop properties first (if any)
            // if (propertyLookup.Contains(true))
            // {
            //     // we have properties whose value is a for-expression
            //     this.EmitCopyProperty(() =>
            //     {
            //         this.writer.WriteStartArray();

            //         foreach (var property in propertyLookup[true])
            //         {
            //             var key = property.TryGetKeyText();
            //             if (key is null || property.Value is not ForSyntax @for)
            //             {
            //                 // should be caught by loop emit limitation checks
            //                 throw new InvalidOperationException("Encountered a property with an expression-based key whose value is a for-expression.");
            //             }

            //             this.EmitCopyObject(key, @for, @for.Body);
            //         }

            //         this.writer.WriteEndArray();
            //     });
            // }

            foreach (ObjectPropertySyntax propertySyntax in objectSyntax.Properties)
            {
                switch(propertySyntax.Key)
                {
                    case IdentifierSyntax identifierSyntax:
                    {
                        jsonWriter.WritePropertyName(identifierSyntax.IdentifierName);
                        break;
                    }
                    case StringSyntax stringSyntax:
                    {
                        jsonWriter.WritePropertyName(string.Join("", stringSyntax.SegmentValues));
                        break;
                    }
                }
                EmitExpression(propertySyntax.Value, jsonWriter);
            }
        }
    }
}
