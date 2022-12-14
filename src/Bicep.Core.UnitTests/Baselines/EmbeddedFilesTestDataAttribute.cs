// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using FluentAssertions;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bicep.Core.UnitTests.Assertions;

namespace Bicep.Core.UnitTests.Baselines;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class EmbeddedFilesTestDataAttribute : Attribute, ITestDataSource
{
    public EmbeddedFilesTestDataAttribute(string regexFilter)
    {
        RegexFilter = regexFilter;
    }

    public string RegexFilter { get; }

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
    {
        var files = EmbeddedFile.LoadAll(methodInfo.DeclaringType!.Assembly, new Regex(RegexFilter));

        var testCategories = methodInfo.GetCustomAttributes().OfType<TestCategoryAttribute>()
            .Should().Contain(
                x => x.TestCategories.Contains(BaselineHelper.BaselineTestCategory),
                $"Expected test method to have the {BaselineHelper.BaselineTestCategory} category");
        files.Should().NotBeEmpty($"Expected filter {RegexFilter} to match at least 1 file");

        return files.Select(x => new object[] { x });
    }

    public string GetDisplayName(MethodInfo methodInfo, object[] data)
    {
        var file = (data[0] as EmbeddedFile)!;

        return $"{methodInfo.Name}({file.StreamPath})";
    }
}
