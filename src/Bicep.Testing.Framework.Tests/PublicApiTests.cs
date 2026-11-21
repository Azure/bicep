// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using PublicApiGenerator;

namespace Bicep.Testing.Framework.Tests;

[TestClass]
public class PublicApiTests
{
    public TestContext TestContext { get; set; } = null!;

    [TestMethod]
    [TestCategory(BaselineHelper.BaselineTestCategory)]
    [EmbeddedFilesTestData(@"^Files\/PublicApis\/Azure.Bicep.Testing.Framework.txt$")]
    public void PublicApi_should_be_up_to_date(EmbeddedFile publicApiFile)
    {
        var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, publicApiFile);
        var result = baselineFolder.GetFileOrEnsureCheckedIn(publicApiFile.FileName);

        var publicApi = typeof(BicepTestHelper).Assembly.GeneratePublicApi();
        publicApi = publicApi.Replace("\r\n", "\n");

        result.WriteToOutputFolder(publicApi);
        result.ShouldHaveExpectedValue();
    }
}
