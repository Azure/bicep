// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.Text;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Bicep.LangServer.IntegrationTests.Assertions;
using Bicep.LangServer.IntegrationTests.Extensions;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SymbolKind = Bicep.Core.Semantics.SymbolKind;

namespace Bicep.LangServer.IntegrationTests;

[TestClass]
public class ReferencesTests : TestBase
{
    private static readonly SharedLanguageHelperManager DefaultServer = new();

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
        DefaultServer.Initialize(async () => await MultiFileLanguageServerHelper.StartLanguageServer(testContext));
    }

    [ClassCleanup]
    public static async Task ClassCleanup()
    {
        await DefaultServer.DisposeAsync();
    }

    [TestMethod]
    [DataRow("""
var te|st = 'asdf'

var asdf = test
var blah = '${test}'
""", """
1| var test = 'asdf'
       ~~~~ here
2|
3| var asdf = test
              ~~~~ here
4| var blah = '${test}'
                 ~~~~ here

""")]
    public async Task FindReferencesWithDeclarationsShouldProduceCorrectResults(string inputWithCursor, string outputAnnotated)
    {
        var (contents, cursor) = ParserHelper.GetFileWithSingleCursor(inputWithCursor);
        var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(contents);

        var locations = await file.RequestReferences(cursor, includeDeclaration: true);

        // all URIs should be the same in the results
        locations!.Select(r => r.Uri.ToUriEncoded()).Should().AllBeEquivalentTo(file.Source.FileUri);

        AnnotateWithLocations(file, locations).Should().EqualIgnoringTrailingWhitespace(outputAnnotated);
    }

    [TestMethod]
    [DataRow("""
var te|st = 'asdf'

var asdf = test
var blah = '${test}'
""", """
2|
3| var asdf = test
              ~~~~ here
4| var blah = '${test}'
                 ~~~~ here

""")]
    public async Task FindReferencesWithoutDeclarationsShouldProduceCorrectResults(string inputWithCursor, string outputAnnotated)
    {
        var (contents, cursor) = ParserHelper.GetFileWithSingleCursor(inputWithCursor);
        var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(contents);

        var locations = await file.RequestReferences(cursor, includeDeclaration: false);

        // all URIs should be the same in the results
        locations!.Select(r => r.Uri.ToUriEncoded()).Should().AllBeEquivalentTo(file.Source.FileUri);

        AnnotateWithLocations(file, locations).Should().EqualIgnoringTrailingWhitespace(outputAnnotated);
    }

    [TestMethod]
    [DataRow("""
var test = 'asdf'
|
var asdf = test
var blah = '${test}'
""")]
    public async Task FindReferencesOnNonSymbolsShouldProduceEmptyResult(string inputWithCursor)
    {
        var (contents, cursor) = ParserHelper.GetFileWithSingleCursor(inputWithCursor);
        var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(contents);

        var locations = await file.RequestReferences(cursor, includeDeclaration: false);

        // all URIs should be the same in the results
        locations.Should().BeNullOrEmpty();
    }

    [TestMethod]
    public async Task FindReferences_displays_references_on_namespaced_and_non_namespaced_methods()
    {
        var (contents, cursors) = ParserHelper.GetFileWithCursors(@"
var rg1 = resourc|eGroup().location

var rg2 = az.r|esourceGroup()

var rg3 = resourceGr|oup().id

var dep1 = az.depl|oyment()

var dep2 = az.deploy|ment()
",
            '|');

        var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(contents);
        var references = await RequestReferences(file, cursors);

        references.Should().SatisfyRespectively(
            r => r.Should().HaveCount(3),
            r => r.Should().HaveCount(3),
            r => r.Should().HaveCount(3),
            r => r.Should().HaveCount(2),
            r => r.Should().HaveCount(2));
    }

    private static string AnnotateWithLocations(FileRequestHelper file, LocationContainer? locations)
        => PrintHelper.PrintWithAnnotations(
            file.Source,
            locations!.Select(x => new PrintHelper.Annotation(Assertions.AssertionScopeExtensions.FromRange(file.Source, x.Range), "here")), 1, true);

    private static async Task<IEnumerable<LocationContainer?>> RequestReferences(FileRequestHelper file, IEnumerable<int> cursors)
    {
        var references = new List<LocationContainer?>();
        foreach (var cursor in cursors)
        {
            var referenceList = await file.RequestReferences(cursor, includeDeclaration: false);
            references.Add(referenceList);
        }

        return references;
    }
}
