// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests.Assertions;
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
            DocumentUri documentUri = DocumentUri.Parse($"/{DataSets.Parameters_LF.Name}.bicep");
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, DataSets.Parameters_LF.Bicep, true);
            BicepBuildCommandHandler bicepBuildCommandHandler = new BicepBuildCommandHandler(bicepCompilationManager, Repository.Create<ISerializer>().Object);
            string expected = await bicepBuildCommandHandler.Handle(documentUri.Path, CancellationToken.None);

            expected.Should().BeEquivalentToIgnoringNewlines(@"Build started...
bicep build Parameters_LF.bicep
Build failed. Please fix errors in Parameters_LF.bicep
");
        }

        [TestMethod]
        public async Task Handle_WithValidPath_AndNoErrorsInInputFile_ReturnsBuildSucceededMessage()
        {
            DocumentUri documentUri = DocumentUri.Parse($"/{DataSets.Empty.Name}.bicep");
            BicepCompilationManager bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, DataSets.Empty.Bicep, true);
            BicepBuildCommandHandler bicepBuildCommandHandler = new BicepBuildCommandHandler(bicepCompilationManager, Repository.Create<ISerializer>().Object);
            string expected = await bicepBuildCommandHandler.Handle(documentUri.Path, CancellationToken.None);

            expected.Should().BeEquivalentToIgnoringNewlines(@"Build started...
bicep build Empty.bicep
Build succeeded. Created compiled file: Empty.json
");
        }
    }
}
