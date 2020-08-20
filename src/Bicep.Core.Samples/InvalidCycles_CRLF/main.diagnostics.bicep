
//self-cycle
var x = x
//@[8:9) Error The expression is involved in a cycle. |x|
var q = base64(q, !q)
//@[15:16) Error The expression is involved in a cycle. |q|
//@[19:20) Error The expression is involved in a cycle. |q|

//2-cycle
var a = b
//@[8:9) Error The expression is involved in a cycle. |b|
var b = add(a,1)
//@[12:13) Error The expression is involved in a cycle. |a|

//3-cycle
var e = f
//@[8:9) Error The expression is involved in a cycle. |f|
var f = g && true
//@[8:9) Error The expression is involved in a cycle. |g|
var g = e ? e : e
//@[8:9) Error The expression is involved in a cycle. |e|
//@[12:13) Error The expression is involved in a cycle. |e|
//@[16:17) Error The expression is involved in a cycle. |e|
