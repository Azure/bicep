
param (
  # Folder containing bicep executable
  [Parameter(Mandatory = $true)]
  [string]
  $BicepPath,

  # E.g. ~/repos/bicep-registry-modules/modules
  #   or ~/repos/azure-quickstart-templates/quickstarts or 
  [Parameter(Mandatory = $true)]
  [string]
  $ExamplesPath,

  [Parameter(Mandatory = $true)]
  [string]
  $RegistryName,

  [Parameter(Mandatory = $true)]
  [string]
  $Tag
)

$ErrorActionPreference = 'Stop';

$previousLocation = Get-Location;
try
{
  Set-Location $BicepPath;

  $levelDirs = Get-ChildItem -Path $ExamplesPath;

  foreach ($levelDir in $levelDirs) {
    $moduleDirs = Get-ChildItem -Path $levelDir.FullName;
    
    foreach ($moduleDir in $moduleDirs) {
      $bicep = Join-Path -Path $moduleDir.Fullname -ChildPath 'main.bicep';

      if(Test-Path $bicep)
      {
        $artifactRef = "br:$($RegistryName).azurecr.io/examples/$($levelDir.Name)/$($moduleDir.Name):$($Tag)";
        Write-Output $artifactRef;
        ./bicep publish $bicep --target $artifactRef
      }
    }
  }
}
finally
{
  # restore previous location
  Set-Location $previousLocation;
}

