# Variable 系列命令

[主页](/docs/home.md)

## variable.set

设置变量的值

|参数|类型|描述|
|-|-|-|
| `varName` |string|变量名 (不可以带 `$$` ，必须纯变量名。比如 `var1` 而不是 `$var1$` !!)|
| `value` |object|目标值，可使用 `+`  `-`  `*`  `/` 字符进行运算，例如 `$var1$ + 1` |

## variable.get

获取变量的值（这根 `$varName$` 有啥区别？？）

|参数|类型|描述|
|-|-|-|
| `varName` |string|变量名 (不可以带 `$$` ，必须纯变量名。比如 `var1` 而不是 `$var1$` !!)|

|返回值类型|描述|
|-|-|
|object|目标变量的值|
