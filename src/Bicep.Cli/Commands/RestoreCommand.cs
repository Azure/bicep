// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Services;
using Bicep.Core.FileSystem;
using System.Threading.Tasks;

namespace Bicep.Cli.Commands
{
    public class RestoreCommand : ICommand
    {
        private readonly CompilationService compilationService;

        public RestoreCommand(CompilationService compilationService)
        {
            this.compilationService = compilationService;
        }

        public async Task<int> RunAsync(RestoreArguments args)
        {
            var inputPath = PathHelper.ResolvePath(args.InputFile);
            await this.compilationService.RestoreAsync(inputPath);

            return 0;
        }
    }
}
