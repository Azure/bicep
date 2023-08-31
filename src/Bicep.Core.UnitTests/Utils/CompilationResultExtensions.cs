// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static Bicep.Core.UnitTests.Utils.CompilationHelper;

namespace Bicep.Core.UnitTests.Utils
{
    public static class CompilationResultExtensions
    {
        public static CursorLookupResult GetInfoAtCursor(this CompilationResult result, int cursor)
        {
            var model = result.Compilation.GetEntrypointSemanticModel();

            var node = result.BicepFile.ProgramSyntax.TryFindMostSpecificNodeInclusive(
                cursor,
                n => n is not IdentifierSyntax && n is not Token);

            node.Should().NotBeNull();

            var symbol = model.GetSymbolInfo(node!);
            var type = model.GetTypeInfo(node!);

            symbol.Should().NotBeNull();

            return new(node!, symbol!, type);
        }

        public static ImmutableArray<CursorLookupResult> GetInfoAtCursors(this CompilationResult result, IEnumerable<int> cursors)
            => cursors.Select(x => GetInfoAtCursor(result, x)).ToImmutableArray();
    }
}
