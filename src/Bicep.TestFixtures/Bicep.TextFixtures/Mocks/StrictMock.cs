// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Moq;

namespace Bicep.TextFixtures.Mocks
{
    public static class StrictMock
    {
        public static Mock<T> Of<T>() where T : class
        {
#pragma warning disable RS0030 // Do not use banned APIs
            return new Mock<T>(MockBehavior.Strict);
#pragma warning restore RS0030 // Do not use banned APIs
        }
    }
}
