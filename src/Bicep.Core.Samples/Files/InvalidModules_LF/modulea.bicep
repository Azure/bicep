param stringParamA string = 'test'
param stringParamB string
param objParam object
param arrayParam array
@secure()
param secureStringParam string = ''
@secure()
param secureObjectParam object = {}

output stringOutputA string = stringParamA
output stringOutputB string = stringParamB
output objOutput object = objParam
output arrayOutput array = arrayParam