// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Expression.Configuration;
using Azure.Deployments.Expression.Serializers;
using Bicep.Decompiler.ArmHelpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests.ArmHelpers
{
    [TestClass]
    public class ExpressionHelpersTests
    {
        private static readonly ExpressionSerializer ExpressionSerializer = new ExpressionSerializer(new ExpressionSerializerSettings
        {
            IncludeOuterSquareBrackets = true,

            // this setting will ensure that we emit strings instead of a string literal expressions
            SingleStringHandling = ExpressionSerializerSingleStringHandling.SerializeAsString
        });

        [DataTestMethod]
        [DataRow("[concat('abc', concat('def'), 'ghi')]", "abcdefghi")]
        [DataRow("[format('{0}/{1}/{2}', 'abc', concat('def'), 'ghi')]", "abc/def/ghi")]
        [DataRow("[format('{2}/{0}/{1}', 'abc', concat('def'), 'ghi')]", "ghi/abc/def")]
        [DataRow("[concat('abc', parameters('def'), 'ghi')]", "[concat('abc', parameters('def'), 'ghi')]")]
        [DataRow("[format('{0}/{1}/{2}', 'abc', concat('def', 'ghi'), parameters('jkl'))]", "[concat('abc/defghi/', parameters('jkl'))]")]
        public void FlattenStringOperations_flattens_expressions_correctly(string input, string expectedOutput)
        {
            var inputExpression = ExpressionHelpers.ParseExpression(input);
            var outputExpression = ExpressionHelpers.FlattenStringOperations(inputExpression);
            var output = ExpressionSerializer.SerializeExpression(outputExpression);

            output.Should().Be(expectedOutput);
        }

        [DataTestMethod]
        [DataRow("[resourceId('Microsoft.Sql/servers', variables('dbServerName'))]", "Microsoft.Sql/servers", "[variables('dbServerName')]")]
        [DataRow("[resourceId('Microsoft.Sql/servers/databases', variables('dbServerName'), variables('dbName'))]", "Microsoft.Sql/servers/databases", "[concat(variables('dbServerName'), '/', variables('dbName'))]")]
        [DataRow("[concat('Microsoft.Sql/servers/', variables('dbServerName'), '/databases/', variables('dbName'))]", "Microsoft.Sql/servers/databases", "[concat(variables('dbServerName'), '/', variables('dbName'))]")]
        [DataRow("[concat('Microsoft.Sql/servers', '/', variables('dbServerName'), '/', 'databases', '/', variables('dbName'))]", "Microsoft.Sql/servers/databases", "[concat(variables('dbServerName'), '/', variables('dbName'))]")]
        [DataRow("[concat('Microsoft.Sql/servers/', variables('dbServerName'))]", "Microsoft.Sql/servers", "[variables('dbServerName')]")]
        [DataRow("[resourceId('Microsoft.Compute/virtualMachines/extensions', 'BE0', 'BE0Setup')]", "Microsoft.Compute/virtualMachines/extensions", "[concat('BE0', '/', 'BE0Setup')]")]
        public void TryGetResourceNormalizedForm_returns_normalized_resource_expression(string input, string expectedTypeString, string expectedNameExpression)
        {
            var inputExpression = ExpressionHelpers.ParseExpression(input);
            var normalizedForm = ExpressionHelpers.TryGetResourceNormalizedForm(inputExpression);

            normalizedForm.Should().NotBeNull();
            var (typeString, nameExpression) = (normalizedForm!.Value);
            var nameExpressionString = ExpressionSerializer.SerializeExpression(nameExpression);

            typeString.Should().Be(expectedTypeString);
            nameExpressionString.Should().Be(expectedNameExpression);
        }
        
        [DataTestMethod]
        [DataRow("[concat('Microsoft.Network/networkSecurityGroups/', concat('nsg', variables('subnet0Name')))]")]
        [DataRow("[resourceId(parameters('vnetResourceGroupName'), 'Microsoft.Network/virtualNetworks', parameters('vnetResourceName'))]")]
        [DataRow("[variables('hostingPlanName')]")]
        public void TryGetResourceNormalizedForm_fails_to_normalize_more_unusual_expressions(string input)
        {
            var inputExpression = ExpressionHelpers.ParseExpression(input);
            var normalizedForm = ExpressionHelpers.TryGetResourceNormalizedForm(inputExpression);

            normalizedForm.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("[uri('test.com', 'path/to/file.json')]", "path/to/file.json")]
        [DataRow("[uri('test.com', 'path/to/file.json', parameters('sasUri'))]", "path/to/file.json")]
        [DataRow("[concat(uri('test.com', 'path/to/file.json'), parameters('sasUri'))]", "path/to/file.json")]
        [DataRow("[concat(parameters('myUri'), '/path/to/file.json')]", "path/to/file.json")]
        public void TryGetLocalFilePathForTemplateLink_finds_path_for_specific_expression_formats(string input, string expectedOutput)
        {
            var inputExpression = ExpressionHelpers.ParseExpression(input);
            var output = ExpressionHelpers.TryGetLocalFilePathForTemplateLink(inputExpression);

            output.Should().Be(expectedOutput);
        }

        [DataTestMethod]
        [DataRow("https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/modules/Microsoft.KeyVault/vaults/keys/0.9/azuredeploy.json")]
        [DataRow("[parameters('location')]")]
        [DataRow("[variables('networkSettings').subnet.dse]")]
        public void TryGetLocalFilePathForTemplateLink_fails_to_find_path_for_undecidable_expression(string input)
        {
            var inputExpression = ExpressionHelpers.ParseExpression(input);
            var output = ExpressionHelpers.TryGetLocalFilePathForTemplateLink(inputExpression);

            output.Should().BeNull();
        }
    }
}