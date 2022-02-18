// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Azure.ResourceManager;
using Bicep.Core.Deploy;
using Bicep.Core.UnitTests.Mock;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Deploy
{
    [TestClass]

    public class DeploymentHelperTests
    {
        [TestMethod]
        public async Task CreateDeployment_WithInvalidScope_ReturnsDeploymentFailureMessage()
        {
            var armClient = CreateMockArmClient();
            var result = await DeploymentHelper.CreateDeployment(armClient, string.Empty, string.Empty, "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41", "invalid_scope", string.Empty);

            result.Should().Be("Deployment failed. Please provide a valid scope.");
        }

        private static ArmClient CreateMockArmClient()
        {
            var clientMock = StrictMock.Of<ArmClient>();

            return clientMock.Object;
        }
    }
}
