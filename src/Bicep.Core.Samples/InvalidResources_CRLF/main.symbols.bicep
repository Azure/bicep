
// wrong declaration
bad

// incomplete
resource 
resource foo
resource fo/o
resource foo 'ddd'
resource foo 'ddd'=

// wrong resource type
resource foo 'ddd'={
//@[9:12) Resource foo. Type: error. Declaration start char: 0, length: 23
}

// using string interpolation for the resource type
resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= {
//@[9:12) Resource foo. Type: error. Declaration start char: 0, length: 64
}

// missing required property
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'={
//@[9:12) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 55
}

// duplicate property at the top level
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 85
  name: 'foo'
  name: true
}

// duplicate property at the top level with string literal syntax
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 87
  name: 'foo'
  'name': true
}

// duplicate property inside
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 121
  name: 'foo'
  properties: {
    foo: 'a'
    foo: 'a'
  }
}

// duplicate property inside with string literal syntax
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 123
  name: 'foo'
  properties: {
    foo: 'a'
    'foo': 'a'
  }
}

// wrong property types
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) Resource foo. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 124
  name: 'foo'
  location: [
  ]
  tags: 'tag are not a string?'
}

resource bar 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[9:12) Resource bar. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 231
  name: true ? 's' : 'a' + 1
  properties: {
    x: foo()
    y: true && (null || !4)
    a: [
      a
      !null
      true && true || true + -true * 4
    ]
  }
}

// unsupported resource ref
var resrefvar = bar.name
//@[4:13) Variable resrefvar. Type: string. Declaration start char: 0, length: 24

param resrefpar string = foo.id
//@[6:15) Parameter resrefpar. Type: string. Declaration start char: 0, length: 31

output resrefout bool = bar.id
//@[7:16) Output resrefout. Type: bool. Declaration start char: 0, length: 30

// attempting to set read-only properties
resource baz 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[9:12) Resource baz. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 119
  name: 'test'
  id: 2
  type: 'hello'
  apiVersion: true
}

resource badDepends 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[9:19) Resource badDepends. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 113
  name: 'test'
  dependsOn: [
    baz.id
  ]
}

resource badDepends2 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[9:20) Resource badDepends2. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 125
  name: 'test'
  dependsOn: [
    'hello'
    true
  ]
}

resource badDepends3 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[9:20) Resource badDepends3. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 81
  name: 'test'
}

resource badDepends4 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[9:20) Resource badDepends4. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 119
  name: 'test'
  dependsOn: [
    badDepends3
  ]
}

resource badDepends5 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[9:20) Resource badDepends5. Type: Microsoft.Foo/foos@2020-02-02-alpha. Declaration start char: 0, length: 117
  name: 'test'
  dependsOn: badDepends3.dependsOn
}
