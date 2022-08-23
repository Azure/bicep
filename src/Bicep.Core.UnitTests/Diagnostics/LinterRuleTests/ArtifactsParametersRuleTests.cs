// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CA1825 // Avoid zero-length array allocations

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class ArtifactsParametersRuleTests : LinterRuleTestsBase
    {
        private static void CompileAndTest(string bicepText, string[] expectedMessagesForCode, OnCompileErrors onCompileErrors = OnCompileErrors.IncludeErrors)
        {
            var options = new Options(onCompileErrors, IncludePosition.None);
            AssertLinterRuleDiagnostics(ArtifactsParametersRule.Code, bicepText, expectedMessagesForCode, options);
        }

        [TestMethod]
        public void MissingUnderscoreInParamNames_ArtifactsLocation_TreatAsNonArtifactsParameter()
        {
            CompileAndTest(
                @"
                    @secure()
                    param artifactsLocation string = 'something'

                    @secure()
                    param _artifactsLocationSasToken string = ''
                ",
                new string[]
                {
                        "If an '_artifactsLocationSasToken' parameter is provided, an '_artifactsLocation' parameter must also be provided.",
                }
            );
        }
        [TestMethod]
        public void MissingUnderscoreInParamNames_SasToken_TreatAsNonArtifactsParameter()
        {
            CompileAndTest(
            @"
                    @secure()
                    param _artifactsLocation string = deployment().properties.templateLink.uri

                    @secure()
                    param artifactsLocationSasToken string = 'something'
                "
            ,
            new string[]
            {
                "If an '_artifactsLocation' parameter is provided, an '_artifactsLocationSasToken' parameter must also be provided.",
            }
            );
        }
        [TestMethod]
        public void ArmTtk_DeploymentFunction_Pass()
        {
            CompileAndTest(
                @"
                    param _artifactsLocation string = deployment().properties.templateLink.uri

                    @secure()
                    param _artifactsLocationSasToken string = ''
                ",
                new string[]
                {
                }
           );
        }

        [TestMethod]
        public void ArmTtk_RawRepoPath_Pass()
        {
            CompileAndTest(
                @"
                    @secure()
                    param _artifactsLocation string = 'https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/sample-path-for-unit-tests-do-not-change/'

                    @secure()
                    param _artifactsLocationSasToken string = ''
                ",
                new string[]
                {
                }
           );
        }

        [TestMethod]
        public void NeitherParameterIsSupplied_Pass()
        {
            CompileAndTest(
                @"
                    param _artifactsLoc string
                    @secure()
                    param _artifactsLocSasToken string
                ",
                new string[]
                {
                }
           );
        }

        [TestMethod]
        public void ArmTtk_MissingDefaults_Both_Pass()
        {
            /* TTK result:
     [-] artifacts parameter (24 ms)                                                                                     
         The _artifactsLocation parameter in "mainTemplate.json" must have a defaultValue in the main template           
         The _artifactsLocationSasToken in "mainTemplate.json" has an incorrect defaultValue, must be an empty string   
 */
            CompileAndTest(
                @"
                    //both are missing defaultValues
                    param _artifactsLocation string
                    @secure()
                    param _artifactsLocationSasToken string
                ",
                new string[]
                {
                    // In the ARM TTK for a main template, this would have failed.  For Bicep, we accept this for all templates, and instead check that explicit
                    //   values for these parameters are passed in when a template with artifacts parameters is used as a module.
                    //   as modules
                }
           );
        }

        [TestMethod]
        public void ArmTtk_MissingDefaults_ArtifactsLocation_EmptyString_Fail()
        {
            /* TTK result:                                                                                  
                [-] artifacts parameter (2 ms)                                                                                      
                    The _artifactsLocation parameter in "mainTemplate.json" must have a defaultValue in the main template   
           */
            CompileAndTest(
                @"
                    @secure()  
                    param _artifactsLocation string = ''

                    @secure()
                    param _artifactsLocationSasToken string
                ",
                new string[]
                {
                    // In the ARM TTK for a main template, this would have failed because the default value for _artifactsLocation is "missing".
                    // For Bicep, we don't care if it's missing, but we don't accept an empty string.

                   "If the '_artifactsLocation' parameter has a default value, it must be a raw URL or an expression like 'deployment().properties.templateLink.uri'."
                }
           );
        }

        [TestMethod]
        public void ArmTtk_MissingDefaults_SasToken_Pass()
        {
            CompileAndTest(
                @"
                    param _artifactsLocation string = deployment().properties.templateLink.uri

                    @secure()
                    param _artifactsLocationSasToken string = ''
                ",
                new string[]
                {
                    // In the ARM TTK for a main template, this would have failed.  For Bicep, we accept this for all templates, and instead check that explicit
                    //   values for these parameters are passed in when a template with artifacts parameters is used as a module.
                    //   as modules
                }
           );
        }

        [TestMethod]
        public void ArmTtk_MissingSas_Fail()
        {
            /* TTK result:
                 [-] artifacts parameter (3 ms)                                                                                      
                    Template "mainTemplate.json" is missing _artifactsLocationSasToken parameter                                    
                    The _artifactsLocationSasToken in "mainTemplate.json" has an incorrect defaultValue, must be an empty string    
            */
            CompileAndTest(
                @"
                   param _artifactsLocation string = deployment().properties.templateLink.uri
                ",
                new string[]
                {
                    // In the ARM TTK for a main template, this would have failed for this reason:
                    //   The _artifactsLocationSasToken in "mainTemplate.json" has an incorrect defaultValue, must be an empty string
                    // For Bicep we don't care about that, but still both parameters must be present if either of them are.
                    "If an '_artifactsLocation' parameter is provided, an '_artifactsLocationSasToken' parameter must also be provided."
                }
           );
        }

        [TestMethod]
        public void ArmTtk_MissingLocation_Fail()
        {
            CompileAndTest(
                @"
                   param _artifactsLocationSasToken string
                ",
                new string[]
                {
                    // In the ARM TTK for a main template, this would have failed for this reason:
                    //   The _artifactsLocationSasToken in "mainTemplate.json" has an incorrect defaultValue, must be an empty string
                    // For Bicep we don't care about that, but still both parameters must be present if either of them are.
                    "If an '_artifactsLocationSasToken' parameter is provided, an '_artifactsLocation' parameter must also be provided."
                }
           );
        }

        [TestMethod]
        public void ArmTtk_WrongDefaults_Fail()
        {
            /* TTK result:
              [-] artifacts parameter (3 ms)                                                                                      
                    ENV:SAMPLE_NAME is empty - using placeholder for manual verification: 100-blank-template                        
                    The _artifactsLocation in ""mainTemplate.json"" has an incorrect defaultValue, found: http://myweb.com/           
                    Must be one of: https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/100-blank-template/ or deployment().properties.templateLink.uri 
            */

            /*                                                                 
                [-] Secure String Parameters Cannot Have Default (3 ms)                                                             
                    Parameter _artifactsLocationSasToken is a SecureString and must not have a default value unless it is an expression that contains the newGuid() function.
             */
            CompileAndTest(
                @"
                    param _artifactsLocation string = 'something besides an http'

                    @secure()
                    param _artifactsLocationSasToken string = 'hard coded token' //this shouldn't be a default (use a param if needed)
                ",
                new string[]
                {
                    $"If the '_artifactsLocation' parameter has a default value, it must be a raw URL or an expression like 'deployment().properties.templateLink.uri'.",
                    $"If the '_artifactsLocationSasToken' parameter has a default value, it must be an empty string.",
                });
        }

        [TestMethod]
        public void WrongDefaults2_Pass()
        {
            CompileAndTest(
                @"
                    param _artifactsLocation string
                    @secure()
                    param _artifactsLocationSasToken string
                ",
                new string[]
                {
                });
        }
        [TestMethod]
        public void WrongDefaults3_Fail()
        {
            CompileAndTest(
                @"
                    param _artifactsLocation string = deployment().properties
                    @secure()
                    param _artifactsLocationSasToken string = deployment().properties
                ",
                new string[]
                {
                    "If the '_artifactsLocation' parameter has a default value, it must be a raw URL or an expression like 'deployment().properties.templateLink.uri'.",
                    "If the '_artifactsLocationSasToken' parameter has a default value, it must be an empty string.",
                },
                OnCompileErrors.Ignore);
        }

        [TestMethod]
        public void ArmTtk_WrongType_Fail()
        {
            /* TTK result:
                [-] artifacts parameter (2 ms)                                                                                      
                    The _artifactsLocation in "mainTemplate.json" parameter must be a 'string' type in the parameter declaration "array"
                    The _artifactsLocationSasToken in "mainTemplate.json" parameter must be of type 'secureString'.                 
                    The _artifactsLocation parameter in "mainTemplate.json" must have a defaultValue in the main template           
                    The _artifactsLocationSasToken in "mainTemplate.json" has an incorrect defaultValue, must be an empty string    
             */
            CompileAndTest(
                @"
                    param _artifactsLocation array //wrong type
                    param _artifactsLocationSasToken int //wrong type
                ",
                new string[]
                {
                    "Artifacts parameter '_artifactsLocation' must be of type 'string'",
                    "Artifacts parameter '_artifactsLocationSasToken' must be of type 'string'",
                    "Artifacts parameter '_artifactsLocationSasToken' must use the @secure() attribute",
                }
           );
        }

        [TestMethod]
        public void ArmTtk_WrongType_NotSecure_Fail()
        {
            /* TTK result:
                [-] artifacts parameter (2 ms)                                                                                      
                    The _artifactsLocation in "mainTemplate.json" parameter must be a 'string' type in the parameter declaration "array"
                    The _artifactsLocationSasToken in "mainTemplate.json" parameter must be of type 'secureString'.                 
                    The _artifactsLocation parameter in "mainTemplate.json" must have a defaultValue in the main template           
                    The _artifactsLocationSasToken in "mainTemplate.json" has an incorrect defaultValue, must be an empty string    
             */
            CompileAndTest(
                @"
                    param _artifactsLocation string // okay
                    param _artifactsLocationSasToken string // should be secure
                ",
                new string[]
                {
                    "Artifacts parameter '_artifactsLocationSasToken' must use the @secure() attribute"
                }
           );
        }

        [TestMethod]
        public void ArmTtk_WrongType_InterpolatedString_Pass()
        {
            CompileAndTest(
                @"
                    param _artifactsLocation string = '${'a'}${'b'}'
                    @secure()
                    param _artifactsLocationSasToken string = '${'a'}${'b'}'
                ",
                new string[]
                {
                    "If the '_artifactsLocation' parameter has a default value, it must be a raw URL or an expression like 'deployment().properties.templateLink.uri'.",
                    "If the '_artifactsLocationSasToken' parameter has a default value, it must be an empty string."
                }
           );
        }

        [TestMethod]
        public void ArmTtk_WrongType_StringExpression_TypesOkay()
        {
            CompileAndTest(
                @"
                    param _artifactsLocation string = resourceGroup().location
                    @secure()
                    param _artifactsLocationSasToken string = resourceGroup().location
                ",
                new string[]
                {
                    "If the '_artifactsLocation' parameter has a default value, it must be a raw URL or an expression like 'deployment().properties.templateLink.uri'.",
                    "If the '_artifactsLocationSasToken' parameter has a default value, it must be an empty string.",
                }
           );
        }

        [TestMethod]
        public void ArmTtk_WrongType_StringVariable_TypesOkay()
        {
            CompileAndTest(
                @"
                    param resourceGroupLocation string = resourceGroup().location
                    param emptyString string = ''

                    param _artifactsLocation string = resourceGroupLocation
                    @secure()
                    param _artifactsLocationSasToken string = emptyString
                ",
                new string[]
                {
                    "If the '_artifactsLocation' parameter has a default value, it must be a raw URL or an expression like 'deployment().properties.templateLink.uri'.",
                    "If the '_artifactsLocationSasToken' parameter has a default value, it must be an empty string.",
                }
           );
        }

        [TestMethod]
        public void CaseInsensitive()
        {
            CompileAndTest(
                @"
                    param _ARTIFACTSLocation string = 'bad default'
                    @secure()
                    param _artifactsLocationSASToken string = 'bad default'
                ",
                new string[]
                {
                    "If the '_artifactsLocation' parameter has a default value, it must be a raw URL or an expression like 'deployment().properties.templateLink.uri'.",
                    "If the '_artifactsLocationSasToken' parameter has a default value, it must be an empty string.",
                }
           );
        }

        [TestMethod]
        public void InvalidParameterSyntax()
        {
            CompileAndTest(
                 @"
                    param _artifactsLocation
                    @secure()
                    param _artifactsLocationSASToken
                ",
                 new string[]
                 {
                 },
                 OnCompileErrors.Ignore
            );
        }

        private const string CreateVMBicepContents = @"
                        @description('OS Admin password or SSH Key depending on value of authentication type')
                        @secure()
                        param adminPasswordOrKey string

                        @description('The base URI where artifacts required by this template are located. When the template is deployed using the accompanying scripts, a private location in the subscription will be used and this value will be automatically generated.')
                        param _artifactsLocation string

                        @description('The sasToken required to access _artifactsLocation.')
                        @secure()
                        param _artifactsLocationSasToken string = ''

                        @description('Username for the Virtual Machine.')
                        param adminUsername string
        ";

        [TestMethod]
        public void ReferencingModulesWithArtifactsParameters_Pass()
        {
            var result = CompilationHelper.Compile(
                new (string fileName, string fileContents)[] {
                    ("createVM.bicep", CreateVMBicepContents),
                    ("main.bicep", @"
                        module creatingVM 'createVM.bicep' = {
                          name: 'creatingVM'
                          params: {
                            _artifactsLocation: 'my artifactsLocation'
                            _artifactsLocationSasToken: 'my artifactsLocationSasToken'
                            adminPasswordOrKey: 'adminPasswordOrKey'
                            adminUsername: 'adminUsername'
                          }
                        }
                    ")});

            result.Diagnostics.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void ReferencingModulesWithArtifactsParameters_Fail()
        {
            var result = CompilationHelper.Compile(
                new (string fileName, string fileContents)[] {
                    ("createVM.bicep", CreateVMBicepContents),
                    ("main.bicep", @"
                        module creatingVM 'createVM.bicep' = {
                          name: 'creatingVM'
                          params: {
                            adminPasswordOrKey: 'adminPasswordOrKey'
                            adminUsername: 'adminUsername'
                          }
                        }
                    ")});

            result.Diagnostics.Should().HaveDiagnostics(new[]
            {
                  ("artifacts-parameters", DiagnosticLevel.Warning, "Parameter '_artifactsLocation' of module 'creatingVM' should be assigned an explicit value."),
                  ("artifacts-parameters", DiagnosticLevel.Warning, "Parameter '_artifactsLocationSasToken' of module 'creatingVM' should be assigned an explicit value."),
                  ("BCP035", DiagnosticLevel.Error, "The specified \"object\" declaration is missing the following required properties: \"_artifactsLocation\"."),
            });
        }
    }
}
