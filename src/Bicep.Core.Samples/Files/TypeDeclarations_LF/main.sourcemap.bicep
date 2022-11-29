@description('The foo type')
//@        "description": "The foo type"
@sealed()
type foo = {
//@    "foo": {
//@      "type": "object",
//@      "required": [
//@        "stringProp",
//@        "objectProp",
//@        "typeRefProp",
//@        "literalProp"
//@      ],
//@      "properties": {
//@        "stringProp": {
//@          "type": "string",
//@          "metadata": {
//@          },
//@        },
//@        "objectProp": {
//@          "type": "object",
//@          "required": [
//@            "intProp"
//@          ],
//@          "properties": {
//@            "intProp": {
//@              "type": "int",
//@            },
//@            "intArrayArrayProp": {
//@              "type": "array",
//@              "items": {
//@                "type": "array",
//@                "items": {
//@                  "type": "int"
//@                }
//@              }
//@            }
//@          }
//@        },
//@        "typeRefProp": {
//@          "$ref": "#/definitions/bar"
//@        },
//@        "literalProp": {
//@          "type": "string",
//@          "allowedValues": [
//@          ]
//@        },
//@        "recursion": {
//@          "$ref": "#/definitions/foo"
//@        }
//@      },
//@      "additionalProperties": false,
//@      "metadata": {
//@      }
//@    },
  @minLength(3)
//@          "minLength": 3
  @maxLength(10)
//@          "maxLength": 10,
  @description('A string property')
//@            "description": "A string property"
  stringProp: string

  objectProp: {
    @minValue(1)
//@              "minValue": 1
    intProp: int

    intArrayArrayProp?: int [] []
  }

  typeRefProp: bar

  literalProp: 'literal'
//@            "literal"

  recursion?: foo
}

@minLength(3)
//@      "minLength": 3
@description('An array of array of arrays of arrays of ints')
//@        "description": "An array of array of arrays of arrays of ints"
@metadata({
  examples: [
//@        "examples": [
//@        ],
    [[[[1]]], [[[2]]], [[[3]]]]
//@          [
//@            [
//@              [
//@                [
//@                  1
//@                ]
//@              ]
//@            ],
//@            [
//@              [
//@                [
//@                  2
//@                ]
//@              ]
//@            ],
//@            [
//@              [
//@                [
//@                  3
//@                ]
//@              ]
//@            ]
//@          ]
  ]
})
type bar = int[][][][]
//@    "bar": {
//@      "type": "array",
//@      "items": {
//@        "type": "array",
//@        "items": {
//@          "type": "array",
//@          "items": {
//@            "type": "array",
//@            "items": {
//@              "type": "int"
//@            }
//@          }
//@        }
//@      },
//@      "metadata": {
//@      },
//@    },

type aUnion = 'snap'|'crackle'|'pop'
//@    "aUnion": {
//@      "type": "string",
//@      "allowedValues": [
//@        "crackle",
//@        "pop",
//@        "snap"
//@      ]
//@    },

type expandedUnion = aUnion|'fizz'|'buzz'|'pop'
//@    "expandedUnion": {
//@      "type": "string",
//@      "allowedValues": [
//@        "buzz",
//@        "crackle",
//@        "fizz",
//@        "pop",
//@        "snap"
//@      ]
//@    },

type tupleUnion = ['foo', 'bar', 'baz']|['fizz', 'buzz']|['snap', 'crackle', 'pop']
//@    "tupleUnion": {
//@      "type": "array",
//@      "allowedValues": [
//@        [
//@          "fizz",
//@          "buzz"
//@        ],
//@        [
//@          "foo",
//@          "bar",
//@          "baz"
//@        ],
//@        [
//@          "snap",
//@          "crackle",
//@          "pop"
//@        ]
//@      ]
//@    },

type mixedArray = ('heffalump'|'woozle'|{ shape: '*', size: '*'}|10|-10|true|!true|null)[]
//@    "mixedArray": {
//@      "type": "array",
//@      "allowedValues": [
//@        "heffalump",
//@        "woozle",
//@        -10,
//@        10,
//@        false,
//@        null,
//@        true,
//@        {
//@          "shape": "*",
//@          "size": "*"
//@        }
//@      ]
//@    },

type bool = string
//@    "bool": {
//@      "type": "string"
//@    },

param inlineObjectParam {
//@    "inlineObjectParam": {
//@      "type": "object",
//@      "required": [
//@        "foo",
//@        "bar",
//@        "baz"
//@      ],
//@      "properties": {
//@        "foo": {
//@          "type": "string"
//@        },
//@        "bar": {
//@          "type": "int",
//@          "allowedValues": [
//@            100,
//@            200,
//@            300,
//@            400,
//@            500
//@          ]
//@        },
//@        "baz": {
//@          "type": "bool"
//@        }
//@      },
//@    },
  foo: string
  bar: 100|200|300|400|500
  baz: sys.bool
} = {
//@      "defaultValue": {
//@      }
  foo: 'foo'
//@        "foo": "foo",
  bar: 300
//@        "bar": 300,
  baz: false
//@        "baz": false
}

param unionParam {property: 'ping'}|{property: 'pong'} = {property: 'pong'}
//@    "unionParam": {
//@      "type": "object",
//@      "allowedValues": [
//@        {
//@          "property": "ping"
//@        },
//@        {
//@          "property": "pong"
//@        }
//@      ],
//@      "defaultValue": {
//@        "property": "pong"
//@      }
//@    },

param paramUsingType mixedArray
//@    "paramUsingType": {
//@      "$ref": "#/definitions/mixedArray"
//@    }

type tuple = [
//@    "tuple": {
//@      "type": "array",
//@      "prefixItems": [
//@        {
//@          "type": "string",
//@          "metadata": {
//@          }
//@        },
//@        {
//@          "$ref": "#/definitions/bar",
//@          "metadata": {
//@          }
//@        }
//@      ],
//@      "items": false
//@    }
    @description('A leading string')
//@            "description": "A leading string"
    string

    @description('A second element using a type alias')
//@            "description": "A second element using a type alias"
    bar
]
