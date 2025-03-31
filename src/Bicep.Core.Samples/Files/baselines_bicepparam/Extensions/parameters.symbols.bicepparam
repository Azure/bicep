using 'main.bicep'

extension k8s with {
//@[10:13) ExtensionConfigAssignment k8s. Type: configuration. Declaration start char: 0, length: 89
  kubeConfig: 'configFromParamFile'
  namespace: 'nsFromParamFile'
}

