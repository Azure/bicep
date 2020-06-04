# Language Grammar

```
program -> statement* EOF ;
statement -> inputStmt | outputStmt | variableStmt | resourceStmt ;

inputStmt -> "input" IDENTIFIER ;
outputStmt -> "output" IDENTIFIER ":" expression ;
variableStmt -> "variable" IDENTIFIER ":" expression ;
resourceStmt -> "resource" IDENTIFIER STRING IDENTIFIER ":" expression ;

expression -> ternary ;
ternary -> booleanOr ( "?" booleanOr ":" booleanOr )? ;
booleanOr -> booleanAnd ( "||" booleanOr )? ;
booleanAnd -> equality ( "&&" booleanAnd )? ;
equality -> comparison ( ( "==" | "!=" | "=~" | "!~" ) equality )? ;
comparison -> addition ( ( ">" | ">=" | "<" | "<=" ) comparison )? ;
addition -> multiplication ( ( "-" | "+" ) addition )? ;
multiplication -> unary ( ( "/" | "*" | "%" ) multiplication )? ;
unary -> ( "!" | "-" ) unary | memberAccess ;
memberAccess -> functionCall ( "." functionCall | "[" expression "]" )* ;
functionCall -> primary ( "(" ( expression ( "," expression )* )? ")" )? ;
primary -> NUMBER | STRING | IDENTIFIER | object | array | "false" | "true" | "null" | "(" expression ")" ;
object -> "{" ( objectProperty "," )* objectProperty? "}" ;
objectProperty -> IDENTIFIER ":" expression ;
array -> "[" ( expression "," )* expression? "]" ;
```