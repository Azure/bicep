// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.Syntax;

namespace Bicep.Core.Parsing
{
    [Flags]
    public enum ExpressionFlags
    {
        None = 0,
        AllowComplexLiterals = 1,
        AllowResourceDeclarations = 2,

        /// <summary>
        /// Used to indicate that the expression appears directly within a colon-delimited context such as the 
        /// 'true' part of a ternary operator. This is used to resolve an ambiuity with the ':' operator for
        /// nested resource access.
        /// </summary>
        InsideColonDelimitedContext = 4,
    }
}