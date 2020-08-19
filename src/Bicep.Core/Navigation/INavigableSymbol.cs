namespace Bicep.Core.Navigation
{
    /// <summary>
    /// Represents a symbol that can be navigated to in the editor.
    /// </summary>
    /// <remarks>This is intended for symbols that exist in source code and can be navigated to.
    /// (Function and namespace symbols cannot currently be navigated to because they do not exist in the source code.)</remarks>
    public interface INavigableSymbol
    {
    }
}
