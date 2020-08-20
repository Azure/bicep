
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
//@[9:12] Resource foo
}

// using string interpolation for the resource type
resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= {
//@[9:12] Resource foo
}

// missing required property
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'={
//@[9:12] Resource foo
}

// duplicate property at the top level
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12] Resource foo
  name: 'foo'
  name: true
}

// duplicate property inside
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12] Resource foo
  name: 'foo'
  properties: {
    foo: 'a'
    foo: 'a'
  }
}

// wrong property types
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12] Resource foo
  name: 'foo'
  location: [
  ]
  tags: 'tag are not a string?'
}

resource bar 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[9:12] Resource bar
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
//@[4:13] Variable resrefvar

param resrefpar string = foo.id
//@[6:15] Parameter resrefpar

output resrefout bool = bar.id
//@[7:16] Output resrefout
