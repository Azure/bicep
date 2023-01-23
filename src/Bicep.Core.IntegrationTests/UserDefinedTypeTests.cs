// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class UserDefinedTypeTests
{
    private ServiceBuilder ServicesWithUserDefinedTypes => new ServiceBuilder()
        .WithFeatureOverrides(new(TestContext, UserDefinedTypesEnabled: true));

    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public void Type_declarations_are_disabled_unless_feature_is_enabled()
    {
        var result = CompilationHelper.Compile(@"
type myString = string
");
        result.Should().HaveDiagnostics(new[] {
            ("BCP280", DiagnosticLevel.Error, "Using a type declaration statement requires enabling EXPERIMENTAL feature \"UserDefinedTypes\"."),
        });
    }

    [TestMethod]
    public void Inline_object_types_are_disabled_unless_feature_is_enabled()
    {
        var result = CompilationHelper.Compile(@"
param complexParam {
    property1: string
    property2: string
}
");
        result.Should().ContainDiagnostic("BCP282", DiagnosticLevel.Error, "Using a strongly-typed object type declaration requires enabling EXPERIMENTAL feature \"UserDefinedTypes\".");
    }

    [TestMethod]
    public void Inline_type_literals_are_disabled_unless_feature_is_enabled()
    {
        var result = CompilationHelper.Compile(@"
param thirtyThreeParam 33
");
        result.Should().ContainDiagnostic("BCP283", DiagnosticLevel.Error, "Using a literal value as a type requires enabling EXPERIMENTAL feature \"UserDefinedTypes\".");
    }

    [TestMethod]
    public void Inline_union_types_are_disabled_unless_feature_is_enabled()
    {
        var result = CompilationHelper.Compile(@"
param oneOfSeveralStrings 'this one'|'that one'|'perhaps this one instead'
");
        result.Should().ContainDiagnostic("BCP284", DiagnosticLevel.Error, "Using a type union declaration requires enabling EXPERIMENTAL feature \"UserDefinedTypes\".");
    }

    [TestMethod]
    public void Nullable_types_are_disabled_unless_feature_is_enabled()
    {
        var result = CompilationHelper.Compile(@"
param nullableString string?
");
        result.Should().ContainDiagnostic("BCP320", DiagnosticLevel.Error, "Using nullable types requires enabling EXPERIMENTAL feature \"UserDefinedTypes\".");
    }

    [TestMethod]
    public void Namespaces_cannot_be_used_as_types()
    {
        var result = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
param azParam az
");
        result.Should().ContainDiagnostic("BCP306", DiagnosticLevel.Error, "The name \"az\" refers to a namespace, not to a type.");
    }

    [TestMethod]
    public void Namespaces_cannot_be_assigned_to_types()
    {
        var result = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
type sysAlias = sys
");
        result.Should().ContainDiagnostic("BCP306", DiagnosticLevel.Error, "The name \"sys\" refers to a namespace, not to a type.");
    }

    [TestMethod]
    public void Masked_types_still_accessible_via_qualified_reference()
    {
        var result = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
type string = int

param stringParam string = 'foo'
");

        result.Should().HaveDiagnostics(new[] {
            ("no-unused-params", DiagnosticLevel.Warning, "Parameter \"stringParam\" is declared but never used."),
            ("BCP033", DiagnosticLevel.Error, "Expected a value of type \"int\" but the provided value is of type \"'foo'\"."),
        });

        // fix by fully-qualifying
        result = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
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
        var result = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
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
        var result = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
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
        var blockedBecauseOfCycle = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
type anObject = {
    recur: {foo: 'bar'}|anObject
}
");

        blockedBecauseOfCycle.Should().HaveDiagnostics(new[] {
            ("BCP298", DiagnosticLevel.Error, "This type definition includes itself as required component, which creates a constraint that cannot be fulfilled."),
            ("BCP062", DiagnosticLevel.Error, "The referenced declaration with name \"anObject\" is not valid."),
        });

        var blockedBecauseOfUnionSemantics = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
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
        var blockedBecauseOfCycle = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
type anObject = {
    recur: !anObject
}
");

        blockedBecauseOfCycle.Should().HaveDiagnostics(new[] {
            ("BCP298", DiagnosticLevel.Error, "This type definition includes itself as required component, which creates a constraint that cannot be fulfilled."),
            ("BCP062", DiagnosticLevel.Error, "The referenced declaration with name \"anObject\" is not valid."),
        });

        var blockedBecauseOfUnaryOperationSemantics = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
type anObject = {
    recur: !anObject?
}
");

        blockedBecauseOfUnaryOperationSemantics.Should().HaveDiagnostics(new[] {
            ("BCP285", DiagnosticLevel.Error, "The type expression could not be reduced to a literal value."),
        });
    }

    [TestMethod]
    public void Arrays_that_incorporate_their_parent_object_do_not_blow_the_stack()
    {
        var blockedBecauseOfCycle = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
type anObject = {
    recur: anObject[]
}
");

        blockedBecauseOfCycle.Should().HaveDiagnostics(new[] {
            ("BCP298", DiagnosticLevel.Error, "This type definition includes itself as required component, which creates a constraint that cannot be fulfilled."),
            ("BCP062", DiagnosticLevel.Error, "The referenced declaration with name \"anObject\" is not valid."),
        });

        var permitted = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
type anObject = {
    recur: (anObject?)[]
}
");

        permitted.Should().NotHaveAnyDiagnostics();

        permitted = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
type anArray = (anArray?)[]
");

        permitted.Should().NotHaveAnyDiagnostics();

        permitted = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
type anArray = anArray[]?
");

        permitted.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Cyclic_nullables_do_not_blow_the_stack()
    {
        var blockedBecauseOfCycle = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
type nullable = nullable?
");

        blockedBecauseOfCycle.Should().HaveDiagnostics(new[] {
            ("BCP298", DiagnosticLevel.Error, "This type definition includes itself as required component, which creates a constraint that cannot be fulfilled."),
        });
    }

    [TestMethod]
    public void Tuples_that_incorporate_their_parent_object_do_not_blow_the_stack()
    {
        var blockedBecauseOfCycle = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
type anObject = {
    recur: [anObject]
}
");

        blockedBecauseOfCycle.Should().HaveDiagnostics(new[] {
            ("BCP298", DiagnosticLevel.Error, "This type definition includes itself as required component, which creates a constraint that cannot be fulfilled."),
            ("BCP062", DiagnosticLevel.Error, "The referenced declaration with name \"anObject\" is not valid."),
        });

        var permitted = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
type anObject = {
    recur: [anObject]?
}
");

        permitted.Should().NotHaveAnyDiagnostics();

        permitted = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
type anObject = {
    recur: [anObject?]
}
");

        permitted.Should().NotHaveAnyDiagnostics();

        permitted = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
type aTuple = [aTuple?]
");

        permitted.Should().NotHaveAnyDiagnostics();

        permitted = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
type aTuple = [aTuple]?
");

        permitted.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Objects_that_incorporate_their_parent_object_do_not_blow_the_stack()
    {
        var blockedBecauseOfCycle = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
type anObject = {
    recurEventually: {
        recurNow: anObject
    }
}
");

        blockedBecauseOfCycle.Should().HaveDiagnostics(new[] {
            ("BCP298", DiagnosticLevel.Error, "This type definition includes itself as required component, which creates a constraint that cannot be fulfilled."),
            ("BCP062", DiagnosticLevel.Error, "The referenced declaration with name \"anObject\" is not valid."),
        });

        var permitted = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
type anObject = {
    recurEventually: {
        recurNow: anObject?
    }
}
");

        permitted.Should().NotHaveAnyDiagnostics();

        permitted = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
type anObject = {
    recurEventually: {
        recurNow: anObject
    }?
}
");

        permitted.Should().NotHaveAnyDiagnostics();

        permitted = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
type anObject = {
    recurEventually: {
        recurNow: anObject
    }
}?
");

        permitted.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Warning_should_be_shown_when_reading_unknown_properties_on_unsealed_objects()
    {
        var result = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
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
        var result = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
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
        var result = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
#disable-next-line no-unused-params
param aDict {
  *: int
} = {prop: 'someVal'}
");

        result.Should().HaveDiagnostics(new[] {
            ("BCP036", DiagnosticLevel.Error, @"The property ""prop"" expected a value of type ""int"" but the provided value is of type ""'someVal'""."),
        });

        result = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
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
        var result = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
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
        var result = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
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

        var result = CompilationHelper.Compile(ServicesWithUserDefinedTypes, templateWithPossiblyNullDeref);
        result.Should().HaveDiagnostics(new []
        {
          ("BCP318", DiagnosticLevel.Warning, @"The value of type ""null | { bar: { baz: { quux: 'quux' } } }"" may be null at the start of the deployment, which would cause this access expression (and the overall deployment with it) to fail."),
        });
        result.Diagnostics.Single().Should().BeAssignableTo<IFixable>();
        result.Diagnostics.Single().As<IFixable>().Fixes.Single().Should().HaveResult(templateWithPossiblyNullDeref, templateWithNonNullAssertion);

        result = CompilationHelper.Compile(ServicesWithUserDefinedTypes, templateWithNonNullAssertion);
        result.Should().NotHaveAnyDiagnostics();
        result.Should().HaveTemplateWithOutput("quux", "[parameters('foos')[0].bar.baz.quux]");
    }

    [TestMethod]
    public void Error_should_be_emitted_when_setting_a_default_value_on_a_nullable_parameter()
    {
        var result = CompilationHelper.Compile(ServicesWithUserDefinedTypes, @"
#disable-next-line no-unused-params
param myParam string? = 'foo'
");

        result.Should().HaveDiagnostics(new[] {
            ("BCP322", DiagnosticLevel.Error, "Nullable-typed parameters may not be assigned default values. They have an implicit default of 'null' that cannot be overridden."),
        });
    }
}
