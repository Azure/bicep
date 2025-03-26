// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Reflection;
using System.Text.RegularExpressions;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.CodeAction;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Json;
using Bicep.Core.Parsing;
using Bicep.Core.Registry.Catalog;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using static Bicep.Core.UnitTests.Utils.CompilationHelper;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    // More extensive tests in src/Bicep.Cli.IntegrationTests/UseRecentModuleVersionsIntegrationTests.cs

    [TestClass]
    public partial class UseRecentModuleVersionsRuleTests : LinterRuleTestsBase
    {
        private static readonly ServiceBuilder Services = new ServiceBuilder()
            .WithRegistration(x => x.AddSingleton(
                IConfigurationManager.WithStaticConfiguration(
                    IConfigurationManager.GetBuiltInConfiguration()
                    .WithAllAnalyzers())));

        private static CompilationResult Compile(
            string bicep,
            string[] availableModules,
            string[] availableVersions, // for simplicity, mock returns these same versions for all available modules
            string? downloadError = null)
        {
            var publicModuleMetadataProvider = StrictMock.Of<IPublicModuleMetadataProvider>();
            publicModuleMetadataProvider.Setup(x => x.GetCachedModules())
                .Returns([.. availableModules
                    .Select(m => new RegistryModuleMetadata(
                        "mcr.microsoft.com",
                        m,
                        new RegistryModuleMetadata.ComputedData(
                            new RegistryMetadataDetails(null, null),
                            [.. availableVersions
                                .Select(v => new RegistryModuleVersionMetadata(v, IsBicepModule: true, new("det", "doc.html")))]
                        )))
                ]);

            publicModuleMetadataProvider.Setup(x => x.IsCached)
                .Returns(availableModules.Length > 0);
            publicModuleMetadataProvider.Setup(x => x.DownloadError)
                .Returns(downloadError);

            var services = Services.WithRegistration(x => x.AddSingleton(publicModuleMetadataProvider.Object));
            var result = CompilationHelper.Compile(services, [("main.bicep", bicep)]);
            return result;
        }

        [TestMethod]
        public void IfUsingOldVersionThatWeDontKnowAbout_Fail()
        {
            var result = Compile("""
                    module m1 'br/public:avm/res/network/public-ip-address:0.4.0' = {
                    }
                    """,
                ["bicep/avm/res/network/public-ip-address"],
                ["1.0.0"]
            );
            result.Diagnostics.Where(d => d.Code == UseRecentModuleVersionsRule.Code)
                .Should()
                .HaveDiagnostics([
                    (UseRecentModuleVersionsRule.Code, DiagnosticLevel.Warning, "Use a more recent version of module 'avm/res/network/public-ip-address'. The most recent version is 1.0.0.")
                ]);
        }

        [TestMethod]
        public void IfUsingNewVersionThatWeDontKnowAbout_Passes()
        {
            // This is the scenario where one or more versions have been published since the cache was updated, and the
            //   user is using a newer version  not in our cache.
            // Either
            var result = Compile("""
                    module m1 'br/public:avm/res/network/public-ip-address:1.4.0' = {
                    }
                    """,
                ["bicep/avm/res/network/public-ip-address"],
                ["1.0.0"]
            );
            result.Diagnostics.Where(d => d.Code == UseRecentModuleVersionsRule.Code)
                .Should()
                .NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void UsingNewestVersion_NoFailures()
        {
            var result = Compile("""
                    module m1 'br/public:avm/res/network/public-ip-address:0.4.0' = {
                    }
                    """,
                ["bicep/avm/res/network/public-ip-address"],
                ["0.3.0", "0.3.1", "0.4.0"]
            );
            result.Diagnostics.Where(d => d.Code == UseRecentModuleVersionsRule.Code)
                .Should()
                .NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void IfVersionsNotDownloaded_ThenShowExactlyOneFailure()
        {
            var result = Compile("""
                    module m1 'br/public:avm/res/network/public-ip-address:0.4.0' = {
                    }
                    module m1 'br/public:avm/res/network/public-ip-address:0.4.0' = {
                    }
                    module m1 'br/public:avm/res/network/public-ip-address:0.4.0' = {
                    }
                    """,
                [], // not yet downloaded
                []
            );
            result.Diagnostics.Where(d => d.Code == UseRecentModuleVersionsRule.Code)
                .Should()
                .HaveDiagnostics([
                    // Should only show this warning for the first module we find
                    ("use-recent-module-versions", DiagnosticLevel.Warning, "Available module versions have not yet been downloaded. If running from the command line, be sure --no-restore is not specified.")
                ]);
        }

        [TestMethod]
        public void IfVersionsDownloadFailed_ThenShowExactlyOneFailure()
        {
            var result = Compile("""
                    module m1 'br/public:avm/res/network/public-ip-address:0.4.0' = {
                    }
                    module m1 'br/public:avm/res/network/public-ip-address:0.4.0' = {
                    }
                    module m1 'br/public:avm/res/network/public-ip-address:0.4.0' = {
                    }
                    """,
                [],
                [],
                "download error"
            );
            result.Diagnostics.Where(d => d.Code == UseRecentModuleVersionsRule.Code)
                .Should()
                .HaveDiagnostics([
                    // Should only show this warning for the first module
                    ("use-recent-module-versions", DiagnosticLevel.Warning, "Could not download available module versions: download error")
                ]);
        }

        [TestMethod]
        public void IgnoreNonPublicModules()
        {
            var result = Compile("""
                    module m1 'br/whatever:avm/res/network/public-ip-address:0.4.0' = {
                    }
                    """,
                [],
                []
            );
            result.Diagnostics.Where(d => d.Code == UseRecentModuleVersionsRule.Code)
                .Should()
                .NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void HasDownloadError()
        {
            var result = Compile("""
                    module m1 'br/public:avm/res/network/public-ip-address:0.4.0' = {
                    }
                    """,
                ["bicep/avm/res/network/public-ip-address"],
                ["1.0.0"],
                "My download error"
            );
            result.Diagnostics.Where(d => d.Code == UseRecentModuleVersionsRule.Code)
                .Should()
                .HaveDiagnostics([
                    ("use-recent-module-versions", DiagnosticLevel.Warning, "Could not download available module versions: My download error")
                ]);
        }

        [TestMethod]
        public void SingleMinorVersionBehind()
        {
            var result = Compile("""
                    module m1 'br/public:avm/res/network/public-ip-address:0.4.0' = {
                    }
                    """,
                ["bicep/avm/res/network/public-ip-address"],
                ["0.3.0", "0.4.0", "0.5.0", "1.0.1"]
            );
            result.Diagnostics.Where(d => d.Code == UseRecentModuleVersionsRule.Code)
                .Should()
                .HaveDiagnostics([
                    ("use-recent-module-versions", DiagnosticLevel.Warning, "Use a more recent version of module 'avm/res/network/public-ip-address'. The most recent version is 1.0.1.")
                ]);
        }

        [TestMethod]
        public void SingleMajorVersionBehind()
        {
            var result = Compile("""
                    module m1 'br/public:avm/res/network/public-ip-address:0.4.0' = {
                    }
                    """,
                ["bicep/avm/res/network/public-ip-address"],
                ["0.3.0", "0.4.0", "1.0.0"]
            );
            result.Diagnostics.Where(d => d.Code == UseRecentModuleVersionsRule.Code)
                .Should()
                .HaveDiagnostics([
                    ("use-recent-module-versions", DiagnosticLevel.Warning, "Use a more recent version of module 'avm/res/network/public-ip-address'. The most recent version is 1.0.0.")
                ]);
        }

        [TestMethod]
        public void SinglePatchVersionBehind()
        {
            var result = Compile("""
                    module m1 'br/public:avm/res/network/public-ip-address:0.4.1' = {
                    }
                    """,
                ["bicep/avm/res/network/public-ip-address"],
                ["0.3.0", "0.4.1", "0.4.2", "0.5.0"]
            );
            result.Diagnostics.Where(d => d.Code == UseRecentModuleVersionsRule.Code)
                .Should()
                .HaveDiagnostics([
                    ("use-recent-module-versions", DiagnosticLevel.Warning, "Use a more recent version of module 'avm/res/network/public-ip-address'. The most recent version is 0.5.0.")
                ]);
        }

        [TestMethod]
        public void MultiplePatchVersionsBehind()
        {
            var result = Compile("""
                    module m1 'br/public:avm/res/network/public-ip-address:0.4.1' = {
                    }
                    """,
                ["bicep/avm/res/network/public-ip-address"],
                ["0.3.0", "0.4.1", "0.4.2", "0.4.5"]
            );
            result.Diagnostics.Where(d => d.Code == UseRecentModuleVersionsRule.Code)
                .Should()
                .HaveDiagnostics([
                    ("use-recent-module-versions", DiagnosticLevel.Warning, "Use a more recent version of module 'avm/res/network/public-ip-address'. The most recent version is 0.4.5.")
                ]);
        }

        [TestMethod]
        public void HasFix()
        {
            var result = Compile("""
                    module m1 'br/public:avm/res/network/public-ip-address:0.4.0' = {
                    }
                    """,
                ["bicep/avm/res/network/public-ip-address"],
                ["0.3.0", "0.4.0", "0.5.0", "1.0.1"]
            );

            var diag = result.Diagnostics.Where(d => d.Code == UseRecentModuleVersionsRule.Code).First();
            var fixes = diag.Fixes.ToArray();
            fixes.Should().HaveCount(1);
            fixes[0].Replacements.Should().HaveCount(1);
            fixes[0].Replacements.First().Text.Should().Be("1.0.1");
            fixes[0].Title.Should().Be("Replace with most recent version '1.0.1'");
        }
    }
}
