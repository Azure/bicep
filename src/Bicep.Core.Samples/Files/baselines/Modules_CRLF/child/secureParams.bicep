@secure()
param secureStringParam1 string
@secure()
param secureStringParam2 string = ''

output exposedSecureString string = secureStringParam1