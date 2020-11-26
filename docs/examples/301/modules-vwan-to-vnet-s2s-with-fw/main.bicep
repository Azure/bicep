targetScope = 'subscription'

param location string {
  default: 'westeurope'
  metadata: {
    description: 'Specify the location for the hub Virtual Network and its related resources'
  }
}
param vwanlocation string {
  default: 'eastus'
  metadata: {
    description: 'Specify the location for the vWAN and its related resources'
  }
}
param nameprefix string {
  default: 'contoso'
  metadata: {
    description: 'Specify the name prefix for all resources and resource groups'
  }
}
param psk string {
  secure: true
  metadata: {
    'description': 'PSK to use for the site to site tunnel between Virtual Hub and On-Prem VNet'
  }
}

var vnetname = '${nameprefix}-vnet'
var vpngwname = '${vnetname}-vpn-gw'
var vpngwpipname = '${vnetname}-vpn-gw'
var vpnconname = '${vnetname}-to-${vhubname}-cn'
var lgwname = '${vwanlocation}-site-lgw'
var fwname = '${vnetname}-fw'
var fwpolicyname = '${nameprefix}-${location}-fw-policy'
var fwpipname = '${vnetname}-fw-pip'
var fwprefixname = '${vnetname}-fw-ipprefix'
var vwanname = '${nameprefix}-vwan'
var vhubname = '${nameprefix}-vhub-${vwanlocation}'
var vhubfwname = '${vhubname}-fw'
var vhubfwpolicyname = '${nameprefix}-${vwanlocation}-fw-policy'
var vhubvpngwname = '${vhubname}-vpn-gw'

resource hubrg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
  name: '${nameprefix}-hubvnet-rg'
  location: vwanlocation
}

resource vwanrg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
  name: '${nameprefix}-vwan-rg'
  location: location
}

module vnet './vnet.bicep' = {
  name: vnetname
  scope: resourceGroup(hubrg.name)
  params: {
    vnetname: vnetname
    location: location
    addressprefix: '10.0.0.0/20'
    serversubnetprefix: '10.0.0.0/24'
    bastionsubnetprefix: '10.0.1.0/24'
    firewallsubnetprefix: '10.0.2.0/24'
    gatewaysubnetprefix: '10.0.3.0/24'
  }
}

module vpngw './vnetvpngw.bicep' = {
  name: 'vpngw-deploy'
  scope: resourceGroup(hubrg.name)
  params: {
    location: location
    vpngwname: vpngwname
    subnetref: vnet.outputs.subnets[2].id
    vpngwpipname: vpngwpipname
    asn: 65010
  }
}

module fwpolicy './azfwpolicy.bicep' = {
  name: 'fwpolicy-deploy'
  scope: resourceGroup(hubrg.name)
  params: {
    policyname: fwpolicyname
    location: location
  }
}

module fwpip './azfwpip.bicep' = {
  name: 'pip-deploy'
  scope: resourceGroup(hubrg.name)
  params: {
    location: location
    pipname: fwpipname
    ipprefixlength: 31
    ipprefixname: fwprefixname
  }
}

module fw './azfw.bicep' = {
  name: 'fw-deploy'
  scope: resourceGroup(hubrg.name)
  params: {
    location: location
    fwname: fwname
    fwtype: 'VNet'
    fwpolicyid: fwpolicy.outputs.id
    publicipid: fwpip.outputs.id
    subnetid: vnet.outputs.subnets[3].id
  }
}

module vwan './vwan.bicep' = {
  name: 'vwan-deploy'
  scope: resourceGroup(vwanrg.name)
  params: {
    location: vwanlocation
    wanname: vwanname
    wantype: 'Standard'
  }
}

module vhub 'vhub.bicep' = {
  name: 'vhub-deploy'
  scope: resourceGroup(vwanrg.name)
  params: {
    location: vwanlocation
    hubname: vhubname
    hubaddressprefix: '10.10.0.0/24'
    wanid: vwan.outputs.id
  }
}

module vhubfwpolicy './azfwpolicy.bicep' = {
  name: 'vhubfwpolicy-deploy'
  scope: resourceGroup(vwanrg.name)
  params: {
    policyname: vhubfwpolicyname
    location: vwanlocation
  }
}

module vhubfw 'azfw.bicep' = {
  name: 'vhubfw-deploy'
  scope: resourceGroup(vwanrg.name)
  params: {
    location: vwanlocation
    fwname: vhubfwname
    fwtype: 'vWAN'
    hubid: vhub.outputs.id
    hubpublicipcount: 1
    fwpolicyid: vhubfwpolicy.outputs.id
  }
}

module vhubvpngw 'vhubvpngw.bicep' = {
  name: 'vhubvpngw'
  scope: resourceGroup(vwanrg.name)
  params: {
    location: vwanlocation
    hubvpngwname: vhubvpngwname
    hubid: vhub.outputs.id
    asn: 65515
  }
}

module vwanvpnsite './vwanvpnsite.bicep' = {
  name: 'vwanvpnsite-deploy'
  scope: resourceGroup(vwanrg.name)
  params: {
    vpnsitename: '${location}-vpnsite'
    location: vwanlocation
    addressprefix: '10.0.0.0/20'
    bgppeeringpddress: vpngw.outputs.vpngwbgpaddress
    ipaddress: vpngw.outputs.vpngwip
    remotesiteasn: vpngw.outputs.bgpasn
    wanid: vwan.outputs.id
  }
}

module vhubs2s './vhubvpngwcon.bicep' = {
  name: 'vhubs2s-deploy'
  scope: resourceGroup(vwanrg.name)
  params: {
    hubvpngwname: vhubvpngw.outputs.name
    psk: psk
    vpnsiteid: vwanvpnsite.outputs.id
  }
}

module vnets2s './vnetsitetosite.bicep' = {
  name: 'vnets2s-deploy'
  scope: resourceGroup(hubrg.name)
  params: {
    location: location
    localnetworkgwname: lgwname
    addressprefixes: [
      '10.10.0.0/24'
    ]
    connectionname: vpnconname
    bgppeeringpddress: vhubvpngw.outputs.gwprivateip
    gwipaddress: vhubvpngw.outputs.gwpublicip
    remotesiteasn: vhubvpngw.outputs.bgpasn
    psk: psk
    vpngwid: vpngw.outputs.id
  }
}