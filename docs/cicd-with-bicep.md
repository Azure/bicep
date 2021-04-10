# Adding Bicep to a CI/CD pipeline

As your Bicep practice matures, you will want to check-in your Bicep code into source control and kick off a pipeline or workflow, which would do the following:

1. Build your Bicep file into an ARM Template
1. Deploy the generated ARM template

With the current Azure CLI 2.20 now installed in GitHub and also on Azure DevOps, Bicep CLI can be automatically triggerd by using `az bicep build` command and an explicit task to download Bicep CLI is no more needed.

The two examples below illustrates this. It assumes the following prerequisite:

* The Bicep file you want to transpile and deploy is called `main.bicep` and exists in the root of the repo
* The parameters file you want to use is called `parameters.json` and exists in the root of the repo
* You are deploying the transpiled ARM Template to a resource group. Deploying to another scope like a subscription requires a different CLI command.

## GitHub Workflow

```yaml

name: bicep build and deploy

on: push

env:
  # Common variables
  AZURE_RESOURCE_GROUP: 'myResourceGroupName'

jobs:
  bicep-build-and-deploy:
    name: bicep build and deploy
    runs-on: ubuntu-latest

    steps:
      # Checks out a copy of your repository on the ubuntu-latest machine
      - name: Checkout code
        uses: actions/checkout@v2

      # Transpile bicep file into ARM template
      - name: Build ARM Template from bicep file
        run: |
          az bicep build --files ./main.bicep
      
      # Stop here if you only want to do "CI" which just generates the 
      # build artifact (ARM Template JSON)

      # Login to Azure
      - name: Azure Login
        uses: azure/login@v1
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }} 

      # Emit template what-if to show what template will do
      - name: Run what-if
        uses: azure/CLI@v1
        with:
          inlineScript: |
            az account show
            az deployment group what-if -f ./main.json -p ./parameters.json -g ${{ env.AZURE_RESOURCE_GROUP }}

      # You may want a human approval in between the what-if step 
      # and the deploy step to evaluate output before deployment

      # Deploy template
      - name: Deploy template
        uses: azure/CLI@v1
        with:
          inlineScript: |
            az account show
            az deployment group create -f ./main.json -g ${{ env.AZURE_RESOURCE_GROUP }}
```
## Azure DevOps Pipeline

```yaml
trigger:
- main
name: 'bicep build and deploy'

variables:
  vmImageName: 'ubuntu-latest'
  workingDirectory: '$(System.DefaultWorkingDirectory)/'
  geoLocation: 'West Europe'

  azureServiceConnection: 'My-Azure-DevOps-ServicePrincipalName'
  subscriptionId: 'My-Subscription-Id'
  AZURE_RESOURCE_GROUP: 'myResourceGroupName'
  
stages:
- stage: Build
  displayName: Build
      
  jobs:
  - job: Build
    displayName: Validate and Publish
    pool:
     vmImage: $(vmImageName)
      
    steps:
      - task: AzureCLI@2
        displayName: Build ARM Template from bicep file
        inputs:
          azureSubscription: '$(azureServiceConnection)'
          scriptType: bash
          scriptLocation: inlineScript
          inlineScript: |
            az --version
            az bicep build --files ./main.bicep

      - task: AzureResourceManagerTemplateDeployment@3
        displayName: 'Validate APIM Templates'
        inputs:
          azureResourceManagerConnection: '$(azureServiceConnection)'
          subscriptionId: '$(subscriptionId)'
          resourceGroupName: '$(AZURE_RESOURCE_GROUP)'
          location: '$(geoLocation)'
          csmFile: main.json
          csmParametersFile: parameters.json
          deploymentMode: Validation
          
      - task: CopyFiles@2
        displayName: 'Copy Templates'
        inputs:
          SourceFolder: bicep
          TargetFolder: '$(build.artifactstagingdirectory)'
          
      - task: PublishBuildArtifacts@1
        displayName: 'Publish Artifact: drop'
        inputs:
          PathtoPublish: '$(build.artifactstagingdirectory)'
          ArtifactName: 'drop'

- stage: Development
  displayName: Deploy to Development
  dependsOn: Build
  condition: succeeded()
  jobs:
    - deployment: Deploy
      displayName: 'Deploying APIM Template'
      environment: 'Development'
      pool:
        vmImage: $(vmImageName)
      strategy:
        runOnce:
          deploy:
            steps:
              - task: AzureResourceManagerTemplateDeployment@3
                displayName: 'Deploy/Update APIM (Dev)'
                inputs:
                  azureResourceManagerConnection: '$(azureServiceConnection)'
                  subscriptionId: '$(subscriptionId)'
                  resourceGroupName: '$(AZURE_RESOURCE_GROUP)'
                  location: '$(geoLocation)'
                  csmFile: '$(Pipeline.Workspace)/drop/main.json'
                  csmParametersFile: '$(Pipeline.Workspace)/drop/parameters.json'
                  deploymentMode: 'Incremental'        
```