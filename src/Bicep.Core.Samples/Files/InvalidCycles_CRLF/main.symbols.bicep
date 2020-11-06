
//self-cycle
var x = x
//@[4:5) Variable x. Type: error. Declaration start char: 0, length: 9
var q = base64(q, !q)
//@[4:5) Variable q. Type: error. Declaration start char: 0, length: 21

//2-cycle
var a = b
//@[4:5) Variable a. Type: error. Declaration start char: 0, length: 9
var b = max(a,1)
//@[4:5) Variable b. Type: error. Declaration start char: 0, length: 16

//3-cycle
var e = f
//@[4:5) Variable e. Type: error. Declaration start char: 0, length: 9
var f = g && true
//@[4:5) Variable f. Type: error. Declaration start char: 0, length: 17
var g = e ? e : e
//@[4:5) Variable g. Type: error. Declaration start char: 0, length: 17

//4-cycle
var aa = {
//@[4:6) Variable aa. Type: error. Declaration start char: 0, length: 23
  bb: bb
}
var bb = {
//@[4:6) Variable bb. Type: error. Declaration start char: 0, length: 23
  cc: cc
}
var cc = {
//@[4:6) Variable cc. Type: error. Declaration start char: 0, length: 23
  dd: dd
}
var dd = {
//@[4:6) Variable dd. Type: error. Declaration start char: 0, length: 23
  aa: aa
}

// variable completion cycles
var one = {
//@[4:7) Variable one. Type: error. Declaration start char: 0, length: 28
  first: two
}
// #completionTest(15) -> empty
var two = one.f
//@[4:7) Variable two. Type: error. Declaration start char: 0, length: 15
// #completionTest(17) -> empty
var twotwo = one.
//@[4:10) Variable twotwo. Type: error. Declaration start char: 0, length: 17

// resource completion cycles
resource res1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[9:13) Resource res1. Type: Microsoft.Storage/storageAccounts@2019-06-01. Declaration start char: 0, length: 250
  // #completionTest(14) -> empty
  name: res2.n
  location: 'l'
  sku: {
    name: 'Premium_LRS'
    // #completionTest(15) -> empty
    tier: res2.
  }
  kind: 'StorageV2'
}
resource res2 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[9:13) Resource res2. Type: Microsoft.Storage/storageAccounts@2019-06-01. Declaration start char: 0, length: 246
  name: res1.name
  location: 'l'
  sku: {
    name: 'Premium_LRS'
  }
  properties: {
    // #completionTest(21) -> empty
    accessTier: res1.
  }
  kind: 'StorageV2'
}
