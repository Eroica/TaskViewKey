using System;

namespace TaskViewKey
{
    internal class Program
    {
        public static TaskViewController kh;

        [STAThread]
        static void Main()
        {
            WindowHelper.HideConsoleWindow();

            using (kh = new TaskViewController())
            {
                System.Windows.Forms.Application.Run();
            }
        }
    }
}
