// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Samples;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class CompilationTests
    {
        [TestMethod]
        public void EmptyProgram_SyntaxTreeGrouping_should_be_persisted()
        {
            var program = SyntaxFactory.CreateFromText(DataSets.Empty.Bicep);
            var compilation = new Compilation(TestResourceTypeProvider.Create(), program);

            compilation.SyntaxTreeGrouping.Should().BeSameAs(program);
            compilation.GetEntrypointSemanticModel().Should().NotBeNull();
        }
    }
}

