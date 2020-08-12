// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

namespace Azure.ResourceManager.Deployments.Expression.Configuration
{
    /// <summary>
    /// Configures how expression serializer handles single string literal expressions.
    /// </summary>
    public enum ExpressionSerializerSingleStringHandling
    {
        /// <summary>
        /// Not specified.
        /// </summary>
        NotSpecified = 0,

        /// <summary>
        /// Serializes the single string literal as a string literal expression of the form
        /// ['string contents']. This is the default behavior. The behavior does not apply for
        /// Language expressions that are not a single JTokenExpression with a string value.
        /// </summary>
        SerializeAsJTokenExpression = 1,

        /// <summary>
        /// Serializes the single string literal as a string value. If the string begins with a
        /// [ character, it will be escaped with [[. The behavior does not apply for
        /// Language expressions that are not a single JTokenExpression with a string value.
        /// </summary>
        SerializeAsString = 2,
    }
}
