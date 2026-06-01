# Control 系列命令

[主页](/docs/home.md)

Control 系列命令用于命令的流程控制

## control.sleep

等待一段时间后再执行后方的代码

|参数|类型|描述|
|-|-|-|
| `time` |int|等待的时间(单位: ms)|

## control.if

条件判断语句

|参数|类型|描述|
|-|-|-|
| `condition` |object|条件，可使用 `==`  `!=`  `<=`  `>=`  `<`  `>` 来判断条件是否成立，也可以输入布尔值 |
| `then` |list<command>|条件成立执行的命令|
| `else` |list<command>?|条件不成立执行的命令|

这是一段简单的示例

```json
{
    "name": "Demo",
    "variables": {
        "test-var": 1
    },
    "commands": [
        {
            "command": "control.if",
            "arguments": {
                "condition": "$test-var$ == 1",
                "then": [
                    {
                        "command": "console.printline",
                        "arguments": {
                            "message": "test-var的值是1"
                        }
                    }
                ],
                "else": [
                    {
                        "command": "console.printline",
                        "arguments": {
                            "message": "test-var的值不是1"
                        }
                    }
                ]
            }
        }
    ]
}
```

## control.loop

条件判断循环，如符合条件就一直执行内部的命令，直到不符合条件时退出循环

|参数|类型|描述|
|-|-|-|
| `condition` |object|条件，可使用 `==`  `!=`  `<=`  `>=`  `<`  `>` 来判断条件是否成立，也可以输入布尔值 |
| `do` |list<command>|条件成立时一直执行的命令|
