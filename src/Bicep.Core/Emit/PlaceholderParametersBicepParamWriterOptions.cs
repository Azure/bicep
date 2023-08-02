// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bicep.Core.Emit.Options
{
    public enum OutputFormatOption
    {
        Json,

        BicepParam
    }

    public enum IncludeParamsOption
    {
        RequiredOnly,

        All
    }
}
