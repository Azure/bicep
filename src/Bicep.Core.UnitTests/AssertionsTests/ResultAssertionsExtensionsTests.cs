// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Utils;
using Bicep.Core.UnitTests.Assertions;
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
    public void ShouldBeFailure_WithError()
    {
        var result = new Result<string, int>(404);
        result.Should().BeFailure(404, "Simon didn't say");
    }

    [TestMethod]
    public void ShouldBeFailure_ButIsSuccess()
    {
        var result = new Result<string, int>("my success");
        var func = () => result.Should().BeFailure("Simon didn't say");
        func.Should().Throw<AssertFailedException>().WithMessage("Expected result to be a failure because Simon didn't say, but it was a success with value \"my success\"");
    }

    [TestMethod]
    public void ShouldBeFailure_WithError_ButIsSuccess()
    {
        var result = new Result<string, int>("my success");
        var func = () => result.Should().BeFailure(501, "Simon didn't say");
        func.Should().Throw<AssertFailedException>().WithMessage("Expected result to be a failure with value 501 because Simon didn't say, but it was a success with value \"my success\"");
    }

    [TestMethod]
    public void ShouldBeFailure_WithWrongError()
    {
        var result = new Result<string, int>(404);
        var func = () => result.Should().BeFailure(501, "Simon didn't say");
        func.Should().Throw<AssertFailedException>().WithMessage("Expected result to be a failure with value 501 because Simon didn't say, but the failure had value 404");
    }

}
