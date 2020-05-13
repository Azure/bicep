package parser

type TokenType int

const (
	TokenTypeLeftBrace TokenType = iota
	TokenTypeRightBrace
	TokenTypeLeftParen
	TokenTypeRightParen
	TokenTypeLeftSquare
	TokenTypeRightSquare
	TokenTypeComma
	TokenTypeDot
	TokenTypeQuestion
	TokenTypeColon
	TokenTypeSemicolon
	TokenTypePlus
	TokenTypeMinus
	TokenTypeAsterisk
	TokenTypeSlash
	TokenTypeModulus
	TokenTypeExclamation
	TokenTypeLessThan
	TokenTypeGreaterThan
	TokenTypeLessThanOrEqual
	TokenTypeGreaterThanOrEqual
	TokenTypeEquals
	TokenTypeNotEquals
	TokenTypeEqualsInsensitive
	TokenTypeNotEqualsInsensitive
	TokenTypeBinaryAnd
	TokenTypeBinaryOr
	TokenTypeIdentifier
	TokenTypeString
	TokenTypeNumber
	TokenTypeInputKeyword
	TokenTypeOutputKeyword
	TokenTypeVariableKeyword
	TokenTypeResourceKeyword
	TokenTypeModuleKeyword
	TokenTypeTrueKeyword
	TokenTypeFalseKeyword
	TokenTypeNullKeyword
	TokenTypeEndOfFile
)

type Position struct {
	start int
	end   int
}

type Positionable interface {
	GetPosition() *Position
}

func MakePosition(start Positionable, end Positionable) *Position {
	return &Position{
		start: start.GetPosition().start,
		end:   end.GetPosition().end,
	}
}

type Token struct {
	tokenType TokenType
	RawValue  string
	position  Position
}

func (token Token) GetPosition() *Position {
	return &token.position
}
