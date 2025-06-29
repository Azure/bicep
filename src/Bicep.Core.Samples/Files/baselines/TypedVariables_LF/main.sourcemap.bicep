@export()
var exportedString string = 'foo'
//@        "type": {
//@          "type": "string"
//@        }
//@    "exportedString": "foo",

@export()
var exporteInlineType {
//@        "type": {
//@          "type": "object",
//@          "properties": {
//@          }
//@        }
  foo: string
//@            "foo": {
//@              "type": "string"
//@            },
  bar: int
//@            "bar": {
//@              "type": "int"
//@            }
} = {
//@    "exporteInlineType": {
//@    },
  foo: 'abc'
//@      "foo": "abc",
  bar: 123
//@      "bar": 123
}

type FooType = {
//@    "FooType": {
//@      "type": "object",
//@      "properties": {
//@      }
//@    }
  foo: string
//@        "foo": {
//@          "type": "string"
//@        },
  bar: int
//@        "bar": {
//@          "type": "int"
//@        }
}

@export()
var exportedTypeRef FooType = {
//@        "type": {
//@          "$ref": "#/definitions/FooType"
//@        }
//@    "exportedTypeRef": {
//@    },
  foo: 'abc'
//@      "foo": "abc",
  bar: 123
//@      "bar": 123
}

var unExported FooType = {
//@    "unExported": {
//@    }
  foo: 'abc'
//@      "foo": "abc",
  bar: 123
//@      "bar": 123
}

