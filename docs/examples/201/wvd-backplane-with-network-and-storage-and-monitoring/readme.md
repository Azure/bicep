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
| `wvd-fileservices-privateendpoint-module.bicep`         | Bicep module to configure PE and Private DNS for Storage Account to WVD VNET            |
| `wvd-LogAnalytics.bicep`         | Bicep module that creates a log analytics workspace                            |
| `wvd-monitor-diag.bicep`         | Bicep module that configured diagnostic settings for WVD components            |
| `wvd-sig-module.bicep`         | Bicep module that creates Shared Image Gallery            |
| `wvd-image-builder-module.bicep`         | Bicep module that configures Azure Image builder and creates template. (Option to run a deployment script to start image build off)            |
| `main.json`                      | The JSON result after transpiling 1. Deploy-Modules.bicep                      |


## main.bicep
This main bicep file creates a WVD environment in Azure, based on Bicep 0.2 creating Resource Groups, WVD Backplane
components, Vnet with Subnet, Storage container with FileShare, Storage Private Endpoint, Log Analytics Workspace, Shared Image Gallery and Azure Image Builder Template. 
This main bicep file calls the bicep modules as outlined below.
 - wvd-backplane-module.bicep
 - wvd-network-module.bicep
 - wvd-fileservices-module.bicep
 - wvd-fileservices-privateendpoint-module.bicep
 - wvd-LogAnalytics.bicep
 - wvd-monitor-diag.bicep
 - wvd-sig-module.bicep
 - wvd-image-builder-module.bicep
 

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

## wvd-fileservices-privateendpoint-module.bicep
This Bicep Module creates a File Services Private Endpoint in Azure connected to the WVD VNET. The following objects are created.
 - Private Endpoint
 - Private DNS Zone Group
 - Private DNS Zone
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

  ## wvd-sig-module.bicep
This Bicep Module creates a Shared Image Gallery and Image Definition. The following objects
are created
 - Shared Image Gallery
 - Shared Image Gallery Image Definition
 This Bicep module can be run separatly or as part of main.bicep

  ## wvd-image-builder-module.bicep
This Bicep Module creates components to run Azure Image Builder and update the Shared Image Gallery Image Definition. 
The following objects are created
 - User Assigned Managed Identity (UAMI)
 - Role for the UAMI to Modify Gallery and Template Images
 - Image Template - Optional Customisations to run Optimization and Teams install Powershell scripts (Uncomment if needed)
 
 Optional (Experimental)
 If parameter InvokeRunImageBuildThroughDeploymentScript in main.bicep is set to True then the following will be triggered:
 - Additional Role definitions created and assigned to UAMI to be able to run Image Template builds, become a managed identity operator and create the relevant container and storage accounts needed to run a script deployment using the Microsoft.Resources/deploymentScripts provider.
 - Using the Microsoft.Resources/deploymentScripts provider, spins up a container and storage account and runs a Powershell script to start the build of the AIB Image and upload the image once complete to the SIG definition created earlier.

 This process may leave some orphaned Resource Groups from Image Builder in your Subscription usually prefixed 'IT_SIGRESOUCEGROUPNAME'. Make sure to delete if not required.
 
 
 This Bicep module can be run separatly or as part of main.bicep

## main.json
This file is the JSON output after transpiling 'main.bicep'
The following command was used: bicep build '.\main.bicep'

## Contributing

This project welcomes contributions and suggestions.
