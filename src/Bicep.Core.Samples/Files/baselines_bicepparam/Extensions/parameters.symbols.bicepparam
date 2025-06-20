using 'main.bicep'

extensionConfig k8s with {
//@[16:19) ExtensionConfigAssignment k8s. Type: configuration. Declaration start char: 0, length: 159
  kubeConfig: az.getSecret('00000000-0000-0000-0000-000000000001', 'mock-rg', 'kv', 'myKubeConfig')
  namespace: 'nsFromParamFile'
}

