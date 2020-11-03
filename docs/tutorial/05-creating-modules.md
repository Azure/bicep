# Creating and consuming modules

Now that we have our immensely useful bicep file for creating a storage account and container, we may want to share it or use it as part of a larger bicep project. To do that, we can use `modules`.

Let's start by renaming our `main.bicep` file to `storage.bicep`. This is the file we will use as a `module`. Any bicep file can be a module, so there are no other syntax changes we need to make to this file. It can already be used as a module.

Next let's create a new, empty `main.bicep` file. Your file structure should look like this:

```bash
.
├── main.bicep
└── storage.bicep
```

In `main.bicep`, we'll add the following code to instantiate our module:

```
module stg './storage.bicep' = {
  name: 'storageDeploy'
  params: {
    namePrefix: 'contoso'
  }
}
```

Let's compile `main.bicep` and look at the output.

```json

```

When modules are transpiled into ARM template JSON, they are turned into a nested inline deployment. If you don't know anything about nested deployments, you don't need to understand them to leverage modules. At the end of the day, the same resources will be deployed. The `name` property of the module is the name we use for the nested deployment resource in the ARM template.

Notice modules, just like everything in bicep, has an identifier and we can use that identifier to retrieve information like `outputs` from the module. We can add an output to `main.bicep` to retrieve the storage account name:

```
module stg './storage.bicep' = {
  name: 'storageDeploy'
  params: {
    namePrefix: 'contoso'
  }
}

output storageName string = stg.outputs.computedStorageName
```

Modules also support a `scope` property, which allows you to specify a scope that is different that the target scope of the deployment. For example, we may want the storage module to be deployed to a different resource group. To do so, let's add the `scope` property to our module declaration:

```
module stg './storage.bicep' = {
  name: 'storageDeploy'
  scope: resourceGroup('another-rg') // this will target another resource group in the same subscription
  params: {
    namePrefix: 'contoso'
  }
}

output storageName string = stg.outputs.computedStorageName
```
