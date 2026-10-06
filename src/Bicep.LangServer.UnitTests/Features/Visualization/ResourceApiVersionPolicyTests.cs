// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.LanguageServer.Features.Custom.Visualization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.UnitTests.Features.Visualization;

[TestClass]
public class ResourceApiVersionPolicyTests
{
    [DataTestMethod]
    [DataRow("2021-01-01-preview", true)]
    [DataRow("2021-01-01", false)]
    [DataRow("not-a-version", false)]
    public void IsPreview_RecognizesPreviewVersions(string apiVersion, bool expected)
    {
        ResourceApiVersionPolicy.IsPreview(apiVersion).Should().Be(expected);
    }

    [TestMethod]
    public void SelectDefault_PrefersStableOverNewerPreview()
    {
        ResourceApiVersionPolicy.SelectDefault(["2020-01-01", "2023-01-01-preview", "2021-01-01"])
            .Should().Be("2021-01-01");
    }

    [TestMethod]
    public void SelectDefault_PreviewOnly_ReturnsNewestPreview()
    {
        string?[] apiVersions = ["2022-01-01-preview", "2023-01-01-preview", null];

        ResourceApiVersionPolicy.SelectDefault(apiVersions).Should().Be("2023-01-01-preview");
    }

    [TestMethod]
    public void SortNewestFirst_RemovesDuplicatesAndIncludesPreviews()
    {
        string?[] apiVersions = ["2020-01-01", "2021-01-01-preview", "2021-01-01", "2020-01-01", null];

        ResourceApiVersionPolicy.SortNewestFirst(apiVersions).Should().Equal("2021-01-01", "2021-01-01-preview", "2020-01-01");
    }
}
