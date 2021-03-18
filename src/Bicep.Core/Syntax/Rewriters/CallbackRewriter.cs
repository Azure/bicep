// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.Core.Syntax.Rewriters
{
    /// <summary>
    /// Rewriter that allows use of a callback to rewrite any type of node.
    /// </summary>
    public class CallbackRewriter : SyntaxRewriteVisitor
    {
        private readonly Func<SyntaxBase, SyntaxBase> callback;

        public static TSyntax Rewrite<TSyntax>(TSyntax syntax, Func<SyntaxBase, SyntaxBase> callback)
            where TSyntax : SyntaxBase
        {
            var rewriter = new CallbackRewriter(callback);

            return rewriter.Rewrite(syntax);
        }

        /// <summary>
        /// Creates a new rewriter with the specified callback.
        /// </summary>
        /// <param name="callback">The rewrite callback function that will be invoked on each node.</param>
        private CallbackRewriter(Func<SyntaxBase, SyntaxBase> callback)
        {
            this.callback = callback;
        }

        protected override SyntaxBase RewriteInternal(SyntaxBase syntax)
        {
            return callback(base.RewriteInternal(syntax));
        }
    }
}

