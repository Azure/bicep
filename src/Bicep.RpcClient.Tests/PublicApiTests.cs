// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using FluentAssertions;
using PublicApiGenerator;

namespace Bicep.RpcClient.Tests;

[TestClass]
public class PublicApiTests
{
    public TestContext TestContext { get; set; } = null!;

    [TestMethod]
    [TestCategory(BaselineHelper.BaselineTestCategory)]
    [EmbeddedFilesTestData(@"^Files\/PublicApis\/Azure.Bicep.RpcClient.txt$")]
    public void PublicApi_should_be_up_to_date(EmbeddedFile publicApiFile)
    {
        // This test just asserts that the public API surface of the assembly as defined in Azure.Bicep.RpcClient.txt is up to date.
        // This ensures that any changes to the public API are reviewed.
        var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, publicApiFile);
        var result = baselineFolder.GetFileOrEnsureCheckedIn(publicApiFile.FileName);

        var publicApi = typeof(BicepClientConfiguration).Assembly.GeneratePublicApi();

        result.WriteToOutputFolder(publicApi);
        result.ShouldHaveExpectedValue();
    }

    [TestMethod]
    public void Dependencies_should_be_minimal()
    {
        var referencedAssemblies = typeof(BicepClientConfiguration).Assembly
            .GetReferencedAssemblies()
            .OrderBy(x => x.Name)
            .Select(x => x.Name);

        referencedAssemblies.Except(["netstandard"]).Should().BeEquivalentTo([
            // Be careful when adding new dependencies to the ClientTools assembly - this assembly is intentionally slim.
            // The assembly is used in Microsoft internal tools, where dependency management is complex, so we want to avoid transitively depending on ResourceStack
            "System.Collections.Immutable",
            "System.IO.Pipelines",
            "System.Memory",
            "System.Text.Encodings.Web",
            "System.Text.Json",
            "System.Threading.Tasks.Extensions"
        ]);
    }
}
