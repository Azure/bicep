// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
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
            ("BCP187", DiagnosticLevel.Warning, "The property \"prop\" does not exist in the resource or type definition, although it might still be valid. If this is a resource type definition inaccuracy, report it using https://aka.ms/bicep-type-issues."),
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
    public void Empty_object_should_be_distinguishable_from_untyped_object_in_compiled_JSON()
    {
        var result = CompilationHelper.Compile("""
            type emptyObject = {}
            type untypedObject = object
            """);

        result.Template.Should().HaveValueAtPath("definitions.emptyObject.properties", new JObject());
        result.Template.Should().NotHaveValueAtPath("definitions.untypedObject.properties");
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
            ("BCP062", DiagnosticLevel.Error, "The referenced declaration with name \"anObject\" is not valid."),
        });
    }

    [TestMethod]
    [Timeout(5_0000)]
    public void Parsing_incomplete_tuple_type_expressions_halts()
    {
        var result = CompilationHelper.Compile("""
            type myType = {
                name: [string
            }
            """);

        result.Template.Should().BeNull();
    }

    [TestMethod]
    public void Resource_derived_type_should_compile_successfully()
    {
        var result = CompilationHelper.Compile(new UnitTests.ServiceBuilder().WithFeatureOverrides(new(TestContext)),
            """
            type myType = resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.name
            """);

        result.Template.Should().HaveValueAtPath("definitions", JToken.Parse($$"""
            {
                "myType": {
                    "type": "string",
                    "metadata": {
                        "{{LanguageConstants.MetadataResourceDerivedTypePropertyName}}": {
                            "{{LanguageConstants.MetadataResourceDerivedTypePointerPropertyName}}": "Microsoft.Storage/storageAccounts@2022-09-01#properties/name"
                        }
                    }
                }
            }
            """));
    }

    [TestMethod]
    public void Resource_derived_type_should_compile_successfully_with_namespace_qualified_syntax()
    {
        var result = CompilationHelper.Compile(new UnitTests.ServiceBuilder().WithFeatureOverrides(new(TestContext)),
            """
            var resource = 'foo'
            type myType = sys.resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.name
            """);

        result.Template.Should().HaveValueAtPath("definitions", JToken.Parse($$"""
            {
                "myType": {
                    "type": "string",
                    "metadata": {
                        "{{LanguageConstants.MetadataResourceDerivedTypePropertyName}}": {
                            "{{LanguageConstants.MetadataResourceDerivedTypePointerPropertyName}}": "Microsoft.Storage/storageAccounts@2022-09-01#properties/name"
                        }
                    }
                }
            }
            """));
    }

    [TestMethod]
    public void Param_with_resource_derived_type_can_be_loaded()
    {
        var result = CompilationHelper.Compile(new UnitTests.ServiceBuilder().WithFeatureOverrides(new(TestContext)),
            ("main.bicep", """
                param location string = resourceGroup().location

                module mod 'mod.json' = {
                    name: 'mod'
                    params: {
                        foo: {
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
                    }
                }
                """),
            ("mod.json", $$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "languageVersion": "2.0",
                    "contentVersion": "1.0.0.0",
                    "parameters": {
                        "foo": {
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
    public void Param_with_resource_derived_type_property_can_be_loaded()
    {
        var result = CompilationHelper.Compile(new UnitTests.ServiceBuilder().WithFeatureOverrides(new(TestContext)),
            ("main.bicep", """
                @minLength(2)
                param saName string

                module mod 'mod.json' = {
                    name: 'mod'
                    params: {
                        saName: saName
                        connectionParameterType: 'sting'
                        ipRuleAction: 'Deny'
                    }
                }
                """),
            ("mod.json", $$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "languageVersion": "2.0",
                    "contentVersion": "1.0.0.0",
                    "parameters": {
                        "saName": {
                            "type": "string",
                            "metadata": {
                                "{{LanguageConstants.MetadataResourceDerivedTypePropertyName}}": "Microsoft.Storage/storageAccounts@2022-09-01#properties/name"
                            }
                        },
                        "connectionParameterType": {
                            "type": "string",
                            "metadata": {
                                "{{LanguageConstants.MetadataResourceDerivedTypePropertyName}}": "Microsoft.Web/customApis@2016-06-01#properties/properties/properties/connectionParameters/additionalProperties/properties/type"
                            }
                        },
                        "ipRuleAction": {
                            "type": "string",
                            "metadata": {
                                "{{LanguageConstants.MetadataResourceDerivedTypePropertyName}}": "Microsoft.Storage/storageAccounts@2022-09-01#properties/properties/properties/networkAcls/properties/ipRules/items/properties/action"
                            }
                        }
                    },
                    "resources": []
                }
                """));

        result.Should().NotHaveAnyCompilationBlockingDiagnostics();
        result.Should().HaveDiagnostics(new[]
        {
            ("BCP334", DiagnosticLevel.Warning, "The provided value can have a length as small as 2 and may be too short to assign to a target with a configured minimum length of 3."),
            ("BCP088", DiagnosticLevel.Warning, """The property "connectionParameterType" expected a value of type "'array' | 'bool' | 'connection' | 'int' | 'oauthSetting' | 'object' | 'secureobject' | 'securestring' | 'string'" but the provided value is of type "'sting'". Did you mean "'string'"?"""),
            ("BCP036", DiagnosticLevel.Warning, """The property "ipRuleAction" expected a value of type "'Allow'" but the provided value is of type "'Deny'"."""),
        });

    }

    [TestMethod]
    public void Output_with_resource_derived_type_can_be_loaded()
    {
        var result = CompilationHelper.Compile(new UnitTests.ServiceBuilder().WithFeatureOverrides(new(TestContext)),
            ("main.bicep", """
                module mod 'mod.json' = {
                    name: 'mod'
                }

                output out string = mod.outputs.foo.bar.properties.unknownProperty
                """),
            ("mod.json", $$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "languageVersion": "2.0",
                    "contentVersion": "1.0.0.0",
                    "outputs": {
                        "foo": {
                            "type": "object",
                            "additionalProperties": {
                                "type": "object",
                                "metadata": {
                                    "{{LanguageConstants.MetadataResourceDerivedTypePropertyName}}": "Microsoft.Storage/storageAccounts@2022-09-01"
                                }
                            },
                            "value": {}
                        }
                    },
                    "resources": []
                }
                """));

        result.Should().NotHaveAnyCompilationBlockingDiagnostics();
        result.Should().HaveDiagnostics(new[]
        {
            ("BCP053", DiagnosticLevel.Warning, """The type "StorageAccountPropertiesCreateParametersOrStorageAccountProperties" does not contain property "unknownProperty". Available properties include "accessTier", "allowBlobPublicAccess", "allowCrossTenantReplication", "allowedCopyScope", "allowSharedKeyAccess", "azureFilesIdentityBasedAuthentication", "blobRestoreStatus", "creationTime", "customDomain", "defaultToOAuthAuthentication", "dnsEndpointType", "encryption", "failoverInProgress", "geoReplicationStats", "immutableStorageWithVersioning", "isHnsEnabled", "isLocalUserEnabled", "isNfsV3Enabled", "isSftpEnabled", "keyCreationTime", "keyPolicy", "largeFileSharesState", "lastGeoFailoverTime", "minimumTlsVersion", "networkAcls", "primaryEndpoints", "primaryLocation", "privateEndpointConnections", "provisioningState", "publicNetworkAccess", "routingPreference", "sasPolicy", "secondaryEndpoints", "secondaryLocation", "statusOfPrimary", "statusOfSecondary", "storageAccountSkuConversionStatus", "supportsHttpsTrafficOnly"."""),
        });
    }

    // https://github.com/azure/bicep/issues/12920
    [TestMethod]
    public void Type_property_access_is_valid_type()
    {
        var result = CompilationHelper.Compile("""
            type test2 = {
              foo: {
                bar: string
              }
            }

            type test3 = test2.foo
            """);

        result.Should().NotHaveAnyCompilationBlockingDiagnostics();
        result.Template.Should().HaveValueAtPath("definitions.test3", JToken.Parse("""
            {
              "$ref": "#/definitions/test2/properties/foo"
            }
            """));
    }

    // cf https://www.rfc-editor.org/rfc/rfc6901#section-6
    [TestMethod]
    public void Type_property_access_is_escaped_correctly()
    {
        var result = CompilationHelper.Compile("""
            type test = {
              '': string
              'a/b': int
              'c%d': bool
              'e^f': string
              'g|h': int
              'i\\j': bool
              'k"l': string
              ' ': int
              'm~n': bool
            }

            type test1 = test['']
            type test2 = test['a/b']
            type test3 = test['c%d']
            type test4 = test['e^f']
            type test5 = test['g|h']
            type test6 = test['i\\j']
            type test7 = test['k"l']
            type test8 = test[' ']
            type test9 = test['m~n']
            """);

        result.Should().NotHaveAnyCompilationBlockingDiagnostics();
        result.Template.Should().HaveValueAtPath("definitions", JToken.Parse("""
            {
              "test": {
                "type": "object",
                "properties": {
                  "": {
                    "type": "string"
                  },
                  "a/b": {
                    "type": "int"
                  },
                  "c%d": {
                    "type": "bool"
                  },
                  "e^f": {
                    "type": "string"
                  },
                  "g|h": {
                    "type": "int"
                  },
                  "i\\j": {
                    "type": "bool"
                  },
                  "k\"l": {
                    "type": "string"
                  },
                  " ": {
                    "type": "int"
                  },
                  "m~n": {
                    "type": "bool"
                  }
                }
              },
              "test1": {
                "$ref": "#/definitions/test/properties/"
              },
              "test2": {
                "$ref": "#/definitions/test/properties/a~1b"
              },
              "test3": {
                "$ref": "#/definitions/test/properties/c%25d"
              },
              "test4": {
                "$ref": "#/definitions/test/properties/e%5Ef"
              },
              "test5": {
                "$ref": "#/definitions/test/properties/g%7Ch"
              },
              "test6": {
                "$ref": "#/definitions/test/properties/i%5Cj"
              },
              "test7": {
                "$ref": "#/definitions/test/properties/k%22l"
              },
              "test8": {
                "$ref": "#/definitions/test/properties/%20"
              },
              "test9": {
                "$ref": "#/definitions/test/properties/m~0n"
              }
            }
            """));
    }

    // https://github.com/azure/bicep/issues/12920
    [DataTestMethod]
    [DataRow("test.bar", "BCP053", """The type "{ foo: { bar: string } }" does not contain property "bar". Available properties include "foo".""")]
    [DataRow("{ foo: string }.foo", "BCP391", "Type member access is only supported on a reference to a named type.")]
    public void Invalid_type_property_access_raises_diagnostic(string accessExpression, string expectedErrorCode, string expectedErrorMessage)
    {
        var result = CompilationHelper.Compile($$"""
            type test = {
              foo: {
                bar: string
              }
            }

            type test2 = {{accessExpression}}
            """);

        result.Should().HaveDiagnostics(new[]
        {
            (expectedErrorCode, DiagnosticLevel.Error, expectedErrorMessage),
        });
    }

    [TestMethod]
    public void Type_property_access_can_be_used_on_resource_derived_types()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(TestContext)),
            ("main.bicep", """
                type storageAccountName = resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.name
                """));

        result.Should().NotHaveAnyCompilationBlockingDiagnostics();
        result.Template.Should().HaveValueAtPath("definitions.storageAccountName", JToken.Parse($$"""
            {
                "type": "string",
                "metadata": {
                    "{{LanguageConstants.MetadataResourceDerivedTypePropertyName}}": {
                        "{{LanguageConstants.MetadataResourceDerivedTypePointerPropertyName}}": "Microsoft.Storage/storageAccounts@2022-09-01#properties/name"
                    }
                }
            }
            """));
    }

    [TestMethod]
    public void Type_property_access_resolves_refs_and_traverses_imports()
    {
        var result = CompilationHelper.Compile(new ServiceBuilder().WithFeatureOverrides(new(TestContext)),
            ("types.bicep", """
                @export()
                type myObject = {
                  quux: int
                  saSku: resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'>.sku
                }
                """),
            ("main.bicep", """
                import * as types from 'types.bicep'

                type test = {
                  baz: types.myObject
                }

                type test2 = {
                  foo: {
                    bar: test
                  }
                }

                type test3 = test2.foo.bar.baz.quux
                type test4 = test2.foo.bar.baz.saSku.name
                """));

        result.Should().NotHaveAnyCompilationBlockingDiagnostics();
        result.Template.Should().HaveValueAtPath("definitions.test3", JToken.Parse("""
            {
              "$ref": "#/definitions/_1.myObject/properties/quux"
            }
            """));
        result.Template.Should().HaveValueAtPath("definitions.test4", JToken.Parse($$"""
            {
              "type": "string",
              "metadata": {
                "{{LanguageConstants.MetadataResourceDerivedTypePropertyName}}": {
                    "{{LanguageConstants.MetadataResourceDerivedTypePointerPropertyName}}": "Microsoft.Storage/storageAccounts@2022-09-01#properties/sku/properties/name"
                }
              }
            }
            """));
    }

    [TestMethod]
    public void Type_index_access_is_valid_type()
    {
        var result = CompilationHelper.Compile("""
            type test = [
              { bar: string }
            ]

            type test2 = test[0]
            """);

        result.Should().NotHaveAnyCompilationBlockingDiagnostics();
        result.Template.Should().HaveValueAtPath("definitions.test2", JToken.Parse("""
            {
              "$ref": "#/definitions/test/prefixItems/0"
            }
            """));
    }

    [DataTestMethod]
    [DataRow("test[1]", "BCP311", """The provided index value of "1" is not valid for type "[{ bar: string }]". Indexes for this type must be between 0 and 0.""")]
    [DataRow("test[-1]", "BCP387", "Indexing into a type requires an integer greater than or equal to 0.")]
    [DataRow("[string][0]", "BCP391", "Type member access is only supported on a reference to a named type.")]
    public void Invalid_type_index_access_raises_diagnostic(string accessExpression, string expectedErrorCode, string expectedErrorMessage)
    {
        var result = CompilationHelper.Compile($$"""
            type test = [
              { bar: string }
            ]

            type test2 = {{accessExpression}}
            """);

        result.Should().HaveDiagnostics(new[]
        {
            (expectedErrorCode, DiagnosticLevel.Error, expectedErrorMessage),
        });
    }

    [TestMethod]
    public void Type_index_access_resolves_refs_and_traverses_imports()
    {
        var result = CompilationHelper.Compile(
            ("types.bicep", """
                @export()
                type myTuple = [int, string]
                """),
            ("main.bicep", """
                import * as types from 'types.bicep'

                type test = [
                  types.myTuple
                ]

                type test2 = [string, bool, test]

                type test3 = test2[2][0][1]
                """));

        result.Should().NotHaveAnyCompilationBlockingDiagnostics();
        result.Template.Should().HaveValueAtPath("definitions.test3", JToken.Parse("""
            {
              "$ref": "#/definitions/_1.myTuple/prefixItems/1"
            }
            """));
    }

    [TestMethod]
    public void Type_additional_properties_access_is_valid_type()
    {
        var result = CompilationHelper.Compile("""
            type test = {
              foo: string
              bar: string
              *: int
            }

            type test2 = test.*
            """);

        result.Should().NotHaveAnyCompilationBlockingDiagnostics();
        result.Template.Should().HaveValueAtPath("definitions.test2", JToken.Parse("""
            {
              "$ref": "#/definitions/test/additionalProperties"
            }
            """));
    }

    [DataTestMethod]
    [DataRow("test.*", "BCP389", """The type "{ foo: string }" does not declare an additional properties type.""")]
    [DataRow("object.*", "BCP389", """The type "object" does not declare an additional properties type.""")]
    [DataRow("{ *: string }.*", "BCP391", "Type member access is only supported on a reference to a named type.")]
    public void Invalid_additional_properties_access_raises_diagnostic(string accessExpression, string expectedErrorCode, string expectedErrorMessage)
    {
        var result = CompilationHelper.Compile($$"""
            type test = {
              foo: string
            }

            type test2 = {{accessExpression}}
            """);

        result.Should().HaveDiagnostics(new[]
        {
            (expectedErrorCode, DiagnosticLevel.Error, expectedErrorMessage),
        });
    }

    [TestMethod]
    public void Type_additional_properties_access_can_be_used_on_resource_derived_types()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(TestContext)),
            ("main.bicep", """
                type tag = resourceInput<'Microsoft.Resources/tags@2022-09-01'>.properties.tags.*
                """));

        result.Should().NotHaveAnyCompilationBlockingDiagnostics();
        result.Template.Should().HaveValueAtPath("definitions.tag", JToken.Parse($$"""
            {
                "type": "string",
                "metadata": {
                    "{{LanguageConstants.MetadataResourceDerivedTypePropertyName}}": {
                        "{{LanguageConstants.MetadataResourceDerivedTypePointerPropertyName}}": "Microsoft.Resources/tags@2022-09-01#properties/properties/properties/tags/additionalProperties"
                    }
                }
            }
            """));
    }

    [TestMethod]
    public void Type_additional_properties_access_resolves_refs_and_traverses_imports()
    {
        var result = CompilationHelper.Compile(new ServiceBuilder().WithFeatureOverrides(new(TestContext)),
            ("types.bicep", """
                type tagsDict = {
                  *: resourceInput<'Microsoft.Resources/tags@2022-09-01'>.properties.tags
                }

                @export()
                type myObject = {
                  namedTagBags: tagsDict
                  *: int
                }
                """),
            ("main.bicep", """
                import * as types from 'types.bicep'

                type test = {
                  *: types.myObject
                }

                type test2 = {
                  *: {
                    *: test
                  }
                }

                type test3 = test2.*.*.*.*
                type test4 = test2.*.*.*.namedTagBags.*.*
                """));

        result.Should().NotHaveAnyCompilationBlockingDiagnostics();
        result.Template.Should().HaveValueAtPath("definitions.test3", JToken.Parse("""
            {
              "$ref": "#/definitions/_1.myObject/additionalProperties"
            }
            """));
        result.Template.Should().HaveValueAtPath("definitions.test4", JToken.Parse($$"""
            {
              "type": "string",
              "metadata": {
                "{{LanguageConstants.MetadataResourceDerivedTypePropertyName}}": {
                    "{{LanguageConstants.MetadataResourceDerivedTypePointerPropertyName}}": "Microsoft.Resources/tags@2022-09-01#properties/properties/properties/tags/additionalProperties"
                }
              }
            }
            """));
    }

    [TestMethod]
    public void Type_element_access_is_valid_type()
    {
        var result = CompilationHelper.Compile("""
            type test = string[]

            type test2 = test[*]
            """);

        result.Should().NotHaveAnyCompilationBlockingDiagnostics();
        result.Template.Should().HaveValueAtPath("definitions.test2", JToken.Parse("""
            {
              "$ref": "#/definitions/test/items"
            }
            """));
    }

    [DataTestMethod]
    [DataRow("test[*]", "BCP390", "The array item type access operator ('[*]') can only be used with typed arrays.")]
    [DataRow("array[*]", "BCP390", "The array item type access operator ('[*]') can only be used with typed arrays.")]
    [DataRow("test[0][*]", "BCP390", "The array item type access operator ('[*]') can only be used with typed arrays.")]
    [DataRow("string[][*]", "BCP391", "Type member access is only supported on a reference to a named type.")]
    public void Invalid_type_items_access_raises_diagnostic(string accessExpression, string expectedErrorCode, string expectedErrorMessage)
    {
        var result = CompilationHelper.Compile($$"""
            type test = [
              { bar: string }
            ]

            type test2 = {{accessExpression}}
            """);

        result.Should().HaveDiagnostics(new[]
        {
            (expectedErrorCode, DiagnosticLevel.Error, expectedErrorMessage),
        });
    }

    [TestMethod]
    public void Type_element_access_can_be_used_on_resource_derived_types()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(TestContext)),
            ("main.bicep", """
                type storageAccountName = resourceInput<'Microsoft.KeyVault/vaults@2022-07-01'>.properties.accessPolicies[*]
                """));

        result.Should().NotHaveAnyCompilationBlockingDiagnostics();
        result.Template.Should().HaveValueAtPath("definitions.storageAccountName", JToken.Parse($$"""
            {
                "type": "object",
                "metadata": {
                    "{{LanguageConstants.MetadataResourceDerivedTypePropertyName}}": {
                        "{{LanguageConstants.MetadataResourceDerivedTypePointerPropertyName}}": "Microsoft.KeyVault/vaults@2022-07-01#properties/properties/properties/accessPolicies/items"
                    }
                }
            }
            """));
    }

    [TestMethod]
    public void Type_element_access_resolves_refs_and_traverses_imports()
    {
        var result = CompilationHelper.Compile(new ServiceBuilder().WithFeatureOverrides(new(TestContext)),
            ("types.bicep", """
                @export()
                type accessPolicy = resourceInput<'Microsoft.KeyVault/vaults@2022-07-01'>.properties.accessPolicies[*]

                @export()
                type strings = string[]
                """),
            ("main.bicep", """
                import * as types from 'types.bicep'

                type accessPolicy = resourceInput<'Microsoft.KeyVault/vaults@2022-07-01'>.properties.accessPolicies[*]

                type test = types.strings[]

                type test2 = test[]

                type test3 = test2[*][*][*]
                type test4 = accessPolicy.permissions.keys[*]
                type test5 = types.accessPolicy.permissions.keys[*]
                """));

        result.Should().NotHaveAnyCompilationBlockingDiagnostics();
        result.Template.Should().HaveValueAtPath("definitions.test3", JToken.Parse("""
            {
              "$ref": "#/definitions/_1.strings/items"
            }
            """));
        result.Template.Should().HaveValueAtPath("definitions.test4", JToken.Parse($$"""
            {
              "type": "string",
              "metadata": {
                "{{LanguageConstants.MetadataResourceDerivedTypePropertyName}}": {
                    "{{LanguageConstants.MetadataResourceDerivedTypePointerPropertyName}}": "Microsoft.KeyVault/vaults@2022-07-01#properties/properties/properties/accessPolicies/items/properties/permissions/properties/keys/items"
                }
              }
            }
            """));
        result.Template.Should().HaveValueAtPath("definitions.test5", JToken.Parse($$"""
            {
              "type": "string",
              "metadata": {
                "{{LanguageConstants.MetadataResourceDerivedTypePropertyName}}": {
                    "{{LanguageConstants.MetadataResourceDerivedTypePointerPropertyName}}": "Microsoft.KeyVault/vaults@2022-07-01#properties/properties/properties/accessPolicies/items/properties/permissions/properties/keys/items"
                }
              }
            }
            """));
    }

    [TestMethod]
    public void Using_a_complete_resource_body_as_a_type_should_not_throw_exception()
    {
        var result = CompilationHelper.Compile(new ServiceBuilder().WithFeatureOverrides(new(TestContext)),
            ("main.bicep", """
                param subnets resourceInput<'Microsoft.Network/virtualNetworks/subnets@2023-09-01'>[]
                """));

        result.Template.Should().BeNull();
        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP394", DiagnosticLevel.Error, "Resource-derived type expressions must dereference a property within the resource body. Using the entire resource body type is not permitted."),
        });
    }

    [TestMethod]
    public void Resource_derived_type_nullability_should_be_preserved_when_loading_from_ARM_JSON()
    {
        var result = CompilationHelper.Compile(new ServiceBuilder().WithFeatureOverrides(new(TestContext)),
            ("main.bicep", """
                module mod 'mod.json' = {
                    name: 'mod'
                }
                """),
            ("mod.json", $$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "languageVersion": "2.0",
                    "contentVersion": "1.0.0.0",
                    "parameters": {
                        "foo": {
                            "type": "string",
                            "metadata": {
                                "{{LanguageConstants.MetadataResourceDerivedTypePropertyName}}": {
                                    "{{LanguageConstants.MetadataResourceDerivedTypePointerPropertyName}}": "Microsoft.Storage/storageAccounts@2022-09-01#properties/sku/properties/name"
                                }
                            },
                            "nullable": true
                        }
                    },
                    "resources": []
                }
                """));

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Issue_14869()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(TestContext)),
            ("main.bicep", """
                param container resourceInput<'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-05-15'>.properties.resource.indexingPolicy

                resource sa 'Microsoft.Storage/storageAccounts@2023-05-01'  = {
                  location: resourceGroup().location
                  sku: { name: 'Standard_GRS' }
                  kind: 'StorageV2'
                  name: 'mysa'
                  properties: {
                    accessTier: 'Hot'
                    azureFilesIdentityBasedAuthentication: container
                  }
                }
                """));

        result.Should().HaveDiagnostics(new[]
        {
            ("BCP035", DiagnosticLevel.Warning, """The specified "object" declaration is missing the following required properties: "directoryServiceOptions". If this is a resource type definition inaccuracy, report it using https://aka.ms/bicep-type-issues."""),
            ("BCP037", DiagnosticLevel.Warning, """The property "automatic" is not allowed on objects of type "AzureFilesIdentityBasedAuthentication". Permissible properties include "activeDirectoryProperties", "defaultSharePermission", "directoryServiceOptions". If this is a resource type definition inaccuracy, report it using https://aka.ms/bicep-type-issues."""),
            ("BCP037", DiagnosticLevel.Warning, """The property "compositeIndexes" is not allowed on objects of type "AzureFilesIdentityBasedAuthentication". Permissible properties include "activeDirectoryProperties", "defaultSharePermission", "directoryServiceOptions". If this is a resource type definition inaccuracy, report it using https://aka.ms/bicep-type-issues."""),
            ("BCP037", DiagnosticLevel.Warning, """The property "excludedPaths" is not allowed on objects of type "AzureFilesIdentityBasedAuthentication". Permissible properties include "activeDirectoryProperties", "defaultSharePermission", "directoryServiceOptions". If this is a resource type definition inaccuracy, report it using https://aka.ms/bicep-type-issues."""),
            ("BCP037", DiagnosticLevel.Warning, """The property "includedPaths" is not allowed on objects of type "AzureFilesIdentityBasedAuthentication". Permissible properties include "activeDirectoryProperties", "defaultSharePermission", "directoryServiceOptions". If this is a resource type definition inaccuracy, report it using https://aka.ms/bicep-type-issues."""),
            ("BCP037", DiagnosticLevel.Warning, """The property "indexingMode" is not allowed on objects of type "AzureFilesIdentityBasedAuthentication". Permissible properties include "activeDirectoryProperties", "defaultSharePermission", "directoryServiceOptions". If this is a resource type definition inaccuracy, report it using https://aka.ms/bicep-type-issues."""),
            ("BCP037", DiagnosticLevel.Warning, """The property "spatialIndexes" is not allowed on objects of type "AzureFilesIdentityBasedAuthentication". Permissible properties include "activeDirectoryProperties", "defaultSharePermission", "directoryServiceOptions". If this is a resource type definition inaccuracy, report it using https://aka.ms/bicep-type-issues."""),
        });
    }

    [TestMethod]
    public void Parameterized_types_should_require_parameterization()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(TestContext)),
            """type t = resourceInput""");

        result.Should().HaveDiagnostics([
            ("BCP384", DiagnosticLevel.Error, """The "resourceInput<ResourceTypeIdentifier>" type requires 1 argument(s)."""),
        ]);
    }

    [TestMethod]
    public void Resource_input_type_should_raise_no_diagnostic_when_resource_writeOnly_property_accessed()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(TestContext)),
            """
                param orderProperties resourceInput<'Microsoft.Capacity/reservationOrders@2022-11-01'>.properties

                output orderScopeType string = orderProperties.appliedScopeType
                """);

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Assignment_to_readOnly_property_diagnostic_should_be_raised_when_resource_output_is_assigned_to_resource_input()
    {
        var result = CompilationHelper.Compile("""
            param siteProperties object

            resource appService1 'Microsoft.Web/sites@2022-09-01' = {
              name: 'name'
              location: resourceGroup().location
              properties: siteProperties
            }

            resource appService2 'Microsoft.Web/sites@2022-09-01' = {
              name: 'name2'
              location: resourceGroup().location
              properties: appService1.properties
            }
            """);

        result.Diagnostics.Should().NotBeNullOrEmpty();
        result.Diagnostics.Should().ContainDiagnostic(
            "BCP073",
            DiagnosticLevel.Warning,
            """The property "availabilityState" is read-only. Expressions cannot be assigned to read-only properties. If this is a resource type definition inaccuracy, report it using https://aka.ms/bicep-type-issues."""
        );
    }

    [TestMethod]
    public void Assignment_to_readOnly_property_diagnostic_should_not_be_raised_when_resourceInput_typed_param_is_assigned_to_resource_input()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(TestContext)),
            """
                param siteProperties resourceInput<'Microsoft.Web/sites@2022-09-01'>.properties

                resource appService 'Microsoft.Web/sites@2022-09-01' = {
                  name: 'name'
                  location: resourceGroup().location
                  properties: siteProperties
                }
                """);

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Assignment_to_readOnly_property_diagnostic_should_not_be_raised_when_resource_output_is_assigned_to_resourceOutput_typed_target()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(TestContext)),
            """
                param siteProperties resourceInput<'Microsoft.Web/sites@2022-09-01'>.properties

                resource appService 'Microsoft.Web/sites@2022-09-01' = {
                  name: 'name'
                  location: resourceGroup().location
                  properties: siteProperties
                }

                @secure()
                output siteProperties resourceOutput<'Microsoft.Web/sites@2022-09-01'>.properties = appService.properties
                """);

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Assignment_to_readOnly_property_diagnostic_should_be_raised_when_resource_output_is_assigned_to_resourceInput_typed_target()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(TestContext)),
            """
                param siteProperties resourceInput<'Microsoft.Web/sites@2022-09-01'>.properties

                resource appService 'Microsoft.Web/sites@2022-09-01' = {
                  name: 'name'
                  location: resourceGroup().location
                  properties: siteProperties
                }

                output siteProperties resourceInput<'Microsoft.Web/sites@2022-09-01'>.properties = appService.properties
                """);

        result.Diagnostics.Should().NotBeNullOrEmpty();
        result.Diagnostics.Should().ContainDiagnostic(
            "BCP073",
            DiagnosticLevel.Warning,
            """The property "availabilityState" is read-only. Expressions cannot be assigned to read-only properties."""
        );
    }

    [DataTestMethod]
    [DataRow("type resourceInput = resourceInput<'Microsoft.Compute/virtualMachines'>")] // should be caught at syntax level
    [DataRow("type resourceInput = resourceInput<'Microsoft.Compute/virtualMachines'>.properties")] // should be caught by type manager
    public void Parameterized_type_recursion_raises_diagnostic(string template)
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(TestContext)),
            template);

        result.Should().HaveDiagnostics(new[]
        {
            ("BCP298", DiagnosticLevel.Error, "This type definition includes itself as required component, which creates a constraint that cannot be fulfilled."),
        });
    }

    // https://www.github.com/Azure/bicep/issues/15277
    [DataTestMethod]
    [DataRow("type resourceDerived = resourceInput<'Microsoft.Compute/virtualMachines/extensions@2019-12-01'>.properties.settings")]
    [DataRow("param resourceDerived resourceInput<'Microsoft.Compute/virtualMachines/extensions@2019-12-01'>.properties.settings")]
    [DataRow("output resourceDerived resourceInput<'Microsoft.Compute/virtualMachines/extensions@2019-12-01'>.properties.settings = 'foo'")]
    [DataRow("type t = { property: resourceInput<'Microsoft.Compute/virtualMachines/extensions@2019-12-01'>.properties.settings }")]
    [DataRow("type t = { *: resourceInput<'Microsoft.Compute/virtualMachines/extensions@2019-12-01'>.properties.settings }")]
    [DataRow("type t = [ resourceInput<'Microsoft.Compute/virtualMachines/extensions@2019-12-01'>.properties.settings ]")]
    [DataRow("type t = resourceInput<'Microsoft.Compute/virtualMachines/extensions@2019-12-01'>.properties.settings[]")]
    [DataRow("func f() resourceInput<'Microsoft.Compute/virtualMachines/extensions@2019-12-01'>.properties.settings => 'foo'")]
    [DataRow("func f(p resourceInput<'Microsoft.Compute/virtualMachines/extensions@2019-12-01'>.properties.settings) string => 'foo'")]
    public void Type_expressions_that_will_become_ARM_schema_nodes_are_checked_for_ARM_type_system_compatibility_prior_to_compilation(string template)
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(TestContext)),
            template);

        result.Template.Should().BeNull();
        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP411", DiagnosticLevel.Error, """The type "any" cannot be used in a type assignment because it does not fit within one of ARM's primitive type categories (string, int, bool, array, object). If this is a resource type definition inaccuracy, report it using https://aka.ms/bicep-type-issues."""),
        });
    }

    [TestMethod]
    public void Diagnostic_should_be_emitted_for_safe_access_of_non_existent_property()
    {
        var result = CompilationHelper.Compile("""
            param unsealed {
              requiredProperty: string
            }

            output x string = unsealed.?undeclaredProperty
            """);

        result.Should().NotHaveAnyCompilationBlockingDiagnostics();
        result.Should().HaveDiagnostics(new[]
        {
            ("BCP187", DiagnosticLevel.Info, "The property \"undeclaredProperty\" does not exist in the resource or type definition, although it might still be valid. If this is a resource type definition inaccuracy, report it using https://aka.ms/bicep-type-issues."),
        });
    }

    [TestMethod]
    public void Accessing_property_of_resource_derived_type_when_feature_is_disabled_raises_useful_error()
    {
        var result = CompilationHelper.Compile("""
            param probes resourceInput<'Microsoft.App/containerApps@2024-10-02-preview'>.properties.templates.containers[*].probes
            """);

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP083", DiagnosticLevel.Error, "The type \"ContainerAppProperties\" does not contain property \"templates\". Did you mean \"template\"?"),
        });
    }

    [TestMethod]
    public void FromEnd_indexing_of_tuple_resolves_correct_type()
    {
        var result = CompilationHelper.Compile("""
            param foo [int, string]
            output foo int = foo[^2]
            """);

        result.Should().NotHaveAnyDiagnostics();

        result = CompilationHelper.Compile("""
            param foo [int, string]
            output foo int = foo[^1]
            """);

        result.Should().HaveDiagnostics(new[]
        {
            ("BCP033", DiagnosticLevel.Error, "Expected a value of type \"int\" but the provided value is of type \"string\"."),
        });
    }

    [TestMethod]
    public void Safe_FromEnd_indexing_of_tuple_resolves_correct_type()
    {
        var result = CompilationHelper.Compile("""
            param foo [int, string]?
            output foo int? = foo[?^2]
            """);

        result.Should().NotHaveAnyDiagnostics();

        result = CompilationHelper.Compile("""
            param foo [int, string]?
            output foo int? = foo[?^1]
            """);

        result.Should().HaveDiagnostics(new[]
        {
            ("BCP033", DiagnosticLevel.Error, "Expected a value of type \"int | null\" but the provided value is of type \"null | string\"."),
        });
    }

    [TestMethod]
    public void Narrowing_a_recursive_type_against_itself_does_not_recur_infinitely()
    {
        var result = CompilationHelper.Compile("""
            type recursiveType = {
              recursion: recursiveType?
            }

            param p recursiveType

            output o recursiveType = p
            """);

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void User_defined_validators_are_blocked_if_feature_flag_is_not_enabled()
    {
        var result = CompilationHelper.Compile("""
            @validate(x => startsWith(x, 'foo'))
            param foo string
            """);

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(
        [
            ("BCP057", DiagnosticLevel.Error, "The name \"validate\" does not exist in the current context."),
        ]);
    }

    [TestMethod]
    public void User_defined_validator_can_be_attached_to_a_parameter_statement()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(TestContext, UserDefinedConstraintsEnabled: true)),
            """
            @validate(x => startsWith(x, 'foo'), 'Should have started with \'foo\'')
            param foo string
            """);

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().NotBeNull();
        result.Template.Should().HaveJsonAtPath("$.parameters.foo.validate",
            """["[lambda('x', startsWith(lambdaVariables('x'), 'foo'))]", "Should have started with 'foo'"]""");
    }

    [TestMethod]
    public void User_defined_validator_checks_lambda_type_against_declared_type()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(TestContext, UserDefinedConstraintsEnabled: true)),
            """
            @validate(x => startsWith(x, 'foo'))
            param foo int
            """);

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(
        [
            ("BCP070", DiagnosticLevel.Error, "Argument of type \"int => error\" is not assignable to parameter of type \"any => bool\"."),
        ]);
    }

    [TestMethod]
    public void User_defined_validator_disallows_runtime_expressions()
    {
        var result = CompilationHelper.Compile(
            new ServiceBuilder().WithFeatureOverrides(new(TestContext, UserDefinedConstraintsEnabled: true)),
            """
            resource sa 'Microsoft.Storage/storageAccounts@2025-01-01' existing = {
              name: 'acct'
            }

            var indirection = sa.properties.allowBlobPublicAccess

            @validate(x => x == !indirection)
            param foo bool
            """);

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(
        [
            ("BCP428", DiagnosticLevel.Error, "This expression is being used in parameter \"predicate\" of the function \"validate\", which requires a value that can be calculated at the start of the deployment. You are referencing a variable which cannot be calculated at the start (\"indirection\" -> \"sa\"). Properties of sa which can be calculated at the start include \"apiVersion\", \"id\", \"name\", \"type\"."),
        ]);
    }
}
