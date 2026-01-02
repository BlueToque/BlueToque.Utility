using System.Runtime.InteropServices;

namespace BlueToque.Utility.Windows
{
    public static partial class ConsoleShell
    {
        #region console

        /// <summary>
        /// Attach a console if it isn't already
        /// If we didn't run on the command line, open a console
        /// </summary>
        /// <param name="args"></param>
        public static void Attach()
        {
            if (AttachInternal()) return;
            AllocInternal();
        }

        static bool AllocInternal()
        {
            if (NativeMethods.AllocConsole()) return true;

            var error = Marshal.GetLastWin32Error();
            var exception = Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
            if (exception != null)
                Trace.TraceError("Error allocating console: {0}\r\n{1}", error, exception);
            else
                Trace.TraceError("Error allocating console: {0}", error);
            return false;
        }

        static bool AttachInternal()
        {
            if (NativeMethods.AttachConsole(NativeMethods.ATTACH_PARENT_PROCESS)) return true;

            var error = Marshal.GetLastWin32Error();
            var exception = Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
            if (exception == null)
                Trace.TraceError("Error attaching console: {0}", error);
            else
                Trace.TraceError("Error attaching console: {0}\r\n{1}", error, exception);
            return false;
        }


        #endregion

    }
}
