// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.FileSystem;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests.Semantics
{
    [TestClass]
    public class CompilationTests
    {
        private static ServiceBuilder Services => new ServiceBuilder().WithEmptyAzResources();

        [TestMethod]
        public void EmptyProgram_SourceFileGrouping_should_be_persisted()
        {
            var sourceFileGrouping = Services.BuildSourceFileGrouping(DataSets.Empty.Bicep);
            var compilation = Services.Build().BuildCompilation(sourceFileGrouping);

            compilation.SourceFileGrouping.Should().BeSameAs(sourceFileGrouping);
            compilation.GetEntrypointSemanticModel().Should().NotBeNull();
        }
    }
}
