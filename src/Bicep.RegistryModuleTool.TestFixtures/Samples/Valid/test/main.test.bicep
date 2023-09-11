module testMain '../main.bicep' = {
  name: 'testMain'
  params: {
    sshRSAPublicKey: ''
    dnsPrefix: ''
    linuxAdminUsername: ''
    servicePrincipalClientSecret: ''
    servicePrincipalClientId: ''
    osDiskSizeGB: 1 
  }
}
