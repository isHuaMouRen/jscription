# Jscription 语法基础

[主页](/docs/home.md)

## 脚本结构简述

Jscription 采用声明式的 JSON 格式来编排业务逻辑。

### 基础示例

以下是一个最基础的 Jscription 脚本示例：

```json
{
    "name": "Demo",
    "commands": [
        {
            "console.writeline": {
                "message": "Hello world!"
            }
        }
    ]
}
```

* **`name`**: 脚本的唯一标识名称。
* **`commands`**: 命令执行队列。队列内部的命令严格按照**由上至下**的顺序串行执行。
* **`<命令名称>`**: 待执行的命令名称（例如 `console.writeline`）。
* **`<命令特定属性>`**: 由具体命令定义的强类型属性。例如 `console.writeline` 接收 `message` 属性，引擎在运行时会动态将该属性的值输出至控制台。

---

### 变量系统

通过在脚本根节点编写 `variables` 字段，可以定义全局变量并在后续命令中引用。

```json
{
    "name": "Demo",
    "variables": {
        "text": "Hello world!"
    },
    "commands": [
        {
            "console.writeline": {
                "message": "$text$"
            }
        }
    ]
}
```

#### 语法规则

* **变量定义**：在 `variables` 对象中以 `"键": "值"` 的形式声明。
* **变量引用**：使用 `$<变量名>$` 的语法来调用变量内容。

#### 引用场景

1. **纯变量输出**：如上例所示，`message` 的值为 `"$text$"`。解析器会识别出这不是普通字符串，而是变量引用，最终动态替换为 `Hello world!`。
2. **字符串插值**：变量可以嵌入普通字符串中。例如：
 `"The value of the 'text' variable is: $text$"`

最终将输出：
 `The value of the 'text' variable is: Hello world!`

---

### 返回值

并非所有命令都只执行无返回值的操作（如 `console.writeline` ）。某些命令（如文件读取 `file.read` ）会产生输出数据，此时需要将具有返回值的命令写到其他命令的参数中。以便使用返回的内容

```json
{
    "name": "Demo",
    "commands": [
        {
            "console.writeline": {
                "message": {
                    "file.read": {
                        "path": "C:\\test.txt"
                    }
                }
            }
        }
    ]
}
```

> **提示**：在 JSON 字符串中表示文件路径时，反斜杠 `\` 需要进行转义，写作 `\\` 。

#### 执行流程解析：

1. **执行message参数内的file.read** 读取位于 `C:\test.txt` 的文件，并返回
2. **捕获返回值**：`console.writeline` 的 `message` 参数会采用 `file.read` 返回的结果。
3. **执行console.writeline**: `console.writeline` 会将从 `file.read` 命令得到的文件内容打印到控制台内

#### 控制台预期输出：

```text
Welcome to Jscription Runner 1.0.0-Indev.2 (运行时版本信息)

This is an ordinary txt document. The file name is "test.txt".

```
