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
        public void PrintProgram_SkippedTriviaSyntax_ShouldPrintAsIs() =>
            this.TestPrintProgram(
// Raw.
@"parm foo string
### blah blah blah


   

blah


var bar     = true    blah /* asfjljasfs */     blah

var baz = {
}    blah      blah


output foobar array = [
]           null

output pi object = {
} /* leading whitespaces after me */        null



concat('foo',             'bar')

1 + 2
resource vnet 'Microsoft.Network/virtualNetworks@2020-01-01' = { // some comment
             name: 'myVnet'
}    something",
// Formatted.
@"parm foo string
### blah blah blah

blah

var bar = true blah /* asfjljasfs */     blah

var baz = {} blah      blah

output foobar array = [] null

output pi object = {} /* leading whitespaces after me */        null

concat('foo',             'bar')

1 + 2
resource vnet 'Microsoft.Network/virtualNetworks@2020-01-01' = {
  // some comment
  name: 'myVnet'
} something");

        [DataTestMethod]
        // Broken targetScope assignments.
        [DataRow(
@"targetScope
targetScope         =
targetScope =###")]
        // Broken variable declarations.
        [DataRow(
@"var  foo  10000
var   foo =  /* missing '{' */ }
var foo   = {
        something = true

}
var   foo = [
    1,
    2,
    3
]")]
        // Broken module declarations.
        [DataRow(
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
}")]
        // Broken resource declarations.
        [DataRow(
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
}")]
        // Broken output declarations.
        [DataRow(
@"output  foo   =
output  foo string
output  foo   object = {")]
        public void PrintProgram_BrokenStatement_ShouldPrintAsIs(string programText) =>
            this.TestPrintProgram(programText, programText);

        [TestMethod]
        public void PrintProgram_SomeBrokenSyntaxes_ShouldFormatTheOtherValidSyntaxes() =>
            this.TestPrintProgram(
// Raw.
@"targetScope='subscription'

// Broken.
targetScope   =

param   foo string
param foo   int

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
output foo =          true",
// Formatted.
@"targetScope = 'subscription'

// Broken.
targetScope   =

param foo string
param foo int

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
