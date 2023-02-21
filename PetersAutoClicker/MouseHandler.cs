using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PetersAutoClicker
{
    public class MouseHandler
    {
        public MouseHandler() 
        { 

        }

        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy,int dwData, int dwExtraInfo);

        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }

        public void mouseLeftClick()
        {
            var cursor_x = Cursor.Position.X;
            var cursor_y = Cursor.Position.Y;
            mouse_event((int)MouseEventFlags.LEFTDOWN, cursor_x, cursor_y, 0, 0);
            mouse_event((int)MouseEventFlags.LEFTUP,cursor_x,cursor_y, 0, 0);
        }
    }
}
