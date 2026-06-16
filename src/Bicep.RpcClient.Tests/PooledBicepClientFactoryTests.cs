// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.InteropServices;
using Bicep.Core.UnitTests.Utils;
using Bicep.RpcClient.Models;
using FluentAssertions;

namespace Bicep.RpcClient.Tests;

[TestClass]
public class PooledBicepClientFactoryTests
{
    public TestContext TestContext { get; set; } = null!;

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(TestContext.TestResultsDirectory))
        {
            Directory.Delete(TestContext.TestResultsDirectory, true);
        }
    }

    [TestMethod]
    public async Task Disposing_one_wrapper_does_not_dispose_shared_client()
    {
        using var factory = new PooledBicepClientFactory();
        var config = new BicepClientConfiguration { ExistingCliPath = GetCliPath() };

        var wrapperA = await factory.Initialize(config, TestContext.CancellationTokenSource.Token);
        var wrapperB = await factory.Initialize(config, TestContext.CancellationTokenSource.Token);

        wrapperA.Dispose();

        var version = await wrapperB.GetVersion(TestContext.CancellationTokenSource.Token);

        version.Should().MatchRegex(@"^\d+\.\d+\.\d+(-.+)?$");

        wrapperB.Dispose();
    }

    [TestMethod]
    public async Task Initialize_throws_after_factory_disposed()
    {
        var factory = new PooledBicepClientFactory();
        factory.Dispose();

        await FluentActions.Invoking(() =>
                factory.Initialize(new BicepClientConfiguration { ExistingCliPath = GetCliPath() }, TestContext.CancellationTokenSource.Token))
            .Should().ThrowAsync<ObjectDisposedException>();
    }

    [TestMethod]
    public async Task Concurrent_requests_succeed_with_multiple_wrappers()
    {
        using var factory = new PooledBicepClientFactory();
        var config = new BicepClientConfiguration { ExistingCliPath = GetCliPath() };

        var wrappers = await Task.WhenAll(Enumerable.Range(0, 4)
            .Select(_ => factory.Initialize(config, TestContext.CancellationTokenSource.Token)));

        try
        {
            var bicepFile = FileHelper.SaveResultFile(TestContext, "main.bicep", "param location string");

            var results = await Task.WhenAll(wrappers.Select(wrapper =>
                wrapper.Compile(new CompileRequest(bicepFile), TestContext.CancellationTokenSource.Token)));

            results.Should().OnlyContain(result => result.Success);
        }
        finally
        {
            foreach (var wrapper in wrappers)
            {
                wrapper.Dispose();
            }
        }
    }

    private static string GetCliPath()
    {
        var cliName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "bicep.exe" : "bicep";
        return Path.GetFullPath(Path.Combine(typeof(PooledBicepClientFactoryTests).Assembly.Location, $"../{cliName}"));
    }
}
