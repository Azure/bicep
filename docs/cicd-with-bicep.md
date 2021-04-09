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

## Build with MSBuild
If your existing CI pipeline heavily relies on MSBuild, you can use our MSBuild task and CLI packages to compile your .bicep files into ARM templates.

This functionality relies on the following NuGet packages:
| Package Name | Description |
|:-|-|
| `Azure.Bicep.MSBuild` | Cross-platform MSBuild task to invoke the Bicep CLI to compile .bicep files into ARM templates |
| `Azure.Bicep.CommandLine.win-x64` | Bicep CLI for Windows |
| `Azure.Bicep.CommandLine.linux-x64` | Bicep CLI for Linux |
| `Azure.Bicep.CommandLine.osx-x64` | Bicep CLI for OSX |

### `Azure.Bicep.CommandLine.*` packages
When referenced in a project file via a `PackageReference`, the `Azure.Bicep.CommandLine.*` packages set the `BicepPath` property to the full path of the Bicep executable for the platform. The reference to this package may be omitted if Bicep CLI is installed through other means and the `BicepPath` environment variable or MSBuild property are set accordingly.

### `Azure.Bicep.MSBuild` package
When referenced in a project file via a `PackageReference`, the `Azure.Bicep.MSBuild` package imports the `Bicep` task used to invoke the Bicep CLI and convert its output into MSBuild errors and the `BicepCompile` target used to simplify the usage of the `Bicep` task. By default the `BicepCompile` runs after the `Build` target and will compile all `@(Bicep)` items and place the output in `$(OutputPath)` with the same file name and the `.json` extension.

The following will compile `one.bicep` and `two.bicep` files in the same directory as the project file and placed the compiled `one.json` and `two.json` in the `$(OutputPath)`.
```msbuild
<ItemGroup>
  <Bicep Include="one.bicep" />
  <Bicep Include="two.bicep" />
</ItemGroup>
```

You can override the output path per file using the `OutputFile` metadata on `Bicep` items. The following will recursively find all `main.bicep` files and place the compiled `.json` files in `$(OutputPath)` under a sub-directory with the same name in `$(OutputPath)`:
```msbuild
<ItemGroup>
  <Bicep Include="**\main.bicep" OutputFile="$(OutputPath)\%(RecursiveDir)\%(FileName).json" />
</ItemGroup>
```

Additional customizations can be performed by setting one of the following properties in your project:
| Property Name | Default Value | Description |
|:-|:-|:-|
| `BicepCompileAfterTargets` | `Build` | Used as `AfterTargets` value for the `BicepCompile` target. Change the value to override the scheduling of the `BicepCompile` target in your project. |
| `BicepCompileDependsOn` | None | Used as `DependsOnTargets` value for the `BicepCompile` target. This can be set to targets that you want `BicepCompile` target to depend on. |
| `BicepCompileBeforeTargets` | None | Used as `BeforeTargets` value for the `BicepCompile` target. |
| `BicepOutputPath` | `$(OutputPath)` | Set this to override the default output path for the compiled ARM template. `OutputFile` metadata on `Bicep` items takes precedence over this value. |

The `Azure.Bicep.MSBuild` requires the `BicepPath` property to be set either in order to function. You may set it by referencing the appropriate `Azure.Bicep.CommandLine.*` package for your OS or manually by installing the Bicep CLI and setting the `BicepPath` environment variable or MSBuild property. The examples below assume Windows.

### C# Project file (.csproj) example
> Replace `__PACKAGE_VERSION__` with the latest version of the Bicep NuGet packages.

```msbuild
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netstandard2.1</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Newtonsoft.Json" Version="13.0.1" />
    <PackageReference Include="System.Collections.Immutable" Version="5.0.0" />

    <!-- Bicep Packages -->
    <PackageReference Include="Azure.Bicep.CommandLine.win-x64" Version="__PACKAGE_VERSION__" />
    <PackageReference Include="Azure.Bicep.MSBuild" Version="__PACKAGE_VERSION__" />
  </ItemGroup>

  <ItemGroup>
    <Bicep Include="**\main.bicep" OutputFile="$(OutputPath)\%(RecursiveDir)\%(FileName).json" />
  </ItemGroup>
</Project>
```

### NoTargets SDK example
> Replace `__PACKAGE_VERSION__` with the latest version of the Bicep NuGet packages.

```msbuild
<Project Sdk="Microsoft.Build.NoTargets">
  <PropertyGroup>
    <TargetFramework>net472</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Azure.Bicep.CommandLine.win-x64" Version="__PACKAGE_VERSION__" />
    <PackageReference Include="Azure.Bicep.MSBuild" Version="__PACKAGE_VERSION__" />
  </ItemGroup>

  <ItemGroup>
    <Bicep Include="main.bicep"/>
  </ItemGroup>
</Project>
```