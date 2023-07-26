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
        .WithFeatureOverrides(new(TestContext, CompileTimeImportsEnabled: true, UserDefinedTypesEnabled: true));

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
            ("BCP354", DiagnosticLevel.Error, "Using compile-time import statements requires enabling EXPERIMENTAL feature \"CompileTimeImports\"."),
        });
    }

    [TestMethod]
    public void Importing_unexported_symbol_should_raise_diagnostic()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {foo} from 'mod.bicep'
                """),
            ("mod.bicep", """
                type foo = string[]
                """));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP357", DiagnosticLevel.Error, "The 'foo' symbol was not found in (or was not exported by) the imported template."),
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
            ("BCP358", DiagnosticLevel.Error, "The \"@export()\" decorator must target a top-level statement.")
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
            ("BCP356", DiagnosticLevel.Error, "A compile-time import can only reference a Bicep file, an ARM template, a registry artifact, or a template spec.")
        });
    }

    [TestMethod]
    public void Imported_symbols_should_have_declarations_injected_into_compiled_template()
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
        result.Template.Should().HaveValueAtPath("definitions", JToken.Parse("""
            {
                "foo": {
                    "type": "array",
                    "items": {
                        "type": "string"
                    }
                }
            }
            """));
    }

    [TestMethod]
    public void Symbols_imported_from_ARM_json_should_have_declarations_injected_into_compiled_template()
    {
        var result = CompilationHelper.Compile(ServicesWithCompileTimeTypeImports,
            ("main.bicep", """
                import {foo} from 'mod.json'
                """),
            ("mod.json", $$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "contentVersion": "1.0.0.0",
                    "languageVersion": "1.10-experimental",
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
        result.Template.Should().HaveValueAtPath("definitions", JToken.Parse("""
            {
                "foo": {
                    "type": "array",
                    "items": {
                        "$ref": "#/definitions/667DA15BA24E05778924A37D15988F81D593A3CD71B5CF17AF835094AEF6B0EF.bar"
                    }
                },
                "667DA15BA24E05778924A37D15988F81D593A3CD71B5CF17AF835094AEF6B0EF.bar": {
                    "type": "string"
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
                    "languageVersion": "1.10-experimental",
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
        result.Template.Should().HaveValueAtPath("definitions", JToken.Parse("""
            {
                "fizz": {
                    "type": "array",
                    "allowedValues": ["bar", "foo"]
                },
                "buzz": {
                    "type": "string",
                    "allowedValues": ["bar", "foo"]
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
                """));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("definitions", JToken.Parse("""
            {
                "F50265A833D6D0931D901EA006975762A8CB6AC8CA807421DA00C1DC781CAC2C.foo": {
                    "type": "array",
                    "items": {
                        "type": "string"
                    }
                }
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
        result.Template.Should().HaveValueAtPath("definitions", JToken.Parse("""
            {
                "foo": {
                    "type": "array",
                    "items": {
                        "type": "string"
                    }
                },
                "F50265A833D6D0931D901EA006975762A8CB6AC8CA807421DA00C1DC781CAC2C.fizz": {
                    "type": "int",
                    "minValue": 1,
                    "maxValue": 10
                }
            }
            """));
    }

    [TestMethod]
    public void Imported_symbols_with_a_lengthy_reference_chain_should_have_declarations_injected_into_compiled_template()
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
        result.Template.Should().HaveValueAtPath("definitions", JToken.Parse("""
            {
                "foo": {
                    "type": "object",
                    "properties": {
                        "bar": {
                            "$ref": "#/definitions/651C7BC080E02B2F6882703968B1EDF5B6AA76E92C6FACFC0F9050C3023C2331.bar"
                        },
                        "anotherProperty": {
                            "$ref": "#/definitions/F50265A833D6D0931D901EA006975762A8CB6AC8CA807421DA00C1DC781CAC2C.unexported"
                        }
                    }
                },
                "F50265A833D6D0931D901EA006975762A8CB6AC8CA807421DA00C1DC781CAC2C.unexported": {
                    "type": "string",
                    "allowedValues": [
                        "buzz",
                        "fizz",
                        "pop"
                    ]
                },
                "651C7BC080E02B2F6882703968B1EDF5B6AA76E92C6FACFC0F9050C3023C2331.bar": {
                    "type": "object",
                    "properties": {
                        "foo": {
                            "$ref": "#/definitions/5F9E55E40B10AD26BDDC7BBB870824D715967A0FDF6F44AB8A392F717C3FB5A0.bar"
                        },
                        "prop": {
                            "$ref": "#/definitions/651C7BC080E02B2F6882703968B1EDF5B6AA76E92C6FACFC0F9050C3023C2331.unexported"
                        }
                    }
                },
                "651C7BC080E02B2F6882703968B1EDF5B6AA76E92C6FACFC0F9050C3023C2331.unexported": {
                    "type": "object",
                    "properties": {
                        "nested": {
                            "$ref": "#/definitions/651C7BC080E02B2F6882703968B1EDF5B6AA76E92C6FACFC0F9050C3023C2331.alsoNotExported"
                        }
                    }
                },
                "651C7BC080E02B2F6882703968B1EDF5B6AA76E92C6FACFC0F9050C3023C2331.alsoNotExported": {
                    "type": "int"
                },
                "5F9E55E40B10AD26BDDC7BBB870824D715967A0FDF6F44AB8A392F717C3FB5A0.bar": {
                    "$ref": "#/definitions/DC87F6056263791CB99D224E81E281E05A23CE9F66479A7CF2F5138CCD9412B6.foo"
                },
                "DC87F6056263791CB99D224E81E281E05A23CE9F66479A7CF2F5138CCD9412B6.foo": {
                    "type": "array",
                    "items": {
                        "type": "string"
                    }
                }
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
            ("BCP359", DiagnosticLevel.Error, "This symbol is imported multiple times under the names 'foo', 'fizz'."),
            ("BCP359", DiagnosticLevel.Error, "This symbol is imported multiple times under the names 'foo', 'fizz'."), // The same diagnostic should be raised on each import
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
                    "languageVersion": "1.10-experimental",
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
            ("BCP359", DiagnosticLevel.Error, "This symbol is imported multiple times under the names 'foo', 'fizz'."),
            ("BCP359", DiagnosticLevel.Error, "This symbol is imported multiple times under the names 'foo', 'fizz'."), // The same diagnostic should be raised on each import
        });
    }
}
