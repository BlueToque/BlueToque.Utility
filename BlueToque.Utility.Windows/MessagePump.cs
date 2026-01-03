using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace BlueToque.Utility.Windows
{
    public static class MessagePump
    {
        public const int HWND_BROADCAST = 0xffff;
        public static readonly int WM_SHOWME = NativeMethods.RegisterWindowMessage("WM_SHOWME");
        public static readonly int WM_USER = 0x400;
        public static readonly int WM_COPYDATA = 0x4A;

        public static IntPtr SendWindowsStringMessage(IntPtr hWnd, int wParam, string msg)
        {
            if (hWnd == IntPtr.Zero)
                return IntPtr.Zero;

            var cds = new NativeMethods.COPYDATASTRUCT();
            byte[] buff = Encoding.Default.GetBytes(msg);
            //byte[] buff = Encoding.ASCII.GetBytes(msg);
            cds.dwData = (IntPtr)42;
            cds.lpData = Marshal.AllocHGlobal(buff.Length);
            Marshal.Copy(buff, 0, cds.lpData, buff.Length);
            cds.cbData = buff.Length;
            var ret = NativeMethods.SendMessage(hWnd, WM_COPYDATA, 0, ref cds);

            return ret;
        }

        public static string RecieveStringMessage(ref Message m)
        {
            NativeMethods.COPYDATASTRUCT cds = (NativeMethods.COPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.COPYDATASTRUCT))!;
            byte[] buff = new byte[cds.cbData];
            Marshal.Copy(cds.lpData, buff, 0, cds.cbData);
            string msg = Encoding.Default.GetString(buff, 0, cds.cbData);
            m.Result = (IntPtr)1234;
            return msg;
        }

    }
}