
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

  # Registry name (to be placed in front of ".azurecr.io")
  [Parameter(Mandatory = $true)]
  [string]
  $RegistryName,

  [Parameter(Mandatory = $true)]
  [string]
  $Tag,

  [Parameter(Mandatory = $false)]
  [string]
  $RegistryPathPrefix = "examples"
)

$ErrorActionPreference = 'Stop';

# Resolve paths before we change current location
$BicepPath = Resolve-Path $BicepPath
$ExamplesPath = Resolve-Path $ExamplesPath

$previousLocation = Get-Location;
try {
  Set-Location $BicepPath;

  $levelDirs = Get-ChildItem -Path $ExamplesPath;

  foreach ($levelDir in $levelDirs) {
    $moduleDirs = Get-ChildItem -Path $levelDir.FullName -Directory

    foreach ($moduleDir in $moduleDirs) {
      $modulePath = Join-Path -Path $moduleDir.Fullname -ChildPath 'main.bicep'
      Write-Host "Checking for $($modulePath)"

      if (Test-Path $modulePath) {
        $artifactRef = "br:$($RegistryName).azurecr.io/$($RegistryPathPrefix)/$($levelDir.Name.ToLowerInvariant())/$($moduleDir.Name.ToLowerInvariant()):$($Tag)";

        Write-Host "./bicep publish $modulePath --target $artifactRef --with-source --force"
        ./bicep publish $modulePath --target $artifactRef --with-source --force

        # Write out the bicep needed to reference the module
        Write-Output @"
// $($levelDir.Name)/$($moduleDir.Name)
module $($moduleDir.Name -replace "-","_") '$($artifactRef)' = {
  name: '$($moduleDir.Name)'
  params: {}
}
"@
        Write-Host "Published $($modulePath) to $($artifactRef)"
        Write-Host
      }
    }
  }
}
finally {
  # restore previous location
  Set-Location $previousLocation;
}

