// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Diagnostics;
using Bicep.Core.SemanticModel;
using Bicep.Core.UnitTests.Assertions;

namespace Bicep.Core.UnitTests.Utils
{
    public class CompilationHelper
    {
        private readonly Func<string, Compilation> createCompilationFunc;

        public CompilationHelper(Func<string, Compilation> createCompilationFunc)
        {
            this.createCompilationFunc = createCompilationFunc;
        }

        public void ProgramShouldHaveDiagnostics(string program, params (string code, DiagnosticLevel level, string messgae)[] diagnostics)
        {
            createCompilationFunc(program).Should().HaveDiagnostics(diagnostics);
        }
    }
}