// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Diagnostics;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Analyzers.Linter.Rules;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class DecoratorTests
    {
        [TestMethod]
        public void ParameterDecorator_MissingDeclaration_ExpectedParameterDeclaration()
        {
            var (template, diagnostics, _) = CompilationHelper.Compile(@"
@secure()
");
            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP147", DiagnosticLevel.Error, "Expected a parameter declaration after the decorator."),
                });
            }
        }

        [TestMethod]
        public void ParameterDecorator_AttachedToOtherKindsOfDeclarations_CannotBeUsedAsDecoratorSpecificToTheDeclarations()
        {
            var mainUri = new Uri("file:///main.bicep");
            var moduleUri = new Uri("file:///module.bicep");

            var files = new Dictionary<Uri, string>
            {
                [mainUri] = @"
@maxLength(1)
var foo = true

@allowed([
  true
])
resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
    location: 'westus'
    name: 'myVNet'
    properties:{
        addressSpace: {
            addressPrefixes: [
                '10.0.0.0/20'
            ]
        }
    }
}

@secure()
module myModule 'module.bicep' = {
  name: 'moduleb'
  params: {
    inputa: 'foo'
    inputb: 'bar'
  }
}

@minValue(2)
output bar bool = false
",
                [moduleUri] = @"
param inputa string
param inputb string
",
            };

            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), SourceFileGroupingFactory.CreateForFiles(files, mainUri, BicepTestConstants.FileResolver, BicepTestConstants.BuiltInConfiguration), BicepTestConstants.BuiltInConfiguration);
            var diagnosticsByFile = compilation.GetAllDiagnosticsByBicepFile().ToDictionary(kvp => kvp.Key.FileUri, kvp => kvp.Value);
            var success = diagnosticsByFile.Values.SelectMany(x => x).All(d => d.Level != DiagnosticLevel.Error);

            using (new AssertionScope())
            {
                diagnosticsByFile[mainUri].ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().HaveDiagnostics(new[] {
                    ("BCP126", DiagnosticLevel.Error, "Function \"maxLength\" cannot be used as a variable decorator."),
                    ("BCP127", DiagnosticLevel.Error, "Function \"allowed\" cannot be used as a resource decorator."),
                    ("BCP128", DiagnosticLevel.Error, "Function \"secure\" cannot be used as a module decorator."),
                    ("BCP129", DiagnosticLevel.Error, "Function \"minValue\" cannot be used as an output decorator."),
                });
                success.Should().BeFalse();
            }
        }

        [TestMethod]
        public void NonDecoratorFunction_MissingDeclaration_CannotBeUsedAsDecorator()
        {
            var (template, diagnostics, _) = CompilationHelper.Compile(@"
@concat()
@resourceId()
");
            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP152", DiagnosticLevel.Error, "Function \"concat\" cannot be used as a decorator."),
                    ("BCP132", DiagnosticLevel.Error, "Expected a declaration after the decorator."),
                    ("BCP152", DiagnosticLevel.Error, "Function \"resourceId\" cannot be used as a decorator.")
                });
            }
        }

        [TestMethod]
        public void NonDecoratorFunction_AttachedToDeclaration_CannotBeUsedAsDecorator()
        {
            var mainUri = new Uri("file:///main.bicep");
            var moduleUri = new Uri("file:///module.bicep");

            var files = new Dictionary<Uri, string>
            {
                [mainUri] = @"
@resourceId()
param foo string

@concat()
var bar = true

@environment()
resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
    location: 'westus'
    name: 'myVNet'
    properties:{
        addressSpace: {
            addressPrefixes: [
                '10.0.0.0/20'
            ]
        }
    }
}

@union()
module myModule 'module.bicep' = {
  name: 'moduleb'
  params: {
    inputa: 'foo'
    inputb: 'bar'
  }
}

@guid()
output baz bool = false
",
                [moduleUri] = @"
param inputa string
param inputb string
",
            };

            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), SourceFileGroupingFactory.CreateForFiles(files, mainUri, BicepTestConstants.FileResolver, BicepTestConstants.BuiltInConfiguration), BicepTestConstants.BuiltInConfiguration);
            var diagnosticsByFile = compilation.GetAllDiagnosticsByBicepFile().ToDictionary(kvp => kvp.Key.FileUri, kvp => kvp.Value);
            var success = diagnosticsByFile.Values.SelectMany(x => x).All(d => d.Level != DiagnosticLevel.Error);

            using (new AssertionScope())
            {
                diagnosticsByFile[mainUri].ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().HaveDiagnostics(new[] {
                    ("BCP152", DiagnosticLevel.Error, "Function \"resourceId\" cannot be used as a decorator."),
                    ("BCP152", DiagnosticLevel.Error, "Function \"concat\" cannot be used as a decorator."),
                    ("BCP152", DiagnosticLevel.Error, "Function \"environment\" cannot be used as a decorator."),
                    ("BCP152", DiagnosticLevel.Error, "Function \"union\" cannot be used as a decorator."),
                    ("BCP152", DiagnosticLevel.Error, "Function \"guid\" cannot be used as a decorator."),
                });
                success.Should().BeFalse();
            }
        }
    }
}


