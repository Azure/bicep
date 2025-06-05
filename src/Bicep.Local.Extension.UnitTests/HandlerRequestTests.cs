// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Bicep.Local.Extension.Host.Handlers;

namespace Bicep.Local.Extension.UnitTests;
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

        Assert.AreEqual(type, request.Type);
        Assert.AreEqual(apiVersion, request.ApiVersion);
        Assert.AreEqual(extensionSettings, request.ExtensionSettings);
        Assert.AreEqual(resourceJson, request.ResourceJson);
    }

    [TestMethod]
    public void Constructor_Allows_Null_ApiVersion()
    {
        var type = "MyType";
        string? apiVersion = null;
        var extensionSettings = new JsonObject();
        var resourceJson = new JsonObject();

        var request = new HandlerRequest(type, apiVersion, extensionSettings, resourceJson);

        Assert.AreEqual(type, request.Type);
        Assert.IsNull(request.ApiVersion);
        Assert.AreEqual(extensionSettings, request.ExtensionSettings);
        Assert.AreEqual(resourceJson, request.ResourceJson);
    }

    [TestMethod]
    public void Constructor_Defaults_Optional_Properties()
    {
        var type = "MyType";
        string? apiVersion = "2024-01-01";

        var request = new HandlerRequest(type, apiVersion);

        Assert.AreEqual(type, request.Type);
        Assert.AreEqual(apiVersion, request.ApiVersion);
        Assert.IsNotNull(request.ExtensionSettings);
        Assert.IsNotNull(request.ResourceJson);
        Assert.AreEqual(0, request.ExtensionSettings.Count);
        Assert.AreEqual(0, request.ResourceJson.Count);
    }

    [TestMethod]
    public void HandlerRequestTResource_Constructor_Assigns_Properties()
    {
        var resource = new TestResource();
        string? apiVersion = "2024-01-01";
        var extensionSettings = new JsonObject { ["foo"] = "bar" };
        var resourceJson = new JsonObject { ["baz"] = 42 };

        var request = new HandlerRequest<TestResource>(resource, apiVersion, extensionSettings, resourceJson);

        Assert.AreEqual(typeof(TestResource).Name, request.Type);
        Assert.AreEqual(apiVersion, request.ApiVersion);
        Assert.AreEqual(extensionSettings, request.ExtensionSettings);
        Assert.AreEqual(resourceJson, request.ResourceJson);
        Assert.AreEqual(resource, request.Resource);
    }

    [TestMethod]
    public void Constructor_Throws_When_Type_Is_Null_Or_Whitespace()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new HandlerRequest(null!, "2024-01-01"));
        Assert.ThrowsException<ArgumentNullException>(() => new HandlerRequest("", "2024-01-01"));
        Assert.ThrowsException<ArgumentNullException>(() => new HandlerRequest("   ", "2024-01-01"));
    }

    [TestMethod]
    public void HandlerRequestTResource_Throws_When_Resource_Is_Null()
    {
        var extensionSettings = new JsonObject();
        var resourceJson = new JsonObject();
        Assert.ThrowsException<ArgumentNullException>(() =>
            new HandlerRequest<TestResource>(null!, "2024-01-01", extensionSettings, resourceJson));
    }

}
