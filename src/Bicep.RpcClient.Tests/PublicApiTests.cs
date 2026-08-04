// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Testing;
using Bicep.Testing.Baselines;
using FluentAssertions;
using PublicApiGenerator;

namespace Bicep.RpcClient.Tests;

[TestClass]
public class PublicApiTests
{
    public TestContext TestContext { get; set; } = null!;

    [TestMethod]
    [TestCategory(TestCategories.Baseline)]
    [TestEmbeddedFileData(@"^Files\/PublicApis\/Azure.Bicep.RpcClient.txt$")]
    public void PublicApi_should_be_up_to_date(TestEmbeddedFile publicApiFile)
    {
        var baselineFiles = TestContext.MaterializeBaseline(publicApiFile);
        var result = baselineFiles.GetFile(publicApiFile.FileName);

        var publicApi = typeof(BicepClientConfiguration).Assembly.GeneratePublicApi();

        publicApi = publicApi.Replace("\r\n", "\n");

        publicApi.Should().MatchTextBaseline(result);
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
