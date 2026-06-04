# 编译 Jscription 脚本为可执行文件

[主页](/docs/home.md)

## 准备工作

在开始编译之前，请确保您的开发环境满足以下条件：

1. **操作系统**：必须为 **Windows** 操作系统（编译功能目前仅支持 Windows 平台）。
2. **环境依赖**：电脑中已安装 **.NET SDK**（推荐安装 `.NET SDK 10` 或更高版本）。
3. **脚本文件**：准备好一份编写完毕的 Jscription 脚本文件，且该脚本已通过 `jscription run` 命令测试，可正常运行。

## 执行编译命令

打开终端（如 PowerShell 或 CMD），执行以下命令进行编译：

```bash
jscription compile <脚本文件路径>
```
