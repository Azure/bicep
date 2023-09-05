#disable-next-line
//@[18:20) NewLine |\r\n|
param storageAccount1 string = 'testStorageAccount'
//@[00:05) Identifier |param|
//@[06:21) Identifier |storageAccount1|
//@[22:28) Identifier |string|
//@[29:30) Assignment |=|
//@[31:51) StringComplete |'testStorageAccount'|
//@[51:55) NewLine |\r\n\r\n|

#  disable-next-line  no-unused-params
//@[00:01) Unrecognized |#|
//@[03:10) Identifier |disable|
//@[10:11) Minus |-|
//@[11:15) Identifier |next|
//@[15:16) Minus |-|
//@[16:20) Identifier |line|
//@[22:24) Identifier |no|
//@[24:25) Minus |-|
//@[25:31) Identifier |unused|
//@[31:32) Minus |-|
//@[32:38) Identifier |params|
//@[38:40) NewLine |\r\n|
param storageAccount2 string = 'testStorageAccount'
//@[00:05) Identifier |param|
//@[06:21) Identifier |storageAccount2|
//@[22:28) Identifier |string|
//@[29:30) Assignment |=|
//@[31:51) StringComplete |'testStorageAccount'|
//@[51:55) NewLine |\r\n\r\n|

/* comment before */#disable-next-line no-unused-params
//@[20:21) Unrecognized |#|
//@[21:28) Identifier |disable|
//@[28:29) Minus |-|
//@[29:33) Identifier |next|
//@[33:34) Minus |-|
//@[34:38) Identifier |line|
//@[39:41) Identifier |no|
//@[41:42) Minus |-|
//@[42:48) Identifier |unused|
//@[48:49) Minus |-|
//@[49:55) Identifier |params|
//@[55:57) NewLine |\r\n|
param storageAccount3 string = 'testStorageAccount'
//@[00:05) Identifier |param|
//@[06:21) Identifier |storageAccount3|
//@[22:28) Identifier |string|
//@[29:30) Assignment |=|
//@[31:51) StringComplete |'testStorageAccount'|
//@[51:55) NewLine |\r\n\r\n|

#disable-next-line/* comment between */ no-unused-params
//@[40:42) Identifier |no|
//@[42:43) Minus |-|
//@[43:49) Identifier |unused|
//@[49:50) Minus |-|
//@[50:56) Identifier |params|
//@[56:58) NewLine |\r\n|
param storageAccount4 string = 'testStorageAccount'
//@[00:05) Identifier |param|
//@[06:21) Identifier |storageAccount4|
//@[22:28) Identifier |string|
//@[29:30) Assignment |=|
//@[31:51) StringComplete |'testStorageAccount'|
//@[51:55) NewLine |\r\n\r\n|

//#disable-next-line no-unused-params
//@[37:39) NewLine |\r\n|
param storageAccount5 string = 'testStorageAccount'
//@[00:05) Identifier |param|
//@[06:21) Identifier |storageAccount5|
//@[22:28) Identifier |string|
//@[29:30) Assignment |=|
//@[31:51) StringComplete |'testStorageAccount'|
//@[51:55) NewLine |\r\n\r\n|

#disable-next-line 
//@[19:21) NewLine |\r\n|
no-unused-params
//@[00:02) Identifier |no|
//@[02:03) Minus |-|
//@[03:09) Identifier |unused|
//@[09:10) Minus |-|
//@[10:16) Identifier |params|
//@[16:18) NewLine |\r\n|
param storageAccount6 string = 'testStorageAccount'
//@[00:05) Identifier |param|
//@[06:21) Identifier |storageAccount6|
//@[22:28) Identifier |string|
//@[29:30) Assignment |=|
//@[31:51) StringComplete |'testStorageAccount'|
//@[51:55) NewLine |\r\n\r\n|

#madeup-directive
//@[00:01) Unrecognized |#|
//@[01:07) Identifier |madeup|
//@[07:08) Minus |-|
//@[08:17) Identifier |directive|
//@[17:19) NewLine |\r\n|
param storageAccount7 string = 'testStorageAccount'
//@[00:05) Identifier |param|
//@[06:21) Identifier |storageAccount7|
//@[22:28) Identifier |string|
//@[29:30) Assignment |=|
//@[31:51) StringComplete |'testStorageAccount'|
//@[51:55) NewLine |\r\n\r\n|

var terminatedWithDirective = 'foo' #disable-next-line no-unused-params
//@[00:03) Identifier |var|
//@[04:27) Identifier |terminatedWithDirective|
//@[28:29) Assignment |=|
//@[30:35) StringComplete |'foo'|
//@[36:37) Unrecognized |#|
//@[37:44) Identifier |disable|
//@[44:45) Minus |-|
//@[45:49) Identifier |next|
//@[49:50) Minus |-|
//@[50:54) Identifier |line|
//@[55:57) Identifier |no|
//@[57:58) Minus |-|
//@[58:64) Identifier |unused|
//@[64:65) Minus |-|
//@[65:71) Identifier |params|
//@[71:73) NewLine |\r\n|
param storageAccount8 string = 'testStorageAccount'
//@[00:05) Identifier |param|
//@[06:21) Identifier |storageAccount8|
//@[22:28) Identifier |string|
//@[29:30) Assignment |=|
//@[31:51) StringComplete |'testStorageAccount'|
//@[51:51) EndOfFile ||
