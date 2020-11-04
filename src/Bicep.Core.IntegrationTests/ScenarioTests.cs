// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.FileSystem;
using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Bicep.Core.UnitTests.Assertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions.Execution;
using FluentAssertions;
using System;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ScenarioTests
    {
        private static (string? jsonOutput, IEnumerable<Diagnostic> diagnostics) Compile(IReadOnlyDictionary<Uri, string> files, Uri entryFileUri)
        {
            var syntaxTreeGrouping = SyntaxFactory.CreateForFiles(files, entryFileUri);
            var compilation = new Compilation(new AzResourceTypeProvider(), syntaxTreeGrouping);
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());
            
            var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();

            string? jsonOutput = null;
            if (!compilation.GetEntrypointSemanticModel().HasErrors())
            {
                using var stream = new MemoryStream();
                var emitResult = emitter.Emit(stream);

                if (emitResult.Status != EmitStatus.Failed)
                {
                    stream.Position = 0;
                    jsonOutput = new StreamReader(stream).ReadToEnd();
                }
            }

            return (jsonOutput, diagnostics);
        }        

        private static void AssertFailureWithDiagnostics(string fileContents,  IEnumerable<(string code, DiagnosticLevel level, string message)> expectedDiagnostics)
        {
            var entryFileUri = new Uri("file:///main.bicep");

            AssertFailureWithDiagnostics(new Dictionary<Uri, string> { [entryFileUri] = fileContents }, entryFileUri, expectedDiagnostics);
        }

        private static void AssertFailureWithDiagnostics(IReadOnlyDictionary<Uri, string> files, Uri entryFileUri, IEnumerable<(string code, DiagnosticLevel level, string message)> expectedDiagnostics)
        {
            var (jsonOutput, diagnostics) = Compile(files, entryFileUri);
            using (new AssertionScope())
            {
                jsonOutput.Should().BeNull();
                diagnostics.Should().HaveDiagnostics(expectedDiagnostics);
            }
        }

        private static string AssertSuccessWithTemplateOutput(IReadOnlyDictionary<Uri, string> files, Uri entryFileUri)
        {
            var (jsonOutput, diagnostics) = Compile(files, entryFileUri);
            using (new AssertionScope())
            {
                jsonOutput.Should().NotBeNull();
                diagnostics.Should().BeEmpty();
            }

            return jsonOutput!;
        }

        [TestMethod]
        public void Test_Issue746()
        {
            var bicepContents = @"
var l = l
param l
";

            AssertFailureWithDiagnostics(
                bicepContents,
                new [] {
                    ("BCP028", DiagnosticLevel.Error, "Identifier \"l\" is declared multiple times. Remove or rename the duplicates."),
                    ("BCP079", DiagnosticLevel.Error, "This expression is referencing its own declaration, which is not allowed."),
                    ("BCP028", DiagnosticLevel.Error, "Identifier \"l\" is declared multiple times. Remove or rename the duplicates."),
                    ("BCP014", DiagnosticLevel.Error, "Expected a parameter type at this location. Please specify one of the following types: \"array\", \"bool\", \"int\", \"object\", \"string\"."),
                });
        }

        [TestMethod]
        public void Test_Issue801()
        {
            var files = new Dictionary<Uri, string>
            {
                [new Uri("file:///path/to/main.bicep")] = @"
targetScope = 'subscription'

resource rg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
    location: 'eastus'
    name: 'vnet-rg'
}

module vnet './vnet.bicep' = {
    scope: resourceGroup('vnet-rg')
    name: 'network-module'
    params: {
        location: 'eastus'
        name: 'myVnet'
    }
    dependsOn: [
        rg        
    ]  
}

output vnetid string = vnet.outputs.vnetId
output vnetstate string = vnet.outputs.vnetstate
",
                [new Uri("file:///path/to/vnet.bicep")] = @"
param location string
param name string

resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
    location: location
    name: name
    properties:{
        addressSpace: {
            addressPrefixes: [
                '10.0.0.0/20'
            ]
        }
        subnets: [
            {
                name: 'snet-apim'
                properties: {
                    addressPrefix: '10.0.0.0/24'
                }
            }
        ]
    }
}

output vnetId string = vnet.id
output vnetstate string = vnet.properties.provisioningState
",
            };

            var jsonOutput = AssertSuccessWithTemplateOutput(files, new Uri("file:///path/to/main.bicep"));

            // ensure we're generating the correct expression with 'subscriptionResourceId', and using the correct name for the module
            jsonOutput.Should().Contain("[reference(subscriptionResourceId('Microsoft.Resources/deployments', 'network-module'), '2019-10-01').outputs.vnetId.value]");
            jsonOutput.Should().Contain("[reference(subscriptionResourceId('Microsoft.Resources/deployments', 'network-module'), '2019-10-01').outputs.vnetstate.value]");
        }
    }
}