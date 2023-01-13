// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.Registry.Oci
{
    public class OciAnnotations
    {
        public OciAnnotations(string documentation)
        {
            this.Documentation = documentation;
        }

        public string Documentation { get; }
    }
}
