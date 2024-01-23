func buildUrl(https bool, hostname string, path string) string => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'
//@        "buildUrl": {
//@          "parameters": [
//@            {
//@              "type": "bool",
//@              "name": "https"
//@            },
//@            {
//@              "type": "string",
//@              "name": "hostname"
//@            },
//@            {
//@              "type": "string",
//@              "name": "path"
//@            }
//@          ],
//@          "output": {
//@            "type": "string",
//@            "value": "[format('{0}://{1}{2}', if(parameters('https'), 'https', 'http'), parameters('hostname'), if(empty(parameters('path')), '', format('/{0}', parameters('path'))))]"
//@          }
//@        },

output foo string = buildUrl(true, 'google.com', 'search')
//@    "foo": {
//@      "type": "string",
//@      "value": "[__bicep.buildUrl(true(), 'google.com', 'search')]"
//@    },

func sayHello(name string) string => 'Hi ${name}!'
//@        "sayHello": {
//@          "parameters": [
//@            {
//@              "type": "string",
//@              "name": "name"
//@            }
//@          ],
//@          "output": {
//@            "type": "string",
//@            "value": "[format('Hi {0}!', parameters('name'))]"
//@          }
//@        },

output hellos array = map(['Evie', 'Casper'], name => sayHello(name))
//@    "hellos": {
//@      "type": "array",
//@      "value": "[map(createArray('Evie', 'Casper'), lambda('name', __bicep.sayHello(lambdaVariables('name'))))]"
//@    },

func objReturnType(name string) object => {
//@        "objReturnType": {
//@          "parameters": [
//@            {
//@              "type": "string",
//@              "name": "name"
//@            }
//@          ],
//@          "output": {
//@            "type": "object",
//@            "value": {
//@            }
//@          }
//@        },
  hello: 'Hi ${name}!'
//@              "hello": "[format('Hi {0}!', parameters('name'))]"
}

func arrayReturnType(name string) array => [
//@        "arrayReturnType": {
//@          "parameters": [
//@            {
//@              "type": "string",
//@              "name": "name"
//@            }
//@          ],
//@          "output": {
//@            "type": "array",
//@            "value": [
//@            ]
//@          }
//@        },
  name
//@              "[parameters('name')]"
]

func asdf(name string) array => [
//@        "asdf": {
//@          "parameters": [
//@            {
//@              "type": "string",
//@              "name": "name"
//@            }
//@          ],
//@          "output": {
//@            "type": "array",
//@            "value": [
//@            ]
//@          }
//@        },
  'asdf'
//@              "asdf",
  name
//@              "[parameters('name')]"
]

func person(name string,
//@        "person": {
//@          "parameters": [
//@            {
//@              "type": "string",
//@              "name": "name"
//@            },
//@            {
//@              "name": "age"
//@            },
//@            {
//@              "name": "weight"
//@            },
//@            {
//@              "name": "height"
//@            }
//@          ],
//@          "output": {
//@          }
//@        },
  age int,
//@              "type": "int",
weight int,
//@              "type": "int",
height int) array => [
//@              "type": "int",
//@            "type": "array",
//@            "value": [
//@            ]
  name
//@              "[parameters('name')]",
  age
//@              "[parameters('age')]",
  weight
//@              "[parameters('weight')]",
  height
//@              "[parameters('height')]"
]

func longParameterList(one string, two string, three string, /* comment comment comment comment */ four string) array => [
//@        "longParameterList": {
//@          "parameters": [
//@            {
//@              "type": "string",
//@              "name": "one"
//@            },
//@            {
//@              "type": "string",
//@              "name": "two"
//@            },
//@            {
//@              "type": "string",
//@              "name": "three"
//@            },
//@            {
//@              "type": "string",
//@              "name": "four"
//@            }
//@          ],
//@          "output": {
//@            "type": "array",
//@            "value": [
//@            ]
//@          }
//@        },
  one
//@              "[parameters('one')]",
  two
//@              "[parameters('two')]",
  three
//@              "[parameters('three')]",
  four
//@              "[parameters('four')]"
]

@minValue(0)
//@      "minValue": 0
type positiveInt = int
//@    "positiveInt": {
//@      "type": "int",
//@    }

func typedArg(input string[]) positiveInt => length(input)
//@        "typedArg": {
//@          "parameters": [
//@            {
//@              "type": "array",
//@              "items": {
//@                "type": "string"
//@              },
//@              "name": "input"
//@            }
//@          ],
//@          "output": {
//@            "$ref": "#/definitions/positiveInt",
//@            "value": "[length(parameters('input'))]"
//@          }
//@        },

func barTest() array => ['abc', 'def']
//@        "barTest": {
//@          "parameters": [],
//@          "output": {
//@            "type": "array",
//@            "value": [
//@              "abc",
//@              "def"
//@            ]
//@          }
//@        },
func fooTest() array => map(barTest(), a => 'Hello ${a}!')
//@        "fooTest": {
//@          "parameters": [],
//@          "output": {
//@            "type": "array",
//@            "value": "[map(__bicep.barTest(), lambda('a', format('Hello {0}!', lambdaVariables('a'))))]"
//@          }
//@        },

output fooValue array = fooTest()
//@    "fooValue": {
//@      "type": "array",
//@      "value": "[__bicep.fooTest()]"
//@    }

func test() object => loadJsonContent('./repro-data.json')
//@        "test": {
//@          "parameters": [],
//@          "output": {
//@            "type": "object",
//@            "value": {}
//@          }
//@        },
func test2() string => loadTextContent('./repro-data.json')
//@        "test2": {
//@          "parameters": [],
//@          "output": {
//@            "type": "string",
//@            "value": "{}"
//@          }
//@        },
func test3() object => loadYamlContent('./repro-data.json')
//@        "test3": {
//@          "parameters": [],
//@          "output": {
//@            "type": "object",
//@            "value": {}
//@          }
//@        },
func test4() string => loadFileAsBase64('./repro-data.json')
//@        "test4": {
//@          "parameters": [],
//@          "output": {
//@            "type": "string",
//@            "value": "e30="
//@          }
//@        }

