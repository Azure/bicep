// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json.Linq;

namespace Arm.Expression.Expressions
{
    public class JTokenExpression : LanguageExpression
    {
        public JToken Value { get; }

        public JTokenExpression(string value)
        {
            this.Value = value;
        }

        public JTokenExpression(int value)
        {
            this.Value = value;
        }
    }
}
