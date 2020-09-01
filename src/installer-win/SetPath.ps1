param (
  [Parameter()]
  [ValidateNotNullOrEmpty()]
  [string] $AppPath,

  [Parameter()]
  [bool]
  $Remove = $false
)

$ErrorActionPreference = 'Stop';

$registryPath = 'HKCU:\Environment';
$environmentVariable = 'Path';

$environmentKey = Get-Item -path $registryPath;
[string]$currentPath = $environmentKey.GetValue($environmentVariable, '', [Microsoft.Win32.RegistryValueOptions]::DoNotExpandEnvironmentNames);

$paths = $currentPath.Split(@(';'), [System.StringSplitOptions]::RemoveEmptyEntries);

if($Remove)
{
  # remove path
  if($paths -contains $AppPath)
  {
    $paths = $paths | Where-Object { $_ -ne $AppPath };
    $modified = $true;
  }
}
else
{
  # upsert path
  if(-not ($paths -contains $AppPath))
  {
    $paths += $AppPath;
    $modified = $true;
  }
}

if(-not $paths)
{
  $paths = @();
}

if($modified)
{
  $newPath = [string]::Join(';', $paths);
  New-ItemProperty -Path $registryPath -Name $environmentVariable -PropertyType ExpandString -Value $newPath -Force | Out-Null;
}
