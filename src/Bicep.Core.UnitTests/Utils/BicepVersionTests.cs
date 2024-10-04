// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Utils;

[TestClass]
public class BicepVersionTests
{
    [TestMethod]
    public void BicepVersion_should_return_assembly_info()
    {
        var result = BicepVersion.Instance;
        result.Value.Should().MatchRegex(@"^[0-9]+\.[0-9]+\.[0-9]+");
        if (result.CommitHash is {})
        {
            result.CommitHash.Should().MatchRegex(@"^[0-9a-f]{7,40}$");
        }
    }
}