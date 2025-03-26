// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.SourceGraph;

namespace Bicep.LanguageServer.CompilationManager
{
    /// <summary>
    /// Base class for both successful and unsafe compilation contexts.
    /// It allows us to propagate source file kind for untitled files
    /// in cases of fatal errors for better recovery experience in the
    /// editor.
    /// </summary>
    public abstract class CompilationContextBase
    {
        protected CompilationContextBase(BicepSourceFileKind? sourceFileKind)
        {
            SourceFileKind = sourceFileKind;
        }

        public BicepSourceFileKind? SourceFileKind { get; }
    }
}
