using System.Windows.Forms;

namespace TaskViewKey
{
    internal static class KeyboardActions
    {
        public static void OpenTaskView()
        {
            KeyboardHelper.KeyDown(Keys.LWin);
            KeyboardHelper.KeyDown(Keys.Tab);
            KeyboardHelper.KeyUp(Keys.Tab);
            KeyboardHelper.KeyUp(Keys.LWin);
        }
    }
}
