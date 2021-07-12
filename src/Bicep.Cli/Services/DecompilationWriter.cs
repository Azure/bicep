// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.IO;

namespace Bicep.Cli.Services
{
    public class DecompilationWriter
    {
        private readonly InvocationContext invocationContext;

        public DecompilationWriter(InvocationContext invocationContext)
        {
            this.invocationContext = invocationContext;
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
                invocationContext.OutputWriter.Write(bicepOutput);
            }

            return 0;
        }
    }
}
