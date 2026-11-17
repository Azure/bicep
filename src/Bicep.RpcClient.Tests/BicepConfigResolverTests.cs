// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;

namespace Bicep.RpcClient.Tests;

[TestClass]
public class BicepConfigResolverTests
{
    public TestContext TestContext { get; set; } = null!;

    private string Root => FileHelper.GetTestOutputRootPath(TestContext);

    private CancellationToken Token => TestContext.CancellationTokenSource.Token;

    private string WriteConfig(string relativePath, string content)
    {
        var path = Path.Combine(Root, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);
        return path;
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(Root))
        {
            Directory.Delete(Root, true);
        }
    }

    [TestMethod]
    public async Task ResolveConfigChain_returns_no_constraint_when_no_config_is_found()
    {
        var result = await BicepConfigResolver.ResolveConfigChain(Path.Combine(Root, "main.bicep"), Token);

        result.BicepVersionRange.Should().BeNull();
        result.DeclaringConfigPath.Should().BeNull();
    }

    [TestMethod]
    [DataRow("main.bicep")]
    [DataRow("nested/main.bicepparam")]
    public async Task ResolveConfigChain_discovers_config_above_source_and_accepts_jsonc(string source)
    {
        var config = WriteConfig("bicepconfig.json", """
            {
                // A project version constraint.
                "bicep": { "version": ">=0.40, <1", },
            }
            """);

        var result = await BicepConfigResolver.ResolveConfigChain(Path.Combine(Root, source), Token);

        result.BicepVersionRange!.ToString().Should().Be(">=0.40.0, <1.0.0");
        result.DeclaringConfigPath.Should().Be(config);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task ResolveConfigChain_uses_nearest_declaring_layer(bool overrideVersion)
    {
        var parent = WriteConfig("shared/base.json", """{"bicep":{"version":"0.40.1"}}""");
        WriteConfig("project/intermediate.json", """{"extends":"../shared/base.json"}""");
        var leaf = WriteConfig("project/bicepconfig.json", overrideVersion
            ? """{"extends":"intermediate.json","bicep":{"version":"0.41.0"}}"""
            : """{"extends":"intermediate.json"}""");

        var result = await BicepConfigResolver.ResolveConfigChain(Path.Combine(Root, "project/nested/main.bicep"), Token);

        result.BicepVersionRange!.ToString().Should().Be(overrideVersion ? "0.41.0" : "0.40.1");
        result.DeclaringConfigPath.Should().Be(overrideVersion ? leaf : parent);
    }

    [TestMethod]
    public async Task ResolveConfigChain_does_not_merge_unextended_ancestor_config()
    {
        WriteConfig("bicepconfig.json", """{"bicep":{"version":"0.40.1"}}""");
        WriteConfig("nested/bicepconfig.json", "{}");

        var result = await BicepConfigResolver.ResolveConfigChain(Path.Combine(Root, "nested/main.bicep"), Token);

        result.BicepVersionRange.Should().BeNull();
    }

    [TestMethod]
    [DataRow("""{"bicep":{"version":""}}""", "*not a valid Bicep version*")]
    [DataRow("""{"bicep":{"version":"^1.2.3"}}""", "*not a valid Bicep version*")]
    [DataRow("""{"bicep":{"version":">=2, <1"}}""", "*can never be satisfied*")]
    [DataRow("""{"bicep":{"version":">1.2.3, <1.2.4"}}""", "*can never be satisfied*")]
    public async Task ResolveConfigChain_rejects_invalid_properties(string json, string message)
    {
        WriteConfig("bicepconfig.json", json);

        var act = () => BicepConfigResolver.ResolveConfigChain(Path.Combine(Root, "main.bicep"), Token);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage(message);
    }

    [TestMethod]
    [DataRow("{")]
    [DataRow("[]")]
    [DataRow("null")]
    [DataRow("""{"bicep":{"version":42}}""")]
    [DataRow("""{"extends":42}""")]
    [DataRow("""{"bicep":42}""")]
    public async Task ResolveConfigChain_rejects_invalid_json_with_config_path(string json)
    {
        var config = WriteConfig("bicepconfig.json", json);

        var act = () => BicepConfigResolver.ResolveConfigChain(Path.Combine(Root, "main.bicep"), Token);

        await act.Should().ThrowAsync<JsonException>().WithMessage($"*{config}*");
    }

    [TestMethod]
    [DataRow("""{"extends":"base.json","bicep":null}""")]
    [DataRow("""{"extends":"base.json","bicep":{"version":null}}""")]
    [DataRow("""{"extends":"base.json","bicep":{}}""")]
    public async Task ResolveConfigChain_null_or_missing_version_inherits_from_base(string json)
    {
        WriteConfig("bicepconfig.json", json);
        var parent = WriteConfig("base.json", """{"bicep":{"version":"0.40.1"}}""");

        var result = await BicepConfigResolver.ResolveConfigChain(Path.Combine(Root, "main.bicep"), Token);

        result.BicepVersionRange!.ToString().Should().Be("0.40.1");
        result.DeclaringConfigPath.Should().Be(parent);
    }

    [TestMethod]
    public async Task ResolveConfigChain_ignores_unrelated_settings()
    {
        WriteConfig("bicepconfig.json", """
            {
                "extends": null,
                "analyzers": { "core": { "enabled": true } },
                "bicep": { "version": "0.40.1", "otherSetting": [1, 2] }
            }
            """);

        var result = await BicepConfigResolver.ResolveConfigChain(Path.Combine(Root, "main.bicep"), Token);

        result.BicepVersionRange!.ToString().Should().Be("0.40.1");
    }

    [TestMethod]
    public async Task ResolveConfigChain_rejects_missing_extended_file()
    {
        WriteConfig("bicepconfig.json", """{"extends":"missing.json"}""");

        var act = () => BicepConfigResolver.ResolveConfigChain(Path.Combine(Root, "main.bicep"), Token);

        await act.Should().ThrowAsync<FileNotFoundException>();
    }

    [TestMethod]
    public async Task ResolveConfigChain_rejects_absolute_extends_path()
    {
        WriteConfig("bicepconfig.json", JsonSerializer.Serialize(new { extends = Path.Combine(Root, "base.json") }));

        var act = () => BicepConfigResolver.ResolveConfigChain(Path.Combine(Root, "main.bicep"), Token);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*must be a relative path*");
    }

    [TestMethod]
    public async Task ResolveConfigChain_validates_full_chain_even_after_finding_version()
    {
        WriteConfig("bicepconfig.json", """{"extends":"base.json","bicep":{"version":"1.2.3"}}""");
        WriteConfig("base.json", """{"extends":"./nested/../bicepconfig.json"}""");

        var act = () => BicepConfigResolver.ResolveConfigChain(Path.Combine(Root, "main.bicep"), Token);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Cycle detected*");
    }

    [TestMethod]
    [DataRow(64)]
    [DataRow(65)]
    public async Task ResolveConfigChain_enforces_depth_limit(int layers)
    {
        WriteConfig("bicepconfig.json", """{"extends":"1.json"}""");
        for (var i = 1; i < layers; i++)
        {
            WriteConfig($"{i}.json", i == layers - 1
                ? """{"bicep":{"version":"1.2.3"}}"""
                : JsonSerializer.Serialize(new { extends = $"{i + 1}.json" }));
        }

        var act = () => BicepConfigResolver.ResolveConfigChain(Path.Combine(Root, "main.bicep"), Token);
        if (layers == 64)
        {
            (await act()).BicepVersionRange!.ToString().Should().Be("1.2.3");
        }
        else
        {
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*maximum supported depth of 64*");
        }
    }

    [TestMethod]
    public async Task ResolveConfigChain_honors_cancellation_even_without_config()
    {
        var act = () => BicepConfigResolver.ResolveConfigChain(Path.Combine(Root, "main.bicep"), new(canceled: true));

        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
