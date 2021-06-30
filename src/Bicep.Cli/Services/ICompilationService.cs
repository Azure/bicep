// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using Bicep.Core.Semantics;

namespace Bicep.Cli.Services
{
    public interface ICompilationService
    {
        public Compilation Compile(string inputPath = "");
        public (Uri, ImmutableDictionary<Uri, string>) Decompile(string inputPath, string outputPath = "");
    }
}
