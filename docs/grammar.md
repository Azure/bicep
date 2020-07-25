# Language Grammar
The following is the active pseudo-grammar of the bicep language.
```
program -> statement* EOF 
statement -> 
  parameterDecl | 
  variableDecl |
  resourceDecl |
  outputDecl |
  NL

parameterDecl -> "parameter" IDENTIFIER(name) IDENTIFIER(type) (parameterDefaultValue | object(modifier))? NL
parameterDefaultValue -> "=" expression

variableDecl -> "variable" IDENTIFIER(name) "=" expression NL

resourceDecl -> "resource" IDENTIFIER(name) STRING(type) "=" object NL

outputDecl -> "output" IDENTIFIER(name) IDENTIFIER(type) "=" expression NL

NL -> ("\n" | "\r")+

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
  memberExpression "." IDENTIFIER(property)

primaryExpression ->
  functionCall |
  literalValue |
  array |
  object |
  parenthesizedExpression

functionCall -> IDENTIFIER "(" argumentList? ")"

argumentList -> expression ("," expression)*

parenthesizedExpression -> "(" expression ")"

literalValue -> NUMBER | STRING | "true" | "false" | "null"

object -> "{" NL+ ( objectProperty NL+ )* "}" 
objectProperty -> IDENTIFIER(name) ":" expression 

array -> "[" NL+ arrayItem* "]"
arrayItem -> expression NL+

```


Ignore everything below

```
expression -> ternary 
ternary -> booleanOr ( "?" booleanOr ":" booleanOr )? 
booleanOr -> booleanAnd ( "||" booleanOr )? 
booleanAnd -> equality ( "&&" booleanAnd )? 
equality -> comparison ( ( "==" | "!=" | "=~" | "!~" ) equality )? 
comparison -> addition ( ( ">" | ">=" | "<" | "<=" ) comparison )? 
addition -> multiplication ( ( "-" | "+" ) addition )? 
multiplication -> unary ( ( "/" | "*" | "%" ) multiplication )? 
unary -> ( "!" | "-" ) unary | memberAccess 
memberAccess -> functionCall ( "." functionCall | "[" expression "]" )* 
functionCall -> primary ( "(" ( expression ( "," expression )* )? ")" )? 
primary -> NUMBER | STRING | IDENTIFIER | object | array | "false" | "true" | "null" | "(" expression ")" 
object -> "{" ( objectProperty "," )* objectProperty? "}" 
objectProperty -> IDENTIFIER ":" expression 
array -> "[" ( expression "," )* expression? "]" 
```