// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.Providers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LangServer.UnitTests
{
    [TestClass]
    public class BicepCompilationProviderTests
    {
        private static IFileResolver CreateFileResolver(Uri fileUri, string contents)
            => new InMemoryFileResolver(new Dictionary<Uri, string>
            {
                [fileUri] = contents,
            });

        [TestMethod]
        public void Create_ShouldReturnValidCompilation()
        {
            var fileUri = DocumentUri.Parse($"/{DataSets.Parameters_LF.Name}.bicep");
            var fileResolver = CreateFileResolver(fileUri.ToUri(), DataSets.Parameters_LF.Bicep);
            var dispatcher = new ModuleDispatcher(new DefaultModuleRegistryProvider(fileResolver, BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory, BicepTestConstants.Features));

            var provider = new BicepCompilationProvider(TestTypeHelper.CreateEmptyProvider(), fileResolver, dispatcher);

            var sourceFile = SourceFileFactory.CreateSourceFile(fileUri.ToUri(), DataSets.Parameters_LF.Bicep);
            var workspace = new Workspace();
            workspace.UpsertSourceFile(sourceFile);
            var configuration = BicepTestConstants.BuiltInConfigurationWithAnalyzersDisabled;
            var context = provider.Create(workspace, fileUri, ImmutableDictionary<ISourceFile, ISemanticModel>.Empty, configuration);

            context.Compilation.Should().NotBeNull();
            // TODO: remove Where when the support of modifiers is dropped.
            context.Compilation.GetEntrypointSemanticModel().GetAllDiagnostics().Should().BeEmpty();
            context.LineStarts.Should().NotBeEmpty();
            context.LineStarts[0].Should().Be(0);
        }
    }
}

