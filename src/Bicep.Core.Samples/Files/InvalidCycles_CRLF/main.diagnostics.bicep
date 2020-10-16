
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
var b = add(a,1)
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
