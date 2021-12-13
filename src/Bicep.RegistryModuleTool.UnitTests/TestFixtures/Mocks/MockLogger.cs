// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Moq;

namespace Bicep.RegistryModuleTool.UnitTests.TestFixtures.Mocks
{
    public static class MockLogger
    {
        public static MockGenericLogger<It.IsAnyType> Create() => MockGenericLogger<It.IsAnyType>.Create();
    }
}
