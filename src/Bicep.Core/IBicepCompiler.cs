// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Threading.Tasks;
using Bicep.Core.Semantics;
using Bicep.Core.Workspaces;

namespace Bicep.Core;

public interface IBicepCompiler
{
    Task<Compilation> CreateCompilation(Uri bicepFile, bool skipRestore = false, IReadOnlyWorkspace? workspace = null);
}
