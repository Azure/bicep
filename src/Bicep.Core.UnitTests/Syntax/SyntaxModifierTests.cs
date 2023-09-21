// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Rewriters;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Completions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.UnitTests.Syntax;

[TestClass]
public class SyntaxModifierTests
{
    private static TSyntax GetLastNode<TSyntax>(List<SyntaxBase> nodes)
         where TSyntax : SyntaxBase
    {
        var node = SyntaxMatcher.FindLastNodeOfType<TSyntax, TSyntax>(nodes).node;
        node.Should().NotBeNull();

        return node!;
    }

    private static string RewriteProgram(string fileWithCursor, Func<ProgramSyntax, List<SyntaxBase>, IDiagnosticLookup, ProgramSyntax> rewriteFunc)
    {
        var (file, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursor, '|');

        var program = ParserHelper.Parse(file, out var _, out var parsingErrorLookup);
        var nodes = SyntaxMatcher.FindNodesMatchingOffset(program, cursor);

        var rewrittenProgram = rewriteFunc(program, nodes, parsingErrorLookup);

        return rewrittenProgram.ToTextPreserveFormatting();
    }

    [DataTestMethod]
    [DataRow(@"
var adsf = [
  'abc'
  'de|f'
  'ghi'
]
", @"
var adsf = [
  'abc'
  'ghi'
]
")]
    [DataRow(@"
var adsf = [
  'ab|c'
  'def'
  'ghi'
]
", @"
var adsf = [
  'def'
  'ghi'
]
")]
    [DataRow(@"
var adsf = [
  'abc'
  'def'
  'gh|i'
]
", @"
var adsf = [
  'abc'
  'def'
]
")]
    [DataRow(@"
var adsf = [          'ab|c', 'def', 'ghi' ]
", @"
var adsf = [          'def', 'ghi' ]
")]
    [DataRow(@"
var adsf = [ 'abc', 'de|f', 'ghi' ]
", @"
var adsf = [ 'abc', 'ghi' ]
")]
    [DataRow(@"
var adsf = [ 'abc', 'def', /* comment */ 'g|hi' ]
", @"
var adsf = [ 'abc', 'def']
")]
    // syntax with errors is not rewritten
    [DataRow(@"
var adsf = [
  'abc'
  'de|f'
  'ghi',,,,
]
", @"
var adsf = [
  'abc'
  'def'
  'ghi',,,,
]
")]
    public void TryRemoveItem_removes_matching_array_items(string fileWithCursor, string expected)
    {
        var rewritten = RewriteProgram(
            fileWithCursor,
            (program, nodes, parsingErrorLookup) =>
            {
                var array = GetLastNode<ArraySyntax>(nodes);
                var item = GetLastNode<ArrayItemSyntax>(nodes);

                return CallbackRewriter.Rewrite(
                    program,
                    node => node == array && SyntaxModifier.TryRemoveItem(array, item, parsingErrorLookup) is { } newArray ? newArray : node);
            });

        rewritten.Should().BeEquivalentToIgnoringNewlines(expected);
    }

    [DataTestMethod]
    [DataRow(@"
var adsf = {
  abc: 'abc'
  def: 'de|f'
  ghi: 'ghi'
}
", @"
var adsf = {
  abc: 'abc'
  ghi: 'ghi'
}
")]
    [DataRow(@"
var adsf = {
  abc: 'a|bc'
  def: 'def'
  ghi: 'ghi'
}
", @"
var adsf = {
  def: 'def'
  ghi: 'ghi'
}
")]
    [DataRow(@"
var adsf = {
  abc: 'abc'
  def: 'def'
  ghi: 'g|hi'
}
", @"
var adsf = {
  abc: 'abc'
  def: 'def'
}
")]
    [DataRow(@"
var adsf = {         abc: 'ab|c', def: 'def', ghi: 'ghi' }
", @"
var adsf = {         def: 'def', ghi: 'ghi' }
")]
    [DataRow(@"
var adsf = { abc: 'abc', def: 'd|ef', ghi: 'ghi' }
", @"
var adsf = { abc: 'abc', ghi: 'ghi' }
")]
    [DataRow(@"
var adsf = { abc: 'abc', def: 'def', /* comment */ ghi: 'g|hi' }
", @"
var adsf = { abc: 'abc', def: 'def'}
")]
    // syntax with errors is not rewritten
    [DataRow(@"
var adsf = {
  abc: 'abc'
  def: 'de|f'
  ghi:
}
", @"
var adsf = {
  abc: 'abc'
  def: 'def'
  ghi:
}
")]
    public void TryRemoveProperty_removes_matching_object_properties(string fileWithCursor, string expected)
    {
        var rewritten = RewriteProgram(
            fileWithCursor,
            (program, nodes, parsingErrorLookup) =>
            {
                var @object = GetLastNode<ObjectSyntax>(nodes);
                var property = GetLastNode<ObjectPropertySyntax>(nodes);

                return CallbackRewriter.Rewrite(
                    program,
                    node => node == @object && SyntaxModifier.TryRemoveProperty(@object, property, parsingErrorLookup) is { } newObject ? newObject : node);
            });

        rewritten.Should().BeEquivalentToIgnoringNewlines(expected);
    }

    [DataTestMethod]
    [DataRow(@"
var adsf = {
  ab|c: 'abc'
  ghi: 'ghi'
}
", @"
var adsf = {
  'Before prop!': 'Before Value'
  abc: 'abc'
  'After prop!': 'After Value'
  ghi: 'ghi'
}
")]
    [DataRow(@"
var adsf = {
  abc: 'abc'
  ghi: '|ghi'
}
", @"
var adsf = {
  abc: 'abc'
  'Before prop!': 'Before Value'
  ghi: 'ghi'
  'After prop!': 'After Value'
}
")]
    [DataRow(@"
var adsf = { ab|c: 'abc', ghi: 'ghi' }
", @"
var adsf = { 'Before prop!': 'Before Value', abc: 'abc', 'After prop!': 'After Value', ghi: 'ghi' }
")]
    [DataRow(@"
var adsf = { abc: 'abc', ghi: 'g|hi' }
", @"
var adsf = { abc: 'abc', 'Before prop!': 'Before Value', ghi: 'ghi' , 'After prop!': 'After Value'}
")]
    // syntax with errors is not rewritten
    [DataRow(@"
var adsf = {
  abc: 'abc'
  def: 'de|f'
  ghi:
}
", @"
var adsf = {
  abc: 'abc'
  def: 'def'
  ghi:
}
")]
    public void TryAddProperty_adds_object_properties(string fileWithCursor, string expected)
    {
        var startProp = SyntaxFactory.CreateObjectProperty("Before prop!", SyntaxFactory.CreateStringLiteral("Before Value"));
        var endProp = SyntaxFactory.CreateObjectProperty("After prop!", SyntaxFactory.CreateStringLiteral("After Value"));

        var rewritten = RewriteProgram(
            fileWithCursor,
            (program, nodes, parsingErrorLookup) =>
            {
                var @object = GetLastNode<ObjectSyntax>(nodes);
                var prevProperty = GetLastNode<ObjectPropertySyntax>(nodes);
                var prevIndex = @object.Properties.IndexOf(x => x == prevProperty);

                return CallbackRewriter.Rewrite(
                    program,
                    node => (node == @object &&
                      SyntaxModifier.TryAddProperty(@object, startProp, parsingErrorLookup, prevIndex) is { } newObject &&
                      SyntaxModifier.TryAddProperty(newObject, endProp, parsingErrorLookup, prevIndex + 2) is { } newObject2)
                      ? newObject2 : node);
            });

        rewritten.Should().BeEquivalentToIgnoringNewlines(expected);
    }
}
