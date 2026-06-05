# Process 系列命令

[主页](/docs/home.md)

Process 系列命令用于进程操作

## process.start

运行程序

|参数|类型|描述|
|-|-|-|
| `path` |string|要运行的程序的路径，也可以是URL|
| `args` |string?|参数|
| `useShell` |bool?|是否以系统Shell运行|
| `workingDir` |string?|工作目录|
| `createNoWindow` |bool?|无窗口创建进程|
