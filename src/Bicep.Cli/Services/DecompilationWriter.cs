// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.IO;

namespace Bicep.Cli.Services
{
    public class DecompilationWriter
    {
        private readonly IOContext io;

        public DecompilationWriter(IOContext io)
        {
            this.io = io;
        }

        public int ToFile((Uri, ImmutableDictionary<Uri, string>) decompilation)
        {

            var (_, filesToSave) = decompilation;

            foreach (var (fileUri, bicepOutput) in filesToSave)
            {
                File.WriteAllText(fileUri.LocalPath, bicepOutput);
            }

            return 0;
        }

        public int ToStdout((Uri, ImmutableDictionary<Uri, string>) decompilation)
        {
            var (_, filesToSave) = decompilation;

            foreach (var (_, bicepOutput) in filesToSave)
            {
                io.Output.Write(bicepOutput);
            }

            return 0;
        }
    }
}
