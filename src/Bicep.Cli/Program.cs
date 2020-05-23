﻿using Bicep.Parser;
using System;
using System.IO;
using System.Text;

namespace Bicep.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var contents = File.ReadAllText("../../basic.arm", Encoding.UTF8);
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
