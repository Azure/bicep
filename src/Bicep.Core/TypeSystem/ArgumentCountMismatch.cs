// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.Core.TypeSystem
{
    public class ArgumentCountMismatch
    {
        public ArgumentCountMismatch(int argumentCount, int minimumArgumentCount, int? maximumArgumentCount)
        {
            this.ArgumentCount = argumentCount;
            this.MinimumArgumentCount = minimumArgumentCount;
            this.MaximumArgumentCount = maximumArgumentCount;
        }

        public int ArgumentCount { get; }

        public int MinimumArgumentCount { get; }

        public int? MaximumArgumentCount { get; }

        public void Deconstruct(out int argumentCount, out int minimumArgumentCount, out int? maximumArgumentCount)
        {
            argumentCount = this.ArgumentCount;
            minimumArgumentCount = this.MinimumArgumentCount;
            maximumArgumentCount = this.MaximumArgumentCount;
        }

        public static ArgumentCountMismatch Reduce(ArgumentCountMismatch first, ArgumentCountMismatch second)
        {
            if (first.ArgumentCount != second.ArgumentCount)
            {
                throw new ArgumentException($"Cannot merge two ArgumentCountMismatch instances with different ArgumentCount values: {first.ArgumentCount}, {second.ArgumentCount}.");
            }

            int minimumArgumentCount = Math.Min(first.MinimumArgumentCount, second.MinimumArgumentCount);
            int? maximumArgumentCount = first.MaximumArgumentCount.HasValue && second.MaximumArgumentCount.HasValue
                ? Math.Max(first.MaximumArgumentCount.Value, second.MaximumArgumentCount.Value)
                : first.MaximumArgumentCount;

            return new ArgumentCountMismatch(first.ArgumentCount, minimumArgumentCount, maximumArgumentCount);
        }
    }
}

