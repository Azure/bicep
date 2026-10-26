#!/usr/bin/env pwsh

[CmdletBinding()]
param (
    [Parameter(Mandatory)]
    [string] $BicepBaseFolder
)

$ErrorActionPreference = 'Stop'

function ExecSafe([scriptblock] $ScriptBlock) {
    & $ScriptBlock
    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE
    }
}

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '../../../..')).Path
$projectPath = "$repoRoot/src/Bicep.Cli/Bicep.Cli.csproj"
$buildOutputPath = "$repoRoot/src/Bicep.Cli/bin/profile/Release"
$bicepPath = if ($IsWindows) { "$buildOutputPath/bicep.exe" } else { "$buildOutputPath/bicep" }

ExecSafe { dotnet build $projectPath --configuration Release --output $buildOutputPath --nologo }

$traceFile = Join-Path ([System.IO.Path]::GetTempPath()) "profile-$([DateTime]::UtcNow.ToString('yyyyMMdd-HHmmss')).nettrace"
$bicepPattern = [System.IO.Path]::Combine($BicepBaseFolder, '**/*.bicep')

ExecSafe { dotnet trace collect `
    --show-child-io `
    --profile 'gc-verbose,dotnet-common,dotnet-sampled-thread-time' `
    --output $traceFile `
    -- $bicepPath build --pattern "$bicepPattern" }