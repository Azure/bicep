
//self-cycle
var x = x
var q = base64(q, !q)

//2-cycle
var a = b
var b = max(a,1)

//3-cycle
var e = f
var f = g && true
var g = e ? e : e

//4-cycle
var aa = {
  bb: bb
}
var bb = {
  cc: cc
}
var cc = {
  dd: dd
}
var dd = {
  aa: aa
}

// variable completion cycles
var one = {
  first: two
}
// #completionTest(15) -> empty
var two = one.f
// #completionTest(17) -> empty
var twotwo = one.

// resource completion cycles
resource res1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
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