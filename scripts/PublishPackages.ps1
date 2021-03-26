<#
Prerequisites:
- latest nuget.exe somewhere on your machine
- API key to nuget.org with appropriate permissions (this is only available to Bicep team admins)

Parameters:
- PackageDirectory - directory where the nuget packages to publish are located. This should come from
                     official build artifacts.

- NuGetPath        - directory containing the nuget tool you want to use. If not specified, PATH will 
                     be used.

Usage example:
  .\PublishPackages.ps1 -PackageDirectory D:\NugetUpload\drop_build_main -NuGetPath D:\NugetUpload
#>

param (
    [Parameter(Mandatory = $true)]
    [string]
    $PackageDirectory,

    [Parameter(Mandatory = $false)]
    [string]
    $NuGetPath
)

$ErrorActionPreference = 'Stop';

# there doesn't appear to be an AAD-based credential provider for nuget.org
# we will instead prompt for the API key (the user name doesn't matter)
$cred = Get-Credential;
$apiKey = $cred.GetNetworkCredential().Password;

$nugetToolPath = 'nuget.exe';
if($NuGetPath)
{
  $nugetToolPath = Join-Path -Path $NuGetPath -ChildPath $nugetToolPath;
}

$packageSearchPath = Join-Path $PackageDirectory -ChildPath '*.nupkg';

& $nugetToolPath push $packageSearchPath -Source nuget.org -ApiKey $apiKey -SkipDuplicate