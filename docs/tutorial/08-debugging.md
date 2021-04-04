# DEBUGGING
Inevitably in your process things aren't going to work.
This is a means and methods for getting "unstuck" when things go wrong.

This document is intended to deal with some of the rough edges for a person new to Azure Resource
Manager Templates (ARM), Bicep, as they make progression beyond the "HelloWorld" tutorials. 

## Getting Help
* StackOverflow, Google, etc.  
* Azure on Reddit http://reddit.com/azure 
* Twitter twitter.com/biceplang

## Limit the Size of the Crater
Bicep is powerful but potentially destructive force so there is a certain amount of both reverance 
and anxiety for a n00b getting started.  Pay attention to the [targetScope=](resource-scopes.md) 
directive since that limits the size of the crater you can leave in your companies cloud. 

Some syntax only work at some scopes, usually the intellisense plugin will catch those but not always. 

When getting started, isolate your project to a private resource group level and 
build up into larger modules. 
Thus targetScope="resourceGroup" will limit the damage you can do and make cleanup easier. 
Don't start trying to wield targetScope = 'subscription' by building an organizational ARM on day #1!  
Simply put: that's insane.  Build small & test small pieces and cobble those together into bigger pieces. 


## Using Protection: Confirm with What If "-c"
Familiarity of "Confirm with What If" is the #1 interview question to see if a candidate knows Bicep or Bullsh*t. 
 
While debugging it's often best to use the --confirm-with-what-if (shortcut: "-c") cli flag to invoke
the what-if featureset.   The What If featureset also contains --what-if-exclude-change-types
to filter the results if you're dealing with pages of changes. 

Allowed values for --what-if-exclude-change-types: Create, Delete, Deploy, Ignore, Modify, NoChange
So you can optionally reduce clutter: 
```
az deployment group create -f ./main.bicep -g yourRG --what-if-exclude-change-types NoChange
```

The Azure team says there are still edge cases where changes won't be detected but they are rare. 


## Debugging ARM Templates

Despite the VS intellisense codes best efforts, Bicep will absolutely generate invalid ARM template code. 

Using the --debug command line parameter to get a copy of the ARM template
that was sent to Azure cloud.  Use this with the -c "What If" parameter to get the string and find 
the column to get some insight about the problem. 

Because bicep is only at v0.3, it's anticipated in the future there will be improvement in the debuggability
of the underlying ARM syntax, enhanced use of the blink tag. ;-) 

## Undo with --mode Complete

The default behavior of Bicep are lazy Incremental updates, that is lazy because they leave any resource that
isn't explicitly modified by a template "as-is.  Incremental is strictly add/update changes to state. 
But in case you create something on accident the --mode Complete will destructively remove resources too. 

```
$ az deployment group create --mode Complete 
```

## Decompile Step-by-Step

If you're new to Bicep &amp; ARM - it's handy to know you can generate valid arm templates for decompilation
by deploying a resource in Azure Console and then selecting the "Download a Template for Automation" after 
creating or "Automation | Export Template" once the resource is provisioned. 

Please take note that in Azure console there is also a Resource Json. 
"Resource Json" is not the same as an ARM template, so DO NOT make the step below
and get confused!  (included bad example for SEO)

```
# NOTE: This *DOES NOT* work! Azure Console Resource Json is != to ARM template
$ az bicep decompile --files azureconsole_resource.json

WARNING: Decompilation is a best-effort process, as there is no guaranteed mapping from ARM JSON to Bicep.
You may need to fix warnings and errors in the generated bicep file(s), or decompilation may fail entirely if an accurate conversion is not possible.
If you would like to report any issues or inaccurate conversions, please see https://github.com/Azure/bicep/issues.
/home/brianh/sportsworld-bicep/azureconsole_resource.json: Decompilation failed with fatal error "[1:1]: Unable to find a template property named $schema."
```

Next you should follow the steps in (decompiling.md)[decompiling.md] for the basics once you've got an ARM template.
But sometimes the steps in decompile don't work completely, such as the example below:

```
az group export --name "yourResourceGroup" > az-group-export.json
```

But it's not really that easy.  You might also encounter an issue like this:
```
WARNING: Export template operation completed with errors. Some resources were not exported. Please see details for more information.
ERROR: Could not get resources of the type 'Microsoft.ContainerRegistry/registries/connectedRegistries'. Resources of this type will not be exported.
ERROR: Could not get resources of the type 'Microsoft.Insights/components/Annotations'. Resources of this type will not be exported.
ERROR: Could not get resources of the type 'Microsoft.KeyVault/vaults/keys'. Resources of this type will not be exported.
ERROR: Could not get resources of the type 'Microsoft.Sql/servers/databases/advisors'. Resources of this type will not be exported.
ERROR: Could not get resources of the type 'Microsoft.Sql/servers/databases/workloadGroups'. Resources of this type will not be exported.
ERROR: Could not get resources of the type 'Microsoft.Web/sites/functions'. Resources of this type will not be exported.
ERROR: Could not get resources of the type 'Microsoft.Web/sites/extensions'. Resources of this type will not be exported.
```

There is still some ambiguity &amp; uncertainty about what these errors mean among the Stack Overflow discussion or if there are work-arounds.   
Non-authorative answers speculate that these are unimplemented services, or could result in a massive deluge of data or too-easy divulging
of keys, etc.  I for one appreciate any hesitancy with implementing a "dump all key vaults" command. 

In these cases it's a one-way write, so you'd need to look up the ARM definition for a keyvault. 

## InvalidTemplate (Error in ARM transpilation)
```
InvalidTemplate - Deployment template validation failed: 'The resource 'Microsoft.Resources/deployments/storageDeploy' at line '1' and column 'NNN' is defined multiple times in a template. Please see https://aka.ms/arm-template/#resources for usage details.'.
```

```
$ az bicep build --files manybuggy.bicep
# ^^ this doesn't work, because we can't correlate it with the line NNN above
```
But you can use the 

## Deployment Failed
Deployment failed. Correlation ID: XXXXX-XXXX-XXXXX. {
  "error": {
    "code": "ParentResourceNotFound",
    "message": "Can not perform requested operation on nested resource. Parent resource 'storageDeploy' not found."
  }
}

## InvalidTemplateDeployment 
The template deployment 'main' is not valid according to the validation procedure. 
The tracking id is 'XXXXX-XXXXX-XXXXXX'. See inner errors for details.



