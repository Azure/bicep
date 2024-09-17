// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Deploy.Exceptions
{
    public class DeploymentException : Exception
    {
        public DeploymentException(string message)
            : base(message)
        {
        }

        public DeploymentException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
