// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using Bicep.Decompiler;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;

namespace Bicep.TextFixtures.Utils
{
    public class TestDecompiler
    {
        private readonly TestServices services;

        public TestDecompiler()
        {
            this.services = new();
            this.services.AddSingleton<IFileSystem, FileSystem>();
            this.services.AddSingleton<IFileExplorer, FileSystemFileExplorer>();
        }

        public TestDecompiler ConfigureServices(Action<TestServices> configure)
        {
            configure(this.services);

            return this;
        }

        public Task<DecompileResult> Decompile(IOUri bicepUri, string jsonContent, DecompileOptions? options = null) =>
            this.services.Get<BicepDecompiler>().Decompile(bicepUri, jsonContent, options);

        public DecompileResult DecompileParameters(string content, IOUri entryBicepparamUri, IOUri? bicepFileUri, DecompileParamOptions? options = null) =>
            this.services.Get<BicepDecompiler>().DecompileParameters(content, entryBicepparamUri, bicepFileUri, options);
    }
}
