// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;


namespace Bicep.LanguageServer.CompilationManager
{
    public record ParamsCompilationContext(ParamsSemanticModel ParamsSemanticModel)
    {
        public ProgramSyntax ProgramSyntax => ParamsSemanticModel.BicepParamFile.ProgramSyntax;

        public ImmutableArray<int> LineStarts => ParamsSemanticModel.BicepParamFile.LineStarts;
    }
}
