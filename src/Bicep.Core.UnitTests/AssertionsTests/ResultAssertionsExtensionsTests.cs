// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.AssertionsTests;

[TestClass]
public class ResultAssertionsExtensionsTests
{
    [TestMethod]
    public void ShouldBeFailure()
    {
        var result = new Result<string, int>(404);
        result.Should().BeFailure("Simon didn't say");
    }

    [TestMethod]
    public void ShouldBeFailureWithValue()
    {
        var result = new Result<string, int>(404);
        result.Should().BeFailureWithValue(404, "Simon didn't say");
    }

    [TestMethod]
    public void ShouldBeFailure_ButIsSuccess()
    {
        var result = new Result<string, int>("my success");
        var func = () => result.Should().BeFailure("Simon didn't say");
        func.Should().Throw<AssertFailedException>().WithMessage("Expected result to be a failure because Simon didn't say, but it was a success with value \"my success\"");
    }

    [TestMethod]
    public void ShouldBeFailureWithValue_ButIsSuccess()
    {
        var result = new Result<string, int>("my success");
        var func = () => result.Should().BeFailureWithValue(501, "Simon didn't say");
        func.Should().Throw<AssertFailedException>().WithMessage("Expected result to be a failure with value 501 because Simon didn't say, but it was a success with value \"my success\"");
    }

    [TestMethod]
    public void ShouldBeFailureWithValue_WithWrongValue()
    {
        var result = new Result<string, int>(404);
        var func = () => result.Should().BeFailureWithValue(501, "Simon didn't say");
        func.Should().Throw<AssertFailedException>().WithMessage("Expected result to be a failure with value 501 because Simon didn't say, but the failure had value 404");
    }

    [TestMethod]
    public void ShouldBeSuccess()
    {
        var result = new Result<string, int>("my success");
        result.Should().BeSuccess("Simon said");
    }

    [TestMethod]
    public void ShouldBeSuccessWithValue()
    {
        var result = new Result<string, int>("my success");
        result.Should().BeSuccessWithValue("my success", "Simon said");
    }

    [TestMethod]
    public void ShouldBeSuccess_ButIsFailure()
    {
        var result = new Result<string, int>(404);
        var func = () => result.Should().BeSuccess("Simon said");
        func.Should().Throw<AssertFailedException>().WithMessage("Expected result to be a success because Simon said, but it was a failure with value 404");
    }

    [TestMethod]
    public void ShouldBeSuccessWithValue_ButIsFailure()
    {
        var result = new Result<string, int>(404);
        var func = () => result.Should().BeSuccessWithValue("my success", "Simon said");
        func.Should().Throw<AssertFailedException>().WithMessage("Expected result to be a success with value \"my success\" because Simon said, but it was a failure with value 404");
    }

    [TestMethod]
    public void ShouldBeSuccessWithValue_WithWrongValue()
    {
        var result = new Result<string, int>("your success");
        var func = () => result.Should().BeSuccessWithValue("my success", "Red Rover should come over");
        func.Should().Throw<AssertFailedException>().WithMessage("Expected result to be a success with value \"my success\" because Red Rover should come over, but the actual value was \"your success\"");
    }
}
