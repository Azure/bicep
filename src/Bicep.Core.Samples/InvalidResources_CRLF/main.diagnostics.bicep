
// wrong declaration
bad
//@[0:3] Error This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. |bad|

// incomplete
resource 
//@[9:9] Error Expected a resource identifier at this location. ||
resource foo
resource fo/o
//@[0:8] Error Expected the '=' character at this location. |resource|
resource foo 'ddd'
//@[18:18] Error Expected the '=' character at this location. ||
resource foo 'ddd'=
//@[19:19] Error Expected the '{' character at this location. ||

// wrong resource type
resource foo 'ddd'={
//@[9:12] Error Identifier 'foo' is declared multiple times. Remove or rename the duplicates. |foo|
//@[13:18] Error The resource type is not valid. Specify a valid resource type. |'ddd'|
}

// using string interpolation for the resource type
resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= {
//@[9:12] Error Identifier 'foo' is declared multiple times. Remove or rename the duplicates. |foo|
//@[13:58] Error String interpolation is unsupported for specifying the resource type. |'Microsoft.${provider}/foos@2020-02-02-alpha'|
}

// missing required property
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'={
//@[9:12] Error Identifier 'foo' is declared multiple times. Remove or rename the duplicates. |foo|
//@[51:55] Error The specified object is missing the following required properties: name. |{\r\n}|
}

// duplicate property at the top level
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12] Error Identifier 'foo' is declared multiple times. Remove or rename the duplicates. |foo|
  name: 'foo'
//@[2:6] Error The property 'name' is declared multiple times in this object. Remove or rename the duplicate properties. |name|
  name: true
//@[2:6] Error The property 'name' is declared multiple times in this object. Remove or rename the duplicate properties. |name|
}

// duplicate property inside
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12] Error Identifier 'foo' is declared multiple times. Remove or rename the duplicates. |foo|
  name: 'foo'
  properties: {
    foo: 'a'
//@[4:7] Error The property 'foo' is declared multiple times in this object. Remove or rename the duplicate properties. |foo|
    foo: 'a'
//@[4:7] Error The property 'foo' is declared multiple times in this object. Remove or rename the duplicate properties. |foo|
  }
}

// wrong property types
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12] Error Identifier 'foo' is declared multiple times. Remove or rename the duplicates. |foo|
  name: 'foo'
  location: [
//@[12:18] Error The property 'location' expected a value of type 'string' but the provided value is of type 'array'. |[\r\n  ]|
  ]
  tags: 'tag are not a string?'
//@[8:31] Error The property 'tags' expected a value of type 'Tags' but the provided value is of type 'string'. |'tag are not a string?'|
}

resource bar 'Microsoft.Foo/foos@2020-02-02-alpha' = {
  name: true ? 's' : 'a' + 1
//@[21:28] Error Cannot apply operator '+' to operands of type 'string' and 'int'. |'a' + 1|
  properties: {
    x: foo()
//@[7:10] Error The name 'foo' is not a function. |foo|
    y: true && (null || !4)
//@[24:26] Error Cannot apply operator '!' to operand of type 'int'. |!4|
    a: [
      a
//@[6:7] Error The name 'a' does not exist in the current context. |a|
      !null
//@[6:11] Error Cannot apply operator '!' to operand of type 'null'. |!null|
      true && true || true + -true * 4
//@[29:34] Error Cannot apply operator '-' to operand of type 'bool'. |-true|
    ]
  }
}

// unsupported resource ref
var resrefvar = bar.name
//@[16:19] Error The referenced declaration with name 'bar' is not valid. |bar|

param resrefpar string = foo.id

output resrefout bool = bar.id
//@[24:27] Error The referenced declaration with name 'bar' is not valid. |bar|
