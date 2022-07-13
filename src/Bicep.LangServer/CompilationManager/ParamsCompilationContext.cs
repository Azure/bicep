// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;


namespace Bicep.LanguageServer.CompilationManager
{
    public record ParamsCompilationContext(ParamsSemanticModel ParamsSemanticModel, ProgramSyntax ProgramSyntax, ImmutableArray<int> LineStarts);
}
