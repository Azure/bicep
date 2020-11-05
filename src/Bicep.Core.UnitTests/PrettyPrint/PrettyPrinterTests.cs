// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.PrettyPrint
{
    [TestClass]
    public class PrettyPrinterTests
    {
        private static readonly PrettyPrintOptions CommonOptions = new PrettyPrintOptions(
            NewlineOption.Auto,
            IndentKindOption.Space,
            2,
            false);

        [TestMethod]
        public void PrintProgram_SyntaxHasDiagnostics_ShouldReturnNull()
        {
            var syntax = ParserHelper.Parse("var foo = concat(");

            var output = PrettyPrinter.PrintProgram(syntax, CommonOptions);

            output.Should().BeNull();
        }

        [TestMethod]

        public void PrintProgram_TooManyNewlines_RemovesExtraNewlines()
        {
            const string programText = @"



param foo int




var bar = 1 + mod(foo, 3)
var baz = {
    x: [

111
222



333
444

555
666


]
      y: {

mmm: nnn
ppp: qqq





aaa: bbb
ccc: ddd



}
}




";
            ProgramSyntax programSyntax = ParserHelper.Parse(programText);

            var output = PrettyPrinter.PrintProgram(programSyntax, CommonOptions);

            output.Should().Be(
@"param foo int

var bar = 1 + mod(foo, 3)
var baz = {
  x: [
    111
    222

    333
    444

    555
    666
  ]
  y: {
    mmm: nnn
    ppp: qqq

    aaa: bbb
    ccc: ddd
  }
}");
        }

        [TestMethod]
        public void PrintSyntax_IndentKindOptionTab_ShouldIndentUsingTabs()
        {
            var programSyntax = ParserHelper.Parse(@"
var foo = {
xxx: true
yyy: {
mmm: false
nnn: [
100
'something'
{
aaa: bbb
}
]
}
}");
            var options = new PrettyPrintOptions(NewlineOption.Auto, IndentKindOption.Tab, 5, false);

            var output = PrettyPrinter.PrintProgram(programSyntax, options);

            output.Should().Be(
@"var foo = {
	xxx: true
	yyy: {
		mmm: false
		nnn: [
			100
			'something'
			{
				aaa: bbb
			}
		]
	}
}");
        }

        [TestMethod]
        public void PrintProgram_RequestInsertFinalNewline_ShouldInsertNewlineAtTheEnd()
        {
            var programSyntax = ParserHelper.Parse(string.Concat(new[]
            {
                "var foo = bar\n",
                "var bar = foo"
            }));

            var options = new PrettyPrintOptions(NewlineOption.LF, IndentKindOption.Space, 2, true);

            var output = PrettyPrinter.PrintProgram(programSyntax, options);

            output.Should().Be(string.Concat(new[]
            {
                "var foo = bar\n",
                "var bar = foo\n"
            }));
        }

        [TestMethod]
        public void PrintProgram_DoesNotRequestInsertFinalNewline_ShouldTrimNewlineAtTheEnd()
        {
            var programSyntax = ParserHelper.Parse(string.Concat(new[]
            {
                "var foo = bar\n",
                "var bar = foo\n"
            }));

            var options = new PrettyPrintOptions(NewlineOption.LF, IndentKindOption.Space, 2, false);

            var output = PrettyPrinter.PrintProgram(programSyntax, options);

            output.Should().Be(string.Concat(new[]
            {
                "var foo = bar\n",
                "var bar = foo"
            }));
        }


        [DataTestMethod]
        [DataRow(NewlineOption.LF, "\r", "\n")]
        [DataRow(NewlineOption.CRLF, "\r", "\r\n")]
        [DataRow(NewlineOption.CR, "\n", "\r")]
        public void PrintProgram_NewLineOptionNotAuto_ShouldUseSpecifiedNewline(NewlineOption newlineOption, string originalNewline, string expectedNewline)
        {
            var programSyntax = ParserHelper.Parse(string.Join(originalNewline, new[]
            {
                "var foo = bar",
                "var bar = foo",
                "var baz = bar"
            }));

            var options = new PrettyPrintOptions(newlineOption, IndentKindOption.Space, 2, false);

            var output = PrettyPrinter.PrintProgram(programSyntax, options);

            output.Should().Be(string.Join(expectedNewline, new[]
            {
                "var foo = bar",
                "var bar = foo",
                "var baz = bar"
            }));
        }

        [TestMethod]
        public void PrintProgram_NewLineOptionAuto_ShouldInferNewlineKindFromTheFirstNewline()
        {
            var programSyntax = ParserHelper.Parse(string.Concat(new[]
            {
                "var foo = bar\r",
                "var bar = foo\n"
            }));

            var options = new PrettyPrintOptions(NewlineOption.Auto, IndentKindOption.Space, 2, true);

            var output = PrettyPrinter.PrintProgram(programSyntax, options);

            output.Should().Be(string.Concat(new[]
            {
                "var foo = bar\r",
                "var bar = foo\r"
            }));
        }

        [TestMethod]
        public void PrintProgram_NewLineOptionAutoWithNoNewlineInProgram_ShouldUseEnvironmentNewline()
        {
            var programSyntax = ParserHelper.Parse("var foo = bar");
            var options = new PrettyPrintOptions(NewlineOption.Auto, IndentKindOption.Space, 2, true);

            var output = PrettyPrinter.PrintProgram(programSyntax, options);

            output.Should().Be($"var foo = bar{Environment.NewLine}");
        }

        [TestMethod]
        public void PrintProgram_CommentAfterOpenSyntax_ShouldMoveToNextLineAndIndent()
        {
            var programSyntax = ParserHelper.Parse(@"
param foo object = { // I can be anywhere
}

param foo object = { // I can be anywhere
  abc: true
}

param foo object = { /* I can be anywhere */
  abc: true
  xyz: false
}

param foo object = { /* I can
  be anywhere */
  abc: true
  xyz: false
}

param bar array = [ // I can be anywhere
]

param bar array = [ // I can be anywhere
  true
]

param bar array = [     /*I can be anywhere */          // I can be anywhere
  true
  false
]");

            var output = PrettyPrinter.PrintProgram(programSyntax, CommonOptions);

            output.Should().Be(
@"param foo object = {
  // I can be anywhere
}

param foo object = {
  // I can be anywhere
  abc: true
}

param foo object = {
  /* I can be anywhere */
  abc: true
  xyz: false
}

param foo object = {
  /* I can
  be anywhere */
  abc: true
  xyz: false
}

param bar array = [
  // I can be anywhere
]

param bar array = [
  // I can be anywhere
  true
]

param bar array = [
  /*I can be anywhere */ // I can be anywhere
  true
  false
]");
        }

        [TestMethod]
        public void PrintProgram_CommentBeforeCloseSyntax_ShouldMoveOneLineAboveAndIndent()
        {
            var programSyntax = ParserHelper.Parse(@"
param foo object = { /* I can be anywhere */ }

param foo object = {
  /* I can be anywhere */ }

param foo object = {
  abc: true
/* I can be anywhere */}

param foo object = {
  abc: true
  xyz: false
  /* I can be anywhere */}

param foo object = {
  abc: true
  xyz: false
            /* I
  can
  be anywhere
  */}

param bar array = [
/* I can be anywhere */]

param bar array = [ /* I can be anywhere */]

param bar array = [
  true
/* I can be anywhere */   ]

param bar array = [
  true
  false
   /* I can be anywhere */       /* I can be anywhere */]");

            var output = PrettyPrinter.PrintProgram(programSyntax, CommonOptions);

            output.Should().Be(
@"param foo object = {
  /* I can be anywhere */
}

param foo object = {
  /* I can be anywhere */
}

param foo object = {
  abc: true
  /* I can be anywhere */
}

param foo object = {
  abc: true
  xyz: false
  /* I can be anywhere */
}

param foo object = {
  abc: true
  xyz: false
  /* I
  can
  be anywhere
  */
}

param bar array = [
  /* I can be anywhere */
]

param bar array = [
  /* I can be anywhere */
]

param bar array = [
  true
  /* I can be anywhere */
]

param bar array = [
  true
  false
  /* I can be anywhere */ /* I can be anywhere */
]");
        }

        [TestMethod]
        public void PrintProgram_EmptyBlocks_ShouldFormatCorrectly()
        {
            var programSyntax = ParserHelper.Parse(@"
param foo object = {}
param foo object = {
}
param foo object = {

}
param foo object = {




}

param bar array = []
param bar array = [
]
param bar array = [

]
param bar array = [




]");

            var output = PrettyPrinter.PrintProgram(programSyntax, CommonOptions);

            output.Should().Be(
@"param foo object = {}
param foo object = {}
param foo object = {}
param foo object = {}

param bar array = []
param bar array = []
param bar array = []
param bar array = []");
        }

        [TestMethod]
        public void PrintProgram_SimpleProgram_ShouldFormatCorrectly()
        {
            var programSyntax = ParserHelper.Parse(@"
param string banana
param  string apple {

    allowed   : [
'Static'
'Dynamic'
]

        metadata: {
    description        : 'no description'
}

}

   var num = 1
var call = func1(     func2 (1), func3 (true)[0]       .a   .    b.c    )

     resource     myResource1               'myResource'      ={
        name : 'myName'
    obj : {

x: y
m: [
    1
    false
null
{
    abc: edf
}
]

}
}


module myModule 'myModule' = {

name  : concat('a', 'b', 'c')

params       : {
    myParam: call . blah [3]
}

}


resource myResource2  'myResource'={
   something: 'foo/${myName}/bar'
    properties: {
emptyObj: {
}
    emptyArr: [
]
}
}


output       myOutput1 int    =     1 +    num *    3
    output      myOutput2  string =   yes   ?   'yes'   :   'no'
    output      myOutput3  object =   yes   ?   {
    value : 42
}:{






}

");
            var output = PrettyPrinter.PrintProgram(programSyntax, CommonOptions);

            output.Should().Be(
@"param string banana
param string apple {
  allowed: [
    'Static'
    'Dynamic'
  ]

  metadata: {
    description: 'no description'
  }
}

var num = 1
var call = func1(func2(1), func3(true)[0].a.b.c)

resource myResource1 'myResource' = {
  name: 'myName'
  obj: {
    x: y
    m: [
      1
      false
      null
      {
        abc: edf
      }
    ]
  }
}

module myModule 'myModule' = {
  name: concat('a', 'b', 'c')

  params: {
    myParam: call.blah[3]
  }
}

resource myResource2 'myResource' = {
  something: 'foo/${myName}/bar'
  properties: {
    emptyObj: {}
    emptyArr: []
  }
}

output myOutput1 int = 1 + num * 3
output myOutput2 string = yes ? 'yes' : 'no'
output myOutput3 object = yes ? {
  value: 42
} : {}");
        }

        /*
         sljfaljsfasfljasf 

        */
        [TestMethod]
        public void PrintProgram_CommentBomb_ShouldFormatCorrectly()
        {
            var programSyntax = ParserHelper.Parse(
@" // I can be anywhere
/*
 * I can
 * be anywhere
 */

/* I can be any
where */   module /* I can be anywhere */ foo  /* I can be anywhere */  './myModule' = /* I can be anywhere *//* I can be anywhere */{
name/* I can be any where */ : value // I can be anywhere
}


var foo = {
// I can be any where
}


param foo bool {
  default: (true /* I can be anywhere */ ? /*
I can be any
where
*/null : false /* I can be anywhere */)
/* I can be anywhere */}


/* I can be anywhere */              // I can be anywhere
// I can be anywhere
param foo string // I can be anywhere
// I can be anywhere
param bar string = { /* I can be
anywhere */    /* I can be anywhere */
                           foo  : true
    bar /* I can be anywhere */  : false
  /* I can be anywhere */    baz  : [
bar
az /* I can be anywhere */ .func /* I can be anywhere */ ('foobar', '/', 'bar')[/* I can be anywhere */  1   /* I can be anywhere */] /* I can be anywhere */  .  /* I can be anywhere */ baz // I can be anywhere
        true
        {
        m: [
] /* I can be any
where */
            kkk: [
// I can be any where
// I can be any where

]
 x: y
p: q
/* I can be any where */
                 // I can be any where
// I can be anywhere
}
null
/* I can be anywhere *//* I can be anywhere */] // I can be any where
}
     /* I can be anywhere */
");

            var output = PrettyPrinter.PrintProgram(programSyntax, CommonOptions);

            output.Should().Be(
@"// I can be anywhere
/*
 * I can
 * be anywhere
 */

/* I can be any
where */module  /* I can be anywhere */foo  /* I can be anywhere */'./myModule' =  /* I can be anywhere */ /* I can be anywhere */{
  name /* I can be any where */: value // I can be anywhere
}

var foo = {
  // I can be any where
}

param foo bool {
  default: (true  /* I can be anywhere */?  /*
I can be any
where
*/null : false /* I can be anywhere */)
  /* I can be anywhere */
}

/* I can be anywhere */ // I can be anywhere
// I can be anywhere
param foo string // I can be anywhere
// I can be anywhere
param bar string = {
  /* I can be
anywhere */ /* I can be anywhere */
  foo: true
  bar /* I can be anywhere */: false
  /* I can be anywhere */baz: [
    bar
    az /* I can be anywhere */.func /* I can be anywhere */('foobar', '/', 'bar')[ /* I can be anywhere */1 /* I can be anywhere */] /* I can be anywhere */. /* I can be anywhere */baz // I can be anywhere
    true
    {
      m: [] /* I can be any
where */
      kkk: [
        // I can be any where
        // I can be any where
      ]
      x: y
      p: q
      /* I can be any where */
      // I can be any where
      // I can be anywhere
    }
    null
    /* I can be anywhere */ /* I can be anywhere */
  ] // I can be any where
}
/* I can be anywhere */");
        }
    }
}
