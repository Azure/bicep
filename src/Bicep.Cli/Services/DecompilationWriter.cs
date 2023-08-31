// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Decompiler;
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

        public int ToFile(DecompileResult decompilation)
        {
            foreach (var (fileUri, bicepOutput) in decompilation.FilesToSave)
            {
                File.WriteAllText(fileUri.LocalPath, bicepOutput);
            }

            return 0;
        }

        public int ToStdout(DecompileResult decompilation)
        {
            foreach (var (_, bicepOutput) in decompilation.FilesToSave)
            {
                io.Output.Write(bicepOutput);
            }

            return 0;
        }
    }
}
