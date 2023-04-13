// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class TypeAssignmentVisitorTests
    {
        private static ServiceBuilder Services => new ServiceBuilder();

        private static SemanticModel GetSemanticModelForTest(string programText)
        {
            var compilation = Services
                .WithConfigurationPatch(c => c.WithAllAnalyzersDisabled())
                .BuildCompilation(programText);

            return compilation.GetEntrypointSemanticModel();
        }

        [TestMethod]
        public void Negative_array_index_access_emits_error_diagnostic() 
        {
            var template = @"
param anArray array

output lastElement int = anArray[-1]
            ";

            var model = GetSemanticModelForTest(template);
            model.GetAllDiagnostics().Should().SatisfyRespectively(
                x => x.Should().HaveCodeAndSeverity("BCP339", DiagnosticLevel.Error)
                    .And.HaveMessage("""The provided array index value of "-1" is not valid. Array index should be greater than or equal to 0.""")
            );
        }
    }
}