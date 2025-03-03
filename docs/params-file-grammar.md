program -> statement* EOF 
statement -> 
  usingDecl |
  parameterDecl |
  extensionConfigDecl |
  NL

usingDecl ->
  "using" stringLiteral NL


  importDecl -> decorator* "import" IDENTIFIER(providerName) "as" IDENTIFIER(aliasName) object? NL

parameterDecl ->
  "parameter" IDENTIFIER(name) "=" literalValue NL

extensionConfigDecl ->
  "extension" IDENTIFIER(name) "with" object NL

stringLiteral -> "'" STRINGCHAR* "'"

literalValue -> NUMBER | "true" | "false" | "null" | stringLiteral | object | array

object -> "{" ( NL+ ( objectProperty NL+ )* )? "}"
objectProperty -> ( IDENTIFIER(name) | stringLiteral ) ":" literalValue 

array -> "[" ( NL+ arrayItem* )? "]"
arrayItem -> literalValue NL+