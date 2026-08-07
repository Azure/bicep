@sys.description('Hello world!')
param parTest string

@description('')
var varTest string = 'Hello world!'

@description('Test')
output outSomething string = varTest

@description('')
type FooConfig = {
  type: 'foo'
  value: int
}

@description('')
type BarConfig = {
  type: 'bar'
  value: bool
}

@discriminator('type')
type ServiceConfig = FooConfig | BarConfig | {
    type: 'baz'
    *: string
}
