@description('The foo type')
//@      "metadata": {
//@        "description": "The foo type"
//@      }
@sealed()
//@      "additionalProperties": false,
type foo = {
//@    "foo": {
//@      "type": "object",
//@      "properties": {
//@      },
//@    },
  @minLength(3)
//@          "minLength": 3,
  @maxLength(10)
//@          "maxLength": 10,
  @description('A string property')
//@          "metadata": {
//@            "description": "A string property"
//@          }
  stringProp: string
//@        "stringProp": {
//@          "type": "string",
//@        },

  objectProp: {
//@        "objectProp": {
//@          "type": "object",
//@          "properties": {
//@          }
//@        },
    @minValue(1)
//@              "minValue": 1
    intProp: int
//@            "intProp": {
//@              "type": "int",
//@            },

    intArrayArrayProp: int [] [] ?
//@            "intArrayArrayProp": {
//@              "type": "array",
//@              "items": {
//@                "type": "array",
//@                "items": {
//@                  "type": "int"
//@                }
//@              },
//@              "nullable": true
//@            }
  }

  typeRefProp: bar
//@        "typeRefProp": {
//@          "$ref": "#/definitions/bar"
//@        },

  literalProp: 'literal'
//@        "literalProp": {
//@          "type": "string",
//@          "allowedValues": [
//@            "literal"
//@          ]
//@        },

  recursion: foo?
//@        "recursion": {
//@          "$ref": "#/definitions/foo",
//@          "nullable": true
//@        }
}

@minLength(3)
//@      "minLength": 3
@description('An array of array of arrays of arrays of ints')
//@        "description": "An array of array of arrays of arrays of ints"
@metadata({
//@      "metadata": {
//@      },
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

type tupleUnion = ['foo', 'bar', 'baz']
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
|['fizz', 'buzz']
|['snap', 'crackle', 'pop']

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
//@      "properties": {
//@      },
//@    },
  foo: string
//@        "foo": {
//@          "type": "string"
//@        },
  bar: 100|200|300|400|500
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
  baz: sys.bool
//@        "baz": {
//@          "type": "bool"
//@        }
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
//@    },

output outputUsingType mixedArray = paramUsingType
//@    "outputUsingType": {
//@      "$ref": "#/definitions/mixedArray",
//@      "value": "[parameters('paramUsingType')]"
//@    },

type tuple = [
//@    "tuple": {
//@      "type": "array",
//@      "prefixItems": [
//@      ],
//@      "items": false
//@    },
    @description('A leading string')
//@          "metadata": {
//@            "description": "A leading string"
//@          }
    string
//@        {
//@          "type": "string",
//@        },

    @description('A second element using a type alias')
//@          "metadata": {
//@            "description": "A second element using a type alias"
//@          }
    bar
//@        {
//@          "$ref": "#/definitions/bar",
//@        }
]

type stringStringDictionary = {
//@    "stringStringDictionary": {
//@      "type": "object",
//@    },
    *: string
//@      "additionalProperties": {
//@        "type": "string"
//@      }
}

@minValue(1)
//@      "minValue": 1,
@maxValue(10)
//@      "maxValue": 10
type constrainedInt = int
//@    "constrainedInt": {
//@      "type": "int",
//@    },

param mightIncludeNull ({key: 'value'} | null)[]
//@    "mightIncludeNull": {
//@      "type": "array",
//@      "allowedValues": [
//@        null,
//@        {
//@          "key": "value"
//@        }
//@      ]
//@    },

var nonNull = mightIncludeNull[0]!.key
//@    "nonNull": "[parameters('mightIncludeNull')[0].key]",

output nonNull string = nonNull
//@    "nonNull": {
//@      "type": "string",
//@      "value": "[variables('nonNull')]"
//@    },

var maybeNull = mightIncludeNull[0].?key
//@    "maybeNull": "[tryGet(parameters('mightIncludeNull')[0], 'key')]"

output maybeNull string? = maybeNull
//@    "maybeNull": {
//@      "type": "string",
//@      "nullable": true,
//@      "value": "[variables('maybeNull')]"
//@    },

type nullable = string?
//@    "nullable": {
//@      "type": "string",
//@      "nullable": true
//@    },

type nonNullable = nullable!
//@    "nonNullable": {
//@      "$ref": "#/definitions/nullable",
//@      "nullable": false
//@    },

type typeA = {
//@    "typeA": {
//@      "type": "object",
//@      "properties": {
//@      }
//@    },
  type: 'a'
//@        "type": {
//@          "type": "string",
//@          "allowedValues": [
//@            "a"
//@          ]
//@        },
  value: string
//@        "value": {
//@          "type": "string"
//@        }
}

type typeB = {
//@    "typeB": {
//@      "type": "object",
//@      "properties": {
//@      }
//@    },
  type: 'b'
//@        "type": {
//@          "type": "string",
//@          "allowedValues": [
//@            "b"
//@          ]
//@        },
  value: int
//@        "value": {
//@          "type": "int"
//@        }
}

type typeC = {
//@    "typeC": {
//@      "type": "object",
//@      "properties": {
//@      }
//@    },
  type: 'c'
//@        "type": {
//@          "type": "string",
//@          "allowedValues": [
//@            "c"
//@          ]
//@        },
  value: bool
//@        "value": {
//@          "$ref": "#/definitions/bool"
//@        },
  value2: string
//@        "value2": {
//@          "type": "string"
//@        }
}

type typeD = {
//@    "typeD": {
//@      "type": "object",
//@      "properties": {
//@      }
//@    },
  type: 'd'
//@        "type": {
//@          "type": "string",
//@          "allowedValues": [
//@            "d"
//@          ]
//@        },
  value: object
//@        "value": {
//@          "type": "object"
//@        }
}

type typeE = {
//@    "typeE": {
//@      "type": "object",
//@      "properties": {
//@      }
//@    },
  type: 'e'
//@        "type": {
//@          "type": "string",
//@          "allowedValues": [
//@            "e"
//@          ]
//@        },
  value: 'a' | 'b'
//@        "value": {
//@          "type": "string",
//@          "allowedValues": [
//@            "a",
//@            "b"
//@          ]
//@        }
}

type typeF = {
//@    "typeF": {
//@      "type": "object",
//@      "properties": {
//@      },
//@    },
  type: 'f'
//@        "type": {
//@          "type": "string",
//@          "allowedValues": [
//@            "f"
//@          ]
//@        }
  *: string
//@      "additionalProperties": {
//@        "type": "string"
//@      }
}

@discriminator('type')
type discriminatedUnion1 = typeA | typeB
//@    "discriminatedUnion1": {
//@      "type": "object",
//@      "discriminator": {
//@        "propertyName": "type",
//@        "mapping": {
//@          "a": {
//@            "$ref": "#/definitions/typeA"
//@          },
//@          "b": {
//@            "$ref": "#/definitions/typeB"
//@          }
//@        }
//@      }
//@    },
//@          "a": {
//@            "$ref": "#/definitions/typeA"
//@          },
//@          "b": {
//@            "$ref": "#/definitions/typeB"
//@          },
//@          "a": {
//@            "$ref": "#/definitions/typeA"
//@          },
//@          "b": {
//@            "$ref": "#/definitions/typeB"
//@          },
//@                    "a": {
//@                      "$ref": "#/definitions/typeA"
//@                    },
//@                    "b": {
//@                      "$ref": "#/definitions/typeB"
//@                    },

@discriminator('type')
type discriminatedUnion2 = { type: 'c', value: string } | { type: 'd', value: bool }
//@    "discriminatedUnion2": {
//@      "type": "object",
//@      "discriminator": {
//@        "propertyName": "type",
//@        "mapping": {
//@          "c": {
//@            "type": "object",
//@            "properties": {
//@              "type": {
//@                "type": "string",
//@                "allowedValues": [
//@                  "c"
//@                ]
//@              },
//@              "value": {
//@                "type": "string"
//@              }
//@            }
//@          },
//@          "d": {
//@            "type": "object",
//@            "properties": {
//@              "type": {
//@                "type": "string",
//@                "allowedValues": [
//@                  "d"
//@                ]
//@              },
//@              "value": {
//@                "$ref": "#/definitions/bool"
//@              }
//@            }
//@          }
//@        }
//@      }
//@    },
//@          "c": {
//@            "type": "object",
//@            "properties": {
//@              "type": {
//@                "type": "string",
//@                "allowedValues": [
//@                  "c"
//@                ]
//@              },
//@              "value": {
//@                "type": "string"
//@              }
//@            }
//@          },
//@          "d": {
//@            "type": "object",
//@            "properties": {
//@              "type": {
//@                "type": "string",
//@                "allowedValues": [
//@                  "d"
//@                ]
//@              },
//@              "value": {
//@                "$ref": "#/definitions/bool"
//@              }
//@            }
//@          },
//@          "c": {
//@            "type": "object",
//@            "properties": {
//@              "type": {
//@                "type": "string",
//@                "allowedValues": [
//@                  "c"
//@                ]
//@              },
//@              "value": {
//@                "type": "string"
//@              }
//@            }
//@          },
//@          "d": {
//@            "type": "object",
//@            "properties": {
//@              "type": {
//@                "type": "string",
//@                "allowedValues": [
//@                  "d"
//@                ]
//@              },
//@              "value": {
//@                "$ref": "#/definitions/bool"
//@              }
//@            }
//@          },
//@                    "c": {
//@                      "type": "object",
//@                      "properties": {
//@                        "type": {
//@                          "type": "string",
//@                          "allowedValues": [
//@                            "c"
//@                          ]
//@                        },
//@                        "value": {
//@                          "type": "string"
//@                        }
//@                      }
//@                    },
//@                    "d": {
//@                      "type": "object",
//@                      "properties": {
//@                        "type": {
//@                          "type": "string",
//@                          "allowedValues": [
//@                            "d"
//@                          ]
//@                        },
//@                        "value": {
//@                          "$ref": "#/definitions/bool"
//@                        }
//@                      }
//@                    }

@discriminator('type')
type discriminatedUnion3 = discriminatedUnion1 | discriminatedUnion2 | { type: 'e', value: string }
//@    "discriminatedUnion3": {
//@      "type": "object",
//@      "discriminator": {
//@        "propertyName": "type",
//@        "mapping": {
//@          "e": {
//@            "type": "object",
//@            "properties": {
//@              "type": {
//@                "type": "string",
//@                "allowedValues": [
//@                  "e"
//@                ]
//@              },
//@              "value": {
//@                "type": "string"
//@              }
//@            }
//@          }
//@        }
//@      }
//@    },

@discriminator('type')
type discriminatedUnion4 = discriminatedUnion1 | (discriminatedUnion2 | typeE)
//@    "discriminatedUnion4": {
//@      "type": "object",
//@      "discriminator": {
//@        "propertyName": "type",
//@        "mapping": {
//@          "e": {
//@            "$ref": "#/definitions/typeE"
//@          }
//@        }
//@      }
//@    },

@discriminator('type')
type discriminatedUnion5 = (typeA | typeB)?
//@    "discriminatedUnion5": {
//@      "type": "object",
//@      "discriminator": {
//@        "propertyName": "type",
//@        "mapping": {
//@          "a": {
//@            "$ref": "#/definitions/typeA"
//@          },
//@          "b": {
//@            "$ref": "#/definitions/typeB"
//@          }
//@        }
//@      },
//@      "nullable": true
//@    },

@discriminator('type')
type discriminatedUnion6 = (typeA | typeB)!
//@    "discriminatedUnion6": {
//@      "type": "object",
//@      "discriminator": {
//@        "propertyName": "type",
//@        "mapping": {
//@          "a": {
//@            "$ref": "#/definitions/typeA"
//@          },
//@          "b": {
//@            "$ref": "#/definitions/typeB"
//@          }
//@        }
//@      },
//@      "nullable": false
//@    },

type inlineDiscriminatedUnion1 = {
//@    "inlineDiscriminatedUnion1": {
//@      "type": "object",
//@      "properties": {
//@      }
//@    },
  @discriminator('type')
  prop: typeA | typeC
//@        "prop": {
//@          "type": "object",
//@          "discriminator": {
//@            "propertyName": "type",
//@            "mapping": {
//@              "a": {
//@                "$ref": "#/definitions/typeA"
//@              },
//@              "c": {
//@                "$ref": "#/definitions/typeC"
//@              }
//@            }
//@          }
//@        }
}

type inlineDiscriminatedUnion2 = {
//@    "inlineDiscriminatedUnion2": {
//@      "type": "object",
//@      "properties": {
//@      }
//@    },
  @discriminator('type')
  prop: { type: 'a', value: bool } | typeB
//@        "prop": {
//@          "type": "object",
//@          "discriminator": {
//@            "propertyName": "type",
//@            "mapping": {
//@              "a": {
//@                "type": "object",
//@                "properties": {
//@                  "type": {
//@                    "type": "string",
//@                    "allowedValues": [
//@                      "a"
//@                    ]
//@                  },
//@                  "value": {
//@                    "$ref": "#/definitions/bool"
//@                  }
//@                }
//@              },
//@              "b": {
//@                "$ref": "#/definitions/typeB"
//@              }
//@            }
//@          }
//@        }
}

@discriminator('type')
type inlineDiscriminatedUnion3 = {
//@    "inlineDiscriminatedUnion3": {
//@      "type": "object",
//@      "discriminator": {
//@        "propertyName": "type",
//@        "mapping": {
//@          "a": {
//@            "type": "object",
//@            "properties": {
//@            }
//@          },
//@        }
//@      }
//@    },
  type: 'a'
//@              "type": {
//@                "type": "string",
//@                "allowedValues": [
//@                  "a"
//@                ]
//@              },
  @discriminator('type')
  prop: { type: 'a', value: bool } | typeB
//@              "prop": {
//@                "type": "object",
//@                "discriminator": {
//@                  "propertyName": "type",
//@                  "mapping": {
//@                    "a": {
//@                      "type": "object",
//@                      "properties": {
//@                        "type": {
//@                          "type": "string",
//@                          "allowedValues": [
//@                            "a"
//@                          ]
//@                        },
//@                        "value": {
//@                          "$ref": "#/definitions/bool"
//@                        }
//@                      }
//@                    },
//@                    "b": {
//@                      "$ref": "#/definitions/typeB"
//@                    }
//@                  }
//@                }
//@              }
} | {
//@          "b": {
//@            "type": "object",
//@            "properties": {
//@            }
//@          }
  type: 'b'
//@              "type": {
//@                "type": "string",
//@                "allowedValues": [
//@                  "b"
//@                ]
//@              },
  @discriminator('type')
  prop: discriminatedUnion1 | discriminatedUnion2
//@              "prop": {
//@                "type": "object",
//@                "discriminator": {
//@                  "propertyName": "type",
//@                  "mapping": {
//@                  }
//@                }
//@              }
}

type inlineDiscriminatedUnion4 = {
//@    "inlineDiscriminatedUnion4": {
//@      "type": "object",
//@      "properties": {
//@      }
//@    },
  @discriminator('type')
  prop: (typeA | typeC)?
//@        "prop": {
//@          "type": "object",
//@          "discriminator": {
//@            "propertyName": "type",
//@            "mapping": {
//@              "a": {
//@                "$ref": "#/definitions/typeA"
//@              },
//@              "c": {
//@                "$ref": "#/definitions/typeC"
//@              }
//@            }
//@          },
//@          "nullable": true
//@        }
}

type discriminatorUnionAsPropertyType = {
//@    "discriminatorUnionAsPropertyType": {
//@      "type": "object",
//@      "properties": {
//@      }
//@    },
  prop1: discriminatedUnion1
//@        "prop1": {
//@          "$ref": "#/definitions/discriminatedUnion1"
//@        },
  prop2: discriminatedUnion3
//@        "prop2": {
//@          "$ref": "#/definitions/discriminatedUnion3"
//@        }
}

type discriminatedUnionInlineAdditionalProps1 = {
//@    "discriminatedUnionInlineAdditionalProps1": {
//@      "type": "object",
//@    },
  @discriminator('type')
  *: typeA | typeB
//@      "additionalProperties": {
//@        "type": "object",
//@        "discriminator": {
//@          "propertyName": "type",
//@          "mapping": {
//@            "a": {
//@              "$ref": "#/definitions/typeA"
//@            },
//@            "b": {
//@              "$ref": "#/definitions/typeB"
//@            }
//@          }
//@        }
//@      }
}

type discriminatedUnionInlineAdditionalProps2 = {
//@    "discriminatedUnionInlineAdditionalProps2": {
//@      "type": "object",
//@    },
  @discriminator('type')
  *: (typeA | typeB)?
//@      "additionalProperties": {
//@        "type": "object",
//@        "discriminator": {
//@          "propertyName": "type",
//@          "mapping": {
//@            "a": {
//@              "$ref": "#/definitions/typeA"
//@            },
//@            "b": {
//@              "$ref": "#/definitions/typeB"
//@            }
//@          }
//@        },
//@        "nullable": true
//@      }
}

@discriminator('type')
type discriminatorMemberHasAdditionalProperties1 = typeA | typeF | { type: 'g', *: int }
//@    "discriminatorMemberHasAdditionalProperties1": {
//@      "type": "object",
//@      "discriminator": {
//@        "propertyName": "type",
//@        "mapping": {
//@          "a": {
//@            "$ref": "#/definitions/typeA"
//@          },
//@          "f": {
//@            "$ref": "#/definitions/typeF"
//@          },
//@          "g": {
//@            "type": "object",
//@            "properties": {
//@              "type": {
//@                "type": "string",
//@                "allowedValues": [
//@                  "g"
//@                ]
//@              }
//@            },
//@            "additionalProperties": {
//@              "type": "int"
//@            }
//@          }
//@        }
//@      }
//@    },

@discriminator('type')
type discriminatorInnerSelfOptionalCycle1 = typeA | {
//@    "discriminatorInnerSelfOptionalCycle1": {
//@      "type": "object",
//@      "discriminator": {
//@        "propertyName": "type",
//@        "mapping": {
//@          "a": {
//@            "$ref": "#/definitions/typeA"
//@          },
//@          "b": {
//@            "type": "object",
//@            "properties": {
//@            }
//@          }
//@        }
//@      }
//@    },
  type: 'b'
//@              "type": {
//@                "type": "string",
//@                "allowedValues": [
//@                  "b"
//@                ]
//@              },
  value: discriminatorInnerSelfOptionalCycle1?
//@              "value": {
//@                "$ref": "#/definitions/discriminatorInnerSelfOptionalCycle1",
//@                "nullable": true
//@              }
}

type discriminatedUnionMemberOptionalCycle1 = {
//@    "discriminatedUnionMemberOptionalCycle1": {
//@      "type": "object",
//@      "properties": {
//@      }
//@    },
  type: 'b'
//@        "type": {
//@          "type": "string",
//@          "allowedValues": [
//@            "b"
//@          ]
//@        },
  @discriminator('type')
  prop: (typeA | discriminatedUnionMemberOptionalCycle1)?
//@        "prop": {
//@          "type": "object",
//@          "discriminator": {
//@            "propertyName": "type",
//@            "mapping": {
//@              "a": {
//@                "$ref": "#/definitions/typeA"
//@              },
//@              "b": {
//@                "$ref": "#/definitions/discriminatedUnionMemberOptionalCycle1"
//@              }
//@            }
//@          },
//@          "nullable": true
//@        }
}

type discriminatedUnionTuple1 = [
//@    "discriminatedUnionTuple1": {
//@      "type": "array",
//@      "prefixItems": [
//@      ],
//@      "items": false
//@    },
  discriminatedUnion1
//@        {
//@          "$ref": "#/definitions/discriminatedUnion1"
//@        },
  string
//@        {
//@          "type": "string"
//@        }
]

type discriminatedUnionInlineTuple1 = [
//@    "discriminatedUnionInlineTuple1": {
//@      "type": "array",
//@      "prefixItems": [
//@      ],
//@      "items": false
//@    }
  @discriminator('type')
  typeA | typeB | { type: 'c', value: object }
//@        {
//@          "type": "object",
//@          "discriminator": {
//@            "propertyName": "type",
//@            "mapping": {
//@              "a": {
//@                "$ref": "#/definitions/typeA"
//@              },
//@              "b": {
//@                "$ref": "#/definitions/typeB"
//@              },
//@              "c": {
//@                "type": "object",
//@                "properties": {
//@                  "type": {
//@                    "type": "string",
//@                    "allowedValues": [
//@                      "c"
//@                    ]
//@                  },
//@                  "value": {
//@                    "type": "object"
//@                  }
//@                }
//@              }
//@            }
//@          }
//@        },
  string
//@        {
//@          "type": "string"
//@        }
]

param paramDiscriminatedUnionTypeAlias1 discriminatedUnion1
//@    "paramDiscriminatedUnionTypeAlias1": {
//@      "$ref": "#/definitions/discriminatedUnion1"
//@    },
param paramDiscriminatedUnionTypeAlias2 discriminatedUnion5
//@    "paramDiscriminatedUnionTypeAlias2": {
//@      "$ref": "#/definitions/discriminatedUnion5"
//@    },

@discriminator('type')
param paramInlineDiscriminatedUnion1 typeA | typeB
//@    "paramInlineDiscriminatedUnion1": {
//@      "type": "object",
//@      "discriminator": {
//@        "propertyName": "type",
//@        "mapping": {
//@          "a": {
//@            "$ref": "#/definitions/typeA"
//@          },
//@          "b": {
//@            "$ref": "#/definitions/typeB"
//@          }
//@        }
//@      }
//@    },

@discriminator('type')
param paramInlineDiscriminatedUnion2 (typeA | typeB) = { type: 'b', value: 0 }
//@    "paramInlineDiscriminatedUnion2": {
//@      "type": "object",
//@      "discriminator": {
//@        "propertyName": "type",
//@        "mapping": {
//@          "a": {
//@            "$ref": "#/definitions/typeA"
//@          },
//@          "b": {
//@            "$ref": "#/definitions/typeB"
//@          }
//@        }
//@      },
//@      "defaultValue": {
//@        "type": "b",
//@        "value": 0
//@      }
//@    },

@discriminator('type')
param paramInlineDiscriminatedUnion3 (typeA | typeB)?
//@    "paramInlineDiscriminatedUnion3": {
//@      "type": "object",
//@      "discriminator": {
//@        "propertyName": "type",
//@        "mapping": {
//@          "a": {
//@            "$ref": "#/definitions/typeA"
//@          },
//@          "b": {
//@            "$ref": "#/definitions/typeB"
//@          }
//@        }
//@      },
//@      "nullable": true
//@    }

output outputDiscriminatedUnionTypeAlias1 discriminatedUnion1 = { type: 'a', value: 'str' }
//@    "outputDiscriminatedUnionTypeAlias1": {
//@      "$ref": "#/definitions/discriminatedUnion1",
//@      "value": {
//@        "type": "a",
//@        "value": "str"
//@      }
//@    },
@discriminator('type')
output outputDiscriminatedUnionTypeAlias2 discriminatedUnion1 = { type: 'a', value: 'str' }
//@    "outputDiscriminatedUnionTypeAlias2": {
//@      "$ref": "#/definitions/discriminatedUnion1",
//@      "value": {
//@        "type": "a",
//@        "value": "str"
//@      }
//@    },
output outputDiscriminatedUnionTypeAlias3 discriminatedUnion5 = null
//@    "outputDiscriminatedUnionTypeAlias3": {
//@      "$ref": "#/definitions/discriminatedUnion5",
//@      "value": null
//@    },

@discriminator('type')
output outputInlineDiscriminatedUnion1 typeA | typeB | { type: 'c', value: int } = { type: 'a', value: 'a' }
//@    "outputInlineDiscriminatedUnion1": {
//@      "type": "object",
//@      "discriminator": {
//@        "propertyName": "type",
//@        "mapping": {
//@          "a": {
//@            "$ref": "#/definitions/typeA"
//@          },
//@          "b": {
//@            "$ref": "#/definitions/typeB"
//@          },
//@          "c": {
//@            "type": "object",
//@            "properties": {
//@              "type": {
//@                "type": "string",
//@                "allowedValues": [
//@                  "c"
//@                ]
//@              },
//@              "value": {
//@                "type": "int"
//@              }
//@            }
//@          }
//@        }
//@      },
//@      "value": {
//@        "type": "a",
//@        "value": "a"
//@      }
//@    },

@discriminator('type')
output outputInlineDiscriminatedUnion2 typeA | typeB | ({ type: 'c', value: int }) = { type: 'c', value: 1 }
//@    "outputInlineDiscriminatedUnion2": {
//@      "type": "object",
//@      "discriminator": {
//@        "propertyName": "type",
//@        "mapping": {
//@          "a": {
//@            "$ref": "#/definitions/typeA"
//@          },
//@          "b": {
//@            "$ref": "#/definitions/typeB"
//@          },
//@          "c": {
//@            "type": "object",
//@            "properties": {
//@              "type": {
//@                "type": "string",
//@                "allowedValues": [
//@                  "c"
//@                ]
//@              },
//@              "value": {
//@                "type": "int"
//@              }
//@            }
//@          }
//@        }
//@      },
//@      "value": {
//@        "type": "c",
//@        "value": 1
//@      }
//@    },

@discriminator('type')
output outputInlineDiscriminatedUnion3 (typeA | typeB)? = null
//@    "outputInlineDiscriminatedUnion3": {
//@      "type": "object",
//@      "discriminator": {
//@        "propertyName": "type",
//@        "mapping": {
//@          "a": {
//@            "$ref": "#/definitions/typeA"
//@          },
//@          "b": {
//@            "$ref": "#/definitions/typeB"
//@          }
//@        }
//@      },
//@      "nullable": true,
//@      "value": null
//@    }

