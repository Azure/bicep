// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.IO.Abstraction
{
    public interface IResourceHandle : IEquatable<IResourceHandle>
    {
        public ResourceIdentifier Identifier { get; }

        public bool Exists();

        public string ToString();
    }
}
