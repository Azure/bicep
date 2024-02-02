// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bicep.Core.UnitTests.Assertions;


namespace Bicep.Core.IntegrationTests;

[TestClass]
public class CentralizedProviderVersionManagementTests : TestBase
{
    private ServiceBuilder Services => new ServiceBuilder()
            .WithFeatureOverrides(new(ExtensibilityEnabled: true, DynamicTypeLoadingEnabled: true));

    [TestMethod]
    public void ProvidersConfig_SupportForConfigManagedProviderDeclarationSyntax()
    {
        var result = CompilationHelper.Compile(Services, @"
        provider sys
        ");

        result.Should().NotHaveAnyDiagnostics();
    }


}
