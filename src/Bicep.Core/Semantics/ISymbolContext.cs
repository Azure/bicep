// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Semantics
{
    public interface ISymbolContext
    {
        ITypeManager TypeManager { get; }

        Compilation Compilation { get; }

        IBinder Binder { get; }
    }
}
