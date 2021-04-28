// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests.Semantics
{
    [TestClass]
    public class CompilationTests
    {
        [TestMethod]
        public void EmptyProgram_SyntaxTreeGrouping_should_be_persisted()
        {
            var program = SyntaxTreeGroupingFactory.CreateFromText(DataSets.Empty.Bicep);
            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), program);

            compilation.SyntaxTreeGrouping.Should().BeSameAs(program);
            compilation.GetEntrypointSemanticModel().Should().NotBeNull();
        }
    }
}

