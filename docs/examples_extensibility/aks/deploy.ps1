# In order to use this functionality, you must first have been enrolled in the extensibility preview.
# You will also need to ensure you've set the BICEP_IMPORTS_ENABLED_EXPERIMENTAL env var to 'true' in both VSCode and your CLI environment.

$baseName="majastrzkub"
$adminUsername="marcin"
$sshRSAPublicKey = Get-Content "~\.ssh\id_rsa.pub";

$env:BICEP_IMPORTS_ENABLED_EXPERIMENTAL = 'true'

Select-AzSubscription 996a2f3f-ee01-4ffd-9765-d2c3fc98f30a

# end-to-end deployment
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

# deploy kubernetes resources individually
$kubeConfig = ''

New-AzResourceGroupDeployment `
  -Name "$baseName-voteapp" `
  -TemplateFile '.\modules/kubernetes.bicep' `
  -ResourceGroupName $baseName `
  -TemplateParameterObject @{
    kubeConfig = $kubeConfig
  }