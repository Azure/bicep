@description('The foo type')
//@        "description": "The foo type"
@sealed()
type foo = {
//@    "foo": {
//@      "type": "object",
//@      "properties": {
//@        "stringProp": {
//@          "type": "string",
//@          "metadata": {
//@          },
//@        },
//@        "objectProp": {
//@          "type": "object",
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
//@              },
//@              "nullable": true
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
//@          "$ref": "#/definitions/foo",
//@          "nullable": true
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

    intArrayArrayProp: int [] [] ?
  }

  typeRefProp: bar

  literalProp: 'literal'
//@            "literal"

  recursion: foo?
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
//@    },
    @description('A leading string')
//@            "description": "A leading string"
    string

    @description('A second element using a type alias')
//@            "description": "A second element using a type alias"
    bar
]

type stringStringDictionary = {
//@    "stringStringDictionary": {
//@      "type": "object",
//@      "additionalProperties": {
//@        "type": "string"
//@      }
//@    },
    *: string
}

@minValue(1)
//@      "minValue": 1
@maxValue(10)
//@      "maxValue": 10,
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
//@    }

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
//@    }

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
//@        "type": {
//@          "type": "string",
//@          "allowedValues": [
//@          ]
//@        },
//@        "value": {
//@          "type": "string"
//@        }
//@      }
//@    },
  type: 'a'
//@            "a"
  value: string
}

type typeB = {
//@    "typeB": {
//@      "type": "object",
//@      "properties": {
//@        "type": {
//@          "type": "string",
//@          "allowedValues": [
//@          ]
//@        },
//@        "value": {
//@          "type": "int"
//@        }
//@      }
//@    },
  type: 'b'
//@            "b"
  value: int
}

type typeC = {
//@    "typeC": {
//@      "type": "object",
//@      "properties": {
//@        "type": {
//@          "type": "string",
//@          "allowedValues": [
//@          ]
//@        },
//@        "value": {
//@          "$ref": "#/definitions/bool"
//@        },
//@        "value2": {
//@          "type": "string"
//@        }
//@      }
//@    },
  type: 'c'
//@            "c"
  value: bool
  value2: string
}

type typeD = {
//@    "typeD": {
//@      "type": "object",
//@      "properties": {
//@        "type": {
//@          "type": "string",
//@          "allowedValues": [
//@          ]
//@        },
//@        "value": {
//@          "type": "object"
//@        }
//@      }
//@    },
  type: 'd'
//@            "d"
  value: object
}

type typeE = {
//@    "typeE": {
//@      "type": "object",
//@      "properties": {
//@        "type": {
//@          "type": "string",
//@          "allowedValues": [
//@          ]
//@        }
//@      },
//@      "additionalProperties": {
//@        "type": "string"
//@      }
//@    },
  type: 'e'
//@            "e"
  *: string
}

@discriminator('type')
type discriminatedUnion1 = typeA | typeB
//@    "discriminatedUnion1": {
//@      "type": "object",
//@      "discriminator": {
//@        "propertyName": "type",
//@        "mapping": {
//@          "a": {
//@            "properties": {
//@              "value": {
//@                "type": "string"
//@              }
//@            }
//@          },
//@          "b": {
//@            "properties": {
//@              "value": {
//@                "type": "int"
//@              }
//@            }
//@          }
//@        }
//@      }
//@    },

@discriminator('type')
type discriminatedUnion2 = { type: 'c', value: string } | { type: 'd', value: bool }
//@    "discriminatedUnion2": {
//@      "type": "object",
//@      "discriminator": {
//@        "propertyName": "type",
//@        "mapping": {
//@          "c": {
//@            "properties": {
//@              "value": {
//@                "type": "string"
//@              }
//@            }
//@          },
//@          "d": {
//@            "properties": {
//@              "value": {
//@                "$ref": "#/definitions/bool"
//@              }
//@            }
//@          }
//@        }
//@      }
//@    },

@discriminator('type')
type discriminatedUnion3 = discriminatedUnion1 | discriminatedUnion2 | { type: 'e', *: string }
//@    "discriminatedUnion3": {
//@      "type": "object",
//@      "discriminator": {
//@        "propertyName": "type",
//@        "mapping": {
//@          "a": {
//@            "properties": {
//@              "value": {
//@                "type": "string"
//@              }
//@            }
//@          },
//@          "b": {
//@            "properties": {
//@              "value": {
//@                "type": "int"
//@              }
//@            }
//@          },
//@          "c": {
//@            "properties": {
//@              "value": {
//@                "type": "string"
//@              }
//@            }
//@          },
//@          "d": {
//@            "properties": {
//@              "value": {
//@                "$ref": "#/definitions/bool"
//@              }
//@            }
//@          },
//@          "e": {
//@            "additionalProperties": {
//@              "type": "string"
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
//@          "a": {
//@            "properties": {
//@              "value": {
//@                "type": "string"
//@              }
//@            }
//@          },
//@          "b": {
//@            "properties": {
//@              "value": {
//@                "type": "int"
//@              }
//@            }
//@          },
//@          "c": {
//@            "properties": {
//@              "value": {
//@                "type": "string"
//@              }
//@            }
//@          },
//@          "d": {
//@            "properties": {
//@              "value": {
//@                "$ref": "#/definitions/bool"
//@              }
//@            }
//@          },
//@          "e": {
//@            "additionalProperties": {
//@              "type": "string"
//@            }
//@          }
//@        }
//@      }
//@    },

type inlineDiscriminatedUnion1 = {
//@    "inlineDiscriminatedUnion1": {
//@      "type": "object",
//@      "properties": {
//@        "prop": {
//@          "type": "object",
//@          "discriminator": {
//@            "propertyName": "type",
//@            "mapping": {
//@              "a": {
//@                "properties": {
//@                  "value": {
//@                    "type": "string"
//@                  }
//@                }
//@              },
//@              "c": {
//@                "properties": {
//@                  "value": {
//@                    "$ref": "#/definitions/bool"
//@                  },
//@                  "value2": {
//@                    "type": "string"
//@                  }
//@                }
//@              }
//@            }
//@          }
//@        }
//@      }
//@    },
  @discriminator('type')
  prop: typeA | typeC
}

type inlineDiscriminatedUnion2 = {
//@    "inlineDiscriminatedUnion2": {
//@      "type": "object",
//@      "properties": {
//@        "prop": {
//@          "type": "object",
//@          "discriminator": {
//@            "propertyName": "type",
//@            "mapping": {
//@              "a": {
//@                "properties": {
//@                  "value": {
//@                    "$ref": "#/definitions/bool"
//@                  }
//@                }
//@              },
//@              "b": {
//@                "properties": {
//@                  "value": {
//@                    "type": "int"
//@                  }
//@                }
//@              }
//@            }
//@          }
//@        }
//@      }
//@    },
  @discriminator('type')
  prop: { type: 'a', value: bool } | typeB
}

@discriminator('type')
type inlineDiscriminatedUnion3 = {
//@    "inlineDiscriminatedUnion3": {
//@      "type": "object",
//@      "discriminator": {
//@        "propertyName": "type",
//@        "mapping": {
//@          "a": {
//@            "properties": {
//@              "prop": {
//@                "type": "object",
//@                "discriminator": {
//@                  "propertyName": "type",
//@                  "mapping": {
//@                    "a": {
//@                      "properties": {
//@                        "value": {
//@                          "$ref": "#/definitions/bool"
//@                        }
//@                      }
//@                    },
//@                    "b": {
//@                      "properties": {
//@                        "value": {
//@                          "type": "int"
//@                        }
//@                      }
//@                    }
//@                  }
//@                }
//@              }
//@            }
//@          },
//@          "b": {
//@            "properties": {
//@              "prop": {
//@                "type": "object",
//@                "discriminator": {
//@                  "propertyName": "type",
//@                  "mapping": {
//@                    "a": {
//@                      "properties": {
//@                        "value": {
//@                          "type": "string"
//@                        }
//@                      }
//@                    },
//@                    "b": {
//@                      "properties": {
//@                        "value": {
//@                          "type": "int"
//@                        }
//@                      }
//@                    },
//@                    "c": {
//@                      "properties": {
//@                        "value": {
//@                          "type": "string"
//@                        }
//@                      }
//@                    },
//@                    "d": {
//@                      "properties": {
//@                        "value": {
//@                          "$ref": "#/definitions/bool"
//@                        }
//@                      }
//@                    }
//@                  }
//@                }
//@              }
//@            }
//@          }
//@        }
//@      }
//@    },
  type: 'a'
  @discriminator('type')
  prop: { type: 'a', value: bool } | typeB
} | {
  type: 'b'
  @discriminator('type')
  prop: discriminatedUnion1 | discriminatedUnion2
}

type discriminatorUnionAsPropertyType = {
//@    "discriminatorUnionAsPropertyType": {
//@      "type": "object",
//@      "properties": {
//@        "prop1": {
//@          "$ref": "#/definitions/discriminatedUnion1"
//@        },
//@        "prop2": {
//@          "$ref": "#/definitions/discriminatedUnion3"
//@        }
//@      }
//@    },
  prop1: discriminatedUnion1
  prop2: discriminatedUnion3
}

@discriminator('type')
type discriminatorInnerSelfOptionalCycle1 = typeA | {
//@    "discriminatorInnerSelfOptionalCycle1": {
//@      "type": "object",
//@      "discriminator": {
//@        "propertyName": "type",
//@        "mapping": {
//@          "a": {
//@            "properties": {
//@              "value": {
//@                "type": "string"
//@              }
//@            }
//@          },
//@          "b": {
//@            "properties": {
//@              "value": {
//@                "$ref": "#/definitions/discriminatorInnerSelfOptionalCycle1",
//@                "nullable": true
//@              }
//@            }
//@          }
//@        }
//@      }
//@    }
  type: 'b'
  value: discriminatorInnerSelfOptionalCycle1?
}
