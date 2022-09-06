param dataFactoryName string
param backupIntervalInHours int

resource dataFactoryName_Backup_Trigger 'Microsoft.DataFactory/factories/triggers@2018-06-01' = {
  name: '${dataFactoryName}/Backup Trigger'
  properties: {
    annotations: []
    runtimeState: 'Started'
//@[4:16) [BCP073 (Warning)] The property "runtimeState" is read-only. Expressions cannot be assigned to read-only properties. If this is an inaccuracy in the documentation, please report it to the Bicep Team. (CodeDescription: bicep(https://aka.ms/bicep-type-issues)) |runtimeState|
    pipelines: [
      {
        pipelineReference: {
          referenceName: 'Backup Database'
          type: 'PipelineReference'
        }
        parameters: {
        }
      }
    ]
    type: 'ScheduleTrigger'
    typeProperties: {
      recurrence: {
        frequency: 'Hour'
        interval: backupIntervalInHours
        startTime: '2020-05-13T08:00:00Z'
        timeZone: 'UTC'
      }
    }
  }
}
