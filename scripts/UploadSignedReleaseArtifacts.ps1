<#
The script downloads signed build artifacts to a local machine and uploads them to the specified release.

Note: Non-MSFT users do not have permissions to execute this script successfully.

Prerequisites:
- GitHub CLI installed on the machine
- Azure CLI installed on the machine
- The azure-devops extension for Azure CLI installed
- Completed login procecures for both Azure CLI and GH CLI

Parameters:
- WorkingDir - Directory used for temporary work. Contents will be deleted.
- TagName    - The tag name of the GitHub release release. Should be in the format v<x>.<y>.<z> where <x>, <y>, and <z> are numbers.
- BuildId    - Set it to a DevOps build ID to use a specific signed build instead of the latest.

#>
[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)]
    [string] $WorkingDir,

    [Parameter(Mandatory = $true)]
    [string] $TagName,

    [Parameter(Mandatory = $false)]
    [int] $BuildId
)

$ErrorActionPreference = 'Stop';

if($PSVersionTable.PSEdition -ne 'Core')
{
  # The Compress-Archive cmdlet on Windows PowerShell uses \ as the directory separator charactor
  # when creating .zip files. When extracted on a non-Windows machine, \ is a valid file name character
  # so the user gets a flat list of files instead of directories and subdirectories.
  # This was fixed in PS Core/.net 5.
  Write-Error "This script requires PowerShell Core to run.";
}

$org = 'https://dev.azure.com/msazure/';
$project = 'One';
$pipelineName = 'BicepMirror-Official'

if($TagName -notmatch 'v\d+\.\d+\.\d+')
{
  Write-Error "The specified tag name '$($TagName)' is not in the expected format 'v<x>.<y>.<z>' where <x>, <y>, and <z> are numbers.";
}

$buildVersion = $TagName.Substring(1);

$artifacts = @(
  @{
    buildArtifactName = 'drop_build_bicep_linux';
    assets = @(
      @{
        assetName = 'bicep-linux-x64';
        relativePath = 'bicep-Release-linux-x64/bicep';
      }
    );
  },
  @{
    buildArtifactName = 'drop_build_bicep_linux_musl';
    assets = @(
      @{
        assetName = 'bicep-linux-musl-x64';
        relativePath = 'bicep-Release-linux-musl-x64/bicep';
      }
    );
  },
  @{
    buildArtifactName = 'drop_build_bicep_osx';
    assets = @(
      @{
        assetName = 'bicep-osx-x64';
        relativePath = 'bicep-Release-osx-x64/bicep';
      }
    );
  },
  @{
    buildArtifactName = 'drop_build_bicep_windows';
    assets = @(
      @{
        assetName = 'bicep-setup-win-x64.exe';
        relativePath = 'bicep-setup-win-x64/bicep-setup-win-x64.exe';
      },
      @{
        assetName = 'bicep-win-x64.exe';
        relativePath = 'bicep-Release-win-x64/bicep.exe';
      },
      @{
        assetName = "Azure.Bicep.MSBuild.$buildVersion.nupkg";
        relativePath = "Azure.Bicep.MSBuild.$buildVersion.nupkg";
      },
      @{
        assetName = "Azure.Bicep.MSBuild.$buildVersion.snupkg";
        relativePath = "Azure.Bicep.MSBuild.$buildVersion.snupkg";
      }
    );
    zipAssets = @(
      @{
        assetName = 'bicep-langserver.zip';

        # trailing asterisk prevents the bicep.LangServer from becoming the root directory in the generated .zip file
        relativePath = 'bicep.LangServer/*';
      }
    )
  },
  @{
    buildArtifactName = 'drop_build_vsix';
    assets = @(
      @{
        assetName = 'vscode-bicep.vsix';
        relativePath = 'vscode-bicep\vscode-bicep.vsix';
      }
    );
  },
  @{
    buildArtifactName = 'drop_build_packages_windows';
    assets = @(
      @{
        assetName = "Azure.Bicep.CommandLine.win-x64.$buildVersion.nupkg";
        relativePath = "Azure.Bicep.CommandLine.win-x64.$buildVersion.nupkg";
      }
    );
  },
  @{
    buildArtifactName = 'drop_build_packages_linux';
    assets = @(
      @{
        assetName = "Azure.Bicep.CommandLine.linux-x64.$buildVersion.nupkg";
        relativePath = "Azure.Bicep.CommandLine.linux-x64.$buildVersion.nupkg";
      }
    );
  },
  @{
    buildArtifactName = 'drop_build_packages_osx';
    assets = @(
      @{
        assetName = "Azure.Bicep.CommandLine.osx-x64.$buildVersion.nupkg";
        relativePath = "Azure.Bicep.CommandLine.osx-x64.$buildVersion.nupkg";
      }
    );
  }
)

Write-Output "Removing working dir...";
if(Test-Path -Path $WorkingDir)
{
  Remove-Item -Path $WorkingDir -Recurse -Force
}

Write-Output "Creating working dir...";
New-Item -ItemType Directory -Path $WorkingDir -Force;

Write-Output "Creating asset dir...";
$assetDir = Join-Path -Path $WorkingDir -ChildPath '__assets';
New-Item -ItemType Directory -Path $assetDir -Force;

Write-Output "Resolving build definition...";
$buildDefinition = az pipelines show --org $org --project $project --name $pipelineName | ConvertFrom-Json;

if($BuildId)
{
  # TODO we can probably make this better by searching for a specific version instead of build ID
  # get specific build
  Write-Output "Resolving build by ID...";
  $build = az pipelines runs show --org $org --project $project --id $BuildId | ConvertFrom-Json;
  
  # the build could be for any pipeline
  if($build.definition.id -ne $buildDefinition.id) {
    Write-Error "The specified Build ID '$BuildId' belongs to build definition '$($build.definition.name)'. Expected the '$($buildDefinition.name)' build definition instead.";
  }
}
else
{
  # get latest build
  Write-Output "Resolving latest build...";
  $build = (az pipelines runs list --org $org --project $project --pipeline-ids $buildDefinition.id --top 1 | ConvertFrom-Json)[0];
}

Write-Output "Getting artifact URLs...";
$buildArtifacts = az pipelines runs artifact list --org $org --project $project --run-id $build.id | ConvertFrom-Json;

foreach ($artifact in $artifacts)
{
  Write-Output "Processing artifact $($artifact.buildArtifactName)...";

  $artifactDownloadPath = Join-Path -Path $WorkingDir -ChildPath $artifact.buildArtifactName;
  $buildArtifact = $buildArtifacts | Where-Object { $_.name -eq $artifact.buildArtifactName};

  if($buildArtifact)
  {
    # downloads and unzips to the specified directory
    az pipelines runs artifact download --org $org --project $project --artifact-name $artifact.buildArtifactName --path $artifactDownloadPath --run-id $build.id

    foreach ($asset in $artifact.assets) {
      Write-Output "Processing file asset $($asset.assetName)...";
      
      # we need to rename the files before uploading to the release (we have multiple files called "bicep", for example)
      $assetFilePath = Join-Path $artifactDownloadPath -ChildPath $asset.relativePath;
      $assetUploadPath = Join-Path -Path $assetDir -ChildPath $asset.assetName;
      Copy-Item -Path $assetFilePath -Destination $assetUploadPath;

      gh release upload $TagName $assetUploadPath --clobber
    }

    foreach($zipAsset in $artifact.zipAssets)
    {
      Write-Output "Processing zip asset $($zipAsset.assetName)...";

      # directories need to be compressed into zip files before upload
      $archiveSource = Join-Path -Path $artifactDownloadPath -ChildPath $zipAsset.relativePath;
      $assetUploadPath = Join-Path -Path $assetDir -ChildPath $zipAsset.assetName;
      Compress-Archive -Path $archiveSource -DestinationPath $assetUploadPath -CompressionLevel Optimal;

      gh release upload $TagName $assetUploadPath --clobber
    }
  }
  else
  {
    Write-Warning "The artifact '$($artifact.buildArtifactName)' is not present in the specified build.";
  }
}