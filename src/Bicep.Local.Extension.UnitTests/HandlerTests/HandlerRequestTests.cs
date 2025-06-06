// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Bicep.Local.Extension.Host.Handlers;
using FluentAssertions;

namespace Bicep.Local.Extension.UnitTests.HandlerTests;
[TestClass]
public class HandlerRequestTests
{
    private class TestResource { }

    [TestMethod]
    public void Constructor_Assigns_Properties_Correctly()
    {
        var type = "MyType";
        string? apiVersion = "2024-01-01";
        var extensionSettings = new JsonObject { ["setting"] = "value" };
        var resourceJson = new JsonObject { ["prop"] = 123 };

        var request = new HandlerRequest(type, apiVersion, extensionSettings, resourceJson);

        request.Type.Should().Be(type);
        request.ApiVersion.Should().Be(apiVersion);
        request.ExtensionSettings.Should().BeSameAs(extensionSettings);
        request.ResourceJson.Should().BeSameAs(resourceJson);
    }

    [TestMethod]
    public void Constructor_Allows_Null_ApiVersion()
    {
        var type = "MyType";
        string? apiVersion = null;
        var extensionSettings = new JsonObject();
        var resourceJson = new JsonObject();

        var request = new HandlerRequest(type, apiVersion, extensionSettings, resourceJson);

        request.Type.Should().Be(type);
        request.ApiVersion.Should().BeNull();
        request.ExtensionSettings.Should().BeSameAs(extensionSettings);
        request.ResourceJson.Should().BeSameAs(resourceJson);
    }

    [TestMethod]
    public void Constructor_Defaults_Optional_Properties()
    {
        var type = "MyType";
        string? apiVersion = "2024-01-01";

        var request = new HandlerRequest(type, apiVersion);

        request.Type.Should().Be(type);
        request.ApiVersion.Should().Be(apiVersion);
        request.ExtensionSettings.Should().NotBeNull();
        request.ResourceJson.Should().NotBeNull();
        request.ExtensionSettings?.Count.Should().Be(0);
        request.ResourceJson?.Count.Should().Be(0);
    }

    [TestMethod]
    public void HandlerRequestTResource_Constructor_Assigns_Properties()
    {
        var resource = new TestResource();
        string? apiVersion = "2024-01-01";
        var extensionSettings = new JsonObject { ["foo"] = "bar" };
        var resourceJson = new JsonObject { ["baz"] = 42 };

        var request = new HandlerRequest<TestResource>(resource, apiVersion, extensionSettings, resourceJson);

        request.Type.Should().Be(typeof(TestResource).Name);
        request.ApiVersion.Should().Be(apiVersion);
        request.ExtensionSettings.Should().BeSameAs(extensionSettings);
        request.ResourceJson.Should().BeSameAs(resourceJson);
        request.Resource.Should().BeSameAs(resource);
    }

    [TestMethod]
    public void Constructor_Throws_When_Type_Is_Null_Or_Whitespace()
    {
        Action act1 = () => new HandlerRequest(null!, "2024-01-01");
        Action act2 = () => new HandlerRequest("", "2024-01-01");
        Action act3 = () => new HandlerRequest("   ", "2024-01-01");

        act1.Should().Throw<ArgumentNullException>();
        act2.Should().Throw<ArgumentNullException>();
        act3.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void HandlerRequestTResource_Throws_When_Resource_Is_Null()
    {
        var extensionSettings = new JsonObject();
        var resourceJson = new JsonObject();
        Action act = () => new HandlerRequest<TestResource>(null!, "2024-01-01", extensionSettings, resourceJson);

        act.Should().Throw<ArgumentNullException>();
    }
}
