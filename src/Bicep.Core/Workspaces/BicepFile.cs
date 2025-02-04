// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.Syntax;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Workspaces
{
    public class BicepFile : BicepSourceFile
    {
        public readonly static BicepFile Dummy = new(
            new Uri($"file:///dummy-{Guid.NewGuid()}.bicep"),
            [],
            SyntaxFactory.EmptyProgram,
            DummyConfigurationManager.Instance,
            DummyFeatureProviderFactory.Instance,
            EmptyDiagnosticLookup.Instance,
            EmptyDiagnosticLookup.Instance);

        public BicepFile(
            Uri fileUri,
            ImmutableArray<int> lineStarts,
            ProgramSyntax programSyntax,
            IConfigurationManager configurationManager,
            IFeatureProviderFactory featureProviderFactory,
            IDiagnosticLookup lexingErrorLookup,
            IDiagnosticLookup parsingErrorLookup)
            : base(
                  fileUri,
                  lineStarts,
                  programSyntax,
                  configurationManager,
                  featureProviderFactory,
                  lexingErrorLookup,
                  parsingErrorLookup)
        {
        }

        private BicepFile(BicepFile original) : base(original)
        {
        }

        public override BicepSourceFileKind FileKind => BicepSourceFileKind.BicepFile;

        public override BicepSourceFile ShallowClone() => new BicepFile(this);


        private class DummyConfigurationManager : IConfigurationManager
        {
            public static readonly DummyConfigurationManager Instance = new();

            public RootConfiguration GetConfiguration(Uri sourceFileUri) => IConfigurationManager.GetBuiltInConfiguration();
        }

        private class DummyFileExplorer : IFileExplorer
        {
            public static readonly DummyFileExplorer Instance = new();

            public IDirectoryHandle GetDirectory(IOUri uri) => throw new NotImplementedException();

            public IFileHandle GetFile(IOUri uri) => throw new NotImplementedException();
        }

        private class DummyFeatureProviderFactory : IFeatureProviderFactory
        {
            public static readonly DummyFeatureProviderFactory Instance = new();

            public IFeatureProvider GetFeatureProvider(Uri fileUri) => new FeatureProvider(
                IConfigurationManager.GetBuiltInConfiguration(),
                DummyFileExplorer.Instance);
        }
    }
}
