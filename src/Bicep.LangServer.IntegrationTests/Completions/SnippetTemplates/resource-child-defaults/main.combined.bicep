// $0 = suppressionId: any(1)
// $1 = testIdentifier
// $2 = Microsoft.Advisor/recommendations/suppressions@2020-01-01
// $3 = 'testResource/'

resource testIdentifier 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
  name: 'testResource/'
  properties: {
    suppressionId: any(1)
  }
}// Insert snippet here

