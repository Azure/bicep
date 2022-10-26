// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.Extensions
{
    public static class SyntaxBaseExtensions
    {
        public static T As<T>(this SyntaxBase syntax) where T : SyntaxBase =>
            syntax as T ?? throw new InvalidOperationException($"Expected {nameof(syntax)} of {syntax.GetType()} to be an instance of {nameof(T)}.");
    }
}
