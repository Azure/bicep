// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using Bicep.Core.Semantics;

namespace Bicep.Cli.Services
{
    public interface IWriter : IService
    {
        public void WriteCompilation(Compilation compilation);
        public void WriteDecompilation((Uri, ImmutableDictionary<Uri, string>) decompilation);
    }

    public interface IService
    {
    }
}