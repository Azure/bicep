// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;
using System.Linq;
using Bicep.Cli.CommandLine;
using Bicep.Cli.CommandLine.Arguments;
using Bicep.Cli.Logging;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.Workspaces;
using Bicep.Decompiler;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Bicep.Cli
{
    public class Program
    {
        private readonly TextWriter outputWriter;
        private readonly TextWriter errorWriter;
        private readonly IResourceTypeProvider resourceTypeProvider;

        private readonly string assemblyFileVersion;

        public Program(IResourceTypeProvider resourceTypeProvider, TextWriter outputWriter, TextWriter errorWriter, string assemblyFileVersion)
        {
            this.resourceTypeProvider = resourceTypeProvider;
            this.outputWriter = outputWriter;
            this.errorWriter = errorWriter;
            this.assemblyFileVersion = assemblyFileVersion;
        }

        public static int Main(string[] args)
        {
            BicepDeploymentsInterop.Initialize();
            var program = new Program(new AzResourceTypeProvider(), Console.Out, Console.Error, ThisAssembly.AssemblyFileVersion);
            return program.Run(args);
        }

        public int Run(string[] args)
        {
            // ReSharper disable once ConvertToUsingDeclaration
            using (var loggerFactory = CreateLoggerFactory())
            {
                // the only value in using the dotnet logging framework is that we can easily implement filters
                // and logging to multiple targets in the future (stdout AND a log file, for example)
                // it does not help us with formatting of the messages however, so we will have to workaround that
                var logger = loggerFactory.CreateLogger("bicep");
                try
                {
                    switch (ArgumentParser.TryParse(args))
                    {
                        case BuildArguments buildArguments: // build
                            return Build(logger, buildArguments);
                        case DecompileArguments decompileArguments:
                            return Decompile(logger, decompileArguments);
                        case NewArguments newArguments:
                            return CommandNew(logger, newArguments, this.outputWriter);
                        case VersionArguments _: // --version
                            ArgumentParser.PrintVersion(this.outputWriter);
                            return 0;
                        case HelpArguments _: // --help
                            ArgumentParser.PrintUsage(this.outputWriter);
                            return 0;
                        default:
                            var exeName = ArgumentParser.GetExeName();
                            var arguments = string.Join(' ', args);
                            this.errorWriter.WriteLine($"Unrecognized arguments '{arguments}' specified. Use '{exeName} --help' to view available options.");
                            return 1;
                    }
                }
                catch (CommandLineException exception)
                {
                    this.errorWriter.WriteLine(exception.Message);
                    return 1;
                }
                catch (BicepException exception)
                {
                    this.errorWriter.WriteLine(exception.Message);
                    return 1;
                }
                catch (ErrorDiagnosticException exception)
                {
                    this.errorWriter.WriteLine(exception.Message);
                    return 1;
                }
            }
        }

        private ILoggerFactory CreateLoggerFactory()
        {
            // apparently logging requires a factory factory 🤦‍
            return LoggerFactory.Create(builder =>
            {
                builder.AddProvider(new BicepLoggerProvider(new BicepLoggerOptions(true, ConsoleColor.Red, ConsoleColor.DarkYellow, this.errorWriter)));
            });
        }

        private int Build(ILogger logger, BuildArguments arguments)
        {
            var diagnosticLogger = new BicepDiagnosticLogger(logger);
            var bicepPath = PathHelper.ResolvePath(arguments.InputFile);

            if (arguments.OutputToStdOut)
            {
                BuildToStdout(diagnosticLogger, bicepPath);
            }
            else if (arguments.OutputDir is not null)
            {
                var outputDir = PathHelper.ResolvePath(arguments.OutputDir);
                if (!Directory.Exists(outputDir))
                {
                    throw new CommandLineException($"The specified output directory \"{outputDir}\" does not exist.");
                }

                var outputPath = Path.Combine(outputDir, Path.GetFileName(bicepPath));

                BuildToFile(diagnosticLogger, bicepPath, PathHelper.GetDefaultOutputPath(outputPath));
            }
            else if (arguments.OutputFile is not null)
            {
                BuildToFile(diagnosticLogger, bicepPath, arguments.OutputFile);
            }
            else
            {
                BuildToFile(diagnosticLogger, bicepPath, PathHelper.GetDefaultOutputPath(bicepPath));
            }

            // return non-zero exit code on errors
            return diagnosticLogger.HasLoggedErrors ? 1 : 0;
        }

        private static bool LogDiagnosticsAndCheckSuccess(IDiagnosticLogger logger, Compilation compilation)
        {
            var success = true;
            foreach (var (syntaxTree, diagnostics) in compilation.GetAllDiagnosticsBySyntaxTree())
            {
                foreach (var diagnostic in diagnostics)
                {
                    logger.LogDiagnostic(syntaxTree.FileUri, diagnostic, syntaxTree.LineStarts);
                    success &= diagnostic.Level != DiagnosticLevel.Error;
                }
            }

            return success;
        }

        private void BuildToFile(IDiagnosticLogger logger, string bicepPath, string outputPath)
        {
            var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(new FileResolver(), new Workspace(), PathHelper.FilePathToFileUrl(bicepPath));
            var compilation = new Compilation(resourceTypeProvider, syntaxTreeGrouping);

            var success = LogDiagnosticsAndCheckSuccess(logger, compilation);
            if (success)
            {
                var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel(), this.assemblyFileVersion);

                using var outputStream = CreateFileStream(outputPath);
                emitter.Emit(outputStream);
            }
        }

        private void BuildToStdout(IDiagnosticLogger logger, string bicepPath)
        {
            using var writer = new JsonTextWriter(this.outputWriter)
            {
                Formatting = Formatting.Indented
            };

            var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(new FileResolver(), new Workspace(), PathHelper.FilePathToFileUrl(bicepPath));
            var compilation = new Compilation(resourceTypeProvider, syntaxTreeGrouping);

            var success = LogDiagnosticsAndCheckSuccess(logger, compilation);
            if (success)
            {
                var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel(), this.assemblyFileVersion);

                emitter.Emit(writer);
            }
        }

        private static FileStream CreateFileStream(string path)
        {
            try
            {
                return new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            }
            catch (Exception exception)
            {
                throw new BicepException(exception.Message, exception);
            }
        }

        public int Decompile(ILogger logger, DecompileArguments arguments)
        {
            logger.LogWarning(
                "WARNING: Decompilation is a best-effort process, as there is no guaranteed mapping from ARM JSON to Bicep.\n" +
                "You may need to fix warnings and errors in the generated bicep file(s), or decompilation may fail entirely if an accurate conversion is not possible.\n" +
                "If you would like to report any issues or inaccurate conversions, please see https://github.com/Azure/bicep/issues.");

            var diagnosticLogger = new BicepDiagnosticLogger(logger);
            var jsonPath = PathHelper.ResolvePath(arguments.InputFile);

            try
            {
                var (bicepUri, filesToSave) = TemplateDecompiler.DecompileFileWithModules(resourceTypeProvider, new FileResolver(), PathHelper.FilePathToFileUrl(jsonPath));
                foreach (var (fileUri, bicepOutput) in filesToSave)
                {
                    File.WriteAllText(fileUri.LocalPath, bicepOutput);
                }

                var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(new FileResolver(), new Workspace(), bicepUri);
                var compilation = new Compilation(resourceTypeProvider, syntaxTreeGrouping);

                return LogDiagnosticsAndCheckSuccess(diagnosticLogger, compilation) ? 0 : 1;
            }
            catch (Exception exception)
            {
                this.errorWriter.WriteLine($"{jsonPath}: Decompilation failed with fatal error \"{exception.Message}\"");
                return 1;
            }
        }
 
        public int CommandNew(ILogger logger, NewArguments arguments, TextWriter writer)
        {

            var diagnosticLogger = new BicepDiagnosticLogger(logger);
            
            const string defaultRepoUri = "https://github.com/Azure/bicep/raw/main/docs/examples/index.json";
            //const string defaultFileName = "main.bicep";

            // TODO: null URIs to arguments
            string repoUri = arguments.IsCustomRepository ? arguments.Repository ??"" : defaultRepoUri; //fix it

            if (!Uri.IsWellFormedUriString(repoUri, UriKind.RelativeOrAbsolute))
            {
                throw new CommandLineException($"The specified repository path is invalid.");
            }

            if (arguments.Template == null)
            {
                return CommandNewListTemplates(writer, repoUri);
            }
            else
            {
                var outputDir = arguments.OutputDir ?? Path.GetDirectoryName(arguments.OutputFile);
                if (outputDir != null && !Directory.Exists(outputDir))
                {
                    throw new CommandLineException($"The specified output directory \"{outputDir}\" does not exist.");
                }

                return CommandNewGetTemplate(writer, repoUri, arguments.Template, outputDir, arguments.OutputFile, arguments.OutputToStdOut);
            }
        }

        private int CommandNewGetTemplate(TextWriter writer, string repoUri, string template, string? outputDir, string? outputFileName, bool printOut)
        {
            try
            {
                using (System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient())
                {
                    httpClient.DefaultRequestHeaders
                        .Accept
                        .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    string templateUri = repoUri[..(repoUri.LastIndexOf("/") + 1)] + template;

                    var task = System.Threading.Tasks.Task.Run(() => httpClient.GetAsync(templateUri));
                    task.Wait();
                    System.Net.Http.HttpResponseMessage response = task.Result;
                    response.EnsureSuccessStatusCode();
                    string bicepOutput = response.Content.ReadAsStringAsync().Result;

                    if (printOut)
                    {
                        writer.Write($"{bicepOutput}{Environment.NewLine})");
                        writer.Flush();
                    }
                    else
                    {
                        outputFileName = Path.GetFileName(outputFileName);
                        outputFileName ??= templateUri[(templateUri.LastIndexOf("/") + 1)..];
                        var outputPath = PathHelper.ResolvePath(outputFileName,outputDir);

                        File.WriteAllText(outputPath, bicepOutput);
                        writer.Write($"Created {outputPath} from '{template}' template.{Environment.NewLine}");
                        writer.Flush();
                    }
                }

                return 0;
            }
            catch (Exception exception)
            {
                this.errorWriter.WriteLine($"failed with fatal error \"{exception.Message}\"");
                return 1;
            }
        }

        private int CommandNewListTemplates(TextWriter writer, string repoUri)
        {
            try
            {
                using (System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient())
                {
                    httpClient.DefaultRequestHeaders
                        .Accept
                        .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    writer.Write($"Repository: {repoUri} ...\n");

                    var task = System.Threading.Tasks.Task.Run(() => httpClient.GetAsync(repoUri));
                    task.Wait();
                    System.Net.Http.HttpResponseMessage response = task.Result;
                    response.EnsureSuccessStatusCode();

                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    if (responseBody == null)
                    {
                        throw new Exception("Empty repository index");
                    }

                    Newtonsoft.Json.Linq.JArray jObj =
                        JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JArray>(responseBody);

                    writer.Write($"has {jObj.Count} templates:\n");

                    foreach (var item in jObj)
                    {
                        string id = item["filePath"]?.ToString() ?? "";

                        //TODO: is filePath a secure string ?
                        writer.Write($"{id}\n");
                    }
                    writer.Flush();

                }
                return 0;
            }
            catch (Exception exception)
            {
                this.errorWriter.WriteLine($"failed with fatal error \"{exception.Message}\"");
                return 1;
            }
        }

    }

}

