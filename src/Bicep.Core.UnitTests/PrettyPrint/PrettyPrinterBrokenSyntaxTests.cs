// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.PrettyPrint;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.PrettyPrint
{
    [TestClass]
    public class PrettyPrinterBrokenSyntaxTests : PrettyPrinterTestsBase
    {
        [TestMethod]
        public void PrintProgram_SkippedTriviaSyntax_ShouldPrintAsIs()
        {
            ProgramSyntax? programSyntax = ParserHelper.Parse(
@"parm foo string
### blah blah blah


   

blah


var bar     = true    blah /* asfjljasfs */     blah

var baz = {
}    blah      blah


output foobar array = [
]           null



concat('foo',             'bar')

1 + 2
resource vnet 'Microsoft.Network/virtualNetworks@2020-01-01' = { // some comment
             name: 'myVnet'
}    something");

            string? output = PrettyPrinter.PrintProgram(programSyntax, CommonOptions);

            output.Should().Be(
@"parm foo string
### blah blah blah

blah

var bar = true blah /* asfjljasfs */     blah

var baz = {} blah      blah

output foobar array = [] null

concat('foo',             'bar')

1 + 2
resource vnet 'Microsoft.Network/virtualNetworks@2020-01-01' = {
  // some comment
  name: 'myVnet'
} something");
        }

        [TestMethod]
        public void PrintProgram_BrokenParameterDeclarationSyntax_ShouldPrintAsIs()
        {
            const string programText =
@"param  foo
param foo =   true
param foo    object = {
   test: true
anminals   [
  

    'cat'
    'dog' something
}
param   foo string {
    allowed:
        'Dynamic'
  
        'Static'
    ]



}";
            var programSyntax = ParserHelper.Parse(programText);

            string? output = PrettyPrinter.PrintProgram(programSyntax, CommonOptions);

            output.Should().Be(programText);
        }

        [TestMethod]
        public void PrintProgram_BrokenVariableDeclarationSyntax_ShouldPrintAsIs()
        {
            const string programText =
@"var  foo  10000
var   foo =  /* missing '{' */ }
var foo   = {
        something = true

}
var   foo = [
    1,
    2,
    3
]";
            var programSyntax = ParserHelper.Parse(programText);

            string? output = PrettyPrinter.PrintProgram(programSyntax, CommonOptions);

            output.Should().Be(programText);
        }

        [TestMethod]
        public void PrintProgram_BrokenModuleDeclarationSyntax_ShouldPrintAsIs()
        {
            const string programText =
@"module  foo   =
module  foo './foo'  = {
       name:
}
module  foo './foo'  = {
       name 'myModule'
}
module  foo './foo'  = {
       : 'myModule'
}
module  foo './foo'  = {
       name: 'myModule'

    params: {
     location: 'westus'
// missing '}'
}";
            var programSyntax = ParserHelper.Parse(programText);

            string? output = PrettyPrinter.PrintProgram(programSyntax, CommonOptions);

            output.Should().Be(programText);
        }

        [TestMethod]
        public void PrintProgram_BrokenResourceDeclarationSyntax_ShouldPrintAsIs()
        {
            const string programText =
@"resource  foo   =
resource  foo 'Provider/Type@api-version'
resource  foo   'Provider/Type@api-version' = {

name: {

}

resource  /* missing an indentifier */   'Provider/Type@api-version' = {
name: 'foo'
}
resource  foo  'provider'  {
  name: '${resourceName}'
  properties: concat('foo'    'bar')
}";
            var programSyntax = ParserHelper.Parse(programText);

            string? output = PrettyPrinter.PrintProgram(programSyntax, CommonOptions);

            output.Should().Be(programText);
        }

        [TestMethod]
        public void PrintProgram_BrokenOutputDeclarationSyntax_ShouldPrintAsIs()
        {
            const string programText =
@"output  foo   =
output  foo string
output  foo   object = {";
            var programSyntax = ParserHelper.Parse(programText);

            string? output = PrettyPrinter.PrintProgram(programSyntax, CommonOptions);

            output.Should().Be(programText);
        }

        [TestMethod]
        public void PrintProgram_BrokenSyntaxes_ShouldFormatTheOtherValidSyntaxes()
        {
            const string programText = @"
param   foo string
param foo   int
param foo string {
} /* skipped trivia */   something

// Broken.
param foo string {
    allowed [  ]
}


// Skipped trivia
###
something
something
@@@

var foo = something
var foo  =  {
key: value
}  blah   blah // skipped trivia

/* comment */      var foo = [
1
2
3
4



] some trivia
    

// Broken.
    var  foo something

module foo './foo'     = {
    name: 'foo'
}


// Broken.
module foo './foo'   = []


resource foo 'Foo' = {
    name: 'foo'
    properties: {
}
}


// Broken.
resource foo 'Foo' = {
    name: 'foo'
    properties: {
    key: 
value
}
}

output foo string = '${bar}/${baz}'
output foo int = mod(1, 2)


// Broken
output foo =          true";
            var programSyntax = ParserHelper.Parse(programText);

            string? output = PrettyPrinter.PrintProgram(programSyntax, CommonOptions);

            output.Should().Be(
@"param foo string
param foo int
param foo string {} /* skipped trivia */    something

// Broken.
param foo string {
    allowed [  ]
}

// Skipped trivia
###
something
something
@@@

var foo = something
var foo = {
  key: value
} blah   blah // skipped trivia

/* comment */var foo = [
  1
  2
  3
  4
] some trivia
    

// Broken.
var  foo something

module foo './foo' = {
  name: 'foo'
}

// Broken.
module foo './foo'   = []

resource foo 'Foo' = {
  name: 'foo'
  properties: {}
}

// Broken.
resource foo 'Foo' = {
    name: 'foo'
    properties: {
    key: 
value
}
}

output foo string = '${bar}/${baz}'
output foo int = mod(1, 2)

// Broken
output foo =          true");
        }
    }
}
