using 'main.bicep'

extension k8s with {
  kubeConfig: 'configFromParamFile'
  namespace: 'nsFromParamFile'
}

