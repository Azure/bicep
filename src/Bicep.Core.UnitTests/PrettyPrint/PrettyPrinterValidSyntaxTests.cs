// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.PrettyPrint
{
    [TestClass]
    public class PrettyPrinterValidSyntaxTests : PrettyPrinterTestsBase
    {
        [TestMethod]
        public void PrintProgram_TooManyNewlines_RemovesExtraNewlines() => this.TestPrintProgram(
// Raw.
@"



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

  


",
// Formatted.
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

        [TestMethod]
        public void PrintSyntax_IndentKindOptionTab_ShouldIndentUsingTabs() => this.TestPrintProgram(
// Raw.
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
}",
// Formatted.
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
}".Replace("\n", Environment.NewLine),
            new PrettyPrintOptions(NewlineOption.Auto, IndentKindOption.Tab, 5, false));

        [TestMethod]
        public void PrintProgram_RequestInsertFinalNewline_ShouldInsertNewlineAtTheEnd() => this.TestPrintProgram(
            string.Concat(new[]
            {
                "var foo = bar\n",
                "var bar = foo"
            }),
            string.Concat(new[]
            {
                "var foo = bar\n",
                "var bar = foo\n"
            }),
            new PrettyPrintOptions(NewlineOption.LF, IndentKindOption.Space, 2, true));

        [TestMethod]
        public void PrintProgram_DoesNotRequestInsertFinalNewline_ShouldTrimNewlineAtTheEnd() => this.TestPrintProgram(
            string.Concat(new[]
            {
                "var foo = bar\n",
                "var bar = foo\n"
            }),
            string.Concat(new[]
            {
                "var foo = bar\n",
                "var bar = foo"
            }),
            new PrettyPrintOptions(NewlineOption.LF, IndentKindOption.Space, 2, false));


        [DataTestMethod]
        [DataRow(NewlineOption.LF, "\r", "\n")]
        [DataRow(NewlineOption.CRLF, "\r", "\r\n")]
        [DataRow(NewlineOption.CR, "\n", "\r")]
        public void PrintProgram_NewLineOptionNotAuto_ShouldUseSpecifiedNewline(NewlineOption newlineOption, string originalNewline, string expectedNewline) => this.TestPrintProgram(
            string.Join(originalNewline, new[]
            {
                "var foo = bar",
                "var bar = foo",
                "var baz = bar"
            }),
            string.Join(expectedNewline, new[]
            {
                "var foo = bar",
                "var bar = foo",
                "var baz = bar"
            }),
            new PrettyPrintOptions(newlineOption, IndentKindOption.Space, 2, false));

        [TestMethod]
        public void PrintProgram_NewLineOptionAuto_ShouldInferNewlineKindFromTheFirstNewline() => this.TestPrintProgram(
            string.Concat(new[]
            {
                "var foo = bar\r",
                "var bar = foo\n"
            }),
            string.Concat(new[]
            {
                "var foo = bar\r",
                "var bar = foo\r"
            }),
            new PrettyPrintOptions(NewlineOption.Auto, IndentKindOption.Space, 2, true));

        [TestMethod]
        public void PrintProgram_NewLineOptionAutoWithNoNewlineInProgram_ShouldUseEnvironmentNewline() => this.TestPrintProgram(
            "var foo = bar",
            $"var foo = bar{Environment.NewLine}",
            new PrettyPrintOptions(NewlineOption.Auto, IndentKindOption.Space, 2, true));

        [TestMethod]
        public void PrintProgram_CommentAfterOpenSyntax_ShouldMoveToNextLineAndIndent() => this.TestPrintProgram(
// Raw.
@"
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
]",
// Formatted.
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

        [TestMethod]
        public void PrintProgram_CommentBeforeCloseSyntax_ShouldMoveOneLineAboveAndIndent() => this.TestPrintProgram(
// Raw.
@"param foo object = { /* I can be anywhere */ }

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
   /* I can be anywhere */       /* I can be anywhere */]",
// Formatted.
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

        [TestMethod]
        public void PrintProgram_EmptyBlocks_ShouldFormatCorrectly() => this.TestPrintProgram(
//Raw.
@"param foo object = {}
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

  


]",
// Formatted.
@"param foo object = {}
param foo object = {}
param foo object = {}
param foo object = {}

param bar array = []
param bar array = []
param bar array = []
param bar array = []");

        [TestMethod]
        public void PrintProgram_MultilineComment_ShouldReplaceNewlinesInTheCommentToo() => this.TestPrintProgram(
            string.Join("\n", new[]
            {
                "var foo = bar",
                "/* a multiline\ncomment\n */",
                "var baz = foo"
            }),
            string.Join("\r\n", new[]
            {
                "var foo = bar",
                "/* a multiline\r\ncomment\r\n */",
                "var baz = foo"
            }),
            new PrettyPrintOptions(NewlineOption.CRLF, IndentKindOption.Space, 2, false)
            );

        [TestMethod]
        public void PrintProgram_SimpleProgram_ShouldFormatCorrectly() => this.TestPrintProgram(
// Raw.
@"targetScope=                    'managementGroup'

param string banana

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
",
// Formatted.
@"targetScope = 'managementGroup'

param string banana

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

        [TestMethod]
        public void PrintProgram_CommentBomb_ShouldFormatCorrectly()
        {
            this.TestPrintProgram(
// Raw.
@" // I can be anywhere
/*
 * I can
 * be anywhere
 */

/* I can be any
where */   module /* I can be anywhere */ foo  /* I can be anywhere */  './myModule' = /* I can be anywhere */{
name/* I can be any where */ : value // I can be anywhere
}


var foo = {
// I can be any where
}



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
",
// Formatted.
@"// I can be anywhere
/*
 * I can
 * be anywhere
 */

/* I can be any
where */module /* I can be anywhere */ foo /* I can be anywhere */ './myModule' = /* I can be anywhere */ {
  name /* I can be any where */: value // I can be anywhere
}

var foo = {
  // I can be any where
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
