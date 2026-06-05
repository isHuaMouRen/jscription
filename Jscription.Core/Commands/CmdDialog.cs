using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Jscription.Core.Commands
{
    public class CmdDialog
    {
        public class MessageBox : CmdRoot
        {
            public string? title { get; set; }
            public string? message { get; set; }

            public override object? Run()
            {
                return NativeMessageBox.Show(title, message);
            }
        }












        private static class NativeMessageBox
        {
            //Windows API调用
            [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            private static extern int MessageBoxW(IntPtr hWnd, string lpText, string lpCaption, uint uType);
            private const uint MB_OKCANCEL = 0x00000001;
            private const uint MB_ICONINFORMATION = 0x00000040;
            private const int IDOK = 1;
            private const int IDYES = 6;

            public static bool Show(string? title, string? message)
            {
                if (OperatingSystem.IsOSPlatform("Windows"))
                {
                    int result = MessageBoxW(IntPtr.Zero, message ?? "", title ?? "", MB_OKCANCEL | MB_ICONINFORMATION);
                    return result == IDOK || result == IDYES;
                }
                else if (OperatingSystem.IsOSPlatform("Linux"))
                {
                    return ShowLinuxMessageBox(title, message);
                }
                else
                    throw new Exception("检测到未知的平台，原生对话框无法调用");
            }

            private static bool ShowLinuxMessageBox(string? title, string? message)
            {
                try
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = "zenity",
                        Arguments = $"--question --title=\"{title}\" --text=\"{message}\" --width=300",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using (var process = Process.Start(startInfo))
                    {
                        process?.WaitForExit();
                        return process?.ExitCode == 0;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"调用Linux原生对话框失败，此功能依赖\"zenity\"库，检查是否安装。详细信息:\n{ex}");
                }
            }
        }
    }
}
