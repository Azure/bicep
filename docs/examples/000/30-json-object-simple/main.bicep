//Json object samples

//Array in JSON String
var jsonString = '''
[
  "one",
  "two",
  "three"
]
'''
var jsonArray = json(jsonString)

//Array in flat JSON String
var jsonString2 = '["four","five","six"]'
var jsonArray2 = json(jsonString2)

output jsonArray array = [for (name, i) in jsonArray: {
  name: name
}]

output jsonArray2 array = [for (name, i) in jsonArray2: {
  name: name
}]
