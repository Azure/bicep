namespace Bicep.Core.Syntax
{
    public enum BinaryOperation
    {
        // booleanOr
        Or,
        // booleanAnd
        And,
        // equality
        Equals,
        NotEquals,
        EqualsInsensitive,
        NotEqualsInsensitive,
        // comparison
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        // addition
        Add,
        Subtract,
        // multiplication
        Multiply,
        Divide,
        Modulus,
    }
}