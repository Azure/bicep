
//self-cycle
variable x = x
variable q = base64(q, !q)

//2-cycle
variable a = b
variable b = add(a,1)

//3-cycle
variable e = f
variable f = g && true
variable g = e ? e : e