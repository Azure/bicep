# Language Grammar
The following is the active pseudo-grammar of the bicep language.
```
program -> statement* EOF
statement ->
  targetScopeDecl |
  extensionDecl |
  compileTimeImportDecl |
  metadataDecl |
  parameterDecl |
  typeDecl |
  variableDecl |
  resourceDecl |
  moduleDecl |
  testDecl |
  assertDel |
  outputDecl |
  functionDecl |
  NL

targetScopeDecl -> "targetScope" "=" expression

extensionDecl -> decorator* "extension" interpString(specification) extensionWithClause? extensionAsClause? NL

extensionWithClause -> "with" object

extensionAsClause -> "as" IDENTIFIER(alias)

compileTimeImportDecl -> decorator* "import" compileTimeImportTarget compileTimeImportFromClause

compileTimeImportTarget ->
  importedSymbolsList |
  wildcardImport

importedSymbolsList -> "{" ( NL+ ( importedSymbolsListItem NL+ )* )? "}"

importedSymbolsListItem -> IDENTIFIER(originalSymbolName) extensionAsClause?

wildcardImport -> "*" extensionAsClause

compileTimeImportFromClause -> "from" interpString(path)

metadataDecl -> "metadata" IDENTIFIER(name) "=" expression NL

parameterDecl ->
  decorator* "parameter" IDENTIFIER(name) typeExpression parameterDefaultValue? NL |
  decorator* "parameter" IDENTIFIER(name) "resource" interpString(type) parameterDefaultValue? NL |
parameterDefaultValue -> "=" expression

typeDecl -> decorator* "type" IDENTIFIER(name) "=" typeExpression NL

variableDecl -> decorator* "variable" IDENTIFIER(name) "=" expression NL

resourceDecl -> decorator* "resource" IDENTIFIER(name) interpString(type) "existing"? "=" (ifCondition | object | forExpression) NL

moduleDecl -> decorator* "module" IDENTIFIER(name) interpString(type) "=" (ifCondition | object | forExpression) NL

testDecl -> "test" IDENTIFIER(name) interpString(type) "=" (object) NL

assertDecl -> decorator* "assert" IDENTIFIER(name) "=" expression NL

outputDecl ->
  decorator* "output" IDENTIFIER(name) IDENTIFIER(type) "=" expression NL
  decorator* "output" IDENTIFIER(name) "resource" interpString(type) "=" expression NL
NL -> ("\n" | "\r")+

functionDecl -> decorator* "func" IDENTIFIER(name) typedLambdaExpression NL

decorator -> "@" decoratorExpression NL

disableNextLineDiagnosticsDirective-> #disable-next-line diagnosticCode1 diagnosticCode2 diagnosticCode3 NL

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
  equalityExpression "!=" relationalExpression |
  equalityExpression "=~" relationalExpression |
  equalityExpression "!~" relationalExpression

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
  memberExpression "[^" expression "]" |
  memberExpression "[?" expression "]" |
  memberExpression "[?^" expression "]" |
  memberExpression "." IDENTIFIER(property) |
  memberExpression ".?" IDENTIFIER(property) |
  memberExpression "." functionCall |
  memberExpression "::" IDENTIFIER(name) |
  memberExpression "!"

primaryExpression ->
  functionCall |
  literalValue |
  interpString |
  multilineString |
  array |
  forExpression |
  object |
  parenthesizedExpression |
  lambdaExpression

decoratorExpression -> functionCall | memberExpression "." functionCall

argumentList -> expression ("," expression)*
functionCall -> IDENTIFIER "(" argumentList? ")"

parenthesizedExpression -> "(" expression ")"

localVariable -> IDENTIFIER
variableBlock -> "(" NL* ( localVariable NL* ("," NL* localVariable NL*)* )? ")"
lambdaExpression -> ( variableBlock | localVariable ) "=>" expression

typedLocalVariable -> IDENTIFIER primaryTypeExpression
typedVariableBlock -> "(" NL* ( typedLocalVariable NL* ("," NL* typedLocalVariable NL*)* )? ")"
typedLambdaExpression -> typedVariableBlock primaryTypeExpression "=>" expression

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

object -> "{" NL? ( | ( objectProperty ( objectSep objectProperty )* ( | objectSep ) ) ) "}"
objectSep -> "," | NL
objectKey -> IDENTIFIER(name) | interpString
objectProperty -> ( objectKey ":" expression ) | ( "..." expression )

array -> "[" NL? ( | ( arrayItem ( arraySep arrayItem )* ( | arraySep ) ) ) "]"
arraySep -> "," | NL
arrayItem -> expression | ( "..." expression )

typeExpression ->
  singularTypeExpression |
  unionTypeExpression

unionTypeExpression -> "|"? singularTypeExpression ("|" singularTypeExpression)*

singularTypeExpression ->
  primaryTypeExpression |
  singularTypeExpression "[]" |
  singularTypeExpression "?" |
  parenthesizedTypeExpression

primaryTypeExpression ->
  typeReference |
  literalValue |
  unaryOperator literalValue |
  stringComplete |
  multilineString |
  objectType |
  tupleType

typeReference ->
  ambientTypeReference |
  fullyQualifiedAmbientTypeReference |
  IDENTIFIER(type) |
  IDENTIFIER(importedType) |
  IDENTIFIER(wildcardImport) "." IDENTIFIER(type) |
  typeReference "." IDENTIFIER(property) |
  typeReference "[" (NUMBER | "*") "]" |
  typeReference ".*"

ambientTypeReference -> "string" | "int" | "bool" | "array" | "object"

fullyQualifiedAmbientTypeReference -> IDENTIFIER(sysNamespace) "." ambientTypeReference

objectType -> "{" (NL+ ((objectTypeProperty | objectTypeAdditionalPropertiesMatcher) NL+ )* )? "}"
objectTypeProperty -> decorator* ( IDENTIFIER(name) | stringComplete | multilineString ) ":" typeExpression
objectTypeAdditionalPropertiesMatcher -> decorator* "*:" typeExpression

tupleType -> "[" (NL+ tupleItem* )? "]"
tupleItem -> decorator* typeExpression NL+

parenthesizedTypeExpression -> "(" typeExpression ")"

```
