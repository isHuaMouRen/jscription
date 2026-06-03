# Console 系列命令

[主页](/docs/home.md)

Console 系列命令用于控制控制台

## console.write

在控制台打印文字

|参数|类型|描述|
|-|-|-|
| `message` |string?|输出的内容|

## console.writeline

在控制台打印一行文字（即在输出内容后方添加换行符）

|参数|类型|描述|
|-|-|-|
| `message` |string?|输出的内容|

## console.readline

获得用户输入，在用户输入完后点击回车，返回用户输入内容

|返回值类型|描述|
|-|-|
| `string` |用户输入的内容|

## console.setcolor

设置控制台前景色

|参数|类型|描述|
|-|-|-|
| `color` |string|目标颜色，仅支持输入以下颜色。大小写不敏感|

|颜色|
|-|
|Black|
|DarkBlue|
|DarkGreen|
|DarkCyan|
|DarkRed|
|DarkMagenta|
|DarkYellow|
|Gray|
|DarkGray|
|Blue|
|Green|
|Cyan|
|Red|
|Magenta|
|Yellow|
|White|

## console.getcolor

获得当前前景色

|返回值类型|描述|
|-|-|
| `string` |当前颜色，返回的内容见 `console.setcolor` 的颜色表|
