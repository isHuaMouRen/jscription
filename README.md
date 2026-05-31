# Jscription

**Jscription** 可以使你的Json可运行，并且编译为一个可执行程序

## 示例

就像这样编写一个Json

> source.json

```json
{
    "name": "Demo",
    "commands": [
        {
            "command": "console.print",
            "arguments": {
                "message": "Hello world!"
            }
        }
    ]
}
```

然后丢给 **Jscription** 来执行这个Json:

```bash
Jscription.Runner.exe --source source.json
```

就可以运行了！

```
Welcome to Jscription Runner <version>

Hello world!
```

如果你想更深入的学习，请查看 [Jscription 文档](/docs/home.md)
