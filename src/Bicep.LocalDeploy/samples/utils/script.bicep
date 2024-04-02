provider 'utils@0.0.1'

param name string
param platform 'bash' | 'powershell'

resource sayHelloWithBash 'Script' = if (platform == 'bash') {
  type: 'bash'
  script: replace(loadTextContent('./script.sh'), '$INPUT_NAME', name)
}

resource sayHelloWithPowerShell 'Script' = if (platform == 'powershell') {
  type: 'powershell'
  script: replace(loadTextContent('./script.ps1'), '$INPUT_NAME', name)
}

output stdout string = (platform == 'bash') ? sayHelloWithBash.stdout : sayHelloWithPowerShell.stdout
