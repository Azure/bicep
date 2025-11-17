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

            [TestMethod]
    public void Multi_document_YAML_with_separator_triggers_error()
    {
        var multiDocYml = @"---
                          name: George Washington
                          age: 89
                          ---
                          name: John Adams
                          age: 90";

        var span = new TextSpan(0, 10);
        new YamlObjectParser().TryExtractFromObject(multiDocYml, null, [span], out var errorDiagnostic, out JToken? jToken);
        
        Assert.IsNotNull(errorDiagnostic);
        Assert.AreEqual("BCP442", errorDiagnostic.Code); 
        Assert.IsNull(jToken);
    }

    [TestMethod]
    public void Multi_document_YAML_with_multiple_separators_triggers_error()
    {
        var multiDocYml = @"---
                          document: first
                          ---
                          document: second
                          ---
                          document: third";

        var span = new TextSpan(0, 10);
        new YamlObjectParser().TryExtractFromObject(multiDocYml, null, [span], out var errorDiagnostic, out JToken? jToken);
        
        Assert.IsNotNull(errorDiagnostic);
        Assert.AreEqual("BCP442", errorDiagnostic.Code);
        Assert.IsNull(jToken);
    }

    [TestMethod]
    public void Single_document_YAML_with_leading_separator_parses_successfully()
    {
        var singleDocYml = @"---
                          name: George Washington
                          age: 89
                          addresses:
                            home:
                              city: Louaryland";

        var arguments = new FunctionArgumentSyntax[4];
        new YamlObjectParser().TryExtractFromObject(singleDocYml, null, [arguments[0]], out var errorDiagnostic, out JToken? jToken);
        
        Assert.IsNull(errorDiagnostic);
        Assert.IsNotNull(jToken);
        Assert.AreEqual("George Washington", jToken!["name"]);
        Assert.AreEqual(89, jToken["age"]);
    }

    [TestMethod]
    public void Single_document_YAML_without_separator_parses_successfully()
    {
        var singleDocYml = @"
                          name: Thomas Jefferson
                          age: 83
                          city: Monticello";

        var arguments = new FunctionArgumentSyntax[4];
        new YamlObjectParser().TryExtractFromObject(singleDocYml, null, [arguments[0]], out var errorDiagnostic, out JToken? jToken);
        
        Assert.IsNull(errorDiagnostic);
        Assert.IsNotNull(jToken);
        Assert.AreEqual("Thomas Jefferson", jToken!["name"]);
        Assert.AreEqual(83, jToken["age"]);
    }

    [TestMethod]
    public void Multi_document_YAML_with_comments_between_separators_triggers_error()
    {
        var multiDocYml = @"---
                          # First document
                          name: George Washington
                          age: 89
                          # End of first document
                          ---
                          # Second document
                          name: John Adams
                          age: 90";

        var span = new TextSpan(0, 10);
        new YamlObjectParser().TryExtractFromObject(multiDocYml, null, [span], out var errorDiagnostic, out JToken? jToken);
        
        Assert.IsNotNull(errorDiagnostic);
        Assert.AreEqual("BCP442", errorDiagnostic.Code);
        Assert.IsNull(jToken);
    }

    [TestMethod]
    public void YAML_with_separator_in_middle_of_content_triggers_error()
    {
        var multiDocYml = @"
                          name: George Washington
                          age: 89
                          ---
                          addresses:
                            home:
                              city: Louaryland";

        var span = new TextSpan(0, 10);
        new YamlObjectParser().TryExtractFromObject(multiDocYml, null, [span], out var errorDiagnostic, out JToken? jToken);
        
        Assert.IsNotNull(errorDiagnostic);
        Assert.AreEqual("BCP442", errorDiagnostic.Code);
        Assert.IsNull(jToken);
    }

    [TestMethod]
    public void YAML_with_whitespace_around_separator_triggers_error()
    {
        var multiDocYml = @"
                          name: George Washington
                            ---  
                          name: John Adams";

        var span = new TextSpan(0, 10);
        new YamlObjectParser().TryExtractFromObject(multiDocYml, null, [span], out var errorDiagnostic, out JToken? jToken);
        
        Assert.IsNotNull(errorDiagnostic);
        Assert.AreEqual("BCP442", errorDiagnostic.Code);
        Assert.IsNull(jToken);
    }

    [TestMethod]
    public void YAML_with_only_comments_and_empty_lines_before_separator_parses_successfully()
    {
        var singleDocYml = @"
                          # This is a comment

                          ---
                          # Another comment
                          name: George Washington
                          age: 89";

        var span = new TextSpan(0, 10);  // ← Add this line
        new YamlObjectParser().TryExtractFromObject(singleDocYml, null, [span], out var errorDiagnostic, out JToken? jToken);
        
        Assert.IsNull(errorDiagnostic);
        Assert.IsNotNull(jToken);
        Assert.AreEqual("George Washington", jToken!["name"]);
    }
        }

    }
