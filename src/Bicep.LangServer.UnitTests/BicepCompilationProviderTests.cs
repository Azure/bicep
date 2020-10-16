// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.FileSystem;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Providers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LangServer.UnitTests
{
    [TestClass]
    public class BicepCompilationProviderTests
    {
        private static IFileResolver CreateEmptyFileResolver()
            => new InMemoryFileResolver(new Dictionary<string, string>());

        [TestMethod]
        public void Create_ShouldReturnValidCompilation()
        {
            var provider = new BicepCompilationProvider(TestResourceTypeProvider.Create(), CreateEmptyFileResolver());

            var fileUri = DocumentUri.Parse($"/{DataSets.Parameters_LF.Name}.bicep");
            var context = provider.Create(fileUri, DataSets.Parameters_LF.Bicep);

            context.Compilation.Should().NotBeNull();
            context.Compilation.GetEntrypointSemanticModel().GetAllDiagnostics().Should().BeEmpty();
            context.LineStarts.Should().NotBeEmpty();
            context.LineStarts[0].Should().Be(0);
        }
    }
}

