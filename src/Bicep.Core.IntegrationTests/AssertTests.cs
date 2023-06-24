// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class AssertTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private ServiceBuilder ServicesWithAsserts => new ServiceBuilder()
    .WithFeatureOverrides(new(TestContext, AssertsEnabled: true));


        [TestMethod]
        public void Asserts_are_disabled_unless_feature_is_enabled()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new());

            var result = CompilationHelper.Compile(services, @"
                assert a1 = true
            ");

            result.Should().HaveDiagnostics(new[] {
                ("BCP345", DiagnosticLevel.Error, "Using an assert declaration requires enabling EXPERIMENTAL feature \"Asserts\".")
            });

            result = CompilationHelper.Compile(ServicesWithAsserts, @"
                assert a1 = true
            ");

            result.Should().NotHaveAnyDiagnostics();
        }
    }
}
