// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.UnitTests.Semantics
{

    [TestClass]
    public class ObjectDeserializationTests
    {

        private const string SIMPLE_JSON = """
            {
              "string": "someVal",
              "string1": "10",
              "int": 123,
              "array": [
                1,
                2
              ],
              "object": {
                "nestedString": "someVal",
                "nestedObject": {
                  "nestedInt": 1
                },
                "nestedArray": [
                  1,
                  2
                ]
              }
            }
            """;

        private const string COMPLEX_JSON = """
            {
              "label": "dateTimeFromEpoch",
              "kind": "function",
              "value": "```bicep\ndateTimeFromEpoch([epochTime: int]): string\n\n```\nConverts an epoch time integer value to an [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) dateTime string.\n",
              "documentation": {
                "kind": "markdown",
                "value": "```bicep\ndateTimeFromEpoch([epochTime: int]): string\n\n```\nConverts an epoch time integer value to an [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) dateTime string.\n"
              },
              "deprecated": false,
              "preselect": false,
              "sortText": "3_dateTimeFromEpoch",
              "insertTextFormat": "snippet",
              "insertTextMode": "adjustIndentation",
              "textEdit": {
                "range": {},
                "newText": "dateTimeFromEpoch($0)"
                },
              "command": {
                "title": "signature help",
                "command": "editor.action.triggerParameterHints"
              }
            }
            """;

        [TestMethod]
        public void Commented_YAML_file_content_gets_deserialized_into_JSON()
        {
            var yml = @"
                string: someVal
                int: 123
                array:
                - 1
                #comment
                - 2
                #comment
                object: #more comments
                    nestedString: someVal
                    nestedObject:
                        nestedInt: 1
                    nestedArray:
                    - 1
                    - 2";

            CompareSimpleJSON(yml);
        }

        [TestMethod]
        public void JSON_file_content_gets_deserialized_into_JSON()
        {
            var json = SIMPLE_JSON;
            CompareSimpleJSON(json);
        }


        private static void CompareSimpleJSON(string json)
        {
            var arguments = new FunctionArgumentSyntax[4];
            new YamlObjectParser().TryExtractFromObject(json, null, [arguments[0]], out var errorDiagnostic, out JToken? jToken);
            var correctList = new List<int> { 1, 2 };
            var correctObject = new Dictionary<string, int> { { "nestedInt", 1 }, };

            Assert.AreEqual("someVal", jToken!["string"]);
            Assert.AreEqual(123, jToken["int"]);

            CollectionAssert.AreEqual(correctList, jToken["array"]?.ToObject<List<int>>());
            Assert.AreEqual(1, jToken["array"]![0]);
            Assert.AreEqual(2, jToken["array"]![1]);

            CollectionAssert.AreEqual(correctObject, jToken["object"]?["nestedObject"]?.ToObject<Dictionary<string, int>>());
            Assert.AreEqual("someVal", jToken["object"]?["nestedString"]);

            Assert.AreEqual(1, jToken["object"]?["nestedObject"]?["nestedInt"]);
            Assert.AreEqual(1, jToken["object"]?["nestedArray"]![0]);
            Assert.AreEqual(2, jToken["object"]?["nestedArray"]![1]);
        }

        [TestMethod]
        public void Unparsable_YAML()
        {
            var invalidYml = @"
                string: someVal
                int: 123
                array:
                - 1
                // comment
                - 2
                object:
                    nestedString: someVal
                    nestedObject:
                        nestedInt: 1
                    nestedArray:
                    - 1
                    - 2";

            var span = new TextSpan(0, 10 - 0);
            new YamlObjectParser().TryExtractFromObject(invalidYml, null, [span], out var errorDiagnostic, out JToken? jToken);
            Assert.AreEqual(errorDiagnostic!.Code, "BCP340");
        }

        [TestMethod]
        public void Unparsable_JSON()
        {
            var invalidJson = @"
                string: someVal
                int: 123
                array:
                - 1
                // comment
                - 2
                object:
                    nestedString: someVal
                    nestedObject:
                        nestedInt: 1
                    nestedArray:
                    - 1
                    - 2";

            var span = new TextSpan(0, 10 - 0);
            new JsonObjectParser().TryExtractFromObject(invalidJson, null, [span], out var errorDiagnostic, out JToken? jToken);
            Assert.AreEqual(errorDiagnostic!.Code, "BCP186");
        }

        [TestMethod]
        public void Complex_JSON_gets_deserialized_into_JSON()
        {
            var json = COMPLEX_JSON;
            var arguments = new FunctionArgumentSyntax[4];
            new YamlObjectParser().TryExtractFromObject(json, null, [arguments[0]], out var errorDiagnostic, out JToken? jToken);
            var expectedValue = "```bicep\ndateTimeFromEpoch([epochTime: int]): string\n\n```\nConverts an epoch time integer value to an [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) dateTime string.\n";
            Assert.AreEqual(expectedValue, jToken!["documentation"]!["value"]);
        }


        [TestMethod]
        public void Simple_YAML_file_content_gets_deserialized_into_JSON()
        {
            var yml = @"
                 name: George Washington
                 age: 89
                 height_in_inches: 5.75
                 addresses:
                   home:
                     street:
                        house_number: '400'
                        street_name: Mockingbird Lane
                     city: Louaryland
                     state: Hawidaho
                     zip: 99970";

            var arguments = new FunctionArgumentSyntax[4];
            new YamlObjectParser().TryExtractFromObject(yml, null, [arguments[0]], out var errorDiagnostic, out JToken? jToken);

            Assert.AreEqual("George Washington", jToken!["name"]);
            Assert.AreEqual("400", jToken["addresses"]!["home"]!["street"]!["house_number"]);
            Assert.AreEqual(89, jToken["age"]);
            Assert.AreEqual("Louaryland", jToken["addresses"]!["home"]!["city"]);
        }
    }

}
