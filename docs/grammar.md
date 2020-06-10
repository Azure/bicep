# Language Grammar
The following is the active pseudo-grammar of the bicep language.
```
program -> statement* EOF 
statement -> parameterStmt | NL

parameterStmt -> "parameter" IDENTIFIER(name) IDENTIFIER(type) ("=" value )? NL

value -> NUMBER | STRING | "true" | "false"

NL -> "\n" | "\r\n" | "\r"
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