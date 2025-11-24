// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Providers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LangServer.UnitTests
{
    [TestClass]
    public class BicepCompilationProviderTests
    {
        [TestMethod]
        public void Create_ShouldReturnValidCompilation()
        {
            var fileUri = DocumentUri.From($"{DataSets.Parameters_LF.Name}.bicep");
            var services = ServiceBuilder.Create(services => services
                .WithDisabledAnalyzersConfiguration()
                .AddSingleton<BicepCompilationProvider>());

            var sourceFileFactory = services.Construct<ISourceFileFactory>();
            var sourceFile = sourceFileFactory.CreateBicepFile(fileUri.ToUriEncoded().ToIOUri(), DataSets.Parameters_LF.Bicep);
            var workspace = new ActiveSourceFileSet();
            workspace.UpsertSourceFile(sourceFile);

            var provider = services.Construct<BicepCompilationProvider>();
            var context = provider.Create(workspace, fileUri, []);

            context.Compilation.Should().NotBeNull();
            // TODO: remove Where when the support of modifiers is dropped.
            context.Compilation.GetEntrypointSemanticModel().GetAllDiagnostics().Should().BeEmpty();
            context.LineStarts.Should().NotBeEmpty();
            context.LineStarts[0].Should().Be(0);
        }
    }
}
