// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LangServer.UnitTests.Handlers
{
    [TestClass]
    [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Test methods do not need to follow this convention.")]
    public class BicepBuildCommandHandlerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static readonly MockRepository Repository = new(MockBehavior.Strict);
        private static readonly ISerializer Serializer = Repository.Create<ISerializer>().Object;

        [DataRow("invalid_path")]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [DataTestMethod]
        public void Handle_WithInvalidPath_ShouldThrowArgumentException(string path)
        {
            ICompilationManager bicepCompilationManager = Repository.Create<ICompilationManager>().Object;
            BicepBuildCommandHandler bicepBuildCommandHandler = new BicepBuildCommandHandler(bicepCompilationManager, Serializer);

            Action sut = () => bicepBuildCommandHandler.Handle(path, CancellationToken.None);

            sut.Should().Throw<ArgumentException>().WithMessage("Invalid input file");
        }

        [TestMethod]
        public void Handle_WithNullContext_ShouldThrowInvalidOperationException()
        {
            DocumentUri documentUri = DocumentUri.Parse($"/{DataSets.Parameters_LF.Name}.bicep");
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, string.Empty, false);
            BicepBuildCommandHandler bicepBuildCommandHandler = new BicepBuildCommandHandler(bicepCompilationManager, Serializer);

            Action sut = () => bicepBuildCommandHandler.Handle(documentUri.Path, CancellationToken.None);

            sut.Should().Throw<InvalidOperationException>().WithMessage("Unable to get compilation context");
        }

        [TestMethod]
        public async Task Handle_WithValidPath_AndErrorsInInputFile_ReturnsBuildFailedMessage()
        {
            DocumentUri documentUri = DocumentUri.Parse("test.bicep");
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, @"targetScope

// #completionTest(12) -> empty
targetScope 

// #completionTest(13,14) -> targetScopes
targetScope = 


targetScope = 'asdfds'

targetScope = { }

targetScope = true
", true);
            BicepBuildCommandHandler bicepBuildCommandHandler = new BicepBuildCommandHandler(bicepCompilationManager, Repository.Create<ISerializer>().Object);
            string expected = await bicepBuildCommandHandler.Handle(documentUri.Path, CancellationToken.None);

            expected.Should().BeEquivalentToIgnoringNewlines(@"Build started...
bicep build test.bicep
Build failed. Please fix below errors in test.bicep :
1. The ""targetScope"" cannot be declared multiple times in one file. bicep(BCP112). [1, 1]
2. Expected the ""="" character at this location. bicep(BCP018). [1, 12]
3. Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. bicep(BCP009). [1, 12]
4. The ""targetScope"" cannot be declared multiple times in one file. bicep(BCP112). [4, 1]
5. Expected the ""="" character at this location. bicep(BCP018). [4, 13]
6. Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. bicep(BCP009). [4, 13]
7. The ""targetScope"" cannot be declared multiple times in one file. bicep(BCP112). [7, 1]
8. Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. bicep(BCP009). [7, 15]
9. The ""targetScope"" cannot be declared multiple times in one file. bicep(BCP112). [10, 1]
10. Expected a value of type ""'managementGroup' | 'resourceGroup' | 'subscription' | 'tenant'"" but the provided value is of type ""'asdfds'"". bicep(BCP033). [10, 15]
11. The ""targetScope"" cannot be declared multiple times in one file. bicep(BCP112). [12, 1]
12. Expected a value of type ""'managementGroup' | 'resourceGroup' | 'subscription' | 'tenant'"" but the provided value is of type ""object"". bicep(BCP033). [12, 15]
13. The ""targetScope"" cannot be declared multiple times in one file. bicep(BCP112). [14, 1]
14. Expected a value of type ""'managementGroup' | 'resourceGroup' | 'subscription' | 'tenant'"" but the provided value is of type ""bool"". bicep(BCP033). [14, 15]
");
        }

        [TestMethod]
        public async Task Handle_WithValidPath_AndNoErrorsInInputFile_ReturnsBuildSucceededMessage()
        {
            string bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", string.Empty);
            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, string.Empty, true);
            BicepBuildCommandHandler bicepBuildCommandHandler = new BicepBuildCommandHandler(bicepCompilationManager, Repository.Create<ISerializer>().Object);
            string expected = await bicepBuildCommandHandler.Handle(bicepFilePath, CancellationToken.None);

            expected.Should().BeEquivalentToIgnoringNewlines(@"Build started...
bicep build input.bicep
Build succeeded. Created transpiled ARM template: input.json
");
        }
    }
}
