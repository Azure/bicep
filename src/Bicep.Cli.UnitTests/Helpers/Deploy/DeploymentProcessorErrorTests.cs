// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Nodes;
using Azure.ResourceManager.Resources.Models;
using Bicep.Cli.Helpers.Deploy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.UnitTests.Helpers.Deploy
{
    [TestClass]
    public class DeploymentProcessorErrorTests
    {
        [TestMethod]
        public void FormatError_formats_correctly_with_symbolicName()
        {
            var result = DeploymentProcessor.FormatError(
                symbolicName: "symName",
                resourceType: "Microsoft.Foo/bar",
                resourceName: "myResource",
                errorCode: "BadRequest",
                errorMessage: "Something went wrong"
            );

            result.Should().Be("Resource 'symName' (Microsoft.Foo/bar 'myResource'): BadRequest: Something went wrong");
        }

        [TestMethod]
        public void FormatError_formats_correctly_without_symbolicName()
        {
            var result = DeploymentProcessor.FormatError(
                symbolicName: null,
                resourceType: "Microsoft.Foo/bar",
                resourceName: "myResource",
                errorCode: "BadRequest",
                errorMessage: "Something went wrong"
            );

            result.Should().Be("Resource Microsoft.Foo/bar 'myResource': BadRequest: Something went wrong");
        }

        [TestMethod]
        public void FormatError_formats_correctly_with_no_resource_info()
        {
            var result = DeploymentProcessor.FormatError(
                symbolicName: null,
                resourceType: null,
                resourceName: null,
                errorCode: "BadRequest",
                errorMessage: "Something went wrong"
            );

            result.Should().Be("BadRequest: Something went wrong");
        }

        [TestMethod]
        public void GetError_from_ArmDeploymentOperation_returns_null_if_no_error()
        {
            var operationMock = new Mock<ArmDeploymentOperation>();
            operationMock.Setup(o => o.Properties.StatusMessage).Returns((ArmDeploymentStatusMessage)null);

            DeploymentProcessor.GetError(operationMock.Object).Should().BeNull();
        }

        [TestMethod]
        public void GetError_from_ArmDeploymentOperation_returns_formatted_error()
        {
            var targetResource = new ArmDeploymentTargetResource
            {
                SymbolicName = "sym",
                ResourceName = "myResource",
                ResourceType = "Microsoft.Foo/bar"
            };

            var statusMessage = new ArmDeploymentStatusMessage
            {
                Error = new ArmDeploymentError
                {
                    Code = "BadRequest",
                    Message = "Something went wrong"
                }
            };

            var operationMock = new Mock<ArmDeploymentOperation>();
            operationMock.Setup(o => o.Properties.TargetResource).Returns(targetResource);
            operationMock.Setup(o => o.Properties.StatusMessage).Returns(statusMessage);

            var error = DeploymentProcessor.GetError(operationMock.Object);
            error.Should().Be("Resource 'sym' (Microsoft.Foo/bar 'myResource'): BadRequest: Something went wrong");
        }

        [TestMethod]
        public void GetError_from_DeploymentOperationDefinition_returns_null_if_no_error()
        {
            var operation = new DeploymentOperationDefinition
            {
                Properties = new DeploymentOperationProperties
                {
                    StatusMessage = JObject.Parse("""{}"""),
                    TargetResource = new DeploymentTargetResource()
                }
            };

            DeploymentProcessor.GetError(operation).Should().BeNull();
        }

        [TestMethod]
        public void GetError_from_DeploymentOperationDefinition_returns_formatted_error()
        {
            var operation = new DeploymentOperationDefinition
            {
                Properties = new DeploymentOperationProperties
                {
                    StatusMessage = JObject.Parse("""
                        {
                            "error": {
                                "code": "BadRequest",
                                "message": "Something went wrong"
                            }
                        }
                    """),
                    TargetResource = new DeploymentTargetResource
                    {
                        SymbolicName = "sym",
                        ResourceName = "myResource",
                        ResourceType = "Microsoft.Foo/bar"
                    }
                }
            };

            var error = DeploymentProcessor.GetError(operation);
            error.Should().Be("Resource 'sym' (Microsoft.Foo/bar 'myResource'): BadRequest: Something went wrong");
        }
    }
}
