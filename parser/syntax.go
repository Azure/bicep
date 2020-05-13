package parser

type UnaryOp int

const (
	UnaryOpNot UnaryOp = iota
	UnaryOpNegate
)

type BinaryOp int

const (
	BinaryOpOr BinaryOp = iota
	BinaryOpAnd
	BinaryOpEquals
	BinaryOpNotEquals
	BinaryOpEqualsInsensitive
	BinaryOpNotEqualsInsensitive
	BinaryOpLessThan
	BinaryOpLessThanOrEqual
	BinaryOpGreaterThan
	BinaryOpGreaterThanOrEqual
	BinaryOpAdd
	BinaryOpSubtract
	BinaryOpMultiply
	BinaryOpDivide
	BinaryOpModulus
)

type SyntaxVisitor interface {
	VisitArrayAccessSyntax(syntax ArrayAccessSyntax)
	VisitArraySyntax(syntax ArraySyntax)
	VisitBinaryOperationSyntax(syntax BinaryOperationSyntax)
	VisitBooleanLiteralSyntax(syntax BooleanLiteralSyntax)
	VisitFunctionCallSyntax(syntax FunctionCallSyntax)
	VisitGroupingSyntax(syntax GroupingSyntax)
	VisitIdentifierSyntax(syntax IdentifierSyntax)
	VisitInputDeclSyntax(syntax InputDeclSyntax)
	VisitNullLiteralSyntax(syntax NullLiteralSyntax)
	VisitNumericLiteralSyntax(syntax NumericLiteralSyntax)
	VisitObjectPropertySyntax(syntax ObjectPropertySyntax)
	VisitObjectSyntax(syntax ObjectSyntax)
	VisitOutputDeclSyntax(syntax OutputDeclSyntax)
	VisitProgramSyntax(syntax ProgramSyntax)
	VisitPropertyAccessSyntax(syntax PropertyAccessSyntax)
	VisitResourceDeclSyntax(syntax ResourceDeclSyntax)
	VisitStringSyntax(syntax StringSyntax)
	VisitUnaryOperationSyntax(syntax UnaryOperationSyntax)
	VisitVarDeclSyntax(syntax VarDeclSyntax)
	VisitErrorSyntax(syntax ErrorSyntax)
}

type Syntax interface {
	Accept(visitor SyntaxVisitor)
	GetPosition() *Position
}

type ArrayAccessSyntax struct {
	Parent      Syntax
	LeftSquare  Token
	Property    Syntax
	RightSquare Token
}

func (syntax ArrayAccessSyntax) Accept(visitor SyntaxVisitor) {
	visitor.VisitArrayAccessSyntax(syntax)
}
func (syntax ArrayAccessSyntax) GetPosition() *Position {
	return MakePosition(syntax.Parent, syntax.RightSquare)
}

type ArraySyntax struct {
	LeftSquare  Token
	Items       []Syntax
	RightSquare Token
}

func (syntax ArraySyntax) Accept(visitor SyntaxVisitor) {
	visitor.VisitArraySyntax(syntax)
}
func (syntax ArraySyntax) GetPosition() *Position {
	return MakePosition(syntax.LeftSquare, syntax.RightSquare)
}

type BinaryOperationSyntax struct {
	LeftExpression  Syntax
	Operator        Token
	RightExpression Syntax
	Operation       BinaryOp
}

func (syntax BinaryOperationSyntax) Accept(visitor SyntaxVisitor) {
	visitor.VisitBinaryOperationSyntax(syntax)
}
func (syntax BinaryOperationSyntax) GetPosition() *Position {
	return MakePosition(syntax.LeftExpression, syntax.RightExpression)
}

type BooleanLiteralSyntax struct {
	Literal Token
	Value   bool
}

func (syntax BooleanLiteralSyntax) Accept(visitor SyntaxVisitor) {
	visitor.VisitBooleanLiteralSyntax(syntax)
}
func (syntax BooleanLiteralSyntax) GetPosition() *Position {
	return MakePosition(syntax.Literal, syntax.Literal)
}

type FunctionCallSyntax struct {
	Parent     Syntax
	LeftParen  Token
	Arguments  []Syntax
	RightParen Token
}

func (syntax FunctionCallSyntax) Accept(visitor SyntaxVisitor) {
	visitor.VisitFunctionCallSyntax(syntax)
}
func (syntax FunctionCallSyntax) GetPosition() *Position {
	return MakePosition(syntax.Parent, syntax.RightParen)
}

type GroupingSyntax struct {
	LeftParen  Token
	Expression Syntax
	RightParen Token
}

func (syntax GroupingSyntax) Accept(visitor SyntaxVisitor) {
	visitor.VisitGroupingSyntax(syntax)
}
func (syntax GroupingSyntax) GetPosition() *Position {
	return MakePosition(syntax.LeftParen, syntax.RightParen)
}

type IdentifierSyntax struct {
	Identifier Token
}

func (syntax IdentifierSyntax) Accept(visitor SyntaxVisitor) {
	visitor.VisitIdentifierSyntax(syntax)
}
func (syntax IdentifierSyntax) GetPosition() *Position {
	return MakePosition(syntax.Identifier, syntax.Identifier)
}

type InputDeclSyntax struct {
	InputKeyword Token
	Type         IdentifierSyntax
	Identifier   IdentifierSyntax
	Semicolon    Token
}

func (syntax InputDeclSyntax) Accept(visitor SyntaxVisitor) {
	visitor.VisitInputDeclSyntax(syntax)
}
func (syntax InputDeclSyntax) GetPosition() *Position {
	return MakePosition(syntax.InputKeyword, syntax.Semicolon)
}

type NullLiteralSyntax struct {
	Literal Token
}

func (syntax NullLiteralSyntax) Accept(visitor SyntaxVisitor) {
	visitor.VisitNullLiteralSyntax(syntax)
}
func (syntax NullLiteralSyntax) GetPosition() *Position {
	return MakePosition(syntax.Literal, syntax.Literal)
}

type NumericLiteralSyntax struct {
	Literal Token
	Value   int
}

func (syntax NumericLiteralSyntax) Accept(visitor SyntaxVisitor) {
	visitor.VisitNumericLiteralSyntax(syntax)
}
func (syntax NumericLiteralSyntax) GetPosition() *Position {
	return MakePosition(syntax.Literal, syntax.Literal)
}

type ObjectPropertySyntax struct {
	Identifier IdentifierSyntax
	Colon      Token
	Expression Syntax
}

func (syntax ObjectPropertySyntax) Accept(visitor SyntaxVisitor) {
	visitor.VisitObjectPropertySyntax(syntax)
}
func (syntax ObjectPropertySyntax) GetPosition() *Position {
	return MakePosition(syntax.Identifier, syntax.Expression)
}

type ObjectSyntax struct {
	LeftBrace  Token
	Properties []ObjectPropertySyntax
	RightBrace Token
}

func (syntax ObjectSyntax) Accept(visitor SyntaxVisitor) {
	visitor.VisitObjectSyntax(syntax)
}
func (syntax ObjectSyntax) GetPosition() *Position {
	return MakePosition(syntax.LeftBrace, syntax.RightBrace)
}

type OutputDeclSyntax struct {
	OutputKeyword Token
	Identifier    IdentifierSyntax
	Colon         Token
	Expression    Syntax
	Semicolon     Token
}

func (syntax OutputDeclSyntax) Accept(visitor SyntaxVisitor) {
	visitor.VisitOutputDeclSyntax(syntax)
}
func (syntax OutputDeclSyntax) GetPosition() *Position {
	return MakePosition(syntax.OutputKeyword, syntax.Semicolon)
}

type ProgramSyntax struct {
	Statements []Syntax
}

func (syntax ProgramSyntax) Accept(visitor SyntaxVisitor) {
	visitor.VisitProgramSyntax(syntax)
}
func (syntax ProgramSyntax) GetPosition() *Position {
	length := len(syntax.Statements)
	if length == 0 {
		return &Position{
			start: 0,
			end:   0,
		}
	}

	return MakePosition(syntax.Statements[0], syntax.Statements[length-1])
}

type PropertyAccessSyntax struct {
	Parent   Syntax
	Dot      Token
	Property Syntax
}

func (syntax PropertyAccessSyntax) Accept(visitor SyntaxVisitor) {
	visitor.VisitPropertyAccessSyntax(syntax)
}
func (syntax PropertyAccessSyntax) GetPosition() *Position {
	return MakePosition(syntax.Parent, syntax.Property)
}

type ResourceDeclSyntax struct {
	ResourceKeyword Token
	Provider        IdentifierSyntax
	Type            StringSyntax
	Identifier      IdentifierSyntax
	Colon           Token
	Expression      Syntax
	Semicolon       Token
}

func (syntax ResourceDeclSyntax) Accept(visitor SyntaxVisitor) {
	visitor.VisitResourceDeclSyntax(syntax)
}
func (syntax ResourceDeclSyntax) GetPosition() *Position {
	return MakePosition(syntax.ResourceKeyword, syntax.Semicolon)
}

type StringSyntax struct {
	StringToken Token
}

func (syntax StringSyntax) Accept(visitor SyntaxVisitor) {
	visitor.VisitStringSyntax(syntax)
}
func (syntax StringSyntax) GetPosition() *Position {
	return MakePosition(syntax.StringToken, syntax.StringToken)
}

type UnaryOperationSyntax struct {
	Operator   Token
	Expression Syntax
	Operation  UnaryOp
}

func (syntax UnaryOperationSyntax) Accept(visitor SyntaxVisitor) {
	visitor.VisitUnaryOperationSyntax(syntax)
}
func (syntax UnaryOperationSyntax) GetPosition() *Position {
	return MakePosition(syntax.Operator, syntax.Expression)
}

type VarDeclSyntax struct {
	VariableKeyword Token
	Identifier      IdentifierSyntax
	Colon           Token
	Expression      Syntax
	Semicolon       Token
}

func (syntax VarDeclSyntax) Accept(visitor SyntaxVisitor) {
	visitor.VisitVarDeclSyntax(syntax)
}
func (syntax VarDeclSyntax) GetPosition() *Position {
	return MakePosition(syntax.VariableKeyword, syntax.Semicolon)
}

type ErrorSyntax struct {
	Tokens []Token
}

func (syntax ErrorSyntax) Accept(visitor SyntaxVisitor) {
	visitor.VisitErrorSyntax(syntax)
}
func (syntax ErrorSyntax) GetPosition() *Position {
	return MakePosition(syntax.Tokens[0], syntax.Tokens[len(syntax.Tokens)-1])
}
