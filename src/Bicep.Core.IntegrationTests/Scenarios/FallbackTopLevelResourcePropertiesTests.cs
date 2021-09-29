// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace Bicep.Core.IntegrationTests.Scenarios
{
    /// <summary>
    /// https://github.com/Azure/bicep/issues/3000
    /// </summary>
    [TestClass]
    public class FallbackTopLevelResourcePropertiesTests
    {
        private static readonly RootConfiguration configuration = BicepTestConstants.BuiltInConfigurationWithAnalyzersDisabled;

        private static Compilation CreateCompilation(string program) => new(
            BuiltInTestTypes.Create(),
            SourceFileGroupingFactory.CreateFromText(program, new Mock<IFileResolver>(MockBehavior.Strict).Object),
            configuration);

        public static IEnumerable<object[]> FallbackProperties
        {
            get
            {
                yield return new object[] { "sku", "{}" };
                yield return new object[] { "kind", "''" };
                yield return new object[] { "managedBy", "''" };
                yield return new object[] { "managedByExtended", "[]" };
                yield return new object[] { "extendedLocation", "{}" };
                yield return new object[] { "zones", "[]" };
                yield return new object[] { "plan", "{}" };
                yield return new object[] { "eTag", "''" };
                yield return new object[] { "scale", "{}" };
                yield return new object[] { "identity", "{}" };
            }
        }

        [DynamicData(nameof(FallbackProperties))]
        [DataTestMethod]
        public void FallbackProperty_ShouldShowWarningDiagnostics1_WhenNotDefinedInType(string property, string value)
        {

            // Missing top-level properties - should be an error
            var compilation = CreateCompilation(@"
resource fallbackProperty 'Test.Rp/readWriteTests@2020-01-01' = {
  name: 'fallbackProperty'
  properties: {
    required: 'required'
  }
  " + $"{property}: {value}" + @"
}
");
            compilation.Should().HaveDiagnostics(new[] {
                ("BCP187", DiagnosticLevel.Warning, $"The property \"{property}\" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
            });
        }

        [DynamicData(nameof(FallbackProperties))]
        [DataTestMethod]
        public void FallbackProperty_ShouldShowWarningDiagnostics2_WhenNotDefinedInType(string property, string value)
        {

            // Missing top-level properties - should be an error
            var compilation = CreateCompilation(@"
resource fallbackProperty 'Test.Rp/readWriteTests@2020-01-01' = {
  name: 'fallbackProperty'
  properties: {
    required: 'required'  
  " + $"{property}: {value}" + @"
  }
}
");
            compilation.Should().HaveDiagnostics(new[] {
                ("BCP037", DiagnosticLevel.Warning, $"The property \"{property}\" is not allowed on objects of type \"Properties\". Permissible properties include \"readwrite\", \"writeonly\". If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
            });
        }

        [DynamicData(nameof(FallbackProperties))]
        [DataTestMethod]
        public void FallbackProperty_ShouldShowWarningDiagnostics3_WhenNotDefinedInType(string property, string value)
        {

            // Missing top-level properties - should be an error
            var compilation = CreateCompilation(@"
var props = {
  required: 'required'  
  " + $"{property}: {value}" + @"
}

resource fallbackProperty 'Test.Rp/readWriteTests@2020-01-01' = {
  name: 'fallbackProperty'
  properties: props
}
");
            compilation.Should().HaveDiagnostics(new[] {
                ("BCP037", DiagnosticLevel.Warning, $"The property \"{property}\" from source declaration \"props\" is not allowed on objects of type \"Properties\". Permissible properties include \"readwrite\", \"writeonly\". If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
            });
        }

        [DynamicData(nameof(FallbackProperties))]
        [DataTestMethod]
        public void FallbackProperty_ShouldShowWarning_WhenIsRead(string property, string value)
        {

            // Missing top-level properties - should be an error
            var compilation = CreateCompilation(@"
resource fallbackProperty 'Test.Rp/readWriteTests@2020-01-01' = {
  name: 'fallbackProperty'
  properties: {
    required: 'required'    
  }
}

var value = fallbackProperty." + property + @"
");
            compilation.Should().HaveDiagnostics(new[] {
                ("BCP187", DiagnosticLevel.Warning, $"The property \"{property}\" does not exist in the resource definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
            });
        }

        [DynamicData(nameof(FallbackProperties))]
        [DataTestMethod]
        public void FallbackProperty_ShouldNotShowWarning_WhenDefinedInType(string property, string value)
        {

            // Missing top-level properties - should be an error
            var compilation = CreateCompilation(@"
resource fallbackProperty 'Test.Rp/fallbackProperties@2020-01-01' = {
  name: 'fallbackProperty'
  properties: {
    required: 'required'
  }
  " + $"{property}: {value}" + @"
}
");
            compilation.Should().NotHaveAnyDiagnostics();
        }

        [DynamicData(nameof(FallbackProperties))]
        [DataTestMethod]
        public void FallbackProperty_ShouldShowError_WhenUsedOnModule(string property, string value)
        {

            var mainUri = new Uri("file:///main.bicep");
            var moduleAUri = new Uri("file:///modulea.bicep");

            var files = new Dictionary<Uri, string>
            {
                [mainUri] = @"
param inputa string
param inputb string

module modulea 'modulea.bicep' = {
  name: 'modulea'
  params: {
    inputa: inputa
    inputb: inputb
  }
  " + $"{property}: {value}" + @"
}

output outputa string = modulea.outputs.outputa
",
                [moduleAUri] = @"
param inputa string
param inputb string

output outputa string = '${inputa}-${inputb}'
",
            };

            var compilation = new Compilation(BuiltInTestTypes.Create(), SourceFileGroupingFactory.CreateForFiles(files, mainUri, BicepTestConstants.FileResolver), configuration);

            compilation.Should().HaveDiagnostics(new[] {
                ("BCP037", DiagnosticLevel.Error, $"The property \"{property}\" is not allowed on objects of type \"module\". Permissible properties include \"dependsOn\", \"scope\".")
            });
        }

        [DynamicData(nameof(FallbackProperties))]
        [DataTestMethod]
        public void FallbackProperty_ShouldShowError_WhenUsedOnModuleParams(string property, string value)
        {

            var mainUri = new Uri("file:///main.bicep");
            var moduleAUri = new Uri("file:///modulea.bicep");

            var files = new Dictionary<Uri, string>
            {
                [mainUri] = @"
param inputa string
param inputb string

module modulea 'modulea.bicep' = {
  name: 'modulea'
  params: {
    inputa: inputa
    inputb: inputb
    " + $"{property}: {value}" + @"
  }
}

output outputa string = modulea.outputs.outputa
",
                [moduleAUri] = @"
param inputa string
param inputb string
param inputc string = ''

output outputa string = '${inputa}-${inputb}'
",
            };

            var compilation = new Compilation(BuiltInTestTypes.Create(), SourceFileGroupingFactory.CreateForFiles(files, mainUri, BicepTestConstants.FileResolver), configuration);

            compilation.Should().HaveDiagnostics(new[] {
                ("BCP037", DiagnosticLevel.Error, $"The property \"{property}\" is not allowed on objects of type \"params\". Permissible properties include \"inputc\".")
            });
        }

        [DynamicData(nameof(FallbackProperties))]
        [DataTestMethod]
        public void FallbackProperty_ShouldShowError_WhenUsedOnModuleParams_ThroughVariable(string property, string value)
        {

            var mainUri = new Uri("file:///main.bicep");
            var moduleAUri = new Uri("file:///modulea.bicep");

            var files = new Dictionary<Uri, string>
            {
                [mainUri] = @"
param inputa string
param inputb string

var inputs = {
  inputa: inputa
  inputb: inputb
  " + $"{property}: {value}" + @"
}

module modulea 'modulea.bicep' = {
  name: 'modulea'
  params: inputs
}

output outputa string = modulea.outputs.outputa
",
                [moduleAUri] = @"
param inputa string
param inputb string
param inputc string = ''

output outputa string = '${inputa}-${inputb}'
",
            };

            var compilation = new Compilation(BuiltInTestTypes.Create(), SourceFileGroupingFactory.CreateForFiles(files, mainUri, BicepTestConstants.FileResolver), configuration);

            compilation.Should().HaveDiagnostics(new[] {                
                ("BCP037", DiagnosticLevel.Error, $"The property \"{property}\" from source declaration \"inputs\" is not allowed on objects of type \"params\". Permissible properties include \"inputc\"."),
                ("BCP183", DiagnosticLevel.Error, "The value of the module \"params\" property must be an object literal."),
            });
        }

        [DynamicData(nameof(FallbackProperties))]
        [DataTestMethod]
        public void FallbackProperty_ShouldShowError_WhenReadOnModule(string property, string value)
        {
            var mainUri = new Uri("file:///main.bicep");
            var moduleAUri = new Uri("file:///modulea.bicep");

            var files = new Dictionary<Uri, string>
            {
                [mainUri] = @"
param inputa string
param inputb string

module modulea 'modulea.bicep' = {
  name: 'modulea'
  params: {
    inputa: inputa
    inputb: inputb
  }  
}

var check = modulea." + property + @"
",
                [moduleAUri] = @"
param inputa string
param inputb string

output outputa string = '${inputa}-${inputb}'
",
            };

            var compilation = new Compilation(BuiltInTestTypes.Create(), SourceFileGroupingFactory.CreateForFiles(files, mainUri, BicepTestConstants.FileResolver), configuration);

            compilation.Should().HaveDiagnostics(new[] {
                ("BCP053", DiagnosticLevel.Error, $"The type \"module\" does not contain property \"{property}\". Available properties include \"name\", \"outputs\".")
            });
        }
    }
}
