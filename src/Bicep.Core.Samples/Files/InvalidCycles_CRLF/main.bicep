
//self-cycle
var x = x
var q = base64(q, !q)

//2-cycle
var a = b
var b = add(a,1)

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