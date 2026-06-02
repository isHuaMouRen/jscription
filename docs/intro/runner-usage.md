# Jscription Runner 教程

[主页](https://www.google.com/search?q=/docs/home.md)

`Jscription Runner` 是用于执行 Jscription 的程序。由于 Jscription 核心库本身不提供直接的独立运行环境，而是通过 C# API 的形式提供核心功能，因此我们开发了 `Jscription Runner CLI` （命令行工具），方便用户直接在终端中与 Jscription 进行交互和运行脚本。

---

## 基础命令

> 💡 **提示**：下文示例中的 `jscription` 命令，请根据您实际的可执行文件名（如 `Jscription.Runner.win-x64.exe` 等 ）进行替换。

### 1. 运行脚本 ( `run` )

执行指定的 Jscription 脚本文件。

* **命令格式**

```bash
jscription run [参数]

```

* **参数说明**
* `run --source <文件路径>.json`：立即执行指定路径下的 JSON 脚本文件。

### 2. 获取帮助 ( `help` )

查看命令的帮助信息和使用说明。

* **命令格式**

```bash
jscription help [参数]

```

* **参数说明**
* `help`：显示所有可用命令及其简要概述。
* `help <命令名>`：显示指定命令的详细用法和参数说明。

### 3. 查看版本 ( `version` )

查看当前安装的工具版本信息。

* **命令格式**

```bash
jscription version

```

* **参数说明**
* `version`：直接在终端输出当前可执行文件的版本号。
