
// wrong declaration
bad
//@[0:3) Error This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. |bad|

// incomplete
resource 
//@[9:9) Error Expected a resource identifier at this location. ||
resource foo
//@[12:12) Error Expected a resource type string. Specify a valid resource type of format '<provider>/<types>@<apiVersion>'. ||
resource fo/o
//@[11:12) Error Expected a resource type string. Specify a valid resource type of format '<provider>/<types>@<apiVersion>'. |/|
resource foo 'ddd'
//@[18:18) Error Expected the '=' character at this location. ||
resource foo 'ddd'=
//@[19:19) Error Expected the '{' character at this location. ||

// wrong resource type
resource foo 'ddd'={
//@[9:12) Error Identifier 'foo' is declared multiple times. Remove or rename the duplicates. |foo|
//@[13:18) Error The resource type is not valid. Specify a valid resource type of format '<provider>/<types>@<apiVersion>'. |'ddd'|
}

// using string interpolation for the resource type
resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= {
//@[9:12) Error Identifier 'foo' is declared multiple times. Remove or rename the duplicates. |foo|
//@[13:58) Error String interpolation is unsupported for specifying the resource type. |'Microsoft.${provider}/foos@2020-02-02-alpha'|
}

// missing required property
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'={
//@[9:12) Error Identifier 'foo' is declared multiple times. Remove or rename the duplicates. |foo|
//@[51:55) Error The specified object is missing the following required properties: name. |{\r\n}|
}

// duplicate property at the top level
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) Error Identifier 'foo' is declared multiple times. Remove or rename the duplicates. |foo|
  name: 'foo'
//@[2:6) Error The property 'name' is declared multiple times in this object. Remove or rename the duplicate properties. |name|
  name: true
//@[2:6) Error The property 'name' is declared multiple times in this object. Remove or rename the duplicate properties. |name|
}

// duplicate property at the top level with string literal syntax
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) Error Identifier 'foo' is declared multiple times. Remove or rename the duplicates. |foo|
  name: 'foo'
//@[2:6) Error The property 'name' is declared multiple times in this object. Remove or rename the duplicate properties. |name|
  'name': true
//@[2:8) Error The property 'name' is declared multiple times in this object. Remove or rename the duplicate properties. |'name'|
}

// duplicate property inside
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) Error Identifier 'foo' is declared multiple times. Remove or rename the duplicates. |foo|
  name: 'foo'
  properties: {
    foo: 'a'
//@[4:7) Error The property 'foo' is declared multiple times in this object. Remove or rename the duplicate properties. |foo|
    foo: 'a'
//@[4:7) Error The property 'foo' is declared multiple times in this object. Remove or rename the duplicate properties. |foo|
  }
}

// duplicate property inside with string literal syntax
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) Error Identifier 'foo' is declared multiple times. Remove or rename the duplicates. |foo|
  name: 'foo'
  properties: {
    foo: 'a'
//@[4:7) Error The property 'foo' is declared multiple times in this object. Remove or rename the duplicate properties. |foo|
    'foo': 'a'
//@[4:9) Error The property 'foo' is declared multiple times in this object. Remove or rename the duplicate properties. |'foo'|
  }
}

// wrong property types
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) Error Identifier 'foo' is declared multiple times. Remove or rename the duplicates. |foo|
  name: 'foo'
  location: [
//@[12:18) Error The property 'location' expected a value of type string but the provided value is of type array. |[\r\n  ]|
  ]
  tags: 'tag are not a string?'
//@[8:31) Error The property 'tags' expected a value of type Tags but the provided value is of type 'tag are not a string?'. |'tag are not a string?'|
}

resource bar 'Microsoft.Foo/foos@2020-02-02-alpha' = {
  name: true ? 's' : 'a' + 1
//@[21:28) Error Cannot apply operator '+' to operands of type 'a' and int. |'a' + 1|
  properties: {
    x: foo()
//@[7:10) Error The name 'foo' is not a function. |foo|
    y: true && (null || !4)
//@[24:26) Error Cannot apply operator '!' to operand of type int. |!4|
    a: [
      a
//@[6:7) Error The name 'a' does not exist in the current context. |a|
      !null
//@[6:11) Error Cannot apply operator '!' to operand of type null. |!null|
      true && true || true + -true * 4
//@[29:34) Error Cannot apply operator '-' to operand of type bool. |-true|
    ]
  }
}

// unsupported resource ref
var resrefvar = bar.name

param resrefpar string = foo.id
//@[25:28) Error This symbol cannot be referenced here. Only other parameters can be referenced in parameter default values. |foo|
//@[25:28) Error The referenced declaration with name 'foo' is not valid. |foo|

output resrefout bool = bar.id
//@[24:30) Error The output expects a value of type bool but the provided value is of type string. |bar.id|

// attempting to set read-only properties
resource baz 'Microsoft.Foo/foos@2020-02-02-alpha' = {
  name: 'test'
  id: 2
//@[2:4) Error The property 'id' is read-only. Expressions cannot be assigned to read-only properties. |id|
//@[6:7) Error The property 'id' expected a value of type string but the provided value is of type int. |2|
  type: 'hello'
//@[2:6) Error The property 'type' is read-only. Expressions cannot be assigned to read-only properties. |type|
//@[8:15) Error The property 'type' expected a value of type 'Microsoft.Foo/foos' but the provided value is of type 'hello'. |'hello'|
  apiVersion: true
//@[2:12) Error The property 'apiVersion' is read-only. Expressions cannot be assigned to read-only properties. |apiVersion|
//@[14:18) Error The property 'apiVersion' expected a value of type '2020-02-02-alpha' but the provided value is of type bool. |true|
}

resource badDepends 'Microsoft.Foo/foos@2020-02-02-alpha' = {
  name: 'test'
  dependsOn: [
    baz.id
//@[4:10) Error The enclosing array expected an item of type resource, but the provided item was of type string. |baz.id|
  ]
}

resource badDepends2 'Microsoft.Foo/foos@2020-02-02-alpha' = {
  name: 'test'
  dependsOn: [
    'hello'
//@[4:11) Error The enclosing array expected an item of type resource, but the provided item was of type 'hello'. |'hello'|
    true
//@[4:8) Error The enclosing array expected an item of type resource, but the provided item was of type bool. |true|
  ]
}

resource badDepends3 'Microsoft.Foo/foos@2020-02-02-alpha' = {
  name: 'test'
}

resource badDepends4 'Microsoft.Foo/foos@2020-02-02-alpha' = {
  name: 'test'
  dependsOn: [
    badDepends3
  ]
}

resource badDepends5 'Microsoft.Foo/foos@2020-02-02-alpha' = {
  name: 'test'
  dependsOn: badDepends3.dependsOn
//@[25:34) Error The property 'dependsOn' on type 'Microsoft.Foo/foos@2020-02-02-alpha' is write-only. Write-only properties cannot be accessed. |dependsOn|
}
