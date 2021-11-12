// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Collections.Immutable;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.Utils;
using Bicep.Core.Navigation;

namespace Bicep.LangServer.IntegrationTests
{
    public static class IntegrationTestHelper
    {
        private const int DefaultTimeout = 30000;


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "Not an issue in test code.")]
        public static async Task<T> WithTimeoutAsync<T>(Task<T> task, int timeout = DefaultTimeout)
        {
            var completed = await Task.WhenAny(
                task,
                Task.Delay(timeout)
            );

            if (task != completed)
            {
                Assert.Fail($"Timed out waiting for task to complete after {timeout}ms");
            }

            return await task;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "Not an issue in test code.")]
        public static async Task EnsureTaskDoesntCompleteAsync<T>(Task<T> task, int timeout = DefaultTimeout)
        {
            var completed = await Task.WhenAny(
                task,
                Task.Delay(timeout)
            );

            if (task == completed)
            {
                Assert.Fail($"Expected task to not complete, but it completed!");
            }
        }

        public static Position GetPosition(ImmutableArray<int> lineStarts, SyntaxBase syntax)
        {
            if (syntax is ISymbolReference reference)
            {
                // get identifier span otherwise syntax.Span returns the position from the starting position of the whole expression.
                // e.g. in an instance function call such as: az.resourceGroup(), syntax.Span position starts at 'az',
                // whereas instanceFunctionCall.Name.Span the position will start in resourceGroup() which is what it should be in this
                // case.
                return PositionHelper.GetPosition(lineStarts, reference.Name.Span.Position);
            }

            if (syntax is ITopLevelDeclarationSyntax declaration)
            {
                return PositionHelper.GetPosition(lineStarts, declaration.Keyword.Span.Position);
            }

            return PositionHelper.GetPosition(lineStarts, syntax.Span.Position);
        }
    }
}
