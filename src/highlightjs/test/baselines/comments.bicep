resource test 'Microsoft.AAD/domainServices@2021-03-01' = {
  name: 'asdfsdf'
  // this is a comment
  properties: {/*comment*/
    domainConfigurationType/*comment*/:/*comment*/'as//notacomment!d/* also not a comment */fsdf'// test!/*
    /* multi
    line
    comment */ domainName: /*
    asdf*/'test'
    // comment
  }
}
