module databaseContainerList
{
    param accountName string {
        defaultValue: 'sql-{uniqueString(resourceGroup().id)}'
        minLength: 3
        maxLength: 44
        description: 'Cosmos DB account name'
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

    param throughput int {
        defaultValue: 400,
        minValue: 400,
        maxValue: 1000000,
        description: 'The throughput for the container'
    }

    list databaseContainerList for containerIndex in length(param.containerReference)
    {
        resource container 'Microsoft.DocumentDB/databaseAccounts/sqlDatabase/containers@2020-03-01' {
            name: '{param.accountName}/{param.containerReference[containerIndex].databaseName}/{param.containerReference[containerIndex].containerName}',
            dependsOn: sqlDatabaseList,
            properties: {
                resource: {
                    id: '{param.containerReference[containerIndex].containerName}',
                    partitionKey: {
                        paths: [
                            '/myPartitionKey'
                        ],
                        containerReference
                    },
                    indexingPolicy: {
                        indexingMode: 'consistent',
                        includedPaths: [
                            path: '/*'
                        ],
                        excludedPaths: [
                            path: '/myPathToNotIndex/*'
                        ],
                        compositeIndexes: [
                            [
                                {
                                    path: '/name',
                                    order: 'ascending'
                                },
                                {
                                    path: '/age',
                                    order: 'descending'
                                }
                            ]
                        ],
                        spatialIndexes: [
                            {
                                path: '/path/to/geojson/property/?',
                                types: [
                                    'Point',
                                    'Polygon',
                                    'MultiPolygon',
                                    'LineString'
                                ]
                            }
                        ]
                    },
                    defaultTtl: 86400,
                    uniqueKeyPolicy: {
                        uniqueKeys: [
                            {
                                paths: [
                                    '/phoneNumber'
                                ]
                            }
                        ]
                    }
                },
                options: {
                    throughput: param.throughput
                }
            }
        }
    } with
    {
        // Metadata describing how list of resources should be deployed.
        mode:'serial
        batchSize:3
    }

    outputs: {
        databaseContainers: {
            type: array,
            value: list databaseContainerList
        }
    }
}