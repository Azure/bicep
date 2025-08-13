var loadedText1 = loadTextContent('Assets/TextFile.CRLF.txt')
//@    "loadedText1": "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\r\n\tProin varius in nunc et laoreet.\r\n  Nam pulvinar ipsum sed lectus porttitor, at porttitor ipsum faucibus.\r\n  \tAliquam euismod, odio tincidunt convallis pulvinar, felis sem porttitor turpis, a condimentum dui erat nec tellus.\r\n  Duis elementum cursus est, congue efficitur risus.\r\n\tMauris sit amet.\r\nExcepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\r\n",
var loadedText2 = sys.loadTextContent('Assets/TextFile.LF.txt')
//@    "loadedText2": "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n Donec laoreet sem tortor, ut dignissim ipsum ornare vel.\n  Duis ac ipsum turpis.\n\tMaecenas at condimentum dui.\n Suspendisse aliquet efficitur iaculis.\nIn hac habitasse platea dictumst.\nEtiam consectetur ut libero ac lobortis.\n\tNullam vitae auctor massa.\nFusce tincidunt urna purus, sit amet.\n",
var loadedTextEncoding1 = loadTextContent('Assets/encoding-ascii.txt', 'us-ascii')
//@    "loadedTextEncoding1": "32 = \n33 = !\n34 = \"\n35 = #\n36 = $\n37 = %\n38 = &\n39 = '\n40 = (\n41 = )\n42 = *\n43 = +\n44 = ,\n45 = -\n46 = .\n47 = /\n48 = 0\n49 = 1\n50 = 2\n51 = 3\n52 = 4\n53 = 5\n54 = 6\n55 = 7\n56 = 8\n57 = 9\n58 = :\n59 = ;\n60 = <\n61 = =\n62 = >\n63 = ?\n64 = @\n65 = A\n66 = B\n67 = C\n68 = D\n69 = E\n70 = F\n71 = G\n72 = H\n73 = I\n74 = J\n75 = K\n76 = L\n77 = M\n78 = N\n79 = O\n80 = P\n81 = Q\n82 = R\n83 = S\n84 = T\n85 = U\n86 = V\n87 = W\n88 = X\n89 = Y\n90 = Z\n91 = [\n92 = \\\n93 = ]\n94 = ^\n95 = _\n96 = `\n97 = a\n98 = b\n99 = c\n100 = d\n101 = e\n102 = f\n103 = g\n104 = h\n105 = i\n106 = j\n107 = k\n108 = l\n109 = m\n110 = n\n111 = o\n112 = p\n113 = q\n114 = r\n115 = s\n116 = t\n117 = u\n118 = v\n119 = w\n120 = x\n121 = y\n122 = z\n123 = {\n124 = |\n125 = }\n126 = ~\n",
var loadedTextEncoding2 = loadTextContent('Assets/encoding-utf8.txt', 'utf-8')
//@    "loadedTextEncoding2": "💪😊😈🍕☕\r\n🐱‍👤\r\n\r\n朝辞白帝彩云间\r\n千里江陵一日还\r\n两岸猿声啼不住\r\n轻舟已过万重山\r\n\r\nΠ π Φ φ\r\n\r\n😎\r\n\r\nαα\r\nΩω\r\nΘ  \r\n\r\nZażółć gęślą jaźń\r\n\r\náéóúñü - ¡Hola!\r\n\r\n二头肌二头肌\r\n\r\n\r\n二头肌\r\nΘ二头肌α\r\n\r\n𐐷\r\n\\u{10437}\r\n\\u{D801}\\u{DC37}\r\n\r\n❆ Hello\\u{20}World\\u{21} ❁\r\n\r\n\ta\tb\tc\td\te\tf\tg\th\t\r\n8\t♜\t♞\t♝\t♛\t♚\t♝\t♞\t♜\t8\r\n7\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t7\r\n6\t\t\t\t\t\t\t\t\t6\r\n5\t\t\t\t\t\t\t\t\t5\r\n4\t\t\t\t\t\t\t\t\t4\r\n3\t\t\t\t\t\t\t\t\t3\r\n2\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t2\r\n1\t♖\t♘\t♗\t♕\t♔\t♗\t♘\t♖\t1\r\n\ta\tb\tc\td\te\tf\tg\th\r\n",
var loadedTextEncoding3 = loadTextContent('Assets/encoding-utf16.txt', 'utf-16')
//@    "loadedTextEncoding3": "💪😊😈🍕☕\r\n🐱‍👤\r\n\r\n朝辞白帝彩云间\r\n千里江陵一日还\r\n两岸猿声啼不住\r\n轻舟已过万重山\r\n\r\nΠ π Φ φ\r\n\r\n😎\r\n\r\nαα\r\nΩω\r\nΘ  \r\n\r\nZażółć gęślą jaźń\r\n\r\náéóúñü - ¡Hola!\r\n\r\n二头肌二头肌\r\n\r\n\r\n二头肌\r\nΘ二头肌α\r\n\r\n𐐷\r\n\\u{10437}\r\n\\u{D801}\\u{DC37}\r\n\r\n❆ Hello\\u{20}World\\u{21} ❁\r\n\r\n\ta\tb\tc\td\te\tf\tg\th\t\r\n8\t♜\t♞\t♝\t♛\t♚\t♝\t♞\t♜\t8\r\n7\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t7\r\n6\t\t\t\t\t\t\t\t\t6\r\n5\t\t\t\t\t\t\t\t\t5\r\n4\t\t\t\t\t\t\t\t\t4\r\n3\t\t\t\t\t\t\t\t\t3\r\n2\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t2\r\n1\t♖\t♘\t♗\t♕\t♔\t♗\t♘\t♖\t1\r\n\ta\tb\tc\td\te\tf\tg\th\r\n",
var loadedTextEncoding4 = loadTextContent('Assets/encoding-utf16be.txt', 'utf-16BE')
//@    "loadedTextEncoding4": "💪😊😈🍕☕\r\n🐱‍👤\r\n\r\n朝辞白帝彩云间\r\n千里江陵一日还\r\n两岸猿声啼不住\r\n轻舟已过万重山\r\n\r\nΠ π Φ φ\r\n\r\n😎\r\n\r\nαα\r\nΩω\r\nΘ  \r\n\r\nZażółć gęślą jaźń\r\n\r\náéóúñü - ¡Hola!\r\n\r\n二头肌二头肌\r\n\r\n\r\n二头肌\r\nΘ二头肌α\r\n\r\n𐐷\r\n\\u{10437}\r\n\\u{D801}\\u{DC37}\r\n\r\n❆ Hello\\u{20}World\\u{21} ❁\r\n\r\n\ta\tb\tc\td\te\tf\tg\th\t\r\n8\t♜\t♞\t♝\t♛\t♚\t♝\t♞\t♜\t8\r\n7\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t7\r\n6\t\t\t\t\t\t\t\t\t6\r\n5\t\t\t\t\t\t\t\t\t5\r\n4\t\t\t\t\t\t\t\t\t4\r\n3\t\t\t\t\t\t\t\t\t3\r\n2\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t2\r\n1\t♖\t♘\t♗\t♕\t♔\t♗\t♘\t♖\t1\r\n\ta\tb\tc\td\te\tf\tg\th\r\n",
var loadedTextEncoding5 = loadTextContent('Assets/encoding-iso.txt', 'iso-8859-1')
//@    "loadedTextEncoding5": "¡\t\tinverted exclamation mark\n¢\t\tcent\n£\t\tpound\n¤\t\tcurrency\n¥\t\tyen\n¦\t\tbroken vertical bar\n§\t\tsection\n¨\t\tspacing diaeresis\n©\t\tcopyright\nª\t\tfeminine ordinal indicator\n«\t\tangle quotation mark (left)\n¬\t\tnegation\n­\t\tsoft hyphen\n®\t\tregistered trademark\n¯\t\tspacing macron\n°\t\tdegree\n±\t\tplus-or-minus\n²\t\tsuperscript 2\n³\t\tsuperscript 3\n´\t\tspacing acute\n¶\t\tparagraph\n·\t\tmiddle dot\n¸\t\tspacing cedilla\n¹\t\tsuperscript 1\nº\t\tmasculine ordinal indicator\n»\t\tangle quotation mark (right)\n¼\t\tfraction 1/4\n½\t\tfraction 1/2\n¾\t\tfraction 3/4\n¿\t\tinverted question mark\nÀ\t\tcapital a, grave accent\nÁ\t\tcapital a, acute accent\nÂ\t\tcapital a, circumflex accent\nÃ\t\tcapital a, tilde\nÄ\t\tcapital a, umlaut mark\nÅ\t\tcapital a, ring\nÆ\t\tcapital ae\nÇ\t\tcapital c, cedilla\nÈ\t\tcapital e, grave accent\nÉ\t\tcapital e, acute accent\nÊ\t\tcapital e, circumflex accent\nË\t\tcapital e, umlaut mark\nÌ\t\tcapital i, grave accent\nÍ\t\tcapital i, acute accent\nÎ\t\tcapital i, circumflex accent\nÏ\t\tcapital i, umlaut mark\nÐ\t\tcapital eth, Icelandic\nÑ\t\tcapital n, tilde\nÒ\t\tcapital o, grave accent\nÓ\t\tcapital o, acute accent\nÔ\t\tcapital o, circumflex accent\nÕ\t\tcapital o, tilde\nÖ\t\tcapital o, umlaut mark\n×\t\tmultiplication\nØ\t\tcapital o, slash\nÙ\t\tcapital u, grave accent\nÚ\t\tcapital u, acute accent\nÛ\t\tcapital u, circumflex accent\nÜ\t\tcapital u, umlaut mark\nÝ\t\tcapital y, acute accent\nÞ\t\tcapital THORN, Icelandic\nß\t\tsmall sharp s, German\nà\t\tsmall a, grave accent\ná\t\tsmall a, acute accent\nâ\t\tsmall a, circumflex accent\nã\t\tsmall a, tilde\nä\t\tsmall a, umlaut mark\nå\t\tsmall a, ring\næ\t\tsmall ae\nç\t\tsmall c, cedilla\nè\t\tsmall e, grave accent\né\t\tsmall e, acute accent\nê\t\tsmall e, circumflex accent\në\t\tsmall e, umlaut mark\nì\t\tsmall i, grave accent\ní\t\tsmall i, acute accent\nî\t\tsmall i, circumflex accent\nï\t\tsmall i, umlaut mark\nð\t\tsmall eth, Icelandic\nñ\t\tsmall n, tilde\nò\t\tsmall o, grave accent\nó\t\tsmall o, acute accent\nô\t\tsmall o, circumflex accent\nõ\t\tsmall o, tilde\nö\t\tsmall o, umlaut mark\n÷\t\tdivision\nø\t\tsmall o, slash\nù\t\tsmall u, grave accent\nú\t\tsmall u, acute accent\nû\t\tsmall u, circumflex accent\nü\t\tsmall u, umlaut mark\ný\t\tsmall y, acute accent\nþ\t\tsmall thorn, Icelandic\nÿ\t\tsmall y, umlaut mark\n",

var loadedBinary1 = loadFileAsBase64('Assets/binary')
//@    "loadedBinary1": "g8fCjQHuFUfnHlTUHxe3qmjeM3HlYSToV7qTGrcJ6vgFNjvgpmxnexFbzjJV/Ejx8jXKa8L32YUn1f/HUnPY6u5c1SaGaP8OiVyRK4ef52hOtc3Yd29c9ubDsLohwLlmuiQDCvVduNMejR6eZy50ti3eYaLu3e04IKC7kTFO0Ph/vSIhlfkS6lUB9e7EJuHAa+3yJFn0uIVFs/BF67fNV9zwx92XyhFL8tmv3IZNd1+0cAby99+zif6iBPXcJ5XTUUz4UHaPmZLPT75hd4iGZzOk/I+FAsUTxRDra76D+sSXay7qBv5TyVLlhs7kqSVAecki6vQG0Siku64tl3PKKEy9JV1lHItgg/IFDYNd8/DKMUpEi90wunW/CfTpQcctzHzZFjl5euswaXgTDvVt2KRHPpi3likE8b1GuyKFVfKNT47VFGSabuUZlhDzbzx7qECnIpA1M7kH+TUHhGTe3ezmmPq+EO6jybYNMpMs+7gcfYAEtzE4gfpubKHLQI8ZYFKFxPCo0ypOwh4Z1nStJkcrNX2UlSDFfPb+LlCaGRxRKPN9md+2sr4x6qm0TptmI9o8wJF7FvqJUS2obFz2KhnfdKg7seuknpisasENchzEO2hoMtpCf5Lt3ZwsLCnFrllFaNmV2BWfA0k74f5MykjIioppM8ajdzORDtjTv/WcNyxdVlTV6nr2Oe43WFKZT0DFMDGHVgTZRBxVH5JtfT1akurR+IyvTegR6kSMHXSWlE1iEoPK8gdNpFLHdAJ8VwPnMGRC8CoCGzDeAakmwiHuecPOzcg9cOQe2Rlo68kJSr6q2hEcTmm9kPj7vfAeocqlosDd2Ci7xcBAJNb/rC4IVllhZPyqt2C8L1VbN5wZfBNgwpA73oOX0kxIKoCqH+Ni167WvC1ZwTqlVAWeAa1RiSplisinKBwDXahu2qDhVJyOt/RwV8Y+W3ynFGXYOW9BDwXVwKUm2zrKl6j0Y1AUTH5HArfCwwThXW2ZFslbr/fM9nJPAbbxpDY2FQBAlt8ST3PyLrFBfXlsV0JqVhidnw94NAbTiPqck15pTGbncg8VunYUAMw/JAOLf2SIJ8cA75IdJMp0UJXoMcOfVKkVdgMbGi7BHtJ3ZFwIajbNdWoNQV0k/LlwwmqGYGkh71wBX4WEdW6MqvvT8Mt/xyA6xzflqnMzgvQPIe6v29FETR9wxSKG52oCOY/DtPXEo+DgZvQwQn3F4BwX+GZawHbbMjWCIDxoSmph5TfZMzF0EkhEi47Uk93FPgiBQPjKSYMxnJ9Lo9UwZjsxdtk7ONKe1GIxfHdx3no4uuRp8WKqjpz017VBOWubdXPxO8Q8QOv6nT9faVVCMUQApehFlg==",
var loadedBinary2 = sys.loadFileAsBase64('Assets/binary')
//@    "loadedBinary2": "g8fCjQHuFUfnHlTUHxe3qmjeM3HlYSToV7qTGrcJ6vgFNjvgpmxnexFbzjJV/Ejx8jXKa8L32YUn1f/HUnPY6u5c1SaGaP8OiVyRK4ef52hOtc3Yd29c9ubDsLohwLlmuiQDCvVduNMejR6eZy50ti3eYaLu3e04IKC7kTFO0Ph/vSIhlfkS6lUB9e7EJuHAa+3yJFn0uIVFs/BF67fNV9zwx92XyhFL8tmv3IZNd1+0cAby99+zif6iBPXcJ5XTUUz4UHaPmZLPT75hd4iGZzOk/I+FAsUTxRDra76D+sSXay7qBv5TyVLlhs7kqSVAecki6vQG0Siku64tl3PKKEy9JV1lHItgg/IFDYNd8/DKMUpEi90wunW/CfTpQcctzHzZFjl5euswaXgTDvVt2KRHPpi3likE8b1GuyKFVfKNT47VFGSabuUZlhDzbzx7qECnIpA1M7kH+TUHhGTe3ezmmPq+EO6jybYNMpMs+7gcfYAEtzE4gfpubKHLQI8ZYFKFxPCo0ypOwh4Z1nStJkcrNX2UlSDFfPb+LlCaGRxRKPN9md+2sr4x6qm0TptmI9o8wJF7FvqJUS2obFz2KhnfdKg7seuknpisasENchzEO2hoMtpCf5Lt3ZwsLCnFrllFaNmV2BWfA0k74f5MykjIioppM8ajdzORDtjTv/WcNyxdVlTV6nr2Oe43WFKZT0DFMDGHVgTZRBxVH5JtfT1akurR+IyvTegR6kSMHXSWlE1iEoPK8gdNpFLHdAJ8VwPnMGRC8CoCGzDeAakmwiHuecPOzcg9cOQe2Rlo68kJSr6q2hEcTmm9kPj7vfAeocqlosDd2Ci7xcBAJNb/rC4IVllhZPyqt2C8L1VbN5wZfBNgwpA73oOX0kxIKoCqH+Ni167WvC1ZwTqlVAWeAa1RiSplisinKBwDXahu2qDhVJyOt/RwV8Y+W3ynFGXYOW9BDwXVwKUm2zrKl6j0Y1AUTH5HArfCwwThXW2ZFslbr/fM9nJPAbbxpDY2FQBAlt8ST3PyLrFBfXlsV0JqVhidnw94NAbTiPqck15pTGbncg8VunYUAMw/JAOLf2SIJ8cA75IdJMp0UJXoMcOfVKkVdgMbGi7BHtJ3ZFwIajbNdWoNQV0k/LlwwmqGYGkh71wBX4WEdW6MqvvT8Mt/xyA6xzflqnMzgvQPIe6v29FETR9wxSKG52oCOY/DtPXEo+DgZvQwQn3F4BwX+GZawHbbMjWCIDxoSmph5TfZMzF0EkhEi47Uk93FPgiBQPjKSYMxnJ9Lo9UwZjsxdtk7ONKe1GIxfHdx3no4uuRp8WKqjpz017VBOWubdXPxO8Q8QOv6nT9faVVCMUQApehFlg==",

var loadedTextInterpolation1 = 'Text: ${loadTextContent('Assets/TextFile.CRLF.txt')}'
//@    "$fxv#0": "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\r\n\tProin varius in nunc et laoreet.\r\n  Nam pulvinar ipsum sed lectus porttitor, at porttitor ipsum faucibus.\r\n  \tAliquam euismod, odio tincidunt convallis pulvinar, felis sem porttitor turpis, a condimentum dui erat nec tellus.\r\n  Duis elementum cursus est, congue efficitur risus.\r\n\tMauris sit amet.\r\nExcepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\r\n",
//@    "loadedTextInterpolation1": "[format('Text: {0}', variables('$fxv#0'))]",
var loadedTextInterpolation2 = 'Text: ${loadTextContent('Assets/TextFile.LF.txt')}'
//@    "$fxv#1": "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n Donec laoreet sem tortor, ut dignissim ipsum ornare vel.\n  Duis ac ipsum turpis.\n\tMaecenas at condimentum dui.\n Suspendisse aliquet efficitur iaculis.\nIn hac habitasse platea dictumst.\nEtiam consectetur ut libero ac lobortis.\n\tNullam vitae auctor massa.\nFusce tincidunt urna purus, sit amet.\n",
//@    "loadedTextInterpolation2": "[format('Text: {0}', variables('$fxv#1'))]",

var loadedTextObject1 = {
//@    "loadedTextObject1": {
//@    },
  'text' : loadTextContent('Assets/TextFile.CRLF.txt')
//@    "$fxv#2": "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\r\n\tProin varius in nunc et laoreet.\r\n  Nam pulvinar ipsum sed lectus porttitor, at porttitor ipsum faucibus.\r\n  \tAliquam euismod, odio tincidunt convallis pulvinar, felis sem porttitor turpis, a condimentum dui erat nec tellus.\r\n  Duis elementum cursus est, congue efficitur risus.\r\n\tMauris sit amet.\r\nExcepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\r\n",
//@      "text": "[variables('$fxv#2')]"
}
var loadedTextObject2 = {
//@    "loadedTextObject2": {
//@    },
  'text' : loadTextContent('Assets/TextFile.LF.txt')  
//@    "$fxv#3": "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n Donec laoreet sem tortor, ut dignissim ipsum ornare vel.\n  Duis ac ipsum turpis.\n\tMaecenas at condimentum dui.\n Suspendisse aliquet efficitur iaculis.\nIn hac habitasse platea dictumst.\nEtiam consectetur ut libero ac lobortis.\n\tNullam vitae auctor massa.\nFusce tincidunt urna purus, sit amet.\n",
//@      "text": "[variables('$fxv#3')]"
}
var loadedBinaryInObject = {
//@    "loadedBinaryInObject": {
//@    },
  file: loadFileAsBase64('Assets/binary')
//@    "$fxv#4": "g8fCjQHuFUfnHlTUHxe3qmjeM3HlYSToV7qTGrcJ6vgFNjvgpmxnexFbzjJV/Ejx8jXKa8L32YUn1f/HUnPY6u5c1SaGaP8OiVyRK4ef52hOtc3Yd29c9ubDsLohwLlmuiQDCvVduNMejR6eZy50ti3eYaLu3e04IKC7kTFO0Ph/vSIhlfkS6lUB9e7EJuHAa+3yJFn0uIVFs/BF67fNV9zwx92XyhFL8tmv3IZNd1+0cAby99+zif6iBPXcJ5XTUUz4UHaPmZLPT75hd4iGZzOk/I+FAsUTxRDra76D+sSXay7qBv5TyVLlhs7kqSVAecki6vQG0Siku64tl3PKKEy9JV1lHItgg/IFDYNd8/DKMUpEi90wunW/CfTpQcctzHzZFjl5euswaXgTDvVt2KRHPpi3likE8b1GuyKFVfKNT47VFGSabuUZlhDzbzx7qECnIpA1M7kH+TUHhGTe3ezmmPq+EO6jybYNMpMs+7gcfYAEtzE4gfpubKHLQI8ZYFKFxPCo0ypOwh4Z1nStJkcrNX2UlSDFfPb+LlCaGRxRKPN9md+2sr4x6qm0TptmI9o8wJF7FvqJUS2obFz2KhnfdKg7seuknpisasENchzEO2hoMtpCf5Lt3ZwsLCnFrllFaNmV2BWfA0k74f5MykjIioppM8ajdzORDtjTv/WcNyxdVlTV6nr2Oe43WFKZT0DFMDGHVgTZRBxVH5JtfT1akurR+IyvTegR6kSMHXSWlE1iEoPK8gdNpFLHdAJ8VwPnMGRC8CoCGzDeAakmwiHuecPOzcg9cOQe2Rlo68kJSr6q2hEcTmm9kPj7vfAeocqlosDd2Ci7xcBAJNb/rC4IVllhZPyqt2C8L1VbN5wZfBNgwpA73oOX0kxIKoCqH+Ni167WvC1ZwTqlVAWeAa1RiSplisinKBwDXahu2qDhVJyOt/RwV8Y+W3ynFGXYOW9BDwXVwKUm2zrKl6j0Y1AUTH5HArfCwwThXW2ZFslbr/fM9nJPAbbxpDY2FQBAlt8ST3PyLrFBfXlsV0JqVhidnw94NAbTiPqck15pTGbncg8VunYUAMw/JAOLf2SIJ8cA75IdJMp0UJXoMcOfVKkVdgMbGi7BHtJ3ZFwIajbNdWoNQV0k/LlwwmqGYGkh71wBX4WEdW6MqvvT8Mt/xyA6xzflqnMzgvQPIe6v29FETR9wxSKG52oCOY/DtPXEo+DgZvQwQn3F4BwX+GZawHbbMjWCIDxoSmph5TfZMzF0EkhEi47Uk93FPgiBQPjKSYMxnJ9Lo9UwZjsxdtk7ONKe1GIxfHdx3no4uuRp8WKqjpz017VBOWubdXPxO8Q8QOv6nT9faVVCMUQApehFlg==",
//@      "file": "[variables('$fxv#4')]"
}

var loadedTextArray = [
//@    "loadedTextArray": [
//@    ],
  loadTextContent('Assets/TextFile.LF.txt')
//@    "$fxv#5": "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n Donec laoreet sem tortor, ut dignissim ipsum ornare vel.\n  Duis ac ipsum turpis.\n\tMaecenas at condimentum dui.\n Suspendisse aliquet efficitur iaculis.\nIn hac habitasse platea dictumst.\nEtiam consectetur ut libero ac lobortis.\n\tNullam vitae auctor massa.\nFusce tincidunt urna purus, sit amet.\n",
//@      "[variables('$fxv#5')]",
  loadFileAsBase64('Assets/binary')
//@    "$fxv#6": "g8fCjQHuFUfnHlTUHxe3qmjeM3HlYSToV7qTGrcJ6vgFNjvgpmxnexFbzjJV/Ejx8jXKa8L32YUn1f/HUnPY6u5c1SaGaP8OiVyRK4ef52hOtc3Yd29c9ubDsLohwLlmuiQDCvVduNMejR6eZy50ti3eYaLu3e04IKC7kTFO0Ph/vSIhlfkS6lUB9e7EJuHAa+3yJFn0uIVFs/BF67fNV9zwx92XyhFL8tmv3IZNd1+0cAby99+zif6iBPXcJ5XTUUz4UHaPmZLPT75hd4iGZzOk/I+FAsUTxRDra76D+sSXay7qBv5TyVLlhs7kqSVAecki6vQG0Siku64tl3PKKEy9JV1lHItgg/IFDYNd8/DKMUpEi90wunW/CfTpQcctzHzZFjl5euswaXgTDvVt2KRHPpi3likE8b1GuyKFVfKNT47VFGSabuUZlhDzbzx7qECnIpA1M7kH+TUHhGTe3ezmmPq+EO6jybYNMpMs+7gcfYAEtzE4gfpubKHLQI8ZYFKFxPCo0ypOwh4Z1nStJkcrNX2UlSDFfPb+LlCaGRxRKPN9md+2sr4x6qm0TptmI9o8wJF7FvqJUS2obFz2KhnfdKg7seuknpisasENchzEO2hoMtpCf5Lt3ZwsLCnFrllFaNmV2BWfA0k74f5MykjIioppM8ajdzORDtjTv/WcNyxdVlTV6nr2Oe43WFKZT0DFMDGHVgTZRBxVH5JtfT1akurR+IyvTegR6kSMHXSWlE1iEoPK8gdNpFLHdAJ8VwPnMGRC8CoCGzDeAakmwiHuecPOzcg9cOQe2Rlo68kJSr6q2hEcTmm9kPj7vfAeocqlosDd2Ci7xcBAJNb/rC4IVllhZPyqt2C8L1VbN5wZfBNgwpA73oOX0kxIKoCqH+Ni167WvC1ZwTqlVAWeAa1RiSplisinKBwDXahu2qDhVJyOt/RwV8Y+W3ynFGXYOW9BDwXVwKUm2zrKl6j0Y1AUTH5HArfCwwThXW2ZFslbr/fM9nJPAbbxpDY2FQBAlt8ST3PyLrFBfXlsV0JqVhidnw94NAbTiPqck15pTGbncg8VunYUAMw/JAOLf2SIJ8cA75IdJMp0UJXoMcOfVKkVdgMbGi7BHtJ3ZFwIajbNdWoNQV0k/LlwwmqGYGkh71wBX4WEdW6MqvvT8Mt/xyA6xzflqnMzgvQPIe6v29FETR9wxSKG52oCOY/DtPXEo+DgZvQwQn3F4BwX+GZawHbbMjWCIDxoSmph5TfZMzF0EkhEi47Uk93FPgiBQPjKSYMxnJ9Lo9UwZjsxdtk7ONKe1GIxfHdx3no4uuRp8WKqjpz017VBOWubdXPxO8Q8QOv6nT9faVVCMUQApehFlg==",
//@      "[variables('$fxv#6')]"
]

var loadedTextArrayInObject = {
//@    "loadedTextArrayInObject": {
//@    },
  'files' : [
//@      "files": [
//@      ]
    loadTextContent('Assets/TextFile.CRLF.txt')
//@    "$fxv#7": "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\r\n\tProin varius in nunc et laoreet.\r\n  Nam pulvinar ipsum sed lectus porttitor, at porttitor ipsum faucibus.\r\n  \tAliquam euismod, odio tincidunt convallis pulvinar, felis sem porttitor turpis, a condimentum dui erat nec tellus.\r\n  Duis elementum cursus est, congue efficitur risus.\r\n\tMauris sit amet.\r\nExcepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\r\n",
//@        "[variables('$fxv#7')]",
    loadFileAsBase64('Assets/binary')
//@    "$fxv#8": "g8fCjQHuFUfnHlTUHxe3qmjeM3HlYSToV7qTGrcJ6vgFNjvgpmxnexFbzjJV/Ejx8jXKa8L32YUn1f/HUnPY6u5c1SaGaP8OiVyRK4ef52hOtc3Yd29c9ubDsLohwLlmuiQDCvVduNMejR6eZy50ti3eYaLu3e04IKC7kTFO0Ph/vSIhlfkS6lUB9e7EJuHAa+3yJFn0uIVFs/BF67fNV9zwx92XyhFL8tmv3IZNd1+0cAby99+zif6iBPXcJ5XTUUz4UHaPmZLPT75hd4iGZzOk/I+FAsUTxRDra76D+sSXay7qBv5TyVLlhs7kqSVAecki6vQG0Siku64tl3PKKEy9JV1lHItgg/IFDYNd8/DKMUpEi90wunW/CfTpQcctzHzZFjl5euswaXgTDvVt2KRHPpi3likE8b1GuyKFVfKNT47VFGSabuUZlhDzbzx7qECnIpA1M7kH+TUHhGTe3ezmmPq+EO6jybYNMpMs+7gcfYAEtzE4gfpubKHLQI8ZYFKFxPCo0ypOwh4Z1nStJkcrNX2UlSDFfPb+LlCaGRxRKPN9md+2sr4x6qm0TptmI9o8wJF7FvqJUS2obFz2KhnfdKg7seuknpisasENchzEO2hoMtpCf5Lt3ZwsLCnFrllFaNmV2BWfA0k74f5MykjIioppM8ajdzORDtjTv/WcNyxdVlTV6nr2Oe43WFKZT0DFMDGHVgTZRBxVH5JtfT1akurR+IyvTegR6kSMHXSWlE1iEoPK8gdNpFLHdAJ8VwPnMGRC8CoCGzDeAakmwiHuecPOzcg9cOQe2Rlo68kJSr6q2hEcTmm9kPj7vfAeocqlosDd2Ci7xcBAJNb/rC4IVllhZPyqt2C8L1VbN5wZfBNgwpA73oOX0kxIKoCqH+Ni167WvC1ZwTqlVAWeAa1RiSplisinKBwDXahu2qDhVJyOt/RwV8Y+W3ynFGXYOW9BDwXVwKUm2zrKl6j0Y1AUTH5HArfCwwThXW2ZFslbr/fM9nJPAbbxpDY2FQBAlt8ST3PyLrFBfXlsV0JqVhidnw94NAbTiPqck15pTGbncg8VunYUAMw/JAOLf2SIJ8cA75IdJMp0UJXoMcOfVKkVdgMbGi7BHtJ3ZFwIajbNdWoNQV0k/LlwwmqGYGkh71wBX4WEdW6MqvvT8Mt/xyA6xzflqnMzgvQPIe6v29FETR9wxSKG52oCOY/DtPXEo+DgZvQwQn3F4BwX+GZawHbbMjWCIDxoSmph5TfZMzF0EkhEi47Uk93FPgiBQPjKSYMxnJ9Lo9UwZjsxdtk7ONKe1GIxfHdx3no4uuRp8WKqjpz017VBOWubdXPxO8Q8QOv6nT9faVVCMUQApehFlg==",
//@        "[variables('$fxv#8')]"
  ]
}

var loadedTextArrayInObjectFunctions = {
//@    "loadedTextArrayInObjectFunctions": {
//@    },
  'files' : [
//@      "files": [
//@      ]
    length(loadTextContent('Assets/TextFile.CRLF.txt'))
//@    "$fxv#9": "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\r\n\tProin varius in nunc et laoreet.\r\n  Nam pulvinar ipsum sed lectus porttitor, at porttitor ipsum faucibus.\r\n  \tAliquam euismod, odio tincidunt convallis pulvinar, felis sem porttitor turpis, a condimentum dui erat nec tellus.\r\n  Duis elementum cursus est, congue efficitur risus.\r\n\tMauris sit amet.\r\nExcepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\r\n",
//@        "[length(variables('$fxv#9'))]",
    sys.length(loadTextContent('Assets/TextFile.LF.txt'))
//@    "$fxv#10": "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n Donec laoreet sem tortor, ut dignissim ipsum ornare vel.\n  Duis ac ipsum turpis.\n\tMaecenas at condimentum dui.\n Suspendisse aliquet efficitur iaculis.\nIn hac habitasse platea dictumst.\nEtiam consectetur ut libero ac lobortis.\n\tNullam vitae auctor massa.\nFusce tincidunt urna purus, sit amet.\n",
//@        "[length(variables('$fxv#10'))]",
    length(loadFileAsBase64('Assets/binary'))
//@    "$fxv#11": "g8fCjQHuFUfnHlTUHxe3qmjeM3HlYSToV7qTGrcJ6vgFNjvgpmxnexFbzjJV/Ejx8jXKa8L32YUn1f/HUnPY6u5c1SaGaP8OiVyRK4ef52hOtc3Yd29c9ubDsLohwLlmuiQDCvVduNMejR6eZy50ti3eYaLu3e04IKC7kTFO0Ph/vSIhlfkS6lUB9e7EJuHAa+3yJFn0uIVFs/BF67fNV9zwx92XyhFL8tmv3IZNd1+0cAby99+zif6iBPXcJ5XTUUz4UHaPmZLPT75hd4iGZzOk/I+FAsUTxRDra76D+sSXay7qBv5TyVLlhs7kqSVAecki6vQG0Siku64tl3PKKEy9JV1lHItgg/IFDYNd8/DKMUpEi90wunW/CfTpQcctzHzZFjl5euswaXgTDvVt2KRHPpi3likE8b1GuyKFVfKNT47VFGSabuUZlhDzbzx7qECnIpA1M7kH+TUHhGTe3ezmmPq+EO6jybYNMpMs+7gcfYAEtzE4gfpubKHLQI8ZYFKFxPCo0ypOwh4Z1nStJkcrNX2UlSDFfPb+LlCaGRxRKPN9md+2sr4x6qm0TptmI9o8wJF7FvqJUS2obFz2KhnfdKg7seuknpisasENchzEO2hoMtpCf5Lt3ZwsLCnFrllFaNmV2BWfA0k74f5MykjIioppM8ajdzORDtjTv/WcNyxdVlTV6nr2Oe43WFKZT0DFMDGHVgTZRBxVH5JtfT1akurR+IyvTegR6kSMHXSWlE1iEoPK8gdNpFLHdAJ8VwPnMGRC8CoCGzDeAakmwiHuecPOzcg9cOQe2Rlo68kJSr6q2hEcTmm9kPj7vfAeocqlosDd2Ci7xcBAJNb/rC4IVllhZPyqt2C8L1VbN5wZfBNgwpA73oOX0kxIKoCqH+Ni167WvC1ZwTqlVAWeAa1RiSplisinKBwDXahu2qDhVJyOt/RwV8Y+W3ynFGXYOW9BDwXVwKUm2zrKl6j0Y1AUTH5HArfCwwThXW2ZFslbr/fM9nJPAbbxpDY2FQBAlt8ST3PyLrFBfXlsV0JqVhidnw94NAbTiPqck15pTGbncg8VunYUAMw/JAOLf2SIJ8cA75IdJMp0UJXoMcOfVKkVdgMbGi7BHtJ3ZFwIajbNdWoNQV0k/LlwwmqGYGkh71wBX4WEdW6MqvvT8Mt/xyA6xzflqnMzgvQPIe6v29FETR9wxSKG52oCOY/DtPXEo+DgZvQwQn3F4BwX+GZawHbbMjWCIDxoSmph5TfZMzF0EkhEi47Uk93FPgiBQPjKSYMxnJ9Lo9UwZjsxdtk7ONKe1GIxfHdx3no4uuRp8WKqjpz017VBOWubdXPxO8Q8QOv6nT9faVVCMUQApehFlg==",
//@        "[length(variables('$fxv#11'))]",
    sys.length(loadFileAsBase64('Assets/binary'))
//@    "$fxv#12": "g8fCjQHuFUfnHlTUHxe3qmjeM3HlYSToV7qTGrcJ6vgFNjvgpmxnexFbzjJV/Ejx8jXKa8L32YUn1f/HUnPY6u5c1SaGaP8OiVyRK4ef52hOtc3Yd29c9ubDsLohwLlmuiQDCvVduNMejR6eZy50ti3eYaLu3e04IKC7kTFO0Ph/vSIhlfkS6lUB9e7EJuHAa+3yJFn0uIVFs/BF67fNV9zwx92XyhFL8tmv3IZNd1+0cAby99+zif6iBPXcJ5XTUUz4UHaPmZLPT75hd4iGZzOk/I+FAsUTxRDra76D+sSXay7qBv5TyVLlhs7kqSVAecki6vQG0Siku64tl3PKKEy9JV1lHItgg/IFDYNd8/DKMUpEi90wunW/CfTpQcctzHzZFjl5euswaXgTDvVt2KRHPpi3likE8b1GuyKFVfKNT47VFGSabuUZlhDzbzx7qECnIpA1M7kH+TUHhGTe3ezmmPq+EO6jybYNMpMs+7gcfYAEtzE4gfpubKHLQI8ZYFKFxPCo0ypOwh4Z1nStJkcrNX2UlSDFfPb+LlCaGRxRKPN9md+2sr4x6qm0TptmI9o8wJF7FvqJUS2obFz2KhnfdKg7seuknpisasENchzEO2hoMtpCf5Lt3ZwsLCnFrllFaNmV2BWfA0k74f5MykjIioppM8ajdzORDtjTv/WcNyxdVlTV6nr2Oe43WFKZT0DFMDGHVgTZRBxVH5JtfT1akurR+IyvTegR6kSMHXSWlE1iEoPK8gdNpFLHdAJ8VwPnMGRC8CoCGzDeAakmwiHuecPOzcg9cOQe2Rlo68kJSr6q2hEcTmm9kPj7vfAeocqlosDd2Ci7xcBAJNb/rC4IVllhZPyqt2C8L1VbN5wZfBNgwpA73oOX0kxIKoCqH+Ni167WvC1ZwTqlVAWeAa1RiSplisinKBwDXahu2qDhVJyOt/RwV8Y+W3ynFGXYOW9BDwXVwKUm2zrKl6j0Y1AUTH5HArfCwwThXW2ZFslbr/fM9nJPAbbxpDY2FQBAlt8ST3PyLrFBfXlsV0JqVhidnw94NAbTiPqck15pTGbncg8VunYUAMw/JAOLf2SIJ8cA75IdJMp0UJXoMcOfVKkVdgMbGi7BHtJ3ZFwIajbNdWoNQV0k/LlwwmqGYGkh71wBX4WEdW6MqvvT8Mt/xyA6xzflqnMzgvQPIe6v29FETR9wxSKG52oCOY/DtPXEo+DgZvQwQn3F4BwX+GZawHbbMjWCIDxoSmph5TfZMzF0EkhEi47Uk93FPgiBQPjKSYMxnJ9Lo9UwZjsxdtk7ONKe1GIxfHdx3no4uuRp8WKqjpz017VBOWubdXPxO8Q8QOv6nT9faVVCMUQApehFlg==",
//@        "[length(variables('$fxv#12'))]"
  ]
}


module module1 'modulea.bicep' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2022-09-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "8283448113925260975"
//@            }
//@          },
//@          "parameters": {
//@            "text": {
//@              "type": "string"
//@            }
//@          },
//@          "resources": []
//@        }
//@      }
//@    },
  name: 'module1'
//@      "name": "module1",
  params: {
//@        "parameters": {
//@        },
    text: loadTextContent('Assets/TextFile.LF.txt')
//@    "$fxv#13": "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n Donec laoreet sem tortor, ut dignissim ipsum ornare vel.\n  Duis ac ipsum turpis.\n\tMaecenas at condimentum dui.\n Suspendisse aliquet efficitur iaculis.\nIn hac habitasse platea dictumst.\nEtiam consectetur ut libero ac lobortis.\n\tNullam vitae auctor massa.\nFusce tincidunt urna purus, sit amet.\n",
//@          "text": {
//@            "value": "[variables('$fxv#13')]"
//@          }
  }
}

module module2 'modulea.bicep' = {
//@    {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2022-09-01",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "8283448113925260975"
//@            }
//@          },
//@          "parameters": {
//@            "text": {
//@              "type": "string"
//@            }
//@          },
//@          "resources": []
//@        }
//@      }
//@    }
  name: 'module2'
//@      "name": "module2",
  params: {
//@        "parameters": {
//@        },
    text: loadFileAsBase64('Assets/binary')
//@    "$fxv#14": "g8fCjQHuFUfnHlTUHxe3qmjeM3HlYSToV7qTGrcJ6vgFNjvgpmxnexFbzjJV/Ejx8jXKa8L32YUn1f/HUnPY6u5c1SaGaP8OiVyRK4ef52hOtc3Yd29c9ubDsLohwLlmuiQDCvVduNMejR6eZy50ti3eYaLu3e04IKC7kTFO0Ph/vSIhlfkS6lUB9e7EJuHAa+3yJFn0uIVFs/BF67fNV9zwx92XyhFL8tmv3IZNd1+0cAby99+zif6iBPXcJ5XTUUz4UHaPmZLPT75hd4iGZzOk/I+FAsUTxRDra76D+sSXay7qBv5TyVLlhs7kqSVAecki6vQG0Siku64tl3PKKEy9JV1lHItgg/IFDYNd8/DKMUpEi90wunW/CfTpQcctzHzZFjl5euswaXgTDvVt2KRHPpi3likE8b1GuyKFVfKNT47VFGSabuUZlhDzbzx7qECnIpA1M7kH+TUHhGTe3ezmmPq+EO6jybYNMpMs+7gcfYAEtzE4gfpubKHLQI8ZYFKFxPCo0ypOwh4Z1nStJkcrNX2UlSDFfPb+LlCaGRxRKPN9md+2sr4x6qm0TptmI9o8wJF7FvqJUS2obFz2KhnfdKg7seuknpisasENchzEO2hoMtpCf5Lt3ZwsLCnFrllFaNmV2BWfA0k74f5MykjIioppM8ajdzORDtjTv/WcNyxdVlTV6nr2Oe43WFKZT0DFMDGHVgTZRBxVH5JtfT1akurR+IyvTegR6kSMHXSWlE1iEoPK8gdNpFLHdAJ8VwPnMGRC8CoCGzDeAakmwiHuecPOzcg9cOQe2Rlo68kJSr6q2hEcTmm9kPj7vfAeocqlosDd2Ci7xcBAJNb/rC4IVllhZPyqt2C8L1VbN5wZfBNgwpA73oOX0kxIKoCqH+Ni167WvC1ZwTqlVAWeAa1RiSplisinKBwDXahu2qDhVJyOt/RwV8Y+W3ynFGXYOW9BDwXVwKUm2zrKl6j0Y1AUTH5HArfCwwThXW2ZFslbr/fM9nJPAbbxpDY2FQBAlt8ST3PyLrFBfXlsV0JqVhidnw94NAbTiPqck15pTGbncg8VunYUAMw/JAOLf2SIJ8cA75IdJMp0UJXoMcOfVKkVdgMbGi7BHtJ3ZFwIajbNdWoNQV0k/LlwwmqGYGkh71wBX4WEdW6MqvvT8Mt/xyA6xzflqnMzgvQPIe6v29FETR9wxSKG52oCOY/DtPXEo+DgZvQwQn3F4BwX+GZawHbbMjWCIDxoSmph5TfZMzF0EkhEi47Uk93FPgiBQPjKSYMxnJ9Lo9UwZjsxdtk7ONKe1GIxfHdx3no4uuRp8WKqjpz017VBOWubdXPxO8Q8QOv6nT9faVVCMUQApehFlg==",
//@          "text": {
//@            "value": "[variables('$fxv#14')]"
//@          }
  }
}

var textFileInSubdirectories = loadTextContent('Assets/../Assets/path/../path/../../Assets/path/to/deep/file/../../../to/deep/file/TextFile.txt')
//@    "textFileInSubdirectories": "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\n Donec laoreet sem tortor, ut dignissim ipsum ornare vel.\n  Duis ac ipsum turpis.\n\tMaecenas at condimentum dui.\n Suspendisse aliquet efficitur iaculis.\nIn hac habitasse platea dictumst.\nEtiam consectetur ut libero ac lobortis.\n\tNullam vitae auctor massa.\nFusce tincidunt urna purus, sit amet.\n",
var binaryFileInSubdirectories = loadFileAsBase64('Assets/../Assets/path/../path/../../Assets/path/to/deep/file/../../../to/deep/file/binary')
//@    "binaryFileInSubdirectories": "g8fCjQHuFUfnHlTUHxe3qmjeM3HlYSToV7qTGrcJ6vgFNjvgpmxnexFbzjJV/Ejx8jXKa8L32YUn1f/HUnPY6u5c1SaGaP8OiVyRK4ef52hOtc3Yd29c9ubDsLohwLlmuiQDCvVduNMejR6eZy50ti3eYaLu3e04IKC7kTFO0Ph/vSIhlfkS6lUB9e7EJuHAa+3yJFn0uIVFs/BF67fNV9zwx92XyhFL8tmv3IZNd1+0cAby99+zif6iBPXcJ5XTUUz4UHaPmZLPT75hd4iGZzOk/I+FAsUTxRDra76D+sSXay7qBv5TyVLlhs7kqSVAecki6vQG0Siku64tl3PKKEy9JV1lHItgg/IFDYNd8/DKMUpEi90wunW/CfTpQcctzHzZFjl5euswaXgTDvVt2KRHPpi3likE8b1GuyKFVfKNT47VFGSabuUZlhDzbzx7qECnIpA1M7kH+TUHhGTe3ezmmPq+EO6jybYNMpMs+7gcfYAEtzE4gfpubKHLQI8ZYFKFxPCo0ypOwh4Z1nStJkcrNX2UlSDFfPb+LlCaGRxRKPN9md+2sr4x6qm0TptmI9o8wJF7FvqJUS2obFz2KhnfdKg7seuknpisasENchzEO2hoMtpCf5Lt3ZwsLCnFrllFaNmV2BWfA0k74f5MykjIioppM8ajdzORDtjTv/WcNyxdVlTV6nr2Oe43WFKZT0DFMDGHVgTZRBxVH5JtfT1akurR+IyvTegR6kSMHXSWlE1iEoPK8gdNpFLHdAJ8VwPnMGRC8CoCGzDeAakmwiHuecPOzcg9cOQe2Rlo68kJSr6q2hEcTmm9kPj7vfAeocqlosDd2Ci7xcBAJNb/rC4IVllhZPyqt2C8L1VbN5wZfBNgwpA73oOX0kxIKoCqH+Ni167WvC1ZwTqlVAWeAa1RiSplisinKBwDXahu2qDhVJyOt/RwV8Y+W3ynFGXYOW9BDwXVwKUm2zrKl6j0Y1AUTH5HArfCwwThXW2ZFslbr/fM9nJPAbbxpDY2FQBAlt8ST3PyLrFBfXlsV0JqVhidnw94NAbTiPqck15pTGbncg8VunYUAMw/JAOLf2SIJ8cA75IdJMp0UJXoMcOfVKkVdgMbGi7BHtJ3ZFwIajbNdWoNQV0k/LlwwmqGYGkh71wBX4WEdW6MqvvT8Mt/xyA6xzflqnMzgvQPIe6v29FETR9wxSKG52oCOY/DtPXEo+DgZvQwQn3F4BwX+GZawHbbMjWCIDxoSmph5TfZMzF0EkhEi47Uk93FPgiBQPjKSYMxnJ9Lo9UwZjsxdtk7ONKe1GIxfHdx3no4uuRp8WKqjpz017VBOWubdXPxO8Q8QOv6nT9faVVCMUQApehFlg==",

var loadWithEncoding01 = loadTextContent('Assets/encoding-iso.txt', 'iso-8859-1')
//@    "loadWithEncoding01": "¡\t\tinverted exclamation mark\n¢\t\tcent\n£\t\tpound\n¤\t\tcurrency\n¥\t\tyen\n¦\t\tbroken vertical bar\n§\t\tsection\n¨\t\tspacing diaeresis\n©\t\tcopyright\nª\t\tfeminine ordinal indicator\n«\t\tangle quotation mark (left)\n¬\t\tnegation\n­\t\tsoft hyphen\n®\t\tregistered trademark\n¯\t\tspacing macron\n°\t\tdegree\n±\t\tplus-or-minus\n²\t\tsuperscript 2\n³\t\tsuperscript 3\n´\t\tspacing acute\n¶\t\tparagraph\n·\t\tmiddle dot\n¸\t\tspacing cedilla\n¹\t\tsuperscript 1\nº\t\tmasculine ordinal indicator\n»\t\tangle quotation mark (right)\n¼\t\tfraction 1/4\n½\t\tfraction 1/2\n¾\t\tfraction 3/4\n¿\t\tinverted question mark\nÀ\t\tcapital a, grave accent\nÁ\t\tcapital a, acute accent\nÂ\t\tcapital a, circumflex accent\nÃ\t\tcapital a, tilde\nÄ\t\tcapital a, umlaut mark\nÅ\t\tcapital a, ring\nÆ\t\tcapital ae\nÇ\t\tcapital c, cedilla\nÈ\t\tcapital e, grave accent\nÉ\t\tcapital e, acute accent\nÊ\t\tcapital e, circumflex accent\nË\t\tcapital e, umlaut mark\nÌ\t\tcapital i, grave accent\nÍ\t\tcapital i, acute accent\nÎ\t\tcapital i, circumflex accent\nÏ\t\tcapital i, umlaut mark\nÐ\t\tcapital eth, Icelandic\nÑ\t\tcapital n, tilde\nÒ\t\tcapital o, grave accent\nÓ\t\tcapital o, acute accent\nÔ\t\tcapital o, circumflex accent\nÕ\t\tcapital o, tilde\nÖ\t\tcapital o, umlaut mark\n×\t\tmultiplication\nØ\t\tcapital o, slash\nÙ\t\tcapital u, grave accent\nÚ\t\tcapital u, acute accent\nÛ\t\tcapital u, circumflex accent\nÜ\t\tcapital u, umlaut mark\nÝ\t\tcapital y, acute accent\nÞ\t\tcapital THORN, Icelandic\nß\t\tsmall sharp s, German\nà\t\tsmall a, grave accent\ná\t\tsmall a, acute accent\nâ\t\tsmall a, circumflex accent\nã\t\tsmall a, tilde\nä\t\tsmall a, umlaut mark\nå\t\tsmall a, ring\næ\t\tsmall ae\nç\t\tsmall c, cedilla\nè\t\tsmall e, grave accent\né\t\tsmall e, acute accent\nê\t\tsmall e, circumflex accent\në\t\tsmall e, umlaut mark\nì\t\tsmall i, grave accent\ní\t\tsmall i, acute accent\nî\t\tsmall i, circumflex accent\nï\t\tsmall i, umlaut mark\nð\t\tsmall eth, Icelandic\nñ\t\tsmall n, tilde\nò\t\tsmall o, grave accent\nó\t\tsmall o, acute accent\nô\t\tsmall o, circumflex accent\nõ\t\tsmall o, tilde\nö\t\tsmall o, umlaut mark\n÷\t\tdivision\nø\t\tsmall o, slash\nù\t\tsmall u, grave accent\nú\t\tsmall u, acute accent\nû\t\tsmall u, circumflex accent\nü\t\tsmall u, umlaut mark\ný\t\tsmall y, acute accent\nþ\t\tsmall thorn, Icelandic\nÿ\t\tsmall y, umlaut mark\n",
var loadWithEncoding06 = loadTextContent('Assets/encoding-ascii.txt', 'us-ascii')
//@    "loadWithEncoding06": "32 = \n33 = !\n34 = \"\n35 = #\n36 = $\n37 = %\n38 = &\n39 = '\n40 = (\n41 = )\n42 = *\n43 = +\n44 = ,\n45 = -\n46 = .\n47 = /\n48 = 0\n49 = 1\n50 = 2\n51 = 3\n52 = 4\n53 = 5\n54 = 6\n55 = 7\n56 = 8\n57 = 9\n58 = :\n59 = ;\n60 = <\n61 = =\n62 = >\n63 = ?\n64 = @\n65 = A\n66 = B\n67 = C\n68 = D\n69 = E\n70 = F\n71 = G\n72 = H\n73 = I\n74 = J\n75 = K\n76 = L\n77 = M\n78 = N\n79 = O\n80 = P\n81 = Q\n82 = R\n83 = S\n84 = T\n85 = U\n86 = V\n87 = W\n88 = X\n89 = Y\n90 = Z\n91 = [\n92 = \\\n93 = ]\n94 = ^\n95 = _\n96 = `\n97 = a\n98 = b\n99 = c\n100 = d\n101 = e\n102 = f\n103 = g\n104 = h\n105 = i\n106 = j\n107 = k\n108 = l\n109 = m\n110 = n\n111 = o\n112 = p\n113 = q\n114 = r\n115 = s\n116 = t\n117 = u\n118 = v\n119 = w\n120 = x\n121 = y\n122 = z\n123 = {\n124 = |\n125 = }\n126 = ~\n",
var loadWithEncoding07 = loadTextContent('Assets/encoding-ascii.txt', 'iso-8859-1')
//@    "loadWithEncoding07": "32 = \n33 = !\n34 = \"\n35 = #\n36 = $\n37 = %\n38 = &\n39 = '\n40 = (\n41 = )\n42 = *\n43 = +\n44 = ,\n45 = -\n46 = .\n47 = /\n48 = 0\n49 = 1\n50 = 2\n51 = 3\n52 = 4\n53 = 5\n54 = 6\n55 = 7\n56 = 8\n57 = 9\n58 = :\n59 = ;\n60 = <\n61 = =\n62 = >\n63 = ?\n64 = @\n65 = A\n66 = B\n67 = C\n68 = D\n69 = E\n70 = F\n71 = G\n72 = H\n73 = I\n74 = J\n75 = K\n76 = L\n77 = M\n78 = N\n79 = O\n80 = P\n81 = Q\n82 = R\n83 = S\n84 = T\n85 = U\n86 = V\n87 = W\n88 = X\n89 = Y\n90 = Z\n91 = [\n92 = \\\n93 = ]\n94 = ^\n95 = _\n96 = `\n97 = a\n98 = b\n99 = c\n100 = d\n101 = e\n102 = f\n103 = g\n104 = h\n105 = i\n106 = j\n107 = k\n108 = l\n109 = m\n110 = n\n111 = o\n112 = p\n113 = q\n114 = r\n115 = s\n116 = t\n117 = u\n118 = v\n119 = w\n120 = x\n121 = y\n122 = z\n123 = {\n124 = |\n125 = }\n126 = ~\n",
var loadWithEncoding08 = loadTextContent('Assets/encoding-ascii.txt', 'utf-8')
//@    "loadWithEncoding08": "32 = \n33 = !\n34 = \"\n35 = #\n36 = $\n37 = %\n38 = &\n39 = '\n40 = (\n41 = )\n42 = *\n43 = +\n44 = ,\n45 = -\n46 = .\n47 = /\n48 = 0\n49 = 1\n50 = 2\n51 = 3\n52 = 4\n53 = 5\n54 = 6\n55 = 7\n56 = 8\n57 = 9\n58 = :\n59 = ;\n60 = <\n61 = =\n62 = >\n63 = ?\n64 = @\n65 = A\n66 = B\n67 = C\n68 = D\n69 = E\n70 = F\n71 = G\n72 = H\n73 = I\n74 = J\n75 = K\n76 = L\n77 = M\n78 = N\n79 = O\n80 = P\n81 = Q\n82 = R\n83 = S\n84 = T\n85 = U\n86 = V\n87 = W\n88 = X\n89 = Y\n90 = Z\n91 = [\n92 = \\\n93 = ]\n94 = ^\n95 = _\n96 = `\n97 = a\n98 = b\n99 = c\n100 = d\n101 = e\n102 = f\n103 = g\n104 = h\n105 = i\n106 = j\n107 = k\n108 = l\n109 = m\n110 = n\n111 = o\n112 = p\n113 = q\n114 = r\n115 = s\n116 = t\n117 = u\n118 = v\n119 = w\n120 = x\n121 = y\n122 = z\n123 = {\n124 = |\n125 = }\n126 = ~\n",
var loadWithEncoding11 = loadTextContent('Assets/encoding-utf8.txt', 'utf-8')
//@    "loadWithEncoding11": "💪😊😈🍕☕\r\n🐱‍👤\r\n\r\n朝辞白帝彩云间\r\n千里江陵一日还\r\n两岸猿声啼不住\r\n轻舟已过万重山\r\n\r\nΠ π Φ φ\r\n\r\n😎\r\n\r\nαα\r\nΩω\r\nΘ  \r\n\r\nZażółć gęślą jaźń\r\n\r\náéóúñü - ¡Hola!\r\n\r\n二头肌二头肌\r\n\r\n\r\n二头肌\r\nΘ二头肌α\r\n\r\n𐐷\r\n\\u{10437}\r\n\\u{D801}\\u{DC37}\r\n\r\n❆ Hello\\u{20}World\\u{21} ❁\r\n\r\n\ta\tb\tc\td\te\tf\tg\th\t\r\n8\t♜\t♞\t♝\t♛\t♚\t♝\t♞\t♜\t8\r\n7\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t7\r\n6\t\t\t\t\t\t\t\t\t6\r\n5\t\t\t\t\t\t\t\t\t5\r\n4\t\t\t\t\t\t\t\t\t4\r\n3\t\t\t\t\t\t\t\t\t3\r\n2\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t2\r\n1\t♖\t♘\t♗\t♕\t♔\t♗\t♘\t♖\t1\r\n\ta\tb\tc\td\te\tf\tg\th\r\n",
var loadWithEncoding12 = loadTextContent('Assets/encoding-utf8-bom.txt', 'utf-8')
//@    "loadWithEncoding12": "💪😊😈🍕☕\r\n🐱‍👤\r\n\r\n朝辞白帝彩云间\r\n千里江陵一日还\r\n两岸猿声啼不住\r\n轻舟已过万重山\r\n\r\nΠ π Φ φ\r\n\r\n😎\r\n\r\nαα\r\nΩω\r\nΘ  \r\n\r\nZażółć gęślą jaźń\r\n\r\náéóúñü - ¡Hola!\r\n\r\n二头肌二头肌\r\n\r\n\r\n二头肌\r\nΘ二头肌α\r\n\r\n𐐷\r\n\\u{10437}\r\n\\u{D801}\\u{DC37}\r\n\r\n❆ Hello\\u{20}World\\u{21} ❁\r\n\r\n\ta\tb\tc\td\te\tf\tg\th\t\r\n8\t♜\t♞\t♝\t♛\t♚\t♝\t♞\t♜\t8\r\n7\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t♟\t7\r\n6\t\t\t\t\t\t\t\t\t6\r\n5\t\t\t\t\t\t\t\t\t5\r\n4\t\t\t\t\t\t\t\t\t4\r\n3\t\t\t\t\t\t\t\t\t3\r\n2\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t♙\t2\r\n1\t♖\t♘\t♗\t♕\t♔\t♗\t♘\t♖\t1\r\n\ta\tb\tc\td\te\tf\tg\th\r\n",

var testJson = json(loadTextContent('./Assets/test.json.txt'))
//@    "$fxv#15": "{\n    \"string\": \"someVal\",\n    \"int\": 123,\n    \"array\": [\n        1,\n        //comment\n        2\n/* multi\n    line\n    comment\n*/\n    ],\n/* multi\n    line\n    comment\n*/\n    \"object\": {\n        \"nestedString\": \"someVal\" //comment\n    }\n}\n",
//@    "testJson": "[json(variables('$fxv#15'))]",
var testJsonString = testJson.string
//@    "testJsonString": "[variables('testJson').string]",
var testJsonInt = testJson.int
//@    "testJsonInt": "[variables('testJson').int]",
var testJsonArrayVal = testJson.array[0]
//@    "testJsonArrayVal": "[variables('testJson').array[0]]",
var testJsonObject = testJson.object
//@    "testJsonObject": "[variables('testJson').object]",
var testJsonNestedString = testJson.object.nestedString
//@    "testJsonNestedString": "[variables('testJson').object.nestedString]",

var testJson2 = loadJsonContent('./Assets/test.json.txt')
//@    "testJson2": "[variables('$fxv#16')]",
var testJsonString2 = testJson.string
//@    "testJsonString2": "[variables('testJson').string]",
var testJsonString2_1 = loadJsonContent('./Assets/test.json.txt', '.string')
//@    "testJsonString2_1": "[variables('$fxv#17')]",
var testJsonInt2 = testJson.int
//@    "testJsonInt2": "[variables('testJson').int]",
var testJsonInt2_1 = loadJsonContent('./Assets/test.json.txt', '.int')
//@    "testJsonInt2_1": "[variables('$fxv#18')]",
var testJsonArrayVal2 = testJson.array[0]
//@    "testJsonArrayVal2": "[variables('testJson').array[0]]",
var testJsonArrayVal2_1 = loadJsonContent('./Assets/test.json.txt', '.array[0]')
//@    "testJsonArrayVal2_1": "[variables('$fxv#19')]",
var testJsonObject2 = testJson.object
//@    "testJsonObject2": "[variables('testJson').object]",
var testJsonObject2_1 = loadJsonContent('./Assets/test.json.txt', '.object')
//@    "testJsonObject2_1": "[variables('$fxv#20')]",
var testJsonNestedString2 = testJson.object.nestedString
//@    "testJsonNestedString2": "[variables('testJson').object.nestedString]",
var testJsonNestedString2_1 = testJsonObject2_1.nestedString
//@    "testJsonNestedString2_1": "[variables('testJsonObject2_1').nestedString]",
var testJsonNestedString2_2 = loadJsonContent('./Assets/test.json.txt', '.object.nestedString')
//@    "testJsonNestedString2_2": "[variables('$fxv#21')]",

var testJsonTokensAsArray = loadJsonContent('./Assets/test2.json.txt', '.products[?(@.price > 3)].name')
//@    "testJsonTokensAsArray": "[variables('$fxv#22')]",

var directoryInfo = loadDirectoryFileInfo('./Assets')
//@    "directoryInfo": "[variables('$fxv#23')]",
var directoryInfoWildcard = loadDirectoryFileInfo('./Assets', '*.txt')
//@    "directoryInfoWildcard": "[variables('$fxv#24')]",

var testYaml = loadYamlContent('./Assets/test.yaml.txt')
//@    "testYaml": "[variables('$fxv#25')]",
var testYamlString = testYaml.string
//@    "testYamlString": "[variables('testYaml').string]",
var testYamlInt = testYaml.int
//@    "testYamlInt": "[variables('testYaml').int]",
var testYamlBool = testYaml.bool
//@    "testYamlBool": "[variables('testYaml').bool]",
var testYamlArrayInt = testYaml.arrayInt
//@    "testYamlArrayInt": "[variables('testYaml').arrayInt]",
var testYamlArrayIntVal = testYaml.arrayInt[0]
//@    "testYamlArrayIntVal": "[variables('testYaml').arrayInt[0]]",
var testYamlArrayString = testYaml.arrayString
//@    "testYamlArrayString": "[variables('testYaml').arrayString]",
var testYamlArrayStringVal = testYaml.arrayString[0]
//@    "testYamlArrayStringVal": "[variables('testYaml').arrayString[0]]",
var testYamlArrayBool = testYaml.arrayBool
//@    "testYamlArrayBool": "[variables('testYaml').arrayBool]",
var testYamlArrayBoolVal = testYaml.arrayBool[0]
//@    "testYamlArrayBoolVal": "[variables('testYaml').arrayBool[0]]",
var testYamlObject = testYaml.object
//@    "testYamlObject": "[variables('testYaml').object]",
var testYamlObjectNestedString = testYaml.object.nestedString
//@    "testYamlObjectNestedString": "[variables('testYaml').object.nestedString]",
var testYamlObjectNestedInt = testYaml.object.nestedInt
//@    "testYamlObjectNestedInt": "[variables('testYaml').object.nestedInt]",
var testYamlObjectNestedBool = testYaml.object.nestedBool
//@    "testYamlObjectNestedBool": "[variables('testYaml').object.nestedBool]"

output testYamlString string = testYamlString
//@    "testYamlString": {
//@      "type": "string",
//@      "value": "[variables('testYamlString')]"
//@    },
output testYamlInt int = testYamlInt
//@    "testYamlInt": {
//@      "type": "int",
//@      "value": "[variables('testYamlInt')]"
//@    },
output testYamlBool bool = testYamlBool
//@    "testYamlBool": {
//@      "type": "bool",
//@      "value": "[variables('testYamlBool')]"
//@    },
output testYamlArrayInt array = testYamlArrayInt
//@    "testYamlArrayInt": {
//@      "type": "array",
//@      "value": "[variables('testYamlArrayInt')]"
//@    },
output testYamlArrayIntVal int = testYamlArrayIntVal
//@    "testYamlArrayIntVal": {
//@      "type": "int",
//@      "value": "[variables('testYamlArrayIntVal')]"
//@    },
output testYamlArrayString array = testYamlArrayString
//@    "testYamlArrayString": {
//@      "type": "array",
//@      "value": "[variables('testYamlArrayString')]"
//@    },
output testYamlArrayStringVal string = testYamlArrayStringVal
//@    "testYamlArrayStringVal": {
//@      "type": "string",
//@      "value": "[variables('testYamlArrayStringVal')]"
//@    },
output testYamlArrayBool array = testYamlArrayBool
//@    "testYamlArrayBool": {
//@      "type": "array",
//@      "value": "[variables('testYamlArrayBool')]"
//@    },
output testYamlArrayBoolVal bool = testYamlArrayBoolVal
//@    "testYamlArrayBoolVal": {
//@      "type": "bool",
//@      "value": "[variables('testYamlArrayBoolVal')]"
//@    },
output testYamlObject object = testYamlObject
//@    "testYamlObject": {
//@      "type": "object",
//@      "value": "[variables('testYamlObject')]"
//@    },
output testYamlObjectNestedString string = testYamlObjectNestedString
//@    "testYamlObjectNestedString": {
//@      "type": "string",
//@      "value": "[variables('testYamlObjectNestedString')]"
//@    },
output testYamlObjectNestedInt int = testYamlObjectNestedInt
//@    "testYamlObjectNestedInt": {
//@      "type": "int",
//@      "value": "[variables('testYamlObjectNestedInt')]"
//@    },
output testYamlObjectNestedBool bool = testYamlObjectNestedBool
//@    "testYamlObjectNestedBool": {
//@      "type": "bool",
//@      "value": "[variables('testYamlObjectNestedBool')]"
//@    }

