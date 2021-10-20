// SQL Database
resource sqlServer 'Microsoft.Sql/servers@2014-04-01' ={
  name: /*${1:'name'}*/'name'
  location: location
}

resource /*${2:sqlServerDatabase}*/sqlServerDatabase 'Microsoft.Sql/servers/databases@2014-04-01' = {
  parent: sqlServer
  name: /*${3:'name'}*/'name'
  location: location
  properties: {
    collation: /*${4:'collation'}*/'collation'
    edition: /*'${5|Basic,Business,BusinessCritical,DataWarehouse,Free,GeneralPurpose,Hyperscale,Premium,PremiumRS,Standard,Stretch,System,System2,Web|}'*/'Basic'
    maxSizeBytes: /*${6:'maxSizeBytes'}*/'maxSizeBytes'
    requestedServiceObjectiveName: /*'${7|Basic,DS100,DS1000,DS1200,DS1500,DS200,DS2000,DS300,DS400,DS500,DS600,DW100,DW1000,DW10000c,DW1000c,DW1200,DW1500,DW15000c,DW1500c,DW200,DW2000,DW2000c,DW2500c,DW300,DW3000,DW30000c,DW3000c,DW400,DW500,DW5000c,DW600,DW6000,DW6000c,DW7500c,ElasticPool,Free,P1,P11,P15,P2,P3,P4,P6,PRS1,PRS2,PRS4,PRS6,S0,S1,S12,S2,S3,S4,S6,S7,S9,System,System0,System1,System2,System2L,System3,System3L,System4,System4L|}'*/'Basic'
  }
}
