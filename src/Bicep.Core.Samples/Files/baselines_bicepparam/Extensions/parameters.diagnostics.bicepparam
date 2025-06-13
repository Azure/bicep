using 'main.bicep'

extension k8s with {
  kubeConfig: az.getSecret('00000000-0000-0000-0000-000000000001', 'mock-rg', 'kv', 'myKubeConfig')
  namespace: 'nsFromParamFile'
}

