// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Syntax;


namespace Bicep.LanguageServer.CompilationManager
{
    public record ParamsCompilationContext(ProgramSyntax ProgramSyntax, ImmutableArray<int> LineStarts);
}
