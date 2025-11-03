// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Azure.Deployments.ClientTools;
using Azure.Deployments.Core.Definitions.Identifiers;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class EvaluationTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public void Basic_arithmetic_expressions_are_evaluated_successfully()
    {
        var (template, _, _) = CompilationHelper.Compile(@"
var sum = 1 + 3
var mult = sum * 5
var modulo = mult % 7
var div = modulo / 11
var sub = div - 13

output sum int = sum
output mult int = mult
output modulo int = modulo
output div int = div
output sub int = sub
");

        using (new AssertionScope())
        {
            var evaluated = TemplateEvaluator.Evaluate(template).ToJToken();

            evaluated.Should().HaveValueAtPath("$.outputs['sum'].value", 4);
            evaluated.Should().HaveValueAtPath("$.outputs['mult'].value", 20);
            evaluated.Should().HaveValueAtPath("$.outputs['modulo'].value", 6);
            evaluated.Should().HaveValueAtPath("$.outputs['div'].value", 0);
            evaluated.Should().HaveValueAtPath("$.outputs['sub'].value", -13);
        }
    }

    [TestMethod]
    public void String_expressions_are_evaluated_successfully()
    {
        var (template, _, _) = CompilationHelper.Compile(@"
var bool = false
var int = 12948
var str = 'hello!'
var obj = {
  a: 'b'
  '!c': 2
}
var arr = [
  true
  2893
  'abc'
]
var multiline = '''
these escapes
  are not
  evaluated:
\r\n\t\\\'\${}
'''

output literal string = str
output interp string = '>${bool}<>${int}<>${str}<>${obj}<>${arr}<'
output escapes string = '\r\n\t\\\'\${}'
output multiline string = multiline
");

        using (new AssertionScope())
        {
            var evaluated = TemplateEvaluator.Evaluate(template).ToJToken();

            evaluated.Should().HaveValueAtPath("$.outputs['literal'].value", "hello!");
            evaluated.Should().HaveValueAtPath("$.outputs['interp'].value", ">False<>12948<>hello!<>{'a':'b','!c':2}<>[true,2893,'abc']<");
            evaluated.Should().HaveValueAtPath("$.outputs['escapes'].value", "\r\n\t\\'${}");
            evaluated.Should().HaveValueAtPath("$.outputs['multiline'].value", "these escapes\n  are not\n  evaluated:\n\\r\\n\\t\\\\\\'\\${}\n");
        }
    }

    [TestMethod]
    public void Multiline_interpolation_is_evaluated_successfully()
    {
        var result = CompilationHelper.Compile(ServiceBuilder.CreateWithFeatures(new(MultilineStringInterpolationEnabled: true)), """
var intVal = 12345
output multiline string = $'''
>${intVal}<
>$${intVal}<
'''
output multiline2 string = $$'''
>${intVal}<
>$${intVal}<
'''
""");

        using (new AssertionScope())
        {
            var evaluated = TemplateEvaluator.Evaluate(result.Template).ToJToken();

            evaluated.Should().HaveValueAtPath("$.outputs['multiline'].value", ">12345<\n>$12345<\n");
            evaluated.Should().HaveValueAtPath("$.outputs['multiline2'].value", ">${intVal}<\n>12345<\n");
        }
    }

    [TestMethod]
    public void Multiline_interpolation_is_blocked_without_feature_flag()
    {
        var result = CompilationHelper.Compile("""
var intVal = 12345
output multiline string = $'''
>${intVal}<
>$${intVal}<
'''
output multiline2 string = $$'''
>${intVal}<
>$${intVal}<
'''
""");

        result.Should().HaveDiagnostics(new[]
        {
            ("BCP442", DiagnosticLevel.Error, """Using multiline string interpolation requires enabling EXPERIMENTAL feature "MultilineStringInterpolation"."""),
            ("BCP442", DiagnosticLevel.Error, """Using multiline string interpolation requires enabling EXPERIMENTAL feature "MultilineStringInterpolation"."""),
        });
    }

    [TestMethod]
    public void ResourceId_expressions_are_evaluated_successfully()
    {
        var bicepparamText = @"
using 'main.bicep'

param parentName = 'myParent'
param childName = 'myChild'
";

        var bicepTemplateText = @"
param parentName string
param childName string

resource existing1 'My.Rp/parent/child@2020-01-01' existing = {
  name: '${parentName}/${childName}'
}

resource existing2 'My.Rp/parent/child@2020-01-01' existing = {
  name: '${parentName}/${childName}'
  scope: resourceGroup('customRg')
}

resource existing3 'My.Rp/parent/child@2020-01-01' existing = {
  name: '${parentName}/${childName}'
  scope: resourceGroup('2e518a80-f860-4467-a00e-d81aaf1ff42e', 'customRg')
}

resource existing4 'My.Rp/parent/child@2020-01-01' existing = {
  name: '${parentName}/${childName}'
  scope: tenant()
}

resource existing5 'My.Rp/parent/child@2020-01-01' existing = {
  name: '${parentName}/${childName}'
  scope: subscription()
}

resource existing6 'My.Rp/parent/child@2020-01-01' existing = {
  name: '${parentName}/${childName}'
  scope: subscription('2e518a80-f860-4467-a00e-d81aaf1ff42e')
}

resource existing7 'My.Rp/parent/child@2020-01-01' existing = {
  name: '${parentName}/${childName}'
  scope: managementGroup('2e518a80-f860-4467-a00e-d81aaf1ff42e')
}

resource existing8 'My.ExtensionRp/parent/child@2020-01-01' existing = {
  name: '${parentName}/${childName}'
  scope: existing4
}

output resource1Id string = existing1.id
output resource2Id string = existing2.id
output resource3Id string = existing3.id
output resource4Id string = existing4.id
output resource5Id string = existing5.id
output resource6Id string = existing6.id
output resource7Id string = existing7.id
output resource8Id string = existing8.id

output resource1Name string = existing1.name
output resource1ApiVersion string = existing1.apiVersion
output resource1Type string = existing1.type
";

        var (parameters, _, _) = CompilationHelper.CompileParams(("parameters.bicepparam", bicepparamText), ("main.bicep", bicepTemplateText));

        var (template, _, _) = CompilationHelper.Compile(bicepTemplateText);

        using (new AssertionScope())
        {
            var testSubscriptionId = "87d64d6d-6d17-4ad7-b507-16d9bc498781";
            var testRgName = "testRg";
            var evaluated = TemplateEvaluator.Evaluate(template, parameters, config => config with
            {
                SubscriptionId = testSubscriptionId,
                ResourceGroup = testRgName,
            }).ToJToken();

            evaluated.Should().HaveValueAtPath("$.outputs['resource1Id'].value", $"/subscriptions/{testSubscriptionId}/resourceGroups/{testRgName}/providers/My.Rp/parent/myParent/child/myChild");
            evaluated.Should().HaveValueAtPath("$.outputs['resource2Id'].value", $"/subscriptions/{testSubscriptionId}/resourceGroups/customRg/providers/My.Rp/parent/myParent/child/myChild");
            evaluated.Should().HaveValueAtPath("$.outputs['resource3Id'].value", $"/subscriptions/2e518a80-f860-4467-a00e-d81aaf1ff42e/resourceGroups/customRg/providers/My.Rp/parent/myParent/child/myChild");
            evaluated.Should().HaveValueAtPath("$.outputs['resource4Id'].value", $"/providers/My.Rp/parent/myParent/child/myChild");
            evaluated.Should().HaveValueAtPath("$.outputs['resource5Id'].value", $"/subscriptions/{testSubscriptionId}/providers/My.Rp/parent/myParent/child/myChild");
            evaluated.Should().HaveValueAtPath("$.outputs['resource6Id'].value", $"/subscriptions/2e518a80-f860-4467-a00e-d81aaf1ff42e/providers/My.Rp/parent/myParent/child/myChild");
            evaluated.Should().HaveValueAtPath("$.outputs['resource7Id'].value", $"/providers/Microsoft.Management/managementGroups/2e518a80-f860-4467-a00e-d81aaf1ff42e/providers/My.Rp/parent/myParent/child/myChild");
            evaluated.Should().HaveValueAtPath("$.outputs['resource8Id'].value", $"/providers/My.Rp/parent/myParent/child/myChild/providers/My.ExtensionRp/parent/myParent/child/myChild");

            evaluated.Should().HaveValueAtPath("$.outputs['resource1Name'].value", "myParent/myChild");
            evaluated.Should().HaveValueAtPath("$.outputs['resource1ApiVersion'].value", "2020-01-01");
            evaluated.Should().HaveValueAtPath("$.outputs['resource1Type'].value", "My.Rp/parent/child");
        }
    }

    [TestMethod]
    public void Comparisons_are_evaluated_correctly()
    {
        var (template, _, _) = CompilationHelper.Compile(@"
output less bool = 123 < 456
output lessOrEquals bool = 123 <= 456
output greater bool = 123 > 456
output greaterOrEquals bool = 123 >= 456
output equals bool = 123 == 456
output not bool = !true
output and bool = true && false
output or bool = true || false
output coalesce int = null ?? 123
");

        using (new AssertionScope())
        {
            var evaluated = TemplateEvaluator.Evaluate(template).ToJToken();

            evaluated.Should().HaveValueAtPath("$.outputs['less'].value", true);
            evaluated.Should().HaveValueAtPath("$.outputs['lessOrEquals'].value", true);
            evaluated.Should().HaveValueAtPath("$.outputs['greater'].value", false);
            evaluated.Should().HaveValueAtPath("$.outputs['greaterOrEquals'].value", false);
            evaluated.Should().HaveValueAtPath("$.outputs['equals'].value", false);
            evaluated.Should().HaveValueAtPath("$.outputs['not'].value", false);
            evaluated.Should().HaveValueAtPath("$.outputs['and'].value", false);
            evaluated.Should().HaveValueAtPath("$.outputs['or'].value", true);
            evaluated.Should().HaveValueAtPath("$.outputs['coalesce'].value", 123);
        }
    }

    [TestMethod]
    public void Resource_property_access_works()
    {
        var bicepparamText = @"
using 'main.bicep'

param abcVal = 'test!!!'
";

        var bicepTemplateText = @"
param abcVal string

resource testRes 'My.Rp/res1@2020-01-01' = {
  name: 'testRes'
  properties: {
    abc: abcVal
  }
}

output abcVal string = testRes.properties.abc
";

        var (parameters, _, _) = CompilationHelper.CompileParams(("parameters.bicepparam", bicepparamText), ("main.bicep", bicepTemplateText));

        var (template, _, _) = CompilationHelper.Compile(bicepTemplateText);

        using (new AssertionScope())
        {
            var evaluated = TemplateEvaluator.Evaluate(template, parameters).ToJToken();

            evaluated.Should().HaveValueAtPath("$.outputs['abcVal'].value", "test!!!");
        }
    }

    [TestMethod]
    public void Existing_resource_property_access_works()
    {
        var (template, _, _) = CompilationHelper.Compile(@"
resource testRes 'My.Rp/res1@2020-01-01' existing = {
  name: 'testRes'
}

output abcVal string = testRes.properties.abc
");

        using (new AssertionScope())
        {
            var evaluated = TemplateEvaluator.Evaluate(template, configBuilder: config => config with
            {
                OnReferenceFunc = (resourceId, apiVersion, fullBody) =>
                {
                    if (resourceId == $"/subscriptions/{config.SubscriptionId}/resourceGroups/{config.ResourceGroup}/providers/My.Rp/res1/testRes" && apiVersion == "2020-01-01" && !fullBody)
                    {
                        return new JObject
                        {
                            ["abc"] = "test!!!",
                        };
                    }

                    throw new NotImplementedException();
                },
            }).ToJToken();

            evaluated.Should().HaveValueAtPath("$.outputs['abcVal'].value", "test!!!");
        }
    }

    [TestMethod]
    public void Items_function_evaluation_works()
    {
        var bicepparamText = @"
using 'main.bicep'

param inputObj = {
  'ghiKey': 'ghiValue'
  'defKey': 'defValue'
  'abcKey': 'abcValue'
  '123Key': '123Value'
  'GHIKey': 'GHIValue'
  'DEFKey': 'DEFValue'
  'ABCKey': 'ABCValue'
  '456Key': '456Value'
}
";

        var bicepTemplateText = @"
param inputObj object

output inputObjKeys array = [for item in items(inputObj): item.key]
output inputObjValues array = [for item in items(inputObj): item.value]
";

        var (parameters, _, _) = CompilationHelper.CompileParams(("parameters.bicepparam", bicepparamText), ("main.bicep", bicepTemplateText));

        var result = CompilationHelper.Compile(bicepTemplateText);

        var evaluated = TemplateEvaluator.Evaluate(result.Template, parameters).ToJToken();

        evaluated.Should().HaveValueAtPath("$.outputs['inputObjKeys'].value", new JArray
            {
                "123Key",
                "456Key",
                "abcKey",
                "ABCKey",
                "defKey",
                "DEFKey",
                "ghiKey",
                "GHIKey",
            });

        evaluated.Should().HaveValueAtPath("$.outputs['inputObjValues'].value", new JArray
            {
                "123Value",
                "456Value",
                "abcValue",
                "ABCValue",
                "defValue",
                "DEFValue",
                "ghiValue",
                "GHIValue",
            });
    }

    [TestMethod]
    public void Join_function_evaluation_works()
    {
        var result = CompilationHelper.Compile(@"
var foo = [
  'abc'
  'def'
  'ghi'
]

output joined1 string = join(foo, '')
output joined2 string = join(foo, ',')
output joined3 string = join([
  'I'
  'love'
  'Bicep'
], ' ')
");

        result.Should().NotHaveAnyDiagnostics();

        var evaluated = TemplateEvaluator.Evaluate(result.Template).ToJToken();
        evaluated.Should().HaveValueAtPath("$.outputs['joined1'].value", "abcdefghi");
        evaluated.Should().HaveValueAtPath("$.outputs['joined2'].value", "abc,def,ghi");
        evaluated.Should().HaveValueAtPath("$.outputs['joined3'].value", "I love Bicep");
    }

    [TestMethod]
    public void indexof_contains_function_evaluation_works()
    {
        var bicepparamText = @"
using 'main.bicep'

param inputString = 'FOOBAR'
param inputArray = [
  'FOO'
  'BAR'
]
";

        var bicepTemplateText = @"
param inputString string
param inputArray array

output strIdxFooLC int = indexOf(inputString, 'foo')
output strIdxFooUC int = indexOf(inputString, 'FOO')
output strIdxBarLC int = indexOf(inputString, 'bar')
output strIdxBarUC int = indexOf(inputString, 'BAR')
output containsStrFooLC bool = contains(inputString, 'foo')
output containsStrFooUC bool = contains(inputString, 'FOO')
output containsStrBarLC bool = contains(inputString, 'bar')
output containsStrBarUC bool = contains(inputString, 'BAR')

output arrIdxFooLC int = indexOf(inputArray, 'foo')
output arrIdxFooUC int = indexOf(inputArray, 'FOO')
output arrIdxBarLC int = indexOf(inputArray, 'bar')
output arrIdxBarUC int = indexOf(inputArray, 'BAR')
output arrIdxfalse int = indexOf(inputArray, false)
output arrIdx123 int = indexOf(inputArray, 123)
output containsArrFooLC bool = contains(inputArray, 'foo')
output containsArrFooUC bool = contains(inputArray, 'FOO')
output containsArrBarLC bool = contains(inputArray, 'bar')
output containsArrBarUC bool = contains(inputArray, 'BAR')
output containsArrfalse bool = contains(inputArray, false)
output containsArr123 bool = contains(inputArray, 123)
";

        var (parameters, _, _) = CompilationHelper.CompileParams(("parameters.bicepparam", bicepparamText), ("main.bicep", bicepTemplateText));

        var (template, _, _) = CompilationHelper.Compile(bicepTemplateText);


        using (new AssertionScope())
        {
            var evaluated = TemplateEvaluator.Evaluate(template, parameters).ToJToken();

            evaluated.Should().HaveValueAtPath("$.outputs['strIdxFooLC'].value", new JValue(0));
            evaluated.Should().HaveValueAtPath("$.outputs['strIdxFooUC'].value", new JValue(0));
            evaluated.Should().HaveValueAtPath("$.outputs['strIdxBarLC'].value", new JValue(3));
            evaluated.Should().HaveValueAtPath("$.outputs['strIdxBarUC'].value", new JValue(3));

            evaluated.Should().HaveValueAtPath("$.outputs['containsStrFooLC'].value", new JValue(false)); // case-sensitive
            evaluated.Should().HaveValueAtPath("$.outputs['containsStrFooUC'].value", new JValue(true));
            evaluated.Should().HaveValueAtPath("$.outputs['containsStrBarLC'].value", new JValue(false)); // case-sensitive
            evaluated.Should().HaveValueAtPath("$.outputs['containsStrBarUC'].value", new JValue(true));

            evaluated.Should().HaveValueAtPath("$.outputs['arrIdxFooLC'].value", new JValue(-1));
            evaluated.Should().HaveValueAtPath("$.outputs['arrIdxFooUC'].value", new JValue(0));
            evaluated.Should().HaveValueAtPath("$.outputs['arrIdxBarLC'].value", new JValue(-1));
            evaluated.Should().HaveValueAtPath("$.outputs['arrIdxBarUC'].value", new JValue(1));
            evaluated.Should().HaveValueAtPath("$.outputs['arrIdxfalse'].value", new JValue(-1));
            evaluated.Should().HaveValueAtPath("$.outputs['arrIdx123'].value", new JValue(-1));

            evaluated.Should().HaveValueAtPath("$.outputs['containsArrFooLC'].value", new JValue(false));
            evaluated.Should().HaveValueAtPath("$.outputs['containsArrFooUC'].value", new JValue(true));
            evaluated.Should().HaveValueAtPath("$.outputs['containsArrBarLC'].value", new JValue(false));
            evaluated.Should().HaveValueAtPath("$.outputs['containsArrBarUC'].value", new JValue(true));
            evaluated.Should().HaveValueAtPath("$.outputs['containsArrfalse'].value", new JValue(false));
            evaluated.Should().HaveValueAtPath("$.outputs['containsArr123'].value", new JValue(false));
        }
    }

    [TestMethod]
    public void List_comprehension_function_evaluation_works()
    {
        var bicepparamText = @"
using 'main.bicep'

param doggos = [
  'Evie'
  'Casper'
  'Indy'
  'Kira'
]
param numbers = [0, 1, 2, 3]
";

        var bicepTemplateText = @"
param doggos array
param numbers array

var sayHello = map(doggos, i => 'Hello ${i}!')

var isEven = filter(numbers, i => 0 == i % 2)

var evenDoggosNestedLambdas = map(filter(numbers, i => contains(filter(numbers, j => 0 == j % 2), i)), x => doggos[x])

var flattenedArrayOfArrays = flatten([[0, 1], [2, 3], [4, 5]])
var flattenedEmptyArray = flatten([])

var mapSayHi = map(['abc', 'def', 'ghi'], foo => 'Hi ${foo}!')
var mapEmpty = map([], foo => 'Hi ${foo}!')
var mapObject = map(range(0, length(doggos)), i => {
  i: i
  doggo: doggos[i]
  greeting: 'Ahoy, ${doggos[i]}!'
})
var mapArray = flatten(map(range(1, 3), i => [i * 2, (i * 2) + 1]))
var mapMultiLineArray = flatten(map(range(1, 3), i => [
  i * 3
  (i * 3) + 1
  (i * 3) + 2
]))

var filterEqualityCheck = filter(['abc', 'def', 'ghi'], foo => 'def' == foo)
var filterEmpty = filter([], foo => 'def' == foo)

var sortNumeric = sort([8, 3, 10, -13, 5], (x, y) => x < y)
var sortAlpha = sort(['ghi', 'abc', 'def'], (x, y) => x < y)
var sortAlphaReverse = sort(['ghi', 'abc', 'def'], (x, y) => x > y)
var sortByObjectKey = sort([
  { key: 124, name: 'Second' }
  { key: 298, name: 'Third' }
  { key: 24, name: 'First' }
  { key: 1232, name: 'Fourth' }
], (x, y) => int(x.key) < int(y.key))
var sortEmpty = sort([], (x, y) => int(x) < int(y))

var reduceStringConcat = reduce(['abc', 'def', 'ghi'], '', (cur, next) => concat(cur, next))
var reduceFactorial = reduce(range(1, 5), 1, (cur, next) => cur * next)
var reduceObjectUnion = reduce([
  { foo: 123 }
  { bar: 456 }
  { baz: 789 }
], {}, (cur, next) => union(cur, next))
var reduceEmpty = reduce([], 0, (cur, next) => cur)

var objectMap = toObject([123, 456, 789], i => '${i / 100}')
var objectMap2 = toObject(numbers, i => '${i}', i => {
  isEven: (i % 2) == 0
  isGreaterThan2: (i > 2)
})
var objectMap3 = toObject(sortByObjectKey, x => x.name)
";

        var (parameters, _, _) = CompilationHelper.CompileParams(("parameters.bicepparam", bicepparamText), ("main.bicep", bicepTemplateText));

        var (template, _, _) = CompilationHelper.Compile(bicepTemplateText);


        using (new AssertionScope())
        {
            var evaluated = TemplateEvaluator.Evaluate(template, parameters).ToJToken();

            evaluated.Should().HaveValueAtPath("$.variables['sayHello']", new JArray
                {
                    "Hello Evie!",
                    "Hello Casper!",
                    "Hello Indy!",
                    "Hello Kira!",
                });
            evaluated.Should().HaveValueAtPath("$.variables['isEven']", new JArray
                {
                    0,
                    2,
                });
            evaluated.Should().HaveValueAtPath("$.variables['evenDoggosNestedLambdas']", new JArray
                {
                    "Evie",
                    "Indy",
                });
            evaluated.Should().HaveValueAtPath("$.variables['flattenedArrayOfArrays']", new JArray {
                    0,
                    1,
                    2,
                    3,
                    4,
                    5
                });
            evaluated.Should().HaveValueAtPath("$.variables['flattenedEmptyArray']", new JArray { });
            evaluated.Should().HaveValueAtPath("$.variables['mapSayHi']", new JArray {
                    "Hi abc!",
                    "Hi def!",
                    "Hi ghi!"
                });
            evaluated.Should().HaveValueAtPath("$.variables['mapEmpty']", new JArray { });
            evaluated.Should().HaveValueAtPath("$.variables['mapObject']", new JArray {
                    new JObject {
                        ["i"] = 0,
                        ["doggo"] = "Evie",
                        ["greeting"] = "Ahoy, Evie!"
                    },
                    new JObject {
                        ["i"] = 1,
                        ["doggo"] = "Casper",
                        ["greeting"] = "Ahoy, Casper!"
                    },
                    new JObject {
                        ["i"] = 2,
                        ["doggo"] = "Indy",
                        ["greeting"] = "Ahoy, Indy!"
                    },
                    new JObject {
                        ["i"] = 3,
                        ["doggo"] = "Kira",
                        ["greeting"] = "Ahoy, Kira!"
                    }
                });
            evaluated.Should().HaveValueAtPath("$.variables['mapArray']", new JArray {
                    2,
                    3,
                    4,
                    5,
                    6,
                    7
                });
            evaluated.Should().HaveValueAtPath("$.variables['mapMultiLineArray']", new JArray {
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11
                });
            evaluated.Should().HaveValueAtPath("$.variables['filterEqualityCheck']", new JArray {
                    "def"
                });
            evaluated.Should().HaveValueAtPath("$.variables['filterEmpty']", new JArray { });
            evaluated.Should().HaveValueAtPath("$.variables['sortNumeric']", new JArray {
                    -13,
                    3,
                    5,
                    8,
                    10
                });
            evaluated.Should().HaveValueAtPath("$.variables['sortAlpha']", new JArray {
                    "abc",
                    "def",
                    "ghi"
                });
            evaluated.Should().HaveValueAtPath("$.variables['sortAlphaReverse']", new JArray {
                    "ghi",
                    "def",
                    "abc"
                });
            evaluated.Should().HaveValueAtPath("$.variables['sortByObjectKey']", new JArray {
                    new JObject {
                        ["key"] = 24,
                        ["name"] = "First"
                    },
                    new JObject {
                        ["key"] = 124,
                        ["name"] = "Second"
                    },
                    new JObject {
                        ["key"] = 298,
                        ["name"] = "Third"
                    },
                    new JObject {
                        ["key"] = 1232,
                        ["name"] = "Fourth"
                    }
                });
            evaluated.Should().HaveValueAtPath("$.variables['sortEmpty']", new JArray { });
            evaluated.Should().HaveValueAtPath("$.variables['reduceStringConcat']", "abcdefghi");
            evaluated.Should().HaveValueAtPath("$.variables['reduceFactorial']", 120);
            evaluated.Should().HaveValueAtPath("$.variables['reduceObjectUnion']", new JObject
            {
                ["foo"] = 123,
                ["bar"] = 456,
                ["baz"] = 789
            });
            evaluated.Should().HaveValueAtPath("$.variables['reduceEmpty']", 0);
            evaluated.Should().HaveValueAtPath("$.variables['objectMap']", JToken.Parse(@"{
  ""1"": 123,
  ""4"": 456,
  ""7"": 789
}"));
            evaluated.Should().HaveValueAtPath("$.variables['objectMap2']", JToken.Parse(@"{
  ""0"": {
    ""isEven"": true,
    ""isGreaterThan2"": false
  },
  ""1"": {
    ""isEven"": false,
    ""isGreaterThan2"": false
  },
  ""2"": {
    ""isEven"": true,
    ""isGreaterThan2"": false
  },
  ""3"": {
    ""isEven"": false,
    ""isGreaterThan2"": true
  }
}"));
            evaluated.Should().HaveValueAtPath("$.variables['objectMap3']", JToken.Parse(@"{
  ""First"": {
    ""key"": 24,
    ""name"": ""First""
  },
  ""Second"": {
    ""key"": 124,
    ""name"": ""Second""
  },
  ""Third"": {
    ""key"": 298,
    ""name"": ""Third""
  },
  ""Fourth"": {
    ""key"": 1232,
    ""name"": ""Fourth""
  }
}"));
        }
    }

    /// <summary>
    /// https://github.com/Azure/bicep/issues/8782
    /// </summary>
    [TestMethod]
    public void Issue8782()
    {
        var result = CompilationHelper.Compile(@"
var testArray = [
  {
    property1: 'test'
    property2: 1
  }
  {
    property1: 'dev'
    property2: 2
  }
  {
    property1: 'test'
    property2: 0
  }
  {
    property1: 'prod'
    property2: 1
  }
  {
    property1: 'prod'
    property2: 0
  }
  {
    property1: 'dev'
    property2: 0
  }
  {
    property1: 'test'
    property2: 0
  }
]

output testMap array = map(testArray, record => {
  result: record.property2 > 0 ? record.property1 : record.property1 =~ 'test' ? 'test!' : 'notTest!'
})

output testFor array = [for record in testArray: {
  result: record.property2 > 0 ? record.property1 : record.property1 =~ 'test' ? 'test!' : 'notTest!'
}]
");

        var evaluated = TemplateEvaluator.Evaluate(result.Template).ToJToken();
        evaluated.Should().HaveValueAtPath("$.outputs['testMap'].value", JToken.Parse(@"
[
  {
    ""result"": ""test""
  },
  {
    ""result"": ""dev""
  },
  {
    ""result"": ""test!""
  },
  {
    ""result"": ""prod""
  },
  {
    ""result"": ""notTest!""
  },
  {
    ""result"": ""notTest!""
  },
  {
    ""result"": ""test!""
  }
]
"));
    }

    /// <summary>
    /// https://github.com/Azure/bicep/issues/8782
    /// </summary>
    [TestMethod]
    public void Issue8782_2()
    {
        var bicepparamText = @"
using 'main.bicep'

param testObject = {
  a: true
  b: false
}
";

        var bicepTemplateText = @"
param testObject object
output output1 array = map(
  items(testObject),
  subObject => 1 == 2 ? [ 'yes' ] : [ 'no' ]
)
output output2 array = map(
  items(testObject),
  subObject => subObject.key == 'a' ? [ 'yes' ] : [ 'no' ]
)";

        var (parameters, diag, comp) = CompilationHelper.CompileParams(("parameters.bicepparam", bicepparamText), ("main.bicep", bicepTemplateText));

        var result = CompilationHelper.Compile(bicepTemplateText);


        var evaluated = TemplateEvaluator.Evaluate(result.Template, parameters).ToJToken();
        evaluated.Should().HaveValueAtPath("$.outputs['output1'].value", JToken.Parse(@"[
  [
    ""no""
  ],
  [
    ""no""
  ]
]"));
        evaluated.Should().HaveValueAtPath("$.outputs['output2'].value", JToken.Parse(@"[
  [
    ""yes""
  ],
  [
    ""no""
  ]
]"));
    }

    /// <summary>
    /// https://github.com/Azure/bicep/issues/8798
    /// </summary>
    [TestMethod]
    public void Issue8798()
    {
        var result = CompilationHelper.Compile(@"
var dogs = [
  {
    name: 'Evie'
    age: 5
    interests: ['Ball', 'Frisbee']
  }
  {
    name: 'Casper'
    age: 3
    interests: ['Other dogs']
  }
  {
    name: 'Indy'
    age: 2
    interests: ['Butter']
  }
  {
    name: 'Kira'
    age: 8
    interests: ['Rubs']
  }
]

output iDogs array = filter(dogs, dog =>  (contains(dog.name, 'C') || contains(dog.name, 'i')))
");

        var evaluated = TemplateEvaluator.Evaluate(result.Template).ToJToken();
        evaluated.Should().HaveValueAtPath("$.outputs['iDogs'].value", JToken.Parse(@"
[
  {
    ""name"": ""Evie"",
    ""age"": 5,
    ""interests"": [
      ""Ball"",
      ""Frisbee""
    ]
  },
  {
    ""name"": ""Casper"",
    ""age"": 3,
    ""interests"": [
      ""Other dogs""
    ]
  },
  {
    ""name"": ""Kira"",
    ""age"": 8,
    ""interests"": [
      ""Rubs""
    ]
  }
]
"));
    }

    [TestMethod]
    public void Module_with_unknown_resourcetype_as_parameter_and_output_has_diagnostics()
    {
        var bicepparamText = @"
using 'main.bicep'

param useMod1 = true
";

        var bicepTemplateText = @"
param useMod1 bool

module mod1 'module.bicep' = {
  name: 'test'
  params: {
    bar: 'abc'
  }
}

module mod2 'module.bicep' = {
  name: 'test2'
  params: {
    bar: 'def'
  }
}

var selectedMod = useMod1 ? mod1 : mod2
var selectedMod2 = true ? (useMod1 ? mod1 : mod2) : mod2

output test1 string = mod1.outputs.foo.bar
output test2 string = mod2.outputs.foo.bar
output test3 string = (useMod1 ? mod1 : mod2).outputs.foo.bar
output test4 string = selectedMod.outputs.foo.bar
output test5 string = selectedMod2.outputs.foo.bar
";
        var bicepModuleText = @"
param bar string

output foo object = {
  bar: bar
}
";

        var (parameters, diag, comp) = CompilationHelper.CompileParams(
            ("parameters.bicepparam", bicepparamText),
            ("main.bicep", bicepTemplateText),
            ("module.bicep", bicepModuleText));

        var result = CompilationHelper.Compile(
            ("main.bicep", bicepTemplateText),
            ("module.bicep", bicepModuleText));

        var evaluated = TemplateEvaluator.Evaluate(result.Template, parameters, config => config with
        {
            OnReferenceFunc = (resourceId, apiVersion, fullBody) =>
            {
                var id = ResourceGroupLevelResourceId.Parse(resourceId);
                var barVal = id.FormatName() == "test" ? "abc" : "def";
                return JToken.Parse(@"{
  ""outputs"": {
    ""foo"": {
      ""value"": {
        ""bar"": """ + barVal + @"""
      }
    }
  }
}");
            },
        }).ToJToken();

        evaluated.Should().HaveValueAtPath("$.outputs['test1'].value", "abc");
        evaluated.Should().HaveValueAtPath("$.outputs['test2'].value", "def");
        evaluated.Should().HaveValueAtPath("$.outputs['test3'].value", "abc");
        evaluated.Should().HaveValueAtPath("$.outputs['test4'].value", "abc");
        evaluated.Should().HaveValueAtPath("$.outputs['test5'].value", "abc");
    }

    [TestMethod]
    public void Az_getsecret_functions_are_evaluated_successfully()
    {
        var bicepTemplateText = @"
param param1 object
param param2 object
output output1 object = param1
output output2 object = param2
";

        var bicepparamText = @"
using 'main.bicep'
param param1 = { reference: 'param1' }
param param2 = union(param1, { reference: 'param2' })
";

        var (parameters, _, _) = CompilationHelper.CompileParams(("parameters.bicepparam", bicepparamText), ("main.bicep", bicepTemplateText));

        var (template, diagnostics, _) = CompilationHelper.Compile(bicepTemplateText);

        using (new AssertionScope())
        {
            var evaluated = TemplateEvaluator.Evaluate(template, parameters).ToJToken();

            diagnostics.Should().NotHaveAnyDiagnostics();

            evaluated.Should().HaveValueAtPath("$.outputs['output1'].value.reference", $"param1");
            evaluated.Should().HaveValueAtPath("$.outputs['output2'].value.reference", $"param2");
        }
    }

    [TestMethod]
    public void Az_getsecret_params_cannot_be_dereferenced()
    {
        var bicepTemplateText = @"
param param1 string
param param2 object
";

        var bicepparamText = @"
using 'main.bicep'
param param1 = getSecret('<subscriptionId>', '<resourceGroupName>', '<keyVaultName>', '<secretName>')
var var1 = 'foo_${param1}'
param param2 = {
  property: param1
  property2: var1
}
";

        var result = CompilationHelper.CompileParams(("parameters.bicepparam", bicepparamText), ("main.bicep", bicepTemplateText));

        result.Should().HaveDiagnostics(new[]
        {
                ("BCP368", DiagnosticLevel.Error, "The value of the \"param1\" parameter cannot be known until the template deployment has started because it uses a reference to a secret value in Azure Key Vault. Expressions that refer to the \"param1\" parameter may be used in .bicep files but not in .bicepparam files."),
                ("BCP368", DiagnosticLevel.Error, "The value of the \"param1\" parameter cannot be known until the template deployment has started because it uses a reference to a secret value in Azure Key Vault. Expressions that refer to the \"param1\" parameter may be used in .bicep files but not in .bicepparam files."),
            });
    }

    [TestMethod]
    public void Nulled_params_cannot_be_dereferenced()
    {
        var bicepTemplateText = @"
param param1 string = newGuid()
param param2 object
";

        var bicepparamText = @"
using 'main.bicep'
param param1 = null
var var1 = 'foo_${param1}'
param param2 = {
  property: param1
  property2: var1
}
";

        var result = CompilationHelper.CompileParams(("parameters.bicepparam", bicepparamText), ("main.bicep", bicepTemplateText));

        result.Should().HaveDiagnostics(new[]
        {
                ("BCP369", DiagnosticLevel.Error, "The value of the \"param1\" parameter cannot be known until the template deployment has started because it uses the default value defined in the template. Expressions that refer to the \"param1\" parameter may be used in .bicep files but not in .bicepparam files."),
                ("BCP369", DiagnosticLevel.Error, "The value of the \"param1\" parameter cannot be known until the template deployment has started because it uses the default value defined in the template. Expressions that refer to the \"param1\" parameter may be used in .bicep files but not in .bicepparam files."),
            });
    }

    [TestMethod]
    public void Safe_dereferences_are_evaluated_successfully()
    {
        var (template, _, _) = CompilationHelper.Compile(@"
var obj = {
  foo: [
    {
      bar: 'baz'
    }
  ]
}

resource testRes 'My.Rp/res1@2020-01-01' = {
  name: 'testRes'
  properties: obj
}

output properties object = {
  exists: testRes.?properties.foo[0].bar
  doesntExist: testRes.properties.?fizz[0].bar
  existsArrayAccess: testRes.properties.foo[?0].bar
  doesntExistArrayAccess: testRes.properties.foo[?10].bar
}
");

        using (new AssertionScope())
        {
            var evaluated = TemplateEvaluator.Evaluate(template).ToJToken();

            evaluated.Should().HaveValueAtPath("$.outputs['properties'].value.exists", "baz");
            evaluated.Should().HaveValueAtPath("$.outputs['properties'].value.doesntExist", JValue.CreateNull());
            evaluated.Should().HaveValueAtPath("$.outputs['properties'].value.existsArrayAccess", "baz");
            evaluated.Should().HaveValueAtPath("$.outputs['properties'].value.doesntExistArrayAccess", JValue.CreateNull());
        }
    }

    [TestMethod]
    public void Assertions_are_evaluated_correctly()
    {
        var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, AssertsEnabled: true));
        var bicepFile = @"
param accountName string
param environment string
param location string

resource stgAccount 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: toLower(accountName)
  location: resourceGroup().location
  kind: 'Storage'
  sku: {
    name: 'Standard_LRS'
  }
}

var myInt = 24

assert a1 = length(accountName) < myInt
assert a2 = contains(location, 'us')
assert a3 = environment == 'dev'
";


        var paramsFile = @"
using 'main.bicep'

// long string to trigger the assertion
param accountName = 'asdgkbauskfabdsfibasdogbnasdognbaosdingoaisdngoisdangoinbdsaoigbsadoibgodsiabgos'
param environment = 'dev'
param location = 'westus'
";

        var (parameters, _, _) = CompilationHelper.CompileParams(services, ("parameters.bicepparam", paramsFile), ("main.bicep", bicepFile));
        var (template, _, _) = CompilationHelper.Compile(services, bicepFile);

        using (new AssertionScope())
        {
            var evaluated = TemplateEvaluator.Evaluate(template, parameters).ToJToken();

            evaluated.Should().HaveValueAtPath("$.asserts['a1']", false);
            evaluated.Should().HaveValueAtPath("$.asserts['a2']", true);
            evaluated.Should().HaveValueAtPath("$.asserts['a3']", true);
        }
    }

    [TestMethod]
    public void Type_syntax_is_evaluated_correctly()
    {
        var (template, _, _) = CompilationHelper.Compile("""
                param foo string?

                output foo string = foo ?? 'not specified'
                """);

        var evaluated = TemplateEvaluator.Evaluate(template).ToJToken();

        evaluated.Should().HaveValueAtPath("$.languageVersion", "2.0");
        evaluated.Should().HaveValueAtPath("$.outputs.foo.value", "not specified");
    }

    [TestMethod]
    public void Functions_are_evaluated_correctly()
    {
        var bicepFile = @"
// yeah, this is a bit pointless
func joinWithSpace(values string[]) string => join(values, ' ')
func greet(names string[]) string => 'Hi, ${joinWithSpace(names)}!'

func replaceMultiple(value string, input object) string => reduce(
  map(items(input), i => { key: i.key, value: i.value, replaced: '' }),
  { replaced: value },
  (cur, next) => { replaced: replace(cur.replaced, next.key, next.value)}).replaced

output sayHiWithComposition string = greet(['Anthony', 'Martin'])
output sayHiWithLambdas string = replaceMultiple('Hi, $firstName $lastName!', {
  '$firstName': 'Anthony'
  '$lastName': 'Martin'
})
";

        var (template, _, _) = CompilationHelper.Compile(bicepFile);

        using (new AssertionScope())
        {
            var evaluated = TemplateEvaluator.Evaluate(template).ToJToken();

            evaluated.Should().HaveValueAtPath("$.outputs['sayHiWithComposition'].value", "Hi, Anthony Martin!");
            evaluated.Should().HaveValueAtPath("$.outputs['sayHiWithLambdas'].value", "Hi, Anthony Martin!");
        }
    }

    [TestMethod]
    public void New_functions_are_evaluated_correctly()
    {
        var bicepFile = @"
func isEven(i int) bool => i % 2 == 0

output sayHello string[] = map(
  ['Evie', 'Casper', 'Lady Lechuga'],
  (dog, i) => '${isEven(i) ? 'Hi' : 'Ahoy'} ${dog}!')

output evenEntries string[] = filter(['a', 'b', 'c', 'd'], (item, i) => isEven(i))

output concatIfEven string = reduce(['abc', 'def', 'ghi'], '', (cur, next, i) => isEven(i) ? concat(cur, next) : cur)

output mapValuesTest object = mapValues({
  a: 123
  b: 456
}, val => val * 2)

output objectKeysTest string[] = objectKeys({
  a: 123
  b: 456
})

output shallowMergeTest object = shallowMerge([{
  a: 123
}, {
  b: 456
}])

output groupByTest object = groupBy([
  { type: 'a', value: 123 }
  { type: 'b', value: 456 }
  { type: 'a', value: 789 }
], arg => arg.type)

output groupByWithValMapTest object = groupBy([
  { type: 'a', value: 123 }
  { type: 'b', value: 456 }
  { type: 'a', value: 789 }
], arg => arg.type, arg => arg.value)
";

        var (template, _, _) = CompilationHelper.Compile(bicepFile);

        using (new AssertionScope())
        {
            var evaluated = TemplateEvaluator.Evaluate(template).ToJToken();

            evaluated.Should().HaveJsonAtPath("$.outputs['sayHello'].value", """
[
  "Hi Evie!",
  "Ahoy Casper!",
  "Hi Lady Lechuga!"
]
""");

            evaluated.Should().HaveJsonAtPath("$.outputs['evenEntries'].value", """
[
  "a",
  "c"
]
""");

            evaluated.Should().HaveValueAtPath("$.outputs['concatIfEven'].value", "abcghi");

            evaluated.Should().HaveJsonAtPath("$.outputs['mapValuesTest'].value", """
{
  "a": 246,
  "b": 912
}
""");

            evaluated.Should().HaveJsonAtPath("$.outputs['objectKeysTest'].value", """
[
  "a",
  "b"
]
""");

            evaluated.Should().HaveJsonAtPath("$.outputs['shallowMergeTest'].value", """
{
  "a": 123,
  "b": 456
}
""");

            evaluated.Should().HaveJsonAtPath("$.outputs['groupByTest'].value", """
{
  "a": [
    {
      "type": "a",
      "value": 123
    },
    {
      "type": "a",
      "value": 789
    }
  ],
  "b": [
    {
      "type": "b",
      "value": 456
    }
  ]
}
""");

            evaluated.Should().HaveJsonAtPath("$.outputs['groupByWithValMapTest'].value", """
{
  "a": [
    123,
    789
  ],
  "b": [
    456
  ]
}
""");
        }
    }

    [TestMethod]
    public async Task ExternalInput_functions_are_evaluated_correctly()
    {
        var bicepFile = @"
param input int
param input2 string
param input3 string
param input4 object
param input5 bool
param input6 int
param myParam customType
param input7 customType
param nestedObj object

output output int = input
output output2 string = input2
output output3 string = input3
output output4 object = input4
output output5 bool = input5
output output6 int = input6
output output7 customType = input7
output nestedObj object = nestedObj

type customType = {
    foo: string
    bar: int
}
";

        var bicepparamText = @"
using 'main.bicep'

param input = int(externalInput('custom.binding', 'input'))

param input2 = string(externalInput('custom.uriForPath', {
  path: '/path/to/file'
}))

param input3 = '${externalInput('custom.binding', '__BINDING__')}_combined_with_${externalInput('custom.binding', '__ANOTHER_BINDING__')}'

param input4 = json(externalInput('custom.binding', 'objectInput'))

param input5 = bool(externalInput('sys.cli'))

var foo = int(externalInput('custom.binding', 'input'))

param input6 = foo

param myParam = json(externalInput('custom.binding', 'input7'))
param input7 = myParam
param nestedObj = {
  foo: externalInput('custom.binding', 'input')
}
"
;

        var paramResult = CompilationHelper.CompileParams(
            ("parameters.bicepparam", bicepparamText), ("main.bicep", bicepFile));

        var templateResult = CompilationHelper.Compile(bicepFile);

        var bindingValues = new Dictionary<JToken, string>
        {
            { "input",  "123" },
            { "__BINDING__", "test1" },
            { "__ANOTHER_BINDING__", "test2" },
            { "objectInput", "{ \"key1\": \"value1\", \"key2\": \"value2\" }" },
            { "input7", "{ \"foo\": \"bar\", \"bar\": 123 }" },
        };

        using (new AssertionScope())
        {
            templateResult.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            paramResult.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            // provide external inputs
            var parametersWithExternalInputs = await ParametersProcessor.Process(paramResult.Parameters!.ToString()!, async (kind, config) =>
            {
                await Task.CompletedTask;

                return kind switch
                {
                    "custom.binding" => bindingValues[config!],
                    "custom.uriForPath" => "https://example.com?sas=token",
                    "sys.cli" => "false",
                    _ => throw new InvalidOperationException(),
                };
            });

            var evaluated = TemplateEvaluator.Evaluate(templateResult.Template, JToken.Parse(parametersWithExternalInputs)).ToJToken();

            evaluated.Should().HaveValueAtPath("$.outputs['output'].value", 123);
            evaluated.Should().HaveValueAtPath("$.outputs['output2'].value", "https://example.com?sas=token");
            evaluated.Should().HaveValueAtPath("$.outputs['output3'].value", "test1_combined_with_test2");
            evaluated.Should().HaveValueAtPath("$.outputs['output4'].value", new JObject
            {
                ["key1"] = "value1",
                ["key2"] = "value2"
            });
            evaluated.Should().HaveValueAtPath("$.outputs['output5'].value", false);
            evaluated.Should().HaveValueAtPath("$.outputs['output6'].value", 123);
            evaluated.Should().HaveValueAtPath("$.outputs['output7'].value", new JObject
            {
                ["foo"] = "bar",
                ["bar"] = 123
            });
            evaluated.Should().HaveValueAtPath("$.outputs['nestedObj'].value", new JObject
            {
                ["foo"] = "123"
            });
        }
    }
}
