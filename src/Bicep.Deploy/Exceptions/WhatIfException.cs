// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Deploy.Exceptions;

public class WhatIfException : Exception
    {
        public WhatIfException(string message)
            : base(message)
        {
        }

        public WhatIfException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
