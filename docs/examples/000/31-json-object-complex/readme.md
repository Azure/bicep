# Multiline string sample

This bicep sample demonstrates complex json objects as strings.

## Code description

This bicep file shows how to create a complex json object in a variable via a string.

## Output

The output of the deployment should be:

``` text
Outputs :
          Name             Type                       Value
          ===============  =========================  ==========
          routeArray       Array                      [
            {
              "name": "OnPrem1",
              "AddressPrefix": "192.168.0.0/16",
              "NextHopType": "VirtualNetworkGateway",
              "NextHopIpAddress": null
            },
            {
              "name": "OnPrem2",
              "AddressPrefix": "10.0.1.0/24",
              "NextHopType": "VirtualAppliance",
              "NextHopIpAddress": "10.200.1.1"
            }
          ]
```

Note the three different outcomes based off of blank lines in the variables
