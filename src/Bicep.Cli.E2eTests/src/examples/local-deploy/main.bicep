targetScope = 'local'

extension mock with {
  foo: 'bar'
}

param payload string

resource sayHi 'echo' = {
  payload: payload
}

output sayHiResult string = sayHi.payload
