# File 系列命令

[主页](/docs/home.md)

## file.write

写入文件

|参数|类型|描述|
|-|-|-|
| `path` |string|文件路径|
| `content` |string?|文件内容，如空则为仅创建文件（不写入任何内容）|

## file.delete

删除文件

|参数|类型|描述|
|-|-|-|
| `path` |string|文件路径|

## file.read

读取文件

|参数|类型|描述|
|-|-|-|
| `path` |string|文件路径|

|返回值类型|描述|
|-|-|
|string|文件的内容|

## file.exists

检测文件是否存在

|参数|类型|描述|
|-|-|-|
| `path` |string|文件路径|

|返回值类型|描述|
|-|-|
|bool|文件是否存在|

## file.copy

复制文件

|参数|类型|描述|
|-|-|-|
| `source` |string|原文件路径|
| `dest` |string|目标路径|

## file.move

移动文件。如果路径相同，名称不同，则为重命名

|参数|类型|描述|
|-|-|-|
| `source` |string|原文件路径|
| `dest` |string|目标路径|
