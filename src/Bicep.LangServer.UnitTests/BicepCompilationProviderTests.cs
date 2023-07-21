// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.TestFiles;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
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
        private static BicepCompilationProvider Create()
        {
            var helper = ServiceBuilder.Create(services => services
                .WithDisabledAnalyzersConfiguration()
                .AddSingleton<BicepCompilationProvider>());

            return helper.Construct<BicepCompilationProvider>();
        }

        [TestMethod]
        public void Create_ShouldReturnValidCompilation()
        {
            var fileUri = DocumentUri.Parse($"/{DataSets.Parameters_LF.Name}.bicep");
            var provider = Create();

            var sourceFile = SourceFileFactory.CreateSourceFile(fileUri.ToUri(), DataSets.Parameters_LF.Bicep);
            var workspace = new Workspace();
            workspace.UpsertSourceFile(sourceFile);

            var context = provider.Create(workspace, fileUri, ImmutableDictionary<ISourceFile, ISemanticModel>.Empty);

            context.Compilation.Should().NotBeNull();
            // TODO: remove Where when the support of modifiers is dropped.
            context.Compilation.GetEntrypointSemanticModel().GetAllDiagnostics().Should().BeEmpty();
            context.LineStarts.Should().NotBeEmpty();
            context.LineStarts[0].Should().Be(0);
        }
    }
}
