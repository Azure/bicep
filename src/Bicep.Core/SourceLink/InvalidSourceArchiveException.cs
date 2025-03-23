// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Exceptions;

namespace Bicep.Core.SourceLink
{
    public class InvalidSourceArchiveException : BicepException
    {
        public InvalidSourceArchiveException(string message)
            : base(message)
        {
        }
    }
}
