//Json object samples

var routeArray = [
  'OnPrem1'
  'OnPrem2'
]
//Array in JSON String
var jsonString = '''
{
  "OnPrem1": {
    "AddressPrefix": "192.168.0.0/16",
    "NextHopType": "VirtualNetworkGateway",
    "NextHopIpAddress": null
  },
  "OnPrem2": {
    "AddressPrefix": "10.0.1.0/24",
    "NextHopType": "VirtualAppliance",
    "NextHopIpAddress": "10.200.1.1"
  }
}
'''
var jsonArray = json(jsonString)

output routeArray array = [for (name, i) in routeArray: {
  name: name
  AddressPrefix: jsonArray[name].AddressPrefix
  NextHopType: jsonArray[name].NextHopType
  NextHopIpAddress: jsonArray[name].NextHopIpAddress
}]
