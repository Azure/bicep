# Multiline string sample

This bicep sample demonstrates multiline strings.

## Code description

This bicep file shows multiline string creation and results.

## Output

The output of the deployment should be:

``` text
Outputs :
          Name             Type                       Value
          ===============  =========================  ==========
          out1             String                     |test
          |
          out2             String                     |test|
          out3             String                     |
          test
          |
```

Note the three different outcomes based off of blank lines in the variables
