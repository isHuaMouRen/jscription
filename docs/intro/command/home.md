# Jscription 命令

[主页](/docs/home.md)

## 简述

**命令** 是 Jscription 的基础构成，就像其他编程语言的代码一样

每个命令由以下结构构成

```json
{
    "<command-name>": {
        "<argument1>": "value1",
        "<argument2>": 1,
        "<argument3>": 2147483649,
        "<argument4>": 1.5,
        "<argument5>": true
    },
    "return": "<variable>"
}
```

* `<command-name>`: 命令名
* `<command-name>: {}`: 命令的参数
  + `<命令特定参数>`: 此部分由命令决定，需要查看命令详细来填写对应参数，值部分也根据命令决定，可为字符串、整数、长整数、浮点数、布尔值...
* `return`: 如果此命令有返回值，可通过此属性传出到变量

> [!note]
> **提示**: 命令名是不区分大小写的。因此你可以将 `console.writeline` 拼写为 `Console.WriteLine` ，甚至 `cOnsoLE.wRiTelIne` ，不过还是推荐全小写，最稳定，最美观

## 全部命令

* [Console 系列命令](/docs/intro/command/console.md)
* [Control 系列命令](/docs/intro/command/control.md)
* [Dir 系列命令](/docs/intro/command/dir.md)
* [File 系列命令](/docs/intro/command/file.md)
* [Process 系列命令](/docs/intro/command/process.md)
* [Variable 系列命令](/docs/intro/command/variable.md)

## 旧语法 (弃用)

在 `1.0.0-indev.5` 版本之前使用的语法，由于太过繁琐而被淘汰。旧语法的支持会在 `1.0.0` 正式版被彻底移除

```json
{
    "command": "<command-name>",
    "arguments": {
        "<argument1>": "value1",
        "<argument2>": 1,
        "<argument3>": 2147483649,
        "<argument4>": 1.5,
        "<argument5>": true
    },
    "return": "<variable>"
}
```
