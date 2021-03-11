param test string

var test_var = 'abcdef${test}ghi/jkl.${test}'
var concats = 'abcdefghijkllmopqr'
var formats = '>>abc<<>>---abc.def---<<'
var escaped = '[]'
var keyescaping = {
  '[]': 'shouldbeescaped'
  '>>abc<<>>---abc.def---<<': 'shouldbeescaped'
  'abcdef${test}ghi/jkl.${test}': 'shouldbeescaped'
}
