
//self-cycle
var x = x
//@[4:5] Variable x
var q = base64(q, !q)
//@[4:5] Variable q

//2-cycle
var a = b
//@[4:5] Variable a
var b = add(a,1)
//@[4:5] Variable b

//3-cycle
var e = f
//@[4:5] Variable e
var f = g && true
//@[4:5] Variable f
var g = e ? e : e
//@[4:5] Variable g
