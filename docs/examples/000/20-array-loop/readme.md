# Array sample

This bicep sample demonstrates array iterations.

## Code description

This bicep file iterates through an array and references items in three ways.

## Output

The output of the deployment should be:

``` text
Outputs :
          Name             Type                       Value
          ===============  =========================  ==========
          out1             Array                      [
            {
              "element": "Michael"
            },
            {
              "element": "Dwight"
            },
            {
              "element": "Jim"
            },
            {
              "element": "Pam"
            }
          ]
          out2             Array                      [
            {
              "element": "Michael"
            },
            {
              "element": "Dwight"
            },
            {
              "element": "Jim"
            },
            {
              "element": "Pam"
            }
          ]
          out3             Array                      [
            {
              "element": "Michael"
            },
            {
              "element": "Dwight"
            },
            {
              "element": "Jim"
            },
            {
              "element": "Pam"
            }
          ]
```
