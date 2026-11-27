// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Semantics.Namespaces
{
    [TestClass]
    public class ResourceIdFunctionTests
    {
        private const string Parameters = """
            param subscriptionId string
            param groupName string
            param parentName string
            param childName string
            param dynamicType string

            """;

        [TestMethod]
        [DataRow("resourceId('Microsoft.Network/virtualNetworks/subnets', parentName)", "Microsoft.Network/virtualNetworks/subnets", 2, 1)]
        [DataRow("az.resourceId('Microsoft.Network/virtualNetworks/subnets', parentName)", "Microsoft.Network/virtualNetworks/subnets", 2, 1)]
        [DataRow("resourceId(groupName, 'Microsoft.Network/virtualNetworks/subnets', parentName)", "Microsoft.Network/virtualNetworks/subnets", 2, 1)]
        [DataRow("resourceId(subscriptionId, groupName, 'Microsoft.Network/virtualNetworks/subnets', parentName)", "Microsoft.Network/virtualNetworks/subnets", 2, 1)]
        [DataRow("resourceId('myGroup', 'Microsoft.Network/virtualNetworks/subnets', parentName)", "Microsoft.Network/virtualNetworks/subnets", 2, 1)]
        [DataRow("resourceId(groupName, 'Microsoft.Storage/storageAccounts')", "Microsoft.Storage/storageAccounts", 1, 0)]
        [DataRow("resourceId(dynamicType, 'Microsoft.Network/virtualNetworks/subnets')", "Microsoft.Network/virtualNetworks/subnets", 2, 0)]
        [DataRow("resourceId('Microsoft.Storage/storageAccounts/blobServices/containers', parentName, childName)", "Microsoft.Storage/storageAccounts/blobServices/containers", 3, 2)]
        public void ResourceId_with_insufficient_names_reports_error(string expression, string resourceType, int required, int supplied)
        {
            var result = TestCompiler.ForInMemoryCompilation().CompileWithoutRestore($"{Parameters}\noutput testId string = {expression}");

            result.Diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP462", DiagnosticLevel.Error, $"The resource type \"{resourceType}\" requires {required} resource name segments, but the \"resourceId\" function was supplied with {supplied}."),
            });
        }

        [TestMethod]
        [DataRow("resourceId('Microsoft.Storage/storageAccounts', parentName)")]
        [DataRow("resourceId('Microsoft.Network/virtualNetworks/subnets', parentName, childName)")]
        [DataRow("resourceId(groupName, 'Microsoft.Network/virtualNetworks/subnets', parentName, childName)")]
        [DataRow("resourceId(subscriptionId, 'Microsoft.Network/virtualNetworks/subnets', parentName, childName)")]
        [DataRow("resourceId(subscriptionId, groupName, 'Microsoft.Network/virtualNetworks/subnets', parentName, childName)")]
        [DataRow("resourceId('Microsoft.Storage/storageAccounts/blobServices/containers', parentName, 'default', childName)")]
        [DataRow("resourceId(dynamicType, parentName)")]
        [DataRow("resourceId(groupName, dynamicType, parentName)")]
        [DataRow("resourceId(subscriptionId, groupName, dynamicType, parentName)")]
        public void ResourceId_with_sufficient_names_or_unknown_type_compiles(string expression)
        {
            var result = TestCompiler.ForInMemoryCompilation().CompileWithoutRestore($"{Parameters}\noutput testId string = {expression}");

            result.Diagnostics.ExcludingLinterDiagnostics().Should().BeEmpty();
        }

        [TestMethod]
        [DataRow("resourceId('Microsoft.Network/virtualNetworks/subnets', 'parent/child')")]
        [DataRow("resourceId('not/a/provider', 'Microsoft.Network/virtualNetworks/subnets', parentName)")]
        [DataRow("resourceId('Microsoft.Network/virtualNetworks/subnets@2024-01-01', parentName)")]
        [DataRow("resourceId('Microsoft.Network/virtualNetworks/subnets/', parentName)")]
        [DataRow("resourceId(subscriptionId, groupName, parentName, 'Microsoft.Network/virtualNetworks/subnets', childName)")]
        [DataRow("resourceId('Microsoft.Storage/storageAccounts', parentName, childName)")]
        public void ResourceId_defers_other_runtime_validation_to_arm(string expression)
        {
            var result = TestCompiler.ForInMemoryCompilation().CompileWithoutRestore($"{Parameters}\noutput testId string = {expression}");

            result.Diagnostics.ExcludingLinterDiagnostics().Should().BeEmpty();
        }
    }
}
