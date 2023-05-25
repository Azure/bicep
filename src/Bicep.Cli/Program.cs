// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Commands;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.Core.Emit;
using Bicep.Core.Exceptions;
using Bicep.Core.Features;
using Bicep.Core.Tracing;
using Bicep.Core.Utils;
using Bicep.Decompiler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime;
using System.Threading.Tasks;

namespace Bicep.Cli
{
    public record IOContext(
        TextWriter Output,
        TextWriter Error);

    public class Program
    {
        private readonly IServiceProvider services;
        private readonly IOContext io;

        public Program(IOContext io, Action<IServiceCollection>? registerAction = null)
        {
            var services = ConfigureServices(io);
            registerAction?.Invoke(services);
            this.services = services.BuildServiceProvider();
            this.io = io;
        }

        public static async Task<int> Main(string[] args)
        {
            string profilePath = DirHelper.GetTempPath();
            ProfileOptimization.SetProfileRoot(profilePath);
            ProfileOptimization.StartProfile("bicep.profile");
            Console.OutputEncoding = TemplateEmitter.UTF8EncodingWithoutBom;

            if (FeatureProvider.TracingEnabled)
            {
                Trace.Listeners.Add(new TextWriterTraceListener(Console.Error));
            }

            // this event listener picks up SDK events and writes them to Trace.WriteLine()
            using (FeatureProvider.TracingEnabled ? AzureEventSourceListenerFactory.Create(FeatureProvider.TracingVerbosity) : null)
            {
                var program = new Program(new(Output: Console.Out, Error: Console.Error));

                // this must be awaited so dispose of the listener occurs in the continuation
                // rather than the sync part at the beginning of RunAsync()
                return await program.RunAsync(args);
            }
        }

        public async Task<int> RunAsync(string[] args)
        {
            try
            {
                switch (ArgumentParser.TryParse(args))
                {
                    case BuildArguments buildArguments when buildArguments.CommandName == Constants.Command.Build: // bicep build [options]
                        return await services.GetRequiredService<BuildCommand>().RunAsync(buildArguments);

                    case BuildParamsArguments buildParamsArguments when buildParamsArguments.CommandName == Constants.Command.BuildParams: // bicep build-params [options]
                        return await services.GetRequiredService<BuildParamsCommand>().RunAsync(buildParamsArguments);

                    case FormatArguments formatArguments when formatArguments.CommandName == Constants.Command.Format: // bicep format [options]
                        return services.GetRequiredService<FormatCommand>().Run(formatArguments);

                    case GenerateParametersFileArguments generateParametersFileArguments when generateParametersFileArguments.CommandName == Constants.Command.GenerateParamsFile: // bicep generate-params [options]
                        return await services.GetRequiredService<GenerateParametersFileCommand>().RunAsync(generateParametersFileArguments);

                    case DecompileArguments decompileArguments when decompileArguments.CommandName == Constants.Command.Decompile: // bicep decompile [options]
                        return await services.GetRequiredService<DecompileCommand>().RunAsync(decompileArguments);

                    case PublishArguments publishArguments when publishArguments.CommandName == Constants.Command.Publish: // bicep publish [options]
                        return await services.GetRequiredService<PublishCommand>().RunAsync(publishArguments);

                    case RestoreArguments restoreArguments when restoreArguments.CommandName == Constants.Command.Restore: // bicep restore
                        return await services.GetRequiredService<RestoreCommand>().RunAsync(restoreArguments);

                    case LintArguments lintArguments when lintArguments.CommandName == Constants.Command.Lint: // bicep lint [options]
                        return await services.GetRequiredService<LintCommand>().RunAsync(lintArguments);

                    case RootArguments rootArguments when rootArguments.CommandName == Constants.Command.Root: // bicep [options]
                        return services.GetRequiredService<RootCommand>().Run(rootArguments);

                    default:
                        io.Error.WriteLine(string.Format(CliResources.UnrecognizedArgumentsFormat, string.Join(' ', args), ThisAssembly.AssemblyName)); // should probably print help here??
                        return 1;
                }
            }
            catch (BicepException exception)
            {
                io.Error.WriteLine(exception.Message);
                return 1;
            }
        }

        private static ILoggerFactory CreateLoggerFactory(IOContext io)
        {
            // apparently logging requires a factory factory 🤦‍
            return LoggerFactory.Create(builder =>
            {
                builder.AddProvider(new BicepLoggerProvider(new BicepLoggerOptions(true, ConsoleColor.Red, ConsoleColor.DarkYellow, io.Error)));
            });
        }

        private static IServiceCollection ConfigureServices(IOContext io)
            => new ServiceCollection()
                .AddBicepCore()
                .AddBicepDecompiler()
                .AddCommands()
                .AddSingleton(CreateLoggerFactory(io).CreateLogger("bicep"))
                .AddSingleton<IDiagnosticLogger, BicepDiagnosticLogger>()
                .AddSingleton<DecompilationWriter>()
                .AddSingleton<CompilationWriter>()
                .AddSingleton<PlaceholderParametersWriter>()
                .AddSingleton<CompilationService>()
                .AddSingleton(io);
    }
}
