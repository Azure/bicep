// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.SourceLink
{
    public record LinkedSourceFile(LinkedSourceFileMetadata Metadata, string Contents)
    {
    }
}
