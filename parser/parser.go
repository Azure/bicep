package parser

import (
	"errors"
	"strconv"
)

type Parser struct {
	current int
	tokens  []Token
	errors  []Error
}

func CreateParser(tokens []Token) *Parser {
	parser := &Parser{
		current: 0,
		tokens:  tokens,
		errors:  make([]Error, 0),
	}

	return parser
}

func (parser *Parser) Parse() Syntax {
	return parser.parseProgram()
}

func (parser *Parser) parseProgram() ProgramSyntax {
	var statements []Syntax
	for !parser.isAtEnd() {
		statements = append(statements, parser.parseStatement())
	}

	return ProgramSyntax{
		Statements: statements,
	}
}

func (parser *Parser) parseStatement() Syntax {
	var nextType = parser.read().tokenType
	switch nextType {
	case TokenTypeInputKeyword:
		return parser.parseInputStatement()
	case TokenTypeOutputKeyword:
		return parser.parseOutputStatement()
	case TokenTypeVariableKeyword:
		return parser.parseVariableStatement()
	case TokenTypeResourceKeyword:
		return parser.parseResourceStatement()
	}

	parser.emitError("Unexpected token")
	return nil
}

func (parser *Parser) parseInputStatement() Syntax {
	return parser.doWithRecovery(parser.current-1, TokenTypeSemicolon, func() (Syntax, error) {
		keyword := parser.prev()
		inputType := parser.parseIdentifier()
		identifier := parser.parseIdentifier()
		semicolon := parser.expect(TokenTypeSemicolon)

		return InputDeclSyntax{
			InputKeyword: keyword,
			Type:         inputType,
			Identifier:   identifier,
			Semicolon:    semicolon,
		}, nil
	})
}

func (parser *Parser) parseOutputStatement() Syntax {
	return parser.doWithRecovery(parser.current-1, TokenTypeSemicolon, func() (Syntax, error) {
		keyword := parser.prev()
		identifier := parser.parseIdentifier()
		colon := parser.expect(TokenTypeColon)
		expression, err := parser.parseExpression()
		if err != nil {
			return nil, err
		}
		semicolon := parser.expect(TokenTypeSemicolon)

		return OutputDeclSyntax{
			OutputKeyword: keyword,
			Identifier:    identifier,
			Colon:         colon,
			Expression:    expression,
			Semicolon:     semicolon,
		}, nil
	})
}

func (parser *Parser) parseVariableStatement() Syntax {
	return parser.doWithRecovery(parser.current-1, TokenTypeSemicolon, func() (Syntax, error) {
		keyword := parser.prev()
		identifier := parser.parseIdentifier()
		colon := parser.expect(TokenTypeColon)
		expression, err := parser.parseExpression()
		if err != nil {
			return nil, err
		}
		semicolon := parser.expect(TokenTypeSemicolon)

		return VarDeclSyntax{
			VariableKeyword: keyword,
			Identifier:      identifier,
			Colon:           colon,
			Expression:      expression,
			Semicolon:       semicolon,
		}, nil
	})
}

func (parser *Parser) parseResourceStatement() Syntax {
	return parser.doWithRecovery(parser.current-1, TokenTypeSemicolon, func() (Syntax, error) {
		keyword := parser.prev()
		provider := parser.parseIdentifier()
		typeString := parser.parseString()
		identifier := parser.parseIdentifier()
		colon := parser.expect(TokenTypeColon)
		expression, err := parser.parseExpression()
		if err != nil {
			return nil, err
		}
		semicolon := parser.expect(TokenTypeSemicolon)

		return ResourceDeclSyntax{
			ResourceKeyword: keyword,
			Provider:        provider,
			Type:            typeString,
			Identifier:      identifier,
			Colon:           colon,
			Expression:      expression,
			Semicolon:       semicolon,
		}, nil
	})
}

func (parser *Parser) parseString() StringSyntax {
	stringToken := parser.expect(TokenTypeString)

	return StringSyntax{
		StringToken: stringToken,
	}
}

func (parser *Parser) parseIdentifier() IdentifierSyntax {
	identifier := parser.expect(TokenTypeIdentifier)

	return IdentifierSyntax{
		Identifier: identifier,
	}
}

func (parser *Parser) parseExpression() (Syntax, error) {
	return parser.parseOr()
}

func (parser *Parser) parseOr() (Syntax, error) {
	expression, err := parser.parseAnd()
	if err != nil {
		return nil, err
	}

	if parser.match(TokenTypeBinaryOr) {
		var operator = parser.prev()
		rightExpression, err := parser.parseOr()
		if err != nil {
			return nil, err
		}
		return BinaryOperationSyntax{
			LeftExpression:  expression,
			Operator:        operator,
			RightExpression: rightExpression,
			Operation:       BinaryOpOr,
		}, nil
	}

	return expression, nil
}

func (parser *Parser) parseAnd() (Syntax, error) {
	expression, err := parser.parseEquality()
	if err != nil {
		return nil, err
	}

	if parser.match(TokenTypeBinaryOr) {
		var operator = parser.prev()
		rightExpression, err := parser.parseAnd()
		if err != nil {
			return nil, err
		}
		return BinaryOperationSyntax{
			LeftExpression:  expression,
			Operator:        operator,
			RightExpression: rightExpression,
			Operation:       BinaryOpAnd,
		}, nil
	}

	return expression, nil
}

func (parser *Parser) parseEquality() (Syntax, error) {
	expression, err := parser.parseComparison()
	if err != nil {
		return nil, err
	}

	if parser.match(TokenTypeEquals) {
		var operator = parser.prev()
		rightExpression, err := parser.parseEquality()
		if err != nil {
			return nil, err
		}
		return BinaryOperationSyntax{
			LeftExpression:  expression,
			Operator:        operator,
			RightExpression: rightExpression,
			Operation:       BinaryOpEquals,
		}, nil
	}
	if parser.match(TokenTypeNotEquals) {
		var operator = parser.prev()
		rightExpression, err := parser.parseEquality()
		if err != nil {
			return nil, err
		}
		return BinaryOperationSyntax{
			LeftExpression:  expression,
			Operator:        operator,
			RightExpression: rightExpression,
			Operation:       BinaryOpNotEquals,
		}, nil
	}
	if parser.match(TokenTypeEqualsInsensitive) {
		var operator = parser.prev()
		rightExpression, err := parser.parseEquality()
		if err != nil {
			return nil, err
		}
		return BinaryOperationSyntax{
			LeftExpression:  expression,
			Operator:        operator,
			RightExpression: rightExpression,
			Operation:       BinaryOpEqualsInsensitive,
		}, nil
	}
	if parser.match(TokenTypeNotEqualsInsensitive) {
		var operator = parser.prev()
		rightExpression, err := parser.parseEquality()
		if err != nil {
			return nil, err
		}
		return BinaryOperationSyntax{
			LeftExpression:  expression,
			Operator:        operator,
			RightExpression: rightExpression,
			Operation:       BinaryOpNotEqualsInsensitive,
		}, nil
	}

	return expression, nil
}

func (parser *Parser) parseComparison() (Syntax, error) {
	expression, err := parser.parseAddition()
	if err != nil {
		return nil, err
	}

	if parser.match(TokenTypeGreaterThan) {
		var operator = parser.prev()
		rightExpression, err := parser.parseComparison()
		if err != nil {
			return nil, err
		}
		return BinaryOperationSyntax{
			LeftExpression:  expression,
			Operator:        operator,
			RightExpression: rightExpression,
			Operation:       BinaryOpGreaterThan,
		}, nil
	}
	if parser.match(TokenTypeGreaterThanOrEqual) {
		var operator = parser.prev()
		rightExpression, err := parser.parseComparison()
		if err != nil {
			return nil, err
		}
		return BinaryOperationSyntax{
			LeftExpression:  expression,
			Operator:        operator,
			RightExpression: rightExpression,
			Operation:       BinaryOpGreaterThanOrEqual,
		}, nil
	}
	if parser.match(TokenTypeLessThan) {
		var operator = parser.prev()
		rightExpression, err := parser.parseComparison()
		if err != nil {
			return nil, err
		}
		return BinaryOperationSyntax{
			LeftExpression:  expression,
			Operator:        operator,
			RightExpression: rightExpression,
			Operation:       BinaryOpLessThan,
		}, nil
	}
	if parser.match(TokenTypeLessThanOrEqual) {
		var operator = parser.prev()
		rightExpression, err := parser.parseComparison()
		if err != nil {
			return nil, err
		}
		return BinaryOperationSyntax{
			LeftExpression:  expression,
			Operator:        operator,
			RightExpression: rightExpression,
			Operation:       BinaryOpLessThanOrEqual,
		}, nil
	}

	return expression, nil
}

func (parser *Parser) parseAddition() (Syntax, error) {
	expression, err := parser.parseMultiplication()
	if err != nil {
		return nil, err
	}

	if parser.match(TokenTypePlus) {
		var operator = parser.prev()
		rightExpression, err := parser.parseAddition()
		if err != nil {
			return nil, err
		}
		return BinaryOperationSyntax{
			LeftExpression:  expression,
			Operator:        operator,
			RightExpression: rightExpression,
			Operation:       BinaryOpAdd,
		}, nil
	}
	if parser.match(TokenTypeMinus) {
		var operator = parser.prev()
		rightExpression, err := parser.parseAddition()
		if err != nil {
			return nil, err
		}
		return BinaryOperationSyntax{
			LeftExpression:  expression,
			Operator:        operator,
			RightExpression: rightExpression,
			Operation:       BinaryOpSubtract,
		}, nil
	}

	return expression, nil
}

func (parser *Parser) parseMultiplication() (Syntax, error) {
	expression, err := parser.parseUnary()
	if err != nil {
		return nil, err
	}

	if parser.match(TokenTypeAsterisk) {
		var operator = parser.prev()
		rightExpression, err := parser.parseMultiplication()
		if err != nil {
			return nil, err
		}
		return BinaryOperationSyntax{
			LeftExpression:  expression,
			Operator:        operator,
			RightExpression: rightExpression,
			Operation:       BinaryOpMultiply,
		}, nil
	}
	if parser.match(TokenTypeSlash) {
		var operator = parser.prev()
		rightExpression, err := parser.parseMultiplication()
		if err != nil {
			return nil, err
		}
		return BinaryOperationSyntax{
			LeftExpression:  expression,
			Operator:        operator,
			RightExpression: rightExpression,
			Operation:       BinaryOpDivide,
		}, nil
	}
	if parser.match(TokenTypeModulus) {
		var operator = parser.prev()
		rightExpression, err := parser.parseMultiplication()
		if err != nil {
			return nil, err
		}
		return BinaryOperationSyntax{
			LeftExpression:  expression,
			Operator:        operator,
			RightExpression: rightExpression,
			Operation:       BinaryOpModulus,
		}, nil
	}

	return expression, nil
}

func (parser *Parser) parseUnary() (Syntax, error) {
	if parser.match(TokenTypeExclamation) {
		var operator = parser.prev()
		expression, err := parser.parseUnary()
		if err != nil {
			return nil, err
		}
		return UnaryOperationSyntax{
			Operator:   operator,
			Expression: expression,
			Operation:  UnaryOpNot,
		}, err
	}
	if parser.match(TokenTypeMinus) {
		var operator = parser.prev()
		expression, err := parser.parseUnary()
		if err != nil {
			return nil, err
		}
		return UnaryOperationSyntax{
			Operator:   operator,
			Expression: expression,
			Operation:  UnaryOpNegate,
		}, nil
	}

	return parser.parseMemberAccess()
}

func (parser *Parser) parseMemberAccess() (Syntax, error) {
	expression, err := parser.parseFunctionCall()
	if err != nil {
		return nil, err
	}

	for true {
		if parser.match(TokenTypeDot) {
			dot := parser.prev()
			member, err := parser.parseFunctionCall()
			if err != nil {
				return nil, err
			}

			expression = PropertyAccessSyntax{
				Parent:   expression,
				Dot:      dot,
				Property: member,
			}
		} else if parser.match(TokenTypeLeftSquare) {
			leftSquare := parser.prev()
			member, err := parser.parseExpression()
			if err != nil {
				return nil, err
			}
			rightSquare := parser.expect(TokenTypeRightSquare)

			expression = ArrayAccessSyntax{
				Parent:      expression,
				LeftSquare:  leftSquare,
				Property:    member,
				RightSquare: rightSquare,
			}
		} else if parser.isAtEnd() {
			parser.emitError("Unexpected end")
		} else {
			break
		}
	}

	return expression, nil
}

func (parser *Parser) parseFunctionCall() (Syntax, error) {
	primary, err := parser.parsePrimary()
	if err != nil {
		return nil, err
	}

	if parser.match(TokenTypeLeftParen) {
		leftParen := parser.prev()
		var arguments []Syntax

		for !parser.match(TokenTypeRightParen) {
			if parser.isAtEnd() {
				parser.emitError("Unexpected end")
				return nil, errors.New("Unexpected end")
			}

			if len(arguments) > 0 {
				parser.expect(TokenTypeComma)
			}

			argument, err := parser.parseExpression()
			if err != nil {
				return nil, err
			}
			arguments = append(arguments, argument)
		}

		rightParen := parser.prev()

		return FunctionCallSyntax{
			Parent:     primary,
			LeftParen:  leftParen,
			Arguments:  arguments,
			RightParen: rightParen,
		}, nil
	}

	return primary, nil
}

func (parser *Parser) parsePrimary() (Syntax, error) {
	if parser.match(TokenTypeNumber) {
		literal := parser.prev()
		value, _ := strconv.ParseInt(literal.RawValue, 10, 64) // todo handle errors
		return NumericLiteralSyntax{
			Literal: literal,
			Value:   int(value), // todo avoid this cast
		}, nil
	}
	if parser.match(TokenTypeString) {
		stringToken := parser.prev()
		return StringSyntax{
			StringToken: stringToken,
		}, nil
	}
	if parser.match(TokenTypeIdentifier) {
		identifier := parser.prev()
		return IdentifierSyntax{
			Identifier: identifier,
		}, nil
	}
	if parser.match(TokenTypeFalseKeyword) {
		literal := parser.prev()
		return BooleanLiteralSyntax{
			Literal: literal,
			Value:   false,
		}, nil
	}
	if parser.match(TokenTypeTrueKeyword) {
		literal := parser.prev()
		return BooleanLiteralSyntax{
			Literal: literal,
			Value:   true,
		}, nil
	}
	if parser.match(TokenTypeNullKeyword) {
		literal := parser.prev()
		return NullLiteralSyntax{
			Literal: literal,
		}, nil
	}
	if parser.match(TokenTypeLeftParen) {
		leftParen := parser.prev()
		expression, err := parser.parseExpression()
		if err != nil {
			return nil, err
		}
		rightParen := parser.expect(TokenTypeRightParen)
		return GroupingSyntax{
			LeftParen:  leftParen,
			Expression: expression,
			RightParen: rightParen,
		}, nil
	}
	if parser.match(TokenTypeLeftBrace) {
		return parser.parseObject()
	}
	if parser.match(TokenTypeLeftSquare) {
		return parser.parseArray()
	}

	parser.emitError("Unexpected token")
	return nil, errors.New("Unexpected token")
}

func (parser *Parser) parseObject() (*ObjectSyntax, error) {
	leftBrace := parser.prev()
	var objectProperties []ObjectPropertySyntax

	for !parser.match(TokenTypeRightBrace) {
		if parser.isAtEnd() {
			parser.emitError("Unexpected end")
		}

		identifier := parser.parseIdentifier()
		colon := parser.expect(TokenTypeColon)
		expression, err := parser.parseExpression()
		if err != nil {
			return nil, err
		}

		var objectProperty = ObjectPropertySyntax{
			Identifier: identifier,
			Colon:      colon,
			Expression: expression,
		}
		objectProperties = append(objectProperties, objectProperty)

		// either expect a comma or the end of the object (since a trailing comma is optional).
		// don't use match for '}' as we want the outer loop to pick it up.
		if !parser.match(TokenTypeComma) && !parser.check(TokenTypeRightBrace) {
			if !parser.check(TokenTypeRightBrace) {
				parser.emitError("Unexpected character")
			}
		}
	}

	rightBrace := parser.prev()

	return &ObjectSyntax{
		LeftBrace:  leftBrace,
		Properties: objectProperties,
		RightBrace: rightBrace,
	}, nil
}

func (parser *Parser) parseArray() (*ArraySyntax, error) {
	leftSquare := parser.prev()
	var items []Syntax

	for !parser.match(TokenTypeRightSquare) {
		if parser.isAtEnd() {
			parser.emitError("Unexpected end")
		}

		expression, err := parser.parseExpression()
		if err != nil {
			return nil, err
		}
		items = append(items, expression)

		// either expect a comma or the end of the object (since a trailing comma is optional).
		// don't use match for ']' as we want the outer loop to pick it up.
		if !parser.match(TokenTypeComma) && !parser.check(TokenTypeRightSquare) {
			if !parser.check(TokenTypeRightSquare) {
				parser.emitError("Unexpected character")
			}
		}
	}

	rightSquare := parser.prev()

	return &ArraySyntax{
		LeftSquare:  leftSquare,
		Items:       items,
		RightSquare: rightSquare,
	}, nil
}

func (parser *Parser) getPosition() Position {
	return Position{
		start: parser.tokens[parser.current].position.start,
		end:   parser.tokens[parser.current].position.end,
	}
}

func (parser *Parser) emitError(message string) {
	err := Error{
		message:  message,
		position: parser.getPosition(),
	}

	parser.errors = append(parser.errors, err)
}

func (parser *Parser) isAtEnd() bool {
	return parser.current >= len(parser.tokens) || parser.peek().tokenType == TokenTypeEndOfFile
}

func (parser *Parser) peek() Token {
	return parser.tokens[parser.current]
}

func (parser *Parser) read() Token {
	output := parser.peek()
	parser.current++
	return output
}

func (parser *Parser) prev() Token {
	return parser.tokens[parser.current-1]
}

func (parser *Parser) expect(tokenType TokenType) Token {
	token := parser.read()
	if token.tokenType == tokenType {
		return token
	}

	parser.emitError("Expected ...")
	return token //todo remove this
}

func (parser *Parser) match(tokenType TokenType) bool {
	if parser.check(tokenType) {
		parser.read()
		return true
	}

	return false
}

func (parser *Parser) check(tokenType TokenType) bool {
	if parser.isAtEnd() {
		return false
	}

	return parser.peek().tokenType == tokenType
}

func (parser *Parser) doWithRecovery(start int, matchTokenType TokenType, innerFunc func() (Syntax, error)) Syntax {
	output, err := innerFunc()
	if err == nil {
		return output
	}

	for !parser.isAtEnd() {
		if parser.match(matchTokenType) {
			break
		}
		parser.read()
	}

	return ErrorSyntax{
		Tokens: parser.tokens[start:parser.current],
	}
}
