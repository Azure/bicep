﻿// Cosmos DB Mongo Database
resource ${1:mongoDb} 'Microsoft.DocumentDB/databaseAccounts/apis/databases@2016-03-31' = {
  name: ${2:'name'}
  properties: {
    resource: {
      id: ${3:'id'}
    }
    options: {}
  }
}
