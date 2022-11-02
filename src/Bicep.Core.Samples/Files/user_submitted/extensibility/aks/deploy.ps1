$baseName="majastrzkub"
$adminUsername="marcin"
$sshRSAPublicKey = Get-Content "~\.ssh\id_rsa.pub";

New-AzSubscriptionDeployment `
  -Name $baseName `
  -Location 'West Central US' `
  -TemplateFile '.\main.bicep' `
  -TemplateParameterObject @{
    baseName = $baseName;
    dnsPrefix = $baseName;
    linuxAdminUsername = $adminUsername;
    sshRSAPublicKey = $sshRSAPublicKey;
  }