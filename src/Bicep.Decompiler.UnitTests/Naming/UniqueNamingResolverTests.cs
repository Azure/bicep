// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Expression.Serializers;
using Bicep.Decompiler.ArmHelpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Decompiler.UnitTests.Naming
{
    [TestClass]
    public class UniqueNamingResolverTests
    {
        [DataTestMethod]
        [DataRow("testName", "test")]
        [DataRow("testRename", "testRename")]
        [DataRow("testName2", "test2")]
        [DataRow("Name", "Name")]
        [DataRow("Name2", "Name2")]
        [DataRow("AName2", "A2")]
        public void RequestResourceName_truncates_when_ending_with_Name(string input, string expectedOutput)
        {
            var resolver = new UniqueNamingResolver();
            var output = resolver.TryRequestResourceName(input, ExpressionHelpers.ParseExpression(input));

            output.Should().NotBeNull();
            output.Should().Be(expectedOutput);
        }

    }
}
