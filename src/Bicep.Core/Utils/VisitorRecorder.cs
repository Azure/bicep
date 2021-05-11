// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Bicep.Core.Utils
{
    public class VisitorRecorder<T> : IEnumerable<T> where T : notnull
    {
        private readonly Stack<T> visitingStack = new();

        public IDisposable Scope(T element)
        {
            return new StackScope(visitingStack, element);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)visitingStack).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)visitingStack).GetEnumerator();
        }

        private class StackScope : IDisposable
        {
            private readonly Stack<T> stack;
            private readonly T element;

            public StackScope(Stack<T> stack, T element)
            {
                this.stack = stack;
                this.element = element;
                this.stack.Push(this.element);
            }
            public void Dispose()
            {
                var popped = stack.Pop();
                if (!popped.Equals(element))
                {
                    //this error is thrown only if we forgot to implement pop in the visitor or we implement it wrong (e.g. double pop)
                    throw new InvalidOperationException($"Unexpected element on visited stack. Expecting {element}, got {popped}");
                }
            }
        }

        public bool TryPeek(out T result) => visitingStack.TryPeek(out result);
    }
}
