// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Features;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class CompileTimeImportTests
{
    private ServiceBuilder ServicesWithCompileTimeTypeImports => new ServiceBuilder()
        .WithFeatureOverrides(new(TestContext, CompileTimeImportsEnabled: true));

    private ServiceBuilder ServicesWithCompileTimeTypeImportsAndUserDefinedFunctions => new ServiceBuilder()
        .WithFeatureOverrides(new(TestContext, CompileTimeImportsEnabled: true, UserDefinedFunctionsEnabled: true));

    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public void Compile_time_imports_are_disabled_unless_feature_is_enabled()
    {
        var result = CompilationHelper.Compile("""
            import * as foo from 'bar.bicep'
            """);

        result.Should().HaveDiagnostics(new[]
        {
            ("BCP357", DiagnosticLevel.Error, "Using compile-time import statements requires enabling EXPERIMENTAL feature \"CompileTimeImports\"."),
        });
    }

    [TestMethod]
    public void Importing_unexported_symbol_should_raise_diagnostic()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {foo, bar} from 'mod.bicep'
                """),
            ("mod.bicep", """
                type foo = string[]
                var bar = 'bar'
                """));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP360", DiagnosticLevel.Error, "The 'foo' symbol was not found in (or was not exported by) the imported template."),
            ("BCP360", DiagnosticLevel.Error, "The 'bar' symbol was not found in (or was not exported by) the imported template."),
        });
    }

    [TestMethod]
    public void Imported_variable_cannot_be_used_in_a_type_expression()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {bar} from 'mod.bicep'

                type foo = bar
                """),
            ("mod.bicep", """
                @export()
                var bar = 'bar'
                """));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP287", DiagnosticLevel.Error, "'bar' refers to a value but is being used as a type here."),
        });
    }

    [TestMethod]
    public void Imported_type_cannot_be_used_in_a_value_expression()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {foo} from 'mod.bicep'

                var bar = foo
                """),
            ("mod.bicep", """
                @export()
                type foo = string[]
                """));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP288", DiagnosticLevel.Error, "'foo' refers to a type but is being used as a value here."),
        });
    }

    [TestMethod]
    public void Dereferencing_unexported_or_unknown_symbol_from_wildcard_should_raise_diagnostic()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import * as bar from 'mod.bicep'
                param baz bar.foo
                """),
            ("mod.bicep", """
                type foo = string[]
                """));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP052", DiagnosticLevel.Error, "The type \"bar\" does not contain property \"foo\"."),
        });
    }

    [TestMethod]
    public void Exporting_type_property_should_raise_diagnostic()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports, """
            type foo = {
                @export()
                property: string
            }
            """);

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP361", DiagnosticLevel.Error, "The \"@export()\" decorator must target a top-level statement.")
        });
    }

    [TestMethod]
    public void Importing_non_template_should_not_raise_diagnostic()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {} from 'main.bicepparam'
                """),
            ("main.bicepparam", """
                using 'mod.bicep'

                var foo = 'bar'

                param bar = foo
                """),
            ("mod.bicep", """
                param bar string
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Imported_type_symbols_should_have_declarations_injected_into_compiled_template()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {foo} from 'mod.bicep'
                """),
            ("mod.bicep", """
                @export()
                type foo = string[]
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("definitions", JToken.Parse($$"""
            {
                "foo": {
                    "type": "array",
                    "items": {
                        "type": "string"
                    },
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod.bicep"
                        }
                    }
                }
            }
            """));
    }

    [TestMethod]
    public void Imported_variable_symbols_should_have_declarations_injected_into_compiled_template()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {foo} from 'mod.bicep'
                """),
            ("mod.bicep", """
                @export()
                @description('The foo variable')
                var foo = 'foo'
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("variables", JToken.Parse($$"""
            {
                "foo": "foo"
            }
            """));
    }

    [TestMethod]
    public void Imported_function_symbols_should_have_declarations_injected_into_compiled_template()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImportsAndUserDefinedFunctions,
            ("main.bicep", """
                import {greet} from 'mod.bicep'
                """),
            ("mod.bicep", """
                @export()
                @description('Say hi to someone')
                func greet(name string) string => 'Hi, ${name}!'
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("functions", JToken.Parse($$"""
            [
                {
                    "namespace": "{{EmitConstants.UserDefinedFunctionsNamespace}}",
                    "members": {
                        "greet": {
                            "parameters": [
                                {
                                    "type": "string",
                                    "name": "name"
                                }
                            ],
                            "output": {
                                "type": "string",
                                "value": "[format('Hi, {0}!', parameters('name'))]"
                            },
                            "metadata": {
                                "{{LanguageConstants.MetadataDescriptionPropertyName}}": "Say hi to someone",
                                "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                                    "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod.bicep"
                                }
                            }
                        }
                    }
                }
            ]
            """));
    }

    [TestMethod]
    public void Importing_functions_should_be_blocked_if_user_defined_functions_feature_not_enabled()
    {
        var featureProviderFactory = StrictMock.Of<IFeatureProviderFactory>();
        var defaultFeatures = new FeatureProvider(IConfigurationManager.GetBuiltInConfiguration());
        featureProviderFactory
            .Setup(m => m.GetFeatureProvider(It.Is<System.Uri>(uri => uri.AbsolutePath.EndsWith("main.bicep"))))
            .Returns(new OverriddenFeatureProvider(defaultFeatures, new(CompileTimeImportsEnabled: true)));
        featureProviderFactory
            .Setup(m => m.GetFeatureProvider(It.Is<System.Uri>(uri => uri.AbsolutePath.EndsWith("mod.bicep"))))
            .Returns(new OverriddenFeatureProvider(defaultFeatures, new(CompileTimeImportsEnabled: true, UserDefinedFunctionsEnabled: true)));

        var result = CompilationHelper.Compile(new ServiceBuilder().WithFeatureProviderFactory(featureProviderFactory.Object),
            ("main.bicep", """
                import {greet} from 'mod.bicep'
                """),
            ("mod.bicep", """
                @export()
                @description('Say hi to someone')
                func greet(name string) string => 'Hi, ${name}!'
                """));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP343", DiagnosticLevel.Error, $@"Using a func declaration statement requires enabling EXPERIMENTAL feature ""{nameof(ExperimentalFeaturesEnabled.UserDefinedFunctions)}"".")
        });
    }

    [TestMethod]
    public void Calling_methods_on_wildcard_imports_should_be_blocked_if_user_defined_functions_feature_not_enabled()
    {
        var featureProviderFactory = StrictMock.Of<IFeatureProviderFactory>();
        var defaultFeatures = new FeatureProvider(IConfigurationManager.GetBuiltInConfiguration());
        featureProviderFactory
            .Setup(m => m.GetFeatureProvider(It.Is<System.Uri>(uri => uri.AbsolutePath.EndsWith("main.bicep"))))
            .Returns(new OverriddenFeatureProvider(defaultFeatures, new(CompileTimeImportsEnabled: true)));
        featureProviderFactory
            .Setup(m => m.GetFeatureProvider(It.Is<System.Uri>(uri => uri.AbsolutePath.EndsWith("mod.bicep"))))
            .Returns(new OverriddenFeatureProvider(defaultFeatures, new(CompileTimeImportsEnabled: true, UserDefinedFunctionsEnabled: true)));

        var result = CompilationHelper.Compile(new ServiceBuilder().WithFeatureProviderFactory(featureProviderFactory.Object),
            ("main.bicep", """
                import * as mod from 'mod.bicep'

                output greeting string = mod.greet('friend')
                """),
            ("mod.bicep", """
                @export()
                @description('Say hi to someone')
                func greet(name string) string => 'Hi, ${name}!'
                """));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP343", DiagnosticLevel.Error, $@"Using a func declaration statement requires enabling EXPERIMENTAL feature ""{nameof(ExperimentalFeaturesEnabled.UserDefinedFunctions)}"".")
        });
    }

    [TestMethod]
    public void Type_symbols_imported_from_ARM_json_should_have_declarations_injected_into_compiled_template()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {foo} from 'mod.json'
                """),
            ("mod.json", $$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "contentVersion": "1.0.0.0",
                    "languageVersion": "2.0",
                    "definitions": {
                        "foo": {
                            "metadata": {
                                "{{LanguageConstants.MetadataExportedPropertyName}}": true
                            },
                            "type": "array",
                            "items": {
                                "$ref": "#/definitions/bar"
                            }
                        },
                        "bar": {
                            "type": "string"
                        }
                    },
                    "resources": {}
                }
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("definitions", JToken.Parse($$"""
            {
                "foo": {
                    "type": "array",
                    "items": {
                        "$ref": "#/definitions/_1.bar"
                    },
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod.json"
                        }
                    }
                },
                "_1.bar": {
                    "type": "string",
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod.json"
                        }
                    }
                }
            }
            """));
    }

    [TestMethod]
    public void Variable_symbols_imported_from_ARM_json_should_have_declarations_injected_into_compiled_template()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {foo} from 'mod.json'
                """),
            ("mod.json", $$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "contentVersion": "1.0.0.0",
                    "metadata": {
                        "{{LanguageConstants.TemplateMetadataExportedVariablesName}}": [
                            {
                                "name": "foo",
                                "description": "A lengthy, florid description"
                            }
                        ]
                    },
                    "variables": {
                        "foo": {
                            "property": "[variables('bar')]"
                        },
                        "bar": "barValue"
                    },
                    "resources": []
                }
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("variables", JToken.Parse($$"""
            {
                "foo": {
                    "property": "[variables('_1.bar')]"
                },
                "_1.bar": "barValue"
            }
            """));
    }

    [TestMethod]
    public void Function_symbols_imported_from_ARM_json_should_have_declarations_injected_into_compiled_template()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImportsAndUserDefinedFunctions,
            ("main.bicep", """
                import {greet, 'ns.cow_say' as cowSay} from 'mod.json'
                """),
            ("mod.json", $$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "contentVersion": "1.0.0.0",
                    "languageVersion": "2.0",
                    "definitions": {
                        "nonEmptyString": {
                            "type": "string",
                            "minLength": 1
                        }
                    },
                    "functions": [
                        {
                            "namespace": "{{EmitConstants.UserDefinedFunctionsNamespace}}",
                            "members": {
                                "greet": {
                                    "parameters": [
                                        {
                                            "type": "string",
                                            "name": "name"
                                        }
                                    ],
                                    "output": {
                                        "type": "string",
                                        "value": "[format('Hi, {0}!', ns.echo(parameters('name')))]"
                                    },
                                    "metadata": {
                                        "{{LanguageConstants.MetadataExportedPropertyName}}": true
                                    }
                                }
                            }
                        },
                        {
                            "namespace": "ns",
                            "members": {
                                "cow_say": {
                                    "parameters": [
                                        {
                                            "type": "string",
                                            "name": "instead_of_a_moo"
                                        }
                                    ],
                                    "output": {
                                        "type": "string",
                                        "value": "[format('The cow says: {0}', parameters('instead_of_a_moo'))]"
                                    },
                                    "metadata": {
                                        "{{LanguageConstants.MetadataExportedPropertyName}}": true
                                    }
                                },
                                "echo": {
                                    "parameters": [
                                        {
                                            "$ref": "#/definitions/nonEmptyString",
                                            "name": "in"
                                        }
                                    ],
                                    "output": {
                                        "$ref": "#/definitions/nonEmptyString",
                                        "value": "[parameters('in')]"
                                    }
                                }
                            }
                        }
                    ],
                    "resources": []
                }
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("functions", JToken.Parse($$"""
            [
                {
                    "namespace": "_1",
                    "members": {
                        "_2": {
                            "parameters": [
                                {
                                    "$ref": "#/definitions/_1.nonEmptyString",
                                    "name": "in"
                                }
                            ],
                            "output": {
                                "$ref": "#/definitions/_1.nonEmptyString",
                                "value": "[parameters('in')]"
                            },
                            "metadata": {
                                "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                                    "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod.json",
                                    "{{LanguageConstants.ImportMetadataOriginalIdentifierPropertyName}}": "ns.echo"
                                }
                            }
                        }
                    }
                },
                {
                    "namespace": "{{EmitConstants.UserDefinedFunctionsNamespace}}",
                    "members": {
                        "cowSay": {
                            "parameters": [
                                {
                                    "type": "string",
                                    "name": "instead_of_a_moo"
                                }
                            ],
                            "output": {
                                "type": "string",
                                "value": "[format('The cow says: {0}', parameters('instead_of_a_moo'))]"
                            },
                            "metadata": {
                                "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                                    "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod.json",
                                    "{{LanguageConstants.ImportMetadataOriginalIdentifierPropertyName}}": "ns.cow_say"
                                }
                            }
                        },
                        "greet": {
                            "parameters": [
                                {
                                    "type": "string",
                                    "name": "name"
                                }
                            ],
                            "output": {
                                "type": "string",
                                "value": "[format('Hi, {0}!', _1._2(parameters('name')))]"
                            },
                            "metadata": {
                                "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                                    "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod.json"
                                }
                            }
                        }
                    }
                }
            ]
            """));

        result.Template.Should().HaveValueAtPath("definitions", JToken.Parse($$"""
            {
                "_1.nonEmptyString": {
                    "type": "string",
                    "minLength": 1,
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod.json"
                        }
                    }
                }
            }
            """));
    }

    [TestMethod]
    public void Funky_ARM_allowedValues_survive_symbol_import_and_injection()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {foo as fizz, bar as buzz} from 'mod.json'
                """),
            ("mod.json", $$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "contentVersion": "1.0.0.0",
                    "languageVersion": "2.0",
                    "definitions": {
                        "foo": {
                            "metadata": {
                                "{{LanguageConstants.MetadataExportedPropertyName}}": true
                            },
                            "type": "array",
                            "allowedValues": ["foo", "bar"]
                        },
                        "bar": {
                            "metadata": {
                                "{{LanguageConstants.MetadataExportedPropertyName}}": true
                            },
                            "type": "string",
                            "allowedValues": ["foo", "bar"]
                        }
                    },
                    "resources": {}
                }
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("definitions", JToken.Parse($$"""
            {
                "fizz": {
                    "type": "array",
                    "allowedValues": ["bar", "foo"],
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod.json",
                            "{{LanguageConstants.ImportMetadataOriginalIdentifierPropertyName}}": "foo"
                        }
                    }
                },
                "buzz": {
                    "type": "string",
                    "allowedValues": ["bar", "foo"],
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod.json",
                            "{{LanguageConstants.ImportMetadataOriginalIdentifierPropertyName}}": "bar"
                        }
                    }
                }
            }
            """));
    }

    [TestMethod]
    public void Symbols_imported_via_wildcard_should_have_declarations_injected_into_compiled_template()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImportsAndUserDefinedFunctions,
            ("main.bicep", """
                import * as foo from 'mod.bicep'
                """),
            ("mod.bicep", """
                @export()
                type foo = string[]

                @export()
                var squares = map(range(0, 100), x => x * x)

                @export()
                func greet(name string) string => 'Hi, ${name}!'
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("definitions", JToken.Parse($$"""
            {
                "_1.foo": {
                    "type": "array",
                    "items": {
                        "type": "string"
                    },
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod.bicep"
                        }
                    }
                }
            }
            """));

        result.Template.Should().HaveValueAtPath("variables", JToken.Parse($$"""
            {
                "_1.squares": "[map(range(0, 100), lambda('x', mul(lambdaVariables('x'), lambdaVariables('x'))))]"
            }
            """));

        result.Template.Should().HaveValueAtPath("functions", JToken.Parse($$"""
            [
                {
                    "namespace": "_1",
                    "members": {
                        "greet": {
                            "parameters": [
                                {
                                    "type": "string",
                                    "name": "name"
                                }
                            ],
                            "output": {
                                "type": "string",
                                "value": "[format('Hi, {0}!', parameters('name'))]"
                            },
                            "metadata": {
                                "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                                    "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod.bicep"
                                }
                            }
                        }
                    }
                }
            ]
            """));
    }

    [TestMethod]
    public void Imported_symbol_closure_is_deduplicated_in_compiled_template()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import * as bar from 'mod.bicep'
                import {foo} from 'mod.bicep'
                """),
            ("mod.bicep", """
                @export()
                type foo = string[]

                @export()
                @minValue(1)
                @maxValue(10)
                type fizz = int
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

        // `foo` is imported both by name and via a wildcard -- make sure it only gets injected once (and that it uses the right name)
        result.Template.Should().HaveValueAtPath("definitions", JToken.Parse($$"""
            {
                "foo": {
                    "type": "array",
                    "items": {
                        "type": "string"
                    },
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod.bicep"
                        }
                    }
                },
                "_1.fizz": {
                    "type": "int",
                    "minValue": 1,
                    "maxValue": 10,
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod.bicep"
                        }
                    }
                }
            }
            """));
    }

    [TestMethod]
    public void Imported_type_symbols_with_a_lengthy_reference_chain_should_have_declarations_injected_into_compiled_template()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {foo} from 'mod.bicep'
                """),
            ("mod.bicep", """
                import {bar} from 'mod2.bicep'

                @export()
                type foo = {
                    bar: bar
                    anotherProperty: unexported
                }

                type unexported = 'fizz' | 'buzz' | 'pop'
                """),
            ("mod2.bicep", """
                import * as foo from 'mod3.bicep'

                @export()
                type bar = {
                    foo: foo.bar
                    prop: unexported
                }

                type unexported = {
                    nested: alsoNotExported
                }

                type alsoNotExported = int
                """),
            ("mod3.bicep", """
                import {foo} from 'mod4.bicep'

                @export()
                type bar = foo
                """),
            ("mod4.bicep", """
                @export()
                type foo = string[]
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("definitions", JToken.Parse($$"""
            {
                "foo": {
                    "type": "object",
                    "properties": {
                        "bar": {
                            "$ref": "#/definitions/_2.bar"
                        },
                        "anotherProperty": {
                            "$ref": "#/definitions/_1.unexported"
                        }
                    },
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod.bicep"
                        }
                    }
                },
                "_1.unexported": {
                    "type": "string",
                    "allowedValues": [
                        "buzz",
                        "fizz",
                        "pop"
                    ],
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod.bicep"
                        }
                    }
                },
                "_2.bar": {
                    "type": "object",
                    "properties": {
                        "foo": {
                            "$ref": "#/definitions/_3.bar"
                        },
                        "prop": {
                            "$ref": "#/definitions/_2.unexported"
                        }
                    },
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod2.bicep"
                        }
                    }
                },
                "_2.unexported": {
                    "type": "object",
                    "properties": {
                        "nested": {
                            "$ref": "#/definitions/_2.alsoNotExported"
                        }
                    },
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod2.bicep"
                        }
                    }
                },
                "_2.alsoNotExported": {
                    "type": "int",
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod2.bicep"
                        }
                    }
                },
                "_3.bar": {
                    "$ref": "#/definitions/_4.foo",
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod3.bicep"
                        }
                    }
                },
                "_4.foo": {
                    "type": "array",
                    "items": {
                        "type": "string"
                    },
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod4.bicep"
                        }
                    }
                }
            }
            """));
    }

    [TestMethod]
    public void Imported_variable_symbols_with_a_lengthy_reference_chain_should_have_declarations_injected_into_compiled_template()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {foo} from 'mod.bicep'
                """),
            ("mod.bicep", """
                import {bar} from 'mod2.bicep'

                @export()
                var foo = {
                    bar: bar
                    anotherProperty: unexported
                }

                var unexported = 'unexported'
                """),
            ("mod2.bicep", """
                import * as foo from 'mod3.bicep'

                @export()
                var bar = {
                    foo: foo.bar
                    prop: unexported
                }

                var unexported = {
                    nested: alsoNotExported
                }

                var alsoNotExported = 45
                """),
            ("mod3.bicep", """
                import {foo} from 'mod4.bicep'

                @export()
                var bar = foo
                """),
            ("mod4.bicep", """
                @export()
                var foo = ['snap', 'crackle', 'pop']
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("variables", JToken.Parse($$"""
            {
                "foo": {
                    "bar": "[variables('_2.bar')]",
                    "anotherProperty": "[variables('_1.unexported')]"
                },
                "_1.unexported": "unexported",
                "_2.bar": {
                    "foo": "[variables('_3.bar')]",
                    "prop": "[variables('_2.unexported')]"
                },
                "_2.unexported": {
                    "nested": "[variables('_2.alsoNotExported')]"
                },
                "_2.alsoNotExported": 45,
                "_3.bar": "[variables('_4.foo')]",
                "_4.foo": [
                    "snap",
                    "crackle",
                    "pop"
                ]
            }
            """));
    }

    [TestMethod]
    public void Imported_function_symbols_with_a_lengthy_reference_chain_should_have_declarations_injected_into_compiled_template()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImportsAndUserDefinedFunctions,
            ("main.bicep", """
                import {foo} from 'mod.bicep'

                var greeting = foo('friend')
                """),
            ("mod.bicep", """
                import {bar} from 'mod2.bicep'

                @export()
                func foo(name nonEmptyString) nonEmptyString => bar(name)

                @minLength(1)
                type nonEmptyString = string
                """),
            ("mod2.bicep", """
                import * as baz from 'mod3.bicep'

                @export()
                func bar(name nonEmptyString) nonEmptyString => baz.quux(name)

                @minLength(1)
                type nonEmptyString = string
                """),
            ("mod3.bicep", """
                import {greet} from 'mod4.bicep'

                @export()
                func quux(name nonEmptyString) nonEmptyString => indirection(name)

                func indirection(name nonEmptyString) nonEmptyString => greet(name)

                @minLength(1)
                type nonEmptyString = string
                """),
            ("mod4.bicep", """
                import {nonEmptyString} from 'mod5.bicep'

                @export()
                func greet(name indirection) indirection => 'Hi, ${name}!'

                type indirection = nonEmptyString
                """),
            ("mod5.bicep", """
                @export()
                @minLength(1)
                type nonEmptyString = string
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("definitions", JToken.Parse($$"""
            {
                "_1.nonEmptyString": {
                    "type": "string",
                    "minLength": 1,
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod.bicep"
                        }
                    }
                },
                "_2.nonEmptyString": {
                    "type": "string",
                    "minLength": 1,
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod2.bicep"
                        }
                    }
                },
                "_3.nonEmptyString": {
                    "type": "string",
                    "minLength": 1,
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod3.bicep"
                        }
                    }
                },
                "_4.indirection": {
                    "$ref": "#/definitions/_5.nonEmptyString",
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod4.bicep"
                        }
                    }
                },
                "_5.nonEmptyString": {
                    "type": "string",
                    "minLength": 1,
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod5.bicep"
                        }
                    }
                },
            }
            """));

        result.Template.Should().HaveValueAtPath("functions", JToken.Parse($$"""
            [
                {
                    "namespace": "_2",
                    "members": {
                        "bar": {
                            "parameters": [
                                {
                                    "$ref": "#/definitions/_2.nonEmptyString",
                                    "name": "name"
                                }
                            ],
                            "output": {
                                "$ref": "#/definitions/_2.nonEmptyString",
                                "value": "[_3.quux(parameters('name'))]"
                            },
                            "metadata": {
                                "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                                    "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod2.bicep"
                                }
                            }
                        }
                    }
                },
                {
                    "namespace": "__bicep",
                    "members": {
                        "foo": {
                            "parameters": [
                                {
                                    "$ref": "#/definitions/_1.nonEmptyString",
                                    "name": "name"
                                }
                            ],
                            "output": {
                                "$ref": "#/definitions/_1.nonEmptyString",
                                "value": "[_2.bar(parameters('name'))]"
                            },
                            "metadata": {
                                "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                                    "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod.bicep"
                                }
                            }
                        }
                    }
                },
                {
                    "namespace": "_4",
                    "members": {
                        "greet": {
                            "parameters": [
                                {
                                    "$ref": "#/definitions/_4.indirection",
                                    "name": "name"
                                }
                            ],
                            "output": {
                                "$ref": "#/definitions/_4.indirection",
                                "value": "[format('Hi, {0}!', parameters('name'))]"
                            },
                            "metadata": {
                                "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                                    "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod4.bicep"
                                }
                            }
                        }
                    }
                },
                {
                    "namespace": "_3",
                    "members": {
                        "indirection": {
                            "parameters": [
                                {
                                    "$ref": "#/definitions/_3.nonEmptyString",
                                    "name": "name"
                                }
                            ],
                            "output": {
                                "$ref": "#/definitions/_3.nonEmptyString",
                                "value": "[_4.greet(parameters('name'))]"
                            },
                            "metadata": {
                                "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                                    "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod3.bicep"
                                }
                            }
                        },
                        "quux": {
                            "parameters": [
                                {
                                    "$ref": "#/definitions/_3.nonEmptyString",
                                    "name": "name"
                                }
                            ],
                            "output": {
                                "$ref": "#/definitions/_3.nonEmptyString",
                                "value": "[_3.indirection(parameters('name'))]"
                            },
                            "metadata": {
                                "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                                    "{{LanguageConstants.ImportMetadataSourceTemplatePropertyName}}": "mod3.bicep"
                                }
                            }
                        }
                    }
                }
            ]
            """));

        result.Template.Should().HaveValueAtPath("variables", JToken.Parse($$"""
            {
                "greeting": "[__bicep.foo('friend')]"
            }
            """));
    }

    [TestMethod]
    public void Importing_the_same_symbol_under_two_separate_names_should_raise_diagnostic()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {foo} from 'mod.bicep'
                import {foo as fizz} from 'mod.bicep'
                """),
            ("mod.bicep", """
                @export()
                type foo = string[]
                """));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP362", DiagnosticLevel.Error, "This symbol is imported multiple times under the names 'foo', 'fizz'."),
            ("BCP362", DiagnosticLevel.Error, "This symbol is imported multiple times under the names 'foo', 'fizz'."), // The same diagnostic should be raised on each import
        });
    }

    [TestMethod]
    public void Importing_the_same_symbol_from_json_template_under_two_separate_names_should_raise_diagnostic()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {foo} from 'mod.json'
                import {foo as fizz} from 'mod.json'
                """),
            ("mod.json", $$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "contentVersion": "1.0.0.0",
                    "languageVersion": "2.0",
                    "definitions": {
                        "foo": {
                            "metadata": {
                                "{{LanguageConstants.MetadataExportedPropertyName}}": true
                            },
                            "type": "string"
                        }
                    },
                    "resources": {}
                }
                """));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP362", DiagnosticLevel.Error, "This symbol is imported multiple times under the names 'foo', 'fizz'."),
            ("BCP362", DiagnosticLevel.Error, "This symbol is imported multiple times under the names 'foo', 'fizz'."), // The same diagnostic should be raised on each import
        });
    }

    [TestMethod]
    public void Exporting_a_variable_that_references_a_parameter_should_raise_diagnostic()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports, """
            param foo string
            var bar = 'x${foo}x'
            var baz = bar

            @export()
            var quux = baz
            """);

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP372", DiagnosticLevel.Error, "The \"@export()\" decorator may not be applied to variables that refer to parameters, modules, or resource, either directly or indirectly. The target of this decorator contains direct or transitive references to the following unexportable symbols: \"foo\"."),
        });
    }

    [TestMethod]
    public void Exporting_a_variable_that_references_a_resource_should_raise_diagnostic()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports, """
            resource foo 'Microsoft.Network/virtualNetworks@2020-06-01' = {
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
            var bar = [
                {vnetName: foo.name, addressSpace: foo.properties.addressSpace}
            ]
            var baz = bar

            @export()
            var quux = baz
            """);

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP372", DiagnosticLevel.Error, "The \"@export()\" decorator may not be applied to variables that refer to parameters, modules, or resource, either directly or indirectly. The target of this decorator contains direct or transitive references to the following unexportable symbols: \"foo\"."),
        });
    }

    [TestMethod]
    public void Exporting_a_variable_that_references_a_local_variable_should_not_raise_diagnostic()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports, """
            @export()
            var foo = map(range(1, 10), x => x * x)
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Importing_a_name_that_refers_to_mulitple_exports_should_raise_diagnostic()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {foo} from 'mod.json'
                """),
            ("mod.json", $$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "contentVersion": "1.0.0.0",
                    "languageVersion": "2.0",
                    "metadata": {
                        "{{LanguageConstants.TemplateMetadataExportedVariablesName}}": [
                            {
                                "name": "foo",
                                "description": "A lengthy, florid description"
                            }
                        ]
                    },
                    "variables": {
                        "foo": "foo"
                    },
                    "definitions": {
                        "foo": {
                            "type": "string",
                            "metadata": {
                                "{{LanguageConstants.MetadataExportedPropertyName}}": true
                            }
                        }
                    },
                    "resources": {}
                }
                """));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP373", DiagnosticLevel.Error, "Unable to import the symbol named \"foo\": The name \"foo\" is ambiguous because it refers to exports of the following kinds: Type, Variable."),
        });
    }

    [TestMethod]
    public void Wildcard_importing_a_target_with_multiple_exports_using_the_same_name_should_raise_diagnostic()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import * as everything from 'mod.json'
                """),
            ("mod.json", $$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "contentVersion": "1.0.0.0",
                    "languageVersion": "2.0",
                    "metadata": {
                        "{{LanguageConstants.TemplateMetadataExportedVariablesName}}": [
                            {
                                "name": "foo",
                                "description": "A lengthy, florid description"
                            }
                        ]
                    },
                    "variables": {
                        "foo": "foo"
                    },
                    "definitions": {
                        "foo": {
                            "type": "string",
                            "metadata": {
                                "{{LanguageConstants.MetadataExportedPropertyName}}": true
                            }
                        }
                    },
                    "resources": {}
                }
                """));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP374", DiagnosticLevel.Error, "The imported model cannot be loaded with a wildcard because it contains the following duplicated exports: \"foo\"."),
        });
    }

    [TestMethod]
    public void Copy_variable_symbols_imported_from_ARM_json_should_have_declarations_injected_into_compiled_template()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {foo} from 'mod.json'
                """),
            ("mod.json", $$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "contentVersion": "1.0.0.0",
                    "metadata": {
                        "{{LanguageConstants.TemplateMetadataExportedVariablesName}}": [
                            {
                                "name": "foo",
                                "description": "A lengthy, florid description"
                            }
                        ]
                    },
                    "variables": {
                        "fooCount": "[add(2, 3)]",
                        "copy": [
                            {
                                "name": "foo",
                                "count": "[variables('fooCount')]",
                                "input": "[copyIndex('foo', 1)]"
                            }
                        ]
                    },
                    "resources": []
                }
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("variables", JToken.Parse($$"""
            {
                "_1.fooCount": "[add(2, 3)]",
                "copy": [
                    {
                        "name": "foo",
                        "count": "[length(range(0, variables('_1.fooCount')))]",
                        "input": "[add(copyIndex('foo'), 1)]"
                    }
                ]
            }
            """));
    }

    [TestMethod]
    public void Copy_variable_symbols_imported_from_ARM_json_as_part_of_import_closure_should_have_declarations_injected_into_compiled_template()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {bar} from 'mod.json'
                """),
            ("mod.json", $$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "contentVersion": "1.0.0.0",
                    "metadata": {
                        "{{LanguageConstants.TemplateMetadataExportedVariablesName}}": [
                            {
                                "name": "bar"
                            }
                        ]
                    },
                    "variables": {
                        "bar": "[variables('foo')[3]]",
                        "fooCount": "[add(2, 3)]",
                        "copy": [
                            {
                                "name": "foo",
                                "count": "[variables('fooCount')]",
                                "input": "[copyIndex('foo', 1)]"
                            }
                        ]
                    },
                    "resources": []
                }
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("variables", JToken.Parse($$"""
            {
                "bar": "[variables('_1.foo')[3]]",
                "_1.fooCount": "[add(2, 3)]",
                "copy": [
                    {
                        "name": "_1.foo",
                        "count": "[length(range(0, variables('_1.fooCount')))]",
                        "input": "[add(copyIndex('_1.foo'), 1)]"
                    }
                ]
            }
            """));
    }

    [TestMethod]
    public void Named_import_can_identify_target_by_quoted_string()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {'a-b' as ab} from 'mod.json'
                """),
            ("mod.json", $$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "contentVersion": "1.0.0.0",
                    "languageVersion": "2.0",
                    "metadata": {
                        "{{LanguageConstants.TemplateMetadataExportedVariablesName}}": [
                            {
                                "name": "a-b",
                                "description": "A lengthy, florid description"
                            }
                        ]
                    },
                    "variables": {
                        "a-b": "a-b"
                    },
                    "resources": {}
                }
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Named_import_by_quoted_string_should_block_interpolation()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {'${originalSymbolName}' as ab} from 'mod.json'

                var originalSymbolName = 'a-b'
                """),
            ("mod.json", $$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "contentVersion": "1.0.0.0",
                    "metadata": {
                        "{{LanguageConstants.TemplateMetadataExportedVariablesName}}": [
                            {
                                "name": "a-b"
                            }
                        ]
                    },
                    "variables": {
                        "a-b": "a-b"
                    },
                    "resources": {}
                }
                """));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP032", DiagnosticLevel.Error, "The value must be a compile-time constant."),
        });
    }

    [TestMethod]
    public void Named_import_by_quoted_string_without_alias_should_raise_diagnostic()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {'foo'} from 'mod.json'
                """),
            ("mod.json", $$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "contentVersion": "1.0.0.0",
                    "languageVersion": "2.0",
                    "metadata": {
                        "{{LanguageConstants.TemplateMetadataExportedVariablesName}}": [
                            {
                                "name": "foo",
                                "description": "A lengthy, florid description"
                            }
                        ]
                    },
                    "variables": {
                        "foo": "foo"
                    },
                    "resources": {}
                }
                """));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP375", DiagnosticLevel.Error, "An import list item that identifies its target with a quoted string must include an 'as <alias>' clause."),
        });
    }

    [TestMethod]
    public void Named_import_with_invalid_original_symbol_name_syntax_should_raise_diagnostic()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {1 as foo} from 'mod.bicep'
                """),
            ("mod.bicep", string.Empty));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP355", DiagnosticLevel.Error, "Expected the name of an exported symbol at this location."),
        });
    }

    [TestMethod]
    public void Importing_type_results_in_ARM_language_version_2()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {foo} from 'mod.bicep'
                """),
            ("mod.bicep", """
                @export()
                type foo = string
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("languageVersion", "2.0");
    }

    [TestMethod]
    public void Importing_variables_only_does_not_result_in_elevated_ARM_language_version()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {bar} from 'mod.bicep'
                """),
            ("mod.bicep", """
                @export()
                type foo = string

                @export()
                var bar = 'bar'
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotHaveValueAtPath("languageVersion");
    }

    [TestMethod]
    public void Named_imports_into_bicepparam_file_are_supported()
    {
        var result = CompilationHelper.CompileParams(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                param intParam int
                """),
            ("parameters.bicepparam", """
                using 'main.bicep'
                import {a} from 'mod1.bicep'
                import {b} from 'mod2.bicep'

                param intParam = a + b + 4
                """),
            ("mod1.bicep", """
                @export()
                var a = 2
                """),
            ("mod2.bicep", """
                @export()
                var b = 3
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

        var parameters = TemplateEvaluator.ParseParametersFile(result.Parameters);
        parameters["intParam"].Should().DeepEqual(9);
    }

    [TestMethod]
    public void Wildcard_imports_into_bicepparam_file_are_supported()
    {
        var result = CompilationHelper.CompileParams(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                param intParam int
                """),
            ("parameters.bicepparam", """
                using 'main.bicep'
                import * as mod1 from 'mod1.bicep'
                import * as mod2 from 'mod2.bicep'

                param intParam = mod1.a + mod2.b + 4
                """),
            ("mod1.bicep", """
                @export()
                var a = 2
                """),
            ("mod2.bicep", """
                @export()
                var b = 3
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

        var parameters = TemplateEvaluator.ParseParametersFile(result.Parameters);
        parameters["intParam"].Should().DeepEqual(9);
    }

    [TestMethod]
    public void Importing_types_is_blocked_in_bicepparam_files()
    {
        var result = CompilationHelper.CompileParams(ServicesWithCompileTimeTypeImports,
            ("parameters.bicepparam", """
                using 'main.bicep'

                import {foo} from 'mod.bicep'
                """),
            ("main.bicep", string.Empty),
            ("mod.bicep", """
                @export()
                type foo = string
                """));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP376", DiagnosticLevel.Error, "The \"foo\" symbol cannot be imported because imports of kind Type are not supported in files of kind ParamsFile."),
        });
    }

    // https://github.com/Azure/bicep/issues/12042
    [TestMethod]
    public void References_to_variable_properties_of_wildcard_imports_generate_references_to_synthesized_variables()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("vars.bicep", """
                @export()
                var obj = {
                    'Key One': 'Val1'
                    'Key Two': 'Val2'
                }

                @export()
                var config = {
                    a: {
                        id: 'A'
                        name: 'AaA'
                    }
                    b: {
                        id: 'B'
                        name: 'BbB'
                    }
                }

                @export()
                var string1 = 'Xyz'
                """),
            ("main.bicep", """
                import * as vars from 'vars.bicep'
                output outObj string = vars.obj['Key One']
                output outConfig string = vars.config.b.id
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("outputs.outObj.value", "[variables('_1.obj')['Key One']]");
        result.Template.Should().HaveValueAtPath("outputs.outConfig.value", "[variables('_1.config').b.id]");
    }

    // https://github.com/Azure/bicep/issues/12052
    [TestMethod]
    public void Test_Issue12052()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {Foo} from 'types.bicep'

                param foo Foo[] = [
                    {
                        bar: true
                    }
                    {
                        bar: false
                    }
                ]
                """),
            ("types.bicep", """
                @export()
                type Foo = {
                    bar: bool
                }
                """));

        result.Should().NotHaveAnyCompilationBlockingDiagnostics();
        result.Template.Should().HaveValueAtPath("parameters.foo.items.$ref", "#/definitions/Foo");
    }

    // https://github.com/Azure/bicep/issues/12401
    [TestMethod]
    public void Symbolic_name_target_is_used_when_function_import_closure_includes_a_user_defined_type()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImportsAndUserDefinedFunctions,
            ("main.bicep", """
                import { capitalizer } from 'function.bicep'
                """),
            ("function.bicep", """
                type myString = string

                @export()
                func capitalizer(in myString) string => toUpper(in)
                """));

        result.Should().NotHaveAnyCompilationBlockingDiagnostics();
        result.Template.Should().HaveValueAtPath("languageVersion", "2.0");
        result.Template.Should().HaveValueAtPath("functions[0].members.capitalizer.parameters[0].$ref", "#/definitions/_1.myString");
    }
}
