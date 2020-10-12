# Module

## Definition

A module is an opaque set of one or more resources to be deployed together. It only exposes parameters and outputs as contract to other bicep files, hiding details on how internal resources are defined. This allows you to abstract away complex details of the raw resource declaration from the end user who now only needs to be concerned about the module contract. Parameters and outputs are optional.

## Declare a module

Any bicep file is itself a module, so there is no specific syntax for defining a module. A module can be a single file or a directory. If a module references a directory, all root files in that directory will be combined. Here is an example bicep file (`sqlDatabases.bicep`) that we will consume as a module:

```
param accountName string

param databaseNames array {
    default: [ "name1", "name2" ]
}

resource[] sqlDatabases 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2020-03-01'= [
    for databaseName in databaseNames: {
        name: '{accountName}/{databaseName}'
        properties: {
            resource: {
                id: databaseName
            }
            options: {
            }
        }
    }
]

output sqlDatabases array = [
    for database in sqlDatabases: { database.id }
]
```

## Usage

`module` is a keyword in bicep. The module location is specified using relative path (`../sqlDatabases` in the below example). Both `\` and `/` are supported.

Here is an example consumption of a module

```
module databases '../sqlDatabases' {
    accountName: 'fooAccount'
    // parameter with default value can be omitted.
}

// To reference module outputs
var myArray array = databases.outputs.sqlDatabases
```

A bicep module can reference another Bicep file or directory of bicep files as a module. This means the module name may refer to either a file or directory. For directory, all files under the directory will be loaded. It is a compiler error if a file and directory with the same name exist under the path.
