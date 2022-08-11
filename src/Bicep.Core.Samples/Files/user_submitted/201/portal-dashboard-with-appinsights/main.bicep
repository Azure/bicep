param location string = resourceGroup().location
param appInsightsName string

resource appinsights 'Microsoft.Insights/components@2018-05-01-preview' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}

resource dashboard 'Microsoft.Portal/dashboards@2015-08-01-preview' = {
  name: guid(resourceGroup().name, 'dashboard')
  location: location
  tags: {
    'hidden-title': 'Bicep Sample Dashboard'
  }
  properties: {
    lenses: {
      '0': {
        order: 0
        parts: {
          '0': {
            position: {
              colSpan: 10
              rowSpan: 5
              x: 0
              y: 0
            }
            metadata: {
              type: 'Extension/Microsoft_OperationsManagementSuite_Workspace/PartType/LogsDashboardPart'
              inputs: [
                {
                  name: 'Scope'
                  value: {
                    resourceIds: [
                      appinsights.id
                    ]
                  }
                }
                {
                  name: 'Dimensions'
                  value: {
                    xAxis: {
                      name: 'timestamp'
                      type: 'datetime'
                    }
                    yAxis: [
                      {
                        name: 'Number of Requests'
                        type: 'long'
                      }
                    ]
                    splitBy: [
                      {
                        name: 'operation_Name'
                        type: 'string'
                      }
                    ]
                    aggregation: 'Sum'
                  }
                }
                {
                  name: 'PartId'
                  value: guid(resourceGroup().name, 'part0')
                }
                {
                  name: 'Version'
                  value: '2.0'
                }
                {
                  name: 'TimeRange'
                  value: 'PT30M'
                }
                {
                  name: 'Query'
                  value: 'set query_bin_auto_size=5m;\r\nrequests\r\n| summarize [\'Number of Requests\']=count() by operation_Name, bin_auto(timestamp)\r\n| render areachart'
                }
                {
                  name: 'PartTitle'
                  value: 'Forwarded Requests per Backend'
                }
                {
                  name: 'PartSubTitle'
                  value: 'On 5-Minute aggregation'
                }
                {
                  name: 'ControlType'
                  value: 'FrameControlChart'
                }
                {
                  name: 'SpecificChart'
                  value: 'StackedArea'
                }
              ]
            }
          }
          '1': {
            position: {
              colSpan: 6
              rowSpan: 5
              x: 10
              y: 0
            }
            metadata: {
              type: 'Extension/Microsoft_OperationsManagementSuite_Workspace/PartType/LogsDashboardPart'
              inputs: [
                {
                  name: 'Scope'
                  value: {
                    resourceIds: [
                      appinsights.id
                    ]
                  }
                }
                {
                  name: 'Dimensions'
                  value: {
                    xAxis: {
                      name: 'Region'
                      type: 'string'
                    }
                    yAxis: [
                      {
                        name: 'Count'
                        type: 'long'
                      }
                    ]
                    splitBy: []
                    aggregation: 'Sum'
                  }
                }
                {
                  name: 'PartId'
                  value: guid(resourceGroup().name, 'part1')
                }
                {
                  name: 'Version'
                  value: '2.0'
                }
                {
                  name: 'TimeRange'
                  value: 'PT30M'
                }
                {
                  name: 'Query'
                  value: 'requests\r\n| summarize Count=count() by Region=client_CountryOrRegion\r\n| render piechart'
                }
                {
                  name: 'PartTitle'
                  value: 'Requests by Client Region'
                }
                {
                  name: 'PartSubTitle'
                  value: 'A cool pie chart'
                }
                {
                  name: 'ControlType'
                  value: 'FrameControlChart'
                }
                {
                  name: 'SpecificChart'
                  value: 'Pie'
                }
              ]
            }
          }
        }
      }
    }
  }
}
