
param (
  [Parameter(Mandatory = $true)]
  [string]
  $BicepPath,

  [Parameter(Mandatory = $true)]
  [string]
  $ExamplesPath,

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
      $json = Join-Path -Path $moduleDir.Fullname -ChildPath 'main.json';

      if((Test-Path $bicep) -and (Test-Path $json))
      {
        $artifactRef = "br:majastrzoci.azurecr.io/examples/$($levelDir.Name)/$($moduleDir.Name):$($Tag)";
        Write-Output $artifactRef;
        .\bicep.exe publish $bicep --target $artifactRef
      }
    }
  }
}
finally
{
  # restore previous location
  Set-Location $previousLocation;
}

