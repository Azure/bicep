// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.PrettyPrintV2.Documents;

using static Bicep.Core.PrettyPrintV2.Documents.DocumentOperators;
using System.IO;
using Bicep.Core.PrettyPrintV2.Options;
using System.Collections.Immutable;

namespace Bicep.Core.UnitTests.PrettyPrint2
{
    [TestClass]
    public class PrintTests
    {
        [TestMethod]
        public void TestPrint()
        {
            //var doc = Bracket(
            //    "[",
            //    Concat(
            //        "Foo",
            //        LineOrCommaSpace,
            //        "Bar"),
            //    "]");

            //var lineBreakers = new[] { doc }.ToImmutableHashSet<Document>();
            ////var lineBreakers = ImmutableHashSet<Document>.Empty;

            //var buffer = new StringBuilder();
            //using var writer = new StringWriter(buffer);
            //var printer = new PrettyPrinter(writer, new PrettyPrintOptions(NewlineKind.Auto, IndentKind.Space, 2, 12, false), lineBreakers);

            //printer.Print(new[] { doc });

            //var output = buffer.ToString();
        }
    }
}
