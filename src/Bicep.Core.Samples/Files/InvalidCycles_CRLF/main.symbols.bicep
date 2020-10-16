
//self-cycle
var x = x
//@[4:5) Variable x. Type: error. Declaration start char: 0, length: 9
var q = base64(q, !q)
//@[4:5) Variable q. Type: error. Declaration start char: 0, length: 21

//2-cycle
var a = b
//@[4:5) Variable a. Type: error. Declaration start char: 0, length: 9
var b = add(a,1)
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
