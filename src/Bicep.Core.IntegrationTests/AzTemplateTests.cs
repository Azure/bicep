// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;

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
            
            // Assert the ARM template has the expected format
            var armTemplate = compilationResult.Template;
            armTemplate.Should().NotBeNull();
            
            // Check for the outputs
            var outputs = armTemplate!["outputs"] as JObject;
            outputs.Should().NotBeNull();
            
            // Verify each output uses the correct function
            var logicalZone = outputs!["logicalZone"]?["value"]?.ToString() ?? string.Empty;
            logicalZone.Should().Contain("toLogicalZone");
            
            var physicalZone = outputs["physicalZone"]?["value"]?.ToString() ?? string.Empty;
            physicalZone.Should().Contain("toPhysicalZone");
            
            var logicalZones = outputs["logicalZones"]?["value"]?.ToString() ?? string.Empty;
            logicalZones.Should().Contain("toLogicalZones");
            
            var physicalZones = outputs["physicalZones"]?["value"]?.ToString() ?? string.Empty;
            physicalZones.Should().Contain("toPhysicalZones");
            
            var emptyLogicalZones = outputs["emptyLogicalZones"]?["value"]?.ToString() ?? string.Empty;
            emptyLogicalZones.Should().Contain("toLogicalZones");
            
            var emptyPhysicalZones = outputs["emptyPhysicalZones"]?["value"]?.ToString() ?? string.Empty;
            emptyPhysicalZones.Should().Contain("toPhysicalZones");
        }
    }
}