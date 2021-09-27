# Language Grammar
The following is the active pseudo-grammar of the bicep language.
```
program -> statement* EOF 
statement -> 
  targetScopeDecl | 
  importDecl | 
  parameterDecl | 
  variableDecl |
  resourceDecl |
  moduleDecl |
  outputDecl |
  NL

targetScopeDecl -> "targetScope" "=" expression

importDecl -> decorator* "import" IDENTIFIER(aliasName) "from" IDENTIFIER(providerName) object? NL

parameterDecl -> decorator* "parameter" IDENTIFIER(name) IDENTIFIER(type) parameterDefaultValue? NL
parameterDefaultValue -> "=" expression

variableDecl -> decorator* "variable" IDENTIFIER(name) "=" expression NL

resourceDecl -> decorator* "resource" IDENTIFIER(name) interpString(type) "existing"? "=" (ifCondition | object | forExpression) NL

moduleDecl -> decorator* "module" IDENTIFIER(name) interpString(type) "=" (ifCondition | object | forExpression) NL

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
  memberExpression ":" IDENTIFIER(name)

primaryExpression ->
  functionCall |
  literalValue |
  interpString |
  multilineString |
  array |
  forExpression |
  object |
  parenthesizedExpression

decoratorExpression -> functionCall | memberExpression "." functionCall

functionCall -> IDENTIFIER "(" argumentList? ")"

argumentList -> expression ("," expression)*

parenthesizedExpression -> "(" expression ")"

ifCondition -> "if" parenthesizedExpression object

forExpression -> "[" "for" (IDENTIFIER(item) | forVariableBlock) "in" expression ":" forBody "]"
forVariableBlock -> "(" IDENTIFIER(item) "," IDENTIFIER(index) ")"
forBody -> expression(body) | ifCondition

interpString ->  stringLeftPiece ( expression stringMiddlePiece )* expression stringRightPiece | stringComplete
stringLeftPiece -> "'" STRINGCHAR* "${"
stringMiddlePiece -> "}" STRINGCHAR* "${"
stringRightPiece -> "}" STRINGCHAR* "'"
stringComplete -> "'" STRINGCHAR* "'"

multilineString -> "'''" + MULTILINESTRINGCHAR+ + "'''"

literalValue -> NUMBER | "true" | "false" | "null"

object -> "{" ( NL+ ( objectProperty NL+ )* )? "}"
objectProperty -> ( IDENTIFIER(name) | interpString ) ":" expression 

array -> "[" ( NL+ arrayItem* )? "]"
arrayItem -> expression NL+

```
