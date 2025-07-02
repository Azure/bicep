# Language Grammar
The following is the active pseudo-grammar of the Bicep parameters file

```
program -> statement* EOF 
statement -> 
  usingDecl |
  paramDecl |
  NL

usingDecl ->
  "using" (stringLiteral | "none") NL

compileTimeImportDecl -> decorator* "import" compileTimeImportTarget compileTimeImportFromClause

compileTimeImportTarget ->
  importedSymbolsList |
  wildcardImport

importedSymbolsList -> "{" ( NL+ ( importedSymbolsListItem NL+ )* )? "}"

importedSymbolsListItem -> IDENTIFIER(originalSymbolName) extensionAsClause?

wildcardImport -> "*" extensionAsClause

compileTimeImportFromClause -> "from" interpString(path)

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
```
