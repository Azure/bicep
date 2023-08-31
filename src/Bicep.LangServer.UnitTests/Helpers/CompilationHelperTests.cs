// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using CompilationHelper = Bicep.LanguageServer.Utils.CompilationHelper;

namespace Bicep.LangServer.UnitTests.Helpers
{
    [TestClass]
    public class CompilationHelperTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task GetCompilation_WithNullCompilationContext_ShouldCreateCompilation()
        {
            string bicepFileContents = @"resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'dnsZone'
  location: 'global'
}";
            string bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var bicepCompiler = ServiceBuilder.Create().GetCompiler();

            // Do not upsert compilation. This will cause CompilationContext to be null
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, upsertCompilation: false);
            var compilationContext = bicepCompilationManager.GetCompilation(documentUri);

            compilationContext.Should().BeNull();

            var compilation = await new CompilationHelper(bicepCompiler, bicepCompilationManager).GetRefreshedCompilation(documentUri);

            compilation.Should().NotBeNull();
        }

        [TestMethod]
        public async Task GetCompilation_WithNonNullCompilationContext_ShouldReuseCompilation()
        {
            string bicepFileContents = @"resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'dnsZone'
  location: 'global'
}";
            string bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var bicepCompiler = ServiceBuilder.Create().GetCompiler();

            // Upsert compilation. This will cause CompilationContext to be non null
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, upsertCompilation: true);

            var compilation = await new CompilationHelper(bicepCompiler, bicepCompilationManager).GetRefreshedCompilation(documentUri);

            compilation.Should().NotBeNull();
            compilation.Should().BeSameAs(bicepCompilationManager.GetCompilation(documentUri)!.Compilation);
        }
    }
}
