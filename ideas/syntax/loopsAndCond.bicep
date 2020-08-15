//loop
copy loopName vmCount serial [ //loopname, count, [deploymentMode]
    resource azrm 'network/publicIpAddresses@2016-03-30' pip {
        name: concat(namePrefix, loopName, '-pip1') // name of the loop can be the index/counter
        location: location
        properties: {
            publicIpAllocationMethod: 'Dynamic'
            dnsSettings: {
                domainNameLabel: concat(dnsPrefix, loopName) 
            }
        }
    }
    //multiple resources could be here but there are not many scenarios that would leverage this (predominantly IaaS/VM)
]

//copy - this shows a simple property could state that I want n copies of something, may only work for resources
resource azrm 'network/publicIpAddresses@2016-03-30' pip {
    copies: { // long form or short form below
        count: vmCount
        mode: serial
    }
    copies: count, mode //short form - in this case the loop does not need a name, the resource name can be used, e.g. pip[]
    copies: count // could separate the propeties - need a way to specify the counter, in this case it could be determined by the system, e.g. i, $i, %i
    copyMode: serial
    name: concat(namePrefix, %i, '-pip1') // %i
    location: location
    properties: {
        publicIpAllocationMethod: 'Dynamic'
        dnsSettings: {
            domainNameLabel: concat(dnsPrefix, %i) 
        }
    }
}

//property copy
resource azrm 'network/networkInterfaces@2016-03-30' nic {
        name: concat(namePrefix, i, '-nic1')
        location: location
        properties: {
            copy loopName loopCount: { // repeat what's in the {} n times
                ipConfigurations: [
                    {
                        name: 'ipconfig${loopName}'
                        properties: {
                            privateIPAllocationMethod: 'Dynamic'
                            publicIPAddress: {
                                id: resourceId(pip[loopName]) //loopName is the counter
                            },
                            subnet: {
                                id: varSubnet[loopName] 
                            }
                        }
                    }
                ]
            }
            copy ipAddresses length(openPorts): {
                
            }
            enableIpForwarding: false
        }
    }

//alternative property copy
resource azrm 'network/networkSecurityGroups@2019-08-01' nsg {
    name: concat(namePrefix, 'nsg')
    location: location
    properties: {
        copy securityRules length(openPorts): [ //securityRules is the counter? or we use %i or user defined
            {
                name: 'default-allow-${openPorts[securityRules]}'
                properties: {
                    priority: 1000 + securityRules
                    access: 'Allow'
                    direction: 'Inbound'
                    destinationPortRange: openPorts[securityRules]
                    protocol: 'Tcp'
                    sourceAddressPrefix: '*'
                    sourcePortRange: '*'
                    destinationAddressRange: '*'
                }
            }
        ]
    }
}

//property condition
    resource azrm 'network/publicIpAddresses@2016-03-30' pip {
        name: concat(namePrefix, i, '-pip1')
        location: location
        properties: {
            publicIpAllocationMethod: 'Dynamic'
            dnsSettings: {
                domainNameLabel: if (dnsPrefix != null) {dnsPrefix} else { null } // probably not more readable when the expression is complex
            }
            dnsSettings: dnsPrefix != null ? $_.domainNameLabel: dnsPrefix : { null } // need a way to reference parent since that's usually what's needed in practice
        }
    }

//conditional resource
resource azrm 'network/publicIpAddresses@2016-03-30' pip {
    condition: (publicIp == true) // not sure I can find anything wrong with this
    name: concat(namePrefix, i, '-pip1')
    location: location
    properties: {
        publicIpAllocationMethod: 'Dynamic'
        dnsSettings: {
            domainNameLabel: if (dnsPrefix != null) { dnsPrefix } else { null }
        }
    }
}

// new/existing pattern
resource azrm 'network/publicIpAddresses@2016-03-30' pip when (publicIp == new)  {
    name: concat(namePrefix, i, '-pip1')
    location: location
    properties: {
        publicIpAllocationMethod: 'Dynamic'
        dnsSettings: {
            domainNameLabel: if (dnsPrefix != null) { dnsPrefix } else { null }
        }
    }
} else { // if I need to reference an existing resource instead
    resourceScope: "resouceGroupName" // or root path to resource
    name: "storageAccountName"
}

if(publicIp){ // could combine more in the same loop
    resource azrm 'network/publicIpAddresses@2016-03-30' pip {
        name: concat(namePrefix, i, '-pip1')
        location: location
        properties: {
            publicIpAllocationMethod: 'Dynamic'
            dnsSettings: {
                domainNameLabel: dnsPrefix
            }
        }
    }
}
