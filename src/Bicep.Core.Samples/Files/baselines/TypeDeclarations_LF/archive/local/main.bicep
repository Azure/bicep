{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "languageVersion": "2.0",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "8162009081861809153"
    }
  },
  "definitions": {
    "foo": {
      "type": "object",
      "properties": {
        "stringProp": {
          "type": "string",
          "minLength": 3,
          "maxLength": 10,
          "metadata": {
            "description": "A string property"
          }
        },
        "objectProp": {
          "type": "object",
          "properties": {
            "intProp": {
              "type": "int",
              "minValue": 1
            },
            "intArrayArrayProp": {
              "type": "array",
              "items": {
                "type": "array",
                "items": {
                  "type": "int"
                }
              },
              "nullable": true
            }
          }
        },
        "typeRefProp": {
          "$ref": "#/definitions/bar"
        },
        "literalProp": {
          "type": "string",
          "allowedValues": [
            "literal"
          ]
        },
        "recursion": {
          "$ref": "#/definitions/foo",
          "nullable": true
        }
      },
      "additionalProperties": false,
      "metadata": {
        "description": "The foo type"
      }
    },
    "fooProperty": {
      "$ref": "#/definitions/foo/properties/objectProp/properties/intProp"
    },
    "bar": {
      "type": "array",
      "items": {
        "type": "array",
        "items": {
          "type": "array",
          "items": {
            "type": "array",
            "items": {
              "type": "int"
            }
          }
        }
      },
      "metadata": {
        "examples": [
          [
            [
              [
                [
                  1
                ]
              ]
            ],
            [
              [
                [
                  2
                ]
              ]
            ],
            [
              [
                [
                  3
                ]
              ]
            ]
          ]
        ],
        "description": "An array of array of arrays of arrays of ints"
      },
      "minLength": 3
    },
    "barElement": {
      "$ref": "#/definitions/bar/items"
    },
    "aUnion": {
      "type": "string",
      "allowedValues": [
        "crackle",
        "pop",
        "snap"
      ]
    },
    "singleMemberUnion": {
      "type": "string",
      "allowedValues": [
        "alone"
      ]
    },
    "expandedUnion": {
      "type": "string",
      "allowedValues": [
        "buzz",
        "crackle",
        "fizz",
        "pop",
        "snap"
      ]
    },
    "tupleUnion": {
      "type": "array",
      "allowedValues": [
        [
          "fizz",
          "buzz"
        ],
        [
          "foo",
          "bar",
          "baz"
        ],
        [
          "snap",
          "crackle",
          "pop"
        ]
      ]
    },
    "mixedArray": {
      "type": "array",
      "allowedValues": [
        "heffalump",
        "woozle",
        -10,
        10,
        false,
        null,
        true,
        {
          "shape": "*",
          "size": "*"
        }
      ]
    },
    "bool": {
      "type": "string"
    },
    "tuple": {
      "type": "array",
      "prefixItems": [
        {
          "type": "string",
          "metadata": {
            "description": "A leading string"
          }
        },
        {
          "$ref": "#/definitions/bar",
          "metadata": {
            "description": "A second element using a type alias"
          }
        }
      ],
      "items": false
    },
    "tupleSecondItem": {
      "$ref": "#/definitions/tuple/prefixItems/1"
    },
    "stringStringDictionary": {
      "type": "object",
      "properties": {},
      "additionalProperties": {
        "type": "string"
      }
    },
    "stringStringDictionaryValue": {
      "$ref": "#/definitions/stringStringDictionary/additionalProperties"
    },
    "constrainedInt": {
      "type": "int",
      "minValue": 1,
      "maxValue": 10
    },
    "nullable": {
      "type": "string",
      "nullable": true
    },
    "nonNullable": {
      "$ref": "#/definitions/nullable",
      "nullable": false
    },
    "typeA": {
      "type": "object",
      "properties": {
        "type": {
          "type": "string",
          "allowedValues": [
            "a"
          ]
        },
        "value": {
          "type": "string"
        }
      }
    },
    "typeB": {
      "type": "object",
      "properties": {
        "type": {
          "type": "string",
          "allowedValues": [
            "b"
          ]
        },
        "value": {
          "type": "int"
        }
      }
    },
    "typeC": {
      "type": "object",
      "properties": {
        "type": {
          "type": "string",
          "allowedValues": [
            "c"
          ]
        },
        "value": {
          "$ref": "#/definitions/bool"
        },
        "value2": {
          "type": "string"
        }
      }
    },
    "typeD": {
      "type": "object",
      "properties": {
        "type": {
          "type": "string",
          "allowedValues": [
            "d"
          ]
        },
        "value": {
          "type": "object"
        }
      }
    },
    "typeE": {
      "type": "object",
      "properties": {
        "type": {
          "type": "string",
          "allowedValues": [
            "e"
          ]
        },
        "value": {
          "type": "string",
          "allowedValues": [
            "a",
            "b"
          ]
        }
      }
    },
    "typeF": {
      "type": "object",
      "properties": {
        "type": {
          "type": "string",
          "allowedValues": [
            "f"
          ]
        }
      },
      "additionalProperties": {
        "type": "string"
      }
    },
    "discriminatedUnion1": {
      "type": "object",
      "discriminator": {
        "propertyName": "type",
        "mapping": {
          "a": {
            "$ref": "#/definitions/typeA"
          },
          "b": {
            "$ref": "#/definitions/typeB"
          }
        }
      }
    },
    "discriminatedUnion2": {
      "type": "object",
      "discriminator": {
        "propertyName": "type",
        "mapping": {
          "c": {
            "type": "object",
            "properties": {
              "type": {
                "type": "string",
                "allowedValues": [
                  "c"
                ]
              },
              "value": {
                "type": "string"
              }
            }
          },
          "d": {
            "type": "object",
            "properties": {
              "type": {
                "type": "string",
                "allowedValues": [
                  "d"
                ]
              },
              "value": {
                "$ref": "#/definitions/bool"
              }
            }
          }
        }
      }
    },
    "discriminatedUnion3": {
      "type": "object",
      "discriminator": {
        "propertyName": "type",
        "mapping": {
          "a": {
            "$ref": "#/definitions/typeA"
          },
          "b": {
            "$ref": "#/definitions/typeB"
          },
          "c": {
            "type": "object",
            "properties": {
              "type": {
                "type": "string",
                "allowedValues": [
                  "c"
                ]
              },
              "value": {
                "type": "string"
              }
            }
          },
          "d": {
            "type": "object",
            "properties": {
              "type": {
                "type": "string",
                "allowedValues": [
                  "d"
                ]
              },
              "value": {
                "$ref": "#/definitions/bool"
              }
            }
          },
          "e": {
            "type": "object",
            "properties": {
              "type": {
                "type": "string",
                "allowedValues": [
                  "e"
                ]
              },
              "value": {
                "type": "string"
              }
            }
          }
        }
      }
    },
    "discriminatedUnion4": {
      "type": "object",
      "discriminator": {
        "propertyName": "type",
        "mapping": {
          "a": {
            "$ref": "#/definitions/typeA"
          },
          "b": {
            "$ref": "#/definitions/typeB"
          },
          "c": {
            "type": "object",
            "properties": {
              "type": {
                "type": "string",
                "allowedValues": [
                  "c"
                ]
              },
              "value": {
                "type": "string"
              }
            }
          },
          "d": {
            "type": "object",
            "properties": {
              "type": {
                "type": "string",
                "allowedValues": [
                  "d"
                ]
              },
              "value": {
                "$ref": "#/definitions/bool"
              }
            }
          },
          "e": {
            "$ref": "#/definitions/typeE"
          }
        }
      }
    },
    "discriminatedUnion5": {
      "type": "object",
      "discriminator": {
        "propertyName": "type",
        "mapping": {
          "a": {
            "$ref": "#/definitions/typeA"
          },
          "b": {
            "$ref": "#/definitions/typeB"
          }
        }
      },
      "nullable": true
    },
    "discriminatedUnion6": {
      "type": "object",
      "discriminator": {
        "propertyName": "type",
        "mapping": {
          "a": {
            "$ref": "#/definitions/typeA"
          },
          "b": {
            "$ref": "#/definitions/typeB"
          }
        }
      },
      "nullable": false
    },
    "inlineDiscriminatedUnion1": {
      "type": "object",
      "properties": {
        "prop": {
          "type": "object",
          "discriminator": {
            "propertyName": "type",
            "mapping": {
              "a": {
                "$ref": "#/definitions/typeA"
              },
              "c": {
                "$ref": "#/definitions/typeC"
              }
            }
          }
        }
      }
    },
    "inlineDiscriminatedUnion2": {
      "type": "object",
      "properties": {
        "prop": {
          "type": "object",
          "discriminator": {
            "propertyName": "type",
            "mapping": {
              "a": {
                "type": "object",
                "properties": {
                  "type": {
                    "type": "string",
                    "allowedValues": [
                      "a"
                    ]
                  },
                  "value": {
                    "$ref": "#/definitions/bool"
                  }
                }
              },
              "b": {
                "$ref": "#/definitions/typeB"
              }
            }
          }
        }
      }
    },
    "inlineDiscriminatedUnion3": {
      "type": "object",
      "discriminator": {
        "propertyName": "type",
        "mapping": {
          "a": {
            "type": "object",
            "properties": {
              "type": {
                "type": "string",
                "allowedValues": [
                  "a"
                ]
              },
              "prop": {
                "type": "object",
                "discriminator": {
                  "propertyName": "type",
                  "mapping": {
                    "a": {
                      "type": "object",
                      "properties": {
                        "type": {
                          "type": "string",
                          "allowedValues": [
                            "a"
                          ]
                        },
                        "value": {
                          "$ref": "#/definitions/bool"
                        }
                      }
                    },
                    "b": {
                      "$ref": "#/definitions/typeB"
                    }
                  }
                }
              }
            }
          },
          "b": {
            "type": "object",
            "properties": {
              "type": {
                "type": "string",
                "allowedValues": [
                  "b"
                ]
              },
              "prop": {
                "type": "object",
                "discriminator": {
                  "propertyName": "type",
                  "mapping": {
                    "a": {
                      "$ref": "#/definitions/typeA"
                    },
                    "b": {
                      "$ref": "#/definitions/typeB"
                    },
                    "c": {
                      "type": "object",
                      "properties": {
                        "type": {
                          "type": "string",
                          "allowedValues": [
                            "c"
                          ]
                        },
                        "value": {
                          "type": "string"
                        }
                      }
                    },
                    "d": {
                      "type": "object",
                      "properties": {
                        "type": {
                          "type": "string",
                          "allowedValues": [
                            "d"
                          ]
                        },
                        "value": {
                          "$ref": "#/definitions/bool"
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
    },
    "inlineDiscriminatedUnion4": {
      "type": "object",
      "properties": {
        "prop": {
          "type": "object",
          "discriminator": {
            "propertyName": "type",
            "mapping": {
              "a": {
                "$ref": "#/definitions/typeA"
              },
              "c": {
                "$ref": "#/definitions/typeC"
              }
            }
          },
          "nullable": true
        }
      }
    },
    "discriminatorUnionAsPropertyType": {
      "type": "object",
      "properties": {
        "prop1": {
          "$ref": "#/definitions/discriminatedUnion1"
        },
        "prop2": {
          "$ref": "#/definitions/discriminatedUnion3"
        }
      }
    },
    "discriminatedUnionInlineAdditionalProps1": {
      "type": "object",
      "properties": {},
      "additionalProperties": {
        "type": "object",
        "discriminator": {
          "propertyName": "type",
          "mapping": {
            "a": {
              "$ref": "#/definitions/typeA"
            },
            "b": {
              "$ref": "#/definitions/typeB"
            }
          }
        }
      }
    },
    "discriminatedUnionInlineAdditionalProps2": {
      "type": "object",
      "properties": {},
      "additionalProperties": {
        "type": "object",
        "discriminator": {
          "propertyName": "type",
          "mapping": {
            "a": {
              "$ref": "#/definitions/typeA"
            },
            "b": {
              "$ref": "#/definitions/typeB"
            }
          }
        },
        "nullable": true
      }
    },
    "discriminatorMemberHasAdditionalProperties1": {
      "type": "object",
      "discriminator": {
        "propertyName": "type",
        "mapping": {
          "a": {
            "$ref": "#/definitions/typeA"
          },
          "f": {
            "$ref": "#/definitions/typeF"
          },
          "g": {
            "type": "object",
            "properties": {
              "type": {
                "type": "string",
                "allowedValues": [
                  "g"
                ]
              }
            },
            "additionalProperties": {
              "type": "int"
            }
          }
        }
      }
    },
    "discriminatorInnerSelfOptionalCycle1": {
      "type": "object",
      "discriminator": {
        "propertyName": "type",
        "mapping": {
          "a": {
            "$ref": "#/definitions/typeA"
          },
          "b": {
            "type": "object",
            "properties": {
              "type": {
                "type": "string",
                "allowedValues": [
                  "b"
                ]
              },
              "value": {
                "$ref": "#/definitions/discriminatorInnerSelfOptionalCycle1",
                "nullable": true
              }
            }
          }
        }
      }
    },
    "discriminatedUnionMemberOptionalCycle1": {
      "type": "object",
      "properties": {
        "type": {
          "type": "string",
          "allowedValues": [
            "b"
          ]
        },
        "prop": {
          "type": "object",
          "discriminator": {
            "propertyName": "type",
            "mapping": {
              "a": {
                "$ref": "#/definitions/typeA"
              },
              "b": {
                "$ref": "#/definitions/discriminatedUnionMemberOptionalCycle1"
              }
            }
          },
          "nullable": true
        }
      }
    },
    "discriminatedUnionMemberOptionalCycle2": {
      "type": "object",
      "properties": {
        "type": {
          "type": "string",
          "allowedValues": [
            "b"
          ]
        }
      },
      "additionalProperties": {
        "type": "object",
        "discriminator": {
          "propertyName": "type",
          "mapping": {
            "a": {
              "$ref": "#/definitions/typeA"
            },
            "b": {
              "$ref": "#/definitions/discriminatedUnionMemberOptionalCycle1"
            }
          }
        }
      }
    },
    "discriminatedUnionTuple1": {
      "type": "array",
      "prefixItems": [
        {
          "$ref": "#/definitions/discriminatedUnion1"
        },
        {
          "type": "string"
        }
      ],
      "items": false
    },
    "discriminatedUnionInlineTuple1": {
      "type": "array",
      "prefixItems": [
        {
          "type": "object",
          "discriminator": {
            "propertyName": "type",
            "mapping": {
              "a": {
                "$ref": "#/definitions/typeA"
              },
              "b": {
                "$ref": "#/definitions/typeB"
              },
              "c": {
                "type": "object",
                "properties": {
                  "type": {
                    "type": "string",
                    "allowedValues": [
                      "c"
                    ]
                  },
                  "value": {
                    "type": "object"
                  }
                }
              }
            }
          }
        },
        {
          "type": "string"
        }
      ],
      "items": false
    }
  },
  "parameters": {
    "inlineObjectParam": {
      "type": "object",
      "properties": {
        "foo": {
          "type": "string"
        },
        "bar": {
          "type": "int",
          "allowedValues": [
            100,
            200,
            300,
            400,
            500
          ]
        },
        "baz": {
          "type": "bool"
        }
      },
      "defaultValue": {
        "foo": "foo",
        "bar": 300,
        "baz": false
      }
    },
    "unionParam": {
      "type": "object",
      "allowedValues": [
        {
          "property": "ping"
        },
        {
          "property": "pong"
        }
      ],
      "defaultValue": {
        "property": "pong"
      }
    },
    "paramUsingType": {
      "$ref": "#/definitions/mixedArray"
    },
    "mightIncludeNull": {
      "type": "array",
      "allowedValues": [
        null,
        {
          "key": "value"
        }
      ]
    },
    "paramDiscriminatedUnionTypeAlias1": {
      "$ref": "#/definitions/discriminatedUnion1"
    },
    "paramDiscriminatedUnionTypeAlias2": {
      "$ref": "#/definitions/discriminatedUnion5"
    },
    "paramInlineDiscriminatedUnion1": {
      "type": "object",
      "discriminator": {
        "propertyName": "type",
        "mapping": {
          "a": {
            "$ref": "#/definitions/typeA"
          },
          "b": {
            "$ref": "#/definitions/typeB"
          }
        }
      }
    },
    "paramInlineDiscriminatedUnion2": {
      "type": "object",
      "discriminator": {
        "propertyName": "type",
        "mapping": {
          "a": {
            "$ref": "#/definitions/typeA"
          },
          "b": {
            "$ref": "#/definitions/typeB"
          }
        }
      },
      "defaultValue": {
        "type": "b",
        "value": 0
      }
    },
    "paramInlineDiscriminatedUnion3": {
      "type": "object",
      "discriminator": {
        "propertyName": "type",
        "mapping": {
          "a": {
            "$ref": "#/definitions/typeA"
          },
          "b": {
            "$ref": "#/definitions/typeB"
          }
        }
      },
      "nullable": true
    }
  },
  "variables": {
    "nonNull": "[parameters('mightIncludeNull')[0].key]",
    "maybeNull": "[tryGet(parameters('mightIncludeNull')[0], 'key')]",
    "maybeNull2": "[tryGet(parameters('mightIncludeNull')[0], 'key')]"
  },
  "resources": {},
  "outputs": {
    "outputUsingType": {
      "$ref": "#/definitions/mixedArray",
      "value": "[parameters('paramUsingType')]"
    },
    "nonNull": {
      "type": "string",
      "value": "[variables('nonNull')]"
    },
    "maybeNull": {
      "type": "string",
      "nullable": true,
      "value": "[variables('maybeNull')]"
    },
    "outputDiscriminatedUnionTypeAlias1": {
      "$ref": "#/definitions/discriminatedUnion1",
      "value": {
        "type": "a",
        "value": "str"
      }
    },
    "outputDiscriminatedUnionTypeAlias2": {
      "$ref": "#/definitions/discriminatedUnion1",
      "value": {
        "type": "a",
        "value": "str"
      }
    },
    "outputDiscriminatedUnionTypeAlias3": {
      "$ref": "#/definitions/discriminatedUnion5",
      "value": null
    },
    "outputInlineDiscriminatedUnion1": {
      "type": "object",
      "discriminator": {
        "propertyName": "type",
        "mapping": {
          "a": {
            "$ref": "#/definitions/typeA"
          },
          "b": {
            "$ref": "#/definitions/typeB"
          },
          "c": {
            "type": "object",
            "properties": {
              "type": {
                "type": "string",
                "allowedValues": [
                  "c"
                ]
              },
              "value": {
                "type": "int"
              }
            }
          }
        }
      },
      "value": {
        "type": "a",
        "value": "a"
      }
    },
    "outputInlineDiscriminatedUnion2": {
      "type": "object",
      "discriminator": {
        "propertyName": "type",
        "mapping": {
          "a": {
            "$ref": "#/definitions/typeA"
          },
          "b": {
            "$ref": "#/definitions/typeB"
          },
          "c": {
            "type": "object",
            "properties": {
              "type": {
                "type": "string",
                "allowedValues": [
                  "c"
                ]
              },
              "value": {
                "type": "int"
              }
            }
          }
        }
      },
      "value": {
        "type": "c",
        "value": 1
      }
    },
    "outputInlineDiscriminatedUnion3": {
      "type": "object",
      "discriminator": {
        "propertyName": "type",
        "mapping": {
          "a": {
            "$ref": "#/definitions/typeA"
          },
          "b": {
            "$ref": "#/definitions/typeB"
          }
        }
      },
      "nullable": true,
      "value": null
    }
  }
}