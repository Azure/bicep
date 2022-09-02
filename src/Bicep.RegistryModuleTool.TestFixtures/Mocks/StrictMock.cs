// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Moq;

namespace Bicep.RegistryModuleTool.TestFixtures.Mocks
{
    public static class StrictMock
    {
        public static Mock<T> Of<T>() where T : class => new(MockBehavior.Strict);
    }
}
