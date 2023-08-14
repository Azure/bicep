test sample 'samples/sample1.bicep' = {
//@[5:11) Test sample. Type: module. Declaration start char: 0, length: 174
  params: {
    location: 'westus'
  }

test sample 'samples/sample1.bicep'{
    params: {
      location: 'westus'
    }
  }

test sample ={
//@[5:11) Test sample. Type: error. Declaration start char: 0, length: 14
    params: {
      location: 'westus'
    }
  }

test sample 'samples/sample1.bicep'{
//@[5:11) Test sample. Type: module. Declaration start char: 0, length: 90
    params: {
      location: 'westus',
    }
  }

test sample{
//@[5:11) Test sample. Type: error. Declaration start char: 0, length: 12
    params: {
      location: 'westus'
    }
  }

test sample{
//@[5:11) Test sample. Type: error. Declaration start char: 0, length: 12
    params: {
      location: 'westus',
    },
  }

test sample{
//@[5:11) Test sample. Type: error. Declaration start char: 0, length: 12
    params: {
      location: 'westus',
      env:'prod'
    },
  }

test 'samples/sample1.bicep'{
//@[5:05) Test <missing>. Type: module. Declaration start char: 0, length: 102
    params: {
      location: 'westus',
      env:'prod'
    },
  }

test
//@[4:04) Test <missing>. Type: error. Declaration start char: 0, length: 4

test sample
//@[5:11) Test sample. Type: error. Declaration start char: 0, length: 11

test sample 'samples/sample1.bicep'
//@[5:11) Test sample. Type: module. Declaration start char: 0, length: 35

test sample 'samples/sample1.bicep' = 
//@[5:11) Test sample. Type: module. Declaration start char: 0, length: 38

test sample 'samples/sample1.bicep' = {
//@[5:11) Test sample. Type: module. Declaration start char: 0, length: 63

test sample '' = {

