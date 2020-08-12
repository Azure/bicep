//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Azure.ResourceManager.Deployments.Expression.Exceptions
{
    using System;
    using Azure.ResourceManager.Deployments.Core.ErrorResponses;

    /// <summary>
    /// Exception thrown when a built-in function has the wrong number of parameters.
    /// </summary>
    public class BuiltinFunctionNumberOfParametersException : ExpressionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuiltinFunctionNumberOfParametersException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="additionalInfo">The additional information for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public BuiltinFunctionNumberOfParametersException(string message, TemplateErrorAdditionalInfo additionalInfo, Exception innerException = null)
            : base(message, additionalInfo, innerException)
        {
        }
    }
}
