func buildUrl = (bool https, string hostname, string path) => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'
//@        "buildUrl": {
//@          "parameters": [
//@            {
//@              "name": "https",
//@              "type": "string"
//@            },
//@            {
//@              "name": "hostname",
//@              "type": "string"
//@            },
//@            {
//@              "name": "path",
//@              "type": "string"
//@            }
//@          ],
//@          "output": {
//@            "type": "string",
//@            "value": "[format('{0}://{1}{2}', if(lambdaVariables('https'), 'https', 'http'), lambdaVariables('hostname'), if(empty(lambdaVariables('path')), '', format('/{0}', lambdaVariables('path'))))]"
//@          }
//@        }

output foo string = buildUrl(true, 'google.com', 'search')
//@    "foo": {
//@      "type": "string",
//@      "value": "[buildUrl(true(), 'google.com', 'search')]"
//@    }

