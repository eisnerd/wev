using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

// xev-like sample application
namespace wev
{
    // Focus event tracking
    class Focus
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        static void Main(string[] args)
        {
            Automation.AddAutomationFocusChangedEventHandler((o, e) =>
            {
                var s = new StringBuilder();
                try
                {
                    var info = (o as AutomationElement).Current;
                    s.Append("Focus received by ");
                    s.Append(info.ControlType.ProgrammaticName);
                    if (!string.IsNullOrWhiteSpace(info.ClassName))
                        s.Append(", ");
                    s.AppendLine(info.ClassName);

                    try
                    {
                        var w = new IntPtr(info.NativeWindowHandle);
                        var sb = new StringBuilder(GetWindowTextLength(w) + 1);
                        GetWindowText(w, sb, sb.Capacity);
                        s.Append("  Window Name: ");
                        s.AppendLine(sb.ToString());
                    }
                    catch { }

                    try
                    {
                        s.Append("  Process Id: "); s.Append(info.ProcessId); s.Append(", Name: ");
                        var p = Process.GetProcessById(info.ProcessId);
                        s.AppendLine(p.ProcessName);
                    }
                    catch { }
                }
                catch { }

                if (s.Length > 0)
                    Console.WriteLine(s.ToString());
            });

            while (true)
                try
                {
                    Thread.Sleep(System.Threading.Timeout.Infinite);
                }
                catch { }
        }
    }
}
