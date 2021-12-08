// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Emit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.UnitTests
{
    [TestClass]
    public static class AssemblyInitializer
    {
        [AssemblyInitialize()]
        public static void AssemblyInitialize(TestContext testContext)
        {
            BicepDeploymentsInterop.Initialize();
        }
    }
}
