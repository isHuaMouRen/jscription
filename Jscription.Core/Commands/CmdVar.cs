using Jscription.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jscription.Core.Commands
{
    public class CmdVar
    {
        public class Set : CmdRoot
        {
            public required string varName { get; set; }
            public required object value { get; set; }

            public override object? Run()
            {
                if (_context?.Variables == null)
                    throw new Exception($"命令 [{CommandName}] 运行时丢失了上下文变量字典。");

                object finalValue = value;

                if (value is string expr)
                {
                    finalValue = EvaluateExpression(expr);
                }

                _context?.Variables[varName] = finalValue;
                return null;
            }

            private object EvaluateExpression(string expr)
            {
                char[] operators = { '+', '-', '*', '/' };
                int opIndex = expr.IndexOfAny(operators);

                if (opIndex == -1) return expr;

                char op = expr[opIndex];

                string leftRaw = expr.Substring(0, opIndex).Trim();
                string rightRaw = expr.Substring(opIndex + 1).Trim();

                bool isLeftNum = double.TryParse(leftRaw, out double leftNum);
                bool isRightNum = double.TryParse(rightRaw, out double rightNum);

                if (isLeftNum && isRightNum)
                {
                    return op switch
                    {
                        '+' => leftNum + rightNum,
                        '-' => leftNum - rightNum,
                        '*' => leftNum * rightNum,
                        '/' => rightNum == 0 ? throw new DivideByZeroException("除数不能为 0。") : leftNum / rightNum,
                        _ => throw new InvalidOperationException($"未知的运算符: {op}")
                    };
                }

                if (op == '+')
                {
                    return leftRaw + rightRaw;
                }

                throw new InvalidOperationException($"无法对非数字文本进行 '{op}' 运算。表达式: {expr}");
            }
        }

        public class Get : CmdRoot
        {
            public required string varName { get; set; }

            public override object? Run()
            {
                if (_context?.Variables == null)
                    throw new Exception($"命令 [{CommandName}] 运行时丢失了上下文变量字典。");

                if (_context?.Variables.TryGetValue(varName, out var value) == true) 
                {
                    return value;
                }

                throw new JscriptionVariableNotFoundException(varName);
            }
        }
    }
}