extension 'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3' as hasObjConfig1
extension 'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3' with { requiredString: 'defaultValue' } as hasObjConfig2
extension 'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3' with { requiredString: 'defaultValue' } as hasObjConfig3
extension 'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3' with { requiredString: 'defaultValue' } as hasObjConfig4
extension 'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3' as hasObjConfig5

extension 'br:mcr.microsoft.com/bicep/extensions/hassecureconfig/v1:1.2.3' as hasSecureConfig1
extension 'br:mcr.microsoft.com/bicep/extensions/hassecureconfig/v1:1.2.3' with { optionalString: 'defaultValue' } as hasSecureConfig2

extension 'br:mcr.microsoft.com/bicep/extensions/hasdiscrimconfig/v1:1.2.3' as hasDiscrimConfig1
extension 'br:mcr.microsoft.com/bicep/extensions/hasdiscrimconfig/v1:1.2.3' with { discrim: 'a' } as hasDiscrimConfig2
extension 'br:mcr.microsoft.com/bicep/extensions/hasdiscrimconfig/v1:1.2.3' as hasDiscrimConfig3

param strParam1 string
@secure()
param secureStrParam1 string
