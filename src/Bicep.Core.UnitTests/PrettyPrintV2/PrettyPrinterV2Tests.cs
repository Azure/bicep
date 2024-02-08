// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.PrettyPrintV2;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.PrettyPrintV2
{
    [TestClass]
    public class PrettyPrinterV2Tests
    {
        private readonly static string ProgramText = """
            var foo = {
            prop1: true
            prop2: false
            prop3: {
            nestedProp1: 1
            nestedProp2: 2
            }
            }

            var bar = [
            1
            2
            {
            prop1: true
            prop2: false
            }
            ]
            """.ReplaceLineEndings("\n");

        [DataTestMethod]
        [DataRow(IndentKind.Space, NewlineKind.CRLF, 2, true, "var foo = {\r\n  prop1: true\r\n  prop2: false\r\n  prop3: {\r\n    nestedProp1: 1\r\n    nestedProp2: 2\r\n  }\r\n}\r\n\r\nvar bar = [\r\n  1\r\n  2\r\n  {\r\n    prop1: true\r\n    prop2: false\r\n  }\r\n]\r\n")]
        [DataRow(IndentKind.Tab, NewlineKind.LF, 0, false, "var foo = {\n\tprop1: true\n\tprop2: false\n\tprop3: {\n\t\tnestedProp1: 1\n\t\tnestedProp2: 2\n\t}\n}\n\nvar bar = [\n\t1\n\t2\n\t{\n\t\tprop1: true\n\t\tprop2: false\n\t}\n]")]
        [DataRow(IndentKind.Tab, NewlineKind.CR, 2, true, "var foo = {\r\tprop1: true\r\tprop2: false\r\tprop3: {\r\t\tnestedProp1: 1\r\t\tnestedProp2: 2\r\t}\r}\r\rvar bar = [\r\t1\r\t2\r\t{\r\t\tprop1: true\r\t\tprop2: false\r\t}\r]\r")]
        public void Print_VariousOptions_PrintsAccordingly(IndentKind indentKind, NewlineKind newlineKind, int indentSize, bool insertFinalNewline, string expectedOutput)
        {
            var options = PrettyPrinterV2Options.Default with
            {
                IndentKind = indentKind,
                NewlineKind = newlineKind,
                IndentSize = indentSize,
                InsertFinalNewline = insertFinalNewline,
            };

            var program = ParserHelper.Parse(ProgramText, out var lexingErrorLookup, out var parsingErrorLookup);
            var context = PrettyPrinterV2Context.Create(options, lexingErrorLookup, parsingErrorLookup);

            var output = PrettyPrinterV2.Print(program, context);

            output.Should().Be(expectedOutput);
        }

        [TestMethod]
        public void Print_HasTrailingSpaces_TrimsTrailingSpaces()
        {
            var programText = " var foo = { \n prop1: true  \n   \nprop2: false }\n";
            var program = ParserHelper.Parse(programText, out var lexingErrorLookup, out var parsingErrorLookup);
            var context = PrettyPrinterV2Context.Create(PrettyPrinterV2Options.Default, lexingErrorLookup, parsingErrorLookup);

            var output = PrettyPrinterV2.Print(program, context);

            output.Should().Be("var foo = {\n  prop1: true\n\n  prop2: false\n}\n");
        }

        [TestMethod]
        public void Print_StartsAndEndsWithNewlines_TrimNewlines()
        {
            var programText = "\n\n\n \n \n // comment  \nvar foo = true\n \n // comment \n \n";
            var program = ParserHelper.Parse(programText, out var lexingErrorLookup, out var parsingErrorLookup);
            var context = PrettyPrinterV2Context.Create(PrettyPrinterV2Options.Default, lexingErrorLookup, parsingErrorLookup);

            var output = PrettyPrinterV2.Print(program, context);

            output.Should().Be("// comment  \nvar foo = true\n\n// comment \n");
        }

        [TestMethod]
        public void Print_HasConsecutiveNewlines_CollapsesNewlines()
        {
            var programText = " var foo = {\n prop1: true\n\n\n \n\n   \n\n\nprop2: false}\n\n \n/* leading comment */ \n\n\n   \nvar bar = 1\n";
            var program = ParserHelper.Parse(programText, out var lexingErrorLookup, out var parsingErrorLookup);
            var context = PrettyPrinterV2Context.Create(PrettyPrinterV2Options.Default, lexingErrorLookup, parsingErrorLookup);

            var output = PrettyPrinterV2.Print(program, context);

            output.Should().Be("var foo = {\n  prop1: true\n\n  prop2: false\n}\n\n/* leading comment */\n\nvar bar = 1\n");
        }
    }
}
