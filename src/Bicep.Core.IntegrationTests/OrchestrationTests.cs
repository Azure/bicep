// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Extensions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using Bicep.TextFixtures.Assertions;
using Bicep.TextFixtures.IO;
using Bicep.TextFixtures.Mocks;
using Bicep.TextFixtures.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class OrchestrationTests
{
    private static ServiceBuilder Services => new ServiceBuilder()
        .WithEmptyAzResources()
        .WithFeatureOverrides(new(OrchestrationEnabled: true));

    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public void Basic_example_can_be_compiled_successfully()
    {
        var result = CompilationHelper.Compile(Services,
            ("main.bicep", """
                    targetScope = 'orchestrator'

                    component foo './global.bicepparam' = {
                    }
                    """),
            ("global.bicepparam", """
                    using './global.bicep'
                    """),
            ("global.bicep", """

                    """));

        result.Should().NotHaveAnyDiagnostics();
    }
}