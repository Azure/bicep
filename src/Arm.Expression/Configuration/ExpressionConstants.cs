// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

namespace Azure.ResourceManager.Deployments.Expression.Configuration
{
    /// <summary>
    /// The constants.
    /// </summary>
    public static class ExpressionConstants
    {
        /// <summary>
        /// The logical and with short circuit function name. This value should be in upper case.
        /// </summary>
        public const string AndFunction = "AND";

        /// <summary>
        /// The logical or with short circuit function name. This value should be in upper case.
        /// </summary>
        public const string OrFunction = "OR";

        /// <summary>
        /// The logical if. This value should be in upper case.
        /// </summary>
        public const string IfFunction = "IF";

        /// <summary>
        /// The get parameter function name. This value should be in upper case.
        /// </summary>
        public const string ParametersFunction = "PARAMETERS";

        /// <summary>
        /// The get field function name. This value should be in upper case.
        /// </summary>
        public const string FieldFunction = "FIELD";

        /// <summary>
        /// The resource group function name. This value should be in upper case.
        /// </summary>
        public const string ResourceGroupFunction = "RESOURCEGROUP";

        /// <summary>
        /// The subscription function name. This value should be in upper case.
        /// </summary>
        public const string SubscriptionFunction = "SUBSCRIPTION";

        /// <summary>
        /// The request context function name. This value should be in upper case.
        /// </summary>
        public const string RequestContextFunction = "REQUESTCONTEXT";

        /// <summary>
        /// The language expression limit.
        /// </summary>
        public static readonly int ExpressionLimit = 81920;

        /// <summary>
        /// The language literal expression limit.
        /// </summary>
        public static readonly int LiteralLimit = 131072;
    }
}