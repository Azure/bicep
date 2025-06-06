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

        response.Type.Should().Be(type);
        response.Status.Should().Be(status);
    }

    [TestMethod]
    public void Constructor_Throws_When_Type_Is_Null_Or_Whitespace()
    {
        Action act1 = () => new HandlerResponse(null!, "2024-01-01", HandlerResponseStatus.Success, new());
        Action act2 = () => new HandlerResponse("", "2024-01-01", HandlerResponseStatus.Success, new());
        Action act3 = () => new HandlerResponse("   ", "2024-01-01", HandlerResponseStatus.Success, new());

        act1.Should().Throw<ArgumentNullException>();
        act2.Should().Throw<ArgumentNullException>();
        act3.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void Properties_Defaults_To_New_Instance_And_Is_Mutable()
    {
        var response = new HandlerResponse("MyType", "2024-01-01", HandlerResponseStatus.Success, null);
        response.Properties.Should().NotBeNull();

        // Test mutability
        response.Properties["newProp"] = 123;
        response.Properties.TryGetPropertyValue("newProp", out var prop);
        prop.Should().NotBeNull();
        prop?.GetValue<int>().Should().Be(123);
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

        response.ApiVersion.Should().Be(apiVersion);
        response.Properties.Should().BeSameAs(properties);
        response.Error.Should().BeSameAs(error);
        response.Message.Should().Be(message);
    }

    [TestMethod]
    public void Constructor_Defaults_Optional_Fields()
    {
        var type = "MyType";
        var status = HandlerResponseStatus.Success;
        var response = new HandlerResponse(type, null, status, null);

        response.ApiVersion.Should().BeNull();
        response.Properties.Should().NotBeNull();
        response.Error.Should().BeNull();
        response.Message.Should().BeNull();
    }

    [TestMethod]
    public void Success_Method_Assigns_Expected_Values()
    {
        var type = "MyType";
        var apiVersion = "2024-01-01";
        var properties = new JsonObject { ["foo"] = "bar" };
        var message = "success";

        var response = HandlerResponse.Success(type, apiVersion, properties, message);

        response.Type.Should().Be(type);
        response.ApiVersion.Should().Be(apiVersion);
        response.Status.Should().Be(HandlerResponseStatus.Success);
        response.Properties.Should().BeSameAs(properties);
        response.Error.Should().BeNull();
        response.Message.Should().Be(message);
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

        response.Type.Should().Be(type);
        response.ApiVersion.Should().Be(apiVersion);
        response.Status.Should().Be(HandlerResponseStatus.Error);
        response.Properties.Should().BeSameAs(properties);
        response.Error.Should().BeSameAs(error);
        response.Message.Should().Be(message);
    }

    [TestMethod]
    public void Canceled_Method_Assigns_Expected_Values()
    {
        var type = "MyType";
        var apiVersion = "2024-01-01";
        var properties = new JsonObject { ["foo"] = "bar" };
        var message = "canceled";

        var response = HandlerResponse.Canceled(type, apiVersion, properties, message);

        response.Type.Should().Be(type);
        response.ApiVersion.Should().Be(apiVersion);
        response.Status.Should().Be(HandlerResponseStatus.Canceled);
        response.Properties.Should().BeSameAs(properties);
        response.Error.Should().BeNull();
        response.Message.Should().Be(message);
    }

    [TestMethod]
    public void TimedOut_Method_Assigns_Expected_Values()
    {
        var type = "MyType";
        var apiVersion = "2024-01-01";
        var properties = new JsonObject { ["foo"] = "bar" };
        var message = "timeout";

        var response = HandlerResponse.TimedOut(type, apiVersion, properties, message);

        response.Type.Should().Be(type);
        response.ApiVersion.Should().Be(apiVersion);
        response.Status.Should().Be(HandlerResponseStatus.TimedOut);
        response.Properties.Should().BeSameAs(properties);
        response.Error.Should().BeNull();
        response.Message.Should().Be(message);
    }

    [TestMethod]
    public void Error_Object_Has_Expected_Content()
    {
        var error = new Error("E001", "targetField", "Something went wrong");
        var response = new HandlerResponse("MyType", "2024-01-01", HandlerResponseStatus.Error, new JsonObject(), error);

        response.Error.Should().NotBeNull();
        response.Error!.Code.Should().Be("E001");
        response.Error.Target.Should().Be("targetField");
        response.Error.Message.Should().Be("Something went wrong");
    }

    [TestMethod]
    public void Static_Methods_Throw_When_Type_Is_Null_Or_Whitespace()
    {
        Action act1 = () => HandlerResponse.Success(null!, "v", null);
        Action act2 = () => HandlerResponse.Success("", "v", null);
        Action act3 = () => HandlerResponse.Success("   ", "v", null);

        Action act4 = () => HandlerResponse.Failed(null!, "v", null);
        Action act5 = () => HandlerResponse.Canceled(null!, "v", null);
        Action act6 = () => HandlerResponse.TimedOut(null!, "v", null);

        act1.Should().Throw<ArgumentNullException>();
        act2.Should().Throw<ArgumentNullException>();
        act3.Should().Throw<ArgumentNullException>();
        act4.Should().Throw<ArgumentNullException>();
        act5.Should().Throw<ArgumentNullException>();
        act6.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void Properties_Object_Is_Not_Shared_Between_Instances()
    {
        var r1 = new HandlerResponse("MyType", "v", HandlerResponseStatus.Success, null);
        var r2 = new HandlerResponse("MyType", "v", HandlerResponseStatus.Success, null);

        r1.Properties["foo"] = 1;
        r2.Properties.ContainsKey("foo").Should().BeFalse();
    }

    [TestMethod]
    public void ExtensionSettings_Is_Always_Null()
    {
        var response = new HandlerResponse("MyType", "v", HandlerResponseStatus.Success, null);
        response.ExtensionSettings.Should().BeNull();
    }
}
