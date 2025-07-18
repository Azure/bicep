// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.Core;
using Bicep.Core.FileSystem;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Utils;
using Bicep.Decompiler;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Decompiler.IntegrationTests
{
    [TestClass]
    public class ParamsDecompilationTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static BicepDecompiler CreateDecompiler(IFileResolver fileResolver)
          => ServiceBuilder.Create(s => s.WithFileResolver(fileResolver)).GetDecompiler();

        [TestMethod]
        public void Decompiler_Decompiles_ValidParametersFile()
        {
            var jsonParametersFile =
                """
                {
                  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
                  "contentVersion": "1.0.0.0",
                  "parameters": {
                    "first": {
                      "value": "test"
                    },
                    "second": {
                      "value": 1
                    },
                    "third" : {
                      "value" : [
                        1,
                        "foo"
                      ]
                    },
                    "fourth" : {
                      "value" : {
                        "firstKey" : "bar",
                        "secondKey" : 1
                      }
                    }
                  }
                }
                """;
            var expectedBicepparamFile =
                """
                using '' /*TODO: Provide a path to a bicep template*/

                param first = 'test'

                param second = 1

                param third = [
                  1
                  'foo'
                ]

                param fourth = {
                  firstKey: 'bar'
                  secondKey: 1
                }

                """;

            var paramFileUri = new Uri("file:///path/to/main.json");

            var fileResolver = new InMemoryFileResolver(new Dictionary<Uri, string>
            {
                [paramFileUri] = jsonParametersFile
            });

            var decompiler = CreateDecompiler(fileResolver);

            var (entryPointUri, filesToSave) = decompiler.DecompileParameters(
                jsonParametersFile,
                PathHelper.ChangeExtension(paramFileUri, LanguageConstants.ParamsFileExtension), null);

            filesToSave[entryPointUri].Should().BeEquivalentToIgnoringNewlines(expectedBicepparamFile);
        }

        [TestMethod]
        public void Decompiler_Decompiles_ValidParamsFileWithBicepFilePath()
        {
            var jsonParametersFile =
                """
                {
                  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
                  "contentVersion": "1.0.0.0",
                  "parameters": {
                    "first": {
                      "value": "test"
                    },
                    "second": {
                      "value": 1
                    },
                    "third" : {
                      "value" : true
                    },
                  }
                }
                """;
            var expectedBicepparamFile =
                """
                using 'dir/main.bicep'

                param first = 'test'

                param second = 1

                param third = true

                """;

            var paramFileUri = new Uri("file:///path/to/main.json");
            var bicepFileUri = new Uri("file:///path/to/dir/main.bicep");

            var fileResolver = new InMemoryFileResolver(new Dictionary<Uri, string>
            {
                [paramFileUri] = jsonParametersFile
            });

            var decompiler = CreateDecompiler(fileResolver);

            var (entryPointUri, filesToSave) = decompiler.DecompileParameters(
              jsonParametersFile,
              PathHelper.ChangeExtension(paramFileUri, LanguageConstants.ParamsFileExtension),
              bicepFileUri);

            filesToSave[entryPointUri].Should().BeEquivalentToIgnoringNewlines(expectedBicepparamFile);
        }

        [TestMethod]
        public void Decompiler_Decompiles_KeyVaultReferenceParameters()
        {
            var jsonParametersFile =
                """
                {
                  "$schema": "https://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#",
                  "contentVersion": "1.0.0.0",
                  "parameters": {
                    "adminUsername": {
                      "value": "tim"
                    },
                    "adminPassword": {
                      "reference": {
                        "keyVault": {
                          "id": "/subscriptions/2fbf906e-1101-4bc0-b64f-adc44e462fff/resourceGroups/INSTRUCTOR/providers/Microsoft.KeyVault/vaults/TimKV"
                        },
                        "secretName": "vm-password",
                        "secretVersion": "1.0"
                      }
                    },

                    "dnsLabelPrefix": {
                      "value": "newvm79347a"
                    }
                  }
                }
                """;
            var expectedBicepparamFile =
                """
                using '' /*TODO: Provide a path to a bicep template*/

                param adminUsername = 'tim'

                param adminPassword = az.getSecret('2fbf906e-1101-4bc0-b64f-adc44e462fff', 'INSTRUCTOR', 'TimKV', 'vm-password', '1.0')

                param dnsLabelPrefix = 'newvm79347a'

                """;

            var paramFileUri = new Uri("file:///path/to/main.json");

            var fileResolver = new InMemoryFileResolver(new Dictionary<Uri, string>
            {
                [paramFileUri] = jsonParametersFile
            });

            var decompiler = CreateDecompiler(fileResolver);

            var (entryPointUri, filesToSave) = decompiler.DecompileParameters(
              jsonParametersFile,
              PathHelper.ChangeExtension(paramFileUri, LanguageConstants.ParamsFileExtension),
              null);

            filesToSave[entryPointUri].Should().Be(expectedBicepparamFile);
        }

        [TestMethod]
        public void Decompiler_Decompiles_ParametersContainingMetadata()
        {
            var jsonParametersFile =
                """
                {
                  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
                  "contentVersion": "1.0.0.0",
                  "parameters": {
                    "regions": {
                      "metadata": {
                          "description": "List of permitted regions",
                          "displayName": "List of regions"
                      },
                      "value": [
                          "North Europe",
                          "West Europe"
                      ]
                    }
                  }
                }
                """;

            var expectedBicepparamFile =
                """
                using '' /*TODO: Provide a path to a bicep template*/
    
                /*
                Parameter metadata is not supported in Bicep Parameters files
                
                Following metadata was not decompiled:
                {
                  "description": "List of permitted regions",
                  "displayName": "List of regions"
                }
                */
                param regions = [
                  'North Europe'
                  'West Europe'
                ]
                
                """;

            var paramFileUri = new Uri("file:///path/to/main.json");

            var fileResolver = new InMemoryFileResolver(new Dictionary<Uri, string>
            {
                [paramFileUri] = jsonParametersFile
            });

            var decompiler = CreateDecompiler(fileResolver);

            var (entryPointUri, filesToSave) = decompiler.DecompileParameters(
              jsonParametersFile,
              PathHelper.ChangeExtension(paramFileUri, LanguageConstants.ParamsFileExtension),
              null);

            filesToSave[entryPointUri].Should().BeEquivalentToIgnoringNewlines(expectedBicepparamFile);
        }
    }
}
