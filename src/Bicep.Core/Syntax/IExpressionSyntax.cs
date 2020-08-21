namespace Bicep.Core.Syntax
{
    /// <summary>
    /// This syntax node is involved in expressions.
    /// </summary>
    public interface IExpressionSyntax
    {
        /// <summary>
        /// Gets the type of the expression node.
        /// </summary>
        ExpressionKind ExpressionKind { get; }
    }
}
