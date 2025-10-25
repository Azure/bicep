// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.Syntax.Visitors
{
    public static class SyntaxAggregator
    {
        /// <summary>
        /// Walks the specified syntax subtree and aggregates the results of queries against it.
        /// </summary>
        /// <typeparam name="TAccumulate">the type of accumulated value per tree node</typeparam>
        /// <typeparam name="TResult">the type of the final result of the aggregation</typeparam>
        /// <param name="source">The starting tree node</param>
        /// <param name="seed">Initial value</param>
        /// <param name="function">The function that updates the accumulated value given the current value and a tree node</param>
        /// <param name="resultSelector">The function that converts the final accumulated value into a result.</param>
        /// <param name="continuationFunction">Optional function to stop processing based on current accumulated value. You can use it to stop processing when a condition is met by returning false. If null, all tree nodes will be processed.</param>
        /// <returns></returns>
        public static TResult Aggregate<TAccumulate, TResult>(
            SyntaxBase source,
            TAccumulate seed,
            Func<TAccumulate, SyntaxBase, TAccumulate> function,
            Func<TAccumulate, TResult> resultSelector,
            Func<TAccumulate, SyntaxBase, bool>? continuationFunction = null,
            bool useCst = false)
        {
            // default to "always continue processing"
            continuationFunction ??= (value, node) => true;

            if (useCst)
            {
                var visitor = new CstAccumulatingVisitor<TAccumulate>(seed, function, continuationFunction);
                visitor.Visit(source);

                return resultSelector(visitor.Value);
            }
            else
            {
                var visitor = new AstAccumulatingVisitor<TAccumulate>(seed, function, continuationFunction);
                visitor.Visit(source);

                return resultSelector(visitor.Value);
            }
        }

        public static IEnumerable<SyntaxBase> Aggregate(SyntaxBase source, Func<SyntaxBase, bool> collectFunc, bool useCst = false)
            => Aggregate(source, new List<SyntaxBase>(), (accumulated, current) =>
                {
                    if (collectFunc(current))
                    {
                        accumulated.Add(current);
                    }

                    return accumulated;
                },
                accumulated => accumulated,
                useCst: useCst);

        public static IEnumerable<TSyntax> AggregateByType<TSyntax>(SyntaxBase source, bool useCst = false)
            where TSyntax : SyntaxBase
            => Aggregate(source, syntax => syntax is TSyntax, useCst: useCst).OfType<TSyntax>();

        private class AstAccumulatingVisitor<TAccumulate> : AstVisitor
        {
            private readonly Func<TAccumulate, SyntaxBase, TAccumulate> function;

            private readonly Func<TAccumulate, SyntaxBase, bool> continuationFunction;

            public AstAccumulatingVisitor(TAccumulate seed, Func<TAccumulate, SyntaxBase, TAccumulate> function, Func<TAccumulate, SyntaxBase, bool> continuationFunction)
            {
                this.Value = seed;
                this.function = function;
                this.continuationFunction = continuationFunction;
            }

            public TAccumulate Value { get; private set; }

            protected override void VisitInternal(SyntaxBase syntax)
            {
                // process other nodes if further processing is required
                if (this.continuationFunction(this.Value, syntax))
                {
                    this.Value = this.function(this.Value, syntax);

                    base.VisitInternal(syntax);
                    return;
                }
            }
        }

        private class CstAccumulatingVisitor<TAccumulate> : CstVisitor
        {
            private readonly Func<TAccumulate, SyntaxBase, TAccumulate> function;

            private readonly Func<TAccumulate, SyntaxBase, bool> continuationFunction;

            public CstAccumulatingVisitor(TAccumulate seed, Func<TAccumulate, SyntaxBase, TAccumulate> function, Func<TAccumulate, SyntaxBase, bool> continuationFunction)
            {
                this.Value = seed;
                this.function = function;
                this.continuationFunction = continuationFunction;
            }

            public TAccumulate Value { get; private set; }

            protected override void VisitInternal(SyntaxBase syntax)
            {
                // process other nodes if further processing is required
                if (this.continuationFunction(this.Value, syntax))
                {
                    this.Value = this.function(this.Value, syntax);

                    base.VisitInternal(syntax);
                    return;
                }
            }
        }
    }
}

