// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class CompileTimeImportTests
{
    private ServiceBuilder ServicesWithCompileTimeTypeImports => new ServiceBuilder()
        .WithFeatureOverrides(new(TestContext, CompileTimeImportsEnabled: true));

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
    public void Importing_non_template_should_raise_diagnostic()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {foo} from 'main.bicepparam'
                """),
            ("main.bicepparam", """
                using 'mod.bicep'

                param bar = 'bar'
                """),
            ("mod.bicep", """
                param bar string
                """));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP359", DiagnosticLevel.Error, "A compile-time import can only reference a Bicep file, an ARM template, a registry artifact, or a template spec.")
        });
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
                            "sourceTemplate": "mod.bicep"
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
                        "$ref": "#/definitions/1.bar"
                    },
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "sourceTemplate": "mod.json"
                        }
                    }
                },
                "1.bar": {
                    "type": "string",
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "sourceTemplate": "mod.json"
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
                    "property": "[variables('1.bar')]"
                },
                "1.bar": "barValue"
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
                            "sourceTemplate": "mod.json",
                            "originalIdentifier": "foo"
                        }
                    }
                },
                "buzz": {
                    "type": "string",
                    "allowedValues": ["bar", "foo"],
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "sourceTemplate": "mod.json",
                            "originalIdentifier": "bar"
                        }
                    }
                }
            }
            """));
    }

    [TestMethod]
    public void Symbols_imported_via_wildcard_should_have_declarations_injected_into_compiled_template()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import * as foo from 'mod.bicep'
                """),
            ("mod.bicep", """
                @export()
                type foo = string[]

                @export()
                var squares = map(range(0, 100), x => x * x)
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("definitions", JToken.Parse($$"""
            {
                "1.foo": {
                    "type": "array",
                    "items": {
                        "type": "string"
                    },
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "sourceTemplate": "mod.bicep"
                        }
                    }
                }
            }
            """));

        result.Template.Should().HaveValueAtPath("variables", JToken.Parse($$"""
            {
                "1.squares": "[map(range(0, 100), lambda('x', mul(lambdaVariables('x'), lambdaVariables('x'))))]"
            }
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
                            "sourceTemplate": "mod.bicep"
                        }
                    }
                },
                "1.fizz": {
                    "type": "int",
                    "minValue": 1,
                    "maxValue": 10,
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "sourceTemplate": "mod.bicep"
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
                            "$ref": "#/definitions/2.bar"
                        },
                        "anotherProperty": {
                            "$ref": "#/definitions/1.unexported"
                        }
                    },
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "sourceTemplate": "mod.bicep"
                        }
                    }
                },
                "1.unexported": {
                    "type": "string",
                    "allowedValues": [
                        "buzz",
                        "fizz",
                        "pop"
                    ],
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "sourceTemplate": "mod.bicep"
                        }
                    }
                },
                "2.bar": {
                    "type": "object",
                    "properties": {
                        "foo": {
                            "$ref": "#/definitions/3.bar"
                        },
                        "prop": {
                            "$ref": "#/definitions/2.unexported"
                        }
                    },
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "sourceTemplate": "mod2.bicep"
                        }
                    }
                },
                "2.unexported": {
                    "type": "object",
                    "properties": {
                        "nested": {
                            "$ref": "#/definitions/2.alsoNotExported"
                        }
                    },
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "sourceTemplate": "mod2.bicep"
                        }
                    }
                },
                "2.alsoNotExported": {
                    "type": "int",
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "sourceTemplate": "mod2.bicep"
                        }
                    }
                },
                "3.bar": {
                    "$ref": "#/definitions/4.foo",
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "sourceTemplate": "mod3.bicep"
                        }
                    }
                },
                "4.foo": {
                    "type": "array",
                    "items": {
                        "type": "string"
                    },
                    "metadata": {
                        "{{LanguageConstants.MetadataImportedFromPropertyName}}": {
                            "sourceTemplate": "mod4.bicep"
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
                    "bar": "[variables('2.bar')]",
                    "anotherProperty": "[variables('1.unexported')]"
                },
                "1.unexported": "unexported",
                "2.bar": {
                    "foo": "[variables('3.bar')]",
                    "prop": "[variables('2.unexported')]"
                },
                "2.unexported": {
                    "nested": "[variables('2.alsoNotExported')]"
                },
                "2.alsoNotExported": 45,
                "3.bar": "[variables('4.foo')]",
                "4.foo": [
                    "snap",
                    "crackle",
                    "pop"
                ]
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
            ("BCP373", DiagnosticLevel.Error, "The name \"foo\" is ambiguous because it refers to exports of the following kinds: Type, Variable."),
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
                "1.fooCount": "[add(2, 3)]",
                "copy": [
                    {
                        "name": "foo",
                        "count": "[length(range(0, variables('1.fooCount')))]",
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
                "bar": "[variables('1.foo')[3]]",
                "1.fooCount": "[add(2, 3)]",
                "copy": [
                    {
                        "name": "1.foo",
                        "count": "[length(range(0, variables('1.fooCount')))]",
                        "input": "[add(copyIndex('1.foo'), 1)]"
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
    public void Named_import_by_quoted_string_self_reference_should_raise_diagnostic()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {'${ab}' as ab} from 'mod.json'
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

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP079", DiagnosticLevel.Error, "This expression is referencing its own declaration, which is not allowed."),
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
            ("BCP376", DiagnosticLevel.Error, "An import list item that identifies its target with a quoted string must include an 'as <alias>' clause."),
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
            ("BCP375", DiagnosticLevel.Error, "The import list item does not identify which symbol to import."),
        });
    }
}
