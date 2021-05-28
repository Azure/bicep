# Multiline string sample

This bicep sample demonstrates json objects as strings.

## Code description

This bicep file shows how to create a json object in a variable via a string.

## Output

The output of the deployment should be:

``` text
Outputs :
          Name             Type                       Value
          ===============  =========================  ==========
          jsonArray        Array                      [
            {
              "name": "one"
            },
            {
              "name": "two"
            },
            {
              "name": "three"
            }
          ]
```

Note the three different outcomes based off of blank lines in the variables
