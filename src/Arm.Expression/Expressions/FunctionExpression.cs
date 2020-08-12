namespace Arm.Expression.Expressions
{
    public class FunctionExpression : LanguageExpression
    {
        public string Function { get; }

        public LanguageExpression[] Parameters { get; }

        public LanguageExpression[] Properties { get; }

        public FunctionExpression(string function, LanguageExpression[] parameters, LanguageExpression[] properties)
        {
            this.Function = function;
            this.Parameters = parameters;
            this.Properties = properties;
        }
    }
}