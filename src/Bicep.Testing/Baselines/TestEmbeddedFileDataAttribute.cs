// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Reflection;
using System.Text.RegularExpressions;
using Bicep.Testing.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Testing.Baselines;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class TestEmbeddedFileDataAttribute(string regexFilter) : Attribute, ITestDataSource
{
    public string RegexFilter { get; } = regexFilter;

    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
    {
        var files = EmbeddedFile.LoadAll(methodInfo.DeclaringType!.Assembly, new Regex(RegexFilter));

        methodInfo.GetCustomAttributes().OfType<TestCategoryAttribute>()
            .Should().Contain(
                x => x.TestCategories.Contains(TestCategories.Baseline),
                $"Expected test method to have the {TestCategories.Baseline} category");
        files.Should().NotBeEmpty($"Expected filter {RegexFilter} to match at least 1 file");

        return files.Select(x => new object[] { x });
    }

    public string? GetDisplayName(MethodInfo methodInfo, object?[]? data)
    {
        var file = (data?[0] as EmbeddedFile)!;

        return $"{methodInfo.Name} ({file.StreamPath})";
    }
}
