// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry
{
    public class OciClientResult
    {
        public OciClientResult(bool success, string? errorMessage)
        {
            this.Success = success;
            this.ErrorMessage = errorMessage;
        }

        public bool Success { get; }

        public string? ErrorMessage { get; }
    }
}
