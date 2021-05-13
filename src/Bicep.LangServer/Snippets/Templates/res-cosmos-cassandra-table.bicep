﻿// Cosmos DB Cassandra Table
resource cassandraKeyspace 'Microsoft.DocumentDB/databaseAccounts/apis/keyspaces@2016-03-31' = {
  name: ${1:'name'}
  properties: {
    resource: {
      id: ${2:'id'}
    }
    options: {}
  }
}

resource ${3:cassandraKeyspaceTable} 'Microsoft.DocumentDb/databaseAccounts/apis/keyspaces/tables@2016-03-31' = {
  parent: cassandraKeyspace
  name: ${4:'name'}
  properties: {
    resource: {
      id: ${5:'id'}
    }
    options: {}
  }
}
