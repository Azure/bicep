// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class AzTemplateTests
    {
        [TestMethod]
        public void Test_Az_Zone_Functions()
        {
            var bicepFile = @"
                            param subscriptionId string = subscription().subscriptionId
                            param location string = 'eastus'

                            output logicalZone string = az.toLogicalZone(subscriptionId, location, 'eastus-az1')
                            output physicalZone string = az.toPhysicalZone(subscriptionId, location, '1')
                            output logicalZones array = az.toLogicalZones(subscriptionId, location, ['eastus-az1', 'eastus-az2'])
                            output physicalZones array = az.toPhysicalZones(subscriptionId, location, ['1', '2'])
                            output emptyLogicalZones array = az.toLogicalZones(subscriptionId, location, [])
                            output emptyPhysicalZones array = az.toPhysicalZones(subscriptionId, location, [])
                            ";

            var compilationResult = CompilationHelper.Compile(bicepFile);
            compilationResult.Diagnostics.Should().BeEmpty();

            var armTemplate = compilationResult.Template;
            armTemplate.Should().NotBeNull();
            var outputs = armTemplate!["outputs"] as JObject;
            outputs.Should().NotBeNull();

            // Define expected function names and output mappings
            var zoneFunctionTests = new Dictionary<string, string>
            {
                { "logicalZone", "toLogicalZone" },
                { "physicalZone", "toPhysicalZone" },
                { "logicalZones", "toLogicalZones" },
                { "physicalZones", "toPhysicalZones" },
                { "emptyLogicalZones", "toLogicalZones" },
                { "emptyPhysicalZones", "toPhysicalZones" }
            };

            foreach (var test in zoneFunctionTests)
            {
                VerifyOutputContainsFunction(outputs!, test.Key, test.Value);
            }
        }

        private static void VerifyOutputContainsFunction(JObject outputs, string outputName, string expectedFunction)
        {
            outputs.Should().ContainKey(outputName, $"Output '{outputName}' should exist in template");
            var output = outputs[outputName];
            output.Should().NotBeNull($"Output '{outputName}' should not be null");

            var valueToken = output!["value"];
            valueToken.Should().NotBeNull($"Output '{outputName}' should have a value property");

            var valueString = valueToken!.ToString();
            valueString.Should().Contain(expectedFunction,
                $"Output '{outputName}' should contain function '{expectedFunction}'");
        }
    }
}
