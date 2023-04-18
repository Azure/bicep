func buildUrl = (bool https, string hostname, string path) => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'
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

func sayHello = (string name) => 'Hi ${name}!'
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
//@        }

output hellos array = map(['Evie', 'Casper'], name => sayHello(name))
//@    "hellos": {
//@      "type": "array",
//@      "value": "[map(createArray('Evie', 'Casper'), lambda('name', __bicep.sayHello(lambdaVariables('name'))))]"
//@    }

