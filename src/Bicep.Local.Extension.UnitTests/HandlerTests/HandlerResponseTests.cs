// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Bicep.Local.Extension.Host.Handlers;
using Json.Path;

namespace Bicep.Local.Extension.UnitTests.HandlerTests;

[TestClass]
public class HandlerResponseTests
{
    [TestMethod]
    public void Constructor_Assigns_Required_Fields()
    {
        var type = "MyType";
        var status = HandlerResponseStatus.Success;
        var response = new HandlerResponse(type, "2024-01-01", status, new());

        Assert.AreEqual(type, response.Type);
        Assert.AreEqual(status, response.Status);
    }

    [TestMethod]
    public void Constructor_Throws_When_Type_Is_Null_Or_Whitespace()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new HandlerResponse(null!, "2024-01-01", HandlerResponseStatus.Success, new()));
        Assert.ThrowsException<ArgumentNullException>(() => new HandlerResponse("", "2024-01-01", HandlerResponseStatus.Success, new()));
        Assert.ThrowsException<ArgumentNullException>(() => new HandlerResponse("   ", "2024-01-01", HandlerResponseStatus.Success, new()));
    }

    [TestMethod]
    public void Properties_Defaults_To_New_Instance_And_Is_Mutable()
    {
        var response = new HandlerResponse("MyType", "2024-01-01", HandlerResponseStatus.Success, null);
        Assert.IsNotNull(response.Properties);

        // Test mutability
        response.Properties["newProp"] = 123;
        response.Properties.TryGetPropertyValue("newProp", out var prop);
        Assert.IsNotNull(prop);
        Assert.AreEqual(123, prop?.GetValue<int>());
    }


    [TestMethod]
    public void Constructor_Assigns_Optional_Fields()
    {
        var type = "MyType";
        string? apiVersion = "2024-01-01";
        var status = HandlerResponseStatus.Error;
        var properties = new JsonObject { ["foo"] = "bar" };
        var error = new Error("code", "target", "message");
        var message = "error message";

        var response = new HandlerResponse(type, apiVersion, status, properties, error, message);

        Assert.AreEqual(apiVersion, response.ApiVersion);
        Assert.AreEqual(properties, response.Properties);
        Assert.AreEqual(error, response.Error);
        Assert.AreEqual(message, response.Message);
    }

    [TestMethod]
    public void Constructor_Defaults_Optional_Fields()
    {
        var type = "MyType";
        var status = HandlerResponseStatus.Success;
        var response = new HandlerResponse(type, null, status, null);

        Assert.IsNull(response.ApiVersion);
        Assert.IsNotNull(response.Properties);
        Assert.IsNull(response.Error);
        Assert.IsNull(response.Message);
    }

    [TestMethod]
    public void Success_Method_Assigns_Expected_Values()
    {
        var type = "MyType";
        var apiVersion = "2024-01-01";
        var properties = new JsonObject { ["foo"] = "bar" };
        var message = "success";

        var response = HandlerResponse.Success(type, apiVersion, properties, message);

        Assert.AreEqual(type, response.Type);
        Assert.AreEqual(apiVersion, response.ApiVersion);
        Assert.AreEqual(HandlerResponseStatus.Success, response.Status);
        Assert.AreEqual(properties, response.Properties);
        Assert.IsNull(response.Error);
        Assert.AreEqual(message, response.Message);
    }

    [TestMethod]
    public void Failed_Method_Assigns_Expected_Values()
    {
        var type = "MyType";
        var apiVersion = "2024-01-01";
        var properties = new JsonObject { ["foo"] = "bar" };
        var error = new Error("code", "target", "message");
        var message = "fail";

        var response = HandlerResponse.Failed(type, apiVersion, properties, error, message);

        Assert.AreEqual(type, response.Type);
        Assert.AreEqual(apiVersion, response.ApiVersion);
        Assert.AreEqual(HandlerResponseStatus.Error, response.Status);
        Assert.AreEqual(properties, response.Properties);
        Assert.AreEqual(error, response.Error);
        Assert.AreEqual(message, response.Message);
    }

    [TestMethod]
    public void Canceled_Method_Assigns_Expected_Values()
    {
        var type = "MyType";
        var apiVersion = "2024-01-01";
        var properties = new JsonObject { ["foo"] = "bar" };
        var message = "canceled";

        var response = HandlerResponse.Canceled(type, apiVersion, properties, message);

        Assert.AreEqual(type, response.Type);
        Assert.AreEqual(apiVersion, response.ApiVersion);
        Assert.AreEqual(HandlerResponseStatus.Canceled, response.Status);
        Assert.AreEqual(properties, response.Properties);
        Assert.IsNull(response.Error);
        Assert.AreEqual(message, response.Message);
    }

    [TestMethod]
    public void TimedOut_Method_Assigns_Expected_Values()
    {
        var type = "MyType";
        var apiVersion = "2024-01-01";
        var properties = new JsonObject { ["foo"] = "bar" };
        var message = "timeout";

        var response = HandlerResponse.TimedOut(type, apiVersion, properties, message);

        Assert.AreEqual(type, response.Type);
        Assert.AreEqual(apiVersion, response.ApiVersion);
        Assert.AreEqual(HandlerResponseStatus.TimedOut, response.Status);
        Assert.AreEqual(properties, response.Properties);
        Assert.IsNull(response.Error);
        Assert.AreEqual(message, response.Message);
    }

    [TestMethod]
    public void Error_Object_Has_Expected_Content()
    {
        var error = new Error("E001", "targetField", "Something went wrong");
        var response = new HandlerResponse("MyType", "2024-01-01", HandlerResponseStatus.Error, new JsonObject(), error);

        Assert.IsNotNull(response.Error);
        Assert.AreEqual("E001", response.Error.Code);
        Assert.AreEqual("targetField", response.Error.Target);
        Assert.AreEqual("Something went wrong", response.Error.Message);
    }

    [TestMethod]
    public void Static_Methods_Throw_When_Type_Is_Null_Or_Whitespace()
    {
        Assert.ThrowsException<ArgumentNullException>(() => HandlerResponse.Success(null!, "v", null));
        Assert.ThrowsException<ArgumentNullException>(() => HandlerResponse.Success("", "v", null));
        Assert.ThrowsException<ArgumentNullException>(() => HandlerResponse.Success("   ", "v", null));

        Assert.ThrowsException<ArgumentNullException>(() => HandlerResponse.Failed(null!, "v", null));
        Assert.ThrowsException<ArgumentNullException>(() => HandlerResponse.Canceled(null!, "v", null));
        Assert.ThrowsException<ArgumentNullException>(() => HandlerResponse.TimedOut(null!, "v", null));
    }

    [TestMethod]
    public void Properties_Object_Is_Not_Shared_Between_Instances()
    {
        var r1 = new HandlerResponse("MyType", "v", HandlerResponseStatus.Success, null);
        var r2 = new HandlerResponse("MyType", "v", HandlerResponseStatus.Success, null);

        r1.Properties["foo"] = 1;
        Assert.IsFalse(r2.Properties.ContainsKey("foo"));
    }

    [TestMethod]
    public void ExtensionSettings_Is_Always_Null()
    {
        var response = new HandlerResponse("MyType", "v", HandlerResponseStatus.Success, null);
        Assert.IsNull(response.ExtensionSettings);
    }

}
