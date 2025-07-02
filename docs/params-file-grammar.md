program -> statement* EOF 
statement -> 
  usingDecl |
  paramDecl |
  NL

usingDecl ->
  "using" stringLiteral | "none" NL

paramDecl ->
  "param" IDENTIFIER(name) "=" literalValue NL

varDecl ->
  "var" IDENTIFIER(name) "=" literalValue NL

stringLiteral -> "'" STRINGCHAR* "'"

literalValue -> NUMBER | "true" | "false" | "null" | stringLiteral | object | array

object -> "{" ( NL+ ( objectProperty NL+ )* )? "}"
objectProperty -> ( IDENTIFIER(name) | stringLiteral ) ":" literalValue 

array -> "[" ( NL+ arrayItem* )? "]"
arrayItem -> literalValue NL+
