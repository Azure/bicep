extension mock

param payload string

resource sayHi 'echo' = {
  payload: payload
}

output sayHiResult string = sayHi.payload
