Param( 
  [string]$bicepFile,
  [string]$location,
  [string]$rg
)

# Preparation

## Install Az Module

Install-Module Az -Scope AllUsers -Acceptlicense -Force

## Install bicep CLI
if ($IsLinux) {
  curl -Lo bicep https://bicepdemo.z22.web.core.windows.net/latest/bicep-linux-x64
  Move-Item -Path bicep -Destination /usr/local/bin/bicep -Force
  chmod +x /usr/local/bin/bicep
  bicep --help
}

if ($IsWindows) {
  $installPath = "$env:USERPROFILE\.bicep"
  $installDir = New-Item -ItemType Directory -Path $installPath -Force
  $installDir.Attributes += 'Hidden'
  (New-Object Net.WebClient).DownloadFile("https://bicepdemo.z22.web.core.windows.net/latest/bicep-win-x64.exe", "$installPath\bicep.exe")
  $currentPath = (Get-Item -path "HKCU:\Environment" ).GetValue('Path', '', 'DoNotExpandEnvironmentNames')
  if (-not $currentPath.Contains("%USERPROFILE%\.bicep")) { setx PATH ($currentPath + ";%USERPROFILE%\.bicep") }
  if (-not $env:path.Contains($installPath)) { $env:path += ";$installPath" }
  bicep --help
}

# Give me some output to see where I am

Write-Output "# InvocationName:" $MyInvocation.InvocationName
Write-Output "# Path:" $MyInvocation.MyCommand.Path
Write-Output "# Script:" $PSCommandPath
Write-Output "# Path:" $PSScriptRoot

# We need an Azure context to execute a deployment in Azure

$context = Get-AzContext

Try {
  bicep build $bicepFile
  if (Test-Path ./main.json) {
    Write-Output "ARM json created!"
  }
  if (!$context) {
    Write-Output "# No Azure context found! Please make sure azlogin has run before."
    exit
  }
  New-AzResourceGroup -Name $rg -Location $location -Force
  New-AzResourceGroupDeployment -TemplateFile ./main.json -ResourceGroupName $rg
}
Catch {
  $_.Exception.Message
  $_.Exception.ItemName
  Throw
}