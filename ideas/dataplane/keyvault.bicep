input namePrefix string

// mapped to https://docs.microsoft.com/en-us/rest/api/keyvault/createcertificate/createcertificate
// and https://docs.microsoft.com/en-us/rest/api/keyvault/getcertificate/getcertificate
// we also need to take care of async operation tracking

resource keyvault 'certificates@7.0' myCert {
  name: 'selfSignedCert01'
  data: {
    policy: {
      key_props: {
        exportable: true
        kty: 'RSA'
        key_size: 2048
        reuse_key: false
      }
      secret_props: {
        contentType: 'application/x-pkcs12'
      }
      x509_props: {
        subject: 'CN=*.microsoft.com'
        sans: {
          dns_names: [
            'onedrive.microsoft.com'
            'xbox.microsoft.com'
          ]
        }
      }
      issuer: {
        name: 'Self'
      }
    }
  }
}

output certThumbprint: myCert.x5t