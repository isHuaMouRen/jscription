# Jscription Runner 用法

[主页](/docs/home.md)

## 什么是 Jscription Runner?

`Jscription Runner` 是运行 Jscription 的载体， Jscription 本身不可以直接运行 Jscription 脚本。但其内部提供了对应的C#方法，你可以通过引用类库的方法来调用。为了平常的使用，便编写了 `Jscription Runner CLI` ，用户可直接通过命令行来与 Jscription 交互（运行脚本等...）

## 命令

(命令开头的 `jscription` 请替换为你的实际可执行文件名)

### Run

#### 调用

```bash
jscription run [参数]
```

#### 用法

* `run --source <文件路径>.json`: 立即执行指定的脚本

### Help

#### 调用

```bash
jscription help [参数]
```

#### 用法

* `help`: 显示所有命令及其简述
* `help <命令名>`: 显示某个命令的用法

### Version

#### 调用

```bash
jscription version
```

#### 用法

* `version`: 直接输出版本信息
