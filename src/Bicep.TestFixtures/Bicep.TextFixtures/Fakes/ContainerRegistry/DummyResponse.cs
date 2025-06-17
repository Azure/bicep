// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure;
using Bicep.Core.UnitTests.Mock;

namespace Bicep.TextFixtures.Fakes.ContainerRegistry
{
    public static class DummyResponse
    {
        public static readonly Response Instance = StrictMock.Of<Response>().Object;
    }
}
