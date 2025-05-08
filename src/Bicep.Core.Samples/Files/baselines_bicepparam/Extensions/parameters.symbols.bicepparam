using 'main.bicep'

extension k8s with {
//@[10:13) ExtensionConfigAssignment k8s. Type: configuration. Declaration start char: 0, length: 153
  kubeConfig: az.getSecret('00000000-0000-0000-0000-000000000001', 'mock-rg', 'kv', 'myKubeConfig')
  namespace: 'nsFromParamFile'
}

