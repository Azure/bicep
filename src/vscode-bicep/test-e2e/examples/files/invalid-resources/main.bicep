
// wrong declaration
bad

// incomplete #completionTest(9) -> empty
resource 
resource foo
resource fo/o
resource foo 'ddd'

// #completionTest(23) -> resourceTypes
resource trailingSpace  

// #completionTest(19,20) -> resourceObject
resource foo 'ddd'= 

// wrong resource type
resource foo 'ddd'={
}

resource foo 'ddd'=if (1 + 1 == 2) {
}

// using string interpolation for the resource type
resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= {
}

resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= if (true) {
}

// missing required property
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'={
}

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (name == 'value') {
}

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if ({ 'a': b }.a == 'foo') {
}

// simulate typing if condition
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (true

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (true)

// missing condition
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if {
  name: 'foo'
}

// empty condition
// #completionTest(56) -> symbols
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if () {
  name: 'foo'
}
