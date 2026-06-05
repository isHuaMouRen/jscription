# Dialog 系列命令

[主页](/docs/home.md)

Dialog 系列命令用于控制对话框显示。各对话框都实现都为Windows(Win32API) 或Linux(zenity) 的原生调用

## dialog.messagebox

显示对话框

|参数|类型|描述|
|-|-|-|
| `title` |string?|对话框标题|
| `message` |string?|对话框内容|

|返回值类型|描述|
|-|-|
|bool|用户的选择|
