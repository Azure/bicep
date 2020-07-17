# Module
> **Note**: Module is not implemented yet. 

## Definition

A module is an opaque set of coherent resources to be deployed together. It only exposes parameters and outputs as contract to other bicep files, hiding details on how internal resources are defined. Parameters and outputs are optional.

## File structure

An example module file `sqlDatabases.arm`.

```
parameter accountName string

parameter databaseNames array {
    default: [ "name1", "name2" ]
}

resource[] sqlDatabases 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2020-03-01': [
    for databaseName in databaseNames: {
        name: '{accountName}/{databaseName}'
        properties: {
            "resource": {
                "id": databaseName
            }
            "options": {
            }
        }
    }
]

output sqlDatabases array = [
    for database in sqlDatabases: { database.id }
]
```

## Usage

```
module databases '../sqlDatabases@1.0' {
    accountName: 'fooAccount'
    // parameter with default value can be omitted.
}

// To reference module outputs
variable myArray array = databases.outputs.sqlDatabases
```

`module` is a keyword in bicep. Module location is specified using relative path, `../` in above example. Both `\` and `/` are supported.

A Bicep file can include any other Bicep file or directory as a modules. This means a module name may refer to either a file or directory. For directory, all files under the directory will be loaded. It is a compiler error if a file and directory with the same name exist under the path.

The version name can be omitted before bicep module registry becomes available.
