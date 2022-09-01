program -> statement* EOF 
statement -> 
  usingDecl |
  parameterDecl |
  NL

usingDecl ->
  "using" stringLiteral NL


  importDecl -> decorator* "import" IDENTIFIER(providerName) "as" IDENTIFIER(aliasName) object? NL

parameterDecl ->
  "parameter" IDENTIFIER(name) "=" literalValue NL

stringLiteral -> "'" STRINGCHAR* "'"

literalValue -> NUMBER | "true" | "false" | "null" | stringLiteral | object | array

object -> "{" ( NL+ ( objectProperty NL+ )* )? "}"
objectProperty -> ( IDENTIFIER(name) | stringLiteral ) ":" literalValue 

array -> "[" ( NL+ arrayItem* )? "]"
arrayItem -> literalValue NL+