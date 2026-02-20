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
//@    },

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
//@        },

// validate formatter works (https://github.com/Azure/bicep/issues/12913)
func a(____________________________________________________________________________________________ string) string => 'a'
//@        "a": {
//@          "parameters": [
//@            {
//@              "type": "string",
//@              "name": "____________________________________________________________________________________________"
//@            }
//@          ],
//@          "output": {
//@            "type": "string",
//@            "value": "a"
//@          }
//@        },
func b(longParameterName1 string, longParameterName2 string, longParameterName3 string, longParameterName4 string) string => 'b'
//@        "b": {
//@          "parameters": [
//@            {
//@              "type": "string",
//@              "name": "longParameterName1"
//@            },
//@            {
//@              "type": "string",
//@              "name": "longParameterName2"
//@            },
//@            {
//@              "type": "string",
//@              "name": "longParameterName3"
//@            },
//@            {
//@              "type": "string",
//@              "name": "longParameterName4"
//@            }
//@          ],
//@          "output": {
//@            "type": "string",
//@            "value": "b"
//@          }
//@        },

func buildUrlMultiLine(
//@        "buildUrlMultiLine": {
//@          "parameters": [
//@            {
//@              "name": "https"
//@            },
//@            {
//@              "name": "hostname"
//@            },
//@            {
//@              "name": "path"
//@            }
//@          ],
//@          "output": {
//@          }
//@        }
  https bool,
//@              "type": "bool",
  hostname string,
//@              "type": "string",
  path string
//@              "type": "string",
) string => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'
//@            "type": "string",
//@            "value": "[format('{0}://{1}{2}', if(parameters('https'), 'https', 'http'), parameters('hostname'), if(empty(parameters('path')), '', format('/{0}', parameters('path'))))]"

output likeExactMatch bool =like('abc', 'abc')
//@    "likeExactMatch": {
//@      "type": "bool",
//@      "value": "[like('abc', 'abc')]"
//@    },
output likeWildCardMatch bool= like ('abcdef', 'a*c*')
//@    "likeWildCardMatch": {
//@      "type": "bool",
//@      "value": "[like('abcdef', 'a*c*')]"
//@    }

