using Bicep.Parser;
using System;
using System.IO;
using System.Text;
using Bicep.Cli.CommandLine;

namespace Bicep.Cli
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Arguments? arguments = ArgumentParser.Parse(args);
            if (arguments == null)
            {
                ArgumentParser.PrintUsage();
                return 0;
            }

            switch (arguments)
            {
                case BuildArguments buildArguments:
                    Build(buildArguments);
                    break;
            }

            // TODO: If we had logging and errors were logged, exit code should be 1
            return 0;
        }

        private static void Build(BuildArguments arguments)
        {
            foreach (string file in arguments.Files)
            {
                BuildSingleFile(ResolvePath(file));
            }
        }

        private static string ResolvePath(string path)
        {
            if (Path.IsPathFullyQualified(path))
            {
                return path;
            }

            return Path.Combine(Environment.CurrentDirectory, path);
        }

        private static void BuildSingleFile(string filePath)
        {
            var contents = File.ReadAllText(filePath, Encoding.UTF8);
            var lexer = new Lexer(new SlidingTextWindow(contents));
            lexer.Lex();

            var tokens = lexer.GetTokens();
            var parser = new Parser.Parser(tokens);

            var program = parser.Parse();

            var printer = new PrintVisitor(Console.Write, false);
            printer.Visit(program);
        }
    }
}
