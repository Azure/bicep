#disable-next-line
param storageAccount1 string = 'testStorageAccount'
//@[6:21) Parameter storageAccount1. Type: string. Declaration start char: 0, length: 51

#  disable-next-line  no-unused-params
param storageAccount2 string = 'testStorageAccount'
//@[6:21) Parameter storageAccount2. Type: string. Declaration start char: 0, length: 51

/* comment before */#disable-next-line no-unused-params
param storageAccount3 string = 'testStorageAccount'
//@[6:21) Parameter storageAccount3. Type: string. Declaration start char: 0, length: 51

#disable-next-line/* comment between */ no-unused-params
param storageAccount4 string = 'testStorageAccount'
//@[6:21) Parameter storageAccount4. Type: string. Declaration start char: 0, length: 51

//#disable-next-line no-unused-params
param storageAccount5 string = 'testStorageAccount'
//@[6:21) Parameter storageAccount5. Type: string. Declaration start char: 0, length: 51

#disable-next-line 
no-unused-params
param storageAccount6 string = 'testStorageAccount'
//@[6:21) Parameter storageAccount6. Type: string. Declaration start char: 0, length: 51

#madeup-directive
param storageAccount7 string = 'testStorageAccount'
//@[6:21) Parameter storageAccount7. Type: string. Declaration start char: 0, length: 51

var terminatedWithDirective = 'foo' #disable-next-line no-unused-params
//@[4:27) Variable terminatedWithDirective. Type: 'foo'. Declaration start char: 0, length: 35
param storageAccount8 string = 'testStorageAccount'
//@[6:21) Parameter storageAccount8. Type: string. Declaration start char: 0, length: 51
