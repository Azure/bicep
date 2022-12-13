param foo string

module baz 'bar/baz.bicep' = {
  name: 'baz'
  params: {
    foo: foo
  }
}

output foo string = baz.outputs.foo
