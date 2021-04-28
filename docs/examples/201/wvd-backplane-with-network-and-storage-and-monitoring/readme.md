---
page_type: resources
languages:
  - md
  - json
  - bicep
description: |
  Multi-module Bicep project that deploys a WVD environment in Azure including some prerequisites that WVD generally needs.
products:
  - azure
  - windows-virtual-desktop
---

#   Multi-module Bicep project for WVD


## Contents


| File/folder                      | Description                                                                    |
|----------------------------------|--------------------------------------------------------------------------------|
| `main.bicep`                     | Main Bicep file that calls bicep modules to deploy WVD,vnet and log analytics  |
| `wvd-backplane-module.bicep`     | Bicep module that creates WVD Hostpool, AppGroup and Workspace                 |
| `wvd-network-module.bicep`       | Bicep module that creates a vnet and subnet                                    |
| `wvd-fileservices-module.bicep`  | Bicep module that creates a storage account and file share                     |
| `wvd-LogAnalytics.bicep`         | Bicep module that creates a log analytics workspace                            |
| `wvd-monitor-diag.bicep`         | Bicep module that configured diagnostic settings for WVD components            |
| `main.json`                      | The JSON result after transpiling 1. Deploy-Modules.bicep                      |


## main.bicep
This main bicep file creates a WVD environment in Azure, based on Bicep 0.2 creating Resource Groups, WVD Backplane
components, Vnet with Subnet, Storage container with FileShare and Log Analytics Workspace. This main bicep files
calls the bicep modules as outlined below.
 - wvd-backplane-module.bicep
 - wvd-network-module.bicep
 - wvd-fileservices-module.bicep
 - wvd-LogAnalytics.bicep
 - wvd-monitor-diag.bicep

The bicep file is based on Bicep 0.2

## wvd-backplane-module.bicep
This Bicep Module creates WVD backplane components in Azure and connects all objects. The following objetcs
are created.
 - WVD Hostpool
 - WVD AppGroup
 - WVD Workspace
 This Bicep module can be run separatly or as part of main.bicep
 
 ## wvd-network-module.bicep
This Bicep Module creates networking components in Azure. The following objects are created.
 - Virtual Network
 - Subnet
 This Bicep module can be run separatly or as part of main.bicep

 ## wvd-fileservices-module.bicep
This Bicep Module creates File Services components in Azure. The following objects are created.
 - Storage Account
 - File Share
 This Bicep module can be run separatly or as part of main.bicep

  ## wvd-LogAnalytics.bicep
This Bicep Module creates Log Analytics components in Azure. The following objects are created.
 - Log Analytics Workspace
 This Bicep module can be run separatly or as part of main.bicep

  ## wvd-monitor-diag.bicep
This Bicep Module configures Log Analytics diagnostics settings for WVD components in Azure. The following objects
are configured for Log Analytics
 - Workspace
 - Hostpool
 This Bicep module can be run separatly or as part of main.bicep

## main.json
This file is the JSON output after transpiling 'main.bicep'
The following command was used: bicep build '.\main.bicep'

## Contributing

This project welcomes contributions and suggestions.
