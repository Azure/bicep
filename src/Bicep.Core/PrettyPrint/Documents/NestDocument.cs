// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Text;
using Bicep.Core.PrettyPrint.Documents;

namespace Bicep.Core.PrettyPrint.DocumentCombinators
{
    public class NestDocument : ILinkedDocument
    {
        public NestDocument(int level, ILinkedDocument successor)
        {
            this.Level = level;
            this.Successor = successor;
        }

        public int Level { get; }

        public ILinkedDocument Successor { get; }

        public ILinkedDocument Concat(ILinkedDocument other)
        {
            return new NestDocument(this.Level, this.Successor.Concat(other));
        }

        public ILinkedDocument Nest(int level)
        {
            return new NestDocument(level + this.Level, this.Successor.Nest(level));
        }

        public void Layout(StringBuilder sb, string indent, string newline)
        {
            sb.Append(newline);

            if (!(this.Successor is NestDocument))
            {
                for (int i = 0; i < this.Level; i++)
                {
                    sb.Append(indent);
                }
            }
            
            this.Successor.Layout(sb, indent, newline);
        }
    }
}
