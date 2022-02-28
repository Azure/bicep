# Let's author multiple Azure resources with Bicep.

First, create a parameter named location:
```bicep
param location string = resourceGroup().location
```
Next, create a parameter named 'appPlanName':
```bicep
param appPlanName string
```

These parameters will be used to fill attributes in your resources.

![Image of typing parameters code into Bicep](2_Type_Params.gif)