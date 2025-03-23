// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;

namespace Bicep.Core.SourceLink
{
    public class SourceNotAvailableException : BicepException
    {
        public SourceNotAvailableException()
            : base("No source code is available for this module")
        { }
    }
}
