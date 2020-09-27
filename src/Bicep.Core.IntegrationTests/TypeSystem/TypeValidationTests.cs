// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Resources;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class TypeValidationTests
    {
        private static Core.SemanticModel.SemanticModel GetSemanticModelForTest(string programText, IEnumerable<ResourceType> definedTypes)
        {
            var typeRegistrarMock = new Mock<IResourceTypeProvider>();

            var registeredTypes = definedTypes.ToDictionary(x => x.TypeReference, ResourceTypeReferenceComparer.Instance);
            typeRegistrarMock
                .Setup(x => x.GetType(It.IsAny<ResourceTypeReference>()))
                .Returns<ResourceTypeReference>(x => registeredTypes.TryGetValue(x, out var resourceType) ? resourceType : new ResourceType(x, LanguageConstants.Object));
            typeRegistrarMock
                .Setup(x => x.HasType(It.IsAny<ResourceTypeReference>()))
                .Returns<ResourceTypeReference>(x => registeredTypes.ContainsKey(x));

            var compilation = new Compilation(new ResourceTypeRegistrar(typeRegistrarMock.Object), SyntaxFactory.CreateFromText(programText));
            return compilation.GetSemanticModel();
        }

        public static ResourceType CreateCustomResourceType(string fullyQualifiedType, string apiVersion, params TypeProperty[] customProperties)
        {
            var reference = ResourceTypeReference.Parse($"{fullyQualifiedType}@{apiVersion}");

            var resourceProperties = LanguageConstants.GetCommonResourceProperties(reference)
                .Concat(new TypeProperty("properties", new NamedObjectType("properties", customProperties, null), TypePropertyFlags.Required));

            return new ResourceType(reference, new NamedObjectType(reference.FormatName(), resourceProperties, null));
        }

        [TestMethod]
        public void Type_validation_runs_on_compilation_successfully()
        {
            var customTypes = new [] {
                CreateCustomResourceType("My.Rp/myType", "2020-01-01"),
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

        [TestMethod]
        public void Type_validation_runs_on_compilation_common_failures()
        {
            var customTypes = new [] {
                CreateCustomResourceType("My.Rp/myType", "2020-01-01",
                    new TypeProperty("readOnlyProp", LanguageConstants.String, TypePropertyFlags.ReadOnly),
                    new TypeProperty("writeOnlyProp", LanguageConstants.String, TypePropertyFlags.WriteOnly),
                    new TypeProperty("requiredProp", LanguageConstants.String, TypePropertyFlags.Required),
                    new TypeProperty("additionalProps", new NamedObjectType(
                        "additionalProps",
                        new [] {
                            new TypeProperty("propA", LanguageConstants.String, TypePropertyFlags.Required),
                            new TypeProperty("propB", LanguageConstants.String),
                        },
                        LanguageConstants.Int
                    )),
                    new TypeProperty("nestedObj", new NamedObjectType(
                        "nestedObj",
                        new [] {
                            new TypeProperty("readOnlyNestedProp", LanguageConstants.String, TypePropertyFlags.ReadOnly),
                            new TypeProperty("writeOnlyNestedProp", LanguageConstants.String, TypePropertyFlags.WriteOnly),
                            new TypeProperty("requiredNestedProp", LanguageConstants.String, TypePropertyFlags.Required),
                        },
                        null
                    ))),
            };
            var program = @"
resource myRes 'My.Rp/myType@2020-01-01' = {
  name: 'steve'
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
                x => x.Message.Should().Be("The specified object is missing the following required properties: requiredProp."),
                x => x.Message.Should().Be("The property 'readOnlyProp' is read-only. Expressions cannot be assigned to read-only properties."),
                x => x.Message.Should().Be("The property 'readOnlyProp' expected a value of type string but the provided value is of type int."),
                x => x.Message.Should().Be("The property 'writeOnlyProp' expected a value of type string but the provided value is of type int."),
                x => x.Message.Should().Be("The specified object is missing the following required properties: propA."),
                x => x.Message.Should().Be("The property 'propB' expected a value of type string but the provided value is of type int."),
                x => x.Message.Should().Be("The specified object is missing the following required properties: requiredNestedProp."),
                x => x.Message.Should().Be("The property 'readOnlyNestedProp' is read-only. Expressions cannot be assigned to read-only properties."),
                x => x.Message.Should().Be("The property 'readOnlyNestedProp' expected a value of type string but the provided value is of type int."),
                x => x.Message.Should().Be("The property 'writeOnlyNestedProp' expected a value of type string but the provided value is of type int."),
                x => x.Message.Should().Be("The property 'writeOnlyProp' on type 'properties' is write-only. Write-only properties cannot be accessed."),
                x => x.Message.Should().Be("The type nestedObj does not contain property 'writeOnlyProp'. Available properties include 'readOnlyNestedProp', 'requiredNestedProp'."),
                x => x.Message.Should().Be("The type properties does not contain property 'missingOutput'. Available properties include 'additionalProps', 'nestedObj', 'readOnlyProp', 'requiredProp'."),
                x => x.Message.Should().Be("The type nestedObj does not contain property 'missingOutput'. Available properties include 'readOnlyNestedProp', 'requiredNestedProp'."),
                x => x.Message.Should().Be("The output expects a value of type int but the provided value is of type string."),
                x => x.Message.Should().Be("The type nestedObj does not contain property 'readOnlyProp'. Available properties include 'readOnlyNestedProp', 'requiredNestedProp'.")
            );
        }

        [TestMethod]
        public void Type_validation_narrowing_on_union_types()
        {
            var customTypes = new [] {
                CreateCustomResourceType("My.Rp/myType", "2020-01-01",
                    new TypeProperty("stringOrInt", UnionType.Create(LanguageConstants.String, LanguageConstants.Int)),
                    new TypeProperty("unspecifiedStringOrInt", UnionType.Create(LanguageConstants.String, LanguageConstants.Int)),
                    new TypeProperty("abcOrDef", UnionType.Create(new StringLiteralType("abc"), new StringLiteralType("def"))),
                    new TypeProperty("unspecifiedAbcOrDef", UnionType.Create(new StringLiteralType("abc"), new StringLiteralType("def")))),
                CreateCustomResourceType("My.Rp/myDependentType", "2020-01-01",
                    new TypeProperty("stringOnly", LanguageConstants.String),
                    new TypeProperty("abcOnly", new StringLiteralType("abc")),
                    new TypeProperty("abcOnlyUnNarrowed", new StringLiteralType("abc")),
                    new TypeProperty("stringOrIntUnNarrowed", UnionType.Create(LanguageConstants.String, LanguageConstants.Int)),
                    new TypeProperty("abcOrDefUnNarrowed", UnionType.Create(new StringLiteralType("abc"), new StringLiteralType("def"), new StringLiteralType("ghi")))),
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
                x => x.Message.Should().Be("The property 'abcOnlyUnNarrowed' expected a value of type 'abc' but the provided value is of type 'abc' | 'def'.")
            );
        }

        [TestMethod]
        public void Type_validation_narrowing_on_discriminated_object_types()
        {
            var customTypes = new [] {
                CreateCustomResourceType("My.Rp/myType", "2020-01-01",
                    new TypeProperty("myDisc1", new DiscriminatedObjectType("myDisc1", "discKey", new [] {
                            new NamedObjectType("choiceA", new [] {
                                new TypeProperty("discKey", new StringLiteralType("choiceA"), TypePropertyFlags.Required),
                                new TypeProperty("valueA", LanguageConstants.String, TypePropertyFlags.Required),
                            }, null),
                            new NamedObjectType("choiceB", new [] {
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
                    x => x.Message.Should().Be("The property 'discKey' requires a value of type 'choiceA' | 'choiceB', but none was supplied.")
                );
            }

            {
                // incorrect discriminator key
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
                    x => x.Message.Should().Be("The property 'discKey' expected a value of type 'choiceA' | 'choiceB' but the provided value is of type 'choiceC'.")
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
                    x => x.Message.Should().Be("The specified object is missing the following required properties: valueA.")
                );
            }

            {
                // all good
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
                    x => x.Message.Should().Be("The type choiceA does not contain property 'valueB'. Available properties include 'discKey', 'valueA'.")
                );
            }
        }
    }
}