var region = externalInput('$region')
var config = externalInput('$config')[region]

deploy 'main.bicepparam' = {
  scope: resourceGroup('mySubscription', 'myResourceGroup')
  /* stacks settings */

  prevalidate: {
    
  }

  postvalidate: {

  }
}
