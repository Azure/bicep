# 

## Details

The quick brown fox jumps over the lazy dog. The quick brown fox jumps over the lazy dog.
The quick brown fox jumps over the lazy dog. The quick brown fox jumps over the lazy dog. The quick brown fox jumps over the lazy dog.
The quick brown fox jumps over the lazy dog.

## Parameters

| Name | Type | Required | Description |
| :--- | :--: | :------: | :---------- |

## Outputs

| Name | Type | Description |
| :--- | :--: | :---------- |

## Examples

### Example 1

```bicep
module mod 'br/public:test/testmodule:1.1.1' = {
  name: 'mod'
  params: {
    dnsPrefix: ''
    linuxAdminUsername: ''
    sshRSAPublicKey: ''
    servicePrincipalClientId: ''
    servicePrincipalClientSecret: ''
    osDiskSizeGB: 1
  }
}
```