# Path 系列命令

[主页](/docs/home.md)

Path 系列命令用于路径操作

## path.combine

安全的路径拼接

因为Windows路径连接符( `\` )与Linux路径连接符( `/` )不同

所以在编写跨平台脚本时需要使用安全的路径拼接

|参数|类型|描述|
|-|-|-|
| `path1` |string?|路径1|
| `path2` |string?|路径2|
| `path3` |string?|路径3|
| `path4` |string?|路径4|

|返回值类型|描述|
|-|-|
|string|拼接完的路径|
