var foo = 'abc'
var bar = guid(subscription().id, 'xxxx', foo)
var abc = guid('blah')
var def = {
  '1234': '1234'
  '${guid('blah')}': guid('blah')
}
