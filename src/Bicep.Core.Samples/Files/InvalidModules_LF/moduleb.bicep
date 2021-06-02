param stringParamA string = 'test'
param stringParamB string
param objParam object
param arrayParam array

@secure()
param secureStringParam string = ''
@secure()
param secureObjectParam object = {}
