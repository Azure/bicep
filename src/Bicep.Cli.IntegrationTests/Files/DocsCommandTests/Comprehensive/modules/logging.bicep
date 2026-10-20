metadata description = 'Configures diagnostic logging.'

@description('Secret passed from the parent module.')
@secure()
param secret string

output configured bool = !empty(secret)
