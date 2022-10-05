// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class TypeValidationTests
    {
        private static ServiceBuilder Services => new ServiceBuilder();

        private static SemanticModel GetSemanticModelForTest(string programText, IEnumerable<ResourceTypeComponents> definedTypes)
        {
            var compilation = Services
                .WithAzResources(definedTypes)
                .WithConfigurationManager(IConfigurationManager.WithStaticConfiguration(BicepTestConstants.BuiltInConfigurationWithAllAnalyzersDisabled))
                .BuildCompilation(programText);

            return compilation.GetEntrypointSemanticModel();
        }

        [DataTestMethod]
        [DataRow(TypeSymbolValidationFlags.Default, DiagnosticLevel.Error)]
        [DataRow(TypeSymbolValidationFlags.WarnOnTypeMismatch, DiagnosticLevel.Warning)]
        public void Type_validation_runs_on_compilation_successfully(TypeSymbolValidationFlags validationFlags, DiagnosticLevel expectedDiagnosticLevel)
        {
            var customTypes = new[] {
                TestTypeHelper.CreateCustomResourceType("My.Rp/myType", "2020-01-01", validationFlags),
            };
            var program = @"
resource myRes 'My.Rp/myType@2020-01-01' = {
  name: 'steve'
  properties: { }
}
";

            var model = GetSemanticModelForTest(program, customTypes);
            model.GetAllDiagnostics().Should().BeEmpty();
        }

        [DataTestMethod]
        [DataRow(TypeSymbolValidationFlags.Default, DiagnosticLevel.Error)]
        [DataRow(TypeSymbolValidationFlags.WarnOnTypeMismatch, DiagnosticLevel.Warning)]
        public void Type_validation_runs_on_compilation_common_failures(TypeSymbolValidationFlags validationFlags, DiagnosticLevel expectedDiagnosticLevel)
        {
            var customTypes = new[] {
                TestTypeHelper.CreateCustomResourceTypeWithTopLevelProperties("My.Rp/myType", "2020-01-01", validationFlags,
                    new [] { new TypeProperty("readOnlyTopLevelProp", LanguageConstants.String, TypePropertyFlags.ReadOnly) },
                    new TypeProperty("readOnlyProp", LanguageConstants.String, TypePropertyFlags.ReadOnly),
                    new TypeProperty("writeOnlyProp", LanguageConstants.String, TypePropertyFlags.WriteOnly | TypePropertyFlags.AllowImplicitNull),
                    new TypeProperty("requiredProp", LanguageConstants.String, TypePropertyFlags.Required),
                    new TypeProperty("additionalProps", new ObjectType(
                        "additionalProps",
                        validationFlags,
                        new [] {
                            new TypeProperty("propA", LanguageConstants.String, TypePropertyFlags.Required),
                            new TypeProperty("propB", LanguageConstants.String, TypePropertyFlags.AllowImplicitNull),
                        },
                        LanguageConstants.Int
                    )),
                    new TypeProperty("nestedObj", new ObjectType(
                        "nestedObj",
                        validationFlags,
                        new [] {
                            new TypeProperty("readOnlyNestedProp", LanguageConstants.String, TypePropertyFlags.ReadOnly),
                            new TypeProperty("writeOnlyNestedProp", LanguageConstants.String, TypePropertyFlags.WriteOnly | TypePropertyFlags.AllowImplicitNull),
                            new TypeProperty("requiredNestedProp", LanguageConstants.String, TypePropertyFlags.Required),
                        },
                        null
                    ))),
            };
            var program = @"
resource myRes 'My.Rp/myType@2020-01-01' = {
  name: 'steve'
  id: '/subscriptions/guid/resourceGroups/rg/My.Rp/myType/steve'
  type: 'My.Rp/myType'
  apiVersion: '2020-01-01'
  readOnlyTopLevelProp: 'abcd'
  properties: {
    readOnlyProp: 123
    writeOnlyProp: 456
    additionalProps: {
      propB: 123
    }
    nestedObj: {
      readOnlyNestedProp: 123
      writeOnlyNestedProp: 456
    }
  }
}

output writeOnlyOutput string = myRes.properties.writeOnlyProp
output writeOnlyOutput2 string = myRes.properties.nestedObj.writeOnlyProp
output missingOutput string = myRes.properties.missingOutput
output missingOutput2 string = myRes.properties.nestedObj.missingOutput
output incorrectTypeOutput int = myRes.properties.readOnlyProp
output incorrectTypeOutput2 int = myRes.properties.nestedObj.readOnlyProp
";

            var model = GetSemanticModelForTest(program, customTypes);
            model.GetAllDiagnostics().Should().SatisfyRespectively(
                x => x.Should().HaveCodeAndSeverity("BCP073", expectedDiagnosticLevel).And.HaveMessage("The property \"id\" is read-only. Expressions cannot be assigned to read-only properties."),
                x => x.Should().HaveCodeAndSeverity("BCP073", expectedDiagnosticLevel).And.HaveMessage("The property \"type\" is read-only. Expressions cannot be assigned to read-only properties."),
                x => x.Should().HaveCodeAndSeverity("BCP073", expectedDiagnosticLevel).And.HaveMessage("The property \"apiVersion\" is read-only. Expressions cannot be assigned to read-only properties."),
                x => x.Should().HaveCodeAndSeverity("BCP073", DiagnosticLevel.Warning).And.HaveMessage("The property \"readOnlyTopLevelProp\" is read-only. Expressions cannot be assigned to read-only properties. If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
                x => x.Should().HaveCodeAndSeverity("BCP035", DiagnosticLevel.Warning).And.HaveMessage("The specified \"object\" declaration is missing the following required properties: \"requiredProp\". If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
                x => x.Should().HaveCodeAndSeverity("BCP073", DiagnosticLevel.Warning).And.HaveMessage("The property \"readOnlyProp\" is read-only. Expressions cannot be assigned to read-only properties. If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
                x => x.Should().HaveCodeAndSeverity("BCP036", DiagnosticLevel.Warning).And.HaveMessage("The property \"writeOnlyProp\" expected a value of type \"null | string\" but the provided value is of type \"int\". If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
                x => x.Should().HaveCodeAndSeverity("BCP035", DiagnosticLevel.Warning).And.HaveMessage("The specified \"object\" declaration is missing the following required properties: \"propA\". If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
                x => x.Should().HaveCodeAndSeverity("BCP036", DiagnosticLevel.Warning).And.HaveMessage("The property \"propB\" expected a value of type \"null | string\" but the provided value is of type \"int\". If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
                x => x.Should().HaveCodeAndSeverity("BCP035", DiagnosticLevel.Warning).And.HaveMessage("The specified \"object\" declaration is missing the following required properties: \"requiredNestedProp\". If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
                x => x.Should().HaveCodeAndSeverity("BCP073", DiagnosticLevel.Warning).And.HaveMessage("The property \"readOnlyNestedProp\" is read-only. Expressions cannot be assigned to read-only properties. If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
                x => x.Should().HaveCodeAndSeverity("BCP036", DiagnosticLevel.Warning).And.HaveMessage("The property \"writeOnlyNestedProp\" expected a value of type \"null | string\" but the provided value is of type \"int\". If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
                x => x.Should().HaveCodeAndSeverity("BCP077", expectedDiagnosticLevel).And.HaveMessage("The property \"writeOnlyProp\" on type \"properties\" is write-only. Write-only properties cannot be accessed."),
                x => x.Should().HaveCodeAndSeverity("BCP053", expectedDiagnosticLevel).And.HaveMessage("The type \"nestedObj\" does not contain property \"writeOnlyProp\". Available properties include \"readOnlyNestedProp\", \"requiredNestedProp\"."),
                x => x.Should().HaveCodeAndSeverity("BCP053", expectedDiagnosticLevel).And.HaveMessage("The type \"properties\" does not contain property \"missingOutput\". Available properties include \"additionalProps\", \"nestedObj\", \"readOnlyProp\", \"requiredProp\"."),
                x => x.Should().HaveCodeAndSeverity("BCP053", expectedDiagnosticLevel).And.HaveMessage("The type \"nestedObj\" does not contain property \"missingOutput\". Available properties include \"readOnlyNestedProp\", \"requiredNestedProp\"."),
                x => x.Should().HaveCodeAndSeverity("BCP026", DiagnosticLevel.Error).And.HaveMessage("The output expects a value of type \"int\" but the provided value is of type \"string\"."),
                x => x.Should().HaveCodeAndSeverity("BCP053", expectedDiagnosticLevel).And.HaveMessage("The type \"nestedObj\" does not contain property \"readOnlyProp\". Available properties include \"readOnlyNestedProp\", \"requiredNestedProp\".")
            );
        }

        [DataTestMethod]
        [DataRow(TypeSymbolValidationFlags.Default, DiagnosticLevel.Error)]
        [DataRow(TypeSymbolValidationFlags.WarnOnTypeMismatch, DiagnosticLevel.Warning)]
        public void Type_validation_narrowing_on_union_types(TypeSymbolValidationFlags validationFlags, DiagnosticLevel expectedDiagnosticLevel)
        {
            var customTypes = new[] {
                TestTypeHelper.CreateCustomResourceType("My.Rp/myType", "2020-01-01", validationFlags,
                    new TypeProperty("stringOrInt", TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Int), TypePropertyFlags.AllowImplicitNull),
                    new TypeProperty("unspecifiedStringOrInt", TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Int), TypePropertyFlags.AllowImplicitNull),
                    new TypeProperty("abcOrDef", TypeHelper.CreateTypeUnion(new StringLiteralType("abc"), new StringLiteralType("def")), TypePropertyFlags.AllowImplicitNull),
                    new TypeProperty("unspecifiedAbcOrDef", TypeHelper.CreateTypeUnion(new StringLiteralType("abc"), new StringLiteralType("def")), TypePropertyFlags.AllowImplicitNull)),
                TestTypeHelper.CreateCustomResourceType("My.Rp/myDependentType", "2020-01-01", validationFlags,
                    new TypeProperty("stringOnly", LanguageConstants.String, TypePropertyFlags.AllowImplicitNull),
                    new TypeProperty("abcOnly", new StringLiteralType("abc"), TypePropertyFlags.AllowImplicitNull),
                    new TypeProperty("abcOnlyUnNarrowed", new StringLiteralType("abc"), TypePropertyFlags.AllowImplicitNull),
                    new TypeProperty("stringOrIntUnNarrowed", TypeHelper.CreateTypeUnion(LanguageConstants.String, LanguageConstants.Int), TypePropertyFlags.AllowImplicitNull),
                    new TypeProperty("abcOrDefUnNarrowed", TypeHelper.CreateTypeUnion(new StringLiteralType("abc"), new StringLiteralType("def"), new StringLiteralType("ghi")), TypePropertyFlags.AllowImplicitNull)),
            };
            var program = @"
resource myRes 'My.Rp/myType@2020-01-01' = {
  name: 'steve'
  properties: {
    stringOrInt: 'abc'
    abcOrDef: 'abc'
  }
}

resource myDependentRes 'My.Rp/myDependentType@2020-01-01' = {
  name: 'steve'
  properties: {
    stringOnly: myRes.properties.stringOrInt // should be allowed
    abcOnly: myRes.properties.abcOrDef
    abcOnlyUnNarrowed: myRes.properties.unspecifiedAbcOrDef
    stringOrIntUnNarrowed: myRes.properties.unspecifiedStringOrInt
    abcOrDefUnNarrowed: myRes.properties.abcOrDef
  }
}
";

            var model = GetSemanticModelForTest(program, customTypes);
            model.GetAllDiagnostics().Should().SatisfyRespectively(
                x => x.Should().HaveCodeAndSeverity("BCP036", DiagnosticLevel.Warning).And.HaveMessage("The property \"abcOnlyUnNarrowed\" expected a value of type \"'abc' | null\" but the provided value is of type \"'abc' | 'def'\". If this is an inaccuracy in the documentation, please report it to the Bicep Team.")
            );
        }

        [DataTestMethod]
        [DataRow(TypeSymbolValidationFlags.Default, DiagnosticLevel.Error)]
        [DataRow(TypeSymbolValidationFlags.WarnOnTypeMismatch, DiagnosticLevel.Warning)]
        public void Type_validation_narrowing_on_discriminated_object_types(TypeSymbolValidationFlags validationFlags, DiagnosticLevel expectedDiagnosticLevel)
        {
            var customTypes = new[] {
                TestTypeHelper.CreateCustomResourceType("My.Rp/myType", "2020-01-01", validationFlags,
                    new TypeProperty("myDisc1", new DiscriminatedObjectType("myDisc1", validationFlags, "discKey", new [] {
                            new ObjectType("choiceA", validationFlags, new [] {
                                new TypeProperty("discKey", new StringLiteralType("choiceA"), TypePropertyFlags.Required),
                                new TypeProperty("valueA", LanguageConstants.String, TypePropertyFlags.Required),
                            }, null),
                            new ObjectType("choiceB", validationFlags, new [] {
                                new TypeProperty("discKey", new StringLiteralType("choiceB"), TypePropertyFlags.Required),
                                new TypeProperty("valueB", LanguageConstants.String, TypePropertyFlags.Required),
                            }, null),
                        }
                    ))),
            };

            {
                // missing discriminator key
                var program = @"
resource myRes 'My.Rp/myType@2020-01-01' = {
  name: 'steve'
  properties: {
    myDisc1: {
      valueA: 'abc'
    }
  }
}
";

                var model = GetSemanticModelForTest(program, customTypes);
                model.GetAllDiagnostics().Should().SatisfyRespectively(
                    x => x.Should().HaveCodeAndSeverity("BCP078", expectedDiagnosticLevel).And.HaveMessage("The property \"discKey\" requires a value of type \"'choiceA' | 'choiceB'\", but none was supplied.")
                );
            }

            {
                // incorrect discriminator key case
                var program = @"
resource myRes 'My.Rp/myType@2020-01-01' = {
  name: 'steve'
  properties: {
    myDisc1: {
      DiscKey: 'choiceA'
    }
  }
}
";

                var model = GetSemanticModelForTest(program, customTypes);
                model.GetAllDiagnostics().Should().SatisfyRespectively(
                    x => x.Should().HaveCodeAndSeverity("BCP078", expectedDiagnosticLevel).And.HaveMessage("The property \"discKey\" requires a value of type \"'choiceA' | 'choiceB'\", but none was supplied."),
                    x => x.Should().HaveCodeAndSeverity("BCP089", expectedDiagnosticLevel).And.HaveMessage("The property \"DiscKey\" is not allowed on objects of type \"'choiceA' | 'choiceB'\". Did you mean \"discKey\"?")
                );
            }

            {
                // incorrect discriminator key
                var program = @"
resource myRes 'My.Rp/myType@2020-01-01' = {
  name: 'steve'
  properties: {
    myDisc1: {
      discKey: 'foo'
    }
  }
}
";

                var model = GetSemanticModelForTest(program, customTypes);
                model.GetAllDiagnostics().Should().SatisfyRespectively(
                    x => x.Should().HaveCodeAndSeverity("BCP036", DiagnosticLevel.Warning).And.HaveMessage("The property \"discKey\" expected a value of type \"'choiceA' | 'choiceB'\" but the provided value is of type \"'foo'\". If this is an inaccuracy in the documentation, please report it to the Bicep Team.")
                );
            }

            {
                // incorrect discriminator key with suggestion
                var program = @"
resource myRes 'My.Rp/myType@2020-01-01' = {
  name: 'steve'
  properties: {
    myDisc1: {
      discKey: 'choiceC'
    }
  }
}
";

                var model = GetSemanticModelForTest(program, customTypes);
                model.GetAllDiagnostics().Should().SatisfyRespectively(
                    x => x.Should().HaveCodeAndSeverity("BCP088", DiagnosticLevel.Warning).And.HaveMessage("The property \"discKey\" expected a value of type \"'choiceA' | 'choiceB'\" but the provided value is of type \"'choiceC'\". Did you mean \"'choiceA'\"?")
                );
            }

            {
                // discriminator key supplied, required value omitted
                var program = @"
resource myRes 'My.Rp/myType@2020-01-01' = {
  name: 'steve'
  properties: {
    myDisc1: {
      discKey: 'choiceA'
    }
  }
}
";

                var model = GetSemanticModelForTest(program, customTypes);
                model.GetAllDiagnostics().Should().SatisfyRespectively(
                    x => x.Should().HaveCodeAndSeverity("BCP035", DiagnosticLevel.Warning).And.HaveMessage("The specified \"object\" declaration is missing the following required properties: \"valueA\". If this is an inaccuracy in the documentation, please report it to the Bicep Team.")
                );
            }

            {
                // discriminator key supplied, case of required property is incorrect
                var program = @"
resource myRes 'My.Rp/myType@2020-01-01' = {
  name: 'steve'
  properties: {
    myDisc1: {
      discKey: 'choiceA'
      ValueA: 'hello'
    }
  }
}
";

                var model = GetSemanticModelForTest(program, customTypes);
                model.GetAllDiagnostics().Should().SatisfyRespectively(
                    x => x.Should().HaveCodeAndSeverity("BCP035", DiagnosticLevel.Warning).And.HaveMessage("The specified \"object\" declaration is missing the following required properties: \"valueA\". If this is an inaccuracy in the documentation, please report it to the Bicep Team."),
                    x => x.Should().HaveCodeAndSeverity("BCP089", expectedDiagnosticLevel).And.HaveMessage("The property \"ValueA\" is not allowed on objects of type \"choiceA\". Did you mean \"valueA\"?")
                );
            }

            {
                // all good, incorrect property access
                var program = @"
resource myRes 'My.Rp/myType@2020-01-01' = {
  name: 'steve'
  properties: {
    myDisc1: {
      discKey: 'choiceA'
      valueA: 'hello'
    }
  }
}

output valueA string = myRes.properties.myDisc1.valueA
output valueB string = myRes.properties.myDisc1.valuuuueB
";

                var model = GetSemanticModelForTest(program, customTypes);
                model.GetAllDiagnostics().Should().SatisfyRespectively(
                    x => x.Should().HaveCodeAndSeverity("BCP053", expectedDiagnosticLevel).And.HaveMessage("The type \"choiceA\" does not contain property \"valuuuueB\". Available properties include \"discKey\", \"valueA\".")
                );
            }

            {
                // all good, incorrect property access with suggestion
                var program = @"
resource myRes 'My.Rp/myType@2020-01-01' = {
  name: 'steve'
  properties: {
    myDisc1: {
      discKey: 'choiceA'
      valueA: 'hello'
    }
  }
}

output valueA string = myRes.properties.myDisc1.valueA
output valueB string = myRes.properties.myDisc1.valueB
";

                var model = GetSemanticModelForTest(program, customTypes);
                model.GetAllDiagnostics().Should().SatisfyRespectively(
                    x => x.Should().HaveCodeAndSeverity("BCP083", expectedDiagnosticLevel).And.HaveMessage("The type \"choiceA\" does not contain property \"valueB\". Did you mean \"valueA\"?")
                );
            }
        }

        [TestMethod]
        public void Json_function_can_obtain_types_for_string_literal_json_args()
        {
            var program = @"
var intJson = json('123')
var floatJson = json('1234.1224')
var stringJson = json('""hello!""')
var nullJson = json('null')
var jsonWithComments = json('''
{
    //here's a comment!
    ""key"": ""value"" /* multi-line
    comment */
}
''')

var objectJson = json('{""validProp"": ""validValue""}')
var propAccess = objectJson.validProp
var commentsPropAccess = jsonWithComments.key
var invalidPropAccess = objectJson.invalidProp
";

            var model = GetSemanticModelForTest(program, Enumerable.Empty<ResourceTypeComponents>());

            GetTypeForNamedSymbol(model, "objectJson").Name.Should().Be("object");
            GetTypeForNamedSymbol(model, "propAccess").Name.Should().Be("'validValue'");

            GetTypeForNamedSymbol(model, "intJson").Name.Should().Be("int");
            GetTypeForNamedSymbol(model, "floatJson").Name.Should().Be("any");
            GetTypeForNamedSymbol(model, "stringJson").Name.Should().Be("'hello!'");
            GetTypeForNamedSymbol(model, "nullJson").Name.Should().Be("null");
            GetTypeForNamedSymbol(model, "commentsPropAccess").Name.Should().Be("'value'");

            GetTypeForNamedSymbol(model, "invalidPropAccess").Name.Should().Be("error");

            model.GetAllDiagnostics().Should().SatisfyRespectively(
                x => x.Should().HaveCodeAndSeverity("BCP083", DiagnosticLevel.Error).And.HaveMessage("The type \"object\" does not contain property \"invalidProp\". Did you mean \"validProp\"?")
            );
        }

        [TestMethod]
        public void Json_function_returns_error_for_unparseable_json()
        {
            var program = @"
var invalidJson = json('{""prop"": ""value')
";

            var model = GetSemanticModelForTest(program, Enumerable.Empty<ResourceTypeComponents>());

            GetTypeForNamedSymbol(model, "invalidJson").Name.Should().Be("error");

            model.GetAllDiagnostics().Should().SatisfyRespectively(
                x => x.Should().HaveCodeAndSeverity("BCP186", DiagnosticLevel.Error).And.HaveMessage("Unable to parse literal JSON value. Please ensure that it is well-formed.")
            );
        }

        [TestMethod]
        public void Items_function_builds_type_definition_from_source_type()
        {
            var program = @"
var basicObject = {
  abc: 'string'
  DEF: true
  '123': {
    abc: 'test'
  }
  'arr': [
    1
    2
    3
  ]
}
var itemsOutput = items(basicObject)
var singleItemKey = itemsOutput[0].key
var singleItemValue = itemsOutput[0].value
";

            var model = GetSemanticModelForTest(program, Enumerable.Empty<ResourceTypeComponents>());

            GetTypeForNamedSymbol(model, "itemsOutput").Name.Should().Be("object[]");
            GetTypeForNamedSymbol(model, "singleItemKey").Name.Should().Be("'123' | 'DEF' | 'abc' | 'arr'");
            GetTypeForNamedSymbol(model, "singleItemValue").Name.Should().Be("any");
        }

        [TestMethod]
        public void Items_function_permits_any_and_returns_generic_type()
        {
            var program = @"
var itemsOutput = items(any('hello!'))
var singleItemKey = itemsOutput[0].key
var singleItemValue = itemsOutput[0].value
";

            var model = GetSemanticModelForTest(program, Enumerable.Empty<ResourceTypeComponents>());

            GetTypeForNamedSymbol(model, "itemsOutput").Name.Should().Be("object[]");
            GetTypeForNamedSymbol(model, "singleItemKey").Name.Should().Be("string");
            GetTypeForNamedSymbol(model, "singleItemValue").Name.Should().Be("any");
        }

        [TestMethod]
        public void Items_function_works_for_resources()
        {
            var program = @"
resource testRes 'Test.Rp/readWriteTests@2020-01-01' existing = {
  name: 'testRes'
}

var itemsOutput = items(testRes.properties)
var singleItemKey = itemsOutput[0].key
var singleItemValue = itemsOutput[0].value
";

            var model = GetSemanticModelForTest(program, BuiltInTestTypes.Types);

            GetTypeForNamedSymbol(model, "itemsOutput").Name.Should().Be("object[]");
            GetTypeForNamedSymbol(model, "singleItemKey").Name.Should().Be("'readonly' | 'readwrite' | 'required'");
            GetTypeForNamedSymbol(model, "singleItemValue").Name.Should().Be("string");
        }

        [TestMethod]
        public void Existing_keyword_enforced_for_readonly_resources()
        {
            var program = @"
resource testRes 'Test.Rp/readOnlyTests@2020-01-01' = {
    name: 'default'
}
";

            var model = GetSemanticModelForTest(program, BuiltInTestTypes.Types);
            model.GetAllDiagnostics().Should().SatisfyRespectively(
                x => x.Should().HaveCodeAndSeverity("BCP245", DiagnosticLevel.Warning).And.HaveMessage(@"Resource type ""Test.Rp/readOnlyTests@2020-01-01"" can only be used with the 'existing' keyword.")
            );
        }

        [TestMethod]
        public void Existing_keyword_enforced_for_resources_that_are_readonly_at_target_scope()
        {
            var customTypes = new[] {
                TestTypeHelper.CreateCustomResourceType("My.Rp/myType", "2020-01-01", TypeSymbolValidationFlags.Default, ResourceScope.Tenant | ResourceScope.Subscription, ResourceScope.Tenant, ResourceFlags.None),
            };

            var program = @"
targetScope = 'tenant'

resource testRes 'My.Rp/myType@2020-01-01' = {
    name: 'default'
}
";

            var model = GetSemanticModelForTest(program, customTypes);
            var diagnostics = model.GetAllDiagnostics().ToList();
            model.GetAllDiagnostics().Should().SatisfyRespectively(
                x => x.Should().HaveCodeAndSeverity("BCP246", DiagnosticLevel.Warning).And.HaveMessage(@"Resource type ""My.Rp/myType@2020-01-01"" can only be used with the 'existing' keyword at the requested scope. Permitted scopes for deployment: ""subscription"".")
            );
        }

        private static TypeSymbol GetTypeForNamedSymbol(SemanticModel model, string symbolName)
        {
            var symbol = model.Root.GetDeclarationsByName(symbolName).Single();

            return symbol.Type;
        }
    }
}
