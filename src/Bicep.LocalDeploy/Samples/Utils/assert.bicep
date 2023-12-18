provider 'utils@0.0.1'

resource assert 'Assert' = {
  name: 'This should fail!'
  condition: false
}
