using System;
using System.Collections.Generic;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;

namespace Bicep.Core.Visitors
{
    public abstract class ErrorVisitor : SyntaxVisitor
    {
        private IList<Error> Errors { get; } = new List<Error>();

        protected void AddError(IPositionable positionable, string message)
        {
            Errors.Add(new Error(message, positionable.Span));
        }

        protected ErrorVisitor(Scope globalScope)
        {
            CurrentScope = globalScope;
        }

        protected Scope CurrentScope { get; }
    }
}