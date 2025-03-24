// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.SourceGraph;

namespace Bicep.LanguageServer.CompilationManager
{
    /// <summary>
    /// Represents a compilation context that failed to produce a compilation
    /// due to an unhandled exception. This only happens due a code bug, but
    /// we handle it in a special way to allow for better recovery of the user
    /// experience as the user is typing. (As rare as they are, fatal exceptions
    /// are typically due to a specific combination of characters in the source file.)
    /// </summary>
    public class UnsafeCompilationContext : CompilationContextBase
    {
        public UnsafeCompilationContext(Exception exception, BicepSourceFileKind? sourceFileKind) : base(sourceFileKind)
        {
            this.Exception = exception;
        }

        public Exception Exception { get; }
    }
}
