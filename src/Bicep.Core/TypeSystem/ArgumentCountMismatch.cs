// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.TypeSystem
{
    public class ArgumentCountMismatch(int argumentCount, int minimumArgumentCount, int? maximumArgumentCount)
    {
        public int ArgumentCount { get; } = argumentCount;

        public int MinimumArgumentCount { get; } = minimumArgumentCount;

        public int? MaximumArgumentCount { get; } = maximumArgumentCount;

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

