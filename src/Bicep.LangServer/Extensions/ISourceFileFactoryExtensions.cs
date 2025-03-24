// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.SourceGraph;

namespace Bicep.LanguageServer.Extensions
{
    public static class ISourceFileFactoryExtensions
    {
        public static BicepFile CreateDummyArtifactReferencingFile(this ISourceFileFactory sourceFileFactory) => sourceFileFactory.CreateBicepFile(new Uri("inmemory:///main.bicep"), "");
    }
}
