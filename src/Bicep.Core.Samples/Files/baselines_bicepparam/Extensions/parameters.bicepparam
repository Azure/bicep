using 'main.bicep'

extensionConfig hasObjConfig1 with {
  requiredString: 'valueFromParams'
}

extensionConfig hasObjConfig2 with {
  optionalString: 'optional'
}

extensionConfig hasObjConfig3 with {}

// hasObjConfig4 not here to test assignment is not required because required field is defaulted

extensionConfig hasSecureConfig1 with {
  requiredSecureString: az.getSecret('00000000-0000-0000-0000-000000000001', 'mock-rg', 'kv', 'mySecret')
}

extensionConfig hasSecureConfig2 with {
  requiredSecureString: 'Inlined'
  optionalString: 'valueFromParams'
}

extensionConfig hasDiscrimConfig1 with {
  discrim: 'a'
  z1: 'z1v'
  a1: 'a1v'
}

extensionConfig hasDiscrimConfig2 with {
  a1: 'a1v'
}

extensionConfig hasDiscrimConfig3 with {
  discrim: 'b'
  b1: 'b1v'
}
