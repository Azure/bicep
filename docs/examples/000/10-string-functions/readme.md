# string function samples

This bicep sample demonstrates some string functions.

## Code description

This bicep file shows some string functions:

* split
* contains
* indexof
* length
* substring

## Output

The output of the deployment should be:

``` text
Outputs :
          Name               Type                       Value
          =================  =========================  ==========
          arrayFromString    Array                      [
            {
              "element": "a"
            },
            {
              "element": "b"
            },
            {
              "element": "c"
            },
            {
              "element": "d"
            }
          ]
          found              String                     Found "this"
          index              Int                        4
          indexNotFound      Int                        -1
          substr             String                     ThisInString
```
