// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Azure;
using Azure.Containers.ContainerRegistry;
using Azure.Identity;
using Bicep.Cli.UnitTests;
using Bicep.Cli.UnitTests.Assertions;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Configuration;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Registry.PublicRegistry;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using Bicep.Core.UnitTests.Mock.Registry;
using Bicep.Core.UnitTests.Registry;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using static Bicep.Core.UnitTests.Utils.RegistryHelper;

namespace Bicep.Cli.IntegrationTests;

[TestClass]
public class UseRecentModuleVersionsIntegrationTests : TestBase
{
    private const string PREFIX = "br:mcr.microsoft.com/bicep";
    private const string NotRestoredErrorCode = "BCP190";
    private const string UnableToRestoreErrorCode = "BCP192";

    private string CacheRoot => Path.Join(FileHelper.GetUniqueTestOutputPath(TestContext), "cacheRoot");

    private class Options(string CacheRoot)
    {
        private IPublicModuleIndexClient? _metadataClient = null;
        private string? _config = null;

        public string Bicep { get; init; } = "/* bicep contents */";
        public (string module, string[] versions)[] ModulesMetadata { get; init; } = [];
        public string[] PublishedModules { get; init; } = [];
        public bool NoRestore { get; init; } = false;

        public string DiagnosticLevel { get; init; } = "Warning"; // This rule normally defaults to "off"

        public string? BicepConfig
        {
            set { _config = value; }
            get
            {
                return _config ?? """
                  {
                    "cacheRootDirectory": "{CACHEROOT}",
                    "analyzers": {
                      "core": {
                        "rules": {
                          "use-recent-module-versions": {
                            "level": "{DiagnosticLevel}"
                          }
                        }
                      }
                    }
                  }
                  """.Replace("{DiagnosticLevel}", this.DiagnosticLevel)
                  .Replace("{CACHEROOT}", CacheRoot.Replace("\\", "\\\\"));
            }
        }

        // Automatically created from ModulesMetadata by default (set manually for testing)
        internal IPublicModuleIndexClient MetadataClient
        {
            set
            {
                _metadataClient = value;
            }
            get => _metadataClient is { } ? _metadataClient : PublicModuleIndexHttpClientMocks.Create(
                ModulesMetadata.Select(mm => new PublicModuleIndexEntry(
                    mm.module,
                    [.. mm.versions],
                    new Dictionary<string, PublicModuleProperties>().ToImmutableDictionary()))).Object;
        }

    }

    private async Task<CliResult> Test(Options options)
    {
        string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);

        // compile and publish modules using throwaway file system
        var clientFactory = await RegistryHelper.CreateMockRegistryClientWithPublishedModulesAsync(
            new MockFileSystem(),
            [.. options.PublishedModules.Select(x => new ModuleToPublish(x, BicepSource: "", WithSource: true))]);

        // create files
        var mainFile = FileHelper.SaveResultFile(TestContext, "main.bicep", options.Bicep, testOutputPath);
        var configurationPath = options.BicepConfig is null ? null : FileHelper.SaveResultFile(TestContext, "bicepconfig.json", options.BicepConfig, testOutputPath);

        // act
        var settings = new InvocationSettings()
        {
            ModuleMetadataClient = options.MetadataClient,
            ClientFactory = clientFactory
        };
        return await Bicep(settings, "lint", mainFile, options.NoRestore ? "--no-restore" : null);
    }

    [TestMethod]
    public async Task IfLevelIsOff_ShouldNotDownloadModuleMetadata()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 '{PREFIX}/fake/avm/res/app/container-app:0.2.0' = {
                  name: 'm1'
                }
                """.Replace("{PREFIX}", PREFIX),
            DiagnosticLevel = "off",
            PublishedModules = [$"{PREFIX}/fake/avm/res/app/container-app:0.2.0"],
            MetadataClient = PublicModuleIndexHttpClientMocks.CreateToThrow(new Exception("unit test failed: shouldn't try to download in this scenario")).Object,
        });

        result.Should().NotHaveStderr();
        result.Should().HaveStdout("");
        result.Should().Succeed();
    }

    [TestMethod]
    // We don't currently cache to disk, but rather on every check to restore modules.
    public async Task IfNoRestoreSpecified_ThenShouldNotDownloadMetadata_AndShouldFailBecauseNoCache()
    {
        var moduleIndexClientMock = PublicModuleIndexHttpClientMocks.CreateToThrow(new Exception("shouldn't try to download metadata --no-restore is set"));
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 '{PREFIX}/fake/avm/res/app/container-app:0.2.0' = {
                  name: 'm1'
                }
                """.Replace("{PREFIX}", PREFIX),
            PublishedModules = [$"{PREFIX}/fake/avm/res/app/container-app:0.2.0"],
            ModulesMetadata = [("fake/avm/res/app/container-app", ["0.2.0"])],
            MetadataClient = moduleIndexClientMock.Object,
            NoRestore = true,
        });

        result.Should().HaveStderrMatch($"*Error {NotRestoredErrorCode}: The artifact with reference \"br:mcr.microsoft.com/bicep/fake/avm/res/app/container-app:0.2.0\" has not been restored.*");
        result.Should().HaveStderrMatch("*Warning use-recent-module-versions: Available module versions have not yet been downloaded. If running from the command line, be sure --no-restore is not specified.*");
        result.Should().HaveStdout("");
        result.Should().Fail();

        moduleIndexClientMock.Verify(client => client.GetModuleIndexAsync(), Times.Never, "shouldn't try to download metadata --no-restore is set");
    }

    [TestMethod]
    // We don't currently cache to disk, but rather on every check to restore modules.
    public async Task IfMetadataDownloadFails_ThenShouldFail()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 '{PREFIX}/fake/avm/res/app/container-app:0.2.0' = {
                  name: 'm1'
                }
                """.Replace("{PREFIX}", PREFIX),
            PublishedModules = [$"{PREFIX}/fake/avm/res/app/container-app:0.2.0"],
            MetadataClient = PublicModuleIndexHttpClientMocks.CreateToThrow(new Exception("Download failed.")).Object,
        });

        result.Should().HaveStderrMatch($"*Warning use-recent-module-versions: Could not download available module versions: Download failed.*");
        result.Should().HaveStdout("");
        result.Should().Succeed();
    }

    [TestMethod]
    public async Task SimpleFailure()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 '{PREFIX}/fake/avm/res/app/container-app:0.2.0' = {
                  name: 'm1'
                }
                """.Replace("{PREFIX}", PREFIX),
            PublishedModules = [
                $"{PREFIX}/fake/avm/res/app/container-app:0.2.0",
                $"{PREFIX}/fake/avm/res/app/container-app:0.3.0"
            ],
            ModulesMetadata = [("fake/avm/res/app/container-app", ["0.2.0", "0.3.0"])],
        });

        result.Should().HaveStderrMatch($"*Warning use-recent-module-versions: Use a more recent version of module 'fake/avm/res/app/container-app'. The most recent version is 0.3.0.*");
        result.Should().HaveStdout("");
        result.Should().Succeed(); // only a warning
    }

    [TestMethod]
    public async Task IfDoesntMatchCase_ThenShouldIgnore()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 '{PREFIX}/fake/avm/res/app/container-app:0.2.0' = {
                  name: 'm1'
                }
                """.Replace("{PREFIX}", PREFIX),
            PublishedModules = [
                $"{PREFIX}/fake/avm/res/app/container-app:0.2.0",
                $"{PREFIX}/fake/avm/res/app/container-app:0.3.0"
            ],
            ModulesMetadata = [("fake/avm/res/app/CONTAINER-app", ["0.2.0", "0.3.0"])],
        });

        result.Should().NotHaveStderr();
        result.Should().HaveStdout("");
        result.Should().Succeed(); // only a warning
    }

    [TestMethod]
    public async Task IfLevelIsError_ShouldShowFailuresAsErrors()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 '{PREFIX}/fake/avm/res/app/container-app:0.2.0' = {
                  name: 'm1'
                }
                """.Replace("{PREFIX}", PREFIX),
            DiagnosticLevel = "Error",
            PublishedModules = [
                $"{PREFIX}/fake/avm/res/app/container-app:0.2.0",
                $"{PREFIX}/fake/avm/res/app/container-app:0.3.0"
            ],
            ModulesMetadata = [("fake/avm/res/app/container-app", ["0.2.0", "0.3.0"])],
        });

        result.Should().HaveStderrMatch($"*Error use-recent-module-versions: Use a more recent version of module 'fake/avm/res/app/container-app'. The most recent version is 0.3.0.*");
        result.Should().HaveStdout("");
        result.Should().Fail();
    }

    [TestMethod]
    public async Task MetadataOrderShouldntMatter1()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 '{PREFIX}/fake/avm/res/app/container-app:0.2.0' = {
                  name: 'm1'
                }
                """.Replace("{PREFIX}", PREFIX),
            PublishedModules = [
                $"{PREFIX}/fake/avm/res/app/container-app:0.2.0",
                $"{PREFIX}/fake/avm/res/app/container-app:0.3.0"
            ],
            ModulesMetadata = [("fake/avm/res/app/container-app", ["0.2.0", "0.3.0"])],
        });

        result.Should().HaveStderrMatch($"*Warning use-recent-module-versions: Use a more recent version of module 'fake/avm/res/app/container-app'. The most recent version is 0.3.0.*");
        result.Should().HaveStdout("");
        result.Should().Succeed();
    }

    [TestMethod]
    public async Task MetadataOrderShouldntMatter2()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 '{PREFIX}/fake/avm/res/app/container-app:0.2.0' = {
                  name: 'm1'
                }
                """.Replace("{PREFIX}", PREFIX),
            PublishedModules = [
                $"{PREFIX}/fake/avm/res/app/container-app:0.2.0",
                $"{PREFIX}/fake/avm/res/app/container-app:0.3.0"
            ],
            ModulesMetadata = [("fake/avm/res/app/container-app", ["0.3.0", "0.2.0"])],
        });

        result.Should().HaveStderrMatch($"*Warning use-recent-module-versions: Use a more recent version of module 'fake/avm/res/app/container-app'. The most recent version is 0.3.0.*");
        result.Should().HaveStdout("");
        result.Should().Succeed();
    }

    [TestMethod]
    public async Task IfModuleNotFound_ShouldIgnore()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 '{PREFIX}/fake/avm/res/app/unknown-module:0.3.0' = { // Our metadata doesn't recognize this
                  name: 'm1'
                }
                """.Replace("{PREFIX}", PREFIX),
            PublishedModules = [$"{PREFIX}/fake/avm/res/app/unknown-module:unknown-version"], // Required for mock to work
            ModulesMetadata = [("fake/avm/res/app/container-app", ["0.3.0", "0.2.0"])],
        });

        result.Should().HaveCompileError(UnableToRestoreErrorCode);
        result.Should().NotHaveRecentModuleVersionsRuleFailure();
        result.Should().HaveStdout("");
        result.Should().Fail();
    }

    [TestMethod]
    public async Task IfModuleUsesVersionsGreaterThanOurCacheRecognizes_ShouldPass()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 '{PREFIX}/fake/avm/res/app/container-app:0.3.0' = { // Our metadata only recognizes up to 0.2.0, but 0.3.0 does exist
                  name: 'm1'
                }
                """.Replace("{PREFIX}", PREFIX),
            PublishedModules = [$"{PREFIX}/fake/avm/res/app/container-app:0.3.0"],
            ModulesMetadata = [("fake/avm/res/app/container-app", ["0.1.0", "0.2.0"])],
        });

        result.Should().NotHaveStderr();
        result.Should().HaveStdout("");
        result.Should().Succeed();
    }

    [TestMethod]
    public async Task IfModuleUsesUnknownVersion_ThatIsLowerThanOurCacheRecognizes_ShouldFail()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 '{PREFIX}/fake/avm/res/app/container-app:0.1.0' = { // Our metadata recognizes 0.2.0, 0.4.0, but 0.1.0 does exist
                  name: 'm1'
                }
                """.Replace("{PREFIX}", PREFIX),
            PublishedModules = [$"{PREFIX}/fake/avm/res/app/container-app:0.1.0"],
            ModulesMetadata = [("fake/avm/res/app/container-app", ["0.2.0", "0.4.0"])],
        });

        result.Should().HaveStderrMatch("*Warning use-recent-module-versions: Use a more recent version of module 'fake/avm/res/app/container-app'. The most recent version is 0.4.0.*");
        result.Should().HaveStdout("");
        result.Should().Succeed(); // only a warning
    }

    [TestMethod]
    public async Task IfModuleUsesUnknownVersion_ThatIsLowerThanOurCacheHighestVersion_ShouldFail()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 '{PREFIX}/fake/avm/res/app/container-app:0.3.0' = { // Our metadata recognizes 0.1.0, 0.2.0, 0.4.0, but 0.3.0 does exist
                  name: 'm1'
                }
                """.Replace("{PREFIX}", PREFIX),
            PublishedModules = [$"{PREFIX}/fake/avm/res/app/container-app:0.3.0"],
            ModulesMetadata = [("fake/avm/res/app/container-app", ["0.1.0", "0.2.0", "0.4.0"])],
        });

        result.Should().HaveStderrMatch("*Warning use-recent-module-versions: Use a more recent version of module 'fake/avm/res/app/container-app'. The most recent version is 0.4.0.*");
        result.Should().HaveStdout("");
        result.Should().Succeed(); // only a warning
    }

    [TestMethod] //asfdg fails when run with others
    public async Task IfModuleVersionIsNotPublished_ButCacheHasMoreRecentVersion_ThenShouldGiveCompilerError_AndLinterShouldFail()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 '{PREFIX}/fake/avm/res/app/container-app:0.3.0' = { // 0.3.0 does not exist
                  name: 'm1'
                }
                """.Replace("{PREFIX}", PREFIX),
            PublishedModules = [
                $"{PREFIX}/fake/avm/res/app/container-app:0.1.0",
                $"{PREFIX}/fake/avm/res/app/container-app:0.2.0",
                $"{PREFIX}/fake/avm/res/app/container-app:0.4.0"],
            ModulesMetadata = [("fake/avm/res/app/container-app", ["0.1.0", "0.2.0", "0.4.0"])],
        });

        result.Should().HaveCompileError(UnableToRestoreErrorCode);
        result.Should().HaveStderrMatch("*Warning use-recent-module-versions: Use a more recent version of module 'fake/avm/res/app/container-app'. The most recent version is 0.4.0.*");
        result.Should().HaveStdout("");
        result.Should().Fail();
    }

    [TestMethod]
    public async Task IfModuleVersionIsHigherThanPublished_ThenShouldGiveCompileError_ButLinterShouldIgnore()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 '{PREFIX}/fake/avm/res/app/container-app:0.3.1' = { // 0.1.0 and 0.2.0 exist
                  name: 'm1'
                }
                """.Replace("{PREFIX}", PREFIX),
            PublishedModules = [
                $"{PREFIX}/fake/avm/res/app/container-app:0.1.0",
                $"{PREFIX}/fake/avm/res/app/container-app:0.2.0"],
            ModulesMetadata = [("fake/avm/res/app/container-app", ["0.1.0", "0.2.0"])],
        });

        result.Should().HaveCompileError(UnableToRestoreErrorCode);
        result.Should().NotHaveRecentModuleVersionsRuleFailure();
        result.Should().HaveStdout("");
    }

    [TestMethod]
    public async Task IfModuleNotRecognizedByMetadata_ThenCompileError_ButLinterShouldIgnore()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 '{PREFIX}/fake/avm/res/app/unknown:0.1.0' = {
                  name: 'm1'
                }
                """.Replace("{PREFIX}", PREFIX),
            PublishedModules = [
                $"{PREFIX}/fake/avm/res/app/container-app:0.1.0",
                $"{PREFIX}/fake/avm/res/app/container-app:0.2.0"],
            ModulesMetadata = [("fake/avm/res/app/container-app", ["0.1.0", "0.2.0"])],
        });

        result.Should().HaveCompileError(UnableToRestoreErrorCode);
        result.Should().NotHaveRecentModuleVersionsRuleFailure();
        result.Should().HaveStdout("");
    }

    [TestMethod]
    public async Task IfModuleVersion_IsNotSemanticVersion_LinterShouldIgnore()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 '{PREFIX}/fake/avm/res/app/container-app:v0.1.0' = {
                  name: 'm1'
                }
                module m2 '{PREFIX}/fake/avm/res/app/container-app:abc' = {
                  name: 'm2'
                }
                module m3 '{PREFIX}/fake/avm/res/app/container-app:0.1.0.0' = {
                  name: 'm3'
                }
                module m4 '{PREFIX}/fake/avm/res/app/container-app:0.1' = {
                  name: 'm4'
                }
                module m5 '{PREFIX}/fake/avm/res/app/container-app:1' = {
                  name: 'm5'
                }
                """.Replace("{PREFIX}", PREFIX),
            PublishedModules = [
                $"{PREFIX}/fake/avm/res/app/container-app:v0.1.0",
                $"{PREFIX}/fake/avm/res/app/container-app:abc",
                $"{PREFIX}/fake/avm/res/app/container-app:0.1.0.0",
                $"{PREFIX}/fake/avm/res/app/container-app:0.1",
                $"{PREFIX}/fake/avm/res/app/container-app:1",
            ],
            ModulesMetadata = [("fake/avm/res/app/container-app", ["10.0.0"])],
        });

        result.Should().NotHaveRecentModuleVersionsRuleFailure();
        result.Should().HaveStdout("");
    }

    [TestMethod]
    public async Task IfModuleVersion_HasOldVersionWithSuffix_Fail()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1a '{PREFIX}/fake/avm/res/app/container-app1a:0.1.0' = { // fail
                  name: 'm1a'
                }
                module m1b '{PREFIX}/fake/avm/res/app/container-app1b:0.1.0-beta' = { // fail
                  name: 'm1b'
                }
                """.Replace("{PREFIX}", PREFIX),
            PublishedModules = [
                $"{PREFIX}/fake/avm/res/app/container-app1a:0.1.0",
                $"{PREFIX}/fake/avm/res/app/container-app1b:0.1.0-beta",
            ],
            ModulesMetadata = [
                ("fake/avm/res/app/container-app1a", ["0.1.0", "0.2.0"]),
                ("fake/avm/res/app/container-app1b", ["0.1.0", "0.2.0"]),
            ],
        });

        result.Should().NotHaveCompileError(UnableToRestoreErrorCode);
        result.Should().HaveStderrMatch("*main.bicep(1,12) : Warning use-recent-module-versions: Use a more recent version of module 'fake/avm/res/app/container-app1a'. The most recent version is 0.2.0. *");
        result.Should().HaveStderrMatch("*main.bicep(4,12) : Warning use-recent-module-versions: Use a more recent version of module 'fake/avm/res/app/container-app1b'. The most recent version is 0.2.0. *");

        result.Should().HaveStdout("");
    }

    [TestMethod]
    public async Task SuffixIsLowerThanVersionWithoutSuffix()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m2 '{PREFIX}/fake/avm/res/app/container-app2:0.2.0-alpha' = { // fail
                  name: 'm2'
                }
                """.Replace("{PREFIX}", PREFIX),
            PublishedModules = [
                $"{PREFIX}/fake/avm/res/app/container-app2:0.2.0-alpha"
            ],
            ModulesMetadata = [
                ("fake/avm/res/app/container-app2", ["0.1.0", "0.2.0"]),
            ],
        });

        result.Should().HaveStderrMatch("*Warning use-recent-module-versions: Use a more recent version of module 'fake/avm/res/app/container-app2'. The most recent version is 0.2.0.*");
        result.Should().HaveStdout("");
    }

    [TestMethod]
    public async Task SuffixWithHigherVersion_IsHigherThanLowerVersionWithoutSuffix()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m2 '{PREFIX}/fake/avm/res/app/container-app2:0.3.0-alpha' = { // pass
                  name: 'm2'
                }
                """.Replace("{PREFIX}", PREFIX),
            PublishedModules = [
                $"{PREFIX}/fake/avm/res/app/container-app2:0.3.0-alpha"
            ],
            ModulesMetadata = [
                ("fake/avm/res/app/container-app2", ["0.1.0", "0.2.0"]),
            ],
        });

        result.Should().NotHaveStderr();
        result.Should().HaveStdout("");
    }

    [TestMethod]
    public async Task IfModuleVersion_InvalidVersion_ThenCompileError_ButLinterShouldIgnore()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 '{PREFIX}/fake/avm/res/app/container-app:1.0.*' = {
                  name: 'm1'
                }
                module m2 '{PREFIX}/fake/avm/res/app/container-app:' = {
                  name: 'm2'
                }
                module m3 '{PREFIX}/fake/avm/res/app/container-app' = {
                  name: 'm3'
                }
                """.Replace("{PREFIX}", PREFIX),
            PublishedModules = [],
            ModulesMetadata = [("fake/avm/res/app/container-app", ["10.0.0"])],
        });

        result.Should().HaveCompileError("BCP198");
        result.Should().NotHaveRecentModuleVersionsRuleFailure();
        result.Should().HaveStdout("");
    }

    // Public modules don't currently have suffixes, but can't guarantee this won't happen in the future
    [TestMethod]
    public async Task IfModuleMetadataIsAlpha_AndReferencesAlpha_ShouldPass()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 '{PREFIX}/fake/avm/res/app/container-app:1.0.0-alpha' = {
                  name: 'm1'
                }
                """.Replace("{PREFIX}", PREFIX),
            PublishedModules = [
                $"{PREFIX}/fake/avm/res/app/container-app:1.0.0-alpha"
            ],
            ModulesMetadata = [("fake/avm/res/app/container-app", ["1.0.0-alpha"])],
        });

        result.Should().NotHaveCompileError();
        result.Should().NotHaveRecentModuleVersionsRuleFailure();
        result.Should().HaveStdout("");
    }

    // Public modules don't currently have suffixes, but can't guarantee this won't happen in the future
    [TestMethod]
    public async Task IfModuleMetadataIsBeta_AndReferencesRelease_ShouldPass()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 '{PREFIX}/fake/avm/res/app/container-app:1.0.0' = {
                  name: 'm1'
                }
                """.Replace("{PREFIX}", PREFIX),
            PublishedModules = [
                $"{PREFIX}/fake/avm/res/app/container-app:1.0.0"
            ],
            ModulesMetadata = [("fake/avm/res/app/container-app", ["1.0.0-beta"])],
        });

        result.Should().NotHaveCompileError();
        result.Should().NotHaveRecentModuleVersionsRuleFailure();
        result.Should().HaveStdout("");
    }

    // Public modules don't currently have suffixes, but can't guarantee this won't happen in the future
    [TestMethod]
    public async Task IfModuleMetadataIsRelease_AndReferencesBeta_ShouldFail()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 '{PREFIX}/fake/avm/res/app/container-app:1.0.0-beta' = {
                  name: 'm1'
                }
                """.Replace("{PREFIX}", PREFIX),
            PublishedModules = [
                $"{PREFIX}/fake/avm/res/app/container-app:1.0.0-beta"
            ],
            ModulesMetadata = [("fake/avm/res/app/container-app", ["1.0.0"])],
        });

        result.Should().NotHaveCompileError();
        result.Should().HaveStderrMatch("*Warning use-recent-module-versions: Use a more recent version of module 'fake/avm/res/app/container-app'. The most recent version is 1.0.0.*");
        result.Should().HaveStdout("");
    }

    // Public modules don't currently have suffixes, but can't guarantee this won't happen in the future
    [TestMethod]
    public async Task IfModuleMetadataHasReleaseAndBeta_AndReferencesBeta_ShouldFail()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 '{PREFIX}/fake/avm/res/app/container-app:1.0.0-beta' = {
                  name: 'm1'
                }
                """.Replace("{PREFIX}", PREFIX),
            PublishedModules = [
                $"{PREFIX}/fake/avm/res/app/container-app:1.0.0-beta"
            ],
            ModulesMetadata = [("fake/avm/res/app/container-app", ["1.0.0", "1.0.0-beta"])],
        });

        result.Should().NotHaveCompileError();
        result.Should().HaveStderrMatch("*Warning use-recent-module-versions: Use a more recent version of module 'fake/avm/res/app/container-app'. The most recent version is 1.0.0.*");
        result.Should().HaveStdout("");
    }

    // Public modules don't currently have suffixes, but can't guarantee this won't happen in the future
    [TestMethod]
    public async Task IfModuleMetadataHasReleaseAndBeta_AndReferencesBeta_ShouldFail_MetadataOrderShouldntMatter()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 '{PREFIX}/fake/avm/res/app/container-app:1.0.0-beta' = {
                  name: 'm1'
                }
                """.Replace("{PREFIX}", PREFIX),
            PublishedModules = [
                $"{PREFIX}/fake/avm/res/app/container-app:1.0.0-beta"
            ],
            ModulesMetadata = [("fake/avm/res/app/container-app", ["1.0.0-beta", "1.0.0"])],
        });

        result.Should().NotHaveCompileError();
        result.Should().HaveStderrMatch("*Warning use-recent-module-versions: Use a more recent version of module 'fake/avm/res/app/container-app'. The most recent version is 1.0.0.*");
        result.Should().HaveStdout("");
    }

    // Public modules don't currently have suffixes, but can't guarantee this won't happen in the future
    [TestMethod]
    public async Task IfModuleMetadataHasReleaseAndBeta_FailureMessageShouldShowReleaseVersion()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 '{PREFIX}/fake/avm/res/app/container-app:1.0.0-beta' = {
                  name: 'm1'
                }
                """.Replace("{PREFIX}", PREFIX),
            PublishedModules = [
                $"{PREFIX}/fake/avm/res/app/container-app:1.0.0-beta"
            ],
            ModulesMetadata = [("fake/avm/res/app/container-app", ["1.0.0", "1.0.0-beta", "2.0.0", "2.0.0-alpha", "2.0.0-beta"])],
        });

        result.Should().NotHaveCompileError();
        result.Should().HaveStderrMatch("*Warning use-recent-module-versions: Use a more recent version of module 'fake/avm/res/app/container-app'. The most recent version is 2.0.0.*");
        result.Should().HaveStdout("");
    }

    [TestMethod]
    public async Task PublicBicepAlias_ShouldWork()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 'br/public:fake/avm/res/app/container-app:1.0.0' = {
                  name: 'm1'
                }
                """,
            PublishedModules = [
                $"{PREFIX}/fake/avm/res/app/container-app:1.0.0"
            ],
            ModulesMetadata = [("fake/avm/res/app/container-app", ["1.0.1", "1.0.0"])],
        });

        result.Should().NotHaveCompileError();
        result.Should().HaveStderrMatch("*Warning use-recent-module-versions: Use a more recent version of module 'fake/avm/res/app/container-app'. The most recent version is 1.0.1.*");
        result.Should().HaveStdout("");
    }

    [TestMethod]
    public async Task PrivateModules_ShouldCurrentlyBeIgnored()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 'br:mcr.private.com/bicep/fake/avm/res/app/container-app:1.0.0' = {
                  name: 'm1'
                }
                """,
            PublishedModules = [
                $"br:mcr.private.com/bicep/fake/avm/res/app/container-app:1.0.0"
            ],
            ModulesMetadata = [("fake/avm/res/app/container-app", ["1.0.1", "1.0.0"])],
        });

        result.Should().NotHaveCompileError();
        result.Should().NotHaveRecentModuleVersionsRuleFailure();
        result.Should().HaveStdout("");
    }

    [TestMethod]
    public async Task PrivateModules_ShouldCurrentlyBeIgnored_EvenIfNoCache()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 'br:mcr.private.com/bicep/fake/avm/res/app/container-app:1.0.0' = {
                  name: 'm1'
                }
                """,
            PublishedModules = [
                $"br:mcr.private.com/bicep/fake/avm/res/app/container-app:1.0.0"
            ],
            ModulesMetadata = [("fake/avm/res/app/container-app", ["1.0.1", "1.0.0"])],
            NoRestore = true,
        });

        result.Should().HaveCompileError(NotRestoredErrorCode);
        result.Should().NotHaveRecentModuleVersionsRuleFailure();
        result.Should().HaveStdout("");
    }

    [TestMethod]
    public async Task PublicModule_WithoutBicepPrefix_ShouldntHappen_ButShouldBeIgnoredIfItDoes()
    {
        var result = await Test(new Options(CacheRoot)
        {
            Bicep = """
                module m1 'br:mcr.microsoft.com/fake/avm/res/app/container-app:1.0.0' = {
                  name: 'm1'
                }
                """,
            PublishedModules = [
                $"br:mcr.microsoft.com/fake/avm/res/app/container-app:1.0.0"
            ],
            ModulesMetadata = [("fake/avm/res/app/container-app", ["1.0.1", "1.0.0"])],
        });

        result.Should().NotHaveCompileError();
        result.Should().NotHaveStderr();
        result.Should().HaveStdout("");
    }
}

public static class CliResultUseRecentModuleVersionsExtensions
{
    public static AndConstraint<CliResultAssertions> HaveRecentModuleVersionsRuleFailure(this CliResultAssertions instance, string because = "", params object[] becauseArgs)
    {
        instance.Subject.Should().HaveRuleFailure(UseRecentModuleVersionsRule.Code, because, becauseArgs);

        return new(instance);
    }

    public static AndConstraint<CliResultAssertions> NotHaveRecentModuleVersionsRuleFailure(this CliResultAssertions instance, string because = "", params object[] becauseArgs)
    {
        instance.Subject.Should().NotHaveRuleFailure(UseRecentModuleVersionsRule.Code, because, becauseArgs);

        return new(instance);
    }
}
