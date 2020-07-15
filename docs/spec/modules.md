# Module
> **Note**: Module is not implemented yet. 

## Definition

A module is an opaque set of coherent resources to be deployed together. It only exposes parameters and outputs as contract to external components, hiding details on how internal resources are defined. Parameters and outputs are optional, but must be strongly typed if defined.

A module can be composed of either a single or multiple files. Single-file module uses file name as module name. Multi-file module includes all files under a same directory (non-recursively) and uses directory name as module name. The module's parameters and outputs are union of those from all files under the directory.

## File structure

Example `sqlDatabases`:

```
parameter accountName string

parameter databaseNames array {
    defaultValue: [ "name1", "name2" ]
}

resource[] sqlDatabases 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2020-03-01' [
    for databaseName in databaseNames {
        name: '{accountName}/{databaseName}',
        properties: {
            <database properties>
        }
    }
]

outputs: {
    sqlDatabases: {
        type: array,
        value: [for database in sqlDatabases: { database.id }]
    }
}
```

A module file can include nested modules.

## Usage

```
module databases '../sqlDatabases@1.0' {
    accountName: 'fooAccount',
    // parameter with default value can be omitted.
}

// To reference module outputs
variable myArray array = databases.outputs.sqlDatabases
```

`module` is a keyword in bicep. Module location is specified using relative path, `../` in above example. Both `\` and `/` are supported. Module name can be either a file name (single-file module) or directory name (multi-file module). If a directory name is provided, all files under the directory will be loaded. It is a compile error if a file and directory with the same name exist under the path.

The version name can be omitted before bicep module registry becomes available.