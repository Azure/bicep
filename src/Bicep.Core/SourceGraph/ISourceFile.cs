// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.IO.Abstraction;

namespace Bicep.Core.SourceGraph
{
    public interface ISourceFile
    {
        // CONSIDER: Uri Uri => this.FileHandle.Uri.ToUri();
        Uri Uri { get; }

        IFileHandle FileHandle { get; }

        string Text { get; }

        RootConfiguration Configuration { get; }

        IFeatureProvider Features { get; }

        BicepSourceFileKind FileKind { get; }
    }
}
