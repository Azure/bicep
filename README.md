# GitHub Action bicep

This action can be used to to consume and deploy bice files.
It actually does the following

1. Consume bicep file (main.bicep).
2. Build ARM json from bicep file.
3. Deploy ARM json in Azure.

To be able to execute scripts in Azure your Action needs to be logged into Azure.
The Action [azlogin](https://github.com/segraef/azlogin) does that for you.

## Usage

```
- name: Bicep build and deploy
  uses: segraef/bicep@v1
  with:
    bicepFile: main.bicep
    location: eastus
    rg: bicep-rg

```


## Requirements

[segraef/azlogin@v1](https://github.com/segraef/azlogin)

## Parameters

- `bicepFile` - **Mandatory**
- `location` - **Mandatory**
- `rg` - **Mandatory**
