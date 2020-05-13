package main

import (
	"fmt"

	"github.com/Azure/bicep/parser"
)

type PrintSyntaxVisitor struct {
	indent int
}

func (visitor PrintSyntaxVisitor) Visit(syntax parser.Syntax) {
	syntax.Accept(visitor)
}

func (visitor PrintSyntaxVisitor) VisitToken(token parser.Token) {
	fmt.Print(token.RawValue, " ")
}

func (visitor PrintSyntaxVisitor) newLine() {
	fmt.Print("\n")
	for i := 0; i < visitor.indent; i++ {
		fmt.Print("  ")
	}
}

func (visitor PrintSyntaxVisitor) VisitArrayAccessSyntax(syntax parser.ArrayAccessSyntax) {
	visitor.Visit(syntax.Parent)
	visitor.VisitToken(syntax.LeftSquare)
	visitor.Visit(syntax.Property)
	visitor.VisitToken(syntax.RightSquare)
}

func (visitor PrintSyntaxVisitor) VisitArraySyntax(syntax parser.ArraySyntax) {
	visitor.VisitToken(syntax.LeftSquare)
	visitor.indent++
	visitor.newLine()
	for _, item := range syntax.Items {
		visitor.Visit(item)
		fmt.Print(",") // todo capture this token
		visitor.newLine()
	}
	visitor.indent--
	visitor.VisitToken(syntax.RightSquare)
}

func (visitor PrintSyntaxVisitor) VisitBinaryOperationSyntax(syntax parser.BinaryOperationSyntax) {
	visitor.Visit(syntax.LeftExpression)
	visitor.VisitToken(syntax.Operator)
	visitor.Visit(syntax.RightExpression)
}

func (visitor PrintSyntaxVisitor) VisitBooleanLiteralSyntax(syntax parser.BooleanLiteralSyntax) {
	visitor.VisitToken(syntax.Literal)
}

func (visitor PrintSyntaxVisitor) VisitFunctionCallSyntax(syntax parser.FunctionCallSyntax) {
	visitor.Visit(syntax.Parent)
	visitor.VisitToken(syntax.LeftParen)
	for i, argument := range syntax.Arguments {
		visitor.Visit(argument)
		if i+1 < len(syntax.Arguments) {
			fmt.Print(",") // todo capture this token
		}
	}
	visitor.VisitToken(syntax.RightParen)
}

func (visitor PrintSyntaxVisitor) VisitGroupingSyntax(syntax parser.GroupingSyntax) {
	visitor.VisitToken(syntax.LeftParen)
	visitor.Visit(syntax.Expression)
	visitor.VisitToken(syntax.RightParen)
}

func (visitor PrintSyntaxVisitor) VisitIdentifierSyntax(syntax parser.IdentifierSyntax) {
	visitor.VisitToken(syntax.Identifier)
}

func (visitor PrintSyntaxVisitor) VisitInputDeclSyntax(syntax parser.InputDeclSyntax) {
	visitor.VisitToken(syntax.InputKeyword)
	visitor.Visit(syntax.Type)
	visitor.Visit(syntax.Identifier)
	visitor.VisitToken(syntax.Semicolon)
	visitor.newLine()
}

func (visitor PrintSyntaxVisitor) VisitNullLiteralSyntax(syntax parser.NullLiteralSyntax) {
	visitor.VisitToken(syntax.Literal)
}

func (visitor PrintSyntaxVisitor) VisitNumericLiteralSyntax(syntax parser.NumericLiteralSyntax) {
	visitor.VisitToken(syntax.Literal)
}

func (visitor PrintSyntaxVisitor) VisitObjectPropertySyntax(syntax parser.ObjectPropertySyntax) {
	visitor.Visit(syntax.Identifier)
	visitor.VisitToken(syntax.Colon)
	visitor.Visit(syntax.Expression)
}

func (visitor PrintSyntaxVisitor) VisitObjectSyntax(syntax parser.ObjectSyntax) {
	visitor.VisitToken(syntax.LeftBrace)
	visitor.indent++
	visitor.newLine()
	for _, property := range syntax.Properties {
		visitor.Visit(property)
		fmt.Print(",") // todo capture this token
		visitor.newLine()
	}
	visitor.indent--
	visitor.VisitToken(syntax.RightBrace)
}

func (visitor PrintSyntaxVisitor) VisitOutputDeclSyntax(syntax parser.OutputDeclSyntax) {
	visitor.VisitToken(syntax.OutputKeyword)
	visitor.Visit(syntax.Identifier)
	visitor.VisitToken(syntax.Colon)
	visitor.Visit(syntax.Expression)
	visitor.VisitToken(syntax.Semicolon)
	visitor.newLine()
}

func (visitor PrintSyntaxVisitor) VisitProgramSyntax(syntax parser.ProgramSyntax) {
	for _, statement := range syntax.Statements {
		visitor.Visit(statement)
	}
}

func (visitor PrintSyntaxVisitor) VisitPropertyAccessSyntax(syntax parser.PropertyAccessSyntax) {
	visitor.Visit(syntax.Parent)
	visitor.VisitToken(syntax.Dot)
	visitor.Visit(syntax.Property)
}

func (visitor PrintSyntaxVisitor) VisitResourceDeclSyntax(syntax parser.ResourceDeclSyntax) {
	visitor.VisitToken(syntax.ResourceKeyword)
	visitor.Visit(syntax.Provider)
	visitor.Visit(syntax.Type)
	visitor.Visit(syntax.Identifier)
	visitor.VisitToken(syntax.Colon)
	visitor.Visit(syntax.Expression)
	visitor.VisitToken(syntax.Semicolon)
	visitor.newLine()
}

func (visitor PrintSyntaxVisitor) VisitStringSyntax(syntax parser.StringSyntax) {
	visitor.VisitToken(syntax.StringToken)
}

func (visitor PrintSyntaxVisitor) VisitUnaryOperationSyntax(syntax parser.UnaryOperationSyntax) {
	visitor.VisitToken(syntax.Operator)
	visitor.Visit(syntax.Expression)
}

func (visitor PrintSyntaxVisitor) VisitVarDeclSyntax(syntax parser.VarDeclSyntax) {
	visitor.VisitToken(syntax.VariableKeyword)
	visitor.Visit(syntax.Identifier)
	visitor.VisitToken(syntax.Colon)
	visitor.Visit(syntax.Expression)
	visitor.VisitToken(syntax.Semicolon)
	visitor.newLine()
}

func (visitor PrintSyntaxVisitor) VisitErrorSyntax(syntax parser.ErrorSyntax) {
	prevIndent := visitor.indent
	visitor.indent = 0
	fmt.Print("[ERR] ")
	for _, token := range syntax.Tokens {
		visitor.VisitToken(token)
	}
	fmt.Print("[/ERR]")
	visitor.newLine()
	visitor.indent = prevIndent
}
