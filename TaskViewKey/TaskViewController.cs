using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TaskViewKey
{
    internal class TaskViewController : IDisposable
    {
        public struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            int scanCode;
            public int flags;
            int time;
            int dwExtraInfo;
        }


        private delegate IntPtr HookHandlerDelegate(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);
        private HookHandlerDelegate Handler;

        private IntPtr hookId = IntPtr.Zero;
        private const int WH_KEYBOARD_ALL = 13;

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;

        private IntPtr HookCallback(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            if (nCode >= 0)
            {
                if (wParam == (IntPtr)WM_KEYDOWN)
                {
                    if ((lParam.vkCode == 91) || (lParam.vkCode == 92))
                    {
                        KeyboardActions.OpenTaskView();
                        return (IntPtr)1;
                    }
                }
            }

            return CallNextHookEx(hookId, nCode, wParam, ref lParam);
        }


        public TaskViewController()
        {
            Handler = new HookHandlerDelegate(HookCallback);
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                hookId = SetWindowsHookEx(WH_KEYBOARD_ALL, Handler, GetModuleHandle(curModule.ModuleName), 0);
            }

        }

        public void Dispose()
        {
            UnhookWindowsHookEx(hookId);
        }


        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(int idHook, HookHandlerDelegate lfpn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);
    }
}
