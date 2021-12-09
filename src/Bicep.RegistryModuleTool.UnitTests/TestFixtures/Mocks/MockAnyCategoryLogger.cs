// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Moq;

namespace Bicep.RegistryModuleTool.UnitTests.TestFixtures.Mocks
{
    public static class MockAnyCategoryLogger
    {
        public static MockLogger<It.IsAnyType> Create() => MockLogger<It.IsAnyType>.Create();
    }
}
