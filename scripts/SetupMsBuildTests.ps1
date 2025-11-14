[CmdletBinding()]
param (
  [Parameter(Mandatory = $true)]
  [ValidateNotNullOrWhiteSpace()]
  [ValidateSet(
    'Debug',
    'Release'
  )]
  [string]
  $Configuration,

  [Parameter(Mandatory = $true)]
  [ValidateNotNullOrWhiteSpace()]
  [ValidateSet(
    'win-x64',
    'linux-x64',
    'linux-musl-x64',
    'osx-x64',
    'linux-arm64',
    'win-arm64',
    'osx-arm64'
  )]
  [string]
  $RuntimeIdentifier
)

function CopyFilesToDirectory($sourceDir, $fileNameFilter, $destinationDir) {
  New-Item -ItemType Directory -Path $destinationDir -Force | Out-Null;
  $files = Get-ChildItem -Path $sourceDir -Filter $fileNameFilter -File;
  foreach ($file in $files) {
    $sourceFile = $file.FullName;
    $destinationFile = Join-Path -Path $destinationDir -ChildPath $file.Name;
    Write-Output "  Copying file from '$sourceFile' to '$destinationFile'..."
    Copy-Item -Path $sourceFile -Destination $destinationFile -Force;
  }
}

function RemoveFilesFromDirectory($dir, $fileNameFilter) {
  if( -not (Test-Path -Path $dir)) {
    Write-Output "  Directory '$dir' does not exist. Skipping removal of files matching '$fileNameFilter'."
    return;
  }

  $files = Get-ChildItem -Path $dir -Filter $fileNameFilter -File;
  foreach ($file in $files) {
    $fileToRemove = $file.FullName;
    Write-Output "  Removing file '$fileToRemove'..."
    Remove-Item -Path $fileToRemove -Force;
  }
}

$ErrorActionPreference = 'Stop';
$TargetFramework = 'net10.0';

$msbuildNuGetFileGlob = "Azure.Bicep.MSBuild.*.nupkg";
$bicepCliNuGetFileGlob = "Azure.Bicep.CommandLine.$RuntimeIdentifier.*.nupkg";

$repoRoot = Join-Path -Path $PSScriptRoot -ChildPath '..';
Write-Output "Repo root = $repoRoot";
Write-Output "Configuration = $Configuration";
Write-Output "RuntimeIdentifier = $RuntimeIdentifier";

$bicepCliProjectDir = Join-Path -Path $repoRoot -ChildPath 'src' -AdditionalChildPath 'Bicep.Cli';
$bicepCliProjectFile = Join-Path -Path $bicepCliProjectDir -ChildPath 'Bicep.Cli.csproj';
$bicepCliPublishDir = Join-Path -Path $bicepCliProjectDir -ChildPath 'bin' -AdditionalChildPath @($Configuration, $TargetFramework, $RuntimeIdentifier, 'publish');

$bicepCliNugetProjectDir = Join-Path -Path $repoRoot -ChildPath 'src' -AdditionalChildPath 'Bicep.Cli.NuGet';
$bicepCliNugetProjectFile = Join-Path -Path $bicepCliNugetProjectDir -ChildPath 'nuget.proj';
$bicepCliNugetProjectToolsDir = Join-Path -Path $bicepCliNugetProjectDir -ChildPath 'tools';

$msbuildProjectDir = Join-Path -Path $repoRoot -ChildPath 'src' -AdditionalChildPath 'Bicep.MSBuild';
$msbuildProjectFile = Join-Path -Path $msbuildProjectDir -ChildPath 'Bicep.MSBuild.csproj';
$msbuildProjectOutputDir = Join-Path $repoRoot -ChildPath 'out';

$e2eTestProjectDir = Join-Path -Path $repoRoot -ChildPath 'src' -AdditionalChildPath 'Bicep.MSBuild.E2eTests';
$e2eTestProjectPackageDir = Join-Path -Path $e2eTestProjectDir -ChildPath 'examples' -AdditionalChildPath 'local-packages';

Write-Output "Cleaning old NuGet packages...";
RemoveFilesFromDirectory -dir $msbuildProjectOutputDir -fileNameFilter $msbuildNuGetFileGlob;
RemoveFilesFromDirectory -dir $bicepCliNugetProjectDir -fileNameFilter $bicepCliNuGetFileGlob;

RemoveFilesFromDirectory -dir $e2eTestProjectPackageDir -fileNameFilter '*.nupkg';

Write-Output "Publishing Bicep CLI...";
dotnet publish --configuration $Configuration --self-contained true -r $RuntimeIdentifier $bicepCliProjectFile

Write-Output "Building and packing Bicep MSBuild package...";
dotnet build --configuration $Configuration $msbuildProjectFile
dotnet pack --configuration $Configuration $msbuildProjectFile

Write-Output "Preparing Bicep CLI package prerequisites...";
CopyFilesToDirectory -sourceDir $bicepCliPublishDir -fileNameFilter 'bicep*' -destinationDir $bicepCliNugetProjectToolsDir;

Write-Output "Creating Bicep CLI NuGet package...";
dotnet build --configuration $Configuration /p:RuntimeSuffix=$RuntimeIdentifier $bicepCliNugetProjectFile

Write-Output "Setting up Bicep MSBuild E2E test prerequisites...";
CopyFilesToDirectory -sourceDir $msbuildProjectOutputDir -fileNameFilter $msbuildNuGetFileGlob -destinationDir $e2eTestProjectPackageDir;
CopyFilesToDirectory -sourceDir $bicepCliNugetProjectDir -fileNameFilter $bicepCliNuGetFileGlob -destinationDir $e2eTestProjectPackageDir;

