// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Configuration
{
    public static class DefaultBicepConfigHelper
    {
        public static string GetDefaultBicepConfig()
        {
            return @"{
  ""analyzers"": {
    ""core"": {
      ""rules"": {
      }
    }
  }
}";
        }
    }
}
