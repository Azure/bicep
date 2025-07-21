extension 'br:mcr.microsoft.com/bicep/extensions/noconfig/v1:1.2.3' as hasNoConfig

extension 'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3' as validAssignment1

extension 'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3' as missingConfigAssignment1

extension 'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3' with { requiredString: 'defaultValue' } as incompleteAssignment1
extension 'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3' with { requiredString: 'defaultValue' } as incompleteAssignment2

extension 'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3' with { requiredString: 'defaultValue' } as invalidSyntax1
extension 'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3' with { requiredString: 'defaultValue' } as invalidSyntax2
extension 'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3' with { requiredString: 'defaultValue' } as invalidSyntax3
extension 'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3' with { requiredString: 'defaultValue' } as invalidSyntax4
extension 'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3' with { requiredString: 'defaultValue' } as invalidSyntax5

extension 'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3' with { requiredString: 'defaultValue' } as invalidAssignment1
extension 'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3' with { requiredString: 'defaultValue' } as invalidAssignment2

extension 'br:mcr.microsoft.com/bicep/extensions/hassecureconfig/v1:1.2.3' as invalidSecretAssignment1
extension 'br:mcr.microsoft.com/bicep/extensions/hassecureconfig/v1:1.2.3' as invalidSecretAssignment2
extension 'br:mcr.microsoft.com/bicep/extensions/hassecureconfig/v1:1.2.3' as invalidSecretAssignment3

extension 'br:mcr.microsoft.com/bicep/extensions/hasdiscrimconfig/v1:1.2.3' with { discrim: 'b' } as invalidDiscrimAssignment1

param strParam1 string
@secure()
param secureStrParam1 string
