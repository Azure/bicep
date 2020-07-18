# Language Grammar
The following is the active pseudo-grammar of the bicep language.
```
program -> statement* EOF 
statement -> parameterDecl | 
             variableDecl |
             resourceDecl |
             outputDecl |
             NL

parameterDecl -> "parameter" IDENTIFIER(name) IDENTIFIER(type) (parameterDefaultValue | object(modifier))? NL
parameterDefaultValue -> "=" value

variableDecl -> "variable" IDENTIFIER(name) "=" value NL

resourceDecl -> "resource" IDENTIFIER(name) STRING(type) "=" object NL

outputDecl -> "output" IDENTIFIER(name) IDENTIFIER(type) "=" value NL

value -> NUMBER | STRING | "true" | "false" | object | array

object -> "{" NL+ ( objectProperty NL+ )* "}" 
objectProperty -> IDENTIFIER(name) ":" value 

array -> "[" NL+ arrayItem* "]"
arrayItem -> value NL+

NL -> ("\n" | "\r")+
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