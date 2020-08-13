// Ideas demonstrated in this file:

// Artifact types: param, var, condition, list, resource, module.

// Artifact definition: <artifact type> [artifact identifier] {<artifact specific syntax>}

// Reference to an artifact definition: <artifact type (short version)>.<artifact identifier>
// such as: param.location, var.accountName, resource.dbAccount, list.databaseContainerList, condition.createAccount

// Region folding. <# marks start of a region, #> is for end the region, such as: <#parameters and #>parameters
// Regions can be nested, but can't be partially overlapping.

// Artifact metadata block. An optional block appended to an artifact definition in form of: <artifact type> {<artifact specific sytax>} with {<artifact metadata block>}
// Refer to module.databaseContainerList for a more detailed example.

// Conditional artifact. condition [artifact identifier] <boolean evaluation logic> { whenTrue: {<logic when condition evaluated true} [whenFalse: {<optional logic when condition evaluated false.>}]}
// Either artifact definitions or their references can be included in a conditional aritifact.

// List artifact. list [artifact identifier] for <index identifier> in <set of artifacts> {<list body>}. List value is an array of fully-qualified resourceIds of the resources created in the list.
// For example, list.databaseContainers is an array of database container resource ids.

// Module artifact. Modules are defined in separate .bicep files and are used in format: module.<moduleName>{parameter list in name:value pairs}.

<#parameters

param accountName string {
    defaultValue: 'sql-{uniqueString(resourceGroup().id)}'
    minLength: 3
    maxLength: 44
    description: 'Cosmos DB account name'
}

param location string {
    defaultValue: resourceGroup().location
    description: 'Location for the Cosmos DB account'
}

param primaryRegion string {
    description: 'The primary replica region for the Cosmos DB account'
}

param secondaryRegion string {
    description: 'The secondary replica region for the Cosmos DB account'
}

param defaultConsistencyLevel string {
    defaultValue: 'Session',
    allowedValues: [
        'Eventual',
        'ConsistentPrefix',
        'Session',
        'BoundedStaleness',
        'Strong'
    ],
    description: 'The default consistency level of the Cosmos DB account'
}

param maxStalenessPrefix int {
    minValue: 10,
    maxValue: 2147483647,
    defaultValue: 100000,
    description: 'Max stale requests. Required for BoundedStaleness.'
}

param maxIntervalInSeconds int {
    minValue: 5,
    maxValue: 86400,
    defaultValue: 300,
    description: 'Max lag time (minutes). Required for BoundedStaleness.'
}

param automaticFailover bool {
    defaultValue: true,
    allowedValues: [
        true,
        false
    ],
    description: 'Enable automatic failover for regions'
}

param databaseName array {
    defaultValue: [
        {
            'raw'
        }
    ],
    description: The variable determines the name of each database that's created.  Duplicate names are not allowed'
}

param containerReference array {
    defaultValue: [
        {
            databaseName: 'raw',
            containerName: 'raw-1',
            partitionKey: 'key1',
            containerThroughput: 400
        }
    ],
    description: 'The variable is used together with containerName, partitionKey and containerThroughput to create determine which container is created under what database.  Duplicates are allowed'
}

param createNewAccount bool {
    defaultValue: true,
    description: 'A switch to decide if to create a new account'
}

#>parameters

<#variables
var accountName = toLower(param.accountName)

var consistencyPolicy = {
    Eventual: {
        defaultConsistencyLevel: 'Eventual'
    },
    ConsistentPrefix: {
        defaultConsistencyLevel: 'ConsistentPrefix'
    },
    Session: {
        defaultConsistencyLevel: 'Session'
    },
    BoundedStaleness: {
        defaultConsistencyLevel: 'BoundedStaleness',
        maxStalenessPrefix: param.maxStalenessPrefix,
        maxIntervalInSeconds: param.maxIntervalInSeconds
    },
    Strong: {
        defaultConsistencyLevel: 'Strong'
    }
}

var locations = [
    {
        locationName: param.primaryRegion,
        failoverPriority: 0,
        isZoneRedundant: false
    },
    {
        locationName: param.secondaryRegion,
        failoverPriority: 1,
        isZoneRedundant: false
    }
]

#>variables

#resources
condition createAccount param.createNewAccount {
    whenTrue: {
        // Three artifacts in whenTrue block.

        // 1. Resource dbAccount that is defined after this conditional block but referenced here using the resource.dbAccount notation.
        resource dbAccount 'Microsoft.DocumentDB/databaseAccounts@2020-03-01' {
            name: var.accountName,
            kind: 'GlobalDocumentDB',
            location: param.location,
            properties: {
                consistencyPolicy: var.consistencyPolicy[param.defaultConsistencyLevel],
                locations: var.locations,
                databaseAccountOffertype; 'Standard',
                enableAutomaticFailover: param.automaticFailover
            }
        }

        // 2. A list of resources that are defined in module sqlDatabaseLists
        module.sqlDatabaseList {
            accountName: var.accountName,
            databaseName: param.databaseName
        }

        // 3. A list of resources that are defined in module databaseContainerList
        module.databaseContainerList {
            accountName: var.accountName,
            containerReference: param.containerReference,
            // param throughput can be omitted because default value is provided in the module
        }
    }
    // can have a whenFalse block if necessary.
}

#resources
