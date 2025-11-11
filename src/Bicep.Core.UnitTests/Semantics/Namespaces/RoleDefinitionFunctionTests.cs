// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Semantics.Namespaces
{
    [TestClass]
    public class RoleDefinitionFunctionTests
    {
        [TestMethod]
        public void RoleDefinitionFunction_CompilationTest()
        {
            var bicepText = @"
                var contributorRole = az.roleDefinition('Contributor')
                var ownerRole = az.roleDefinition('Owner')

                output contributorRoleId string = contributorRole.id
                output contributorRoleDefinitionId string = contributorRole.roleDefinitionId
                output ownerRoleId string = ownerRole.id
            ";

            var result = CompilationHelper.Compile(bicepText);
            result.Should().NotHaveAnyDiagnostics();
            result.Template.Should().NotBeNull();

            // Verify the ARM template contains the expected function calls
            var json = result.Template!.ToString();
            json.Should().Contain("roleDefinition('Contributor')");
            json.Should().Contain("roleDefinition('Owner')");
            json.Should().Contain(".id");
            json.Should().Contain(".roleDefinitionId");
        }

        [TestMethod]
        public void RoleDefinitionFunction_WithRoleAssignment_CompilationTest()
        {
            var bicepText = @"
                targetScope = 'subscription'

                param principalId string = '00000000-0000-0000-0000-000000000000'

                var contributorRole = az.roleDefinition('Contributor')

                resource roleAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
                  name: guid(principalId, 'contributor')
                  properties: {
                    roleDefinitionId: contributorRole.id
                    principalId: principalId
                  }
                }

                output assignedRoleId string = contributorRole.id
            ";

            var result = CompilationHelper.Compile(bicepText);
            result.Should().NotHaveAnyDiagnostics();
            result.Template.Should().NotBeNull();

            var json = result.Template!.ToString();
            json.Should().Contain("roleDefinition('Contributor').id");
            json.Should().Contain("Microsoft.Authorization/roleAssignments");
        }
    }
}
