// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using Bicep.LangServer.IntegrationTests;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Registry;
using CommandLine.Text;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Bicep.Core.UnitTests.Diagnostics.LinterRuleTests.UseRecentApiVersionRuleTests.GetAcceptableApiVersionsInvariantsTests;

namespace Bicep.LangServer.UnitTests.Completions
{
    [TestClass]
    public class PublicRegistryModuleMetadataProviderTests
    {
        private const string ModuleIndex = @"[
  {
    ""moduleName"": ""app/dapr-containerapp"",
    ""tags"": [
      ""1.0.1"",
      ""1.0.2""
    ]
  },
  {
    ""moduleName"": ""app/dapr-containerapps-environment"",
    ""tags"": [
      ""1.0.1"",
      ""1.1.1"",
      ""1.2.1"",
      ""1.2.2""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""azure-gaming/game-dev-vm"",
    ""tags"": [
      ""1.0.1"",
      ""1.0.2"",
      ""2.0.1"",
      ""2.0.2""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""azure-gaming/game-dev-vmss"",
    ""tags"": [
      ""1.0.1"",
      ""1.1.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""compute/availability-set"",
    ""tags"": [
      ""1.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""compute/container-registry"",
    ""tags"": [
      ""1.0.1"",
      ""1.0.2""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""compute/custom-image-vmss"",
    ""tags"": [
      ""1.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""cost/resourcegroup-scheduled-action"",
    ""tags"": [
      ""1.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""cost/subscription-scheduled-action"",
    ""tags"": [
      ""1.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""deployment-scripts/aks-run-command"",
    ""tags"": [
      ""1.0.1"",
      ""1.0.2"",
      ""1.0.3"",
      ""2.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""deployment-scripts/aks-run-helm"",
    ""tags"": [
      ""1.0.1"",
      ""2.0.1"",
      ""2.0.2""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""deployment-scripts/build-acr"",
    ""tags"": [
      ""1.0.1"",
      ""1.0.2"",
      ""2.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""deployment-scripts/create-kv-certificate"",
    ""tags"": [
      ""1.0.1"",
      ""1.1.1"",
      ""1.1.2"",
      ""2.1.1"",
      ""3.0.1"",
      ""3.0.2"",
      ""3.1.1"",
      ""3.2.1"",
      ""3.3.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""deployment-scripts/import-acr"",
    ""tags"": [
      ""1.0.1"",
      ""2.0.1"",
      ""2.1.1"",
      ""3.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""deployment-scripts/wait"",
    ""tags"": [
      ""1.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""identity/user-assigned-identity"",
    ""tags"": [
      ""1.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""lz/sub-vending"",
    ""tags"": [
      ""1.1.1"",
      ""1.1.2"",
      ""1.2.1"",
      ""1.2.2"",
      ""1.3.1""
    ],
    ""properties"": {
      ""1.1.1"": {
        ""description"": ""1.1.1: These are the input parameters for the Bicep module: [`main.bicep`](./main.bicep)\n\nThis is the orchestration module that is used and called by a consumer of the module to deploy a Landing Zone Subscription and its associated resources, based on the parameter input values that are provided to it at deployment time.""
      },
      ""1.1.2"": {
        ""description"": ""1.1.2: These are the input parameters for the Bicep module: [`main.bicep`](./main.bicep)\n\nThis is the orchestration module that is used and called by a consumer of the module to deploy a Landing Zone Subscription and its associated resources, based on the parameter input values that are provided to it at deployment time.""
      },
      ""1.2.1"": {
        ""description"": ""1.2.1: These are the input parameters for the Bicep module: [`main.bicep`](./main.bicep)\n\nThis is the orchestration module that is used and called by a consumer of the module to deploy a Landing Zone Subscription and its associated resources, based on the parameter input values that are provided to it at deployment time.""
      },
      ""1.2.2"": {
        ""description"": ""1.2.2: These are the input parameters for the Bicep module: [`main.bicep`](./main.bicep)\n\nThis is the orchestration module that is used and called by a consumer of the module to deploy a Landing Zone Subscription and its associated resources, based on the parameter input values that are provided to it at deployment time.""
      },
      ""1.3.1"": {
        ""description"": ""1.3.1: These are the input parameters for the Bicep module: [`main.bicep`](./main.bicep)\n\nThis is the orchestration module that is used and called by a consumer of the module to deploy a Landing Zone Subscription and its associated resources, based on the parameter input values that are provided to it at deployment time.""
      }
    }
  },
  {
    ""moduleName"": ""network/dns-zone"",
    ""tags"": [
      ""1.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""network/nat-gateway"",
    ""tags"": [
      ""1.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""network/traffic-manager"",
    ""tags"": [
      ""1.0.1"",
      ""2.0.1"",
      ""2.1.1"",
      ""2.2.1"",
      ""2.3.1"",
      ""2.3.2""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""network/virtual-network"",
    ""tags"": [
      ""1.0.1"",
      ""1.0.2"",
      ""1.0.3"",
      ""1.1.1"",
      ""1.1.2""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""observability/grafana"",
    ""tags"": [
      ""1.0.1"",
      ""1.0.2""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""samples/array-loop"",
    ""tags"": [
      ""1.0.1"",
      ""1.0.2""
    ],
    ""properties"": {
      ""1.0.1"": {
        ""description"": ""Description for 1.0.1""
      }
    }
  },
  {
    ""moduleName"": ""samples/hello-world"",
    ""tags"": [
      ""1.0.1"",
      ""1.0.2"",
      ""1.0.3""
    ],
    ""properties"": {
      ""1.0.3"": {
        ""description"": ""A \""שָׁלוֹם עוֹלָם\"" sample Bicep registry module"",
        ""other property - we have to be forwards compatible"": ""whatever""
      },
      ""unexpected tag should be ignored"": {
      }
    },
    ""other property - we have to be forwards compatible"": ""whatever""
  },
  {
    ""moduleName"": ""security/keyvault"",
    ""tags"": [
      ""1.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""storage/cosmos-db"",
    ""tags"": [
      ""1.0.1"",
      ""2.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""storage/log-analytics-workspace"",
    ""tags"": [
      ""1.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""storage/redis-cache"",
    ""tags"": [
      ""0.0.1""
    ],
    ""properties"": {}
  },
  {
    ""moduleName"": ""storage/storage-account"",
    ""tags"": [
      ""0.0.1"",
      ""1.0.1"",
      ""2.0.1"",
      ""2.0.2""
    ],
    ""properties"": {}
  }
]";

        [TestMethod]
        public void GetExponentialDelay_ZeroCount_ShouldGiveInitialDelay()
        {
            TimeSpan initial = TimeSpan.FromDays(2.5);
            TimeSpan max = TimeSpan.FromDays(10);
            PublicRegistryModuleMetadataProvider provider = new();
            var delay = provider.GetExponentialDelay(initial, 0, max);

            delay.Should().Be(initial);
        }

        [TestMethod]
        public void GetExponentialDelay_1Count_ShouldGiveDoubleInitialDelay()
        {
            TimeSpan initial = TimeSpan.FromDays(2.5);
            TimeSpan max = TimeSpan.FromDays(10);
            PublicRegistryModuleMetadataProvider provider = new();
            var delay = provider.GetExponentialDelay(initial, 1, max);

            delay.Should().Be(initial*2);
        }

        [TestMethod]
        public void GetExponentialDelay_2Count_ShouldGiveQuadrupleInitialDelay()
        {
            TimeSpan initial = TimeSpan.FromDays(2.5);
            TimeSpan max = TimeSpan.FromDays(10);
            PublicRegistryModuleMetadataProvider provider = new();
            var delay = provider.GetExponentialDelay(initial, 2, max);

            delay.Should().Be(initial * 4);
        }

        [TestMethod]
        public void GetExponentialDelay_AboveMaxCount_ShouldGiveMaxDelay()
        {
            TimeSpan initial = TimeSpan.FromSeconds(1);
            TimeSpan max = TimeSpan.FromDays(365);
            PublicRegistryModuleMetadataProvider provider = new();

            TimeSpan exponentiallyGrowingDelay = initial;
            int count = 0;
            while (exponentiallyGrowingDelay < max * 1000)
            {
                var delay = provider.GetExponentialDelay(initial, count, max);

                if (exponentiallyGrowingDelay < max)
                {
                    delay.Should().Be(exponentiallyGrowingDelay);
                }
                else
                {
                    delay.Should().Be(max);
                }

                delay.Should().BeLessThanOrEqualTo(max);

                ++count;
                exponentiallyGrowingDelay *= 2;
            }
        }

        private record ModuleMetadata_Original(string moduleName, List<string> tags);

        [TestMethod]
        public void GetModules_ForwardsCompatibleWithOriginalVersion()
        {
            // Earlier Bicep versions should not be confused by new metadata formats
            var metadataStream = new MemoryStream(Encoding.UTF8.GetBytes(ModuleIndex));
            ModuleMetadata_Original[] metadata = JsonSerializer.Deserialize<ModuleMetadata_Original[]>(metadataStream)!.ToArray();

            metadata.Length.Should().BeGreaterThanOrEqualTo(29);
            metadata.Select(m => m.moduleName).Should().Contain("samples/array-loop");
            metadata.First(m => m.moduleName == "samples/array-loop").tags.Should().Contain("1.0.1");
            metadata.First(m => m.moduleName == "samples/array-loop").tags.Should().Contain("1.0.2");
        }

        [TestMethod]
        public async Task GetModules_NoPropertiesDictionary()
        {
            PublicRegistryModuleMetadataProvider provider = new(ModuleIndex, true);
            (await provider.TryUpdateCacheAsync()).Should().BeTrue();
            var modules = await provider.GetModules();
            modules.Should().HaveCount(29);
            var m = modules.Should().Contain(m => m.Name == "app/dapr-containerapp").Which;
            m.Description.Should().BeNull();
            m.DocumentationUri.Should().Be("https://github.com/Azure/bicep-registry-modules/tree/app/dapr-containerapp/1.0.2/modules/app/dapr-containerapp/README.md");
        }

        [TestMethod]
        public async Task GetModules_PropertiesDictionaryHasNoEntries()
        {
            PublicRegistryModuleMetadataProvider provider = new(ModuleIndex, true);
            (await provider.TryUpdateCacheAsync()).Should().BeTrue();
            var modules = await provider.GetModules();
            modules.Should().HaveCount(29);
            var m = modules.Should().Contain(m => m.Name == "app/dapr-containerapps-environment").Which;
            m.Description.Should().BeNull();
            m.DocumentationUri.Should().Be("https://github.com/Azure/bicep-registry-modules/tree/app/dapr-containerapps-environment/1.2.2/modules/app/dapr-containerapps-environment/README.md");
        }

        [TestMethod]
        public async Task GetModules_ModuleHasNoDescription()
        {
            PublicRegistryModuleMetadataProvider provider = new(ModuleIndex, true);
            (await provider.TryUpdateCacheAsync()).Should().BeTrue();
            var modules = await provider.GetModules();
            modules.Should().HaveCount(29);
            var m = modules.Should().Contain(m => m.Name == "security/keyvault").Which;
            m.Description.Should().BeNull();
            m.DocumentationUri.Should().Be("https://github.com/Azure/bicep-registry-modules/tree/security/keyvault/1.0.1/modules/security/keyvault/README.md");
        }

        [TestMethod]
        public async Task GetModules_OnlyFirstTagHasDescription()
        {
            PublicRegistryModuleMetadataProvider provider = new(ModuleIndex, true);
            (await provider.TryUpdateCacheAsync()).Should().BeTrue();
            var modules = await provider.GetModules();
            modules.Should().HaveCount(29);
            var m = modules.Should().Contain(m => m.Name == "samples/array-loop").Which;
            m.Description.Should().Be("Description for 1.0.1");
            m.DocumentationUri.Should().Be("https://github.com/Azure/bicep-registry-modules/tree/samples/array-loop/1.0.2/modules/samples/array-loop/README.md");
        }

        [TestMethod]
        public async Task GetModules_OnlyLastTagHasDescription()
        {
            PublicRegistryModuleMetadataProvider provider = new(ModuleIndex, true);
            (await provider.TryUpdateCacheAsync()).Should().BeTrue();
            var modules = await provider.GetModules();
            modules.Should().HaveCount(29);
            var m = modules.Should().Contain(m => m.Name == "samples/hello-world").Which;
            m.Description.Should().Be("A \"שָׁלוֹם עוֹלָם\" sample Bicep registry module");
            m.DocumentationUri.Should().Be("https://github.com/Azure/bicep-registry-modules/tree/samples/hello-world/1.0.3/modules/samples/hello-world/README.md");
        }

        [TestMethod]
        public async Task GetModules_MultipleTagsHaveDescriptions()
        {
            PublicRegistryModuleMetadataProvider provider = new(ModuleIndex, true);
            (await provider.TryUpdateCacheAsync()).Should().BeTrue();
            var modules = await provider.GetModules();
            modules.Should().HaveCount(29);
            var m = modules.Should().Contain(m => m.Name == "lz/sub-vending").Which;
            m.Description.Should().Be("1.3.1: These are the input parameters for the Bicep module: [`main.bicep`](./main.bicep)\n\nThis is the orchestration module that is used and called by a consumer of the module to deploy a Landing Zone Subscription and its associated resources, based on the parameter input values that are provided to it at deployment time.");
            m.DocumentationUri.Should().Be("https://github.com/Azure/bicep-registry-modules/tree/lz/sub-vending/1.3.1/modules/lz/sub-vending/README.md");
        }
    }
}
