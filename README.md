# Jscription

**Jscription** 是一个基于 Json 的轻量级数据数据驱动脚本引擎

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
