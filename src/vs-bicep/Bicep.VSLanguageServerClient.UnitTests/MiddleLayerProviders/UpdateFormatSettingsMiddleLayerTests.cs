// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Bicep.VSLanguageServerClient.MiddleLayerProviders;
using Bicep.VSLanguageServerClient.Settings;
using FluentAssertions;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Threading;
using Moq;
using Newtonsoft.Json.Linq;

namespace Bicep.VSLanguageServerClient.UnitTests.MiddleLayerProviders
{
    [TestClass]
    public class UpdateFormatSettingsMiddleLayerTests
    {
        private static readonly MockRepository Repository = new MockRepository(MockBehavior.Strict);
        private Mock<IBicepSettings> BicepSettingsMock = Repository.Create<IBicepSettings>();

        [TestMethod]
        public void UpdateFormatOptions_WithInvalidInput_ShouldThrow()
        {
            var updateFormatSettingsMiddleLayer = new UpdateFormatSettingsMiddleLayer(BicepSettingsMock.Object);
            var jtoken = JToken.FromObject("some_text");

            Task task = updateFormatSettingsMiddleLayer.UpdateFormatOptionsAsync(jtoken);

            Action action = () => ThreadHelper.JoinableTaskFactory.RunAsync(async delegate {
                await updateFormatSettingsMiddleLayer.UpdateFormatOptionsAsync(jtoken);
            });

            action.Should().Throw<Exception>();
        }

        [TestMethod]
        public async Task UpdateFormatOptions_WithValidInput_ShouldUpdateFormattingOptions()
        {
            var documentFormattingParams = new DocumentFormattingParams();
            documentFormattingParams.Options = new FormattingOptions();
            var input = JToken.FromObject(documentFormattingParams);

            BicepSettingsMock.Setup(x => x.GetIntegerAsync(BicepLanguageServerClientConstants.FormatterTabSizeKey, 2).Result).Returns(2);
            BicepSettingsMock.Setup(x => x.GetIntegerAsync(BicepLanguageServerClientConstants.FormatterIndentSizeKey, 2).Result).Returns(2);
            BicepSettingsMock.Setup(x => x.GetIntegerAsync(BicepLanguageServerClientConstants.FormatterIndentTypeKey, (int)IndentType.Spaces).Result).Returns((int)IndentType.Tabs);

            var updateFormatSettingsMiddleLayer = new UpdateFormatSettingsMiddleLayer(BicepSettingsMock.Object);

            var result = (await updateFormatSettingsMiddleLayer.UpdateFormatOptionsAsync(input)).ToObject<DocumentFormattingParams>();

            result.Should().NotBeNull();
            result!.Options.InsertSpaces.Should().BeFalse();
            result.Options.TabSize.Should().Be(2);
        }
    }
}
