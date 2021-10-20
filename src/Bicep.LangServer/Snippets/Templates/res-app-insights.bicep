// Application Insights for Web Apps
resource /*${1:appInsightsComponents}*/appInsightsComponents 'Microsoft.Insights/components@2020-02-02-preview' = {
  name: /*${2:'name'}*/'name'
  location: location
  kind: 'web'
  properties: {
    Application_Type: /*'${3|web,other|}'*/'web'
  }
}
