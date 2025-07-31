// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Extensions
{
    [TestClass]
    public class StringExtensionsTests
    {
        [DataRow(
            "hello there",
            "(?<one>\\w+)",
            new string[] { "one" },
            new string[] { "hello" })]
        [DataRow(
            "br:mockregistry.io/test/module1:v1",
            "^br:(?<registry>.+?)/(?<repo>.+?)[:@](?<tag>.+?)$",
            new string[] { "registry", },
            new string[] { "mockregistry.io" })]
        [DataRow(
            "br:mockregistry.io/test/module1:v1",
            "^br:(?<registry>.+?)/(?<repo>.+?)[:@](?<tag>.+?)$",
            new string[] { "registry", "repo", "tag" },
            new string[] { "mockregistry.io", "test/module1", "v1" })]
        [DataRow(
            "br:mockregistry.io/test/module1:v1",
            "^br:(?<registry>.+?)/(?<repo>.+?)[:@](?<tag>.+?)$",
            new string[] { "repo", "registry", "tag" },
            new string[] { "test/module1", "mockregistry.io", "v1" })]
        [DataTestMethod]
        public void ExtractRegexGroups_ByGroupNames_Valid(string s, string regex, string[] groupNames, string[] expected)
        {
            var result = s.ExtractRegexGroups(regex, groupNames);
            CollectionAssert.AreEqual(expected, result);
        }

        [DataRow(
            "hello there",
            "(?<one>:\\w)",
            new string[] { "one" },
            "No matches were found for regex (?<one>:\\w) in string \"hello there\"")]
        [DataTestMethod]
        public void ExtractRegexGroups_ByGroupNames_Invalid(string s, string regex, string[] groupNames, string expectedError)
        {
            var ex = Assert.ThrowsException<Exception>(() => s.ExtractRegexGroups(regex, groupNames));
            ex.Message.Should().Be(nameof(StringExtensions.ExtractRegexGroups) + ": " + expectedError);
        }

        [DataRow(
            "hello there",
            "(\\w+)",
            new string[] { "hello" })]
        [DataRow(
            "hello there",
            "(?<one>\\w+)",
            new string[] { "hello" })]
        [DataRow(
            "br:mockregistry.io/test/module1:v1",
            "^br:(.+?)/(?<repo>.+?)[:@](?:.+?)$", // last is non-capturing group, it will be ignored
            new string[] { "mockregistry.io", "test/module1" })]
        [DataRow(
            "br:mockregistry.io/test/module1:v1",
            "^br:(?<registry>.+?)/(?<repo>.+?)[:@](?<tag>.+?)$",
            new string[] { "mockregistry.io", "test/module1", "v1" })]
        [DataTestMethod]
        public void ExtractRegexGroups_AllGroups_Valid(string s, string regex, string[] expected)
        {
            var result = s.ExtractRegexGroups(regex);
            CollectionAssert.AreEqual(expected, result);
        }

        [DataRow(
            "123456789",
            "(?<one>:[a-z]+)",
            "No matches were found for regex (?<one>:[a-z]+) in string \"123456789\"")]
        [DataRow(
            "123456789",
            "([a-z]+)",
            "No matches were found for regex ([a-z]+) in string \"123456789\"")]
        [DataRow(
            "123456789",
            "[0-9]+",
            "No groups were found in regex [0-9]+")]
        [DataTestMethod]
        public void ExtractRegexGroups_AllGroups_Invalid(string s, string regex, string expectedError)
        {
            var ex = Assert.ThrowsException<Exception>(() => s.ExtractRegexGroups(regex));
            ex.Message.Should().Be(nameof(StringExtensions.ExtractRegexGroups) + ": " + expectedError);
        }
    }
}
