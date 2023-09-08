// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


using System;
using System.Dynamic;
using Bicep.Core.Diagnostics;
using Bicep.Core.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Utils;

[TestClass]
public class ResultTests
{
    [TestMethod]
    public void Result_should_encapsulate_successful_result()
    {
        var test = new Result<string, Uri>("success!");

        test.IsSuccess().Should().BeTrue();
        test.TryUnwrap().Should().Be("success!");
        test.Unwrap().Should().Be("success!");
        test.IsSuccess(out var success, out var error).Should().Be(true);
        success.Should().Be("success!");
        error.Should().BeNull();
    }

    [TestMethod]
    public void Result_should_encapsulate_error_result()
    {
        var test = new Result<Uri, string>("error!");

        test.IsSuccess().Should().BeFalse();
        test.TryUnwrap().Should().BeNull();
        FluentActions.Invoking(() => test.Unwrap()).Should().Throw<InvalidOperationException>()
            .Which.Message.Should().Be("Cannot unwrap a failed result.");
        test.IsSuccess(out var success, out var error).Should().Be(false);
        success.Should().BeNull();
        error.Should().Be("error!");
    }
}