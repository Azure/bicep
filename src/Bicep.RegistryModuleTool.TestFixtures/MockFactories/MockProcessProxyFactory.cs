// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Proxies;
using Bicep.RegistryModuleTool.TestFixtures.Mocks;
using Moq;
using System.Linq.Expressions;

namespace Bicep.RegistryModuleTool.TestFixtures.MockFactories
{
    public static class MockProcessProxyFactory
    {
        public static IProcessProxy CreateProcessProxy(int exitCode = 0, string standardOutput = "", string standardError = "")
        {
            var proxyMock = StrictMock.Of<IProcessProxy>();
            var proxyMockSetup = proxyMock.Setup(x => x.Start(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((exitCode, standardOutput, standardError));

            return proxyMock.Object;
        }

        public static IProcessProxy CreateProcessProxy(Action? onSuccess)
        {
            var proxyMock = StrictMock.Of<IProcessProxy>();
            var proxyMockSetup = proxyMock.Setup(x => x.Start(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((0, "", ""))
                .Callback(onSuccess);

            return proxyMock.Object;
        }

        public static IProcessProxy CreateProcessProxy(params (Expression<Func<string, bool>> argumentsMatch, Action? onSuccess)[] setups)
        {
            var proxyMock = StrictMock.Of<IProcessProxy>();

            foreach (var (argumentsMatch, onSuccess) in setups)
            {
                var proxyMockSetup = proxyMock.Setup(x => x.Start(It.IsAny<string>(), It.Is(argumentsMatch)))
                    .Returns((0, "", ""))
                    .Callback(onSuccess);
            }

            return proxyMock.Object;
        }
    }
}
