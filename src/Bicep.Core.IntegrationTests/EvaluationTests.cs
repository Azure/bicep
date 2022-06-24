// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class EvaluationTests
    {
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
                var evaluated = TemplateEvaluator.Evaluate(template);

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
  evaluted:
\r\n\t\\\'\${}
'''

output literal string = str
output interp string = '>${bool}<>${int}<>${str}<>${obj}<>${arr}<'
output escapes string = '\r\n\t\\\'\${}'
output multiline string = multiline
");

            using (new AssertionScope())
            {
                var evaluated = TemplateEvaluator.Evaluate(template);

                evaluated.Should().HaveValueAtPath("$.outputs['literal'].value", "hello!");
                evaluated.Should().HaveValueAtPath("$.outputs['interp'].value", ">False<>12948<>hello!<>{'a':'b','!c':2}<>[true,2893,'abc']<");
                evaluated.Should().HaveValueAtPath("$.outputs['escapes'].value", "\r\n\t\\'${}");
                evaluated.Should().HaveValueAtPath("$.outputs['multiline'].value", "these escapes\n  are not\n  evaluted:\n\\r\\n\\t\\\\\\'\\${}\n");
            }
        }

        [TestMethod]
        public void ResourceId_expressions_are_evaluated_successfully()
        {
            var (template, _, _) = CompilationHelper.Compile(@"
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
");

            using (new AssertionScope())
            {
                var testSubscriptionId = "87d64d6d-6d17-4ad7-b507-16d9bc498781";
                var testRgName = "testRg";
                var evaluated = TemplateEvaluator.Evaluate(template, config => config with
                {
                    SubscriptionId = testSubscriptionId,
                    ResourceGroup = testRgName,
                    Parameters = new()
                    {
                        ["parentName"] = "myParent",
                        ["childName"] = "myChild",
                    }
                });

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
                var evaluated = TemplateEvaluator.Evaluate(template);

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
            var (template, _, _) = CompilationHelper.Compile(@"
param abcVal string

resource testRes 'My.Rp/res1@2020-01-01' = {
  name: 'testRes'
  properties: {
    abc: abcVal
  }
}

output abcVal string = testRes.properties.abc
");

            using (new AssertionScope())
            {
                var evaluated = TemplateEvaluator.Evaluate(template, config => config with
                {
                    Parameters = new()
                    {
                        ["abcVal"] = "test!!!",
                    },
                });

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
                var evaluated = TemplateEvaluator.Evaluate(template, config => config with
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
                });

                evaluated.Should().HaveValueAtPath("$.outputs['abcVal'].value", "test!!!");
            }
        }

        [TestMethod]
        public void Items_function_evaluation_works()
        {
            var result = CompilationHelper.Compile(@"
param inputObj object

output inputObjKeys array = [for item in items(inputObj): item.key]
output inputObjValues array = [for item in items(inputObj): item.value]
");

            var evaluated = TemplateEvaluator.Evaluate(result.Template, config => config with
            {
                Parameters = new()
                {
                    ["inputObj"] = new JObject
                    {
                        ["ghiKey"] = "ghiValue",
                        ["defKey"] = "defValue",
                        ["abcKey"] = "abcValue",
                        ["123Key"] = "123Value",
                        ["GHIKey"] = "GHIValue",
                        ["DEFKey"] = "DEFValue",
                        ["ABCKey"] = "ABCValue",
                        ["456Key"] = "456Value",
                    }
                }
            });

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

            var evaluated = TemplateEvaluator.Evaluate(result.Template);
            evaluated.Should().HaveValueAtPath("$.outputs['joined1'].value", "abcdefghi");
            evaluated.Should().HaveValueAtPath("$.outputs['joined2'].value", "abc,def,ghi");
            evaluated.Should().HaveValueAtPath("$.outputs['joined3'].value", "I love Bicep");
        }

        [TestMethod]
        public void indexof_contains_function_evaluation_works()
        {
            var (template, _, _) = CompilationHelper.Compile(@"
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
");

            using (new AssertionScope())
            {
                var evaluated = TemplateEvaluator.Evaluate(template, config => config with
                {
                    Parameters = new()
                    {
                        ["inputString"] = "FOOBAR",
                        ["inputArray"] = new JArray
                        {
                            "FOO",
                            "BAR"
                        }
                    }
                });

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
    }
}
