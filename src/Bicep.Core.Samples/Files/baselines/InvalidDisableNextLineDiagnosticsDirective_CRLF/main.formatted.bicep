#disable-next-line 
param storageAccount1 string = 'testStorageAccount'

#  disable-next-line  no-unused-params
param storageAccount2 string = 'testStorageAccount'

/* comment before */#disable-next-line no-unused-params
param storageAccount3 string = 'testStorageAccount'

#disable-next-line/* comment between */ no-unused-params
param storageAccount4 string = 'testStorageAccount'

//#disable-next-line no-unused-params
param storageAccount5 string = 'testStorageAccount'

#disable-next-line 
no-unused-params
param storageAccount6 string = 'testStorageAccount'

#madeup-directive
param storageAccount7 string = 'testStorageAccount'

var terminatedWithDirective = 'foo'
#disable-next-line no-unused-params
param storageAccount8 string = 'testStorageAccount'
