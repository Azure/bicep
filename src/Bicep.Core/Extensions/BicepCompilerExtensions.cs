// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Emit;
using Bicep.Core.SourceGraph;
using Bicep.IO.Abstraction;

namespace Bicep.Core;

public static class BicepCompilerExtensions
{
    public static async Task<TemplateResult> CompileBicepFile(this BicepCompiler compiler, IOUri bicepUri, bool skipRestore = false, bool forceRestore = false)
    {
        var compilation = await compiler.CreateCompilation(bicepUri, skipRestore: skipRestore, forceRestore: forceRestore);

        return compilation.Emitter.Template();
    }

    public static async Task<ParametersResult> CompileBicepparamFile(this BicepCompiler compiler, IOUri bicepUri, bool skipRestore = false, bool forceRestore = false)
    {
        var compilation = await compiler.CreateCompilation(bicepUri, skipRestore: skipRestore, forceRestore: forceRestore);

        return compilation.Emitter.Parameters();
    }
}
