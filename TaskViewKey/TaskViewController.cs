using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TaskViewKey
{
    internal class TaskViewController : IDisposable
    {

        #region windows.h imports
        public struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            int scanCode;
            public int flags;
            int time;
            int dwExtraInfo;
        }

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WH_KEYBOARD_LL = 13;

        private const int LWIN = 91;
        private const int RWIN = 92;
        #endregion

        // dummy value to trap the key
        private const int TRAP_KEY = 1;

        private delegate IntPtr HookHandlerDelegate(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);
        private HookHandlerDelegate handler;
        private IntPtr hookId = IntPtr.Zero;

        public TaskViewController()
        {
            handler = new HookHandlerDelegate(HookCallback);
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                hookId = SetWindowsHookEx(WH_KEYBOARD_LL, handler, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            if (nCode >= 0)
            {
                if ((lParam.vkCode == LWIN) || (lParam.vkCode == RWIN))
                {
                    if (wParam == (IntPtr)WM_KEYDOWN)
                    {
                        return (IntPtr)TRAP_KEY;
                    }

                    if (wParam == (IntPtr)WM_KEYUP)
                    {
                        KeyboardActions.OpenTaskView();
                        return (IntPtr)TRAP_KEY;
                    }
                }
            }

            return CallNextHookEx(hookId, nCode, wParam, ref lParam);
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
