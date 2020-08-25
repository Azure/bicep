// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Samples;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class CompilationTests
    {
        [TestMethod]
        public void EmptyProgram_SyntaxNodeShouldBePersisted()
        {
            var program = SyntaxFactory.CreateFromText(DataSets.Empty.Bicep);
            var compilation = new Compilation(program);

            compilation.ProgramSyntax.Should().BeSameAs(program);
            compilation.GetSemanticModel().Should().NotBeNull();
        }
    }
}

