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
| `waitForExit` |bool?|是否等待进程退出。如等待，则阻塞后方命令执行，直到进程退出；如不等待，则正常运行后方命令|

|返回值类型|描述|
|-|-|
|?|如程序有输出，则返回程序的输出内容；如无输出则返回程序退出码|
