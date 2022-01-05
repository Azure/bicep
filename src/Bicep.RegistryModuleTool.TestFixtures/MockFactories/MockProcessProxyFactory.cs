// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Proxies;
using Bicep.RegistryModuleTool.TestFixtures.Mocks;
using Moq;

namespace Bicep.RegistryModuleTool.TestFixtures.MockFactories
{
    public static class MockProcessProxyFactory
    {
        public static IProcessProxy CreateProcessProxy(int exitCode = 0, string standardOutput = "", string standardError = "", Action? callback = null)
        {
            var proxyMock = StrictMock.Of<IProcessProxy>();
            var proxyMockSetup = proxyMock.Setup(x => x.Start(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((exitCode, standardOutput, standardError));

            if (callback is not null)
            {
                proxyMockSetup.Callback(callback);
            }

            return proxyMock.Object;
        }
    }
}
