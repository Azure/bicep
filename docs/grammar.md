# Language Grammar
The following is the active pseudo-grammar of the bicep language.
```
program -> statement* EOF 
statement -> 
  targetScopeDecl | 
  parameterDecl | 
  variableDecl |
  resourceDecl |
  moduleDecl |
  outputDecl |
  NL

targetScopeDecl -> "targetScope" "=" expression

parameterDecl -> decorator* "parameter" IDENTIFIER(name) IDENTIFIER(type) parameterDefaultValue? NL
parameterDefaultValue -> "=" expression

variableDecl -> decorator* "variable" IDENTIFIER(name) "=" expression NL

resourceDecl -> decorator* "resource" IDENTIFIER(name) interpString(type) "existing"? "=" ifCondition? object NL

moduleDecl -> decorator* "module" IDENTIFIER(name) interpString(type) "=" object NL

outputDecl -> decorator* "output" IDENTIFIER(name) IDENTIFIER(type) "=" expression NL

NL -> ("\n" | "\r")+

decorator -> "@" decoratorExpression NL

expression -> 
  binaryExpression |
  binaryExpression "?" expression ":" expression

binaryExpression -> 
  equalityExpression |
  binaryExpression "&&" equalityExpression |
  binaryExpression "||" equalityExpression |
  binaryExpression "??" equalityExpression

equalityExpression -> 
  relationalExpression |
  equalityExpression "==" relationalExpression |
  equalityExpression "!=" relationalExpression

relationalExpression -> 
  additiveExpression |
  relationalExpression ">" additiveExpression |
  relationalExpression ">=" additiveExpression |
  relationalExpression "<" additiveExpression |
  relationalExpression "<=" additiveExpression

additiveExpression -> 
  multiplicativeExpression |
  additiveExpression "+" multiplicativeExpression |
  additiveExpression "-" multiplicativeExpression

multiplicativeExpression -> 
  unaryExpression |
  multiplicativeExpression "*" unaryExpression |
  multiplicativeExpression "/" unaryExpression |
  multiplicativeExpression "%" unaryExpression

unaryExpression ->
  memberExpression |
  unaryOperator unaryExpression

unaryOperator -> "!" | "-" | "+"

memberExpression ->
  primaryExpression |
  memberExpression "[" expression "]" |
  memberExpression "." IDENTIFIER(property) |
  memberExpression "." functionCall

primaryExpression ->
  functionCall |
  literalValue |
  interpString |
  array |
  object |
  parenthesizedExpression

decoratorExpression -> functionCall | memberExpression "." functionCall

functionCall -> IDENTIFIER "(" argumentList? ")"

argumentList -> expression ("," expression)*

parenthesizedExpression -> "(" expression ")"

ifCondition -> "if" parenthesizedExpression

interpString ->  interpStringLeftPiece ( expression interpStringMiddlePiece )* expression interpStringRightPiece | literalString
interpStringLeftPiece -> "'" STRINGCHAR* "${"
interpStringMiddlePiece -> "}" STRINGCHAR* "${"
interpStringRightPiece -> "}" STRINGCHAR* "'"
literalString -> "'" STRINGCHAR* "'"

literalValue -> NUMBER | "true" | "false" | "null"

object -> "{" ( NL+ ( objectProperty NL+ )* )? "}"
objectProperty -> ( IDENTIFIER(name) | interpString ) ":" expression 

array -> "[" ( NL+ arrayItem* )? "]"
arrayItem -> expression NL+

```