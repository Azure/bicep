# Environment URL hardcoded

Do not hardcode environment URLs in your template. Instead, use the environment function to dynamically get these URLs during deployment. For a list of the URL hosts that are blocked, see the default list of DisallowedHosts in [`bicepsettings.json`](./src/Bicep.Core/Configuration/bicepsettings.json).

The following example fails this test because the URL is hardcoded.

```bicep
var AzureURL = "https://management.azure.com"
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
