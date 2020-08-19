using Bicep.Core.Syntax;

namespace Bicep.Core.Navigation
{
    public interface ISymbolReference
    {
        IdentifierSyntax Name { get; }
    }
}
