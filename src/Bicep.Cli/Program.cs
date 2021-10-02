// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Commands;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.Configuration;
using Bicep.Core.Emit;
using Bicep.Core.Exceptions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Tracing;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.Utils;
using Bicep.Decompiler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Runtime;
using System.Threading.Tasks;

namespace Bicep.Cli
{
    public class Program
    {
        private readonly InvocationContext invocationContext;

        public Program(InvocationContext invocationContext)
        {
            this.invocationContext = invocationContext;
        }

        public static async Task<int> Main(string[] args)
        {
            string profilePath = DirHelper.GetTempPath();
            ProfileOptimization.SetProfileRoot(profilePath);
            ProfileOptimization.StartProfile("bicep.profile");
            Console.OutputEncoding = TemplateEmitter.UTF8EncodingWithoutBom;

            BicepDeploymentsInterop.Initialize();

            if (FeatureProvider.TracingEnabled)
            {
                Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            }

            // this event listener picks up SDK events and writes them to Trace.WriteLine()
            using(FeatureProvider.TracingEnabled ? AzureEventSourceListenerFactory.Create(FeatureProvider.TracingVerbosity) : null)
            {
                var program = new Program(new InvocationContext(
                    new AzResourceTypeLoader(),
                    Console.Out,
                    Console.Error,
                    features: null,
                    clientFactory: null));

                // this must be awaited so dispose of the listener occurs in the continuation
                // rather than the sync part at the beginning of RunAsync()
                return await program.RunAsync(args);
            }
        }

        public async Task<int> RunAsync(string[] args)
        {
            var serviceProvider = ConfigureServices();

            try
            {
                switch (ArgumentParser.TryParse(args))
                {
                    case BuildArguments buildArguments when buildArguments.CommandName == Constants.Command.Build: // bicep build [options]
                        return await serviceProvider.GetRequiredService<BuildCommand>().RunAsync(buildArguments);

                    case DecompileArguments decompileArguments when decompileArguments.CommandName == Constants.Command.Decompile: // bicep decompile [options]
                        return await serviceProvider.GetRequiredService<DecompileCommand>().RunAsync(decompileArguments);

                    case PublishArguments publishArguments when publishArguments.CommandName == Constants.Command.Publish: // bicep publish [options]
                        return await serviceProvider.GetRequiredService<PublishCommand>().RunAsync(publishArguments);

                    case RestoreArguments restoreArguments when restoreArguments.CommandName == Constants.Command.Restore: // bicep restore
                        return await serviceProvider.GetRequiredService<RestoreCommand>().RunAsync(restoreArguments);

                    case RootArguments rootArguments when rootArguments.CommandName == Constants.Command.Root: // bicep [options]
                        return serviceProvider.GetRequiredService<RootCommand>().Run(rootArguments);

                    default:
                        invocationContext.ErrorWriter.WriteLine(string.Format(CliResources.UnrecognizedArgumentsFormat, string.Join(' ', args), ThisAssembly.AssemblyName)); // should probably print help here??
                        return 1;
                }
            }
            catch (BicepException exception)
            {
                invocationContext.ErrorWriter.WriteLine(exception.Message);
                return 1;
            }
        }

        private ILoggerFactory CreateLoggerFactory()
        {
            // apparently logging requires a factory factory 🤦‍
            return LoggerFactory.Create(builder =>
            {
                builder.AddProvider(new BicepLoggerProvider(new BicepLoggerOptions(true, ConsoleColor.Red, ConsoleColor.DarkYellow, this.invocationContext.ErrorWriter)));
            });
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddCommands()
                .AddInvocationContext(invocationContext)
                .AddSingleton(CreateLoggerFactory().CreateLogger("bicep"))
                .AddSingleton<IDiagnosticLogger, BicepDiagnosticLogger>()
                .AddSingleton<IFileResolver, FileResolver>()
                .AddSingleton<IModuleDispatcher, ModuleDispatcher>()
                .AddSingleton<IModuleRegistryProvider, DefaultModuleRegistryProvider>()
                .AddSingleton<IFileSystem, FileSystem>()
                .AddSingleton<IConfigurationManager, ConfigurationManager>()
                .AddSingleton<TemplateDecompiler>()
                .AddSingleton<DecompilationWriter>()
                .AddSingleton<CompilationWriter>()
                .AddSingleton<CompilationService>()
                .BuildServiceProvider();
        }
    }
}
