// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.VSLanguageServerClient.MiddleLayerProviders;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharpLocation = OmniSharp.Extensions.LanguageServer.Protocol.Models.Location;
using OmniSharpPosition = OmniSharp.Extensions.LanguageServer.Protocol.Models.Position;
using OmniSharpRange = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.VSLanguageServerClient.UnitTests.MiddleLayerProviders
{
    [TestClass]
    public class HandleGotoDefintionMiddleLayerTests
    {
        [TestMethod]
        public void GetVSLocations_WithValidLocation_ShouldReturnVSLocation()
        {
            var handleGotoDefintionMiddleLayer = new HandleGotoDefintionMiddleLayer();
            var uri = DocumentUri.From("some_path");
            var omniSharpLocation = new OmniSharpLocation
            {
                Uri = uri,
                Range = new OmniSharpRange()
                {
                    Start = new OmniSharpPosition(5, 11),
                    End = new OmniSharpPosition(5, 19)
                }
            };
            var locationOrLocationLink = new LocationOrLocationLink(omniSharpLocation);

            var vsLocation = handleGotoDefintionMiddleLayer.GetVSLocation(locationOrLocationLink);

            Assert.IsNotNull(vsLocation);
            vsLocation.Uri.Should().Be(uri.ToUri());

            var resultStart = vsLocation.Range.Start;
            var resultEnd = vsLocation.Range.End;

            resultStart.Line.Should().Be(5);
            resultStart.Character.Should().Be(11);
            resultEnd.Line.Should().Be(5);
            resultEnd.Character.Should().Be(19);
        }

        [TestMethod]
        public void GetVSLocations_WithValidLocationLink_ShouldReturnVSLocation()
        {
            var handleGotoDefintionMiddleLayer = new HandleGotoDefintionMiddleLayer();
            var uri = DocumentUri.From("some_path");
            var omniSharpLocationLink = new LocationLink
            {
                TargetUri = uri,
                OriginSelectionRange = new OmniSharpRange()
                {
                    Start = new OmniSharpPosition(2, 3),
                    End = new OmniSharpPosition(2, 6)
                },
                TargetRange = new OmniSharpRange()
                {
                    Start = new OmniSharpPosition(5, 11),
                    End = new OmniSharpPosition(5, 20)
                },
                TargetSelectionRange = new OmniSharpRange()
                {
                    Start = new OmniSharpPosition(5, 11),
                    End = new OmniSharpPosition(5, 30)
                }
            };
            var locationOrLocationLink = new LocationOrLocationLink(omniSharpLocationLink);

            var vsLocation = handleGotoDefintionMiddleLayer.GetVSLocation(locationOrLocationLink);

            Assert.IsNotNull(vsLocation);
            vsLocation.Uri.Should().Be(uri.ToUri());

            var resultStart = vsLocation.Range.Start;
            var resultEnd = vsLocation.Range.End;

            resultStart.Line.Should().Be(5);
            resultStart.Character.Should().Be(11);
            resultEnd.Line.Should().Be(5);
            resultEnd.Character.Should().Be(30);
        }
    }
}
