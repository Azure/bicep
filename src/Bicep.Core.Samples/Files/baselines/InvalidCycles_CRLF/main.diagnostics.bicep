
//self-cycle
var x = x
//@[08:09) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP079)) |x|
var q = base64(q, !q)
//@[14:21) [BCP071 (Error)] Expected 1 argument, but got 2. (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP071)) |(q, !q)|
//@[15:16) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP079)) |q|
//@[19:20) [BCP079 (Error)] This expression is referencing its own declaration, which is not allowed. (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP079)) |q|

//2-cycle
var a = b
//@[08:09) [BCP080 (Error)] The expression is involved in a cycle ("b" -> "a"). (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP080)) |b|
var b = max(a,1)
//@[12:13) [BCP080 (Error)] The expression is involved in a cycle ("a" -> "b"). (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP080)) |a|

//3-cycle
var e = f
//@[08:09) [BCP080 (Error)] The expression is involved in a cycle ("f" -> "g" -> "e"). (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP080)) |f|
var f = g && true
//@[08:09) [BCP080 (Error)] The expression is involved in a cycle ("g" -> "e" -> "f"). (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP080)) |g|
var g = e ? e : e
//@[08:09) [BCP080 (Error)] The expression is involved in a cycle ("e" -> "f" -> "g"). (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP080)) |e|
//@[12:13) [BCP080 (Error)] The expression is involved in a cycle ("e" -> "f" -> "g"). (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP080)) |e|
//@[16:17) [BCP080 (Error)] The expression is involved in a cycle ("e" -> "f" -> "g"). (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP080)) |e|

//4-cycle
var aa = {
  bb: bb
//@[06:08) [BCP080 (Error)] The expression is involved in a cycle ("bb" -> "cc" -> "dd" -> "aa"). (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP080)) |bb|
}
var bb = {
  cc: cc
//@[06:08) [BCP080 (Error)] The expression is involved in a cycle ("cc" -> "dd" -> "aa" -> "bb"). (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP080)) |cc|
}
var cc = {
  dd: dd
//@[06:08) [BCP080 (Error)] The expression is involved in a cycle ("dd" -> "aa" -> "bb" -> "cc"). (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP080)) |dd|
}
var dd = {
  aa: aa
//@[06:08) [BCP080 (Error)] The expression is involved in a cycle ("aa" -> "bb" -> "cc" -> "dd"). (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP080)) |aa|
}

// variable completion cycles
var one = {
  first: two
//@[09:12) [BCP080 (Error)] The expression is involved in a cycle ("two" -> "one"). (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP080)) |two|
}
// #completionTest(15) -> empty
var two = one.f
//@[10:13) [BCP080 (Error)] The expression is involved in a cycle ("one" -> "two"). (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP080)) |one|
// #completionTest(17) -> empty
var twotwo = one.
//@[04:10) [no-unused-vars (Warning)] Variable "twotwo" is declared but never used. (CodeDescription: Linter(https://aka.ms/bicep/linter/no-unused-vars)) |twotwo|
//@[13:16) [BCP062 (Error)] The referenced declaration with name "one" is not valid. (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP062)) |one|
//@[17:17) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP020)) ||

// resource completion cycles
resource res1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  // #completionTest(14) -> empty
  name: res2.n
//@[08:12) [BCP080 (Error)] The expression is involved in a cycle ("res2" -> "res1"). (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP080)) |res2|
  location: 'l'
  sku: {
    name: 'Premium_LRS'
    // #completionTest(15) -> empty
    tier: res2.
//@[10:14) [BCP080 (Error)] The expression is involved in a cycle ("res2" -> "res1"). (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP080)) |res2|
//@[15:15) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP020)) ||
  }
  kind: 'StorageV2'
}
resource res2 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: res1.name
//@[08:12) [BCP080 (Error)] The expression is involved in a cycle ("res1" -> "res2"). (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP080)) |res1|
  location: 'l'
  sku: {
    name: 'Premium_LRS'
  }
  properties: {
    // #completionTest(21) -> empty
    accessTier: res1.
//@[16:20) [BCP080 (Error)] The expression is involved in a cycle ("res1" -> "res2"). (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP080)) |res1|
//@[21:21) [BCP020 (Error)] Expected a function or property name at this location. (CodeDescription: Core(https://aka.ms/bicep/core-diagnostics#BCP020)) ||
  }
  kind: 'StorageV2'
}
