// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class AdminUsernameShouldNotBeLiteralRuleTests : LinterRuleTestsBase
    {
        private void CompileAndTest(string text, int expectedErrorCount, Options? options = null)
        {
            AssertLinterRuleDiagnostics(AdminUsernameShouldNotBeLiteralRule.Code, text, expectedErrorCount, options);
        }

        [DataRow(1, @" // This is the failing example in the docs
            resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
              name: 'name'
              location: resourceGroup().location
              properties: {
                osProfile: {
                  adminUsername: 'adminUsername'
                }
              }
            }
        ")]
        [DataTestMethod]
        public void If_UsesStringLiteral_ShouldFail(int diagnosticCount, string text)
        {
            CompileAndTest(text, diagnosticCount);
        }

        [DataRow(1, @"        
            resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
              name: 'name'
              location: resourceGroup().location
              properties: {
                osProfile: {
                  ADMINUSERNAME: 'adminUsername'
                }
              }
            }
            ")]
        [DataTestMethod]
        public void If_UsesStringLiteral_And_AdminUserNameMismatchesCase_ShouldStillFail(int diagnosticCount, string text)
        {
            CompileAndTest(text, diagnosticCount);
        }

        [DataRow(1, @"
            resource parent 'Microsoft.Compute/virtualMachines@2020-12-01' = {
              name: 'parent'
              location: resourceGroup().location
              resource child 'extensions' = {
                name: 'child'
                location: resourceGroup().location
                properties: {
                    adminUsername: 'adminUsername'
                }
              }
            }
        ")]
        [DataTestMethod]
        public void If_UsesStringLiteral_AndInsideChildResource_ShouldFail(int diagnosticCount, string text)
        {
            CompileAndTest(text, diagnosticCount);
        }

        [DataRow(3, @"
            resource parent 'Microsoft.Compute/virtualMachines@2020-12-01' = {
              name: 'parent'
              location: resourceGroup().location
              properties: {
                  adminUsername: 'adminUsername-parent'
              }
              resource child 'extensions' = {
                name: 'child'
                location: resourceGroup().location
                properties: {
                    adminUsername: 'adminUsername-child'
                }
              }
            }

            resource standalone 'Microsoft.Compute/virtualMachines@2020-12-01' = {
              name: 'standalone'
              location: resourceGroup().location
              properties: {
                adminUsername: 'adminUsername-standalone'
              }
            }
        ")]
        [DataTestMethod]
        public void If_MultipleResources_FindsAllErrors(int diagnosticCount, string text)
        {
            CompileAndTest(text, diagnosticCount);
        }


        [DataRow(3, @"
            resource parent 'Microsoft.Compute/virtualMachines@2020-12-01' = {
              name: 'parent'
              location: resourceGroup().location
              properties: {
                  adminUsername: 'adminUsername1'
                  level2: {
                    ADMINUSERNAME: 'adminUsername2'
                      level3: {
                        adminusername: 'adminUsername3'
                      }
                  }
              }
            }
        ")]
        [DataTestMethod]
        public void If_MultiplePropertiesLevels_FindsAllErrors(int diagnosticCount, string text)
        {
            CompileAndTest(text, diagnosticCount);
        }

        [DataRow(@"
            param p1 string

            resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
              name: 'name'
              location: resourceGroup().location
              properties: {
                osProfile: {
                  adminUsername: p1
                }
              }
            }        
        ")]
        [DataTestMethod]
        public void If_UsesParameter_ShouldPass(string text)
        {
            CompileAndTest(text, 0);
        }

        [DataRow(@"
            param p object = {
                v: 'hello'
            }

            resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
              name: 'name'
              location: resourceGroup().location
              properties: {
                osProfile: {
                  adminUsername: p.v
                }
              }
            }        
        ")]
        [DataTestMethod]
        public void If_UsesObjectParameterPropertyRef_ShouldPass(string text)
        {
            CompileAndTest(text, 0);
        }

        // TTK shows this error: AdminUsername ""adminUserName"" is variable which is not an expression  
        [DataRow(1, @"
            var adminUsername = 'hello'

            resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
              name: 'name'
              location: resourceGroup().location
              properties: {
                osProfile: {
                  adminUsername: adminUsername
                }
              }
            }        
        ")]
        [DataTestMethod]
        public void If_UsesStringVariable_ShouldFail(int diagnosticCount, string text)
        {
            CompileAndTest(text, diagnosticCount);
        }

        [DataRow(@"
            var v = 'value'
            resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
              name: 'name'
              location: resourceGroup().location
              properties: {
                osProfile: {
                  adminUsername: '${v}'
                }
              }
            }
        ")]
        [DataTestMethod]
        public void If_UsesStringInterpolation_ShouldPass(string text)
        {
            CompileAndTest(text, 0);
        }

        [DataRow(@"        
            resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
              name: 'name'
              location: resourceGroup().location
              properties: {
                osProfile: {
                  adminUsername: concat('a', 'b')
                }
              }
            }
        ")]
        [DataTestMethod]
        public void If_UsesExpression_ShouldPass(string text)
        {
            CompileAndTest(text, 0);
        }

        [DataRow(@"
            param location string
            param username string

            var v1 = {
              hello: 'username'
              there: username
            }

            resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
              name: 'name'
              location: location
              properties: {
                osProfile: {
                  adminUsername: v1.there
                }
              }
            }
        ")]
        [DataTestMethod]
        public void If_UsesObjectVariable_ThatResolvesToParameter_ShouldPass(string text)
        {
            CompileAndTest(text, 0);
        }

        [DataRow(@"
            param location string
            param username string

            var v1 = {
              hello: 'username'
              there: concat('a', 'b')
            }

            resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
              name: 'name'
              location: location
              properties: {
                osProfile: {
                  adminUsername: v1.there
                }
              }
            }
        ")]
        [DataTestMethod]
        public void If_UsesObjectVariable_ThatResolvesToStringExpression_ShouldPass(string text)
        {
            CompileAndTest(text, 0);
        }

        [DataRow(@"
            param location string
            param username string

            var v1 = {
              hello: 'username'
              there: IDoNotExist
            }

            resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
              name: 'name'
              location: resourceGroup().location
              properties: {
                osProfile: {
                  adminUsername: v1.there
                }
              }
            }
        ")]
        [DataTestMethod]
        public void If_UsesObjectVariable_ThatResolvesToUndefined_ShouldPass(string text)
        {
            CompileAndTest(text, 0, new Options(OnCompileErrors.Ignore));
        }

        [DataRow(@"
            param location string
            param username string

            var v1 = {
              hello: 'username'
              there: @ // whoops
            }

            resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
              name: 'name'
              location: resourceGroup().location
              properties: {
                osProfile: {
                  adminUsername: v1.there
                }
              }
            }
        ")]
        [DataTestMethod]
        public void If_UsesObjectVariables_ThatContainsSyntaxError_ShouldPass(string text)
        {
            CompileAndTest(text, 0, new Options(OnCompileErrors.Ignore));
        }

        [DataRow(@"
            param location string
            param username string

            var v1 = {
              hello: 'username'
              there: 'there'
            }

            resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
              name: 'name'
              location: resourceGroup().location
              properties: {
                osProfile: {
                  adminUsername: v1.IDontExist
                }
              }
            }
        ")]
        [DataTestMethod]
        public void If_UsesObjectVariables_AndReferencesInvalidProperty_ShouldPass(string text)
        {
            CompileAndTest(text, 0, new Options(OnCompileErrors.Ignore));
        }

        // TTK shows this error: AdminUsername references variable 'v1', which has a literal value.
        [DataRow(1, @"
            param location string
            var v1 = {
              hello: 'username'
            }

            resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
              name: 'hello'
              location: location
              properties: {
                osProfile: {
                  adminUsername: v1.hello
                }
              }
            }
        ")]
        [DataTestMethod]
        public void If_UsesObjectVariable_ThatResolvesToStringLiteral_ShouldFail(int diagnosticCount, string text)
        {
            CompileAndTest(text, diagnosticCount);
        }

        // TTK shows this error: AdminUsername references variable 'v1', which has a literal value.
        [DataRow(1, @"
            param location string
            var v1 = {
              hello: {
                there: 'username'
              }
            }

            resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
              name: 'hello'
              location: location
              properties: {
                osProfile: {
                  adminUsername: v1.hello.there
                }
              }
            }
        ")]
        [DataTestMethod]
        public void If_UsesNestedObjectVariable_ThatResolvesToStringLiteral_ShouldFail(int diagnosticCount, string text)
        {
            CompileAndTest(text, diagnosticCount);
        }

        [DataRow(1, @"
            var adminUsername = 'hello'
            var o1 = {
                o2: o2
            }
            var o2 = {
                admin: adminUsername
            }

            resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
              name: 'hello'
              location: resourceGroup().location
              properties: {
                osProfile: {
                  adminUsername: o1.o2.admin   // evaluates to a string literal
                }
              }
            }        
        ")]
        [DataTestMethod]
        public void If_UsesObjectVariable_ThatResolvesDeeplyToStringLiteral_ShouldFail(int diagnosticCount, string text)
        {
            CompileAndTest(text, diagnosticCount);
        }

        [DataRow("var adminUsername = 'hello'")]
        [DataRow("param adminUsername string = 'hello'")]
        [DataRow(@"
            var v = {
                adminUserName: 'hello'
            }
            ")]
        [DataRow(@"
            param adminUsername string
            param p object = {
                admin: adminUsername
            }
            ")]
        [DataRow("output adminUsername string = 'hello'")]
        [DataRow(@"
            output o object = {
                adminUsername: 'hello'
            }
            ")]
        [DataTestMethod]
        public void If_NotInsideResource_ShouldPass(string text)
        {
            CompileAndTest(text, 0);
        }
    }
}
