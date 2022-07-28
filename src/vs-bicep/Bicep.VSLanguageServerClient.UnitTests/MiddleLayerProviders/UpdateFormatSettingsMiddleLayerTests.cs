// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.VSLanguageServerClient.MiddleLayerProviders;
using FluentAssertions;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.VSLanguageServerClient.UnitTests.MiddleLayerProviders
{
    [TestClass]
    public class UpdateFormatSettingsMiddleLayerTests
    {
        [TestMethod]
        public void UpdateFormatOptions_WithInvalidInput_ShouldThrow()
        {
            var updateFormatSettingsMiddleLayer = new UpdateFormatSettingsMiddleLayer();
            var jtoken = JToken.FromObject("some_text");

            Action action = () => updateFormatSettingsMiddleLayer.UpdateFormatOptions(jtoken);
            action.Should().Throw<Exception>();
        }

        [TestMethod]
        public void UpdateFormatOptions_WithValidInput_ShouldUpdateFormattingOptions()
        {
            var documentFormattingParams = new DocumentFormattingParams()
            {
                Options = new FormattingOptions()
                {
                    TabSize = 4,
                    InsertSpaces = false
                }
            };
            var input = JToken.FromObject(documentFormattingParams);

            var updateFormatSettingsMiddleLayer = new UpdateFormatSettingsMiddleLayer();

            var result = updateFormatSettingsMiddleLayer.UpdateFormatOptions(input).ToObject<DocumentFormattingParams>();

            result.Should().NotBeNull();
            result!.Options.InsertSpaces.Should().BeTrue();
            result.Options.TabSize.Should().Be(2);
        }

        [TestMethod]
        public void UpdateFormatOptions_WithValidInputAndAppropriateFormattingOptions_DoesNothing()
        {
            var documentFormattingParams = new DocumentFormattingParams()
            {
                Options = new FormattingOptions()
                {
                    TabSize = 2,
                    InsertSpaces = true
                }
            };
            var input = JToken.FromObject(documentFormattingParams);

            var updateFormatSettingsMiddleLayer = new UpdateFormatSettingsMiddleLayer();

            var result = updateFormatSettingsMiddleLayer.UpdateFormatOptions(input).ToObject<DocumentFormattingParams>();

            result.Should().NotBeNull();
            result!.Options.InsertSpaces.Should().BeTrue();
            result.Options.TabSize.Should().Be(2);
        }
    }
}
