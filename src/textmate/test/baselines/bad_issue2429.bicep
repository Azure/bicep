resource relay_hybrid_connection '...' = {
  // other content
  resource authorization_rules '...' = [for rule in someArray: {
    // other content
  }]
  // other content
}
