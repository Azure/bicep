// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;


namespace Bicep.LanguageServer.CompilationManager
{
    public record ParamsCompilationContext(ProgramSyntax Program, int ChangeCount);
}
