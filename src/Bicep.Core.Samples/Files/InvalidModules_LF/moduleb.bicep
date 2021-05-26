param stringParamA string = 'test'
param stringParamB string
param objParam object
param arrayParam array

@secure()
param secureStringParam string = ''
@secure()
param secureObjectParam object = {}

param secureStringParam2 string {
  secure: true
  default: ''
}
param secureObjectParam2 object {
  secure: true
  default: {}
}