provider '$TARGET_REFERENCE'

resource dadJoke 'request@v1' = {
  uri: 'https://icanhazdadjoke.com'
  method: 'GET'
  format: 'json'
}

output joke string = dadJoke.body.joke
