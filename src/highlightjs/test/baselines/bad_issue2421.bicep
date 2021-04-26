//comment - this is highlighted correctly

resource webApp 'Microsoft.Web/sites@2020-12-01' = {
  name: 'foo'
  //comment inside resource - this is not highlighted correctly.  Further highlighting also gets broken
}

resource webApp 'Microsoft.Web/sites@2020-12-01' = {
  name: 'foo'
  //comment inside resource - still broken
}
