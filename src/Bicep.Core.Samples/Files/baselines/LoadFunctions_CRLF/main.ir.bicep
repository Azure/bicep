var loadedText1 = loadTextContent('Assets/TextFile.CRLF.txt')
//@[00:5162) ProgramExpression
//@[00:0000) | └─ObjectExpression [UNPARENTED]
//@[00:0000) |   ├─ObjectPropertyExpression [UNPARENTED]
//@[00:0000) |   | ├─StringLiteralExpression { Value = string } [UNPARENTED]
//@[00:0000) |   | └─StringLiteralExpression { Value = someVal } [UNPARENTED]
//@[00:0000) |   ├─ObjectPropertyExpression [UNPARENTED]
//@[00:0000) |   | ├─StringLiteralExpression { Value = int } [UNPARENTED]
//@[00:0000) |   | └─IntegerLiteralExpression { Value = 123 } [UNPARENTED]
//@[00:0000) |   ├─ObjectPropertyExpression [UNPARENTED]
//@[00:0000) |   | ├─StringLiteralExpression { Value = array } [UNPARENTED]
//@[00:0000) |   | └─ArrayExpression [UNPARENTED]
//@[00:0000) |   |   ├─IntegerLiteralExpression { Value = 1 } [UNPARENTED]
//@[00:0000) |   |   └─IntegerLiteralExpression { Value = 2 } [UNPARENTED]
//@[00:0000) |   └─ObjectPropertyExpression [UNPARENTED]
//@[00:0000) |     ├─StringLiteralExpression { Value = object } [UNPARENTED]
//@[00:0000) |     └─ObjectExpression [UNPARENTED]
//@[00:0000) |       └─ObjectPropertyExpression [UNPARENTED]
//@[00:0000) |         ├─StringLiteralExpression { Value = nestedString } [UNPARENTED]
//@[00:0000) |         └─StringLiteralExpression { Value = someVal } [UNPARENTED]
//@[00:0000) | └─StringLiteralExpression { Value = someVal } [UNPARENTED]
//@[00:0000) | └─IntegerLiteralExpression { Value = 123 } [UNPARENTED]
//@[00:0000) | └─IntegerLiteralExpression { Value = 1 } [UNPARENTED]
//@[00:0000) | └─ObjectExpression [UNPARENTED]
//@[00:0000) |   └─ObjectPropertyExpression [UNPARENTED]
//@[00:0000) |     ├─StringLiteralExpression { Value = nestedString } [UNPARENTED]
//@[00:0000) |     └─StringLiteralExpression { Value = someVal } [UNPARENTED]
//@[00:0000) | └─StringLiteralExpression { Value = someVal } [UNPARENTED]
//@[00:0000) | └─ArrayExpression [UNPARENTED]
//@[00:0000) |   ├─StringLiteralExpression { Value = pizza } [UNPARENTED]
//@[00:0000) |   └─StringLiteralExpression { Value = salad } [UNPARENTED]
//@[00:0000) | └─ObjectExpression [UNPARENTED]
//@[00:0000) |   ├─ObjectPropertyExpression [UNPARENTED]
//@[00:0000) |   | ├─StringLiteralExpression { Value = string } [UNPARENTED]
//@[00:0000) |   | └─StringLiteralExpression { Value = someVal } [UNPARENTED]
//@[00:0000) |   ├─ObjectPropertyExpression [UNPARENTED]
//@[00:0000) |   | ├─StringLiteralExpression { Value = int } [UNPARENTED]
//@[00:0000) |   | └─IntegerLiteralExpression { Value = 123 } [UNPARENTED]
//@[00:0000) |   ├─ObjectPropertyExpression [UNPARENTED]
//@[00:0000) |   | ├─StringLiteralExpression { Value = bool } [UNPARENTED]
//@[00:0000) |   | └─BooleanLiteralExpression { Value = True } [UNPARENTED]
//@[00:0000) |   ├─ObjectPropertyExpression [UNPARENTED]
//@[00:0000) |   | ├─StringLiteralExpression { Value = arrayInt } [UNPARENTED]
//@[00:0000) |   | └─ArrayExpression [UNPARENTED]
//@[00:0000) |   |   ├─IntegerLiteralExpression { Value = 1 } [UNPARENTED]
//@[00:0000) |   |   └─IntegerLiteralExpression { Value = 2 } [UNPARENTED]
//@[00:0000) |   ├─ObjectPropertyExpression [UNPARENTED]
//@[00:0000) |   | ├─StringLiteralExpression { Value = arrayString } [UNPARENTED]
//@[00:0000) |   | └─ArrayExpression [UNPARENTED]
//@[00:0000) |   |   ├─StringLiteralExpression { Value = someVal } [UNPARENTED]
//@[00:0000) |   |   └─StringLiteralExpression { Value = someVal2 } [UNPARENTED]
//@[00:0000) |   ├─ObjectPropertyExpression [UNPARENTED]
//@[00:0000) |   | ├─StringLiteralExpression { Value = arrayBool } [UNPARENTED]
//@[00:0000) |   | └─ArrayExpression [UNPARENTED]
//@[00:0000) |   |   └─BooleanLiteralExpression { Value = True } [UNPARENTED]
//@[00:0000) |   |   └─BooleanLiteralExpression { Value = True } [UNPARENTED]
//@[00:0000) |   └─ObjectPropertyExpression [UNPARENTED]
//@[00:0000) |     ├─StringLiteralExpression { Value = object } [UNPARENTED]
//@[00:0000) |     └─ObjectExpression [UNPARENTED]
//@[00:0000) |       ├─ObjectPropertyExpression [UNPARENTED]
//@[00:0000) |       | ├─StringLiteralExpression { Value = nestedString } [UNPARENTED]
//@[00:0000) |       | └─StringLiteralExpression { Value = someVal } [UNPARENTED]
//@[00:0000) |       ├─ObjectPropertyExpression [UNPARENTED]
//@[00:0000) |       | ├─StringLiteralExpression { Value = nestedInt } [UNPARENTED]
//@[00:0000) |       | └─IntegerLiteralExpression { Value = 123 } [UNPARENTED]
//@[00:0000) |       └─ObjectPropertyExpression [UNPARENTED]
//@[00:0000) |         ├─StringLiteralExpression { Value = nestedBool } [UNPARENTED]
//@[00:0000) |         └─BooleanLiteralExpression { Value = True } [UNPARENTED]
//@[00:0061) ├─DeclaredVariableExpression { Name = loadedText1 }
//@[18:0061) | └─StringLiteralExpression { Value = Lorem ipsum dolor sit amet, consectetur adipiscing elit.\r\n\tProin varius in nunc et laoreet.\r\n  Nam pulvinar ipsum sed lectus porttitor, at porttitor ipsum faucibus.\r\n  \tAliquam euismod, odio tincidunt convallis pulvinar, felis sem porttitor turpis, a condimentum dui erat nec tellus.\r\n  Duis elementum cursus est, congue efficitur risus.\r\n\tMauris sit amet.\r\nExcepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\r\n }
var loadedText2 = sys.loadTextContent('Assets/TextFile.LF.txt')
//@[00:0063) ├─DeclaredVariableExpression { Name = loadedText2 }
//@[18:0063) | └─StringLiteralExpression { Value = Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n Donec laoreet sem tortor, ut dignissim ipsum ornare vel.\n  Duis ac ipsum turpis.\n\tMaecenas at condimentum dui.\n Suspendisse aliquet efficitur iaculis.\nIn hac habitasse platea dictumst.\nEtiam consectetur ut libero ac lobortis.\n\tNullam vitae auctor massa.\nFusce tincidunt urna purus, sit amet.\n }
var loadedTextEncoding1 = loadTextContent('Assets/encoding-ascii.txt', 'us-ascii')
//@[00:0082) ├─DeclaredVariableExpression { Name = loadedTextEncoding1 }
//@[26:0082) | └─StringLiteralExpression { Value = 32 = \n33 = !\n34 = "\n35 = #\n36 = $\n37 = %\n38 = &\n39 = '\n40 = (\n41 = )\n42 = *\n43 = +\n44 = ,\n45 = -\n46 = .\n47 = /\n48 = 0\n49 = 1\n50 = 2\n51 = 3\n52 = 4\n53 = 5\n54 = 6\n55 = 7\n56 = 8\n57 = 9\n58 = :\n59 = ;\n60 = <\n61 = =\n62 = >\n63 = ?\n64 = @\n65 = A\n66 = B\n67 = C\n68 = D\n69 = E\n70 = F\n71 = G\n72 = H\n73 = I\n74 = J\n75 = K\n76 = L\n77 = M\n78 = N\n79 = O\n80 = P\n81 = Q\n82 = R\n83 = S\n84 = T\n85 = U\n86 = V\n87 = W\n88 = X\n89 = Y\n90 = Z\n91 = [\n92 = \\n93 = ]\n94 = ^\n95 = _\n96 = `\n97 = a\n98 = b\n99 = c\n100 = d\n101 = e\n102 = f\n103 = g\n104 = h\n105 = i\n106 = j\n107 = k\n108 = l\n109 = m\n110 = n\n111 = o\n112 = p\n113 = q\n114 = r\n115 = s\n116 = t\n117 = u\n118 = v\n119 = w\n120 = x\n121 = y\n122 = z\n123 = {\n124 = |\n125 = }\n126 = ~\n }
var loadedTextEncoding2 = loadTextContent('Assets/encoding-utf8.txt', 'utf-8')
//@[00:0078) ├─DeclaredVariableExpression { Name = loadedTextEncoding2 }
//@[26:0078) | └─StringLiteralExpression { Value = 💪😊😈🍕☕\r\n🐱‍👤\r\n\r\n朝辞白帝彩云间\r\n千里江陵一日还\r\n两岸猿声啼不住\r\n轻舟已过万重山\r\n\r\nΠ π Φ φ\r\n\r\n😎\r\n\r\nαα\r\nΩω\r\nΘ  \r\n\r\nZażółć gęślą jaźń\r\n\r\náéóúñü - ¡Hola!\r\n\r\n二头肌二头肌\r\n\r\n\r\n二头肌\r\nΘ二头肌α\r\n\r\n𐐷\r\n\u{10437}\r\n\u{D801}\u{DC37}\r\n\r\n❆ Hello\u{20}World\u{21} ❁\r\n\r\n\ta\tb\tc\td\te\tf\tg\th\t\r\n8\t♜\t♞\t♝\t♛\t♚\t♝\t♞\t♜\t8\r\n7\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t7\r\n6\t\t\t\t\t\t\t\t\t6\r\n5\t\t\t\t\t\t\t\t\t5\r\n4\t\t\t\t\t\t\t\t\t4\r\n3\t\t\t\t\t\t\t\t\t3\r\n2\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t2\r\n1\t♖\t♘\t♗\t♕\t♔\t♗\t♘\t♖\t1\r\n\ta\tb\tc\td\te\tf\tg\th\r\n }
var loadedTextEncoding3 = loadTextContent('Assets/encoding-utf16.txt', 'utf-16')
//@[00:0080) ├─DeclaredVariableExpression { Name = loadedTextEncoding3 }
//@[26:0080) | └─StringLiteralExpression { Value = 💪😊😈🍕☕\r\n🐱‍👤\r\n\r\n朝辞白帝彩云间\r\n千里江陵一日还\r\n两岸猿声啼不住\r\n轻舟已过万重山\r\n\r\nΠ π Φ φ\r\n\r\n😎\r\n\r\nαα\r\nΩω\r\nΘ  \r\n\r\nZażółć gęślą jaźń\r\n\r\náéóúñü - ¡Hola!\r\n\r\n二头肌二头肌\r\n\r\n\r\n二头肌\r\nΘ二头肌α\r\n\r\n𐐷\r\n\u{10437}\r\n\u{D801}\u{DC37}\r\n\r\n❆ Hello\u{20}World\u{21} ❁\r\n\r\n\ta\tb\tc\td\te\tf\tg\th\t\r\n8\t♜\t♞\t♝\t♛\t♚\t♝\t♞\t♜\t8\r\n7\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t7\r\n6\t\t\t\t\t\t\t\t\t6\r\n5\t\t\t\t\t\t\t\t\t5\r\n4\t\t\t\t\t\t\t\t\t4\r\n3\t\t\t\t\t\t\t\t\t3\r\n2\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t2\r\n1\t♖\t♘\t♗\t♕\t♔\t♗\t♘\t♖\t1\r\n\ta\tb\tc\td\te\tf\tg\th\r\n }
var loadedTextEncoding4 = loadTextContent('Assets/encoding-utf16be.txt', 'utf-16BE')
//@[00:0084) ├─DeclaredVariableExpression { Name = loadedTextEncoding4 }
//@[26:0084) | └─StringLiteralExpression { Value = 💪😊😈🍕☕\r\n🐱‍👤\r\n\r\n朝辞白帝彩云间\r\n千里江陵一日还\r\n两岸猿声啼不住\r\n轻舟已过万重山\r\n\r\nΠ π Φ φ\r\n\r\n😎\r\n\r\nαα\r\nΩω\r\nΘ  \r\n\r\nZażółć gęślą jaźń\r\n\r\náéóúñü - ¡Hola!\r\n\r\n二头肌二头肌\r\n\r\n\r\n二头肌\r\nΘ二头肌α\r\n\r\n𐐷\r\n\u{10437}\r\n\u{D801}\u{DC37}\r\n\r\n❆ Hello\u{20}World\u{21} ❁\r\n\r\n\ta\tb\tc\td\te\tf\tg\th\t\r\n8\t♜\t♞\t♝\t♛\t♚\t♝\t♞\t♜\t8\r\n7\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t7\r\n6\t\t\t\t\t\t\t\t\t6\r\n5\t\t\t\t\t\t\t\t\t5\r\n4\t\t\t\t\t\t\t\t\t4\r\n3\t\t\t\t\t\t\t\t\t3\r\n2\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t2\r\n1\t♖\t♘\t♗\t♕\t♔\t♗\t♘\t♖\t1\r\n\ta\tb\tc\td\te\tf\tg\th\r\n }
var loadedTextEncoding5 = loadTextContent('Assets/encoding-iso.txt', 'iso-8859-1')
//@[00:0082) ├─DeclaredVariableExpression { Name = loadedTextEncoding5 }
//@[26:0082) | └─StringLiteralExpression { Value = ¡\t\tinverted exclamation mark\n¢\t\tcent\n£\t\tpound\n¤\t\tcurrency\n¥\t\tyen\n¦\t\tbroken vertical bar\n§\t\tsection\n¨\t\tspacing diaeresis\n©\t\tcopyright\nª\t\tfeminine ordinal indicator\n«\t\tangle quotation mark (left)\n¬\t\tnegation\n­\t\tsoft hyphen\n®\t\tregistered trademark\n¯\t\tspacing macron\n°\t\tdegree\n±\t\tplus-or-minus\n²\t\tsuperscript 2\n³\t\tsuperscript 3\n´\t\tspacing acute\n¶\t\tparagraph\n·\t\tmiddle dot\n¸\t\tspacing cedilla\n¹\t\tsuperscript 1\nº\t\tmasculine ordinal indicator\n»\t\tangle quotation mark (right)\n¼\t\tfraction 1/4\n½\t\tfraction 1/2\n¾\t\tfraction 3/4\n¿\t\tinverted question mark\nÀ\t\tcapital a, grave accent\nÁ\t\tcapital a, acute accent\nÂ\t\tcapital a, circumflex accent\nÃ\t\tcapital a, tilde\nÄ\t\tcapital a, umlaut mark\nÅ\t\tcapital a, ring\nÆ\t\tcapital ae\nÇ\t\tcapital c, cedilla\nÈ\t\tcapital e, grave accent\nÉ\t\tcapital e, acute accent\nÊ\t\tcapital e, circumflex accent\nË\t\tcapital e, umlaut mark\nÌ\t\tcapital i, grave accent\nÍ\t\tcapital i, acute accent\nÎ\t\tcapital i, circumflex accent\nÏ\t\tcapital i, umlaut mark\nÐ\t\tcapital eth, Icelandic\nÑ\t\tcapital n, tilde\nÒ\t\tcapital o, grave accent\nÓ\t\tcapital o, acute accent\nÔ\t\tcapital o, circumflex accent\nÕ\t\tcapital o, tilde\nÖ\t\tcapital o, umlaut mark\n×\t\tmultiplication\nØ\t\tcapital o, slash\nÙ\t\tcapital u, grave accent\nÚ\t\tcapital u, acute accent\nÛ\t\tcapital u, circumflex accent\nÜ\t\tcapital u, umlaut mark\nÝ\t\tcapital y, acute accent\nÞ\t\tcapital THORN, Icelandic\nß\t\tsmall sharp s, German\nà\t\tsmall a, grave accent\ná\t\tsmall a, acute accent\nâ\t\tsmall a, circumflex accent\nã\t\tsmall a, tilde\nä\t\tsmall a, umlaut mark\nå\t\tsmall a, ring\næ\t\tsmall ae\nç\t\tsmall c, cedilla\nè\t\tsmall e, grave accent\né\t\tsmall e, acute accent\nê\t\tsmall e, circumflex accent\në\t\tsmall e, umlaut mark\nì\t\tsmall i, grave accent\ní\t\tsmall i, acute accent\nî\t\tsmall i, circumflex accent\nï\t\tsmall i, umlaut mark\nð\t\tsmall eth, Icelandic\nñ\t\tsmall n, tilde\nò\t\tsmall o, grave accent\nó\t\tsmall o, acute accent\nô\t\tsmall o, circumflex accent\nõ\t\tsmall o, tilde\nö\t\tsmall o, umlaut mark\n÷\t\tdivision\nø\t\tsmall o, slash\nù\t\tsmall u, grave accent\nú\t\tsmall u, acute accent\nû\t\tsmall u, circumflex accent\nü\t\tsmall u, umlaut mark\ný\t\tsmall y, acute accent\nþ\t\tsmall thorn, Icelandic\nÿ\t\tsmall y, umlaut mark\n }

var loadedBinary1 = loadFileAsBase64('Assets/binary')
//@[00:0053) ├─DeclaredVariableExpression { Name = loadedBinary1 }
//@[20:0053) | └─StringLiteralExpression { Value = g8fCjQHuFUfnHlTUHxe3qmjeM3HlYSToV7qTGrcJ6vgFNjvgpmxnexFbzjJV/Ejx8jXKa8L32YUn1f/HUnPY6u5c1SaGaP8OiVyRK4ef52hOtc3Yd29c9ubDsLohwLlmuiQDCvVduNMejR6eZy50ti3eYaLu3e04IKC7kTFO0Ph/vSIhlfkS6lUB9e7EJuHAa+3yJFn0uIVFs/BF67fNV9zwx92XyhFL8tmv3IZNd1+0cAby99+zif6iBPXcJ5XTUUz4UHaPmZLPT75hd4iGZzOk/I+FAsUTxRDra76D+sSXay7qBv5TyVLlhs7kqSVAecki6vQG0Siku64tl3PKKEy9JV1lHItgg/IFDYNd8/DKMUpEi90wunW/CfTpQcctzHzZFjl5euswaXgTDvVt2KRHPpi3likE8b1GuyKFVfKNT47VFGSabuUZlhDzbzx7qECnIpA1M7kH+TUHhGTe3ezmmPq+EO6jybYNMpMs+7gcfYAEtzE4gfpubKHLQI8ZYFKFxPCo0ypOwh4Z1nStJkcrNX2UlSDFfPb+LlCaGRxRKPN9md+2sr4x6qm0TptmI9o8wJF7FvqJUS2obFz2KhnfdKg7seuknpisasENchzEO2hoMtpCf5Lt3ZwsLCnFrllFaNmV2BWfA0k74f5MykjIioppM8ajdzORDtjTv/WcNyxdVlTV6nr2Oe43WFKZT0DFMDGHVgTZRBxVH5JtfT1akurR+IyvTegR6kSMHXSWlE1iEoPK8gdNpFLHdAJ8VwPnMGRC8CoCGzDeAakmwiHuecPOzcg9cOQe2Rlo68kJSr6q2hEcTmm9kPj7vfAeocqlosDd2Ci7xcBAJNb/rC4IVllhZPyqt2C8L1VbN5wZfBNgwpA73oOX0kxIKoCqH+Ni167WvC1ZwTqlVAWeAa1RiSplisinKBwDXahu2qDhVJyOt/RwV8Y+W3ynFGXYOW9BDwXVwKUm2zrKl6j0Y1AUTH5HArfCwwThXW2ZFslbr/fM9nJPAbbxpDY2FQBAlt8ST3PyLrFBfXlsV0JqVhidnw94NAbTiPqck15pTGbncg8VunYUAMw/JAOLf2SIJ8cA75IdJMp0UJXoMcOfVKkVdgMbGi7BHtJ3ZFwIajbNdWoNQV0k/LlwwmqGYGkh71wBX4WEdW6MqvvT8Mt/xyA6xzflqnMzgvQPIe6v29FETR9wxSKG52oCOY/DtPXEo+DgZvQwQn3F4BwX+GZawHbbMjWCIDxoSmph5TfZMzF0EkhEi47Uk93FPgiBQPjKSYMxnJ9Lo9UwZjsxdtk7ONKe1GIxfHdx3no4uuRp8WKqjpz017VBOWubdXPxO8Q8QOv6nT9faVVCMUQApehFlg== }
var loadedBinary2 = sys.loadFileAsBase64('Assets/binary')
//@[00:0057) ├─DeclaredVariableExpression { Name = loadedBinary2 }
//@[20:0057) | └─StringLiteralExpression { Value = g8fCjQHuFUfnHlTUHxe3qmjeM3HlYSToV7qTGrcJ6vgFNjvgpmxnexFbzjJV/Ejx8jXKa8L32YUn1f/HUnPY6u5c1SaGaP8OiVyRK4ef52hOtc3Yd29c9ubDsLohwLlmuiQDCvVduNMejR6eZy50ti3eYaLu3e04IKC7kTFO0Ph/vSIhlfkS6lUB9e7EJuHAa+3yJFn0uIVFs/BF67fNV9zwx92XyhFL8tmv3IZNd1+0cAby99+zif6iBPXcJ5XTUUz4UHaPmZLPT75hd4iGZzOk/I+FAsUTxRDra76D+sSXay7qBv5TyVLlhs7kqSVAecki6vQG0Siku64tl3PKKEy9JV1lHItgg/IFDYNd8/DKMUpEi90wunW/CfTpQcctzHzZFjl5euswaXgTDvVt2KRHPpi3likE8b1GuyKFVfKNT47VFGSabuUZlhDzbzx7qECnIpA1M7kH+TUHhGTe3ezmmPq+EO6jybYNMpMs+7gcfYAEtzE4gfpubKHLQI8ZYFKFxPCo0ypOwh4Z1nStJkcrNX2UlSDFfPb+LlCaGRxRKPN9md+2sr4x6qm0TptmI9o8wJF7FvqJUS2obFz2KhnfdKg7seuknpisasENchzEO2hoMtpCf5Lt3ZwsLCnFrllFaNmV2BWfA0k74f5MykjIioppM8ajdzORDtjTv/WcNyxdVlTV6nr2Oe43WFKZT0DFMDGHVgTZRBxVH5JtfT1akurR+IyvTegR6kSMHXSWlE1iEoPK8gdNpFLHdAJ8VwPnMGRC8CoCGzDeAakmwiHuecPOzcg9cOQe2Rlo68kJSr6q2hEcTmm9kPj7vfAeocqlosDd2Ci7xcBAJNb/rC4IVllhZPyqt2C8L1VbN5wZfBNgwpA73oOX0kxIKoCqH+Ni167WvC1ZwTqlVAWeAa1RiSplisinKBwDXahu2qDhVJyOt/RwV8Y+W3ynFGXYOW9BDwXVwKUm2zrKl6j0Y1AUTH5HArfCwwThXW2ZFslbr/fM9nJPAbbxpDY2FQBAlt8ST3PyLrFBfXlsV0JqVhidnw94NAbTiPqck15pTGbncg8VunYUAMw/JAOLf2SIJ8cA75IdJMp0UJXoMcOfVKkVdgMbGi7BHtJ3ZFwIajbNdWoNQV0k/LlwwmqGYGkh71wBX4WEdW6MqvvT8Mt/xyA6xzflqnMzgvQPIe6v29FETR9wxSKG52oCOY/DtPXEo+DgZvQwQn3F4BwX+GZawHbbMjWCIDxoSmph5TfZMzF0EkhEi47Uk93FPgiBQPjKSYMxnJ9Lo9UwZjsxdtk7ONKe1GIxfHdx3no4uuRp8WKqjpz017VBOWubdXPxO8Q8QOv6nT9faVVCMUQApehFlg== }

var loadedTextInterpolation1 = 'Text: ${loadTextContent('Assets/TextFile.CRLF.txt')}'
//@[40:0083) ├─DeclaredVariableExpression { Name = $fxv#0 }
//@[40:0083) | └─StringLiteralExpression { Value = Lorem ipsum dolor sit amet, consectetur adipiscing elit.\r\n\tProin varius in nunc et laoreet.\r\n  Nam pulvinar ipsum sed lectus porttitor, at porttitor ipsum faucibus.\r\n  \tAliquam euismod, odio tincidunt convallis pulvinar, felis sem porttitor turpis, a condimentum dui erat nec tellus.\r\n  Duis elementum cursus est, congue efficitur risus.\r\n\tMauris sit amet.\r\nExcepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\r\n }
//@[00:0085) ├─DeclaredVariableExpression { Name = loadedTextInterpolation1 }
//@[31:0085) | └─InterpolatedStringExpression
//@[40:0083) |   └─SynthesizedVariableReferenceExpression { Name = $fxv#0 }
var loadedTextInterpolation2 = 'Text: ${loadTextContent('Assets/TextFile.LF.txt')}'
//@[40:0081) ├─DeclaredVariableExpression { Name = $fxv#1 }
//@[40:0081) | └─StringLiteralExpression { Value = Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n Donec laoreet sem tortor, ut dignissim ipsum ornare vel.\n  Duis ac ipsum turpis.\n\tMaecenas at condimentum dui.\n Suspendisse aliquet efficitur iaculis.\nIn hac habitasse platea dictumst.\nEtiam consectetur ut libero ac lobortis.\n\tNullam vitae auctor massa.\nFusce tincidunt urna purus, sit amet.\n }
//@[00:0083) ├─DeclaredVariableExpression { Name = loadedTextInterpolation2 }
//@[31:0083) | └─InterpolatedStringExpression
//@[40:0081) |   └─SynthesizedVariableReferenceExpression { Name = $fxv#1 }

var loadedTextObject1 = {
//@[00:0084) ├─DeclaredVariableExpression { Name = loadedTextObject1 }
//@[24:0084) | └─ObjectExpression
  'text' : loadTextContent('Assets/TextFile.CRLF.txt')
//@[11:0054) ├─DeclaredVariableExpression { Name = $fxv#2 }
//@[11:0054) | └─StringLiteralExpression { Value = Lorem ipsum dolor sit amet, consectetur adipiscing elit.\r\n\tProin varius in nunc et laoreet.\r\n  Nam pulvinar ipsum sed lectus porttitor, at porttitor ipsum faucibus.\r\n  \tAliquam euismod, odio tincidunt convallis pulvinar, felis sem porttitor turpis, a condimentum dui erat nec tellus.\r\n  Duis elementum cursus est, congue efficitur risus.\r\n\tMauris sit amet.\r\nExcepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\r\n }
//@[02:0054) |   └─ObjectPropertyExpression
//@[02:0008) |     ├─StringLiteralExpression { Value = text }
//@[11:0054) |     └─SynthesizedVariableReferenceExpression { Name = $fxv#2 }
}
var loadedTextObject2 = {
//@[00:0084) ├─DeclaredVariableExpression { Name = loadedTextObject2 }
//@[24:0084) | └─ObjectExpression
  'text' : loadTextContent('Assets/TextFile.LF.txt')  
//@[11:0052) ├─DeclaredVariableExpression { Name = $fxv#3 }
//@[11:0052) | └─StringLiteralExpression { Value = Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n Donec laoreet sem tortor, ut dignissim ipsum ornare vel.\n  Duis ac ipsum turpis.\n\tMaecenas at condimentum dui.\n Suspendisse aliquet efficitur iaculis.\nIn hac habitasse platea dictumst.\nEtiam consectetur ut libero ac lobortis.\n\tNullam vitae auctor massa.\nFusce tincidunt urna purus, sit amet.\n }
//@[02:0052) |   └─ObjectPropertyExpression
//@[02:0008) |     ├─StringLiteralExpression { Value = text }
//@[11:0052) |     └─SynthesizedVariableReferenceExpression { Name = $fxv#3 }
}
var loadedBinaryInObject = {
//@[00:0074) ├─DeclaredVariableExpression { Name = loadedBinaryInObject }
//@[27:0074) | └─ObjectExpression
  file: loadFileAsBase64('Assets/binary')
//@[08:0041) ├─DeclaredVariableExpression { Name = $fxv#4 }
//@[08:0041) | └─StringLiteralExpression { Value = g8fCjQHuFUfnHlTUHxe3qmjeM3HlYSToV7qTGrcJ6vgFNjvgpmxnexFbzjJV/Ejx8jXKa8L32YUn1f/HUnPY6u5c1SaGaP8OiVyRK4ef52hOtc3Yd29c9ubDsLohwLlmuiQDCvVduNMejR6eZy50ti3eYaLu3e04IKC7kTFO0Ph/vSIhlfkS6lUB9e7EJuHAa+3yJFn0uIVFs/BF67fNV9zwx92XyhFL8tmv3IZNd1+0cAby99+zif6iBPXcJ5XTUUz4UHaPmZLPT75hd4iGZzOk/I+FAsUTxRDra76D+sSXay7qBv5TyVLlhs7kqSVAecki6vQG0Siku64tl3PKKEy9JV1lHItgg/IFDYNd8/DKMUpEi90wunW/CfTpQcctzHzZFjl5euswaXgTDvVt2KRHPpi3likE8b1GuyKFVfKNT47VFGSabuUZlhDzbzx7qECnIpA1M7kH+TUHhGTe3ezmmPq+EO6jybYNMpMs+7gcfYAEtzE4gfpubKHLQI8ZYFKFxPCo0ypOwh4Z1nStJkcrNX2UlSDFfPb+LlCaGRxRKPN9md+2sr4x6qm0TptmI9o8wJF7FvqJUS2obFz2KhnfdKg7seuknpisasENchzEO2hoMtpCf5Lt3ZwsLCnFrllFaNmV2BWfA0k74f5MykjIioppM8ajdzORDtjTv/WcNyxdVlTV6nr2Oe43WFKZT0DFMDGHVgTZRBxVH5JtfT1akurR+IyvTegR6kSMHXSWlE1iEoPK8gdNpFLHdAJ8VwPnMGRC8CoCGzDeAakmwiHuecPOzcg9cOQe2Rlo68kJSr6q2hEcTmm9kPj7vfAeocqlosDd2Ci7xcBAJNb/rC4IVllhZPyqt2C8L1VbN5wZfBNgwpA73oOX0kxIKoCqH+Ni167WvC1ZwTqlVAWeAa1RiSplisinKBwDXahu2qDhVJyOt/RwV8Y+W3ynFGXYOW9BDwXVwKUm2zrKl6j0Y1AUTH5HArfCwwThXW2ZFslbr/fM9nJPAbbxpDY2FQBAlt8ST3PyLrFBfXlsV0JqVhidnw94NAbTiPqck15pTGbncg8VunYUAMw/JAOLf2SIJ8cA75IdJMp0UJXoMcOfVKkVdgMbGi7BHtJ3ZFwIajbNdWoNQV0k/LlwwmqGYGkh71wBX4WEdW6MqvvT8Mt/xyA6xzflqnMzgvQPIe6v29FETR9wxSKG52oCOY/DtPXEo+DgZvQwQn3F4BwX+GZawHbbMjWCIDxoSmph5TfZMzF0EkhEi47Uk93FPgiBQPjKSYMxnJ9Lo9UwZjsxdtk7ONKe1GIxfHdx3no4uuRp8WKqjpz017VBOWubdXPxO8Q8QOv6nT9faVVCMUQApehFlg== }
//@[02:0041) |   └─ObjectPropertyExpression
//@[02:0006) |     ├─StringLiteralExpression { Value = file }
//@[08:0041) |     └─SynthesizedVariableReferenceExpression { Name = $fxv#4 }
}

var loadedTextArray = [
//@[00:0108) ├─DeclaredVariableExpression { Name = loadedTextArray }
//@[22:0108) | └─ArrayExpression
  loadTextContent('Assets/TextFile.LF.txt')
//@[02:0043) ├─DeclaredVariableExpression { Name = $fxv#5 }
//@[02:0043) | └─StringLiteralExpression { Value = Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n Donec laoreet sem tortor, ut dignissim ipsum ornare vel.\n  Duis ac ipsum turpis.\n\tMaecenas at condimentum dui.\n Suspendisse aliquet efficitur iaculis.\nIn hac habitasse platea dictumst.\nEtiam consectetur ut libero ac lobortis.\n\tNullam vitae auctor massa.\nFusce tincidunt urna purus, sit amet.\n }
//@[02:0043) |   ├─SynthesizedVariableReferenceExpression { Name = $fxv#5 }
  loadFileAsBase64('Assets/binary')
//@[02:0035) ├─DeclaredVariableExpression { Name = $fxv#6 }
//@[02:0035) | └─StringLiteralExpression { Value = g8fCjQHuFUfnHlTUHxe3qmjeM3HlYSToV7qTGrcJ6vgFNjvgpmxnexFbzjJV/Ejx8jXKa8L32YUn1f/HUnPY6u5c1SaGaP8OiVyRK4ef52hOtc3Yd29c9ubDsLohwLlmuiQDCvVduNMejR6eZy50ti3eYaLu3e04IKC7kTFO0Ph/vSIhlfkS6lUB9e7EJuHAa+3yJFn0uIVFs/BF67fNV9zwx92XyhFL8tmv3IZNd1+0cAby99+zif6iBPXcJ5XTUUz4UHaPmZLPT75hd4iGZzOk/I+FAsUTxRDra76D+sSXay7qBv5TyVLlhs7kqSVAecki6vQG0Siku64tl3PKKEy9JV1lHItgg/IFDYNd8/DKMUpEi90wunW/CfTpQcctzHzZFjl5euswaXgTDvVt2KRHPpi3likE8b1GuyKFVfKNT47VFGSabuUZlhDzbzx7qECnIpA1M7kH+TUHhGTe3ezmmPq+EO6jybYNMpMs+7gcfYAEtzE4gfpubKHLQI8ZYFKFxPCo0ypOwh4Z1nStJkcrNX2UlSDFfPb+LlCaGRxRKPN9md+2sr4x6qm0TptmI9o8wJF7FvqJUS2obFz2KhnfdKg7seuknpisasENchzEO2hoMtpCf5Lt3ZwsLCnFrllFaNmV2BWfA0k74f5MykjIioppM8ajdzORDtjTv/WcNyxdVlTV6nr2Oe43WFKZT0DFMDGHVgTZRBxVH5JtfT1akurR+IyvTegR6kSMHXSWlE1iEoPK8gdNpFLHdAJ8VwPnMGRC8CoCGzDeAakmwiHuecPOzcg9cOQe2Rlo68kJSr6q2hEcTmm9kPj7vfAeocqlosDd2Ci7xcBAJNb/rC4IVllhZPyqt2C8L1VbN5wZfBNgwpA73oOX0kxIKoCqH+Ni167WvC1ZwTqlVAWeAa1RiSplisinKBwDXahu2qDhVJyOt/RwV8Y+W3ynFGXYOW9BDwXVwKUm2zrKl6j0Y1AUTH5HArfCwwThXW2ZFslbr/fM9nJPAbbxpDY2FQBAlt8ST3PyLrFBfXlsV0JqVhidnw94NAbTiPqck15pTGbncg8VunYUAMw/JAOLf2SIJ8cA75IdJMp0UJXoMcOfVKkVdgMbGi7BHtJ3ZFwIajbNdWoNQV0k/LlwwmqGYGkh71wBX4WEdW6MqvvT8Mt/xyA6xzflqnMzgvQPIe6v29FETR9wxSKG52oCOY/DtPXEo+DgZvQwQn3F4BwX+GZawHbbMjWCIDxoSmph5TfZMzF0EkhEi47Uk93FPgiBQPjKSYMxnJ9Lo9UwZjsxdtk7ONKe1GIxfHdx3no4uuRp8WKqjpz017VBOWubdXPxO8Q8QOv6nT9faVVCMUQApehFlg== }
//@[02:0035) |   └─SynthesizedVariableReferenceExpression { Name = $fxv#6 }
]

var loadedTextArrayInObject = {
//@[00:0142) ├─DeclaredVariableExpression { Name = loadedTextArrayInObject }
//@[30:0142) | └─ObjectExpression
  'files' : [
//@[02:0106) |   └─ObjectPropertyExpression
//@[02:0009) |     ├─StringLiteralExpression { Value = files }
//@[12:0106) |     └─ArrayExpression
    loadTextContent('Assets/TextFile.CRLF.txt')
//@[04:0047) ├─DeclaredVariableExpression { Name = $fxv#7 }
//@[04:0047) | └─StringLiteralExpression { Value = Lorem ipsum dolor sit amet, consectetur adipiscing elit.\r\n\tProin varius in nunc et laoreet.\r\n  Nam pulvinar ipsum sed lectus porttitor, at porttitor ipsum faucibus.\r\n  \tAliquam euismod, odio tincidunt convallis pulvinar, felis sem porttitor turpis, a condimentum dui erat nec tellus.\r\n  Duis elementum cursus est, congue efficitur risus.\r\n\tMauris sit amet.\r\nExcepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\r\n }
//@[04:0047) |       ├─SynthesizedVariableReferenceExpression { Name = $fxv#7 }
    loadFileAsBase64('Assets/binary')
//@[04:0037) ├─DeclaredVariableExpression { Name = $fxv#8 }
//@[04:0037) | └─StringLiteralExpression { Value = g8fCjQHuFUfnHlTUHxe3qmjeM3HlYSToV7qTGrcJ6vgFNjvgpmxnexFbzjJV/Ejx8jXKa8L32YUn1f/HUnPY6u5c1SaGaP8OiVyRK4ef52hOtc3Yd29c9ubDsLohwLlmuiQDCvVduNMejR6eZy50ti3eYaLu3e04IKC7kTFO0Ph/vSIhlfkS6lUB9e7EJuHAa+3yJFn0uIVFs/BF67fNV9zwx92XyhFL8tmv3IZNd1+0cAby99+zif6iBPXcJ5XTUUz4UHaPmZLPT75hd4iGZzOk/I+FAsUTxRDra76D+sSXay7qBv5TyVLlhs7kqSVAecki6vQG0Siku64tl3PKKEy9JV1lHItgg/IFDYNd8/DKMUpEi90wunW/CfTpQcctzHzZFjl5euswaXgTDvVt2KRHPpi3likE8b1GuyKFVfKNT47VFGSabuUZlhDzbzx7qECnIpA1M7kH+TUHhGTe3ezmmPq+EO6jybYNMpMs+7gcfYAEtzE4gfpubKHLQI8ZYFKFxPCo0ypOwh4Z1nStJkcrNX2UlSDFfPb+LlCaGRxRKPN9md+2sr4x6qm0TptmI9o8wJF7FvqJUS2obFz2KhnfdKg7seuknpisasENchzEO2hoMtpCf5Lt3ZwsLCnFrllFaNmV2BWfA0k74f5MykjIioppM8ajdzORDtjTv/WcNyxdVlTV6nr2Oe43WFKZT0DFMDGHVgTZRBxVH5JtfT1akurR+IyvTegR6kSMHXSWlE1iEoPK8gdNpFLHdAJ8VwPnMGRC8CoCGzDeAakmwiHuecPOzcg9cOQe2Rlo68kJSr6q2hEcTmm9kPj7vfAeocqlosDd2Ci7xcBAJNb/rC4IVllhZPyqt2C8L1VbN5wZfBNgwpA73oOX0kxIKoCqH+Ni167WvC1ZwTqlVAWeAa1RiSplisinKBwDXahu2qDhVJyOt/RwV8Y+W3ynFGXYOW9BDwXVwKUm2zrKl6j0Y1AUTH5HArfCwwThXW2ZFslbr/fM9nJPAbbxpDY2FQBAlt8ST3PyLrFBfXlsV0JqVhidnw94NAbTiPqck15pTGbncg8VunYUAMw/JAOLf2SIJ8cA75IdJMp0UJXoMcOfVKkVdgMbGi7BHtJ3ZFwIajbNdWoNQV0k/LlwwmqGYGkh71wBX4WEdW6MqvvT8Mt/xyA6xzflqnMzgvQPIe6v29FETR9wxSKG52oCOY/DtPXEo+DgZvQwQn3F4BwX+GZawHbbMjWCIDxoSmph5TfZMzF0EkhEi47Uk93FPgiBQPjKSYMxnJ9Lo9UwZjsxdtk7ONKe1GIxfHdx3no4uuRp8WKqjpz017VBOWubdXPxO8Q8QOv6nT9faVVCMUQApehFlg== }
//@[04:0037) |       └─SynthesizedVariableReferenceExpression { Name = $fxv#8 }
  ]
}

var loadedTextArrayInObjectFunctions = {
//@[00:0277) ├─DeclaredVariableExpression { Name = loadedTextArrayInObjectFunctions }
//@[39:0277) | └─ObjectExpression
  'files' : [
//@[02:0232) |   └─ObjectPropertyExpression
//@[02:0009) |     ├─StringLiteralExpression { Value = files }
//@[12:0232) |     └─ArrayExpression
    length(loadTextContent('Assets/TextFile.CRLF.txt'))
//@[11:0054) ├─DeclaredVariableExpression { Name = $fxv#9 }
//@[11:0054) | └─StringLiteralExpression { Value = Lorem ipsum dolor sit amet, consectetur adipiscing elit.\r\n\tProin varius in nunc et laoreet.\r\n  Nam pulvinar ipsum sed lectus porttitor, at porttitor ipsum faucibus.\r\n  \tAliquam euismod, odio tincidunt convallis pulvinar, felis sem porttitor turpis, a condimentum dui erat nec tellus.\r\n  Duis elementum cursus est, congue efficitur risus.\r\n\tMauris sit amet.\r\nExcepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\r\n }
//@[04:0055) |       ├─FunctionCallExpression { Name = length }
//@[11:0054) |       | └─SynthesizedVariableReferenceExpression { Name = $fxv#9 }
    sys.length(loadTextContent('Assets/TextFile.LF.txt'))
//@[15:0056) ├─DeclaredVariableExpression { Name = $fxv#10 }
//@[15:0056) | └─StringLiteralExpression { Value = Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n Donec laoreet sem tortor, ut dignissim ipsum ornare vel.\n  Duis ac ipsum turpis.\n\tMaecenas at condimentum dui.\n Suspendisse aliquet efficitur iaculis.\nIn hac habitasse platea dictumst.\nEtiam consectetur ut libero ac lobortis.\n\tNullam vitae auctor massa.\nFusce tincidunt urna purus, sit amet.\n }
//@[04:0057) |       ├─FunctionCallExpression { Name = length }
//@[15:0056) |       | └─SynthesizedVariableReferenceExpression { Name = $fxv#10 }
    length(loadFileAsBase64('Assets/binary'))
//@[11:0044) ├─DeclaredVariableExpression { Name = $fxv#11 }
//@[11:0044) | └─StringLiteralExpression { Value = g8fCjQHuFUfnHlTUHxe3qmjeM3HlYSToV7qTGrcJ6vgFNjvgpmxnexFbzjJV/Ejx8jXKa8L32YUn1f/HUnPY6u5c1SaGaP8OiVyRK4ef52hOtc3Yd29c9ubDsLohwLlmuiQDCvVduNMejR6eZy50ti3eYaLu3e04IKC7kTFO0Ph/vSIhlfkS6lUB9e7EJuHAa+3yJFn0uIVFs/BF67fNV9zwx92XyhFL8tmv3IZNd1+0cAby99+zif6iBPXcJ5XTUUz4UHaPmZLPT75hd4iGZzOk/I+FAsUTxRDra76D+sSXay7qBv5TyVLlhs7kqSVAecki6vQG0Siku64tl3PKKEy9JV1lHItgg/IFDYNd8/DKMUpEi90wunW/CfTpQcctzHzZFjl5euswaXgTDvVt2KRHPpi3likE8b1GuyKFVfKNT47VFGSabuUZlhDzbzx7qECnIpA1M7kH+TUHhGTe3ezmmPq+EO6jybYNMpMs+7gcfYAEtzE4gfpubKHLQI8ZYFKFxPCo0ypOwh4Z1nStJkcrNX2UlSDFfPb+LlCaGRxRKPN9md+2sr4x6qm0TptmI9o8wJF7FvqJUS2obFz2KhnfdKg7seuknpisasENchzEO2hoMtpCf5Lt3ZwsLCnFrllFaNmV2BWfA0k74f5MykjIioppM8ajdzORDtjTv/WcNyxdVlTV6nr2Oe43WFKZT0DFMDGHVgTZRBxVH5JtfT1akurR+IyvTegR6kSMHXSWlE1iEoPK8gdNpFLHdAJ8VwPnMGRC8CoCGzDeAakmwiHuecPOzcg9cOQe2Rlo68kJSr6q2hEcTmm9kPj7vfAeocqlosDd2Ci7xcBAJNb/rC4IVllhZPyqt2C8L1VbN5wZfBNgwpA73oOX0kxIKoCqH+Ni167WvC1ZwTqlVAWeAa1RiSplisinKBwDXahu2qDhVJyOt/RwV8Y+W3ynFGXYOW9BDwXVwKUm2zrKl6j0Y1AUTH5HArfCwwThXW2ZFslbr/fM9nJPAbbxpDY2FQBAlt8ST3PyLrFBfXlsV0JqVhidnw94NAbTiPqck15pTGbncg8VunYUAMw/JAOLf2SIJ8cA75IdJMp0UJXoMcOfVKkVdgMbGi7BHtJ3ZFwIajbNdWoNQV0k/LlwwmqGYGkh71wBX4WEdW6MqvvT8Mt/xyA6xzflqnMzgvQPIe6v29FETR9wxSKG52oCOY/DtPXEo+DgZvQwQn3F4BwX+GZawHbbMjWCIDxoSmph5TfZMzF0EkhEi47Uk93FPgiBQPjKSYMxnJ9Lo9UwZjsxdtk7ONKe1GIxfHdx3no4uuRp8WKqjpz017VBOWubdXPxO8Q8QOv6nT9faVVCMUQApehFlg== }
//@[04:0045) |       ├─FunctionCallExpression { Name = length }
//@[11:0044) |       | └─SynthesizedVariableReferenceExpression { Name = $fxv#11 }
    sys.length(loadFileAsBase64('Assets/binary'))
//@[15:0048) ├─DeclaredVariableExpression { Name = $fxv#12 }
//@[15:0048) | └─StringLiteralExpression { Value = g8fCjQHuFUfnHlTUHxe3qmjeM3HlYSToV7qTGrcJ6vgFNjvgpmxnexFbzjJV/Ejx8jXKa8L32YUn1f/HUnPY6u5c1SaGaP8OiVyRK4ef52hOtc3Yd29c9ubDsLohwLlmuiQDCvVduNMejR6eZy50ti3eYaLu3e04IKC7kTFO0Ph/vSIhlfkS6lUB9e7EJuHAa+3yJFn0uIVFs/BF67fNV9zwx92XyhFL8tmv3IZNd1+0cAby99+zif6iBPXcJ5XTUUz4UHaPmZLPT75hd4iGZzOk/I+FAsUTxRDra76D+sSXay7qBv5TyVLlhs7kqSVAecki6vQG0Siku64tl3PKKEy9JV1lHItgg/IFDYNd8/DKMUpEi90wunW/CfTpQcctzHzZFjl5euswaXgTDvVt2KRHPpi3likE8b1GuyKFVfKNT47VFGSabuUZlhDzbzx7qECnIpA1M7kH+TUHhGTe3ezmmPq+EO6jybYNMpMs+7gcfYAEtzE4gfpubKHLQI8ZYFKFxPCo0ypOwh4Z1nStJkcrNX2UlSDFfPb+LlCaGRxRKPN9md+2sr4x6qm0TptmI9o8wJF7FvqJUS2obFz2KhnfdKg7seuknpisasENchzEO2hoMtpCf5Lt3ZwsLCnFrllFaNmV2BWfA0k74f5MykjIioppM8ajdzORDtjTv/WcNyxdVlTV6nr2Oe43WFKZT0DFMDGHVgTZRBxVH5JtfT1akurR+IyvTegR6kSMHXSWlE1iEoPK8gdNpFLHdAJ8VwPnMGRC8CoCGzDeAakmwiHuecPOzcg9cOQe2Rlo68kJSr6q2hEcTmm9kPj7vfAeocqlosDd2Ci7xcBAJNb/rC4IVllhZPyqt2C8L1VbN5wZfBNgwpA73oOX0kxIKoCqH+Ni167WvC1ZwTqlVAWeAa1RiSplisinKBwDXahu2qDhVJyOt/RwV8Y+W3ynFGXYOW9BDwXVwKUm2zrKl6j0Y1AUTH5HArfCwwThXW2ZFslbr/fM9nJPAbbxpDY2FQBAlt8ST3PyLrFBfXlsV0JqVhidnw94NAbTiPqck15pTGbncg8VunYUAMw/JAOLf2SIJ8cA75IdJMp0UJXoMcOfVKkVdgMbGi7BHtJ3ZFwIajbNdWoNQV0k/LlwwmqGYGkh71wBX4WEdW6MqvvT8Mt/xyA6xzflqnMzgvQPIe6v29FETR9wxSKG52oCOY/DtPXEo+DgZvQwQn3F4BwX+GZawHbbMjWCIDxoSmph5TfZMzF0EkhEi47Uk93FPgiBQPjKSYMxnJ9Lo9UwZjsxdtk7ONKe1GIxfHdx3no4uuRp8WKqjpz017VBOWubdXPxO8Q8QOv6nT9faVVCMUQApehFlg== }
//@[04:0049) |       └─FunctionCallExpression { Name = length }
//@[15:0048) |         └─SynthesizedVariableReferenceExpression { Name = $fxv#12 }
  ]
}


module module1 'modulea.bicep' = {
//@[00:0127) ├─DeclaredModuleExpression
//@[33:0127) | ├─ObjectExpression
  name: 'module1'
//@[02:0017) | | └─ObjectPropertyExpression
//@[02:0006) | |   ├─StringLiteralExpression { Value = name }
//@[08:0017) | |   └─StringLiteralExpression { Value = module1 }
  params: {
//@[10:0069) | └─ObjectExpression
    text: loadTextContent('Assets/TextFile.LF.txt')
//@[10:0051) ├─DeclaredVariableExpression { Name = $fxv#13 }
//@[10:0051) | └─StringLiteralExpression { Value = Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n Donec laoreet sem tortor, ut dignissim ipsum ornare vel.\n  Duis ac ipsum turpis.\n\tMaecenas at condimentum dui.\n Suspendisse aliquet efficitur iaculis.\nIn hac habitasse platea dictumst.\nEtiam consectetur ut libero ac lobortis.\n\tNullam vitae auctor massa.\nFusce tincidunt urna purus, sit amet.\n }
//@[04:0051) |   └─ObjectPropertyExpression
//@[04:0008) |     ├─StringLiteralExpression { Value = text }
//@[10:0051) |     └─SynthesizedVariableReferenceExpression { Name = $fxv#13 }
  }
}

module module2 'modulea.bicep' = {
//@[00:0119) ├─DeclaredModuleExpression
//@[33:0119) | ├─ObjectExpression
  name: 'module2'
//@[02:0017) | | └─ObjectPropertyExpression
//@[02:0006) | |   ├─StringLiteralExpression { Value = name }
//@[08:0017) | |   └─StringLiteralExpression { Value = module2 }
  params: {
//@[10:0061) | └─ObjectExpression
    text: loadFileAsBase64('Assets/binary')
//@[10:0043) ├─DeclaredVariableExpression { Name = $fxv#14 }
//@[10:0043) | └─StringLiteralExpression { Value = g8fCjQHuFUfnHlTUHxe3qmjeM3HlYSToV7qTGrcJ6vgFNjvgpmxnexFbzjJV/Ejx8jXKa8L32YUn1f/HUnPY6u5c1SaGaP8OiVyRK4ef52hOtc3Yd29c9ubDsLohwLlmuiQDCvVduNMejR6eZy50ti3eYaLu3e04IKC7kTFO0Ph/vSIhlfkS6lUB9e7EJuHAa+3yJFn0uIVFs/BF67fNV9zwx92XyhFL8tmv3IZNd1+0cAby99+zif6iBPXcJ5XTUUz4UHaPmZLPT75hd4iGZzOk/I+FAsUTxRDra76D+sSXay7qBv5TyVLlhs7kqSVAecki6vQG0Siku64tl3PKKEy9JV1lHItgg/IFDYNd8/DKMUpEi90wunW/CfTpQcctzHzZFjl5euswaXgTDvVt2KRHPpi3likE8b1GuyKFVfKNT47VFGSabuUZlhDzbzx7qECnIpA1M7kH+TUHhGTe3ezmmPq+EO6jybYNMpMs+7gcfYAEtzE4gfpubKHLQI8ZYFKFxPCo0ypOwh4Z1nStJkcrNX2UlSDFfPb+LlCaGRxRKPN9md+2sr4x6qm0TptmI9o8wJF7FvqJUS2obFz2KhnfdKg7seuknpisasENchzEO2hoMtpCf5Lt3ZwsLCnFrllFaNmV2BWfA0k74f5MykjIioppM8ajdzORDtjTv/WcNyxdVlTV6nr2Oe43WFKZT0DFMDGHVgTZRBxVH5JtfT1akurR+IyvTegR6kSMHXSWlE1iEoPK8gdNpFLHdAJ8VwPnMGRC8CoCGzDeAakmwiHuecPOzcg9cOQe2Rlo68kJSr6q2hEcTmm9kPj7vfAeocqlosDd2Ci7xcBAJNb/rC4IVllhZPyqt2C8L1VbN5wZfBNgwpA73oOX0kxIKoCqH+Ni167WvC1ZwTqlVAWeAa1RiSplisinKBwDXahu2qDhVJyOt/RwV8Y+W3ynFGXYOW9BDwXVwKUm2zrKl6j0Y1AUTH5HArfCwwThXW2ZFslbr/fM9nJPAbbxpDY2FQBAlt8ST3PyLrFBfXlsV0JqVhidnw94NAbTiPqck15pTGbncg8VunYUAMw/JAOLf2SIJ8cA75IdJMp0UJXoMcOfVKkVdgMbGi7BHtJ3ZFwIajbNdWoNQV0k/LlwwmqGYGkh71wBX4WEdW6MqvvT8Mt/xyA6xzflqnMzgvQPIe6v29FETR9wxSKG52oCOY/DtPXEo+DgZvQwQn3F4BwX+GZawHbbMjWCIDxoSmph5TfZMzF0EkhEi47Uk93FPgiBQPjKSYMxnJ9Lo9UwZjsxdtk7ONKe1GIxfHdx3no4uuRp8WKqjpz017VBOWubdXPxO8Q8QOv6nT9faVVCMUQApehFlg== }
//@[04:0043) |   └─ObjectPropertyExpression
//@[04:0008) |     ├─StringLiteralExpression { Value = text }
//@[10:0043) |     └─SynthesizedVariableReferenceExpression { Name = $fxv#14 }
  }
}

var textFileInSubdirectories = loadTextContent('Assets/../Assets/path/../path/../../Assets/path/to/deep/file/../../../to/deep/file/TextFile.txt')
//@[00:0145) ├─DeclaredVariableExpression { Name = textFileInSubdirectories }
//@[31:0145) | └─StringLiteralExpression { Value = Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n Donec laoreet sem tortor, ut dignissim ipsum ornare vel.\n  Duis ac ipsum turpis.\n\tMaecenas at condimentum dui.\n Suspendisse aliquet efficitur iaculis.\nIn hac habitasse platea dictumst.\nEtiam consectetur ut libero ac lobortis.\n\tNullam vitae auctor massa.\nFusce tincidunt urna purus, sit amet.\n }
var binaryFileInSubdirectories = loadFileAsBase64('Assets/../Assets/path/../path/../../Assets/path/to/deep/file/../../../to/deep/file/binary')
//@[00:0142) ├─DeclaredVariableExpression { Name = binaryFileInSubdirectories }
//@[33:0142) | └─StringLiteralExpression { Value = g8fCjQHuFUfnHlTUHxe3qmjeM3HlYSToV7qTGrcJ6vgFNjvgpmxnexFbzjJV/Ejx8jXKa8L32YUn1f/HUnPY6u5c1SaGaP8OiVyRK4ef52hOtc3Yd29c9ubDsLohwLlmuiQDCvVduNMejR6eZy50ti3eYaLu3e04IKC7kTFO0Ph/vSIhlfkS6lUB9e7EJuHAa+3yJFn0uIVFs/BF67fNV9zwx92XyhFL8tmv3IZNd1+0cAby99+zif6iBPXcJ5XTUUz4UHaPmZLPT75hd4iGZzOk/I+FAsUTxRDra76D+sSXay7qBv5TyVLlhs7kqSVAecki6vQG0Siku64tl3PKKEy9JV1lHItgg/IFDYNd8/DKMUpEi90wunW/CfTpQcctzHzZFjl5euswaXgTDvVt2KRHPpi3likE8b1GuyKFVfKNT47VFGSabuUZlhDzbzx7qECnIpA1M7kH+TUHhGTe3ezmmPq+EO6jybYNMpMs+7gcfYAEtzE4gfpubKHLQI8ZYFKFxPCo0ypOwh4Z1nStJkcrNX2UlSDFfPb+LlCaGRxRKPN9md+2sr4x6qm0TptmI9o8wJF7FvqJUS2obFz2KhnfdKg7seuknpisasENchzEO2hoMtpCf5Lt3ZwsLCnFrllFaNmV2BWfA0k74f5MykjIioppM8ajdzORDtjTv/WcNyxdVlTV6nr2Oe43WFKZT0DFMDGHVgTZRBxVH5JtfT1akurR+IyvTegR6kSMHXSWlE1iEoPK8gdNpFLHdAJ8VwPnMGRC8CoCGzDeAakmwiHuecPOzcg9cOQe2Rlo68kJSr6q2hEcTmm9kPj7vfAeocqlosDd2Ci7xcBAJNb/rC4IVllhZPyqt2C8L1VbN5wZfBNgwpA73oOX0kxIKoCqH+Ni167WvC1ZwTqlVAWeAa1RiSplisinKBwDXahu2qDhVJyOt/RwV8Y+W3ynFGXYOW9BDwXVwKUm2zrKl6j0Y1AUTH5HArfCwwThXW2ZFslbr/fM9nJPAbbxpDY2FQBAlt8ST3PyLrFBfXlsV0JqVhidnw94NAbTiPqck15pTGbncg8VunYUAMw/JAOLf2SIJ8cA75IdJMp0UJXoMcOfVKkVdgMbGi7BHtJ3ZFwIajbNdWoNQV0k/LlwwmqGYGkh71wBX4WEdW6MqvvT8Mt/xyA6xzflqnMzgvQPIe6v29FETR9wxSKG52oCOY/DtPXEo+DgZvQwQn3F4BwX+GZawHbbMjWCIDxoSmph5TfZMzF0EkhEi47Uk93FPgiBQPjKSYMxnJ9Lo9UwZjsxdtk7ONKe1GIxfHdx3no4uuRp8WKqjpz017VBOWubdXPxO8Q8QOv6nT9faVVCMUQApehFlg== }

var loadWithEncoding01 = loadTextContent('Assets/encoding-iso.txt', 'iso-8859-1')
//@[00:0081) ├─DeclaredVariableExpression { Name = loadWithEncoding01 }
//@[25:0081) | └─StringLiteralExpression { Value = ¡\t\tinverted exclamation mark\n¢\t\tcent\n£\t\tpound\n¤\t\tcurrency\n¥\t\tyen\n¦\t\tbroken vertical bar\n§\t\tsection\n¨\t\tspacing diaeresis\n©\t\tcopyright\nª\t\tfeminine ordinal indicator\n«\t\tangle quotation mark (left)\n¬\t\tnegation\n­\t\tsoft hyphen\n®\t\tregistered trademark\n¯\t\tspacing macron\n°\t\tdegree\n±\t\tplus-or-minus\n²\t\tsuperscript 2\n³\t\tsuperscript 3\n´\t\tspacing acute\n¶\t\tparagraph\n·\t\tmiddle dot\n¸\t\tspacing cedilla\n¹\t\tsuperscript 1\nº\t\tmasculine ordinal indicator\n»\t\tangle quotation mark (right)\n¼\t\tfraction 1/4\n½\t\tfraction 1/2\n¾\t\tfraction 3/4\n¿\t\tinverted question mark\nÀ\t\tcapital a, grave accent\nÁ\t\tcapital a, acute accent\nÂ\t\tcapital a, circumflex accent\nÃ\t\tcapital a, tilde\nÄ\t\tcapital a, umlaut mark\nÅ\t\tcapital a, ring\nÆ\t\tcapital ae\nÇ\t\tcapital c, cedilla\nÈ\t\tcapital e, grave accent\nÉ\t\tcapital e, acute accent\nÊ\t\tcapital e, circumflex accent\nË\t\tcapital e, umlaut mark\nÌ\t\tcapital i, grave accent\nÍ\t\tcapital i, acute accent\nÎ\t\tcapital i, circumflex accent\nÏ\t\tcapital i, umlaut mark\nÐ\t\tcapital eth, Icelandic\nÑ\t\tcapital n, tilde\nÒ\t\tcapital o, grave accent\nÓ\t\tcapital o, acute accent\nÔ\t\tcapital o, circumflex accent\nÕ\t\tcapital o, tilde\nÖ\t\tcapital o, umlaut mark\n×\t\tmultiplication\nØ\t\tcapital o, slash\nÙ\t\tcapital u, grave accent\nÚ\t\tcapital u, acute accent\nÛ\t\tcapital u, circumflex accent\nÜ\t\tcapital u, umlaut mark\nÝ\t\tcapital y, acute accent\nÞ\t\tcapital THORN, Icelandic\nß\t\tsmall sharp s, German\nà\t\tsmall a, grave accent\ná\t\tsmall a, acute accent\nâ\t\tsmall a, circumflex accent\nã\t\tsmall a, tilde\nä\t\tsmall a, umlaut mark\nå\t\tsmall a, ring\næ\t\tsmall ae\nç\t\tsmall c, cedilla\nè\t\tsmall e, grave accent\né\t\tsmall e, acute accent\nê\t\tsmall e, circumflex accent\në\t\tsmall e, umlaut mark\nì\t\tsmall i, grave accent\ní\t\tsmall i, acute accent\nî\t\tsmall i, circumflex accent\nï\t\tsmall i, umlaut mark\nð\t\tsmall eth, Icelandic\nñ\t\tsmall n, tilde\nò\t\tsmall o, grave accent\nó\t\tsmall o, acute accent\nô\t\tsmall o, circumflex accent\nõ\t\tsmall o, tilde\nö\t\tsmall o, umlaut mark\n÷\t\tdivision\nø\t\tsmall o, slash\nù\t\tsmall u, grave accent\nú\t\tsmall u, acute accent\nû\t\tsmall u, circumflex accent\nü\t\tsmall u, umlaut mark\ný\t\tsmall y, acute accent\nþ\t\tsmall thorn, Icelandic\nÿ\t\tsmall y, umlaut mark\n }
var loadWithEncoding06 = loadTextContent('Assets/encoding-ascii.txt', 'us-ascii')
//@[00:0081) ├─DeclaredVariableExpression { Name = loadWithEncoding06 }
//@[25:0081) | └─StringLiteralExpression { Value = 32 = \n33 = !\n34 = "\n35 = #\n36 = $\n37 = %\n38 = &\n39 = '\n40 = (\n41 = )\n42 = *\n43 = +\n44 = ,\n45 = -\n46 = .\n47 = /\n48 = 0\n49 = 1\n50 = 2\n51 = 3\n52 = 4\n53 = 5\n54 = 6\n55 = 7\n56 = 8\n57 = 9\n58 = :\n59 = ;\n60 = <\n61 = =\n62 = >\n63 = ?\n64 = @\n65 = A\n66 = B\n67 = C\n68 = D\n69 = E\n70 = F\n71 = G\n72 = H\n73 = I\n74 = J\n75 = K\n76 = L\n77 = M\n78 = N\n79 = O\n80 = P\n81 = Q\n82 = R\n83 = S\n84 = T\n85 = U\n86 = V\n87 = W\n88 = X\n89 = Y\n90 = Z\n91 = [\n92 = \\n93 = ]\n94 = ^\n95 = _\n96 = `\n97 = a\n98 = b\n99 = c\n100 = d\n101 = e\n102 = f\n103 = g\n104 = h\n105 = i\n106 = j\n107 = k\n108 = l\n109 = m\n110 = n\n111 = o\n112 = p\n113 = q\n114 = r\n115 = s\n116 = t\n117 = u\n118 = v\n119 = w\n120 = x\n121 = y\n122 = z\n123 = {\n124 = |\n125 = }\n126 = ~\n }
var loadWithEncoding07 = loadTextContent('Assets/encoding-ascii.txt', 'iso-8859-1')
//@[00:0083) ├─DeclaredVariableExpression { Name = loadWithEncoding07 }
//@[25:0083) | └─StringLiteralExpression { Value = 32 = \n33 = !\n34 = "\n35 = #\n36 = $\n37 = %\n38 = &\n39 = '\n40 = (\n41 = )\n42 = *\n43 = +\n44 = ,\n45 = -\n46 = .\n47 = /\n48 = 0\n49 = 1\n50 = 2\n51 = 3\n52 = 4\n53 = 5\n54 = 6\n55 = 7\n56 = 8\n57 = 9\n58 = :\n59 = ;\n60 = <\n61 = =\n62 = >\n63 = ?\n64 = @\n65 = A\n66 = B\n67 = C\n68 = D\n69 = E\n70 = F\n71 = G\n72 = H\n73 = I\n74 = J\n75 = K\n76 = L\n77 = M\n78 = N\n79 = O\n80 = P\n81 = Q\n82 = R\n83 = S\n84 = T\n85 = U\n86 = V\n87 = W\n88 = X\n89 = Y\n90 = Z\n91 = [\n92 = \\n93 = ]\n94 = ^\n95 = _\n96 = `\n97 = a\n98 = b\n99 = c\n100 = d\n101 = e\n102 = f\n103 = g\n104 = h\n105 = i\n106 = j\n107 = k\n108 = l\n109 = m\n110 = n\n111 = o\n112 = p\n113 = q\n114 = r\n115 = s\n116 = t\n117 = u\n118 = v\n119 = w\n120 = x\n121 = y\n122 = z\n123 = {\n124 = |\n125 = }\n126 = ~\n }
var loadWithEncoding08 = loadTextContent('Assets/encoding-ascii.txt', 'utf-8')
//@[00:0078) ├─DeclaredVariableExpression { Name = loadWithEncoding08 }
//@[25:0078) | └─StringLiteralExpression { Value = 32 = \n33 = !\n34 = "\n35 = #\n36 = $\n37 = %\n38 = &\n39 = '\n40 = (\n41 = )\n42 = *\n43 = +\n44 = ,\n45 = -\n46 = .\n47 = /\n48 = 0\n49 = 1\n50 = 2\n51 = 3\n52 = 4\n53 = 5\n54 = 6\n55 = 7\n56 = 8\n57 = 9\n58 = :\n59 = ;\n60 = <\n61 = =\n62 = >\n63 = ?\n64 = @\n65 = A\n66 = B\n67 = C\n68 = D\n69 = E\n70 = F\n71 = G\n72 = H\n73 = I\n74 = J\n75 = K\n76 = L\n77 = M\n78 = N\n79 = O\n80 = P\n81 = Q\n82 = R\n83 = S\n84 = T\n85 = U\n86 = V\n87 = W\n88 = X\n89 = Y\n90 = Z\n91 = [\n92 = \\n93 = ]\n94 = ^\n95 = _\n96 = `\n97 = a\n98 = b\n99 = c\n100 = d\n101 = e\n102 = f\n103 = g\n104 = h\n105 = i\n106 = j\n107 = k\n108 = l\n109 = m\n110 = n\n111 = o\n112 = p\n113 = q\n114 = r\n115 = s\n116 = t\n117 = u\n118 = v\n119 = w\n120 = x\n121 = y\n122 = z\n123 = {\n124 = |\n125 = }\n126 = ~\n }
var loadWithEncoding11 = loadTextContent('Assets/encoding-utf8.txt', 'utf-8')
//@[00:0077) ├─DeclaredVariableExpression { Name = loadWithEncoding11 }
//@[25:0077) | └─StringLiteralExpression { Value = 💪😊😈🍕☕\r\n🐱‍👤\r\n\r\n朝辞白帝彩云间\r\n千里江陵一日还\r\n两岸猿声啼不住\r\n轻舟已过万重山\r\n\r\nΠ π Φ φ\r\n\r\n😎\r\n\r\nαα\r\nΩω\r\nΘ  \r\n\r\nZażółć gęślą jaźń\r\n\r\náéóúñü - ¡Hola!\r\n\r\n二头肌二头肌\r\n\r\n\r\n二头肌\r\nΘ二头肌α\r\n\r\n𐐷\r\n\u{10437}\r\n\u{D801}\u{DC37}\r\n\r\n❆ Hello\u{20}World\u{21} ❁\r\n\r\n\ta\tb\tc\td\te\tf\tg\th\t\r\n8\t♜\t♞\t♝\t♛\t♚\t♝\t♞\t♜\t8\r\n7\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t7\r\n6\t\t\t\t\t\t\t\t\t6\r\n5\t\t\t\t\t\t\t\t\t5\r\n4\t\t\t\t\t\t\t\t\t4\r\n3\t\t\t\t\t\t\t\t\t3\r\n2\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t2\r\n1\t♖\t♘\t♗\t♕\t♔\t♗\t♘\t♖\t1\r\n\ta\tb\tc\td\te\tf\tg\th\r\n }
var loadWithEncoding12 = loadTextContent('Assets/encoding-utf8-bom.txt', 'utf-8')
//@[00:0081) ├─DeclaredVariableExpression { Name = loadWithEncoding12 }
//@[25:0081) | └─StringLiteralExpression { Value = 💪😊😈🍕☕\r\n🐱‍👤\r\n\r\n朝辞白帝彩云间\r\n千里江陵一日还\r\n两岸猿声啼不住\r\n轻舟已过万重山\r\n\r\nΠ π Φ φ\r\n\r\n😎\r\n\r\nαα\r\nΩω\r\nΘ  \r\n\r\nZażółć gęślą jaźń\r\n\r\náéóúñü - ¡Hola!\r\n\r\n二头肌二头肌\r\n\r\n\r\n二头肌\r\nΘ二头肌α\r\n\r\n𐐷\r\n\u{10437}\r\n\u{D801}\u{DC37}\r\n\r\n❆ Hello\u{20}World\u{21} ❁\r\n\r\n\ta\tb\tc\td\te\tf\tg\th\t\r\n8\t♜\t♞\t♝\t♛\t♚\t♝\t♞\t♜\t8\r\n7\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t7\r\n6\t\t\t\t\t\t\t\t\t6\r\n5\t\t\t\t\t\t\t\t\t5\r\n4\t\t\t\t\t\t\t\t\t4\r\n3\t\t\t\t\t\t\t\t\t3\r\n2\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t2\r\n1\t♖\t♘\t♗\t♕\t♔\t♗\t♘\t♖\t1\r\n\ta\tb\tc\td\te\tf\tg\th\r\n }

var testJson = json(loadTextContent('./Assets/test.json.txt'))
//@[20:0061) ├─DeclaredVariableExpression { Name = $fxv#15 }
//@[20:0061) | └─StringLiteralExpression { Value = {\n    "string": "someVal",\n    "int": 123,\n    "array": [\n        1,\n        //comment\n        2\n/* multi\n    line\n    comment\n*/\n    ],\n/* multi\n    line\n    comment\n*/\n    "object": {\n        "nestedString": "someVal" //comment\n    }\n}\n }
//@[00:0062) ├─DeclaredVariableExpression { Name = testJson }
//@[15:0062) | └─FunctionCallExpression { Name = json }
//@[20:0061) |   └─SynthesizedVariableReferenceExpression { Name = $fxv#15 }
var testJsonString = testJson.string
//@[00:0036) ├─DeclaredVariableExpression { Name = testJsonString }
//@[21:0036) | └─PropertyAccessExpression { PropertyName = string }
//@[21:0029) |   └─VariableReferenceExpression { Variable = testJson }
var testJsonInt = testJson.int
//@[00:0030) ├─DeclaredVariableExpression { Name = testJsonInt }
//@[18:0030) | └─PropertyAccessExpression { PropertyName = int }
//@[18:0026) |   └─VariableReferenceExpression { Variable = testJson }
var testJsonArrayVal = testJson.array[0]
//@[00:0040) ├─DeclaredVariableExpression { Name = testJsonArrayVal }
//@[23:0040) | └─ArrayAccessExpression
//@[38:0039) |   ├─IntegerLiteralExpression { Value = 0 }
//@[23:0037) |   └─PropertyAccessExpression { PropertyName = array }
//@[23:0031) |     └─VariableReferenceExpression { Variable = testJson }
var testJsonObject = testJson.object
//@[00:0036) ├─DeclaredVariableExpression { Name = testJsonObject }
//@[21:0036) | └─PropertyAccessExpression { PropertyName = object }
//@[21:0029) |   └─VariableReferenceExpression { Variable = testJson }
var testJsonNestedString = testJson.object.nestedString
//@[00:0055) ├─DeclaredVariableExpression { Name = testJsonNestedString }
//@[27:0055) | └─PropertyAccessExpression { PropertyName = nestedString }
//@[27:0042) |   └─PropertyAccessExpression { PropertyName = object }
//@[27:0035) |     └─VariableReferenceExpression { Variable = testJson }

var testJson2 = loadJsonContent('./Assets/test.json.txt')
//@[16:0057) ├─DeclaredVariableExpression { Name = $fxv#16 }
//@[00:0057) ├─DeclaredVariableExpression { Name = testJson2 }
//@[16:0057) | └─SynthesizedVariableReferenceExpression { Name = $fxv#16 }
var testJsonString2 = testJson.string
//@[00:0037) ├─DeclaredVariableExpression { Name = testJsonString2 }
//@[22:0037) | └─PropertyAccessExpression { PropertyName = string }
//@[22:0030) |   └─VariableReferenceExpression { Variable = testJson }
var testJsonString2_1 = loadJsonContent('./Assets/test.json.txt', '.string')
//@[24:0076) ├─DeclaredVariableExpression { Name = $fxv#17 }
//@[00:0076) ├─DeclaredVariableExpression { Name = testJsonString2_1 }
//@[24:0076) | └─SynthesizedVariableReferenceExpression { Name = $fxv#17 }
var testJsonInt2 = testJson.int
//@[00:0031) ├─DeclaredVariableExpression { Name = testJsonInt2 }
//@[19:0031) | └─PropertyAccessExpression { PropertyName = int }
//@[19:0027) |   └─VariableReferenceExpression { Variable = testJson }
var testJsonInt2_1 = loadJsonContent('./Assets/test.json.txt', '.int')
//@[21:0070) ├─DeclaredVariableExpression { Name = $fxv#18 }
//@[00:0070) ├─DeclaredVariableExpression { Name = testJsonInt2_1 }
//@[21:0070) | └─SynthesizedVariableReferenceExpression { Name = $fxv#18 }
var testJsonArrayVal2 = testJson.array[0]
//@[00:0041) ├─DeclaredVariableExpression { Name = testJsonArrayVal2 }
//@[24:0041) | └─ArrayAccessExpression
//@[39:0040) |   ├─IntegerLiteralExpression { Value = 0 }
//@[24:0038) |   └─PropertyAccessExpression { PropertyName = array }
//@[24:0032) |     └─VariableReferenceExpression { Variable = testJson }
var testJsonArrayVal2_1 = loadJsonContent('./Assets/test.json.txt', '.array[0]')
//@[26:0080) ├─DeclaredVariableExpression { Name = $fxv#19 }
//@[00:0080) ├─DeclaredVariableExpression { Name = testJsonArrayVal2_1 }
//@[26:0080) | └─SynthesizedVariableReferenceExpression { Name = $fxv#19 }
var testJsonObject2 = testJson.object
//@[00:0037) ├─DeclaredVariableExpression { Name = testJsonObject2 }
//@[22:0037) | └─PropertyAccessExpression { PropertyName = object }
//@[22:0030) |   └─VariableReferenceExpression { Variable = testJson }
var testJsonObject2_1 = loadJsonContent('./Assets/test.json.txt', '.object')
//@[24:0076) ├─DeclaredVariableExpression { Name = $fxv#20 }
//@[00:0076) ├─DeclaredVariableExpression { Name = testJsonObject2_1 }
//@[24:0076) | └─SynthesizedVariableReferenceExpression { Name = $fxv#20 }
var testJsonNestedString2 = testJson.object.nestedString
//@[00:0056) ├─DeclaredVariableExpression { Name = testJsonNestedString2 }
//@[28:0056) | └─PropertyAccessExpression { PropertyName = nestedString }
//@[28:0043) |   └─PropertyAccessExpression { PropertyName = object }
//@[28:0036) |     └─VariableReferenceExpression { Variable = testJson }
var testJsonNestedString2_1 = testJsonObject2_1.nestedString
//@[00:0060) ├─DeclaredVariableExpression { Name = testJsonNestedString2_1 }
//@[30:0060) | └─PropertyAccessExpression { PropertyName = nestedString }
//@[30:0047) |   └─VariableReferenceExpression { Variable = testJsonObject2_1 }
var testJsonNestedString2_2 = loadJsonContent('./Assets/test.json.txt', '.object.nestedString')
//@[30:0095) ├─DeclaredVariableExpression { Name = $fxv#21 }
//@[00:0095) ├─DeclaredVariableExpression { Name = testJsonNestedString2_2 }
//@[30:0095) | └─SynthesizedVariableReferenceExpression { Name = $fxv#21 }

var testJsonTokensAsArray = loadJsonContent('./Assets/test2.json.txt', '.products[?(@.price > 3)].name')
//@[28:0104) ├─DeclaredVariableExpression { Name = $fxv#22 }
//@[00:0104) ├─DeclaredVariableExpression { Name = testJsonTokensAsArray }
//@[28:0104) | └─SynthesizedVariableReferenceExpression { Name = $fxv#22 }

var testYaml = loadYamlContent('./Assets/test.yaml.txt')
//@[15:0056) ├─DeclaredVariableExpression { Name = $fxv#23 }
//@[00:0056) ├─DeclaredVariableExpression { Name = testYaml }
//@[15:0056) | └─SynthesizedVariableReferenceExpression { Name = $fxv#23 }
var testYamlString = testYaml.string
//@[00:0036) ├─DeclaredVariableExpression { Name = testYamlString }
//@[21:0036) | └─PropertyAccessExpression { PropertyName = string }
//@[21:0029) |   └─VariableReferenceExpression { Variable = testYaml }
var testYamlInt = testYaml.int
//@[00:0030) ├─DeclaredVariableExpression { Name = testYamlInt }
//@[18:0030) | └─PropertyAccessExpression { PropertyName = int }
//@[18:0026) |   └─VariableReferenceExpression { Variable = testYaml }
var testYamlBool = testYaml.bool
//@[00:0032) ├─DeclaredVariableExpression { Name = testYamlBool }
//@[19:0032) | └─PropertyAccessExpression { PropertyName = bool }
//@[19:0027) |   └─VariableReferenceExpression { Variable = testYaml }
var testYamlArrayInt = testYaml.arrayInt
//@[00:0040) ├─DeclaredVariableExpression { Name = testYamlArrayInt }
//@[23:0040) | └─PropertyAccessExpression { PropertyName = arrayInt }
//@[23:0031) |   └─VariableReferenceExpression { Variable = testYaml }
var testYamlArrayIntVal = testYaml.arrayInt[0]
//@[00:0046) ├─DeclaredVariableExpression { Name = testYamlArrayIntVal }
//@[26:0046) | └─ArrayAccessExpression
//@[44:0045) |   ├─IntegerLiteralExpression { Value = 0 }
//@[26:0043) |   └─PropertyAccessExpression { PropertyName = arrayInt }
//@[26:0034) |     └─VariableReferenceExpression { Variable = testYaml }
var testYamlArrayString = testYaml.arrayString
//@[00:0046) ├─DeclaredVariableExpression { Name = testYamlArrayString }
//@[26:0046) | └─PropertyAccessExpression { PropertyName = arrayString }
//@[26:0034) |   └─VariableReferenceExpression { Variable = testYaml }
var testYamlArrayStringVal = testYaml.arrayString[0]
//@[00:0052) ├─DeclaredVariableExpression { Name = testYamlArrayStringVal }
//@[29:0052) | └─ArrayAccessExpression
//@[50:0051) |   ├─IntegerLiteralExpression { Value = 0 }
//@[29:0049) |   └─PropertyAccessExpression { PropertyName = arrayString }
//@[29:0037) |     └─VariableReferenceExpression { Variable = testYaml }
var testYamlArrayBool = testYaml.arrayBool
//@[00:0042) ├─DeclaredVariableExpression { Name = testYamlArrayBool }
//@[24:0042) | └─PropertyAccessExpression { PropertyName = arrayBool }
//@[24:0032) |   └─VariableReferenceExpression { Variable = testYaml }
var testYamlArrayBoolVal = testYaml.arrayBool[0]
//@[00:0048) ├─DeclaredVariableExpression { Name = testYamlArrayBoolVal }
//@[27:0048) | └─ArrayAccessExpression
//@[46:0047) |   ├─IntegerLiteralExpression { Value = 0 }
//@[27:0045) |   └─PropertyAccessExpression { PropertyName = arrayBool }
//@[27:0035) |     └─VariableReferenceExpression { Variable = testYaml }
var testYamlObject = testYaml.object
//@[00:0036) ├─DeclaredVariableExpression { Name = testYamlObject }
//@[21:0036) | └─PropertyAccessExpression { PropertyName = object }
//@[21:0029) |   └─VariableReferenceExpression { Variable = testYaml }
var testYamlObjectNestedString = testYaml.object.nestedString
//@[00:0061) ├─DeclaredVariableExpression { Name = testYamlObjectNestedString }
//@[33:0061) | └─PropertyAccessExpression { PropertyName = nestedString }
//@[33:0048) |   └─PropertyAccessExpression { PropertyName = object }
//@[33:0041) |     └─VariableReferenceExpression { Variable = testYaml }
var testYamlObjectNestedInt = testYaml.object.nestedInt
//@[00:0055) ├─DeclaredVariableExpression { Name = testYamlObjectNestedInt }
//@[30:0055) | └─PropertyAccessExpression { PropertyName = nestedInt }
//@[30:0045) |   └─PropertyAccessExpression { PropertyName = object }
//@[30:0038) |     └─VariableReferenceExpression { Variable = testYaml }
var testYamlObjectNestedBool = testYaml.object.nestedBool
//@[00:0057) ├─DeclaredVariableExpression { Name = testYamlObjectNestedBool }
//@[31:0057) | └─PropertyAccessExpression { PropertyName = nestedBool }
//@[31:0046) |   └─PropertyAccessExpression { PropertyName = object }
//@[31:0039) |     └─VariableReferenceExpression { Variable = testYaml }

output testYamlString string = testYamlString
//@[00:0045) ├─DeclaredOutputExpression { Name = testYamlString }
//@[22:0028) | ├─AmbientTypeReferenceExpression { Name = string }
//@[31:0045) | └─VariableReferenceExpression { Variable = testYamlString }
output testYamlInt int = testYamlInt
//@[00:0036) ├─DeclaredOutputExpression { Name = testYamlInt }
//@[19:0022) | ├─AmbientTypeReferenceExpression { Name = int }
//@[25:0036) | └─VariableReferenceExpression { Variable = testYamlInt }
output testYamlBool bool = testYamlBool
//@[00:0039) ├─DeclaredOutputExpression { Name = testYamlBool }
//@[20:0024) | ├─AmbientTypeReferenceExpression { Name = bool }
//@[27:0039) | └─VariableReferenceExpression { Variable = testYamlBool }
output testYamlArrayInt array = testYamlArrayInt
//@[00:0048) ├─DeclaredOutputExpression { Name = testYamlArrayInt }
//@[24:0029) | ├─AmbientTypeReferenceExpression { Name = array }
//@[32:0048) | └─VariableReferenceExpression { Variable = testYamlArrayInt }
output testYamlArrayIntVal int = testYamlArrayIntVal
//@[00:0052) ├─DeclaredOutputExpression { Name = testYamlArrayIntVal }
//@[27:0030) | ├─AmbientTypeReferenceExpression { Name = int }
//@[33:0052) | └─VariableReferenceExpression { Variable = testYamlArrayIntVal }
output testYamlArrayString array = testYamlArrayString
//@[00:0054) ├─DeclaredOutputExpression { Name = testYamlArrayString }
//@[27:0032) | ├─AmbientTypeReferenceExpression { Name = array }
//@[35:0054) | └─VariableReferenceExpression { Variable = testYamlArrayString }
output testYamlArrayStringVal string = testYamlArrayStringVal
//@[00:0061) ├─DeclaredOutputExpression { Name = testYamlArrayStringVal }
//@[30:0036) | ├─AmbientTypeReferenceExpression { Name = string }
//@[39:0061) | └─VariableReferenceExpression { Variable = testYamlArrayStringVal }
output testYamlArrayBool array = testYamlArrayBool
//@[00:0050) ├─DeclaredOutputExpression { Name = testYamlArrayBool }
//@[25:0030) | ├─AmbientTypeReferenceExpression { Name = array }
//@[33:0050) | └─VariableReferenceExpression { Variable = testYamlArrayBool }
output testYamlArrayBoolVal bool = testYamlArrayBoolVal
//@[00:0055) ├─DeclaredOutputExpression { Name = testYamlArrayBoolVal }
//@[28:0032) | ├─AmbientTypeReferenceExpression { Name = bool }
//@[35:0055) | └─VariableReferenceExpression { Variable = testYamlArrayBoolVal }
output testYamlObject object = testYamlObject
//@[00:0045) ├─DeclaredOutputExpression { Name = testYamlObject }
//@[22:0028) | ├─AmbientTypeReferenceExpression { Name = object }
//@[31:0045) | └─VariableReferenceExpression { Variable = testYamlObject }
output testYamlObjectNestedString string = testYamlObjectNestedString
//@[00:0069) ├─DeclaredOutputExpression { Name = testYamlObjectNestedString }
//@[34:0040) | ├─AmbientTypeReferenceExpression { Name = string }
//@[43:0069) | └─VariableReferenceExpression { Variable = testYamlObjectNestedString }
output testYamlObjectNestedInt int = testYamlObjectNestedInt
//@[00:0060) ├─DeclaredOutputExpression { Name = testYamlObjectNestedInt }
//@[31:0034) | ├─AmbientTypeReferenceExpression { Name = int }
//@[37:0060) | └─VariableReferenceExpression { Variable = testYamlObjectNestedInt }
output testYamlObjectNestedBool bool = testYamlObjectNestedBool
//@[00:0063) └─DeclaredOutputExpression { Name = testYamlObjectNestedBool }
//@[32:0036)   ├─AmbientTypeReferenceExpression { Name = bool }
//@[39:0063)   └─VariableReferenceExpression { Variable = testYamlObjectNestedBool }

