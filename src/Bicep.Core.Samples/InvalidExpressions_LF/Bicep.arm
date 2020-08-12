/*
  This tests the various cases of invalid expressions.
*/

// bad expressions
variable bad = a+
variable bad = *
variable bad = /
variable bad = %
variable bad = 33-
variable bad = --33
variable bad = 3 * 4 /
variable bad = 222222222222222222222222222222222222222222 * 4
variable bad = (null) ?
variable bad = (null) ? :
variable bad = (null) ? !
variable bad = (null)!
variable bad = (null)[0]
variable bad = ()
variable bad = {}
variable bad = []
variable bad = 

// variables not supported
variable x = a + 2

// unary NOT
variable not = !null
variable not = !4
variable not = !'s'
variable not = ![
]
variable not = !{
}

// unary not chaining will be added in the future
variable not = !!!!!!!true

// unary minus chaining will not be supported (to reserve -- in case we need it)
variable minus = ------12

// unary minus
variable minus = -true
variable minus = -null
variable minus = -'s'
variable minus = -[
]
variable minus = -{
}

// multiplicative
variable mod = 's' % true
variable mul = true * null
variable div = {
} / [
]

// additive
variable add = null + 's'
variable sub = true - false

// equality (== and != can't have a type error because they work on "any" type)
variable eq = true =~ null
variable ne = 15 !~ [
]

// relational
variable lt = 4 < 's'
variable lteq = null <= 10
variable gt = false>[
]
variable gteq = {
} >= false

// logical
variable and = null && 'a'
variable or = 10 || 4

// conditional
variable ternary = null ? 4 : false

// complex expressions
variable complex = test(2 + 3*4, true || false && null)
variable complex = -2 && 3 && !4 && 5
variable complex = null ? !4: false
variable complex = true == false != null == 4 != 'a' ? -2 && 3 && !4 && 5 : true || false && null

variable nestedTernary = null ? 1 : 2 ? true ? 'a': 'b' : false ? 'd' : 15
variable nestedTernary = (null ? 1 : 2) ? (true ? 'a': 'b') : (false ? 'd' : 15)

// bad array access
variable errorInsideArrayAccess = [
  !null
][!0]
variable integerIndexOnNonArray = (null)[0]
variable stringIndexOnNonObject = 'test'['test']
variable malformedStringIndex = {
}['test\e']

// bad propertyAccess
variable dotAccessOnNonObject = true.foo
variable badExpressionInPropertyAccess = resourceGroup()[!'location']

variable propertyAccessOnVariable = x.foo
