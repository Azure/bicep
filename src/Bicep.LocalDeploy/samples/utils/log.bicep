provider 'utils@0.0.1'

param name string

resource log 'Log' = {
  message: 'Hello ${name}!'
}
