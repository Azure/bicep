// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class UserDefinedTypeTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public void Type_declarations_are_enabled()
    {
        var result = CompilationHelper.Compile(@"
type myString = string
");
        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Inline_object_types_are_enabled()
    {
        var result = CompilationHelper.Compile(@"
param complexParam {
    property1: string
    property2: string
}
");
        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Inline_type_literals_are_enabled()
    {
        var result = CompilationHelper.Compile(@"
param thirtyThreeParam 33
");
        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Inline_union_types_are_enabled()
    {
        var result = CompilationHelper.Compile(@"
param oneOfSeveralStrings 'this one'|'that one'|'perhaps this one instead'
");
        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Nullable_types_are_enabled()
    {
        var result = CompilationHelper.Compile(@"
param nullableString string?
");
        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Namespaces_cannot_be_used_as_types()
    {
        var result = CompilationHelper.Compile(@"
param azParam az
");
        result.Should().ContainDiagnostic("BCP306", DiagnosticLevel.Error, "The name \"az\" refers to a namespace, not to a type.");
    }

    [TestMethod]
    public void Namespaces_cannot_be_assigned_to_types()
    {
        var result = CompilationHelper.Compile(@"
type sysAlias = sys
");
        result.Should().ContainDiagnostic("BCP306", DiagnosticLevel.Error, "The name \"sys\" refers to a namespace, not to a type.");
    }

    [TestMethod]
    public void Masked_types_still_accessible_via_qualified_reference()
    {
        var result = CompilationHelper.Compile(@"
type string = int

param stringParam string = 'foo'
");

        result.Should().HaveDiagnostics(new[] {
            ("no-unused-params", DiagnosticLevel.Warning, "Parameter \"stringParam\" is declared but never used."),
            ("BCP033", DiagnosticLevel.Error, "Expected a value of type \"int\" but the provided value is of type \"'foo'\"."),
        });

        // fix by fully-qualifying
        result = CompilationHelper.Compile(@"
type string = int

param stringParam sys.string = 'foo'
");

        result.Should().HaveDiagnostics(new[] {
            ("no-unused-params", DiagnosticLevel.Warning, "Parameter \"stringParam\" is declared but never used."),
        });
    }

    [TestMethod]
    public void Constraint_decorators_prohibited_on_type_refs()
    {
        var result = CompilationHelper.Compile(@"
@minLength(3)
@maxLength(5)
@description('A string with a bunch of constraints')
type constrainedString = string

@minValue(3)
@maxValue(5)
@description('An int with a bunch of constraints')
type constrainedInt = int

@minLength(3)
@maxLength(5)
@description('A type alias with a bunch of constraints pointing to another type alias')
type constrainedStringAlias = constrainedString

@minValue(3)
@maxValue(5)
@description('A type alias with a bunch of constraints pointing to another type alias')
type constrainedIntAlias = constrainedInt

@minLength(3)
@maxLength(5)
@secure()
@allowed(['fizz', 'buzz', 'pop'])
@description('A parameter with a bunch of constraints that uses a type alias')
param stringParam constrainedString

@minValue(3)
@maxValue(5)
@allowed([3, 4, 5])
@description('A parameter with a bunch of constraints that uses a type alias')
param intParam constrainedInt
");

        result.Should().HaveDiagnostics(new[] {
            ("BCP308", DiagnosticLevel.Error, "The decorator \"minLength\" may not be used on statements whose declared type is a reference to a user-defined type."),
            ("BCP308", DiagnosticLevel.Error, "The decorator \"maxLength\" may not be used on statements whose declared type is a reference to a user-defined type."),
            ("BCP308", DiagnosticLevel.Error, "The decorator \"minValue\" may not be used on statements whose declared type is a reference to a user-defined type."),
            ("BCP308", DiagnosticLevel.Error, "The decorator \"maxValue\" may not be used on statements whose declared type is a reference to a user-defined type."),
            ("BCP308", DiagnosticLevel.Error, "The decorator \"minLength\" may not be used on statements whose declared type is a reference to a user-defined type."),
            ("BCP308", DiagnosticLevel.Error, "The decorator \"maxLength\" may not be used on statements whose declared type is a reference to a user-defined type."),
            ("BCP308", DiagnosticLevel.Error, "The decorator \"secure\" may not be used on statements whose declared type is a reference to a user-defined type."),
            ("BCP308", DiagnosticLevel.Error, "The decorator \"allowed\" may not be used on statements whose declared type is a reference to a user-defined type."),
            ("no-unused-params", DiagnosticLevel.Warning, "Parameter \"stringParam\" is declared but never used."),
            ("BCP308", DiagnosticLevel.Error, "The decorator \"minValue\" may not be used on statements whose declared type is a reference to a user-defined type."),
            ("BCP308", DiagnosticLevel.Error, "The decorator \"maxValue\" may not be used on statements whose declared type is a reference to a user-defined type."),
            ("BCP308", DiagnosticLevel.Error, "The decorator \"allowed\" may not be used on statements whose declared type is a reference to a user-defined type."),
            ("no-unused-params", DiagnosticLevel.Warning, "Parameter \"intParam\" is declared but never used."),
        });
    }

    [TestMethod]
    public void Allowed_decorator_may_not_be_used_on_literal_and_union_typed_parameters()
    {
        var result = CompilationHelper.Compile(@"
@allowed([true])
param trueParam true

@allowed([false])
param falseParam !true

@allowed([1])
param oneParam 1

@allowed([-1])
param negativeOneParam -1

@allowed([{fizz: 'buzz'}])
param fizzBuzzParam {fizz: 'buzz'}

@allowed(['fizz'])
param fizzParam 'fizz'

@allowed(['fizz', 'buzz', 'pop'])
param fizzBuzzPopParam 'fizz'|'buzz'|'pop'
");

        result.Should().HaveDiagnostics(new[] {
            ("BCP295", DiagnosticLevel.Error, "The 'allowed' decorator may not be used on targets of a union or literal type. The allowed values for this parameter or type definition will be derived from the union or literal type automatically."),
            ("no-unused-params", DiagnosticLevel.Warning, "Parameter \"trueParam\" is declared but never used."),
            ("BCP295", DiagnosticLevel.Error, "The 'allowed' decorator may not be used on targets of a union or literal type. The allowed values for this parameter or type definition will be derived from the union or literal type automatically."),
            ("no-unused-params", DiagnosticLevel.Warning, "Parameter \"falseParam\" is declared but never used."),
            ("BCP295", DiagnosticLevel.Error, "The 'allowed' decorator may not be used on targets of a union or literal type. The allowed values for this parameter or type definition will be derived from the union or literal type automatically."),
            ("no-unused-params", DiagnosticLevel.Warning, "Parameter \"oneParam\" is declared but never used."),
            ("BCP295", DiagnosticLevel.Error, "The 'allowed' decorator may not be used on targets of a union or literal type. The allowed values for this parameter or type definition will be derived from the union or literal type automatically."),
            ("no-unused-params", DiagnosticLevel.Warning, "Parameter \"negativeOneParam\" is declared but never used."),
            ("BCP295", DiagnosticLevel.Error, "The 'allowed' decorator may not be used on targets of a union or literal type. The allowed values for this parameter or type definition will be derived from the union or literal type automatically."),
            ("no-unused-params", DiagnosticLevel.Warning, "Parameter \"fizzBuzzParam\" is declared but never used."),
            ("BCP295", DiagnosticLevel.Error, "The 'allowed' decorator may not be used on targets of a union or literal type. The allowed values for this parameter or type definition will be derived from the union or literal type automatically."),
            ("no-unused-params", DiagnosticLevel.Warning, "Parameter \"fizzParam\" is declared but never used."),
            ("BCP295", DiagnosticLevel.Error, "The 'allowed' decorator may not be used on targets of a union or literal type. The allowed values for this parameter or type definition will be derived from the union or literal type automatically."),
            ("no-unused-params", DiagnosticLevel.Warning, "Parameter \"fizzBuzzPopParam\" is declared but never used."),
        });
    }

    [TestMethod]
    public void Unions_that_incorporate_their_parent_object_do_not_blow_the_stack()
    {
        var blockedBecauseOfCycle = CompilationHelper.Compile(@"
type anObject = {
    recur: {foo: 'bar'}|anObject
}
");

        blockedBecauseOfCycle.Should().HaveDiagnostics(new[] {
            ("BCP298", DiagnosticLevel.Error, "This type definition includes itself as required component, which creates a constraint that cannot be fulfilled."),
            ("BCP293", DiagnosticLevel.Error, "All members of a union type declaration must be literal values."),
        });

        var blockedBecauseOfUnionSemantics = CompilationHelper.Compile(@"
type anObject = {
    recur: {foo: 'bar'}|anObject?
}
");

        blockedBecauseOfUnionSemantics.Should().HaveDiagnostics(new[] {
            ("BCP293", DiagnosticLevel.Error, "All members of a union type declaration must be literal values."),
        });
    }

    [TestMethod]
    public void Unary_operations_that_incorporate_their_parent_object_do_not_blow_the_stack()
    {
        var blockedBecauseOfCycle = CompilationHelper.Compile(@"
type anObject = {
    recur: !anObject
}
");

        blockedBecauseOfCycle.Should().HaveDiagnostics(new[] {
            ("BCP285", DiagnosticLevel.Error, "The type expression could not be reduced to a literal value."),
        });
    }

    [TestMethod]
    public void Arrays_that_incorporate_their_parent_object_do_not_blow_the_stack()
    {
        var blockedBecauseOfCycle = CompilationHelper.Compile(@"
type anObject = {
    recur: anObject[]
}
");

        blockedBecauseOfCycle.Should().HaveDiagnostics(new[] {
            ("BCP298", DiagnosticLevel.Error, "This type definition includes itself as required component, which creates a constraint that cannot be fulfilled."),
        });

        var permitted = CompilationHelper.Compile(@"
type anObject = {
    recur: (anObject?)[]
}
");

        permitted.Should().NotHaveAnyDiagnostics();

        permitted = CompilationHelper.Compile(@"
type anArray = (anArray?)[]
");

        permitted.Should().NotHaveAnyDiagnostics();

        permitted = CompilationHelper.Compile(@"
type anArray = anArray[]?
");

        permitted.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Cyclic_nullables_do_not_blow_the_stack()
    {
        var blockedBecauseOfCycle = CompilationHelper.Compile(@"
type nullable = nullable?
");

        blockedBecauseOfCycle.Should().HaveDiagnostics(new[] {
            ("BCP298", DiagnosticLevel.Error, "This type definition includes itself as required component, which creates a constraint that cannot be fulfilled."),
        });
    }

    [TestMethod]
    public void Tuples_that_incorporate_their_parent_object_do_not_blow_the_stack()
    {
        var blockedBecauseOfCycle = CompilationHelper.Compile(@"
type anObject = {
    recur: [anObject]
}
");

        blockedBecauseOfCycle.Should().HaveDiagnostics(new[] {
            ("BCP298", DiagnosticLevel.Error, "This type definition includes itself as required component, which creates a constraint that cannot be fulfilled."),
        });

        var permitted = CompilationHelper.Compile(@"
type anObject = {
    recur: [anObject]?
}
");

        permitted.Should().NotHaveAnyDiagnostics();

        permitted = CompilationHelper.Compile(@"
type anObject = {
    recur: [anObject?]
}
");

        permitted.Should().NotHaveAnyDiagnostics();

        permitted = CompilationHelper.Compile(@"
type aTuple = [aTuple?]
");

        permitted.Should().NotHaveAnyDiagnostics();

        permitted = CompilationHelper.Compile(@"
type aTuple = [aTuple]?
");

        permitted.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Objects_that_incorporate_their_parent_object_do_not_blow_the_stack()
    {
        var blockedBecauseOfCycle = CompilationHelper.Compile(@"
type anObject = {
    recurEventually: {
        recurNow: anObject
    }
}
");

        blockedBecauseOfCycle.Should().HaveDiagnostics(new[] {
            ("BCP298", DiagnosticLevel.Error, "This type definition includes itself as required component, which creates a constraint that cannot be fulfilled."),
        });

        var permitted = CompilationHelper.Compile(@"
type anObject = {
    recurEventually: {
        recurNow: anObject?
    }
}
");

        permitted.Should().NotHaveAnyDiagnostics();

        permitted = CompilationHelper.Compile(@"
type anObject = {
    recurEventually: {
        recurNow: anObject
    }?
}
");

        permitted.Should().NotHaveAnyDiagnostics();

        permitted = CompilationHelper.Compile(@"
type anObject = {
    recurEventually: {
        recurNow: anObject
    }
}?
");

        permitted.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Cyclic_check_understands_nullability_modifiers()
    {
        var blockedBecauseOfCycle = CompilationHelper.Compile(@"
type anObject = {
    recurEventually: {
        recurNow: anObject!
    }
}?
");

        blockedBecauseOfCycle.Should().HaveDiagnostics(new[] {
            ("BCP298", DiagnosticLevel.Error, "This type definition includes itself as required component, which creates a constraint that cannot be fulfilled."),
        });

        blockedBecauseOfCycle = CompilationHelper.Compile(@"
type anObject = {
    recurEventually: {
        recurNow: anObject
    }
}?!
");

        blockedBecauseOfCycle.Should().HaveDiagnostics(new[] {
            ("BCP298", DiagnosticLevel.Error, "This type definition includes itself as required component, which creates a constraint that cannot be fulfilled."),
        });

        var permitted = CompilationHelper.Compile(@"
type anObject = {
    recurEventually: {
        recurNow: anObject!?
    }
}?
");

        permitted.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Warning_should_be_shown_when_reading_unknown_properties_on_unsealed_objects()
    {
        var result = CompilationHelper.Compile(@"
param anObject {}

output prop string = anObject.prop
");

        result.Should().HaveDiagnostics(new[] {
            ("BCP187", DiagnosticLevel.Warning, "The property \"prop\" does not exist in the resource or type definition, although it might still be valid. If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
        });
    }

    [TestMethod]
    public void Warning_should_be_shown_when_setting_unknown_properties_on_unsealed_objects()
    {
        var result = CompilationHelper.Compile(@"
#disable-next-line no-unused-params
param anObject {} = {prop: 'someVal'}

#disable-next-line no-unused-params
param anotherObject object = {prop: 'someVal'}
");

        result.Should().HaveDiagnostics(new[] {
            ("BCP037", DiagnosticLevel.Warning, "The property \"prop\" is not allowed on objects of type \"{ }\". No other properties are allowed."),
        });
    }

    [TestMethod]
    public void Error_should_be_shown_when_setting_unknown_properties_that_do_not_match_additional_properties_type()
    {
        var result = CompilationHelper.Compile(@"
#disable-next-line no-unused-params
param aDict {
  *: int
} = {prop: 'someVal'}
");

        result.Should().HaveDiagnostics(new[] {
            ("BCP036", DiagnosticLevel.Error, @"The property ""prop"" expected a value of type ""int"" but the provided value is of type ""'someVal'""."),
        });

        result = CompilationHelper.Compile(@"
#disable-next-line no-unused-params
param aDict {
  *: string
} = {prop: 'someVal'}
");

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Additional_properties_may_be_used_alongside_named_properties()
    {
        var result = CompilationHelper.Compile(@"
#disable-next-line no-unused-params
param aDict {
  knownProp: int
  *: string
} = {
  knownProp: 21
  prop: 'someVal'
}
");

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Constraint_decorators_can_be_used_on_nullably_typed_params()
    {
        var result = CompilationHelper.Compile(@"
@minLength(3)
@maxLength(10)
@secure()
#disable-next-line no-unused-params
param constrainedString string?

@minValue(3)
@maxValue(10)
type constrainedInt = int?

@minLength(3)
@maxLength(10)
type constrainedArray = array?

@sealed()
@secure()
#disable-next-line no-unused-params
param sealedObject {}?
");

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Nullably_typed_values_can_be_used_as_nonnullable_outputs_with_postfix_assertion()
    {
        var templateWithPossiblyNullDeref = @"
param foos (null | { bar: { baz: { quux: 'quux' } } })[]

output quux string = foos[0].bar.baz.quux
";
        var templateWithNonNullAssertion = @"
param foos (null | { bar: { baz: { quux: 'quux' } } })[]

output quux string = foos[0]!.bar.baz.quux
";
        var templateWithSafeDeref = @"
param foos (null | { bar: { baz: { quux: 'quux' } } })[]

output quux string = foos[0].?bar.baz.quux
";

        var result = CompilationHelper.Compile(templateWithPossiblyNullDeref);
        result.Should().HaveDiagnostics(new[]
        {
          ("BCP318", DiagnosticLevel.Warning, @"The value of type ""null | { bar: { baz: { quux: 'quux' } } }"" may be null at the start of the deployment, which would cause this access expression (and the overall deployment with it) to fail."),
        });

        result.Diagnostics.Single().Should().BeAssignableTo<IFixable>();
        var fixAlternatives = new HashSet<string> { templateWithNonNullAssertion, templateWithSafeDeref };
        foreach (var fix in result.Diagnostics.Single().As<IFixable>().Fixes)
        {
            fix.Replacements.Should().HaveCount(1);
            var replacement = fix.Replacements.Single();

            var actualText = templateWithPossiblyNullDeref.Remove(replacement.Span.Position, replacement.Span.Length);
            actualText = actualText.Insert(replacement.Span.Position, replacement.Text);

            fixAlternatives.Remove(actualText);
        }

        fixAlternatives.Should().BeEmpty();

        result = CompilationHelper.Compile(templateWithNonNullAssertion);
        result.Should().NotHaveAnyDiagnostics();
        result.Should().HaveTemplateWithOutput("quux", "[parameters('foos')[0].bar.baz.quux]");
    }

    [TestMethod]
    public void Error_should_be_emitted_when_setting_a_default_value_on_a_nullable_parameter()
    {
        var result = CompilationHelper.Compile(@"
#disable-next-line no-unused-params
param myParam string? = 'foo'
");

        result.Should().HaveDiagnostics(new[] {
            ("BCP326", DiagnosticLevel.Error, "Nullable-typed parameters may not be assigned default values. They have an implicit default of 'null' that cannot be overridden."),
        });
    }

    [TestMethod]
    public void Tuples_with_a_literal_index_use_type_at_index()
    {
        var result = CompilationHelper.Compile(
("main.bicep", @"
var myArray = ['foo', 'bar']

module mod './mod.bicep' = {
  name: 'mod'
  params: {
    myParam: myArray[0]
  }
}
"),
("mod.bicep", @"
param myParam 'foo'
"));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
    }

    [TestMethod]
    public void Tuples_with_a_literal_union_index_use_type_at_indices()
    {
        var result = CompilationHelper.Compile(
("main.bicep", @"
param index 0 | 1

var myArray = ['foo', 'bar', 'baz']

module mod './mod.bicep' = {
  name: 'mod'
  params: {
    myParam: myArray[index]
  }
}
"),
("mod.bicep", @"
param myParam 'foo' | 'bar'
"));

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
    }

    [TestMethod]
    public void Constraint_decorators_permitted_on_outputs()
    {
        var result = CompilationHelper.Compile(@"
@minLength(3)
@maxLength(5)
@description('A string with a bunch of constraints')
output foo string = 'foo'
");

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void User_defined_types_may_be_used_with_outputs()
    {
        var result = CompilationHelper.Compile(@"
@minLength(3)
@maxLength(4)
type constrainedString = string

output arrayOfConstrainedStrings constrainedString[] = ['fizz', 'buzz', 'pop']
");

        result.Should().NotHaveAnyDiagnostics();
    }

    public void Type_aliases_incorporate_modifiers_into_type()
    {
        var result = CompilationHelper.Compile(@"
@maxLength(2)
type shortString = string

param myString shortString = 'foo'
");

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP332", DiagnosticLevel.Error, "The provided value (whose length will always be greater than or equal to 3) is too long to assign to a target for which the maximum allowable length is 2."),
        });
    }

    [TestMethod]
    public void Impossible_integer_domains_raise_descriptive_error()
    {
        var result = CompilationHelper.Compile(@"
@minValue(1)
@maxValue(0)
param myParam int
");

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP331", DiagnosticLevel.Error, "A type's \"minValue\" must be less than or equal to its \"maxValue\", but a minimum of 1 and a maximum of 0 were specified."),
        });
    }

    [TestMethod]
    public void Impossible_array_length_domains_raise_descriptive_error()
    {
        var result = CompilationHelper.Compile(@"
@minLength(1)
@maxLength(0)
param myParam array
");

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP331", DiagnosticLevel.Error, "A type's \"minLength\" must be less than or equal to its \"maxLength\", but a minimum of 1 and a maximum of 0 were specified."),
        });
    }

    [TestMethod]
    public void Impossible_string_length_domains_raise_descriptive_error()
    {
        var result = CompilationHelper.Compile(@"
@minLength(1)
@maxLength(0)
param myParam string
");

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP331", DiagnosticLevel.Error, "A type's \"minLength\" must be less than or equal to its \"maxLength\", but a minimum of 1 and a maximum of 0 were specified."),
        });
    }

    [TestMethod]
    public void Duplicate_property_names_should_raise_descriptive_diagnostic()
    {
        var result = CompilationHelper.Compile("""
            type foo = {
                bar: bool
                bar: string
            }
            """);

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP025", DiagnosticLevel.Error, "The property \"bar\" is declared multiple times in this object. Remove or rename the duplicate properties.")
        });
    }

    [TestMethod]
    public void Union_types_with_single_normalized_member_compile_without_error()
    {
        var result = CompilationHelper.Compile("""
            type union = 'a' | 'a'
            """);

        result.Should().NotHaveAnyDiagnostics();

        result.Template.Should().NotBeNull();
        result.Template!.Should().HaveValueAtPath("definitions.union", JToken.Parse("""
            {
                "type": "string",
                "allowedValues": ["a"]
            }
            """));
    }

    [TestMethod]
    public void Nullable_union_types_do_not_include_null_in_allowed_values_constraint()
    {
        var result = CompilationHelper.Compile("""
            type union = 'a' | 'b' | 'c' | null
            type unionWithOneMember = null | 'a'
            """);

        result.Should().NotHaveAnyDiagnostics();

        result.Template.Should().NotBeNull();
        result.Template!.Should().HaveValueAtPath("definitions.union", JToken.Parse("""
            {
                "type": "string",
                "allowedValues": ["a", "b", "c"],
                "nullable": true
            }
            """));
        result.Template!.Should().HaveValueAtPath("definitions.unionWithOneMember", JToken.Parse("""
            {
                "type": "string",
                "allowedValues": ["a"],
                "nullable": true
            }
            """));
    }

    [TestMethod]
    public void Param_with_null_in_allowedValues_constraint_can_be_loaded()
    {
        var result = CompilationHelper.Compile(
            ("main.bicep", """
                module mod 'mod.json' = {
                    name: 'mod'
                    params: {
                        foo: 'foo'
                    }
                }
                """),
            ("mod.json", """
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "languageVersion": "2.0",
                    "contentVersion": "1.0.0.0",
                    "parameters": {
                        "foo": {
                            "type": "string",
                            "allowedValues": ["foo", "bar", "baz", null]
                        }
                    },
                    "resources": []
                }
                """));

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Self_reference_permitted_in_object_type_additional_properties()
    {
        var result = CompilationHelper.Compile("""
            type anObject = {
                id: int
                flag: bool
                someData: string
                *: anObject
            }
            """);

        result.Should().NotHaveAnyDiagnostics();
    }

    // https://github.com/azure/bicep/issues/12070
    [TestMethod]
    public void Self_property_deref_does_not_blow_the_stack()
    {
        var result = CompilationHelper.Compile("""
            type anObject = {
                property: anObject.property
            }
            """);

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP298", DiagnosticLevel.Error, "This type definition includes itself as required component, which creates a constraint that cannot be fulfilled."),
            ("BCP062", DiagnosticLevel.Error, "The referenced declaration with name \"anObject\" is not valid."),
        });
    }

    // https://github.com/azure/bicep/issues/12070
    [TestMethod]
    public void Self_array_access_does_not_blow_the_stack()
    {
        var result = CompilationHelper.Compile("""
            type anObject = {
                property: anObject['property']
            }
            """);

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP298", DiagnosticLevel.Error, "This type definition includes itself as required component, which creates a constraint that cannot be fulfilled."),
            ("BCP289", DiagnosticLevel.Error, "The type definition is not valid."),
        });
    }

    [TestMethod]
    [Timeout(5_000)]
    public void Parsing_incomplete_tuple_type_expressions_halts()
    {
        var result = CompilationHelper.Compile("""
            type myType = {
                name: [string
            }
            """);

        result.Template.Should().BeNull();
    }
}
