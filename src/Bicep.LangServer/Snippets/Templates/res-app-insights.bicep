// Application Insights for Web Apps
resource /*${1:appInsightsComponents}*/appInsightsComponents 'Microsoft.Insights/components@2020-02-02' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  kind: 'web'
  properties: {
    Application_Type: /*'${4|web,other|}'*/'web'
  }
}
