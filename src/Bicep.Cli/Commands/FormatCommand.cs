// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
using Bicep.Cli.Logging;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Bicep.Cli.Commands
{
    public class FormatCommand : ICommand
    {
        private readonly ILogger logger;
        private readonly IDiagnosticLogger diagnosticLogger;
        private readonly InvocationContext invocationContext;
        private readonly IFileResolver fileResolver;

        public FormatCommand(
            ILogger logger,
            IDiagnosticLogger diagnosticLogger,
            InvocationContext invocationContext,
            IFileResolver fileResolver)
        {
            this.logger = logger;
            this.diagnosticLogger = diagnosticLogger;
            this.invocationContext = invocationContext;
            this.fileResolver = fileResolver;
        }

        public int Run(FormatArguments args)
        {
            var inputPath = PathHelper.ResolvePath(args.InputFile);
            
            if (IsBicepFile(inputPath))
            {
                var inputUri = PathHelper.FilePathToFileUrl(inputPath);
                if (!fileResolver.TryRead(inputUri, out var fileContents, out var failureBuilder))
                {
                    var diagnostic = failureBuilder(DiagnosticBuilder.ForPosition(new TextSpan(0, 0)));
                    throw new ErrorDiagnosticException(diagnostic);
                }
                var parser = new Parser(fileContents);
                var programSyntax = parser.Program();
                var options = new PrettyPrintOptions(
                    args.Newline            ?? NewlineOption.Auto,
                    args.IndentKind         ?? IndentKindOption.Space,
                    args.IndentSize         ?? 2,
                    args.InsertFinalNewline ?? false
                );

                string output = PrettyPrinter.PrintProgram(programSyntax, options);
                if (args.OutputToStdOut)
                {
                    invocationContext.OutputWriter.Write(output);
                    invocationContext.OutputWriter.Flush();
                }
                else
                {
                    static string DefaultOutputPath(string path) => path;
                    var outputPath = PathHelper.ResolveDefaultOutputPath(inputPath, args.OutputDir, args.OutputFile, DefaultOutputPath);

                    File.WriteAllText(outputPath, output);
                }
                
                return diagnosticLogger.ErrorCount > 0 ? 1 : 0;       
            }

            logger.LogError(CliResources.UnrecognizedFileExtensionMessage, inputPath);
            return 1;
        }

        private static bool IsBicepFile(string inputPath) => PathHelper.HasBicepExtension(PathHelper.FilePathToFileUrl(inputPath));
    }
}
