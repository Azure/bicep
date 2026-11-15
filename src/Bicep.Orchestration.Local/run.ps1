#!/usr/bin/env pwsh
# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

#Requires -Version 7.0

$ErrorActionPreference = 'Stop'
$root = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$project = $PSScriptRoot

Push-Location $root
try {
    & npm --prefix "$project/web" ci
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    & npm --prefix "$project/web" run build
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    & dotnet build "$project/Bicep.Orchestration.Local.csproj" --nologo
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    # Run in the foreground so Ctrl+C reaches the host and triggers graceful shutdown.
    & dotnet "$project/bin/Debug/net10.0/Bicep.Orchestration.Local.dll" @args
    exit $LASTEXITCODE
}
finally {
    Pop-Location
}
