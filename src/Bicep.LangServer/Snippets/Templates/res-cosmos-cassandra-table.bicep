// Cosmos DB Cassandra Table
resource ${1:cassandraKeyspaceTable} 'Microsoft.DocumentDb/databaseAccounts/apis/keyspaces/tables@2016-03-31' = {
  parent: databaseAccount
  name: ${2:'name'}
  properties: {
    resource: {
      id: ${3:'id'}
    }
    options: {}
  }
}

resource databaseAccount 'Microsoft.DocumentDB/databaseAccounts/apis/keyspaces@2016-03-31' = {
  name: ${4:'name'}
  properties: {
    resource: {
      id: ${5:'id'}
    }
    options: {}
  }
}
