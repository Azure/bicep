package parser

import (
	"io"
	"io/ioutil"
)

var keywordDict = map[string]TokenType{
	"input":    TokenTypeInputKeyword,
	"output":   TokenTypeOutputKeyword,
	"variable": TokenTypeVariableKeyword,
	"resource": TokenTypeResourceKeyword,
	"module":   TokenTypeModuleKeyword,
	"true":     TokenTypeTrueKeyword,
	"false":    TokenTypeFalseKeyword,
}

type Lexer struct {
	text    []rune
	marker  int
	current int
	Tokens  []Token
	Errors  []Error
}

func Lex(reader io.Reader) *Lexer {
	// todo error handling
	output, _ := ioutil.ReadAll(reader)

	lexer := &Lexer{
		text:    []rune(string(output)),
		marker:  0,
		current: 0,
		Tokens:  make([]Token, 0),
		Errors:  make([]Error, 0),
	}

	lexer.lex()

	return lexer
}

func (lexer *Lexer) lex() {
	for !lexer.isAtEnd() {
		lexer.scanToken()
	}
}

func (lexer *Lexer) getPosition() Position {
	return Position{
		start: lexer.marker,
		end:   lexer.current,
	}
}

func (lexer *Lexer) emitToken(tokenType TokenType) {
	token := Token{
		tokenType: tokenType,
		RawValue:  lexer.getMarkedString(),
		position:  lexer.getPosition(),
	}

	lexer.Tokens = append(lexer.Tokens, token)
}

func (lexer *Lexer) emitError(message string) {
	err := Error{
		message:  message,
		position: lexer.getPosition(),
	}

	lexer.Errors = append(lexer.Errors, err)
}

func (lexer *Lexer) isAtEnd() bool {
	return lexer.current >= len(lexer.text)
}

func (lexer *Lexer) mark() {
	lexer.marker = lexer.current
}

func (lexer *Lexer) peek() rune {
	return lexer.text[lexer.current]
}

func (lexer *Lexer) read() rune {
	output := lexer.peek()
	lexer.current++
	return output
}

func (lexer *Lexer) getMarkedString() string {
	runes := lexer.text[lexer.marker:lexer.current]
	return string(runes)
}

func (lexer *Lexer) scanSingleLineComment() {
	for !lexer.isAtEnd() {
		if lexer.peek() == '\r' {
			lexer.read()
			if lexer.peek() != '\n' {
				lexer.mark()
				lexer.emitError("Found unexpected line feed character")
				continue
			}
		}

		if lexer.peek() == '\n' {
			// todo store comment trivia
			lexer.read()
			return
		}

		lexer.read()
	}

	// todo store comment trivia
}

func (lexer *Lexer) scanMultiLineComment() {
	for !lexer.isAtEnd() {
		nextChar := lexer.read()

		if nextChar != '*' {
			continue
		}

		if lexer.current >= len(lexer.text) {
			break
		}
		nextChar = lexer.read()

		if nextChar == '/' {
			// todo store comment trivia
			return
		}
	}

	// todo store comment trivia
}

func (lexer *Lexer) scanString() {
	for !lexer.isAtEnd() {
		nextChar := lexer.read()

		if nextChar == '\'' {
			lexer.emitToken(TokenTypeString)
			return
		}

		if nextChar != '\\' {
			continue
		}

		if lexer.current >= len(lexer.text) {
			break
		}
		nextChar = lexer.read()

		switch nextChar {
		case 'n':
		case 'r':
		case 't':
		case '\\':
		case '$':
		default:
			lexer.emitError("Unrecognized escape sequence")
		}
	}

	lexer.emitError("Unterminated string")
}

func isAlpha(char rune) bool {
	return (char >= 'a' && char <= 'z') || (char >= 'A' && char <= 'Z') || char == '_'
}

func isDigit(char rune) bool {
	return char >= '0' && char <= '9'
}

func isAlphaNumeric(char rune) bool {
	return isAlpha(char) || isDigit(char)
}

func (lexer *Lexer) scanNumber() {
	for !lexer.isAtEnd() {
		if !isDigit(lexer.peek()) {
			break
		}

		lexer.read()
	}

	lexer.emitToken(TokenTypeNumber)
}

func (lexer *Lexer) scanIdentifier() {
	for !lexer.isAtEnd() {
		if !isAlphaNumeric(lexer.peek()) {
			break
		}

		lexer.read()
	}

	identifier := lexer.getMarkedString()

	tokenType, isKeyword := keywordDict[identifier]
	if !isKeyword {
		tokenType = TokenTypeIdentifier
	}

	lexer.emitToken(tokenType)
}

func (lexer *Lexer) scanToken() {
	lexer.marker = lexer.current
	nextChar := lexer.read()

	switch nextChar {
	case '{':
		lexer.emitToken(TokenTypeLeftBrace)
	case '}':
		lexer.emitToken(TokenTypeRightBrace)
	case '(':
		lexer.emitToken(TokenTypeLeftParen)
	case ')':
		lexer.emitToken(TokenTypeRightParen)
	case '[':
		lexer.emitToken(TokenTypeLeftSquare)
	case ']':
		lexer.emitToken(TokenTypeRightSquare)
	case ',':
		lexer.emitToken(TokenTypeComma)
	case '.':
		lexer.emitToken(TokenTypeDot)
	case '?':
		lexer.emitToken(TokenTypeQuestion)
	case ':':
		lexer.emitToken(TokenTypeColon)
	case ';':
		lexer.emitToken(TokenTypeSemicolon)
	case '+':
		lexer.emitToken(TokenTypePlus)
	case '-':
		lexer.emitToken(TokenTypePlus)
	case '%':
		lexer.emitToken(TokenTypeModulus)
	case '*':
		lexer.emitToken(TokenTypeAsterisk)
	case '/':
		if lexer.isAtEnd() {
			lexer.emitToken(TokenTypeSlash)
			break
		}

		switch lexer.peek() {
		case '/':
			lexer.read()
			lexer.scanSingleLineComment()
		case '*':
			lexer.read()
			lexer.scanMultiLineComment()
		default:
			lexer.emitToken(TokenTypeSlash)
		}
	case '!':
		if lexer.isAtEnd() {
			lexer.emitToken(TokenTypeExclamation)
			break
		}

		switch lexer.peek() {
		case '=':
			lexer.read()
			lexer.emitToken(TokenTypeNotEquals)
		case '~':
			lexer.read()
			lexer.emitToken(TokenTypeNotEqualsInsensitive)
		default:
			lexer.emitToken(TokenTypeExclamation)
		}
	case '<':
		if lexer.isAtEnd() {
			lexer.emitToken(TokenTypeLessThan)
			break
		}

		switch lexer.peek() {
		case '=':
			lexer.read()
			lexer.emitToken(TokenTypeLessThanOrEqual)
		default:
			lexer.emitToken(TokenTypeLessThan)
		}
	case '>':
		if lexer.isAtEnd() {
			lexer.emitToken(TokenTypeGreaterThan)
			break
		}

		switch lexer.peek() {
		case '=':
			lexer.read()
			lexer.emitToken(TokenTypeGreaterThanOrEqual)
		default:
			lexer.emitToken(TokenTypeGreaterThan)
		}
	case '=':
		if lexer.isAtEnd() {
			lexer.emitError("Unrecognized character")
			break
		}

		switch lexer.peek() {
		case '=':
			lexer.read()
			lexer.emitToken(TokenTypeEquals)
		case '~':
			lexer.read()
			lexer.emitToken(TokenTypeEqualsInsensitive)
		default:
			lexer.emitError("Unrecognized character")
		}
	case '&':
		if lexer.isAtEnd() {
			lexer.emitError("Unrecognized character")
			break
		}

		switch lexer.peek() {
		case '&':
			lexer.read()
			lexer.emitToken(TokenTypeBinaryAnd)
		default:
			lexer.emitError("Unrecognized character")
		}
	case '|':
		if lexer.isAtEnd() {
			lexer.emitError("Unrecognized character")
			break
		}

		switch lexer.peek() {
		case '|':
			lexer.read()
			lexer.emitToken(TokenTypeBinaryOr)
		default:
			lexer.emitError("Unrecognized character")
		}
	case ' ':
	case '\t':
	case '\r':
	case '\n':
	case '\'':
		lexer.scanString()
	default:
		if isDigit(nextChar) {
			lexer.scanNumber()
			return
		}

		if isAlpha(nextChar) {
			lexer.scanIdentifier()
			return
		}

		lexer.emitError("Unrecognized character")
	}
}
