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
        [TestMethod]
        public void EmptyProgram_SourceFileGrouping_should_be_persisted()
        {
            var fileResolver = BicepTestConstants.FileResolver;
            var program = SourceFileGroupingFactory.CreateFromText(DataSets.Empty.Bicep, fileResolver);
            var compilation = new Compilation(BicepTestConstants.FeatureProviderFactory, TestTypeHelper.CreateEmptyProvider(), program, BicepTestConstants.BuiltInOnlyConfigurationManager, BicepTestConstants.ApiVersionProviderFactory, BicepTestConstants.LinterAnalyzer);

            compilation.SourceFileGrouping.Should().BeSameAs(program);
            compilation.GetEntrypointSemanticModel().Should().NotBeNull();
        }
    }
}
