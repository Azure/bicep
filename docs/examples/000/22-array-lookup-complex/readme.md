# Array lookup of a complex object sample

This bicep sample demonstrates using an array lookup and returning elements of a
complex object.

## Code description

This bicep file takes a string and returns elements of a complex object based off that name.

## Output

The output of the deployment should be:

``` text
Outputs :
          Name             Type                       Value
          ===============  =========================  ==========
          bicepObject      Array                      [
            {
              "name": "GatewaySubnet",
              "CIDR": "10.1.0.0/27",
              "NSG": false,
              "RouteTable": true
            },
            {
              "name": "AzureBastionSubnet",
              "CIDR": "10.1.0.33/27",
              "NSG": false,
              "RouteTable": false
            },
            {
              "name": "AzureFirewallSubnet",
              "CIDR": "10.1.0.33/27",
              "NSG": false,
              "RouteTable": false
            },
            {
              "name": "websubnet",
              "CIDR": "10.1.1.0/24",
              "NSG": true,
              "RouteTable": true
            },
            {
              "name": "appsubnet",
              "CIDR": "10.1.2.0/24",
              "NSG": true,
              "RouteTable": true
            },
            {
              "name": "sqlsubnet",
              "CIDR": "10.1.3.0/24",
              "NSG": true,
              "RouteTable": true
            }
          ]
```
