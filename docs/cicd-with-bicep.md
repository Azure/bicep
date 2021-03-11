# Adding Bicep to a CI/CD pipeline

As your Bicep practice matures, you will want to check-in your Bicep code into source control and kick off a pipeline or workflow, which would do the following:

1. Build your Bicep file into an ARM Template
1. Deploy the generated ARM template

In order to do this, we need to make sure the Bicep CLI is installed on the build agent. For now, Bicep is not preinstalled on any build agents or tasks provided by Microsoft, but installing it manually as part of the pipeline is straightforward.

The following example is designed to be run in GitHub actions workflow and uses Azure CLI, but could be easily adapted to run in a Azure DevOps Pipeline. It assumes the following prerequisite:

* The Bicep file you want to transpile and deploy is called `main.bicep` and exists in the root of the repo
* You are deploying the transpiled ARM Template to a resource group. Deploying to another scope like a subscription requires a different CLI command.

```yaml

name: bicep build and deploy

on: push

jobs:
  bicep-build-and-deploy:
    name: bicep build and deploy
    runs-on: ubuntu-latest

    steps:
      # Checks out a copy of your repository on the ubuntu-latest machine
      - name: Checkout code
        uses: actions/checkout@v2

      # Install the latest release of the bicep CLI
      - name: Install bicep CLI
        run: |
          curl -Lo bicep https://github.com/Azure/bicep/releases/latest/download/bicep-linux-x64
          chmod +x ./bicep
          sudo mv ./bicep /usr/local/bin/bicep
          bicep --help
           
      # Transpile bicep file into ARM template
      - name: Build ARM Template from bicep file
        run: |
          bicep build ./main.bicep
      
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
            az deployment group what-if -f ./main.json -p ./parameters.json -g my-rg

      # You may want a human approval in between the what-if step 
      # and the deploy step to evaluate output before deployment

      # Deploy template
      - name: Deploy template
        uses: azure/CLI@v1
        with:
          inlineScript: |
            az account show
            az deployment group create -f ./main.json -g my-rg
```

Instead of installing the Bicep CLI manually, you may instead want to use the [community-maintained github action](https://github.com/marketplace/actions/bicep-build) from [@justinyoo](https://github.com/justinyoo) that can run `bicep build` on your behalf.
