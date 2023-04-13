// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.RegularExpressions;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.UnitTests.Assertions;
using System.Dynamic;

namespace Bicep.Core.UnitTests.Semantics
{

    [TestClass]
    public class YamlDeserializationTests
    {
        private const string SIMPLE_JSON = """
            {
              "string": "someVal",
              "int": 123,
              /*
              this is a
              multi line
              comment
              */
              "array": [
                1,
                2
              ],
              //comment
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
                /*
                this is a
                multi line
                comment
                */
                array:
                - 1
                # comment
                - 2
                //comment
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

            var jToken = SystemNamespaceType.ExtractTokenFromObject(json);
            var correctList = new List<int> { 1, 2 };
            var correctObject = new Dictionary<string, int> { { "nestedInt", 1},  };

            Assert.AreEqual("someVal", jToken["string"]);
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

        private void AreJTokensEqual(JToken jTokenNew, JToken jTokenOld)
        {
            foreach (var child in jTokenNew.AsJEnumerable())
            {
                var jTokenNewChildPath = jTokenNew[child.Path.Contains('.') ? child.Path.Split(".")[child.Path.Split(".").Length - 1] : child.Path];
                var jTokenOldChildPath = jTokenOld[child.Path.Contains('.') ? child.Path.Split(".")[child.Path.Split(".").Length - 1] : child.Path];
                switch (jTokenNewChildPath)
                {
                    case JArray:
                        for (var x = 0; x < jTokenNewChildPath.AsArray().Length; x++)
                        {
                            AreJTokensEqual(jTokenNewChildPath![x]!, jTokenOldChildPath![x]!);
                        }
                        break;
                    case JObject:
                        AreJTokensEqual(jTokenNewChildPath, jTokenOldChildPath!);
                        break;
                    default:
                        Assert.AreEqual(jTokenNewChildPath, jTokenOldChildPath);
                        break;

                }
            }
        }

        [DataTestMethod]
        [DataRow(SIMPLE_JSON)]
        [DataRow(COMPLEX_JSON)]
        public void Compare_new_and_old_JSON_parsing(string json)
        {
            var jTokenNew = SystemNamespaceType.ExtractTokenFromObject(json);

#pragma warning disable CS0618 // Disable warning for obsolete method to verify functionality
            var jTokenOld = SystemNamespaceType.OldExtractTokenFromObject(json);
#pragma warning restore CS0618

            new JTokenEqualityComparer().Equals(jTokenNew, jTokenOld);
            Assert.AreEqual(jTokenNew["value"], jTokenOld["value"]);
            Assert.AreEqual(jTokenNew["documentation"]?["value"], jTokenOld["documentation"]?["value"]);
            AreJTokensEqual(jTokenNew, jTokenOld);

        }

        [TestMethod]
        public void Complex_JSON_gets_deserialized_into_JSON()
        {
            var json = COMPLEX_JSON;
            var jToken = SystemNamespaceType.ExtractTokenFromObject(json);
            var expectedValue = "```bicep\ndateTimeFromEpoch([epochTime: int]): string\n\n```\nConverts an epoch time integer value to an [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) dateTime string.\n";
            Assert.AreEqual(expectedValue, jToken["documentation"]?["value"]);

        }


        [TestMethod]
        public void Complex_JSON_gets_deserialized_into_JSON_by_old_method()
        {
            var json = COMPLEX_JSON;

#pragma warning disable CS0618 // Disable warning for obsolete method to verify functionality
            var jTokenOld = SystemNamespaceType.OldExtractTokenFromObject(json);
#pragma warning restore CS0618

            var exptectedValue = "```bicep\ndateTimeFromEpoch([epochTime: int]): string\n\n```\nConverts an epoch time integer value to an [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) dateTime string.\n";

            Assert.AreEqual(exptectedValue, jTokenOld["documentation"]?["value"]);

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
                        house_number: 400
                        street_name: Mockingbird Lane
                     city: Louaryland
                     state: Hawidaho
                     zip: 99970";

            var jToken = SystemNamespaceType.ExtractTokenFromObject(yml);
            Assert.AreEqual("George Washington", jToken["name"]);
            Assert.AreEqual(89, jToken["age"]);
            Assert.AreEqual("Louaryland", jToken["addresses"]!["home"]!["city"]);


        }
    }

}
