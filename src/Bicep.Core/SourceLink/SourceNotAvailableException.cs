// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.SourceLink
{
    public class SourceNotAvailableException : Exception
    {
        public SourceNotAvailableException()
            : base("No source code is available for this module")
        { }
    }
}
