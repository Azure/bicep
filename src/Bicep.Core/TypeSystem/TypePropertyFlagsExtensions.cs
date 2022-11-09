// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.TypeSystem
{
    public static class TypePropertyFlagsExtensions
    {
        public static TypePropertyFlags Clear(this TypePropertyFlags flags, TypePropertyFlags flagsToClear) => flags & ~flagsToClear;

        public static TypePropertyFlags Set(this TypePropertyFlags flags, TypePropertyFlags flagsToSet) => flags & flagsToSet;
    }
}
