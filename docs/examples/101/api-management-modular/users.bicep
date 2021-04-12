// ****************************************
// Azure Bicep Module:
// Create multiple users from sample JSON records
// ****************************************
@minLength(1)
param apimInstanceName string

var usersDJSet = [
  {
    'userId': 'pvd'
    'firstName': 'Paul'
    'lastName': 'Van Dyk'
    'email': 'pvd@vonyc.de'
    'state': 'active'
    'notes': 'Good DJ'
  }
  {
    'userId': 'abuuren'
    'firstName': 'Armin'
    'lastName': 'van Buuren'
    'email': 'armin@armadamusic.com'
    'state': 'active'
    'notes': 'OK meh!'
  }
]

resource apimUser 'Microsoft.ApiManagement/service/users@2020-06-01-preview' = [for usr in usersDJSet: {
  name: '${apimInstanceName}/${usr.userId}'
  properties: {
    firstName: usr.firstName
    lastName: usr.lastName
    email: usr.email
    state: usr.state
    note: usr.notes
  }
}]
