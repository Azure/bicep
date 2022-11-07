// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Threading.Tasks;

namespace Bicep.Decompiler;

public interface IBicepDecompiler
{
    Task<DecompileResult> Decompile(Uri entryJsonUri, Uri entryBicepUri);
}
