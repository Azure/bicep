// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.UnitTests.Mocks;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.JsonRpc;
using static Bicep.LanguageServer.Handlers.BicepDecompileForPasteCommandHandler;

namespace Bicep.LangServer.UnitTests.Handlers
{
    [TestClass]
    public class BicepDecompileForPasteParamsCommandHandlerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private BicepDecompileForPasteCommandHandler CreateHandler(LanguageServerMock server)
        {
            var builder = ServiceBuilder.Create(services => services
                .AddSingleton(StrictMock.Of<ISerializer>().Object)
                .AddSingleton(BicepTestConstants.CreateMockTelemetryProvider().Object)
                .AddSingleton(server.Mock.Object)
                .AddSingleton<BicepDecompileForPasteCommandHandler>()
                );

            return builder.Construct<BicepDecompileForPasteCommandHandler>();
        }

        private record Options(
            string pastedJson,
            PasteType? expectedPasteType = null,
            PasteContext expectedPasteContext = PasteContext.None,
            string? expectedBicep = null,
            bool ignoreGeneratedBicep = false,
            string? expectedErrorMessage = null,
            string? editorContentsWithCursor = null);

        private async Task TestDecompileForPaste(
            string json,
            PasteType? expectedPasteType = null,
            string? expectedBicep = null,
            string? expectedErrorMessage = null,
            string? editorContentsWithCursor = null)
        {
            await TestDecompileForPaste(new(
                json,
                expectedPasteType,
                PasteContext.None,
                expectedBicep: expectedBicep,
                ignoreGeneratedBicep: false,
                expectedErrorMessage: expectedErrorMessage,
                editorContentsWithCursor: editorContentsWithCursor));
        }

        private async Task TestDecompileForPaste(Options options)
        {
            var (editorContents, cursorOffset) = (options.editorContentsWithCursor is not null && options.editorContentsWithCursor.Contains('|'))
                ? ParserHelper.GetFileWithSingleCursor(options.editorContentsWithCursor, '|')
                : (string.Empty, 0);

            var editorContentsWithPastedJson = string.Concat(editorContents.AsSpan(0, cursorOffset), options.pastedJson, editorContents.AsSpan(cursorOffset));
            _ = FileHelper.SaveResultFile(TestContext, "main.bicep", editorContentsWithPastedJson);
            LanguageServerMock server = new();
            var handler = CreateHandler(server);


            var result = await handler.Handle(new(editorContentsWithPastedJson, cursorOffset, options.pastedJson.Length, options.pastedJson, queryCanPaste: false, "bicep-params"), CancellationToken.None);

            result.ErrorMessage.Should().Be(options.expectedErrorMessage);

            if (!options.ignoreGeneratedBicep)
            {
                var expectedBicep = options.expectedBicep?.Trim('\n');
                var actualBicep = result.Bicep?.Trim('\n');
                actualBicep.Should().EqualTrimmedLines(expectedBicep);
            }

            result.PasteContext.Should().Be(options.expectedPasteContext switch
            {
                PasteContext.None => "none",
                PasteContext.String => "string",
                _ => throw new NotImplementedException()
            });

            result.PasteType.Should().Be(options.expectedPasteType switch
            {
                PasteType.None => null,
                PasteType.JsonValue => "jsonValue",
                PasteType.BicepValue => "bicepValue",
                PasteType.FullParams => "fullParams",

                _ => throw new NotImplementedException(),
            });
        }

        #region JSON/Bicep Constants

        private const string jsonFullParamsTemplateMembers = """
                                                        "$schema": "https://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#",
                                                        "contentVersion": "1.0.0.0",
                                                        "parameters": {
                                                          "pString": {
                                                            "value": ""
                                                          },
                                                          "pInt": {
                                                            "value": 0
                                                          },
                                                          "pBool": {
                                                            "value": false
                                                          },
                                                          "pObject": {
                                                            "value": {}
                                                          },
                                                          "pArray": {
                                                            "value": []
                                                          }
                                                        }
                                                      """;
        private const string jsonFullParamsTemplate = $$"""
                                                      {
                                                        {{jsonFullParamsTemplateMembers}}
                                                      }
                                                      """;

        #endregion

        [DataTestMethod]
        [DataRow(
            jsonFullParamsTemplate,
            PasteType.FullParams,
            """
            using '' /*TODO: Provide a path to a bicep template*/
            
            param pString = ''
            
            param pInt = 0
            
            param pBool = false
            
            param pObject = {}
            
            param pArray = []
            """,
            DisplayName = "Full Params"
        )]
        [DataRow(
            $$"""
              {
              {{jsonFullParamsTemplateMembers}}
                  , extraProperty: "hello"
              }
              """,
            PasteType.FullParams,
            """
            using '' /*TODO: Provide a path to a bicep template*/
            
            param pString = ''
            
            param pInt = 0
            
            param pBool = false
            
            param pObject = {}
            
            param pArray = []
            """,
            DisplayName = "Extra property"
        )]
        [DataRow(
            $$"""
              {
              {{jsonFullParamsTemplateMembers}}
              }
              } // extra
              """,
            PasteType.FullParams,
            """
            using '' /*TODO: Provide a path to a bicep template*/
            
            param pString = ''
            
            param pInt = 0
            
            param pBool = false
            
            param pObject = {}
            
            param pArray = []
            """,
            DisplayName = "Extra brace at end (succeeds)"
        )]
        [DataRow(
            $$"""
              {
              {{jsonFullParamsTemplateMembers}}
              }
              random characters
              """,
            PasteType.FullParams,
            """
            using '' /*TODO: Provide a path to a bicep template*/
            
            param pString = ''
            
            param pInt = 0
            
            param pBool = false
            
            param pObject = {}
            
            param pArray = []
            """,
            DisplayName = "Extra random characters at end"
        )]
        [DataRow(
            $$"""
              {
              { // extra
              {{jsonFullParamsTemplateMembers}}
              }
              random characters
              """,
            PasteType.None,
            null,
            DisplayName = "Extra brace at beginning (can't paste)"
        )]
        [DataRow(
            $$"""

              random characters
              {
              {{jsonFullParamsTemplateMembers}}
              }
              """,
            PasteType.None,
            null,
            DisplayName = "Extra random characters at beginning (can't paste)"
        )]
        [DataRow(
            """
            {
                "$schema": {},
                "parameters": {
                    "test": {
                        "value": "test"
                    }
                }
            }
            """,
            PasteType.JsonValue,
            // Treats it simply as a JSON object
            """
              {
                  '$schema': {}
                  parameters: {
                      test: {
                          value: 'test'
                      }
                  }
              }
              """,
            DisplayName = "Schema not a string"
        )]
        public async Task FullJsonParams(string json, PasteType expectedPasteType, string expectedBicep, string? errorMessage = null)
        {
            await TestDecompileForPaste(
                    json: json,
                    expectedPasteType: expectedPasteType,
                    expectedBicep: expectedBicep,
                    errorMessage);
        }

        [TestMethod]
        public async Task JustString_WithNoQuotes_CantConvert()
        {
            string json = @"just a string";
            await TestDecompileForPaste(
                    json: json,
                    PasteType.None,
                    expectedErrorMessage: null,
                    expectedBicep: null);
        }


        [DataTestMethod]
        [DataRow(
            """
            "just a string with double quotes"
            """,
            @"'just a string with double quotes'",
            DisplayName = "String with double quotes"
        )]
        [DataRow(
            """{"hello": "there"}""",
            """
            {
                            hello: 'there'
                        }
            """,
            DisplayName = "simple object"
        )]
        [DataRow(
            """{"hello there": "again"}""",
            """
            {
                            'hello there': 'again'
                        }
            """,
            DisplayName = "object with properties needing quotes"
        )]
        [DataRow(
            """
            "[resourceGroup().location]"
            """,
            @"resourceGroup().location",
            DisplayName = "String with ARM expression"
        )]
        [DataRow(
            """["[resourceGroup().location]"]""",
            """
            [
              resourceGroup().location
            ]
            """,
            DisplayName = "Array with string expression"
        )]
        [DataRow(
            """
            "[concat(variables('leftBracket'), 'dbo', variables('rightBracket'), '.', variables('leftBracket'), 'table', variables('rightBracket')) ]"
            """,
            @"'${leftBracket}dbo${rightBracket}.${leftBracket}table${rightBracket}'",
            DisplayName = "concat changes to interpolated string"
        )]
        [DataRow(
            """
            "[concat('Correctly escaped single quotes ''here'' and ''''here'''' ', variables('and'), ' ''wherever''')]"
            """,
            @"'Correctly escaped single quotes \'here\' and \'\'here\'\' ${and} \'wherever\''",
            DisplayName = "Correctly escaped single quotes"
        )]
        [DataRow(
            """
            "[concat('string', ' ', 'string')]"
            """,
            @"'string string'",
            DisplayName = "Concat is simplified"
        )]
        [DataRow(
            """
            "[concat('''Something in single quotes - '' ', 'and something not ', variables('v1'))]"
            """,
            @"'\'Something in single quotes - \' and something not ${v1}'",
            DisplayName = "Escaped and unescaped single quotes in string"
        )]
        [DataRow(
            """
            "[[this will be in brackets, not an expression - variables('blobName') should not be converted to Bicep, but single quotes should be escaped]"
            """,
            @"'[this will be in brackets, not an expression - variables(\'blobName\') should not be converted to Bicep, but single quotes should be escaped]'",
            DisplayName = "string starting with [[ is not an expression, [[ should get converted to single ["
        )]
        [DataRow(
            """
            "[json(concat('{\"storageAccountType\": \"Premium_LRS\"}'))]"
            """,
            """json('{"storageAccountType": "Premium_LRS"}')""",
            DisplayName = "double quotes inside strings inside object"
        )]
        [DataRow(
            """
            "[concat(variables('blobName'),parameters('blobName'))]"
            """,
            "concat(blobName, blobName_param)",
            DisplayName = "param and variable with same name"
        )]
        [DataRow(
            """
            "Double quotes \"here\""
            """,
            """'Double quotes "here"'""",
            DisplayName = "Double quotes in string"
        )]
        [DataRow(
            """'Double quotes \"here\"'""",
            """'Double quotes "here"'""",
            DisplayName = "Double quotes in single-quote string"
        )]
        [DataRow(
            """
            "['Double quotes \"here\"']"
            """,
            """'Double quotes "here"'""",
            DisplayName = "Double quotes in string inside string expression"
        )]
        [DataRow(
            """
            " [A string that has whitespace before the bracket is not an expression]"
            """,
            @"' [A string that has whitespace before the bracket is not an expression]'",
            DisplayName = "Whitespace before the bracket"
        )]
        [DataRow(
            """
            "[A string that has whitespace after the bracket is not an expression] "
            """,
            @"'[A string that has whitespace after the bracket is not an expression] '",
            DisplayName = "Whitespace after the bracket"
        )]
        [DataRow(
            """
            [
                1, 2,
                3
            ]
            """,
            """
            [
              1
              2
              3
            ]
            """,
            DisplayName = "Multiline array"
        )]
        [DataRow(
            "  \t \"abc\" \t  ",
            "'abc'",
            DisplayName = "Whitespace before/after")]
        [DataRow(
            "  /*comment*/\n // another comment\r\n /*hi*/\"abc\"// there\n/*and another*/ // but wait, there's more!\n// end",
            "'abc'",
            DisplayName = "Comments before/after")]
        [DataRow(
            "\t\"abc\"\t",
            "'abc'",
            DisplayName = "Tabs before/after")]
        [DataRow(
            "\"2012-03-21T05:40Z\"",
            "'2012-03-21T05:40Z'",
            DisplayName = "datetime string")]
        public async Task JsonValue_Valid_ShouldSucceed(string json, string expectedBicep)
        {
            await TestDecompileForPaste(
                    json: json,
                    expectedBicep is null ? PasteType.None : PasteType.JsonValue,
                    expectedErrorMessage: null,
                    expectedBicep: expectedBicep);
        }

        [DataTestMethod]
        [DataRow(
            """
            {
              ipConfigurations: [
                {
                  name: 'ipconfig1'
                  properties: {
                    subnet: {
                      id: 'subnetRef'
                    }
                    privateIPAllocationMethod: 'Dynamic'
                    publicIpAddress: {
                      id: resourceId('Microsoft.Network/publicIPAddresses', 'publicIPAddressName')
                    }
                  }
                }
              ]
              networkSecurityGroup: {
                id: resourceId('Microsoft.Network/networkSecurityGroups', 'networkSecurityGroupName')
              }
            }
            """,
            DisplayName = "Bicep object"
        )]
        [DataRow(
            @"3/14/2001",
            DisplayName = "Date"
        )]
        [DataRow(
            """
            "kubernetesVersion": "1.15.7"
            """,
            DisplayName = "\"property\": \"value\""
        )]
        [DataRow(
            @"// hello there",
            DisplayName = "single-line comment"
        )]
        [DataRow(
            """
            /* hello
                            there */
            """,
            DisplayName = "multi-line comment"
        )]
        [DataRow(
            @"resourceGroup().location",
            DisplayName = "Invalid JSON expression"
        )]
        [DataRow(
            @"[resourceGroup().location]",
            DisplayName = "Invalid JSON expression inside array"
        )]
        [DataRow(
            @"[concat('Unescaped single quotes 'here' ')]",
            DisplayName = "Invalid unescaped single quotes"
        )]
        [DataRow(
            @"",
            DisplayName = "Empty")]
        [DataRow(
            "  \t  ",
            DisplayName = "just whitespace")]
        [DataRow(
            @" /* hello there! */ // more comment",
            DisplayName = "just a comment")]
        [DataRow(
            "2012-03-21T05:40Z",
            DisplayName = "datetime")]
        public async Task JsonValue_Invalid_CantConvert(string json)
        {
            await TestDecompileForPaste(
                    json: json,
                    PasteType.None,
                    expectedErrorMessage: null,
                    expectedBicep: null);
        }

        [DataTestMethod]
        [DataRow(
            @"{ abc: 1, def: 'def' }", // this is not technically valid JSON but the Newtonsoft parser accepts it anyway and it is already valid Bicep
            PasteType.BicepValue, // Valid json and valid Bicep expression
            """
            {
                            abc: 1
                            def: 'def'
                        }
            """)]
        [DataRow(
            @"{ abc: 1, /*hi*/ def: 'def' }", // this is not technically valid JSON but the Newtonsoft parser accepts it anyway and it is already valid Bicep
            PasteType.BicepValue, // Valid json and valid Bicep expression
            """
            {
                            abc: 1
                            def: 'def'
                        }
            """)]
        [DataRow(
            "[1]",
            PasteType.BicepValue, // Valid json and valid Bicep expression
            """
            [
                          1
                        ]
            """)]
        [DataRow(
            "[1, 1]",
            PasteType.BicepValue, // Valid json and valid Bicep expression
            """
            [
              1
              1
            ]
            """)]
        [DataRow(
            "[      /* */  ]",
            PasteType.BicepValue, // Valid json and valid Bicep expression
            "[]")]
        [DataRow(
            """
            [
            /* */  ]
            """,
            PasteType.BicepValue, // Valid json and valid Bicep expression
        "[]")]
        [DataRow(
            """
            [
              1]
            """,
            PasteType.BicepValue, // Valid json and valid Bicep expression
            """
            [
              1
            ]
            """)]
        [DataRow(
            "null",
            PasteType.BicepValue, // Valid json and valid Bicep expression
            "null",
            DisplayName = "null")]
        [DataRow(
            @"'just a string with single quotes'",
            PasteType.BicepValue, // Valid json and valid Bicep expression
            @"'just a string with single quotes'",
            DisplayName = "String with single quotes"
        )]
        [DataRow(
            """
            // comment that shouldn't get removed because code is already valid Bicep
            
                            /* another comment
            
                            */
                            '123' // yet another comment
            
                            /* and another
                            */
            
                            
            """,
            PasteType.BicepValue, // Valid json and valid Bicep expression (will get pasted as original for copy/paste, as '123' for "paste as Bicep" command)
            "'123'",
            DisplayName = "Regress #10940 Paste removes comments when copying/pasting Bicep"
        )]
        [DataRow(
            """
            param p1 string
                          param p2 string
            """,
            PasteType.None, // Valid Bicep, but not as a single expression
            null,
            DisplayName = "multiple valid Bicep statements - shouldn't be changed"
        )]
        [DataRow(
            """
            param p1 string // comment 1
                          // comment 2
                          param p2 string /* comment 3 */
            """,
            PasteType.None, // Valid Bicep, but not as a single expression
            null,
            DisplayName = "multiple valid Bicep statements with comments - shouldn't be changed"
        )]
        [DataRow(
            """
            [
                            1
                            2
                        ]
            """,
            PasteType.None, // Valid Bicep but not valid JSON, therefore not converted
            null,
            DisplayName = "multi-line valid Bicep expression - shouldn't be changed"
        )]
        [DataRow(
            """
            [
                            1 // comment 1
                            // comment 2
                            2 /* comment 3
                            3
                            /*
                            4
                        ]
            """,
            PasteType.None, // Valid Bicep but not valid JSON, therefore not converted
            null,
            DisplayName = "multi-line valid Bicep expression with comments - shouldn't be changed"
        )]
        public async Task IsAlreadyLegalBicep(string pasted, PasteType expectedPasteType, string expectedBicep)
        {
            await TestDecompileForPaste(
                    pasted,
                    expectedPasteType,
                    expectedBicep,
                    expectedErrorMessage: null);
        }

        [TestMethod]
        public async Task Template_JsonConvertsToEmptyBicep()
        {
            await TestDecompileForPaste(
                """
                {
                  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
                  "parameters": {  },
                }
                """,
                    PasteType.FullParams,
                    expectedErrorMessage: null,
                    expectedBicep: "using '' /*TODO: Provide a path to a bicep template*/");
        }

        [TestMethod]
        public async Task Template_JsonConvertsToEmptyBicepIfUsingIsPresent()
        {
            await TestDecompileForPaste(
                """
                {
                  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
                  "parameters": {  },
                }
                """,
                PasteType.None,
                expectedErrorMessage: null,
                expectedBicep: null,
                editorContentsWithCursor: """
                                          using 'exiting.bicep'
                                          |
                                          """
                );
        }

        [DataTestMethod]
        [DataRow(
            """
            |using ''
            param s = ''
            """,
            PasteContext.None,
            DisplayName = "simple: cursor at start of bicep file"
        )]
        [DataRow(
            """
            using ''
            param s = ''|
            """,
            PasteContext.None,
            DisplayName = "simple: cursor at end of bicep file"
        )]
        [DataRow(
            @"var a = |'bicep string'",
            PasteContext.None,
            DisplayName = "variable value: right before string's beginning single quote"
        )]
        [DataRow(
            @"var a = '|bicep string'",
            PasteContext.String,
            DisplayName = "variable value: right after string's beginning single quote"
        )]
        [DataRow(
            @"var a = 'bicep |string'",
            PasteContext.String,
            DisplayName = "variable value: in middle"
        )]
        [DataRow(
            @"var a = 'bicep string|'",
            PasteContext.String,
            DisplayName = "variable value: right before string's ending single quote"
        )]
        [DataRow(
            @"var a = 'bicep string'|",
            PasteContext.None,
            DisplayName = "variable value: right after string's ending single quote"
        )]
        [DataRow(
            @"var a = 1 // 'not a| string'",
            PasteContext.None,
            DisplayName = "comments: string inside a comment"
        )]
        [DataRow(
            @"var a = /* 'not a| string' */ 123",
            PasteContext.None,
            DisplayName = "comments: string inside a /**/ comment"
        )]
        [DataRow(
            """
            var a = /* 
                            'not a| string' */ 123
                            
            """,
            PasteContext.None,
            DisplayName = "comments: string inside a multiline comment"
        )]
        // @description does not use a StringSyntax, we have to look for string tokens...
        [DataRow(
            """
            @description(|'bicep string')
            var s = 'str'
            """,
            PasteContext.None,
            DisplayName = "@description: before beginning quote"
        )]
        [DataRow(
            """
            @description('|bicep string')
            var s = 'str'
            """,
            PasteContext.String,
            DisplayName = "@description: after beginning quote"
        )]
        [DataRow(
            """
            @description('bicep string|')
            var s = 'str'
            """,
            PasteContext.String,
            DisplayName = "@description: before end quote"
        )]
        [DataRow(
            """
            @description('bicep string'|)
            var s = 'str'
            """,
            PasteContext.None,
            DisplayName = "@description: after end quote"
        )]
        [DataRow(
            """
            var s = |'''hello ${not a hole}
                            there '''
            """,
            PasteContext.None,
            DisplayName = "multi-line string: just before starting quotes"
        )]
        [DataRow(
            """
            var s = '''|hello ${not a hole}
                            there '''
            """,
            PasteContext.String,
            DisplayName = "multi-line string: just inside string"
        )]
        [DataRow(
            """
            var s = '''hello ${not a |hole}
                            there '''
            """,
            PasteContext.String,
            DisplayName = "multi-line string: not a hole"
        )]
        [DataRow(
            """
            var s = '''hello ${not a hole}
                            there |'''
            """,
            PasteContext.String,
            DisplayName = "multi-line string: just before ending quotes"
        )]
        [DataRow(
            """
            var s = '''hello ${not a hole}
                            there '''|
            """,
            PasteContext.None,
            DisplayName = "multi-line string: just outside ending quotes"
        )]
        public async Task DontPasteIntoStrings(string editorContentsWithCursor, PasteContext expectedPasteContext)
        {
            await TestDecompileForPaste(new(
                "\"json string\"",
                expectedPasteContext == PasteContext.String ? PasteType.None : PasteType.JsonValue,
                expectedPasteContext,
                ignoreGeneratedBicep: true,
                expectedErrorMessage: null,
                editorContentsWithCursor: editorContentsWithCursor
            ));

            await TestDecompileForPaste(new(
                """
                {
                  "type": "Microsoft.Resources/resourceGroups",
                  "apiVersion": "2022-09-01",
                  "name": "rg",
                  "location": "[parameters('location')]"
                }
                """,
                expectedPasteType: expectedPasteContext == PasteContext.String ? PasteType.None : PasteType.JsonValue,
                expectedPasteContext,
                ignoreGeneratedBicep: true,
                expectedErrorMessage: null,
                editorContentsWithCursor: editorContentsWithCursor
            ));
        }
    }
}

