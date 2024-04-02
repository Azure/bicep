// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
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
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public void Importing_unexported_symbol_should_raise_diagnostic()
    {
        var result = CompilationHelper.Compile(
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
        var result = CompilationHelper.Compile(
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
        var result = CompilationHelper.Compile(
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
        var result = CompilationHelper.Compile(
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
        var result = CompilationHelper.Compile("""
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
        var result = CompilationHelper.Compile(
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
        var result = CompilationHelper.Compile(
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
        var result = CompilationHelper.Compile(
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
        var result = CompilationHelper.Compile(
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
    public void Type_symbols_imported_from_ARM_json_should_have_declarations_injected_into_compiled_template()
    {
        var result = CompilationHelper.Compile(
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

    // https://github.com/Azure/bicep/issues/13248
    [TestMethod]
    public void Type_symbols_imported_from_ARM_json_via_wildcard_should_be_usable_as_types()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                import * as mod from 'mod.json'

                param foo mod.foo
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
        result.Template.Should().HaveValueAtPath("parameters.foo", JToken.Parse("""
            {
                "$ref": "#/definitions/_1.foo"
            }
            """));
    }

    [TestMethod]
    public void Variable_symbols_imported_from_ARM_json_should_have_declarations_injected_into_compiled_template()
    {
        var result = CompilationHelper.Compile(
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
    public void Variable_symbols_imported_from_ARM_json_with_wildcard_syntax_should_be_usable_as_values()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                import * as mod from 'mod.json'

                output bar string = mod.foo.property
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
        result.Template.Should().HaveValueAtPath("outputs.bar.value", "[variables('_1.foo').property]");
    }

    [TestMethod]
    public void Function_symbols_imported_from_ARM_json_should_have_declarations_injected_into_compiled_template()
    {
        var result = CompilationHelper.Compile(
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
        var result = CompilationHelper.Compile(
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
        var result = CompilationHelper.Compile(
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
        var result = CompilationHelper.Compile(
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
        var result = CompilationHelper.Compile(
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
        var result = CompilationHelper.Compile(
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
        var result = CompilationHelper.Compile(
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
        var result = CompilationHelper.Compile(
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
        var result = CompilationHelper.Compile(
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
        var result = CompilationHelper.Compile("""
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
        var result = CompilationHelper.Compile("""
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
        var result = CompilationHelper.Compile("""
            @export()
            var foo = map(range(1, 10), x => x * x)
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Importing_a_name_that_refers_to_mulitple_exports_should_raise_diagnostic()
    {
        var result = CompilationHelper.Compile(
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
        var result = CompilationHelper.Compile(
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
        var result = CompilationHelper.Compile(
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
        var result = CompilationHelper.Compile(
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
        var result = CompilationHelper.Compile(
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
        var result = CompilationHelper.Compile(
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
        var result = CompilationHelper.Compile(
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
        var result = CompilationHelper.Compile(
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
        var result = CompilationHelper.Compile(
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
    public void Importing_variable_results_in_ARM_language_version_2()
    {
        var result = CompilationHelper.Compile(
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
        result.Template.Should().HaveValueAtPath("languageVersion", "2.0");
    }

    [TestMethod]
    public void Named_imports_into_bicepparam_file_are_supported()
    {
        var result = CompilationHelper.CompileParams(
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
        var result = CompilationHelper.CompileParams(
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
        var result = CompilationHelper.CompileParams(
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
        var result = CompilationHelper.Compile(
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
        var result = CompilationHelper.Compile(
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

    // https://github.com/Azure/bicep/issues/12396
    [TestMethod]
    public void Test_Issue12396()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                import { _helloWorld } from 'types.bicep'

                resource st_account 'Microsoft.Storage/storageAccounts@2023-01-01' = {
                    name: last(split(_helloWorld, ''))
                    location: resourceGroup().location
                    sku: {
                        name: 'Standard_LRS'
                    }
                    kind: 'BlobStorage'
                }
                """),
            ("types.bicep", """
                @export()
                var _helloWorld = 'hello world'
                """));

        result.Should().NotHaveAnyCompilationBlockingDiagnostics();
    }

    // https://github.com/Azure/bicep/issues/12401
    [TestMethod]
    public void Symbolic_name_target_is_used_when_function_import_closure_includes_a_user_defined_type()
    {
        var result = CompilationHelper.Compile(
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

    // https://github.com/Azure/bicep/issues/12464
    [TestMethod]
    public void Test_12464()
    {
        var result = CompilationHelper.CompileParams(
            ("bicepconfig.json", """
                {
                    "experimentalFeaturesEnabled": {
                        "compileTimeImports": true
                    }
                }
                """),
            ("bicepconfig.bicep", """
                @export()
                var compileTimeImportsEnabled = loadJsonContent('bicepconfig.json').experimentalFeaturesEnabled.compileTimeImports
                @export()
                var literalTrue = true
                @export()
                var loadJsonContentTrue = compileTimeImportsEnabled
                """),
            ("test.bicep", """
                param boolParamOne bool
                param boolParamTwo bool
                output bothTrue bool = boolParamOne && boolParamTwo
                """),
            ("parameters.bicepparam", """
                using 'test.bicep'
                import * as bicepconfig from 'bicepconfig.bicep'
                // no error
                param boolParamOne = bicepconfig.literalTrue
                // Failed to evaluate parameter "boolParamTwo":
                // Unhandled exception during evaluating template language function 'variables' is not handled.bicep(BCP338)
                param boolParamTwo = bicepconfig.loadJsonContentTrue
                """));

        result.Diagnostics.Should().BeEmpty();
        result.Parameters.Should().HaveValueAtPath("parameters.boolParamTwo.value", true);
    }

    [TestMethod]
    public void Imported_variable_symbols_that_use_compile_time_functions_should_have_synthesized_variable_declarations_injected_into_compiled_template()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                import {fizzBuzz} from 'mod.bicep'
                """),
            ("mod.bicep", """
                var precalculatedFizzBuzz = loadJsonContent('fizz.json').values
                @export()
                var fizzBuzz = map(range(0, 100), i => precalculatedFizzBuzz[i % length(precalculatedFizzBuzz)])
                """),
            ("fizz.json", """
                {
                    "values": [
                        "",
                        "",
                        "fizz",
                        "",
                        "buzz",
                        "",
                        "",
                        "fizz",
                        "",
                        "buzz",
                        "",
                        "",
                        "fizz",
                        "",
                        "fizzbuzz"
                    ]
                }
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("variables", JToken.Parse($$"""
            {
                "_1._2": {
                    "values": [
                        "",
                        "",
                        "fizz",
                        "",
                        "buzz",
                        "",
                        "",
                        "fizz",
                        "",
                        "buzz",
                        "",
                        "",
                        "fizz",
                        "",
                        "fizzbuzz"
                    ]
                },
                "_1.precalculatedFizzBuzz": "[variables('_1._2').values]",
                "fizzBuzz": "[map(range(0, 100), lambda('i', variables('_1.precalculatedFizzBuzz')[mod(lambdaVariables('i'), length(variables('_1.precalculatedFizzBuzz')))]))]"
            }
            """));
    }

    // https://github.com/Azure/bicep/issues/12542
    [TestMethod]
    public void User_defined_function_calls_parameters_to_other_user_defined_function_calls_are_migrated_during_import()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                import {getSubnetNumber, isWindows} from 'types.bicep'

                param plan_name string = 'plan-name-999'

                var subnet_number = getSubnetNumber(plan_name)

                var formatted_subnet_name = '${isWindows(plan_name)}${subnet_number}'

                output out string = formatted_subnet_name
                """),
            ("types.bicep", """
                @export()
                func getPlanNumber(hostingPlanName string) string => substring(hostingPlanName, length(hostingPlanName) - 3)

                @export()
                func isFirstPlan(planNumber string) bool => planNumber == '001'

                @export()
                func isOneLeadingZero(planNumber string) bool => substring(planNumber, 0, 1) == '0'

                @export()
                func isTwoLeadingZeroes(planNumber string) bool => substring(planNumber, 0, 2) == '00'

                @export()
                func getSubnetNumber(plan_name string) string => isFirstPlan(getPlanNumber(plan_name)) ? '' : (isTwoLeadingZeroes(getPlanNumber(plan_name)) ? substring(getPlanNumber(plan_name), 2, 1) : (isOneLeadingZero(getPlanNumber(plan_name)) ? substring(getPlanNumber(plan_name), 1, 2) : getPlanNumber(plan_name)) )

                @export()
                func isWindows(hostingPlanName string) string => contains('-windows-', hostingPlanName) ? 'ASPWindows' : 'ASP'
                """));

        result.Diagnostics.Should().BeEmpty();
        result.Template.Should().NotBeNull();

        var evaluated = TemplateEvaluator.Evaluate(result.Template);
        evaluated.Should().HaveValueAtPath("outputs.out.value", "ASP999");
    }

    [TestMethod]
    public void Imported_objects_can_be_used_in_object_type_narrowing()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                import {obj} from 'mod.bicep'

                @sealed()
                output out {} = obj
                """),
            ("mod.bicep", """
                @export()
                var obj = {foo: 'foo', bar: 'bar'}
                """));

        result.Should().HaveDiagnostics(new[]
        {
            ("BCP037", DiagnosticLevel.Error, """The property "bar" is not allowed on objects of type "{ }". No other properties are allowed."""),
            ("BCP037", DiagnosticLevel.Error, """The property "foo" is not allowed on objects of type "{ }". No other properties are allowed."""),
        });
    }

    [TestMethod]
    public void Imported_objects_can_be_used_in_discriminated_object_type_narrowing()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                import {obj} from 'mod.bicep'

                @discriminator('type')
                output out {type: 'foo', pop: bool} | {type: 'bar', quux: string} = obj
                """),
            ("mod.bicep", """
                @export()
                var obj = {type: 'foo', bar: 'bar'}
                """));

        result.Should().HaveDiagnostics(new[]
        {
            ("BCP035", DiagnosticLevel.Error, """The specified "output" declaration is missing the following required properties: "pop"."""),
        });
    }

    // https://github.com/Azure/bicep/issues/12897
    [TestMethod]
    public void LanguageVersion_2_should_be_used_if_types_imported_via_wildcard()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                import * as types from 'types.bicep'
                """),
            ("types.bicep", """
                @export()
                type str = string
                """));

        result.Diagnostics.Should().BeEmpty();
        result.Template.Should().NotBeNull();
        result.Template.Should().HaveValueAtPath("languageVersion", "2.0");
    }

    [TestMethod]
    public void Bicepparam_imported_variables_from_invalid_bicep_file_cause_errors()
    {
        var result = CompilationHelper.CompileParams(
            ("parameters.bicepparam", """
using 'test.bicep'
import * as foo from 'test.bicep'
import { bar } from 'test.bicep'
"""),
            ("test.bicep", """
INVALID FILE
"""));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP104", DiagnosticLevel.Error, "The referenced module has errors."),
            ("BCP104", DiagnosticLevel.Error, "The referenced module has errors."),
        });
    }

    // https://github.com/Azure/bicep/issues/12899
    [TestMethod]
    public void LanguageVersion_2_should_be_used_if_types_imported_via_closure()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                import {a} from 'shared.bicep'
                """),
            ("shared.bicep", """
                @export()
                var a = b()

                func b() string[] => ['c', 'd']
                """));

        result.Diagnostics.Should().BeEmpty();
        result.Template.Should().NotBeNull();
        result.Template.Should().HaveValueAtPath("languageVersion", "2.0");
    }

    [TestMethod]
    public void Resource_derived_types_are_bound_when_imported_from_ARM_JSON_models()
    {
        var result = CompilationHelper.Compile(new ServiceBuilder().WithFeatureOverrides(new(TestContext, ResourceDerivedTypesEnabled: true)),
            ("main.bicep", """
                import {foo} from 'mod.json'

                param location string = resourceGroup().location
                param fooParam foo = {
                    bar: {
                        name: 'acct'
                        location: location
                        kind: 'StorageV2'
                        sku: {
                            name: 'Standard_LRS'
                        }
                        properties: {
                            unknownProperty: false
                        }
                    }
                }

                output fooOutput foo = fooParam
                """),
            ("mod.json", $$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "languageVersion": "2.0",
                    "contentVersion": "1.0.0.0",
                    "definitions": {
                        "foo": {
                            "metadata": {
                                "{{LanguageConstants.MetadataExportedPropertyName}}": true
                            },
                            "type": "object",
                            "additionalProperties": {
                                "type": "object",
                                "metadata": {
                                    "{{LanguageConstants.MetadataResourceDerivedTypePropertyName}}": "Microsoft.Storage/storageAccounts@2022-09-01"
                                }
                            }
                        }
                    },
                    "resources": []
                }
                """));

        result.Should().NotHaveAnyCompilationBlockingDiagnostics();
        result.Should().HaveDiagnostics(new[]
        {
            ("BCP037", DiagnosticLevel.Warning, """The property "unknownProperty" is not allowed on objects of type "StorageAccountPropertiesCreateParametersOrStorageAccountProperties". Permissible properties include "accessTier", "allowBlobPublicAccess", "allowCrossTenantReplication", "allowedCopyScope", "allowSharedKeyAccess", "azureFilesIdentityBasedAuthentication", "customDomain", "defaultToOAuthAuthentication", "dnsEndpointType", "encryption", "immutableStorageWithVersioning", "isHnsEnabled", "isLocalUserEnabled", "isNfsV3Enabled", "isSftpEnabled", "keyPolicy", "largeFileSharesState", "minimumTlsVersion", "networkAcls", "publicNetworkAccess", "routingPreference", "sasPolicy", "supportsHttpsTrafficOnly"."""),
        });
    }

    [TestMethod]
    public void Resource_derived_typed_compile_time_imports_raise_diagnostic_when_imported_from_ARM_JSON_models_without_feature_flag_set()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                import {foo} from 'mod.json'

                param location string = resourceGroup().location
                param fooParam foo = {
                    bar: {
                        name: 'acct'
                        location: location
                        kind: 'StorageV2'
                        sku: {
                            name: 'Standard_LRS'
                        }
                    }
                }

                output fooOutput foo = fooParam
                """),
            ("mod.json", $$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "languageVersion": "2.0",
                    "contentVersion": "1.0.0.0",
                    "definitions": {
                        "foo": {
                            "metadata": {
                                "{{LanguageConstants.MetadataExportedPropertyName}}": true
                            },
                            "type": "object",
                            "additionalProperties": {
                                "type": "object",
                                "metadata": {
                                    "{{LanguageConstants.MetadataResourceDerivedTypePropertyName}}": "Microsoft.Storage/storageAccounts@2022-09-01"
                                }
                            }
                        }
                    },
                    "resources": []
                }
                """));

        result.Should().HaveDiagnostics(new[]
        {
            ("BCP385", DiagnosticLevel.Error, """Using resource-derived types requires enabling EXPERIMENTAL feature "ResourceDerivedTypes"."""),
        });
    }

    [TestMethod]
    public void Resource_derived_typed_compile_time_imports_raise_diagnostic_when_imported_from_ARM_JSON_models_with_unrecognized_resource()
    {
        var result = CompilationHelper.Compile(new ServiceBuilder().WithFeatureOverrides(new(TestContext, ResourceDerivedTypesEnabled: true)),
            ("main.bicep", """
                import {foo} from 'mod.json'

                param location string = resourceGroup().location
                param fooParam foo = {
                    bar: {
                        name: 'acct'
                        location: location
                        kind: 'StorageV2'
                        sku: {
                            name: 'Standard_LRS'
                        }
                    }
                }

                output fooOutput foo = fooParam
                """),
            ("mod.json", $$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "languageVersion": "2.0",
                    "contentVersion": "1.0.0.0",
                    "definitions": {
                        "foo": {
                            "metadata": {
                                "{{LanguageConstants.MetadataExportedPropertyName}}": true
                            },
                            "type": "object",
                            "additionalProperties": {
                                "type": "object",
                                "metadata": {
                                    "{{LanguageConstants.MetadataResourceDerivedTypePropertyName}}": "Microsoft.Foo/bars@2022-09-01"
                                }
                            }
                        }
                    },
                    "resources": []
                }
                """));

        result.Should().HaveDiagnostics(new[]
        {
            ("BCP081", DiagnosticLevel.Warning, """Resource type "Microsoft.Foo/bars@2022-09-01" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed."""),
        });
    }

    // https://github.com/Azure/bicep/issues/12981
    [TestMethod]
    public void Copy_index_argument_in_imported_copy_variables_is_replaced()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                import * as vars from 'shared.bicep'
                """),
            ("shared.bicep", """
                @export()
                var identityPrefix = '10.150.3.0/24'

                @export()
                var domainControllerIPs = [for i in range(0, 2): cidrHost(identityPrefix, (3 + i))]

                @export()
                var domainController = cidrHost(identityPrefix, 3)

                @export()
                var test = 'test'
                """));

        result.Diagnostics.Should().BeEmpty();
        result.Template.Should().NotBeNull();
        result.Template.Should().HaveValueAtPath("variables.copy[?(@.name == '_1.domainControllerIPs')].input", "[cidrHost(variables('_1.identityPrefix'), add(3, range(0, 2)[copyIndex('_1.domainControllerIPs')]))]");
    }

    [TestMethod]
    public void Copy_index_argument_in_copy_variables_imported_from_json_is_replaced()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                import * as foo from 'mod.json'
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
                        "name": "_1.foo",
                        "count": "[length(range(0, variables('_1.fooCount')))]",
                        "input": "[add(copyIndex('_1.foo'), 1)]"
                    }
                ]
            }
            """));
    }

    [TestMethod]
    public void Tuple_imported_from_json_is_recompiled_to_a_valid_schema()
    {
        var typesBicep = """
            @export()
            type t = {
              p: {
                a: [
                  {
                    b: string
                    c: string
                  }
                ]
              }
            }
            """;

        var expectedCompilationOfTupleA = """
            {
              "type": "array",
              "prefixItems": [
                {
                  "type": "object",
                  "properties": {
                    "b": {
                      "type": "string"
                    },
                    "c": {
                      "type": "string"
                    }
                  }
                }
              ],
              "items": false
            }
            """;

        var resultFromBicep = CompilationHelper.Compile(
            ("types.bicep", typesBicep),
            ("main.bicep", "import {t} from 'types.bicep'"));

        resultFromBicep.Should().NotHaveAnyCompilationBlockingDiagnostics();
        resultFromBicep.Template.Should().NotBeNull();
        resultFromBicep.Template.Should().HaveJsonAtPath("definitions.t.properties.p.properties.a", expectedCompilationOfTupleA);

        var resultFromJson = CompilationHelper.Compile(
            ("types.json", CompilationHelper.Compile(typesBicep).Template!.ToString()),
            ("main.bicep", "import {t} from 'types.json'"));

        resultFromJson.Should().NotHaveAnyCompilationBlockingDiagnostics();
        resultFromJson.Template.Should().NotBeNull();
        resultFromJson.Template.Should().HaveJsonAtPath("definitions.t.properties.p.properties.a", expectedCompilationOfTupleA);
    }
}
