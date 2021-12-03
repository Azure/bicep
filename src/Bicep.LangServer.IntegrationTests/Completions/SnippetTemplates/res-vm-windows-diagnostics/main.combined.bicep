// $1 = windowsVMDiagnostics
// $2 = 'windowsVM/Diagnostics'
// $3 = 'storageAccount'
// $4 = 'storageAccountName'
// $5 = 'storageAccountKey'
// $6 = 'storageAccountEndPoint'

param location string

resource windowsVMDiagnostics 'Microsoft.Compute/virtualMachines/extensions@2020-12-01' = {
  name: 'windowsVM/Diagnostics'
  location: location
  properties: {
    publisher: 'Microsoft.Azure.Diagnostics'
    type: 'IaaSDiagnostics'
    typeHandlerVersion: '1.5'
    autoUpgradeMinorVersion: true
    settings: {
      xmlCfg: base64('<WadCfg> <DiagnosticMonitorConfiguration overallQuotaInMB="4096" xmlns="http://schemas.microsoft.com/ServiceHosting/2010/10/DiagnosticsConfiguration"> <DiagnosticInfrastructureLogs scheduledTransferLogLevelFilter="Error"/> <Logs scheduledTransferPeriod="PT1M" scheduledTransferLogLevelFilter="Error" /> <Directories scheduledTransferPeriod="PT1M"> <IISLogs containerName ="wad-iis-logfiles" /> <FailedRequestLogs containerName ="wad-failedrequestlogs" /> </Directories> <WindowsEventLog scheduledTransferPeriod="PT1M" > <DataSource name="Application!*" /> </WindowsEventLog> <CrashDumps containerName="wad-crashdumps" dumpType="Mini"> <CrashDumpConfiguration processName="WaIISHost.exe"/> <CrashDumpConfiguration processName="WaWorkerHost.exe"/> <CrashDumpConfiguration processName="w3wp.exe"/> </CrashDumps> <PerformanceCounters scheduledTransferPeriod="PT1M"> <PerformanceCounterConfiguration counterSpecifier="\\\\\\\\Memory\\\\\\\\Available MBytes" sampleRate="PT3M" /> <PerformanceCounterConfiguration counterSpecifier="\\\\\\\\Web Service(_Total)\\\\\\\\ISAPI Extension Requests/sec" sampleRate="PT3M" /> <PerformanceCounterConfiguration counterSpecifier="\\\\\\\\Web Service(_Total)\\\\\\\\Bytes Total/Sec" sampleRate="PT3M" /> <PerformanceCounterConfiguration counterSpecifier="\\\\\\\\ASP.NET Applications(__Total__)\\\\\\\\Requests/Sec" sampleRate="PT3M" /> <PerformanceCounterConfiguration counterSpecifier="\\\\\\\\ASP.NET Applications(__Total__)\\\\\\\\Errors Total/Sec" sampleRate="PT3M" /> <PerformanceCounterConfiguration counterSpecifier="\\\\\\\\ASP.NET\\\\\\\\Requests Queued" sampleRate="PT3M" /> <PerformanceCounterConfiguration counterSpecifier="\\\\\\\\ASP.NET\\\\\\\\Requests Rejected" sampleRate="PT3M" /> <PerformanceCounterConfiguration counterSpecifier="\\\\\\\\Processor(_Total)\\\\\\\\% Processor Time" sampleRate="PT3M" /> </PerformanceCounters> </DiagnosticMonitorConfiguration> </WadCfg>')
      storageAccount: 'storageAccount'
    }
    protectedSettings: {
      storageAccountName: 'storageAccountName'
      storageAccountKey: 'storageAccountKey'
      storageAccountEndPoint: 'storageAccountEndPoint'
    }
  }
}
// Insert snippet here

