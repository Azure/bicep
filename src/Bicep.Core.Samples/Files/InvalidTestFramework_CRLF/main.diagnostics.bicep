test sample 'samples/sample1.bicep' = {
  params: {
    location: 'westus'
  }

test sample 'samples/sample1.bicep'{
//@[05:11) [BCP018 (Error)] Expected the ":" character at this location. (CodeDescription: none) |sample|
//@[36:36) [BCP009 (Error)] Expected a literal value, an array, an object, a parenthesized expression, or a function call at this location. (CodeDescription: none) ||
    params: {
      location: 'westus'
    }
  }

test sample ={
//@[12:13) [BCP0347 (Error)] Expected a test path string at this location. (CodeDescription: none) |=|
//@[14:14) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||
    params: {
//@[04:10) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |params|
      location: 'westus'
//@[06:14) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |location|
    }
//@[04:05) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
  }
//@[02:03) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |}|

test sample 'samples/sample1.bicep'{
//@[35:36) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) |{|
    params: {
      location: 'westus',
//@[25:25) [BCP238 (Error)] Unexpected new line character after a comma. (CodeDescription: none) ||
    }
  }

test sample{
//@[11:12) [BCP0347 (Error)] Expected a test path string at this location. (CodeDescription: none) |{|
//@[12:12) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||
    params: {
//@[04:10) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |params|
      location: 'westus'
//@[06:14) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |location|
    }
//@[04:05) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
  }
//@[02:03) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |}|

test sample{
//@[11:12) [BCP0347 (Error)] Expected a test path string at this location. (CodeDescription: none) |{|
//@[12:12) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||
    params: {
//@[04:10) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |params|
      location: 'westus',
//@[06:14) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |location|
    },
//@[04:05) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
  }
//@[02:03) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |}|

test sample{
//@[11:12) [BCP0347 (Error)] Expected a test path string at this location. (CodeDescription: none) |{|
//@[12:12) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||
    params: {
//@[04:10) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |params|
      location: 'westus',
//@[06:14) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |location|
      env:'prod'
//@[06:09) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |env|
    },
//@[04:05) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
  }
//@[02:03) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |}|

test 'samples/sample1.bicep'{
//@[05:28) [BCP0346 (Error)] Expected a test identifier at this location. (CodeDescription: none) |'samples/sample1.bicep'|
//@[28:29) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) |{|
    params: {
      location: 'westus',
//@[25:25) [BCP238 (Error)] Unexpected new line character after a comma. (CodeDescription: none) ||
      env:'prod'
    },
//@[06:06) [BCP238 (Error)] Unexpected new line character after a comma. (CodeDescription: none) ||
  }

test
//@[04:04) [BCP0346 (Error)] Expected a test identifier at this location. (CodeDescription: none) ||

test sample
//@[11:11) [BCP0347 (Error)] Expected a test path string at this location. (CodeDescription: none) ||

test sample 'samples/sample1.bicep'
//@[35:35) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||

test sample 'samples/sample1.bicep' = 
//@[38:38) [BCP018 (Error)] Expected the "{" character at this location. (CodeDescription: none) ||

test sample 'samples/sample1.bicep' = {

test sample '' = {

//@[00:00) [BCP018 (Error)] Expected the "}" character at this location. (CodeDescription: none) ||
