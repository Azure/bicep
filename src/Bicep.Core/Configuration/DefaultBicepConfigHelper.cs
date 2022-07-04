// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Configuration
{
    public static class DefaultBicepConfigHelper
    {
        public const string DefaultRuleCode = "no-unused-params"; // Default rule that's added when creating an empty configuration file (as an example)
        public static string GetDefaultBicepConfig()
        {
            return @"{
  // See https://aka.ms/bicep/config for more information on Bicep configuration options
  // Press CTRL+SPACE/CMD+SPACE at any location to see Intellisense suggestions
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
