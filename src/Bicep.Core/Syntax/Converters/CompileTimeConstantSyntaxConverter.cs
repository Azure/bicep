// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Deployments.Expression.Engines;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Syntax.Converters
{
    public static class CompileTimeConstantSyntaxConverter
    {
        public static SyntaxBase ConvertJToken(JToken value) => value switch
        {
            JObject jObject => ConvertJObject(jObject),
            JArray jArray => ConvertJArray(jArray),
            JValue jValue => ConvertJValue(jValue),
            _ => throw new InvalidOperationException($"Unrecognized token type: \"{value.Type}\"."),
        };

        public static ObjectSyntax ConvertJObject(JToken value)
        {
            if (value is not JObject objectValue)
            {
                throw new SyntaxConversionException($"Expected the value to be an object.", value);
            }

            var properties = new List<ObjectPropertySyntax>();

            foreach (var property in objectValue.Properties())
            {
                var propertyName = property.Name;
                var propertyValue = property.Value;

                if (ExpressionsEngine.IsLanguageExpression(propertyName))
                {
                    throw new SyntaxConversionException($"The property name \"{propertyName}\" must be a compile-time constant.", property);
                }

                properties.Add(SyntaxFactory.CreateObjectProperty(propertyName, ConvertJToken(propertyValue)));
            }

            return SyntaxFactory.CreateObject(properties);
        }

        public static ArraySyntax ConvertJArray(JToken value)
        {
            if (value is not JArray arrayValue)
            {
                throw new SyntaxConversionException($"Expected the value {value} to be an array.", value);
            }

            return SyntaxFactory.CreateArray(value.Select(item => ConvertJToken(item)));
        }

        public static SyntaxBase ConvertJValue(JValue value) => value.Type switch
        {
            JTokenType.String or
            JTokenType.Uri or
            JTokenType.Date => ConvertToStringLiteral(value),
            JTokenType.Float => ConvertToStringLiteral(new JValue(value.ToString())),
            JTokenType.Integer => SyntaxFactory.CreateIntegerLiteral(value.Value<long>()),
            JTokenType.Boolean => SyntaxFactory.CreateBooleanLiteral(value.Value<bool>()),
            JTokenType.Null => SyntaxFactory.CreateNullLiteral(),
            _ => throw new NotImplementedException($"Unrecognized token type {value.Type}"),
        };

        public static StringSyntax ConvertToStringLiteral(JToken value)
        {
            if (value is not JValue { Type: JTokenType.String or JTokenType.Uri or JTokenType.Date })
            {
                throw new SyntaxConversionException($"Expected the value {value} to be a string.", value);
            }

            var stringValue = value.ToString();

            if (ExpressionsEngine.IsLanguageExpression(stringValue))
            {
                throw new SyntaxConversionException($"The value {value} must be a compile-time constant.", value);
            }

            return SyntaxFactory.CreateStringLiteral(stringValue);
        }
    }
}
