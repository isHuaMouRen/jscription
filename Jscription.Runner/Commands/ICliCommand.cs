using System;
using System.Collections.Generic;
using System.Text;

namespace Jscription.Runner.Commands
{
    internal interface ICliCommand
    {
        string Name { get; }//命令名
        string Description { get; }//命令描述
        string Usage { get; }//用法
        int Execute(string[] args);//执行逻辑，返回退出码
    }
}
