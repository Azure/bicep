
//self-cycle
var x = x
//@[4:5) Variable x. Declaration start char: 0, length: 11
var q = base64(q, !q)
//@[4:5) Variable q. Declaration start char: 0, length: 25

//2-cycle
var a = b
//@[4:5) Variable a. Declaration start char: 0, length: 11
var b = add(a,1)
//@[4:5) Variable b. Declaration start char: 0, length: 20

//3-cycle
var e = f
//@[4:5) Variable e. Declaration start char: 0, length: 11
var f = g && true
//@[4:5) Variable f. Declaration start char: 0, length: 19
var g = e ? e : e
//@[4:5) Variable g. Declaration start char: 0, length: 17
