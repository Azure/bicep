// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Commands;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime;

namespace Bicep.Cli
{
    public class Program
    {
        private readonly InvocationContext invocationContext;

        public Program(InvocationContext invocationContext)
        {
            this.invocationContext = invocationContext;
        }

        public static int Main(string[] args)
        {            
            string profilePath = MulticoreJIT.GetMulticoreJITPath();
            ProfileOptimization.SetProfileRoot(profilePath);
            ProfileOptimization.StartProfile("bicep.profile");
            Console.OutputEncoding = TemplateEmitter.UTF8EncodingWithoutBom;

            BicepDeploymentsInterop.Initialize();

            var program = new Program(new InvocationContext(AzResourceTypeProvider.CreateWithAzTypes(), Console.Out, Console.Error, ThisAssembly.AssemblyFileVersion));

            return program.Run(args);
        }

        public int Run(string[] args)
        {
            ServiceProvider serviceProvider = ConfigureServices();

            try
            {
                switch (ArgumentParser.TryParse(args))
                {
                    case BuildArguments buildArguments when buildArguments.CommandName == Constants.Command.Build: // bicep build [options]
                        return serviceProvider.GetRequiredService<BuildCommand>().Run(buildArguments);

                    case DecompileArguments decompileArguments when decompileArguments.CommandName == Constants.Command.Decompile: // bicep decompile [options]
                        return serviceProvider.GetRequiredService<DecompileCommand>().Run(decompileArguments);

                    case RootArguments rootArguments when rootArguments.CommandName == Constants.Command.Root: // bicep [options]
                        return serviceProvider.GetRequiredService<RootCommand>().Run(rootArguments);

                    default:
                        invocationContext.ErrorWriter.WriteLine(string.Format(CliResources.UnrecognizedArgumentsFormat, string.Join(' ', args), ThisAssembly.AssemblyName)); // should probably print help here??
                        return 1;
                }
            }
            catch (CommandLineException exception)
            {
                invocationContext.ErrorWriter.WriteLine(exception.Message);
                return 1;
            }
            catch (BicepException exception)
            {
                invocationContext.ErrorWriter.WriteLine(exception.Message);
                return 1;
            }
            catch (ErrorDiagnosticException exception)
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
                // Adds commands to the DI container
                .AddCommands()

                // Adds the ILogger and IDiagnosticlogger
                .AddSingleton(CreateLoggerFactory().CreateLogger("bicep"))
                .AddSingleton<IDiagnosticLogger, BicepDiagnosticLogger>()

                // Handles the context of this invocation
                .AddSingleton(invocationContext)

                // Adds the various services that
                .AddSingleton<DecompilationWriter>()
                .AddSingleton<CompilationWriter>()
                .AddSingleton<CompilationService>()
                .BuildServiceProvider();
        }
    }
}
