var loadedText1 = loadTextContent('Assets/TextFile.CRLF.txt')
//@[4:15) Variable loadedText1. Type: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit.\r\n\tProin varius in nunc et laoreet.\r\n  Nam pulvinar ipsum sed lectus porttitor, at porttitor ipsum faucibus.\r\n  \tAliquam euismod, odio tincidunt convallis pulvinar, felis sem porttitor turpis, a condimentum dui erat nec tellus.\r\n  Duis elementum cursus est, congue efficitur risus.\r\n\tMauris sit amet.\r\nExcepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\r\n'. Declaration start char: 0, length: 61
var loadedText2 = sys.loadTextContent('Assets/TextFile.LF.txt')
//@[4:15) Variable loadedText2. Type: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n Donec laoreet sem tortor, ut dignissim ipsum ornare vel.\n  Duis ac ipsum turpis.\n\tMaecenas at condimentum dui.\n Suspendisse aliquet efficitur iaculis.\nIn hac habitasse platea dictumst.\nEtiam consectetur ut libero ac lobortis.\n\tNullam vitae auctor massa.\nFusce tincidunt urna purus, sit amet.\n'. Declaration start char: 0, length: 63
var loadedTextEncoding1 = loadTextContent('Assets/encoding-ascii.txt', 'us-ascii')
//@[4:23) Variable loadedTextEncoding1. Type: '32 = \n33 = !\n34 = "\n35 = #\n36 = $\n37 = %\n38 = &\n39 = \'\n40 = (\n41 = )\n42 = *\n43 = +\n44 = ,\n45 = -\n46 = .\n47 = /\n48 = 0\n49 = 1\n50 = 2\n51 = 3\n52 = 4\n53 = 5\n54 = 6\n55 = 7\n56 = 8\n57 = 9\n58 = :\n59 = ;\n60 = <\n61 = =\n62 = >\n63 = ?\n64 = @\n65 = A\n66 = B\n67 = C\n68 = D\n69 = E\n70 = F\n71 = G\n72 = H\n73 = I\n74 = J\n75 = K\n76 = L\n77 = M\n78 = N\n79 = O\n80 = P\n81 = Q\n82 = R\n83 = S\n84 = T\n85 = U\n86 = V\n87 = W\n88 = X\n89 = Y\n90 = Z\n91 = [\n92 = \\\n93 = ]\n94 = ^\n95 = _\n96 = `\n97 = a\n98 = b\n99 = c\n100 = d\n101 = e\n102 = f\n103 = g\n104 = h\n105 = i\n106 = j\n107 = k\n108 = l\n109 = m\n110 = n\n111 = o\n112 = p\n113 = q\n114 = r\n115 = s\n116 = t\n117 = u\n118 = v\n119 = w\n120 = x\n121 = y\n122 = z\n123 = {\n124 = |\n125 = }\n126 = ~\n'. Declaration start char: 0, length: 82
var loadedTextEncoding2 = loadTextContent('Assets/encoding-utf8.txt', 'utf-8')
//@[4:23) Variable loadedTextEncoding2. Type: '💪😊😈🍕☕\r\n🐱‍👤\r\n\r\n朝辞白帝彩云间\r\n千里江陵一日还\r\n两岸猿声啼不住\r\n轻舟已过万重山\r\n\r\nΠ π Φ φ\r\n\r\n😎\r\n\r\nαα\r\nΩω\r\nΘ  \r\n\r\nZażółć gęślą jaźń\r\n\r\náéóúñü - ¡Hola!\r\n\r\n二头肌二头肌\r\n\r\n\r\n二头肌\r\nΘ二头肌α\r\n\r\n𐐷\r\n\\u{10437}\r\n\\u{D801}\\u{DC37}\r\n\r\n❆ Hello\\u{20}World\\u{21} ❁\r\n\r\n\ta\tb\tc\td\te\tf\tg\th\t\r\n8\t♜\t♞\t♝\t♛\t♚\t♝\t♞\t♜\t8\r\n7\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t7\r\n6\t\t\t\t\t\t\t\t\t6\r\n5\t\t\t\t\t\t\t\t\t5\r\n4\t\t\t\t\t\t\t\t\t4\r\n3\t\t\t\t\t\t\t\t\t3\r\n2\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t2\r\n1\t♖\t♘\t♗\t♕\t♔\t♗\t♘\t♖\t1\r\n\ta\tb\tc\td\te\tf\tg\th\r\n'. Declaration start char: 0, length: 78
var loadedTextEncoding3 = loadTextContent('Assets/encoding-utf16.txt', 'utf-16')
//@[4:23) Variable loadedTextEncoding3. Type: '💪😊😈🍕☕\r\n🐱‍👤\r\n\r\n朝辞白帝彩云间\r\n千里江陵一日还\r\n两岸猿声啼不住\r\n轻舟已过万重山\r\n\r\nΠ π Φ φ\r\n\r\n😎\r\n\r\nαα\r\nΩω\r\nΘ  \r\n\r\nZażółć gęślą jaźń\r\n\r\náéóúñü - ¡Hola!\r\n\r\n二头肌二头肌\r\n\r\n\r\n二头肌\r\nΘ二头肌α\r\n\r\n𐐷\r\n\\u{10437}\r\n\\u{D801}\\u{DC37}\r\n\r\n❆ Hello\\u{20}World\\u{21} ❁\r\n\r\n\ta\tb\tc\td\te\tf\tg\th\t\r\n8\t♜\t♞\t♝\t♛\t♚\t♝\t♞\t♜\t8\r\n7\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t7\r\n6\t\t\t\t\t\t\t\t\t6\r\n5\t\t\t\t\t\t\t\t\t5\r\n4\t\t\t\t\t\t\t\t\t4\r\n3\t\t\t\t\t\t\t\t\t3\r\n2\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t2\r\n1\t♖\t♘\t♗\t♕\t♔\t♗\t♘\t♖\t1\r\n\ta\tb\tc\td\te\tf\tg\th\r\n'. Declaration start char: 0, length: 80
var loadedTextEncoding4 = loadTextContent('Assets/encoding-utf16be.txt', 'utf-16BE')
//@[4:23) Variable loadedTextEncoding4. Type: '💪😊😈🍕☕\r\n🐱‍👤\r\n\r\n朝辞白帝彩云间\r\n千里江陵一日还\r\n两岸猿声啼不住\r\n轻舟已过万重山\r\n\r\nΠ π Φ φ\r\n\r\n😎\r\n\r\nαα\r\nΩω\r\nΘ  \r\n\r\nZażółć gęślą jaźń\r\n\r\náéóúñü - ¡Hola!\r\n\r\n二头肌二头肌\r\n\r\n\r\n二头肌\r\nΘ二头肌α\r\n\r\n𐐷\r\n\\u{10437}\r\n\\u{D801}\\u{DC37}\r\n\r\n❆ Hello\\u{20}World\\u{21} ❁\r\n\r\n\ta\tb\tc\td\te\tf\tg\th\t\r\n8\t♜\t♞\t♝\t♛\t♚\t♝\t♞\t♜\t8\r\n7\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t7\r\n6\t\t\t\t\t\t\t\t\t6\r\n5\t\t\t\t\t\t\t\t\t5\r\n4\t\t\t\t\t\t\t\t\t4\r\n3\t\t\t\t\t\t\t\t\t3\r\n2\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t2\r\n1\t♖\t♘\t♗\t♕\t♔\t♗\t♘\t♖\t1\r\n\ta\tb\tc\td\te\tf\tg\th\r\n'. Declaration start char: 0, length: 84
var loadedTextEncoding5 = loadTextContent('Assets/encoding-iso.txt', 'iso-8859-1')
//@[4:23) Variable loadedTextEncoding5. Type: '¡\t\tinverted exclamation mark\n¢\t\tcent\n£\t\tpound\n¤\t\tcurrency\n¥\t\tyen\n¦\t\tbroken vertical bar\n§\t\tsection\n¨\t\tspacing diaeresis\n©\t\tcopyright\nª\t\tfeminine ordinal indicator\n«\t\tangle quotation mark (left)\n¬\t\tnegation\n­\t\tsoft hyphen\n®\t\tregistered trademark\n¯\t\tspacing macron\n°\t\tdegree\n±\t\tplus-or-minus\n²\t\tsuperscript 2\n³\t\tsuperscript 3\n´\t\tspacing acute\n¶\t\tparagraph\n·\t\tmiddle dot\n¸\t\tspacing cedilla\n¹\t\tsuperscript 1\nº\t\tmasculine ordinal indicator\n»\t\tangle quotation mark (right)\n¼\t\tfraction 1/4\n½\t\tfraction 1/2\n¾\t\tfraction 3/4\n¿\t\tinverted question mark\nÀ\t\tcapital a, grave accent\nÁ\t\tcapital a, acute accent\nÂ\t\tcapital a, circumflex accent\nÃ\t\tcapital a, tilde\nÄ\t\tcapital a, umlaut mark\nÅ\t\tcapital a, ring\nÆ\t\tcapital ae\nÇ\t\tcapital c, cedilla\nÈ\t\tcapital e, grave accent\nÉ\t\tcapital e, acute accent\nÊ\t\tcapital e, circumflex accent\nË\t\tcapital e, umlaut mark\nÌ\t\tcapital i, grave accent\nÍ\t\tcapital i, acute accent\nÎ\t\tcapital i, circumflex accent\nÏ\t\tcapital i, umlaut mark\nÐ\t\tcapital eth, Icelandic\nÑ\t\tcapital n, tilde\nÒ\t\tcapital o, grave accent\nÓ\t\tcapital o, acute accent\nÔ\t\tcapital o, circumflex accent\nÕ\t\tcapital o, tilde\nÖ\t\tcapital o, umlaut mark\n×\t\tmultiplication\nØ\t\tcapital o, slash\nÙ\t\tcapital u, grave accent\nÚ\t\tcapital u, acute accent\nÛ\t\tcapital u, circumflex accent\nÜ\t\tcapital u, umlaut mark\nÝ\t\tcapital y, acute accent\nÞ\t\tcapital THORN, Icelandic\nß\t\tsmall sharp s, German\nà\t\tsmall a, grave accent\ná\t\tsmall a, acute accent\nâ\t\tsmall a, circumflex accent\nã\t\tsmall a, tilde\nä\t\tsmall a, umlaut mark\nå\t\tsmall a, ring\næ\t\tsmall ae\nç\t\tsmall c, cedilla\nè\t\tsmall e, grave accent\né\t\tsmall e, acute accent\nê\t\tsmall e, circumflex accent\në\t\tsmall e, umlaut mark\nì\t\tsmall i, grave accent\ní\t\tsmall i, acute accent\nî\t\tsmall i, circumflex accent\nï\t\tsmall i, umlaut mark\nð\t\tsmall eth, Icelandic\nñ\t\tsmall n, tilde\nò\t\tsmall o, grave accent\nó\t\tsmall o, acute accent\nô\t\tsmall o, circumflex accent\nõ\t\tsmall o, tilde\nö\t\tsmall o, umlaut mark\n÷\t\tdivision\nø\t\tsmall o, slash\nù\t\tsmall u, grave accent\nú\t\tsmall u, acute accent\nû\t\tsmall u, circumflex accent\nü\t\tsmall u, umlaut mark\ný\t\tsmall y, acute accent\nþ\t\tsmall thorn, Icelandic\nÿ\t\tsmall y, umlaut mark\n'. Declaration start char: 0, length: 82

var loadedBinary1 = loadFileAsBase64('Assets/binary')
//@[4:17) Variable loadedBinary1. Type: Assets/binary. Declaration start char: 0, length: 53
var loadedBinary2 = sys.loadFileAsBase64('Assets/binary')
//@[4:17) Variable loadedBinary2. Type: Assets/binary. Declaration start char: 0, length: 57

var loadedTextInterpolation1 = 'Text: ${loadTextContent('Assets/TextFile.CRLF.txt')}'
//@[4:28) Variable loadedTextInterpolation1. Type: string. Declaration start char: 0, length: 85
var loadedTextInterpolation2 = 'Text: ${loadTextContent('Assets/TextFile.LF.txt')}'
//@[4:28) Variable loadedTextInterpolation2. Type: string. Declaration start char: 0, length: 83

var loadedTextObject1 = {
//@[4:21) Variable loadedTextObject1. Type: object. Declaration start char: 0, length: 84
  'text' : loadTextContent('Assets/TextFile.CRLF.txt')
}
var loadedTextObject2 = {
//@[4:21) Variable loadedTextObject2. Type: object. Declaration start char: 0, length: 84
  'text' : loadTextContent('Assets/TextFile.LF.txt')  
}
var loadedBinaryInObject = {
//@[4:24) Variable loadedBinaryInObject. Type: object. Declaration start char: 0, length: 74
  file: loadFileAsBase64('Assets/binary')
}

var loadedTextArray = [
//@[4:19) Variable loadedTextArray. Type: ['Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n Donec laoreet sem tortor, ut dignissim ipsum ornare vel.\n  Duis ac ipsum turpis.\n\tMaecenas at condimentum dui.\n Suspendisse aliquet efficitur iaculis.\nIn hac habitasse platea dictumst.\nEtiam consectetur ut libero ac lobortis.\n\tNullam vitae auctor massa.\nFusce tincidunt urna purus, sit amet.\n', Assets/binary]. Declaration start char: 0, length: 108
  loadTextContent('Assets/TextFile.LF.txt')
  loadFileAsBase64('Assets/binary')
]

var loadedTextArrayInObject = {
//@[4:27) Variable loadedTextArrayInObject. Type: object. Declaration start char: 0, length: 142
  'files' : [
    loadTextContent('Assets/TextFile.CRLF.txt')
    loadFileAsBase64('Assets/binary')
  ]
}

var loadedTextArrayInObjectFunctions = {
//@[4:36) Variable loadedTextArrayInObjectFunctions. Type: object. Declaration start char: 0, length: 277
  'files' : [
    length(loadTextContent('Assets/TextFile.CRLF.txt'))
    sys.length(loadTextContent('Assets/TextFile.LF.txt'))
    length(loadFileAsBase64('Assets/binary'))
    sys.length(loadFileAsBase64('Assets/binary'))
  ]
}


module module1 'modulea.bicep' = {
//@[7:14) Module module1. Type: module. Declaration start char: 0, length: 127
  name: 'module1'
  params: {
    text: loadTextContent('Assets/TextFile.LF.txt')
  }
}

module module2 'modulea.bicep' = {
//@[7:14) Module module2. Type: module. Declaration start char: 0, length: 119
  name: 'module2'
  params: {
    text: loadFileAsBase64('Assets/binary')
  }
}

var textFileInSubdirectories = loadTextContent('Assets/../Assets/path/../path/../../Assets/path/to/deep/file/../../../to/deep/file/TextFile.txt')
//@[4:28) Variable textFileInSubdirectories. Type: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n Donec laoreet sem tortor, ut dignissim ipsum ornare vel.\n  Duis ac ipsum turpis.\n\tMaecenas at condimentum dui.\n Suspendisse aliquet efficitur iaculis.\nIn hac habitasse platea dictumst.\nEtiam consectetur ut libero ac lobortis.\n\tNullam vitae auctor massa.\nFusce tincidunt urna purus, sit amet.\n'. Declaration start char: 0, length: 145
var binaryFileInSubdirectories = loadFileAsBase64('Assets/../Assets/path/../path/../../Assets/path/to/deep/file/../../../to/deep/file/binary')
//@[4:30) Variable binaryFileInSubdirectories. Type: Assets/path/to/deep/file/binary. Declaration start char: 0, length: 142

var loadWithEncoding01 = loadTextContent('Assets/encoding-iso.txt', 'iso-8859-1')
//@[4:22) Variable loadWithEncoding01. Type: '¡\t\tinverted exclamation mark\n¢\t\tcent\n£\t\tpound\n¤\t\tcurrency\n¥\t\tyen\n¦\t\tbroken vertical bar\n§\t\tsection\n¨\t\tspacing diaeresis\n©\t\tcopyright\nª\t\tfeminine ordinal indicator\n«\t\tangle quotation mark (left)\n¬\t\tnegation\n­\t\tsoft hyphen\n®\t\tregistered trademark\n¯\t\tspacing macron\n°\t\tdegree\n±\t\tplus-or-minus\n²\t\tsuperscript 2\n³\t\tsuperscript 3\n´\t\tspacing acute\n¶\t\tparagraph\n·\t\tmiddle dot\n¸\t\tspacing cedilla\n¹\t\tsuperscript 1\nº\t\tmasculine ordinal indicator\n»\t\tangle quotation mark (right)\n¼\t\tfraction 1/4\n½\t\tfraction 1/2\n¾\t\tfraction 3/4\n¿\t\tinverted question mark\nÀ\t\tcapital a, grave accent\nÁ\t\tcapital a, acute accent\nÂ\t\tcapital a, circumflex accent\nÃ\t\tcapital a, tilde\nÄ\t\tcapital a, umlaut mark\nÅ\t\tcapital a, ring\nÆ\t\tcapital ae\nÇ\t\tcapital c, cedilla\nÈ\t\tcapital e, grave accent\nÉ\t\tcapital e, acute accent\nÊ\t\tcapital e, circumflex accent\nË\t\tcapital e, umlaut mark\nÌ\t\tcapital i, grave accent\nÍ\t\tcapital i, acute accent\nÎ\t\tcapital i, circumflex accent\nÏ\t\tcapital i, umlaut mark\nÐ\t\tcapital eth, Icelandic\nÑ\t\tcapital n, tilde\nÒ\t\tcapital o, grave accent\nÓ\t\tcapital o, acute accent\nÔ\t\tcapital o, circumflex accent\nÕ\t\tcapital o, tilde\nÖ\t\tcapital o, umlaut mark\n×\t\tmultiplication\nØ\t\tcapital o, slash\nÙ\t\tcapital u, grave accent\nÚ\t\tcapital u, acute accent\nÛ\t\tcapital u, circumflex accent\nÜ\t\tcapital u, umlaut mark\nÝ\t\tcapital y, acute accent\nÞ\t\tcapital THORN, Icelandic\nß\t\tsmall sharp s, German\nà\t\tsmall a, grave accent\ná\t\tsmall a, acute accent\nâ\t\tsmall a, circumflex accent\nã\t\tsmall a, tilde\nä\t\tsmall a, umlaut mark\nå\t\tsmall a, ring\næ\t\tsmall ae\nç\t\tsmall c, cedilla\nè\t\tsmall e, grave accent\né\t\tsmall e, acute accent\nê\t\tsmall e, circumflex accent\në\t\tsmall e, umlaut mark\nì\t\tsmall i, grave accent\ní\t\tsmall i, acute accent\nî\t\tsmall i, circumflex accent\nï\t\tsmall i, umlaut mark\nð\t\tsmall eth, Icelandic\nñ\t\tsmall n, tilde\nò\t\tsmall o, grave accent\nó\t\tsmall o, acute accent\nô\t\tsmall o, circumflex accent\nõ\t\tsmall o, tilde\nö\t\tsmall o, umlaut mark\n÷\t\tdivision\nø\t\tsmall o, slash\nù\t\tsmall u, grave accent\nú\t\tsmall u, acute accent\nû\t\tsmall u, circumflex accent\nü\t\tsmall u, umlaut mark\ný\t\tsmall y, acute accent\nþ\t\tsmall thorn, Icelandic\nÿ\t\tsmall y, umlaut mark\n'. Declaration start char: 0, length: 81
var loadWithEncoding06 = loadTextContent('Assets/encoding-ascii.txt', 'us-ascii')
//@[4:22) Variable loadWithEncoding06. Type: '32 = \n33 = !\n34 = "\n35 = #\n36 = $\n37 = %\n38 = &\n39 = \'\n40 = (\n41 = )\n42 = *\n43 = +\n44 = ,\n45 = -\n46 = .\n47 = /\n48 = 0\n49 = 1\n50 = 2\n51 = 3\n52 = 4\n53 = 5\n54 = 6\n55 = 7\n56 = 8\n57 = 9\n58 = :\n59 = ;\n60 = <\n61 = =\n62 = >\n63 = ?\n64 = @\n65 = A\n66 = B\n67 = C\n68 = D\n69 = E\n70 = F\n71 = G\n72 = H\n73 = I\n74 = J\n75 = K\n76 = L\n77 = M\n78 = N\n79 = O\n80 = P\n81 = Q\n82 = R\n83 = S\n84 = T\n85 = U\n86 = V\n87 = W\n88 = X\n89 = Y\n90 = Z\n91 = [\n92 = \\\n93 = ]\n94 = ^\n95 = _\n96 = `\n97 = a\n98 = b\n99 = c\n100 = d\n101 = e\n102 = f\n103 = g\n104 = h\n105 = i\n106 = j\n107 = k\n108 = l\n109 = m\n110 = n\n111 = o\n112 = p\n113 = q\n114 = r\n115 = s\n116 = t\n117 = u\n118 = v\n119 = w\n120 = x\n121 = y\n122 = z\n123 = {\n124 = |\n125 = }\n126 = ~\n'. Declaration start char: 0, length: 81
var loadWithEncoding07 = loadTextContent('Assets/encoding-ascii.txt', 'iso-8859-1')
//@[4:22) Variable loadWithEncoding07. Type: '32 = \n33 = !\n34 = "\n35 = #\n36 = $\n37 = %\n38 = &\n39 = \'\n40 = (\n41 = )\n42 = *\n43 = +\n44 = ,\n45 = -\n46 = .\n47 = /\n48 = 0\n49 = 1\n50 = 2\n51 = 3\n52 = 4\n53 = 5\n54 = 6\n55 = 7\n56 = 8\n57 = 9\n58 = :\n59 = ;\n60 = <\n61 = =\n62 = >\n63 = ?\n64 = @\n65 = A\n66 = B\n67 = C\n68 = D\n69 = E\n70 = F\n71 = G\n72 = H\n73 = I\n74 = J\n75 = K\n76 = L\n77 = M\n78 = N\n79 = O\n80 = P\n81 = Q\n82 = R\n83 = S\n84 = T\n85 = U\n86 = V\n87 = W\n88 = X\n89 = Y\n90 = Z\n91 = [\n92 = \\\n93 = ]\n94 = ^\n95 = _\n96 = `\n97 = a\n98 = b\n99 = c\n100 = d\n101 = e\n102 = f\n103 = g\n104 = h\n105 = i\n106 = j\n107 = k\n108 = l\n109 = m\n110 = n\n111 = o\n112 = p\n113 = q\n114 = r\n115 = s\n116 = t\n117 = u\n118 = v\n119 = w\n120 = x\n121 = y\n122 = z\n123 = {\n124 = |\n125 = }\n126 = ~\n'. Declaration start char: 0, length: 83
var loadWithEncoding08 = loadTextContent('Assets/encoding-ascii.txt', 'utf-8')
//@[4:22) Variable loadWithEncoding08. Type: '32 = \n33 = !\n34 = "\n35 = #\n36 = $\n37 = %\n38 = &\n39 = \'\n40 = (\n41 = )\n42 = *\n43 = +\n44 = ,\n45 = -\n46 = .\n47 = /\n48 = 0\n49 = 1\n50 = 2\n51 = 3\n52 = 4\n53 = 5\n54 = 6\n55 = 7\n56 = 8\n57 = 9\n58 = :\n59 = ;\n60 = <\n61 = =\n62 = >\n63 = ?\n64 = @\n65 = A\n66 = B\n67 = C\n68 = D\n69 = E\n70 = F\n71 = G\n72 = H\n73 = I\n74 = J\n75 = K\n76 = L\n77 = M\n78 = N\n79 = O\n80 = P\n81 = Q\n82 = R\n83 = S\n84 = T\n85 = U\n86 = V\n87 = W\n88 = X\n89 = Y\n90 = Z\n91 = [\n92 = \\\n93 = ]\n94 = ^\n95 = _\n96 = `\n97 = a\n98 = b\n99 = c\n100 = d\n101 = e\n102 = f\n103 = g\n104 = h\n105 = i\n106 = j\n107 = k\n108 = l\n109 = m\n110 = n\n111 = o\n112 = p\n113 = q\n114 = r\n115 = s\n116 = t\n117 = u\n118 = v\n119 = w\n120 = x\n121 = y\n122 = z\n123 = {\n124 = |\n125 = }\n126 = ~\n'. Declaration start char: 0, length: 78
var loadWithEncoding11 = loadTextContent('Assets/encoding-utf8.txt', 'utf-8')
//@[4:22) Variable loadWithEncoding11. Type: '💪😊😈🍕☕\r\n🐱‍👤\r\n\r\n朝辞白帝彩云间\r\n千里江陵一日还\r\n两岸猿声啼不住\r\n轻舟已过万重山\r\n\r\nΠ π Φ φ\r\n\r\n😎\r\n\r\nαα\r\nΩω\r\nΘ  \r\n\r\nZażółć gęślą jaźń\r\n\r\náéóúñü - ¡Hola!\r\n\r\n二头肌二头肌\r\n\r\n\r\n二头肌\r\nΘ二头肌α\r\n\r\n𐐷\r\n\\u{10437}\r\n\\u{D801}\\u{DC37}\r\n\r\n❆ Hello\\u{20}World\\u{21} ❁\r\n\r\n\ta\tb\tc\td\te\tf\tg\th\t\r\n8\t♜\t♞\t♝\t♛\t♚\t♝\t♞\t♜\t8\r\n7\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t7\r\n6\t\t\t\t\t\t\t\t\t6\r\n5\t\t\t\t\t\t\t\t\t5\r\n4\t\t\t\t\t\t\t\t\t4\r\n3\t\t\t\t\t\t\t\t\t3\r\n2\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t2\r\n1\t♖\t♘\t♗\t♕\t♔\t♗\t♘\t♖\t1\r\n\ta\tb\tc\td\te\tf\tg\th\r\n'. Declaration start char: 0, length: 77
var loadWithEncoding12 = loadTextContent('Assets/encoding-utf8-bom.txt', 'utf-8')
//@[4:22) Variable loadWithEncoding12. Type: '💪😊😈🍕☕\r\n🐱‍👤\r\n\r\n朝辞白帝彩云间\r\n千里江陵一日还\r\n两岸猿声啼不住\r\n轻舟已过万重山\r\n\r\nΠ π Φ φ\r\n\r\n😎\r\n\r\nαα\r\nΩω\r\nΘ  \r\n\r\nZażółć gęślą jaźń\r\n\r\náéóúñü - ¡Hola!\r\n\r\n二头肌二头肌\r\n\r\n\r\n二头肌\r\nΘ二头肌α\r\n\r\n𐐷\r\n\\u{10437}\r\n\\u{D801}\\u{DC37}\r\n\r\n❆ Hello\\u{20}World\\u{21} ❁\r\n\r\n\ta\tb\tc\td\te\tf\tg\th\t\r\n8\t♜\t♞\t♝\t♛\t♚\t♝\t♞\t♜\t8\r\n7\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t7\r\n6\t\t\t\t\t\t\t\t\t6\r\n5\t\t\t\t\t\t\t\t\t5\r\n4\t\t\t\t\t\t\t\t\t4\r\n3\t\t\t\t\t\t\t\t\t3\r\n2\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t2\r\n1\t♖\t♘\t♗\t♕\t♔\t♗\t♘\t♖\t1\r\n\ta\tb\tc\td\te\tf\tg\th\r\n'. Declaration start char: 0, length: 81

var testJson = json(loadTextContent('./Assets/test.json.txt'))
//@[4:12) Variable testJson. Type: object. Declaration start char: 0, length: 62
var testJsonString = testJson.string
//@[4:18) Variable testJsonString. Type: 'someVal'. Declaration start char: 0, length: 36
var testJsonInt = testJson.int
//@[4:15) Variable testJsonInt. Type: int. Declaration start char: 0, length: 30
var testJsonArrayVal = testJson.array[0]
//@[4:20) Variable testJsonArrayVal. Type: int. Declaration start char: 0, length: 40
var testJsonObject = testJson.object
//@[4:18) Variable testJsonObject. Type: object. Declaration start char: 0, length: 36
var testJsonNestedString = testJson.object.nestedString
//@[4:24) Variable testJsonNestedString. Type: 'someVal'. Declaration start char: 0, length: 55

var testJson2 = loadJsonContent('./Assets/test.json.txt')
//@[4:13) Variable testJson2. Type: object. Declaration start char: 0, length: 57
var testJsonString2 = testJson.string
//@[4:19) Variable testJsonString2. Type: 'someVal'. Declaration start char: 0, length: 37
var testJsonString2_1 = loadJsonContent('./Assets/test.json.txt', '.string')
//@[4:21) Variable testJsonString2_1. Type: 'someVal'. Declaration start char: 0, length: 76
var testJsonInt2 = testJson.int
//@[4:16) Variable testJsonInt2. Type: int. Declaration start char: 0, length: 31
var testJsonInt2_1 = loadJsonContent('./Assets/test.json.txt', '.int')
//@[4:18) Variable testJsonInt2_1. Type: int. Declaration start char: 0, length: 70
var testJsonArrayVal2 = testJson.array[0]
//@[4:21) Variable testJsonArrayVal2. Type: int. Declaration start char: 0, length: 41
var testJsonArrayVal2_1 = loadJsonContent('./Assets/test.json.txt', '.array[0]')
//@[4:23) Variable testJsonArrayVal2_1. Type: int. Declaration start char: 0, length: 80
var testJsonObject2 = testJson.object
//@[4:19) Variable testJsonObject2. Type: object. Declaration start char: 0, length: 37
var testJsonObject2_1 = loadJsonContent('./Assets/test.json.txt', '.object')
//@[4:21) Variable testJsonObject2_1. Type: object. Declaration start char: 0, length: 76
var testJsonNestedString2 = testJson.object.nestedString
//@[4:25) Variable testJsonNestedString2. Type: 'someVal'. Declaration start char: 0, length: 56
var testJsonNestedString2_1 = testJsonObject2_1.nestedString
//@[4:27) Variable testJsonNestedString2_1. Type: 'someVal'. Declaration start char: 0, length: 60
var testJsonNestedString2_2 = loadJsonContent('./Assets/test.json.txt', '.object.nestedString')
//@[4:27) Variable testJsonNestedString2_2. Type: 'someVal'. Declaration start char: 0, length: 95

var testJsonTokensAsArray = loadJsonContent('./Assets/test2.json.txt', '.products[?(@.price > 3)].name')
//@[4:25) Variable testJsonTokensAsArray. Type: ('pizza' | 'salad')[]. Declaration start char: 0, length: 104

