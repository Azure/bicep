// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public class DeclaredTypeAssignment
    {
        public DeclaredTypeAssignment(ITypeReference reference, SyntaxBase? declaringSyntax, DeclaredTypeFlags flags)
        {
            this.Reference = reference;
            this.DeclaringSyntax = declaringSyntax;
            this.Flags = flags;
        }

        public DeclaredTypeAssignment(ITypeReference reference, SyntaxBase? declaringSyntax)
            : this(reference, declaringSyntax, DeclaredTypeFlags.None)
        {
        }

        public ITypeReference Reference { get; }

        public DeclaredTypeFlags Flags { get; }

        /// <summary>
        /// Gets the node whose declared type this assignment represents. This is primarily used to resolve nested discriminated object types.
        /// </summary>
        /// <remarks>When declared type is requested for a node that is part of a declaration body (INamedDeclarationSyntax, ObjectSyntax, ArraySyntax,
        /// ObjectPropertySyntax, etc.), you may see DeclaringSyntax to be set to the same node. When requesting declared type for a VariableAccessSyntax,
        /// PropertyAccessSyntax or ArrayAccessSyntax node, it will point to a corresponding node within the referenced declaration. The value may
        /// be null if there is not enough information in the declaration body.</remarks>
        public SyntaxBase? DeclaringSyntax { get; }

        public DeclaredTypeAssignment ReplaceDeclaringSyntax(SyntaxBase? newSyntax) => new DeclaredTypeAssignment(this.Reference, newSyntax, this.Flags);
    }
}