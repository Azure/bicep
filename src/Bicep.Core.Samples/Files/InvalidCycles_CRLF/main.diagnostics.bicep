
//self-cycle
var x = x
//@[8:9) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. |x|
var q = base64(q, !q)
//@[14:21) [BCP071 (Error)] Expected 1 argument, but got 2. |(q, !q)|
//@[15:16) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. |q|
//@[19:20) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. |q|

//2-cycle
var a = b
//@[8:9) [BCP080 (Error)] The expression is involved in a cycle ("b" -> "a"). |b|
var b = max(a,1)
//@[12:13) [BCP080 (Error)] The expression is involved in a cycle ("a" -> "b"). |a|

//3-cycle
var e = f
//@[8:9) [BCP080 (Error)] The expression is involved in a cycle ("f" -> "g" -> "e"). |f|
var f = g && true
//@[8:9) [BCP080 (Error)] The expression is involved in a cycle ("g" -> "e" -> "f"). |g|
var g = e ? e : e
//@[8:9) [BCP080 (Error)] The expression is involved in a cycle ("e" -> "f" -> "g"). |e|
//@[12:13) [BCP080 (Error)] The expression is involved in a cycle ("e" -> "f" -> "g"). |e|
//@[16:17) [BCP080 (Error)] The expression is involved in a cycle ("e" -> "f" -> "g"). |e|

//4-cycle
var aa = {
  bb: bb
//@[6:8) [BCP080 (Error)] The expression is involved in a cycle ("bb" -> "cc" -> "dd" -> "aa"). |bb|
}
var bb = {
  cc: cc
//@[6:8) [BCP080 (Error)] The expression is involved in a cycle ("cc" -> "dd" -> "aa" -> "bb"). |cc|
}
var cc = {
  dd: dd
//@[6:8) [BCP080 (Error)] The expression is involved in a cycle ("dd" -> "aa" -> "bb" -> "cc"). |dd|
}
var dd = {
  aa: aa
//@[6:8) [BCP080 (Error)] The expression is involved in a cycle ("aa" -> "bb" -> "cc" -> "dd"). |aa|
}

// variable completion cycles
var one = {
  first: two
//@[9:12) [BCP080 (Error)] The expression is involved in a cycle ("two" -> "one"). |two|
}
// #completionTest(15) -> empty
var two = one.f
//@[10:13) [BCP080 (Error)] The expression is involved in a cycle ("one" -> "two"). |one|
// #completionTest(17) -> empty
var twotwo = one.
//@[13:16) [BCP080 (Error)] The expression is involved in a cycle ("one" -> "two"). |one|
//@[17:17) [BCP020 (Error)] Expected a function or property name at this location. ||

// resource completion cycles
resource res1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  // #completionTest(14) -> empty
  name: res2.n
//@[8:12) [BCP080 (Error)] The expression is involved in a cycle ("res2" -> "res1"). |res2|
  location: 'l'
  sku: {
    name: 'Premium_LRS'
    // #completionTest(15) -> empty
    tier: res2.
//@[10:14) [BCP080 (Error)] The expression is involved in a cycle ("res2" -> "res1"). |res2|
//@[15:15) [BCP020 (Error)] Expected a function or property name at this location. ||
  }
  kind: 'StorageV2'
}
resource res2 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: res1.name
//@[8:12) [BCP080 (Error)] The expression is involved in a cycle ("res1" -> "res2"). |res1|
  location: 'l'
  sku: {
    name: 'Premium_LRS'
  }
  properties: {
    // #completionTest(21) -> empty
    accessTier: res1.
//@[16:20) [BCP080 (Error)] The expression is involved in a cycle ("res1" -> "res2"). |res1|
//@[21:21) [BCP020 (Error)] Expected a function or property name at this location. ||
  }
  kind: 'StorageV2'
}
