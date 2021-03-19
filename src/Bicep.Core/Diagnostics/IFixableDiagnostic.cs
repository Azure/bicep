// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.CodeAction;
using System.Collections.Generic;

namespace Bicep.Core.Diagnostics
{
    public interface IFixableDiagnostic
    {
        IEnumerable<CodeFix> Fixes { get; }
    }
}