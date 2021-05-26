# Environment URL hardcoded

**Code**: no-hardcoded-env-urls

**Description**: Do not hardcode environment URLs in your template. Instead, use the [environment function](https://docs.microsoft.com/azure/azure-resource-manager/templates/template-functions-deployment?tabs=json#environment) to dynamically get these URLs during deployment. For a list of the URL hosts that are blocked, see the default list of DisallowedHosts in [`bicepconfig.json`](./src/Bicep.Core/Configuration/bicepconfig.json).

The following example fails this test because the URL is hardcoded.

```bicep
var AzureURL = 'https://management.azure.com'
```

The test also fails when used with concat or uri.

```bicep
var AzureSchemaURL1 = concat('https://','gallery.azure.com')
var AzureSchemaURL2 = uri('gallery.azure.com','test')
```

The following example passes this test.

```bicep
var AzureSchemaURL = environment().gallery
```
