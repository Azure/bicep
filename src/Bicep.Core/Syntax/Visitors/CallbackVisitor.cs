using System;

namespace Bicep.Core.Syntax.Visitors
{
    /// <summary>
    /// Visitor that executes a callback before visiting a tree node.
    /// </summary>
    public class CallbackVisitor : SyntaxVisitor
    {
        private readonly Func<SyntaxBase, bool> callback;

        /// <summary>
        /// Creates a new visitor with the specified callback.
        /// </summary>
        /// <param name="callback">The callback function that will be invoked before visiting a node. Return true to visit the node (the callback will be invoked on each child node). Return false to skip this node and its children.</param>
        public CallbackVisitor(Func<SyntaxBase, bool> callback)
        {
            this.callback = callback;
        }

        protected override void VisitInternal(SyntaxBase node)
        {
            if (this.callback(node))
            {
                base.VisitInternal(node);
            }
        }
    }
}
