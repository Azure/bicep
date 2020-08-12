// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

namespace Azure.ResourceManager.Deployments.Expression.Configuration
{
    /// <summary>
    /// Expression serializer settings.
    /// </summary>
    public class ExpressionSerializerSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether the expression serializer will include the outer [ and ]
        /// characters when serializing a LanguageExpression. This setting is ignored if the serializer decides to serialize
        /// the expression into a single string literal.
        /// </summary>
        public bool IncludeOuterSquareBrackets { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the expression serializer will serialize a single string literal
        /// expression as a string
        /// </summary>
        public ExpressionSerializerSingleStringHandling SingleStringHandling { get; set; } = ExpressionSerializerSingleStringHandling.SerializeAsJTokenExpression;
    }
}
