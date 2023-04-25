func buildUrl = (https bool, hostname string, path string) string => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'
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

func sayHello = (name string) string => 'Hi ${name}!'
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
//@    }

func objReturnType = (name string) object => {
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

func arrayReturnType = (name string) array => [
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
//@        }
  name
//@              "[parameters('name')]"
]

