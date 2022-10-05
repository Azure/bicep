// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter;
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
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.IntegrationTests.Scenarios
{

    [TestClass]
    public class TopLevelResourcePropertiesTests
    {
        private static ServiceBuilder Services => new ServiceBuilder().WithAzResources(BuiltInTestTypes.Types).WithDisabledAnalyzersConfiguration();

        /// <summary>
        /// https://github.com/Azure/bicep/issues/3000
        /// </summary>
        public static IEnumerable<object[]> FallbackProperties
        {
            get
            {
                yield return new object[] { "sku", "{}" };
                yield return new object[] { "kind", "''" };
                yield return new object[] { "managedBy", "''" };
                yield return new object[] { "managedByExtended", "[]" };
                yield return new object[] { "extendedLocation", "{'type': 'NotSpecified'}" };
                yield return new object[] { "zones", "[]" };
                yield return new object[] { "plan", "{}" };
                yield return new object[] { "eTag", "''" };
                yield return new object[] { "scale", "{'capacity': 1}" };
                yield return new object[] { "identity", "{'type': 'NotSpecified'}" };
            }
        }

        [DynamicData(nameof(FallbackProperties))]
        [DataTestMethod]
        public void FallbackProperty_ShouldShowWarningDiagnostics1_WhenNotDefinedInType(string property, string value)
        {

            // Missing top-level properties - should be an error
            var compilation = Services.BuildCompilation(@"
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
            var compilation = Services.BuildCompilation(@"
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
            var compilation = Services.BuildCompilation(@"
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
            var compilation = Services.BuildCompilation(@"
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
            var compilation = Services.BuildCompilation(@"
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

            var compilation = Services.WithAzResources(BuiltInTestTypes.Types).BuildCompilation(files, mainUri);

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

            var compilation = Services.WithAzResources(BuiltInTestTypes.Types).BuildCompilation(files, mainUri);

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

            var compilation = Services.WithAzResources(BuiltInTestTypes.Types).BuildCompilation(files, mainUri);

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

            var compilation = Services.WithAzResources(BuiltInTestTypes.Types).BuildCompilation(files, mainUri);

            compilation.Should().HaveDiagnostics(new[] {
                ("BCP053", DiagnosticLevel.Error, $"The type \"module\" does not contain property \"{property}\". Available properties include \"name\", \"outputs\".")
            });
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/5960
        /// </summary>
        [TestMethod]
        public void Test_Issue5960_case1()
        {
            var typeReference = ResourceTypeReference.Parse("My.Rp/myResource@2020-01-01");
            var typeLoader = TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(new[] {
                new ResourceTypeComponents(typeReference, ResourceScope.ResourceGroup, ResourceScope.None, ResourceFlags.None, new ObjectType(typeReference.FormatName(), TypeSymbolValidationFlags.Default, new [] {
                    new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.DeployTimeConstant | TypePropertyFlags.SystemProperty, "name property"),
                    new TypeProperty("required", LanguageConstants.String, TypePropertyFlags.Required, "required property"),
                    new TypeProperty("systemRequired", LanguageConstants.String, TypePropertyFlags.SystemProperty | TypePropertyFlags.Required, "system required property")
                }, null))
            });

            // explicitly pass a valid scope
            var result = CompilationHelper.Compile(typeLoader, ("main.bicep", @"
resource resourceA 'My.Rp/myResource@2020-01-01' = {
  name: 'resourceA'
  systemRequired: 'value'
}
"));
            result.Should().GenerateATemplate().And.HaveDiagnostics(new[]
            {
                ("BCP035", DiagnosticLevel.Warning, "The specified \"resource\" declaration is missing the following required properties: \"required\". If this is an inaccuracy in the documentation, please report it to the Bicep Team.")
            });
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/5960
        /// </summary>
        [TestMethod]
        public void Test_Issue5960_case2()
        {
            var typeReference = ResourceTypeReference.Parse("My.Rp/myResource@2020-01-01");
            var typeLoader = TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(new[] {
                new ResourceTypeComponents(typeReference, ResourceScope.ResourceGroup, ResourceScope.None, ResourceFlags.None, new ObjectType(typeReference.FormatName(), TypeSymbolValidationFlags.Default, new [] {
                    new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.DeployTimeConstant | TypePropertyFlags.SystemProperty, "name property"),
                    new TypeProperty("required", LanguageConstants.String, TypePropertyFlags.Required, "required property"),
                    new TypeProperty("systemRequired", LanguageConstants.String, TypePropertyFlags.SystemProperty | TypePropertyFlags.Required, "system required property")
                }, null))
            });

            // explicitly pass a valid scope
            var result = CompilationHelper.Compile(typeLoader, ("main.bicep", @"
resource resourceA 'My.Rp/myResource@2020-01-01' = {
  name: 'resourceA'
  required: 'value'
}
"));
            result.Should().NotGenerateATemplate().And.HaveDiagnostics(new[]
            {
                ("BCP035", DiagnosticLevel.Error, "The specified \"resource\" declaration is missing the following required properties: \"systemRequired\".")
            });
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/5960
        /// </summary>
        [TestMethod]
        public void Test_Issue5960_case3()
        {
            var typeReference = ResourceTypeReference.Parse("My.Rp/myResource@2020-01-01");
            var typeLoader = TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(new[] {
                new ResourceTypeComponents(typeReference, ResourceScope.ResourceGroup, ResourceScope.None, ResourceFlags.None, new ObjectType(typeReference.FormatName(), TypeSymbolValidationFlags.Default, new [] {
                    new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.DeployTimeConstant | TypePropertyFlags.SystemProperty, "name property"),
                    new TypeProperty("required", LanguageConstants.String, TypePropertyFlags.Required, "required property"),
                    new TypeProperty("systemRequired", LanguageConstants.String, TypePropertyFlags.SystemProperty | TypePropertyFlags.Required, "system required property")
                }, null))
            });

            // explicitly pass a valid scope
            var result = CompilationHelper.Compile(typeLoader, ("main.bicep", @"
resource resourceA 'My.Rp/myResource@2020-01-01' = {
  name: 'resourceA'
}
"));
            result.Should().NotGenerateATemplate().And.HaveDiagnostics(new[]
            {
                ("BCP035", DiagnosticLevel.Error, "The specified \"resource\" declaration is missing the following required properties: \"required\", \"systemRequired\". If this is an inaccuracy in the documentation, please report it to the Bicep Team.")
            });
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/5960
        /// </summary>
        [TestMethod]
        public void Test_Issue5960_case4()
        {
            var typeReference = ResourceTypeReference.Parse("My.Rp/myResource@2020-01-01");
            var typeLoader = TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(new[] {
                new ResourceTypeComponents(typeReference, ResourceScope.ResourceGroup, ResourceScope.None, ResourceFlags.None, new ObjectType(typeReference.FormatName(), TypeSymbolValidationFlags.Default, new [] {
                    new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.DeployTimeConstant | TypePropertyFlags.SystemProperty, "name property"),
                    new TypeProperty("systemRequired", LanguageConstants.String, TypePropertyFlags.SystemProperty | TypePropertyFlags.Required, "system required property")
                }, null))
            });

            // explicitly pass a valid scope
            var result = CompilationHelper.Compile(typeLoader, ("main.bicep", @"
resource resourceA 'My.Rp/myResource@2020-01-01' = {
  name: 'name'
}
"));
            result.Should().NotGenerateATemplate().And.HaveDiagnostics(new[]
            {
                ("BCP035", DiagnosticLevel.Error, "The specified \"resource\" declaration is missing the following required properties: \"systemRequired\".")
            });
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/5960
        /// </summary>
        [TestMethod]
        public void Test_Issue5960_case5()
        {
            // explicitly pass a valid scope
            var result = CompilationHelper.Compile(("main.bicep", @"
module mod 'mod.bicep' = {
}
"), ("mod.bicep", ""));
            result.Should().NotGenerateATemplate().And.HaveDiagnostics(new[]
            {
                ("BCP035", DiagnosticLevel.Error, "The specified \"module\" declaration is missing the following required properties: \"name\".")
            });
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/5960
        /// </summary>
        [TestMethod]
        public void Test_Issue5960_case6()
        {
            var typeReference = ResourceTypeReference.Parse("My.Rp/myResource@2020-01-01");
            var typeLoader = TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(new[] {
                new ResourceTypeComponents(typeReference, ResourceScope.ResourceGroup, ResourceScope.None, ResourceFlags.None, new ObjectType(typeReference.FormatName(), TypeSymbolValidationFlags.Default, new [] {
                    new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.DeployTimeConstant | TypePropertyFlags.SystemProperty, "name property"),
                    new TypeProperty("required", LanguageConstants.Object, TypePropertyFlags.Required, "required property"),
                }, null))
            });

            // explicitly pass a valid scope
            var result = CompilationHelper.Compile(typeLoader, ("main.bicep", @"
param str string
resource resourceA 'My.Rp/myResource@2020-01-01' = {
    name: 'name'
    required: str
}
"));
            result.Should().GenerateATemplate().And.HaveDiagnostics(new[]
            {
                ("BCP036", DiagnosticLevel.Warning, "The property \"required\" expected a value of type \"object\" but the provided value is of type \"string\". If this is an inaccuracy in the documentation, please report it to the Bicep Team.")
            });
        }
        /// <summary>
        /// https://github.com/Azure/bicep/issues/5960
        /// </summary>
        [TestMethod]
        public void Test_Issue5960_case7()
        {
            var typeReference = ResourceTypeReference.Parse("My.Rp/myResource@2020-01-01");
            var typeLoader = TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(new[] {
                new ResourceTypeComponents(typeReference, ResourceScope.ResourceGroup, ResourceScope.None, ResourceFlags.None, new ObjectType(typeReference.FormatName(), TypeSymbolValidationFlags.Default, new [] {
                    new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.DeployTimeConstant | TypePropertyFlags.SystemProperty, "name property"),
                    new TypeProperty("systemRequired", LanguageConstants.Object, TypePropertyFlags.Required | TypePropertyFlags.SystemProperty, "system required property"),
                }, null))
            });

            // explicitly pass a valid scope
            var result = CompilationHelper.Compile(typeLoader, ("main.bicep", @"
param str string
resource resourceA 'My.Rp/myResource@2020-01-01' = {
    name: 'name'
    systemRequired: str
}
"));
            result.Should().NotGenerateATemplate().And.HaveDiagnostics(new[]
            {
                ("BCP036", DiagnosticLevel.Error, "The property \"systemRequired\" expected a value of type \"object\" but the provided value is of type \"string\".")
            });
        }
        /// <summary>
        /// https://github.com/Azure/bicep/issues/5960
        /// </summary>
        [TestMethod]
        public void Test_Issue5960_case8()
        {
            var typeReference = ResourceTypeReference.Parse("My.Rp/myResource@2020-01-01");
            var typeLoader = TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(new[] {
                new ResourceTypeComponents(typeReference, ResourceScope.ResourceGroup, ResourceScope.None, ResourceFlags.None, new ObjectType(typeReference.FormatName(), TypeSymbolValidationFlags.Default, new [] {
                    new TypeProperty("name", LanguageConstants.String, TypePropertyFlags.DeployTimeConstant | TypePropertyFlags.SystemProperty, "name property"),
                    new TypeProperty("systemRequired", LanguageConstants.Object, TypePropertyFlags.Required | TypePropertyFlags.SystemProperty, "system required property"),
                    new TypeProperty("required", LanguageConstants.Object, TypePropertyFlags.Required, "required property"),
                }, null))
            });

            // explicitly pass a valid scope
            var result = CompilationHelper.Compile(typeLoader, ("main.bicep", @"
param str string
resource resourceA 'My.Rp/myResource@2020-01-01' = {
    name: 'name'
    systemRequired: str
    required: str
}
"));
            result.Should().NotGenerateATemplate().And.HaveDiagnostics(new[]
            {
                ("BCP036", DiagnosticLevel.Error, "The property \"systemRequired\" expected a value of type \"object\" but the provided value is of type \"string\"."),
                ("BCP036", DiagnosticLevel.Warning, "The property \"required\" expected a value of type \"object\" but the provided value is of type \"string\". If this is an inaccuracy in the documentation, please report it to the Bicep Team.")
            });
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/5960
        /// </summary>
        [TestMethod]
        public void Test_Issue5960_case9()
        {
            var customTypes = new[] {
                new ResourceTypeComponents(
                    ResourceTypeReference.Parse("My.Rp/myResource@2020-01-01"),
                    ResourceScope.ResourceGroup,
                    ResourceScope.None,
                    ResourceFlags.None,
                    TestTypeHelper.CreateDiscriminatedObjectType(
                        "My.Rp/myResource@2020-01-01",
                        "kind",
                        TestTypeHelper.CreateObjectType(
                            "Val1Type",
                            ("name", LanguageConstants.String),
                            ("kind", new StringLiteralType("val1")),
                            ("properties", TestTypeHelper.CreateObjectType(
                                "properties",
                                ("onlyOnVal1", LanguageConstants.Bool)))),
                        TestTypeHelper.CreateObjectType(
                            "Val2Type",
                            ("name", LanguageConstants.String),
                            ("kind", new StringLiteralType("val2")),
                            ("properties", TestTypeHelper.CreateObjectType(
                                "properties",
                                ("onlyOnVal2", LanguageConstants.Bool)))))),
            };

            var typeLoader = TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(customTypes);

            // explicitly pass a valid scope
            var result = CompilationHelper.Compile(typeLoader, ("main.bicep", @"
resource res 'My.Rp/myResource@2020-01-01' = {
  name: 'name'
  kind: 'otherValue'
}
"));
            result.Should().GenerateATemplate().And.HaveDiagnostics(new[]
            {
                ("BCP036", DiagnosticLevel.Warning, "The property \"kind\" expected a value of type \"'val1' | 'val2'\" but the provided value is of type \"'otherValue'\". If this is an inaccuracy in the documentation, please report it to the Bicep Team.")
            });
        }
    }
}
