package main

import (
	"os"

	"github.com/Azure/bicep/parser"
)

func main() {
	file, _ := os.Open(os.Args[1])
	lexer := parser.Lex(file)
	test := parser.CreateParser(lexer.Tokens)
	syntax := test.Parse()

	visitor := PrintSyntaxVisitor{
		indent: 0,
	}
	visitor.Visit(syntax)
}
