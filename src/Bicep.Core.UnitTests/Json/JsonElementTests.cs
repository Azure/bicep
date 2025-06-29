// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Json;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Json.More;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.UnitTests.Json;

[TestClass]
public class JsonElementTests
{
    [TestMethod]
    public void Merge_can_operate_in_parallel()
    {
        // We were previously hitting an issue due to https://github.com/dotnet/runtime/issues/77421 which has been fixed in System.Text.Json >7.0.3.
        // This test just exists to avoid a regression.

        var source = @"{
  // This is the base configuration which provides the defaults for all values (end users don't see this file).
  ""cloud"": {
    ""currentProfile"": ""AzureCloud"",
    ""profiles"": {
      ""AzureCloud"": {
        ""resourceManagerEndpoint"": ""https://management.azure.com"",
        ""activeDirectoryAuthority"": ""https://login.microsoftonline.com""
      },
      ""AzureChinaCloud"": {
        ""resourceManagerEndpoint"": ""https://management.chinacloudapi.cn"",
        ""activeDirectoryAuthority"": ""https://login.chinacloudapi.cn""
      },
      ""AzureUSGovernment"": {
        ""resourceManagerEndpoint"": ""https://management.usgovcloudapi.net"",
        ""activeDirectoryAuthority"": ""https://login.microsoftonline.us""
      }
    },
    ""credentialPrecedence"": [
      ""AzureCLI"",
      ""AzurePowerShell""
    ]
  },
  ""moduleAliases"": {
    ""ts"": {},
    ""br"": {
      ""public"": {
        ""registry"": ""mcr.microsoft.com"",
        ""modulePath"": ""bicep""
      }
    }
  },
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-hardcoded-env-urls"": {
          ""level"": ""warning"",
          ""disallowedhosts"": [
            ""azuredatalakeanalytics.net"",
            ""azuredatalakestore.net"",
            ""batch.core.windows.net"",
            ""core.windows.net"",
            ""database.windows.net"",
            ""datalake.azure.net"",
            ""gallery.azure.com"",
            ""graph.windows.net"",
            ""login.microsoftonline.com"",
            ""management.azure.com"",
            ""management.core.windows.net"",
            ""vault.azure.net""
          ],
          ""excludedhosts"": [
            ""schema.management.azure.com""
          ]
        }
      }
    }
  },
  ""experimentalFeaturesEnabled"": {}
}";

        var target = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  }
}";
        var element = JsonElementFactory.CreateElement(target);
        var config = JsonElementFactory.CreateElement(source);

        Parallel.ForEach(Enumerable.Range(1, 1000000).ToList().AsParallel(), i =>
        {
            var result = config.Merge(element);
            JToken.Parse(result.ToJsonString()).Should().DeepEqual(JToken.Parse(@"{
  ""cloud"": {
    ""currentProfile"": ""AzureCloud"",
    ""profiles"": {
      ""AzureCloud"": {
        ""resourceManagerEndpoint"": ""https://management.azure.com"",
        ""activeDirectoryAuthority"": ""https://login.microsoftonline.com""
      },
      ""AzureChinaCloud"": {
        ""resourceManagerEndpoint"": ""https://management.chinacloudapi.cn"",
        ""activeDirectoryAuthority"": ""https://login.chinacloudapi.cn""
      },
      ""AzureUSGovernment"": {
        ""resourceManagerEndpoint"": ""https://management.usgovcloudapi.net"",
        ""activeDirectoryAuthority"": ""https://login.microsoftonline.us""
      }
    },
    ""credentialPrecedence"": [
      ""AzureCLI"",
      ""AzurePowerShell""
    ]
  },
  ""moduleAliases"": {
    ""ts"": {},
    ""br"": {
      ""public"": {
        ""registry"": ""mcr.microsoft.com"",
        ""modulePath"": ""bicep""
      }
    }
  },
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-hardcoded-env-urls"": {
          ""level"": ""warning"",
          ""disallowedhosts"": [
            ""azuredatalakeanalytics.net"",
            ""azuredatalakestore.net"",
            ""batch.core.windows.net"",
            ""core.windows.net"",
            ""database.windows.net"",
            ""datalake.azure.net"",
            ""gallery.azure.com"",
            ""graph.windows.net"",
            ""login.microsoftonline.com"",
            ""management.azure.com"",
            ""management.core.windows.net"",
            ""vault.azure.net""
          ],
          ""excludedhosts"": [
            ""schema.management.azure.com""
          ]
        },
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  },
  ""experimentalFeaturesEnabled"": {}
}"));
        });
    }
}
