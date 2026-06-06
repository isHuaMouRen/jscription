# String 系列命令

[主页](/docs/home.md)

String 系列命令用于字符串操作

## string.replace

替换一段字符串内的某个字符或字符串

比如替换 `Hello world!` 内的所有 `o` 为 `e` 则结果为 `Helle werld!`

|参数|类型|描述|
|-|-|-|
| `origin` |string|原字符串|
| `oldChar` |string|旧字符|
| `newChar` |string|要替换为的新字符|

|返回值类型|描述|
|-|-|
|string|替换后的字符串|

## string.trim

去除某字符串的头尾空格，整理字符串

比如输入 `   Hello   ` 会被处理为 `Hello`

|参数|类型|描述|
|-|-|-|
| `origin` |string|原字符串|

|返回值类型|描述|
|-|-|
|string|整理完的字符串|

## string.contains

判断字符串是否包含某字符串

比如判断字符串 `Hello world!` 内是否包含 `Hello` 结果为true

|参数|类型|描述|
|-|-|-|
| `origin` |string|原字符串|
| `value` |string|目标字符串|

|返回值类型|描述|
|-|-|
|bool|原字符串是否包含目标字符串|

## string.length

获得字符串长度

比如获得字符串 `Hello` 的长度为5

|参数|类型|描述|
|-|-|-|
| `origin` |string|原字符串|

|返回值类型|描述|
|-|-|
|int|字符串长度|

## string.substring

获得指定字符串中的某一段字符串

比如获得字符串 `Hello world!` 中，从第二个字符开始，长度为6的一部分字符串为 `llo wo`

|参数|类型|描述|
|-|-|-|
| `origin` |string|原字符串|
| `startIndex` |int|开始截取的位置|
| `length` |int|截取长度|

|返回值类型|描述|
|-|-|
|string|截取后的字符串|
