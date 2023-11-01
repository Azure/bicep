// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Configuration;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics
{
    public interface ISymbolContext
    {
        ITypeManager TypeManager { get; }

        Compilation Compilation { get; }

        IBinder Binder { get; }

        BicepSourceFile SourceFile { get; }
    }
}
