// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Emit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Decompiler.IntegrationTests
{
    [TestClass]
    public static class AssemblyInitializer
    {
        [AssemblyInitialize()]
        public static void MyTestInitialize(TestContext testContext)
        {
            BicepDeploymentsInterop.Initialize();
        }
    }
}