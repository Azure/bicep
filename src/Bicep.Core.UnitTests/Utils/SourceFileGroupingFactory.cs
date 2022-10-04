// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Workspaces;

namespace Bicep.Core.UnitTests.Utils
{
    public static class SourceFileGroupingFactory
    {
        private static ServiceBuilder Services => new ServiceBuilder().WithTestDefaults();

        public static SourceFileGrouping CreateFromText(string text, IFileResolver fileResolver)
        {
            var entryFileUri = new Uri("file:///main.bicep");

            return CreateForFiles(new Dictionary<Uri, string> { [entryFileUri] = text }, entryFileUri, fileResolver, BicepTestConstants.BuiltInConfigurationWithAllAnalyzersDisabled);
        }

        public static SourceFileGrouping CreateForFiles(IReadOnlyDictionary<Uri, string> fileContentsByUri, Uri entryFileUri, IFileResolver fileResolver, RootConfiguration configuration, IFeatureProvider? features = null)
            => CreateForFiles(fileContentsByUri, entryFileUri, fileResolver, IConfigurationManager.WithStaticConfiguration(configuration), features);

        public static SourceFileGrouping CreateForFiles(IReadOnlyDictionary<Uri, string> fileContentsByUri, Uri entryFileUri, IFileResolver fileResolver, IConfigurationManager configurationManager, IFeatureProvider? features = null)
        {
            return Services
                .WithWorkspaceFiles(fileContentsByUri)
                .WithFileResolver(fileResolver)
                .WithConfigurationManager(configurationManager)
                .WithFeatureProvider(features ?? BicepTestConstants.Features)
                .SourceFileGrouping.Build(entryFileUri);
        }
    }
}
