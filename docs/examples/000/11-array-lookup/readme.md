# Array lookup sample

This bicep sample demonstrates using an array lookup.

## Code description

This bicep file takes an `AzRegion` parameter and returns the TimeZone through an
array lookup.


## Output

The output of the deployment should be:

``` text
Outputs :
          Name             Type                       Value
          ===============  =========================  ==========
          azRegion         String                     eastus2
          timezone         String                     Eastern Standard Time
```
